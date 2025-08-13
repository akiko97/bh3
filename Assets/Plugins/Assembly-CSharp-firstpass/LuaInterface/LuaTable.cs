using System;
using System.Collections;

namespace LuaInterface
{
	public class LuaTable : LuaBase
	{
		public object this[string field]
		{
			get
			{
				return _Interpreter.getObject(_Reference, field);
			}
			set
			{
				_Interpreter.setObject(_Reference, field, value);
			}
		}

		public object this[object field]
		{
			get
			{
				return _Interpreter.getObject(_Reference, field);
			}
			set
			{
				_Interpreter.setObject(_Reference, field, value);
			}
		}

		public ICollection Keys
		{
			get
			{
				return _Interpreter.GetTableDict(this).Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				return _Interpreter.GetTableDict(this).Values;
			}
		}

		public LuaTable(int reference, LuaState interpreter)
		{
			_Reference = reference;
			_Interpreter = interpreter;
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return _Interpreter.GetTableDict(this).GetEnumerator();
		}

		public void SetMetaTable(LuaTable metaTable)
		{
			push(_Interpreter.L);
			metaTable.push(_Interpreter.L);
			LuaDLL.lua_setmetatable(_Interpreter.L, -2);
			LuaDLL.lua_pop(_Interpreter.L, 1);
		}

		internal object rawget(string field)
		{
			return _Interpreter.rawGetObject(_Reference, field);
		}

		internal object rawgetFunction(string field)
		{
			object obj = _Interpreter.rawGetObject(_Reference, field);
			if (obj is LuaCSFunction)
			{
				return new LuaFunction((LuaCSFunction)obj, _Interpreter);
			}
			return obj;
		}

		internal void push(IntPtr luaState)
		{
			LuaDLL.lua_getref(luaState, _Reference);
		}

		public override string ToString()
		{
			return "table";
		}
	}
}
