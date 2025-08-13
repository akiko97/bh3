using System.Collections.Generic;

namespace MoleMole
{
	public static class DevLevelConfigData
	{
		public static List<int> monsterInstanceIds = new List<int>();

		public static List<DevMonsterData> monsterDevDatas = new List<DevMonsterData>();

		public static List<string> avatarTypeNames = new List<string>();

		public static List<DevAvatarData> avatarDevDatas = new List<DevAvatarData>();

		public static DevStageData stageDevData;

		public static string LEVEL_PATH = null;

		public static LevelActor.Mode LEVEL_MODE = LevelActor.Mode.Single;

		public static int LEVEL_DIFFICULTY = 0;

		public static LevelManager _levelManager;

		public static bool pariticleMode = true;

		public static bool configFromScene = false;

		public static bool isBenchmark = false;
	}
}
