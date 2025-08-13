using System;

namespace MoleMole
{
	public static class AvatarInputControllerData
	{
		public const string JOYSTICK_UP_PREFAB_PATH = "Levels/InLevelUI/JoyStickUp";

		public const string JOYSTICK_DOWN_PREFAB_PATH = "Levels/InLevelUI/JoyStickDown";

		public const int STICK_BUTTON_DOWN_RADIUS = 320;

		public const int STICK_BUTTON_LEAVE_RADIUS = 450;

		public const string ATTACK_BTN_UP_PREFAB_PATH = "Levels/InLevelUI/FuncBtnUp";

		public const string ATTACK_BTN_DOWN_PREFAB_PATH = "Levels/InLevelUI/FuncBtnDown";

		public const float IP5_IP6_RATIO = 1.125f;

		public const int ATTACK_BUTTON_RADIUS = 84;

		public const int SCREEN_DEFAULT_START_X = 0;

		public const int SCREEN_DEFAULT_END_X = 4096;

		public const int SCREEN_DEFAULT_START_Y = 0;

		public const uint POLE_KEY_JOY_MODE = 1u;

		public const int ATTACK_BUTTON_ID = 1;

		public const int SKILL_BUTTON_1_ID = 2;

		public const int SKILL_BUTTON_2_ID = 3;

		public static uint GetControlTypeByCamearAndControlType(uint contType)
		{
			if (contType == 1)
			{
				return 1u;
			}
			throw new Exception("Invalid Type or State!");
		}
	}
}
