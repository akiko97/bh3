namespace LuaInterface
{
	public class LuaThread : LuaState
	{
		private bool start;

		private int threadRef;

		private LuaState parent;

		private LuaFunction func;

		public LuaThread(LuaState parentState, LuaFunction threadFunc)
		{
			tracebackFunction = parentState.tracebackFunction;
			base.translator = parentState.translator;
			base.translator.interpreter = this;
			panicCallback = parentState.panicCallback;
			printFunction = parentState.printFunction;
			loadfileFunction = parentState.loadfileFunction;
			loaderFunction = parentState.loaderFunction;
			dofileFunction = parentState.dofileFunction;
			func = threadFunc;
			parent = parentState;
			L = LuaDLL.lua_newthread(parent.L);
			threadRef = LuaDLL.luaL_ref(parent.L, LuaIndexes.LUA_REGISTRYINDEX);
		}

		public override void Dispose(bool dispose)
		{
			if (dispose)
			{
				LuaDLL.luaL_unref(parent.L, LuaIndexes.LUA_REGISTRYINDEX, threadRef);
			}
		}

		public void Start()
		{
			if (IsInactive() && !start)
			{
				start = true;
			}
		}

		public int Resume()
		{
			return Resume(null, null);
		}

		public int Resume(object[] args, LuaTable env)
		{
			int result = 0;
			int oldTop = LuaDLL.lua_gettop(L);
			if (start)
			{
				start = false;
				func.push(L);
				if (env != null)
				{
					env.push(L);
					LuaDLL.lua_setfenv(L, -2);
				}
				result = resume(args, oldTop);
			}
			else if (IsSuspended())
			{
				result = resume(args, oldTop);
			}
			return result;
		}

		private int resume(object[] args, int oldTop)
		{
			int narg = 0;
			if (args != null)
			{
				narg = args.Length;
				for (int i = 0; i < args.Length; i++)
				{
					base.translator.push(L, args[i]);
				}
			}
			int num = 0;
			num = LuaDLL.lua_resume(L, narg);
			if (num > 1)
			{
				int oldTop2 = LuaDLL.lua_gettop(L);
				ThrowExceptionFromError(oldTop2);
			}
			return num;
		}

		public bool IsStarted()
		{
			return start;
		}

		public bool IsSuspended()
		{
			int num = LuaDLL.lua_status(L);
			return num == 1;
		}

		public bool IsDead()
		{
			int num = LuaDLL.lua_status(L);
			return num > 1;
		}

		public bool IsInactive()
		{
			int num = LuaDLL.lua_status(L);
			return num == 0;
		}
	}
}
