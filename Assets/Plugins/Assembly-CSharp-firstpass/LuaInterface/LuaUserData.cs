using System;

namespace LuaInterface
{
	public class LuaUserData : LuaBase
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

		public LuaUserData(int reference, LuaState interpreter)
		{
			_Reference = reference;
			_Interpreter = interpreter;
		}

		public object[] Call(params object[] args)
		{
			return _Interpreter.callFunction(this, args);
		}

		internal void push(IntPtr luaState)
		{
			LuaDLL.lua_getref(luaState, _Reference);
		}

		public override string ToString()
		{
			return "userdata";
		}
	}
}
