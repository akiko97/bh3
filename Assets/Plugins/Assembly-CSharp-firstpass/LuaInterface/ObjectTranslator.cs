using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LuaInterface
{
	public class ObjectTranslator
	{
		internal CheckType typeChecker;

		public readonly Dictionary<int, object> objects = new Dictionary<int, object>();

		public readonly Dictionary<object, int> objectsBackMap = new Dictionary<object, int>();

		internal LuaState interpreter;

		public MetaFunctions metaFunctions;

		public List<Assembly> assemblies;

		private LuaCSFunction registerTableFunction;

		private LuaCSFunction unregisterTableFunction;

		private LuaCSFunction getMethodSigFunction;

		private LuaCSFunction getConstructorSigFunction;

		private LuaCSFunction importTypeFunction;

		private LuaCSFunction loadAssemblyFunction;

		private LuaCSFunction ctypeFunction;

		private LuaCSFunction enumFromIntFunction;

		internal EventHandlerContainer pendingEvents = new EventHandlerContainer();

		private static int indexTranslator = 0;

		private static List<ObjectTranslator> list = new List<ObjectTranslator>();

		private int nextObj;

		public ObjectTranslator(LuaState interpreter, IntPtr luaState)
		{
			this.interpreter = interpreter;
			typeChecker = new CheckType(this);
			metaFunctions = new MetaFunctions(this);
			assemblies = new List<Assembly>();
			assemblies.Add(Assembly.GetExecutingAssembly());
			importTypeFunction = importType;
			loadAssemblyFunction = loadAssembly;
			registerTableFunction = registerTable;
			unregisterTableFunction = unregisterTable;
			getMethodSigFunction = getMethodSignature;
			getConstructorSigFunction = getConstructorSignature;
			ctypeFunction = ctype;
			enumFromIntFunction = enumFromInt;
			createLuaObjectList(luaState);
			createIndexingMetaFunction(luaState);
			createBaseClassMetatable(luaState);
			createClassMetatable(luaState);
			createFunctionMetatable(luaState);
			setGlobalFunctions(luaState);
		}

		public static ObjectTranslator FromState(IntPtr luaState)
		{
			LuaDLL.lua_getglobal(luaState, "_translator");
			int index = (int)LuaDLL.lua_tonumber(luaState, -1);
			LuaDLL.lua_pop(luaState, 1);
			return list[index];
		}

		public void PushTranslator(IntPtr L)
		{
			list.Add(this);
			LuaDLL.lua_pushnumber(L, indexTranslator);
			LuaDLL.lua_setglobal(L, "_translator");
			indexTranslator++;
		}

		private void createLuaObjectList(IntPtr luaState)
		{
			LuaDLL.lua_pushstring(luaState, "luaNet_objects");
			LuaDLL.lua_newtable(luaState);
			LuaDLL.lua_newtable(luaState);
			LuaDLL.lua_pushstring(luaState, "__mode");
			LuaDLL.lua_pushstring(luaState, "v");
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_setmetatable(luaState, -2);
			LuaDLL.lua_settable(luaState, LuaIndexes.LUA_REGISTRYINDEX);
		}

		private void createIndexingMetaFunction(IntPtr luaState)
		{
			LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
			LuaDLL.luaL_dostring(luaState, MetaFunctions.luaIndexFunction);
			LuaDLL.lua_rawset(luaState, LuaIndexes.LUA_REGISTRYINDEX);
		}

		private void createBaseClassMetatable(IntPtr luaState)
		{
			LuaDLL.luaL_newmetatable(luaState, "luaNet_searchbase");
			LuaDLL.lua_pushstring(luaState, "__gc");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.gcFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__tostring");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.toStringFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__index");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.baseIndexFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__newindex");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.newindexFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_settop(luaState, -2);
		}

		private void createClassMetatable(IntPtr luaState)
		{
			LuaDLL.luaL_newmetatable(luaState, "luaNet_class");
			LuaDLL.lua_pushstring(luaState, "__gc");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.gcFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__tostring");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.toStringFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__index");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.classIndexFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__newindex");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.classNewindexFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__call");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.callConstructorFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_settop(luaState, -2);
		}

		private void setGlobalFunctions(IntPtr luaState)
		{
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.indexFunction);
			LuaDLL.lua_setglobal(luaState, "get_object_member");
			LuaDLL.lua_pushstdcallcfunction(luaState, importTypeFunction);
			LuaDLL.lua_setglobal(luaState, "import_type");
			LuaDLL.lua_pushstdcallcfunction(luaState, loadAssemblyFunction);
			LuaDLL.lua_setglobal(luaState, "load_assembly");
			LuaDLL.lua_pushstdcallcfunction(luaState, registerTableFunction);
			LuaDLL.lua_setglobal(luaState, "make_object");
			LuaDLL.lua_pushstdcallcfunction(luaState, unregisterTableFunction);
			LuaDLL.lua_setglobal(luaState, "free_object");
			LuaDLL.lua_pushstdcallcfunction(luaState, getMethodSigFunction);
			LuaDLL.lua_setglobal(luaState, "get_method_bysig");
			LuaDLL.lua_pushstdcallcfunction(luaState, getConstructorSigFunction);
			LuaDLL.lua_setglobal(luaState, "get_constructor_bysig");
			LuaDLL.lua_pushstdcallcfunction(luaState, ctypeFunction);
			LuaDLL.lua_setglobal(luaState, "ctype");
			LuaDLL.lua_pushstdcallcfunction(luaState, enumFromIntFunction);
			LuaDLL.lua_setglobal(luaState, "enum");
		}

		private void createFunctionMetatable(IntPtr luaState)
		{
			LuaDLL.luaL_newmetatable(luaState, "luaNet_function");
			LuaDLL.lua_pushstring(luaState, "__gc");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.gcFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__call");
			LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.execDelegateFunction);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_settop(luaState, -2);
		}

		internal void throwError(IntPtr luaState, object e)
		{
			int oldTop = LuaDLL.lua_gettop(luaState);
			LuaDLL.luaL_where(luaState, 1);
			object[] array = popValues(luaState, oldTop);
			string source = string.Empty;
			if (array.Length > 0)
			{
				source = array[0].ToString();
			}
			string text = e as string;
			if (text != null)
			{
				e = new LuaScriptException(text, source);
			}
			else
			{
				Exception ex = e as Exception;
				if (ex != null)
				{
					e = new LuaScriptException(ex, source);
				}
			}
			push(luaState, e);
			LuaDLL.lua_error(luaState);
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int loadAssembly(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = FromState(luaState);
			try
			{
				string text = LuaDLL.lua_tostring(luaState, 1);
				Assembly assembly = null;
				try
				{
					assembly = Assembly.Load(text);
				}
				catch (BadImageFormatException)
				{
				}
				if (assembly == null)
				{
					assembly = Assembly.Load(AssemblyName.GetAssemblyName(text));
				}
				if (assembly != null && !objectTranslator.assemblies.Contains(assembly))
				{
					objectTranslator.assemblies.Add(assembly);
				}
			}
			catch (Exception e)
			{
				objectTranslator.throwError(luaState, e);
			}
			return 0;
		}

		internal Type FindType(string className)
		{
			foreach (Assembly assembly in assemblies)
			{
				Type type = assembly.GetType(className);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int importType(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = FromState(luaState);
			string className = LuaDLL.lua_tostring(luaState, 1);
			Type type = objectTranslator.FindType(className);
			if (type != null)
			{
				objectTranslator.pushType(luaState, type);
			}
			else
			{
				LuaDLL.lua_pushnil(luaState);
			}
			return 1;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int registerTable(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = FromState(luaState);
			if (LuaDLL.lua_type(luaState, 1) == LuaTypes.LUA_TTABLE)
			{
				LuaTable table = objectTranslator.getTable(luaState, 1);
				string text = LuaDLL.lua_tostring(luaState, 2);
				if (text != null)
				{
					Type type = objectTranslator.FindType(text);
					if (type != null)
					{
						object classInstance = CodeGeneration.Instance.GetClassInstance(type, table);
						objectTranslator.pushObject(luaState, classInstance, "luaNet_metatable");
						LuaDLL.lua_newtable(luaState);
						LuaDLL.lua_pushstring(luaState, "__index");
						LuaDLL.lua_pushvalue(luaState, -3);
						LuaDLL.lua_settable(luaState, -3);
						LuaDLL.lua_pushstring(luaState, "__newindex");
						LuaDLL.lua_pushvalue(luaState, -3);
						LuaDLL.lua_settable(luaState, -3);
						LuaDLL.lua_setmetatable(luaState, 1);
						LuaDLL.lua_pushstring(luaState, "base");
						int index = objectTranslator.addObject(classInstance);
						objectTranslator.pushNewObject(luaState, classInstance, index, "luaNet_searchbase");
						LuaDLL.lua_rawset(luaState, 1);
					}
					else
					{
						objectTranslator.throwError(luaState, "register_table: can not find superclass '" + text + "'");
					}
				}
				else
				{
					objectTranslator.throwError(luaState, "register_table: superclass name can not be null");
				}
			}
			else
			{
				objectTranslator.throwError(luaState, "register_table: first arg is not a table");
			}
			return 0;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int unregisterTable(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = FromState(luaState);
			try
			{
				if (LuaDLL.lua_getmetatable(luaState, 1) != 0)
				{
					LuaDLL.lua_pushstring(luaState, "__index");
					LuaDLL.lua_gettable(luaState, -2);
					object rawNetObject = objectTranslator.getRawNetObject(luaState, -1);
					if (rawNetObject == null)
					{
						objectTranslator.throwError(luaState, "unregister_table: arg is not valid table");
					}
					FieldInfo field = rawNetObject.GetType().GetField("__luaInterface_luaTable");
					if (field == null)
					{
						objectTranslator.throwError(luaState, "unregister_table: arg is not valid table");
					}
					field.SetValue(rawNetObject, null);
					LuaDLL.lua_pushnil(luaState);
					LuaDLL.lua_setmetatable(luaState, 1);
					LuaDLL.lua_pushstring(luaState, "base");
					LuaDLL.lua_pushnil(luaState);
					LuaDLL.lua_settable(luaState, 1);
				}
				else
				{
					objectTranslator.throwError(luaState, "unregister_table: arg is not valid table");
				}
			}
			catch (Exception ex)
			{
				objectTranslator.throwError(luaState, ex.Message);
			}
			return 0;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int getMethodSignature(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = FromState(luaState);
			int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
			IReflect reflect;
			object obj;
			if (num != -1)
			{
				reflect = (IReflect)objectTranslator.objects[num];
				obj = null;
			}
			else
			{
				obj = objectTranslator.getRawNetObject(luaState, 1);
				if (obj == null)
				{
					objectTranslator.throwError(luaState, "get_method_bysig: first arg is not type or object reference");
					LuaDLL.lua_pushnil(luaState);
					return 1;
				}
				reflect = obj.GetType();
			}
			string name = LuaDLL.lua_tostring(luaState, 2);
			Type[] array = new Type[LuaDLL.lua_gettop(luaState) - 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = objectTranslator.FindType(LuaDLL.lua_tostring(luaState, i + 3));
			}
			try
			{
				MethodInfo method = reflect.GetMethod(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, array, null);
				objectTranslator.pushFunction(luaState, new LuaMethodWrapper(objectTranslator, obj, reflect, method).call);
			}
			catch (Exception e)
			{
				objectTranslator.throwError(luaState, e);
				LuaDLL.lua_pushnil(luaState);
			}
			return 1;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int getConstructorSignature(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = FromState(luaState);
			IReflect reflect = null;
			int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
			if (num != -1)
			{
				reflect = (IReflect)objectTranslator.objects[num];
			}
			if (reflect == null)
			{
				objectTranslator.throwError(luaState, "get_constructor_bysig: first arg is invalid type reference");
			}
			Type[] array = new Type[LuaDLL.lua_gettop(luaState) - 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = objectTranslator.FindType(LuaDLL.lua_tostring(luaState, i + 2));
			}
			try
			{
				ConstructorInfo constructor = reflect.UnderlyingSystemType.GetConstructor(array);
				objectTranslator.pushFunction(luaState, new LuaMethodWrapper(objectTranslator, null, reflect, constructor).call);
			}
			catch (Exception e)
			{
				objectTranslator.throwError(luaState, e);
				LuaDLL.lua_pushnil(luaState);
			}
			return 1;
		}

		private Type typeOf(IntPtr luaState, int idx)
		{
			int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
			if (num == -1)
			{
				return null;
			}
			ProxyType proxyType = (ProxyType)objects[num];
			return proxyType.UnderlyingSystemType;
		}

		public int pushError(IntPtr luaState, string msg)
		{
			LuaDLL.lua_pushnil(luaState);
			LuaDLL.lua_pushstring(luaState, msg);
			return 2;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int ctype(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = FromState(luaState);
			Type type = objectTranslator.typeOf(luaState, 1);
			if (type == null)
			{
				return objectTranslator.pushError(luaState, "not a CLR class");
			}
			objectTranslator.pushObject(luaState, type, "luaNet_metatable");
			return 1;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int enumFromInt(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = FromState(luaState);
			Type type = objectTranslator.typeOf(luaState, 1);
			if (type == null || !type.IsEnum)
			{
				return objectTranslator.pushError(luaState, "not an enum");
			}
			object o = null;
			switch (LuaDLL.lua_type(luaState, 2))
			{
			case LuaTypes.LUA_TNUMBER:
			{
				int value2 = (int)LuaDLL.lua_tonumber(luaState, 2);
				o = Enum.ToObject(type, value2);
				break;
			}
			case LuaTypes.LUA_TSTRING:
			{
				string value = LuaDLL.lua_tostring(luaState, 2);
				string text = null;
				try
				{
					o = Enum.Parse(type, value);
				}
				catch (ArgumentException ex)
				{
					text = ex.Message;
				}
				if (text != null)
				{
					return objectTranslator.pushError(luaState, text);
				}
				break;
			}
			default:
				return objectTranslator.pushError(luaState, "second argument must be a integer or a string");
			}
			objectTranslator.pushObject(luaState, o, "luaNet_metatable");
			return 1;
		}

		internal void pushType(IntPtr luaState, Type t)
		{
			pushObject(luaState, new ProxyType(t), "luaNet_class");
		}

		internal void pushFunction(IntPtr luaState, LuaCSFunction func)
		{
			pushObject(luaState, func, "luaNet_function");
		}

		internal void pushObject(IntPtr luaState, object o, string metatable)
		{
			int value = -1;
			if (o == null)
			{
				LuaDLL.lua_pushnil(luaState);
				return;
			}
			if (objectsBackMap.TryGetValue(o, out value))
			{
				LuaDLL.luaL_getmetatable(luaState, "luaNet_objects");
				LuaDLL.lua_rawgeti(luaState, -1, value);
				if (LuaDLL.lua_type(luaState, -1) != LuaTypes.LUA_TNIL)
				{
					LuaDLL.lua_remove(luaState, -2);
					return;
				}
				LuaDLL.lua_remove(luaState, -1);
				LuaDLL.lua_remove(luaState, -1);
				collectObject(o, value);
			}
			value = addObject(o);
			pushNewObject(luaState, o, value, metatable);
		}

		private void pushNewObject(IntPtr luaState, object o, int index, string metatable)
		{
			if (metatable == "luaNet_metatable")
			{
				LuaDLL.luaL_getmetatable(luaState, o.GetType().AssemblyQualifiedName);
				if (LuaDLL.lua_isnil(luaState, -1))
				{
					LuaDLL.lua_settop(luaState, -2);
					LuaDLL.luaL_newmetatable(luaState, o.GetType().AssemblyQualifiedName);
					LuaDLL.lua_pushstring(luaState, "cache");
					LuaDLL.lua_newtable(luaState);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushlightuserdata(luaState, LuaDLL.luanet_gettag());
					LuaDLL.lua_pushnumber(luaState, 1.0);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushstring(luaState, "__index");
					LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
					LuaDLL.lua_rawget(luaState, LuaIndexes.LUA_REGISTRYINDEX);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushstring(luaState, "__gc");
					LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.gcFunction);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushstring(luaState, "__tostring");
					LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.toStringFunction);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushstring(luaState, "__newindex");
					LuaDLL.lua_pushstdcallcfunction(luaState, metaFunctions.newindexFunction);
					LuaDLL.lua_rawset(luaState, -3);
				}
			}
			else
			{
				LuaDLL.luaL_getmetatable(luaState, metatable);
			}
			LuaDLL.luaL_getmetatable(luaState, "luaNet_objects");
			LuaDLL.luanet_newudata(luaState, index);
			LuaDLL.lua_pushvalue(luaState, -3);
			LuaDLL.lua_remove(luaState, -4);
			LuaDLL.lua_setmetatable(luaState, -2);
			LuaDLL.lua_pushvalue(luaState, -1);
			LuaDLL.lua_rawseti(luaState, -3, index);
			LuaDLL.lua_remove(luaState, -2);
		}

		internal object getAsType(IntPtr luaState, int stackPos, Type paramType)
		{
			ExtractValue extractValue = typeChecker.checkType(luaState, stackPos, paramType);
			if (extractValue != null)
			{
				return extractValue(luaState, stackPos);
			}
			return null;
		}

		internal void collectObject(int udata)
		{
			object value;
			if (objects.TryGetValue(udata, out value))
			{
				objects.Remove(udata);
				objectsBackMap.Remove(value);
			}
		}

		private void collectObject(object o, int udata)
		{
			objects.Remove(udata);
			objectsBackMap.Remove(o);
		}

		private int addObject(object obj)
		{
			int num = nextObj++;
			objects[num] = obj;
			objectsBackMap[obj] = num;
			return num;
		}

		internal object getObject(IntPtr luaState, int index)
		{
			switch (LuaDLL.lua_type(luaState, index))
			{
			case LuaTypes.LUA_TNUMBER:
				return LuaDLL.lua_tonumber(luaState, index);
			case LuaTypes.LUA_TSTRING:
				return LuaDLL.lua_tostring(luaState, index);
			case LuaTypes.LUA_TBOOLEAN:
				return LuaDLL.lua_toboolean(luaState, index);
			case LuaTypes.LUA_TTABLE:
				return getTable(luaState, index);
			case LuaTypes.LUA_TFUNCTION:
				return getFunction(luaState, index);
			case LuaTypes.LUA_TUSERDATA:
			{
				int num = LuaDLL.luanet_tonetobject(luaState, index);
				if (num != -1)
				{
					return objects[num];
				}
				return getUserData(luaState, index);
			}
			default:
				return null;
			}
		}

		public LuaTable getTable(IntPtr luaState, int index)
		{
			LuaDLL.lua_pushvalue(luaState, index);
			return new LuaTable(LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX), interpreter);
		}

		internal LuaUserData getUserData(IntPtr luaState, int index)
		{
			LuaDLL.lua_pushvalue(luaState, index);
			return new LuaUserData(LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX), interpreter);
		}

		internal LuaFunction getFunction(IntPtr luaState, int index)
		{
			LuaDLL.lua_pushvalue(luaState, index);
			return new LuaFunction(LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX), interpreter);
		}

		internal object getNetObject(IntPtr luaState, int index)
		{
			int num = LuaDLL.luanet_tonetobject(luaState, index);
			if (num != -1)
			{
				return objects[num];
			}
			return null;
		}

		internal object getRawNetObject(IntPtr luaState, int index)
		{
			int num = LuaDLL.luanet_rawnetobj(luaState, index);
			if (num != -1)
			{
				return objects[num];
			}
			return null;
		}

		internal int returnValues(IntPtr luaState, object[] returnValues)
		{
			if (LuaDLL.lua_checkstack(luaState, returnValues.Length + 5))
			{
				for (int i = 0; i < returnValues.Length; i++)
				{
					push(luaState, returnValues[i]);
				}
				return returnValues.Length;
			}
			return 0;
		}

		internal object[] popValues(IntPtr luaState, int oldTop)
		{
			int num = LuaDLL.lua_gettop(luaState);
			if (oldTop == num)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			for (int i = oldTop + 1; i <= num; i++)
			{
				arrayList.Add(getObject(luaState, i));
			}
			LuaDLL.lua_settop(luaState, oldTop);
			return arrayList.ToArray();
		}

		internal object[] popValues(IntPtr luaState, int oldTop, Type[] popTypes)
		{
			int num = LuaDLL.lua_gettop(luaState);
			if (oldTop == num)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			int num2 = ((popTypes[0] == typeof(void)) ? 1 : 0);
			for (int i = oldTop + 1; i <= num; i++)
			{
				arrayList.Add(getAsType(luaState, i, popTypes[num2]));
				num2++;
			}
			LuaDLL.lua_settop(luaState, oldTop);
			return arrayList.ToArray();
		}

		private static bool IsILua(object o)
		{
			if (o is ILuaGeneratedType)
			{
				Type type = o.GetType();
				return type.GetInterface("ILuaGeneratedType") != null;
			}
			return false;
		}

		internal void push(IntPtr luaState, object o)
		{
			if (o == null)
			{
				LuaDLL.lua_pushnil(luaState);
			}
			else if (o is GameObject && (GameObject)o == null)
			{
				LuaDLL.lua_pushnil(luaState);
			}
			else if (o is sbyte || o is byte || o is short || o is ushort || o is int || o is uint || o is long || o is float || o is ulong || o is decimal || o is double)
			{
				double number = Convert.ToDouble(o);
				LuaDLL.lua_pushnumber(luaState, number);
			}
			else if (o is char)
			{
				double number2 = (int)(char)o;
				LuaDLL.lua_pushnumber(luaState, number2);
			}
			else if (o is string)
			{
				string str = (string)o;
				LuaDLL.lua_pushstring(luaState, str);
			}
			else if (o is bool)
			{
				bool value = (bool)o;
				LuaDLL.lua_pushboolean(luaState, value);
			}
			else if (IsILua(o))
			{
				((ILuaGeneratedType)o).__luaInterface_getLuaTable().push(luaState);
			}
			else if (o is LuaTable)
			{
				((LuaTable)o).push(luaState);
			}
			else if (o is LuaCSFunction)
			{
				pushFunction(luaState, (LuaCSFunction)o);
			}
			else if (o is LuaFunction)
			{
				((LuaFunction)o).push(luaState);
			}
			else
			{
				pushObject(luaState, o, "luaNet_metatable");
			}
		}

		internal bool matchParameters(IntPtr luaState, MethodBase method, ref MethodCache methodCache)
		{
			return metaFunctions.matchParameters(luaState, method, ref methodCache);
		}

		internal Array tableToArray(object luaParamValue, Type paramArrayType)
		{
			return metaFunctions.TableToArray(luaParamValue, paramArrayType);
		}
	}
}
