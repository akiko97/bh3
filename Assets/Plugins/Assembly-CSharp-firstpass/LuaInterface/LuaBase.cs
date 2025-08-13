using System;

namespace LuaInterface
{
	public abstract class LuaBase : IDisposable
	{
		private bool _Disposed;

		protected int _Reference;

		protected LuaState _Interpreter;

		~LuaBase()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposeManagedResources)
		{
			if (!_Disposed)
			{
				if (disposeManagedResources && _Reference != 0)
				{
					_Interpreter.dispose(_Reference);
				}
				_Interpreter = null;
				_Disposed = true;
			}
		}

		public override bool Equals(object o)
		{
			if (o is LuaBase)
			{
				LuaBase luaBase = (LuaBase)o;
				return _Interpreter.compareRef(luaBase._Reference, _Reference);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _Reference;
		}
	}
}
