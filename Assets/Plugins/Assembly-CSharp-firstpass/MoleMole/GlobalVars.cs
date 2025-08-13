namespace MoleMole
{
	public static class GlobalVars
	{
		public const string BUNDLE_IDENTIFIER = "com.miHoYo.enterprise.NGHSoD";

		public const string VERSION = "0.9.9";

		public const int IN_LEVEL_FRAME_RATE = 60;

		public const float IN_LEVEL_TIME_PER_FRAME = 1f / 60f;

		public const int MOUSE_LEFT_BUTTON = 0;

		public const int MOUSE_RIGHT_BUTTON = 1;

		public const int MOUSE_MIDDLE_BUTTON = 2;

		public const string CSV_LINE_TOKEN = "\n";

		public const string CSV_MAC_LINE_TOKEN = "\r";

		public const string CSV_WORD_TOKEN = ",";

		public const string CSV_WORD_TAB_TOKEN = "\t";

		public const string DEBUG_LEVEL_LUA_PATH = "Lua/Levels/Common/LevelTest.lua";

		public static bool KEYBOARD_FUNCTION_BUTTON_CONTROL = true;

		public static bool LEVEL_MODE_DEBUG = true;

		public static bool DISABLE_NETWORK_DEBUG;

		public static bool USE_GET_PATH_SWITCH = true;

		public static bool MONSTER_USE_DYNAMIC_BONE = true;

		public static bool AVATAR_USE_DYNAMIC_BONE = true;

		public static bool UI_AVATAR_USE_DYNAMIC_BONE = true;

		public static bool USE_REFLECTION = true;

		public static int DEBUG_NETWORK_DELAY_LEVEL;

		public static bool DEBUG_FEATURE_ON;

		public static bool DataUseAssetBundle;

		public static bool ResourceUseAssetBundle;

		public static bool UseSpliteResources;

		public static bool ENABLE_CONTINUOUS_DETECT_MODE = true;

		public static bool ENABLE_ISLAND_ENTRY = true;

		public static bool IS_BENCHMARK;

		public static bool STATIC_CLOUD_MODE;

		public static bool ENABLE_EXCEPTION_CONTINUE = true;

		public static bool CHECK_CONFIG;

		public static bool ENABLE_AUTO_BATTLE;

		public static bool muteDamageText;

		public static bool muteInlevelLock;

		public static string GetBuildInfo()
		{
			return string.Empty;
		}
	}
}
