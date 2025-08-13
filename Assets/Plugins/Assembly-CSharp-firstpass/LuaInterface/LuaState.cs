using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LuaInterface
{
	public class LuaState : IDisposable
	{
		public IntPtr L;

		internal LuaCSFunction tracebackFunction;

		internal LuaCSFunction panicCallback;

		internal LuaCSFunction printFunction;

		internal LuaCSFunction loadfileFunction;

		internal LuaCSFunction loaderFunction;

		internal LuaCSFunction dofileFunction;

		private readonly List<string> globals = new List<string>();

		private bool globalsSorted;

		public ObjectTranslator translator { get; internal set; }

		public object this[string fullPath]
		{
			get
			{
				object obj = null;
				int newTop = LuaDLL.lua_gettop(L);
				string[] array = fullPath.Split('.');
				LuaDLL.lua_getglobal(L, array[0]);
				obj = translator.getObject(L, -1);
				if (array.Length > 1)
				{
					string[] array2 = new string[array.Length - 1];
					Array.Copy(array, 1, array2, 0, array.Length - 1);
					obj = getObject(array2);
				}
				LuaDLL.lua_settop(L, newTop);
				return obj;
			}
			set
			{
				int newTop = LuaDLL.lua_gettop(L);
				string[] array = fullPath.Split('.');
				if (array.Length == 1)
				{
					translator.push(L, value);
					LuaDLL.lua_setglobal(L, fullPath);
				}
				else
				{
					LuaDLL.lua_getglobal(L, array[0]);
					string[] array2 = new string[array.Length - 1];
					Array.Copy(array, 1, array2, 0, array.Length - 1);
					setObject(array2, value);
				}
				LuaDLL.lua_settop(L, newTop);
				if (value == null)
				{
					globals.Remove(fullPath);
				}
				else if (!globals.Contains(fullPath))
				{
					registerGlobal(fullPath, value.GetType(), 0);
				}
			}
		}

		public IEnumerable<string> Globals
		{
			get
			{
				if (!globalsSorted)
				{
					globals.Sort();
					globalsSorted = true;
				}
				return globals;
			}
		}

		public LuaState()
		{
			L = LuaDLL.luaL_newstate();
			LuaDLL.luaL_openlibs(L);
			LuaDLL.lua_pushstring(L, "LUAINTERFACE LOADED");
			LuaDLL.lua_pushboolean(L, true);
			LuaDLL.lua_settable(L, LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_newtable(L);
			LuaDLL.lua_setglobal(L, "luanet");
			LuaDLL.lua_pushvalue(L, LuaIndexes.LUA_GLOBALSINDEX);
			LuaDLL.lua_getglobal(L, "luanet");
			LuaDLL.lua_pushstring(L, "getmetatable");
			LuaDLL.lua_getglobal(L, "getmetatable");
			LuaDLL.lua_settable(L, -3);
			LuaDLL.lua_replace(L, LuaIndexes.LUA_GLOBALSINDEX);
			translator = new ObjectTranslator(this, L);
			LuaDLL.lua_replace(L, LuaIndexes.LUA_GLOBALSINDEX);
			translator.PushTranslator(L);
			tracebackFunction = LuaStatic.traceback;
			panicCallback = LuaStatic.panic;
			LuaDLL.lua_atpanic(L, panicCallback);
			printFunction = LuaStatic.print;
			LuaDLL.lua_pushstdcallcfunction(L, printFunction);
			LuaDLL.lua_setfield(L, LuaIndexes.LUA_GLOBALSINDEX, "print");
			loadfileFunction = LuaStatic.loadfile;
			LuaDLL.lua_pushstdcallcfunction(L, loadfileFunction);
			LuaDLL.lua_setfield(L, LuaIndexes.LUA_GLOBALSINDEX, "loadfile");
			dofileFunction = LuaStatic.dofile;
			LuaDLL.lua_pushstdcallcfunction(L, dofileFunction);
			LuaDLL.lua_setfield(L, LuaIndexes.LUA_GLOBALSINDEX, "dofile");
			loaderFunction = LuaStatic.loader;
			LuaDLL.lua_pushstdcallcfunction(L, loaderFunction);
			int index = LuaDLL.lua_gettop(L);
			LuaDLL.lua_getfield(L, LuaIndexes.LUA_GLOBALSINDEX, "package");
			LuaDLL.lua_getfield(L, -1, "loaders");
			int num = LuaDLL.lua_gettop(L);
			for (int num2 = LuaDLL.luaL_getn(L, num) + 1; num2 > 1; num2--)
			{
				LuaDLL.lua_rawgeti(L, num, num2 - 1);
				LuaDLL.lua_rawseti(L, num, num2);
			}
			LuaDLL.lua_pushvalue(L, index);
			LuaDLL.lua_rawseti(L, num, 1);
			LuaDLL.lua_settop(L, 0);
			DoString(LuaStatic.init_luanet);
		}

		public void Close()
		{
			if (L != IntPtr.Zero)
			{
				LuaDLL.lua_close(L);
			}
		}

		internal void ThrowExceptionFromError(int oldTop)
		{
			object obj = translator.getObject(L, -1);
			LuaDLL.lua_settop(L, oldTop);
			LuaScriptException ex = obj as LuaScriptException;
			if (ex != null)
			{
				throw ex;
			}
			if (obj == null)
			{
				obj = "Unknown Lua Error";
			}
			throw new LuaScriptException(obj.ToString(), string.Empty);
		}

		internal int SetPendingException(Exception e)
		{
			if (e != null)
			{
				translator.throwError(L, e);
				LuaDLL.lua_pushnil(L);
				return 1;
			}
			return 0;
		}

		public LuaFunction LoadString(string chunk, string name, LuaTable env)
		{
			int oldTop = LuaDLL.lua_gettop(L);
			if (LuaDLL.luaL_loadbuffer(L, chunk, Encoding.UTF8.GetByteCount(chunk), name) != 0)
			{
				ThrowExceptionFromError(oldTop);
			}
			if (env != null)
			{
				env.push(L);
				LuaDLL.lua_setfenv(L, -2);
			}
			LuaFunction function = translator.getFunction(L, -1);
			translator.popValues(L, oldTop);
			return function;
		}

		public LuaFunction LoadString(string chunk, string name)
		{
			return LoadString(chunk, name, null);
		}

		public LuaFunction LoadFile(string fileName)
		{
			int oldTop = LuaDLL.lua_gettop(L);
			TextAsset textAsset = (TextAsset)Resources.Load(fileName);
			if (textAsset == null)
			{
				ThrowExceptionFromError(oldTop);
			}
			if (LuaDLL.luaL_loadbuffer(L, textAsset.text, Encoding.UTF8.GetByteCount(textAsset.text), fileName) != 0)
			{
				ThrowExceptionFromError(oldTop);
			}
			LuaFunction function = translator.getFunction(L, -1);
			translator.popValues(L, oldTop);
			return function;
		}

		public object[] DoString(string chunk)
		{
			return DoString(chunk, "chunk", null);
		}

		public object[] DoString(string chunk, string chunkName, LuaTable env)
		{
			int oldTop = LuaDLL.lua_gettop(L);
			if (LuaDLL.luaL_loadbuffer(L, chunk, Encoding.UTF8.GetByteCount(chunk), chunkName) == 0)
			{
				if (env != null)
				{
					env.push(L);
					LuaDLL.lua_setfenv(L, -2);
				}
				if (LuaDLL.lua_pcall(L, 0, -1, 0) == 0)
				{
					return translator.popValues(L, oldTop);
				}
				ThrowExceptionFromError(oldTop);
			}
			else
			{
				ThrowExceptionFromError(oldTop);
			}
			return null;
		}

		public object[] DoFile(string fileName)
		{
			return DoFile(fileName, null);
		}

		public object[] DoFile(string fileName, LuaTable env)
		{
			LuaDLL.lua_pushstdcallcfunction(L, tracebackFunction);
			int oldTop = LuaDLL.lua_gettop(L);
			TextAsset textAsset = (TextAsset)Resources.Load(fileName);
			if (textAsset == null)
			{
				ThrowExceptionFromError(oldTop);
			}
			if (LuaDLL.luaL_loadbuffer(L, textAsset.text, Encoding.UTF8.GetByteCount(textAsset.text), fileName) == 0)
			{
				if (env != null)
				{
					env.push(L);
					LuaDLL.lua_setfenv(L, -2);
				}
				if (LuaDLL.lua_pcall(L, 0, -1, -2) == 0)
				{
					object[] result = translator.popValues(L, oldTop);
					LuaDLL.lua_pop(L, 1);
					return result;
				}
				ThrowExceptionFromError(oldTop);
			}
			else
			{
				ThrowExceptionFromError(oldTop);
			}
			return null;
		}

		private void registerGlobal(string path, Type type, int recursionCounter)
		{
			if (type == typeof(LuaCSFunction))
			{
				globals.Add(path + "(");
			}
			else if ((type.IsClass || type.IsInterface) && type != typeof(string) && recursionCounter < 2)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.GetCustomAttributes(typeof(LuaHideAttribute), false).Length == 0 && methodInfo.GetCustomAttributes(typeof(LuaGlobalAttribute), false).Length == 0 && methodInfo.Name != "GetType" && methodInfo.Name != "GetHashCode" && methodInfo.Name != "Equals" && methodInfo.Name != "ToString" && methodInfo.Name != "Clone" && methodInfo.Name != "Dispose" && methodInfo.Name != "GetEnumerator" && methodInfo.Name != "CopyTo" && !methodInfo.Name.StartsWith("get_", StringComparison.Ordinal) && !methodInfo.Name.StartsWith("set_", StringComparison.Ordinal) && !methodInfo.Name.StartsWith("add_", StringComparison.Ordinal) && !methodInfo.Name.StartsWith("remove_", StringComparison.Ordinal))
					{
						string text = path + ":" + methodInfo.Name + "(";
						if (methodInfo.GetParameters().Length == 0)
						{
							text += ")";
						}
						globals.Add(text);
					}
				}
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.GetCustomAttributes(typeof(LuaHideAttribute), false).Length == 0 && fieldInfo.GetCustomAttributes(typeof(LuaGlobalAttribute), false).Length == 0)
					{
						registerGlobal(path + "." + fieldInfo.Name, fieldInfo.FieldType, recursionCounter + 1);
					}
				}
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.GetCustomAttributes(typeof(LuaHideAttribute), false).Length == 0 && propertyInfo.GetCustomAttributes(typeof(LuaGlobalAttribute), false).Length == 0 && propertyInfo.Name != "Item")
					{
						registerGlobal(path + "." + propertyInfo.Name, propertyInfo.PropertyType, recursionCounter + 1);
					}
				}
			}
			else
			{
				globals.Add(path);
			}
			globalsSorted = false;
		}

		internal object getObject(string[] remainingPath)
		{
			object obj = null;
			for (int i = 0; i < remainingPath.Length; i++)
			{
				LuaDLL.lua_pushstring(L, remainingPath[i]);
				LuaDLL.lua_gettable(L, -2);
				obj = translator.getObject(L, -1);
				if (obj == null)
				{
					break;
				}
			}
			return obj;
		}

		public double GetNumber(string fullPath)
		{
			return (double)this[fullPath];
		}

		public string GetString(string fullPath)
		{
			return (string)this[fullPath];
		}

		public LuaTable GetTable(string fullPath)
		{
			return (LuaTable)this[fullPath];
		}

		public object GetTable(Type interfaceType, string fullPath)
		{
			translator.throwError(L, "Tables as interfaces not implemnented");
			return CodeGeneration.Instance.GetClassInstance(interfaceType, GetTable(fullPath));
		}

		public LuaFunction GetFunction(string fullPath)
		{
			object obj = this[fullPath];
			return (!(obj is LuaCSFunction)) ? ((LuaFunction)obj) : new LuaFunction((LuaCSFunction)obj, this);
		}

		public Delegate GetFunction(Type delegateType, string fullPath)
		{
			return CodeGeneration.Instance.GetDelegate(delegateType, GetFunction(fullPath));
		}

		internal object[] callFunction(object function, object[] args)
		{
			return callFunction(function, args, null);
		}

		internal object[] callFunction(object function, object[] args, Type[] returnTypes)
		{
			int nArgs = 0;
			int oldTop = LuaDLL.lua_gettop(L);
			if (!LuaDLL.lua_checkstack(L, args.Length + 6))
			{
				throw new LuaException("Lua stack overflow");
			}
			translator.push(L, function);
			if (args != null)
			{
				nArgs = args.Length;
				for (int i = 0; i < args.Length; i++)
				{
					translator.push(L, args[i]);
				}
			}
			if (LuaDLL.lua_pcall(L, nArgs, -1, 0) != 0)
			{
				ThrowExceptionFromError(oldTop);
			}
			if (returnTypes != null)
			{
				return translator.popValues(L, oldTop, returnTypes);
			}
			return translator.popValues(L, oldTop);
		}

		internal void setObject(string[] remainingPath, object val)
		{
			for (int i = 0; i < remainingPath.Length - 1; i++)
			{
				LuaDLL.lua_pushstring(L, remainingPath[i]);
				LuaDLL.lua_gettable(L, -2);
			}
			LuaDLL.lua_pushstring(L, remainingPath[remainingPath.Length - 1]);
			translator.push(L, val);
			LuaDLL.lua_settable(L, -3);
		}

		public void NewTable(string fullPath)
		{
			string[] array = fullPath.Split('.');
			int newTop = LuaDLL.lua_gettop(L);
			if (array.Length == 1)
			{
				LuaDLL.lua_newtable(L);
				LuaDLL.lua_setglobal(L, fullPath);
			}
			else
			{
				LuaDLL.lua_getglobal(L, array[0]);
				for (int i = 1; i < array.Length - 1; i++)
				{
					LuaDLL.lua_pushstring(L, array[i]);
					LuaDLL.lua_gettable(L, -2);
				}
				LuaDLL.lua_pushstring(L, array[array.Length - 1]);
				LuaDLL.lua_newtable(L);
				LuaDLL.lua_settable(L, -3);
			}
			LuaDLL.lua_settop(L, newTop);
		}

		public LuaTable NewTable()
		{
			int newTop = LuaDLL.lua_gettop(L);
			LuaDLL.lua_newtable(L);
			LuaTable result = (LuaTable)translator.getObject(L, -1);
			LuaDLL.lua_settop(L, newTop);
			return result;
		}

		public ListDictionary GetTableDict(LuaTable table)
		{
			ListDictionary listDictionary = new ListDictionary();
			int newTop = LuaDLL.lua_gettop(L);
			translator.push(L, table);
			LuaDLL.lua_pushnil(L);
			while (LuaDLL.lua_next(L, -2) != 0)
			{
				listDictionary[translator.getObject(L, -2)] = translator.getObject(L, -1);
				LuaDLL.lua_settop(L, -2);
			}
			LuaDLL.lua_settop(L, newTop);
			return listDictionary;
		}

		internal void dispose(int reference)
		{
			if (L != IntPtr.Zero)
			{
				LuaDLL.lua_unref(L, reference);
			}
		}

		internal object rawGetObject(int reference, string field)
		{
			int newTop = LuaDLL.lua_gettop(L);
			LuaDLL.lua_getref(L, reference);
			LuaDLL.lua_pushstring(L, field);
			LuaDLL.lua_rawget(L, -2);
			object result = translator.getObject(L, -1);
			LuaDLL.lua_settop(L, newTop);
			return result;
		}

		internal object getObject(int reference, string field)
		{
			int newTop = LuaDLL.lua_gettop(L);
			LuaDLL.lua_getref(L, reference);
			object result = getObject(field.Split('.'));
			LuaDLL.lua_settop(L, newTop);
			return result;
		}

		internal object getObject(int reference, object field)
		{
			int newTop = LuaDLL.lua_gettop(L);
			LuaDLL.lua_getref(L, reference);
			translator.push(L, field);
			LuaDLL.lua_gettable(L, -2);
			object result = translator.getObject(L, -1);
			LuaDLL.lua_settop(L, newTop);
			return result;
		}

		internal void setObject(int reference, string field, object val)
		{
			int newTop = LuaDLL.lua_gettop(L);
			LuaDLL.lua_getref(L, reference);
			setObject(field.Split('.'), val);
			LuaDLL.lua_settop(L, newTop);
		}

		internal void setObject(int reference, object field, object val)
		{
			int newTop = LuaDLL.lua_gettop(L);
			LuaDLL.lua_getref(L, reference);
			translator.push(L, field);
			translator.push(L, val);
			LuaDLL.lua_settable(L, -3);
			LuaDLL.lua_settop(L, newTop);
		}

		public LuaFunction RegisterFunction(string path, object target, MethodBase function)
		{
			int newTop = LuaDLL.lua_gettop(L);
			LuaMethodWrapper luaMethodWrapper = new LuaMethodWrapper(translator, target, function.DeclaringType, function);
			translator.push(L, new LuaCSFunction(luaMethodWrapper.call));
			this[path] = translator.getObject(L, -1);
			LuaFunction function2 = GetFunction(path);
			LuaDLL.lua_settop(L, newTop);
			return function2;
		}

		public LuaFunction CreateFunction(object target, MethodBase function)
		{
			int newTop = LuaDLL.lua_gettop(L);
			LuaMethodWrapper luaMethodWrapper = new LuaMethodWrapper(translator, target, function.DeclaringType, function);
			translator.push(L, new LuaCSFunction(luaMethodWrapper.call));
			object obj = translator.getObject(L, -1);
			LuaFunction result = ((!(obj is LuaCSFunction)) ? ((LuaFunction)obj) : new LuaFunction((LuaCSFunction)obj, this));
			LuaDLL.lua_settop(L, newTop);
			return result;
		}

		internal bool compareRef(int ref1, int ref2)
		{
			int newTop = LuaDLL.lua_gettop(L);
			LuaDLL.lua_getref(L, ref1);
			LuaDLL.lua_getref(L, ref2);
			int num = LuaDLL.lua_equal(L, -1, -2);
			LuaDLL.lua_settop(L, newTop);
			return num != 0;
		}

		internal void pushCSFunction(LuaCSFunction function)
		{
			translator.pushFunction(L, function);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public virtual void Dispose(bool dispose)
		{
			if (dispose && translator != null)
			{
				translator.pendingEvents.Dispose();
				translator = null;
			}
		}
	}
}
