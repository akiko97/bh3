using System;
using System.Text;
using UnityEngine;

namespace LuaInterface
{
	public class LuaStatic
	{
		public delegate string LuaFileReader(string fileName);

		private const string LUA_ROOT = "Lua";

		public static LuaFileReader luaFileReader;

		public static string init_luanet = "local metatable = {}\n            local rawget = rawget\n            local import_type = luanet.import_type\n            local load_assembly = luanet.load_assembly\n            luanet.error, luanet.type = error, type\n            -- Lookup a .NET identifier component.\n            function metatable:__index(key) -- key is e.g. 'Form'\n            -- Get the fully-qualified name, e.g. 'System.Windows.Forms.Form'\n            local fqn = rawget(self,'.fqn')\n            fqn = ((fqn and fqn .. '.') or '') .. key\n\n            -- Try to find either a luanet function or a CLR type\n            local obj = rawget(luanet,key) or import_type(fqn)\n\n            -- If key is neither a luanet function or a CLR type, then it is simply\n            -- an identifier component.\n            if obj == nil then\n                -- It might be an assembly, so we load it too.\n                    pcall(load_assembly,fqn)\n                    obj = { ['.fqn'] = fqn }\n            setmetatable(obj, metatable)\n            end\n\n            -- Cache this lookup\n            rawset(self, key, obj)\n            return obj\n            end\n\n            -- A non-type has been called; e.g. foo = System.Foo()\n            function metatable:__call(...)\n            error('No such type: ' .. rawget(self,'.fqn'), 2)\n            end\n\n            -- This is the root of the .NET namespace\n            luanet['.fqn'] = false\n            setmetatable(luanet, metatable)\n\n            -- Preload the mscorlib assembly\n            luanet.load_assembly('mscorlib')";

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int panic(IntPtr L)
		{
			string message = string.Format("unprotected error in call to Lua API ({0})", LuaDLL.lua_tostring(L, -1));
			throw new LuaException(message);
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int traceback(IntPtr L)
		{
			LuaDLL.lua_getglobal(L, "debug");
			LuaDLL.lua_getfield(L, -1, "traceback");
			LuaDLL.lua_pushvalue(L, 1);
			LuaDLL.lua_pushnumber(L, 2.0);
			LuaDLL.lua_call(L, 2, 1);
			return 1;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int print(IntPtr L)
		{
			return 0;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int loader(IntPtr L)
		{
			string empty = string.Empty;
			empty = LuaDLL.lua_tostring(L, 1);
			empty = empty.Replace('.', '/');
			empty = string.Format("{0}/{1}.lua", "Lua", empty);
			string empty2 = string.Empty;
			if (luaFileReader != null)
			{
				empty2 = luaFileReader(empty);
			}
			else
			{
				TextAsset textAsset = Resources.Load<TextAsset>(empty);
				if (textAsset == null)
				{
					return 0;
				}
				empty2 = textAsset.text;
			}
			LuaDLL.luaL_loadbuffer(L, empty2, Encoding.UTF8.GetByteCount(empty2), empty);
			return 1;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int dofile(IntPtr L)
		{
			string empty = string.Empty;
			empty = LuaDLL.lua_tostring(L, 1);
			empty.Replace('.', '/');
			empty += ".lua";
			int num = LuaDLL.lua_gettop(L);
			TextAsset textAsset = (TextAsset)Resources.Load(empty);
			if (textAsset == null)
			{
				return LuaDLL.lua_gettop(L) - num;
			}
			if (LuaDLL.luaL_loadbuffer(L, textAsset.text, Encoding.UTF8.GetByteCount(textAsset.text), empty) == 0)
			{
				LuaDLL.lua_call(L, 0, LuaDLL.LUA_MULTRET);
			}
			return LuaDLL.lua_gettop(L) - num;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int loadfile(IntPtr L)
		{
			return loader(L);
		}
	}
}
