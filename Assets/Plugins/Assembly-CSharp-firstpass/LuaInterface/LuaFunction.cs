using System;

namespace LuaInterface
{
	public class LuaFunction : LuaBase
	{
		internal LuaCSFunction function;

		public LuaFunction(int reference, LuaState interpreter)
		{
			_Reference = reference;
			function = null;
			_Interpreter = interpreter;
		}

		public LuaFunction(LuaCSFunction function, LuaState interpreter)
		{
			_Reference = 0;
			this.function = function;
			_Interpreter = interpreter;
		}

		internal object[] call(object[] args, Type[] returnTypes)
		{
			return _Interpreter.callFunction(this, args, returnTypes);
		}

		public object[] Call(params object[] args)
		{
			return _Interpreter.callFunction(this, args);
		}

		internal void push(IntPtr luaState)
		{
			if (_Reference != 0)
			{
				LuaDLL.lua_getref(luaState, _Reference);
			}
			else
			{
				_Interpreter.pushCSFunction(function);
			}
		}

		public override string ToString()
		{
			return "function";
		}

		public override bool Equals(object o)
		{
			if (o is LuaFunction)
			{
				LuaFunction luaFunction = (LuaFunction)o;
				if (_Reference != 0 && luaFunction._Reference != 0)
				{
					return _Interpreter.compareRef(luaFunction._Reference, _Reference);
				}
				return function == luaFunction.function;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (_Reference != 0)
			{
				return _Reference;
			}
			return function.GetHashCode();
		}
	}
}
