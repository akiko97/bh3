using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class MonsterData
	{
		public enum MonsterTagGroup
		{
			ShowHit = 0,
			FreezeDirection = 1,
			Throw = 2,
			Movement = 3,
			IdleOrMovement = 4,
			Grounded = 5,
			Idle = 6,
			AttackOrSkill = 7
		}

		public const string MONSTER_LEVEL_TABLE_FILE_PATH = "Entities/Monster/";

		private const string MONSTER_PREFAB_PATH = "Entities/Monster/";

		private const string MONSTER_PREFAB_LOW_SUFFIX = "_Low";

		public const int MONSTER_NUMBER_LENGTH = 3;

		public const float ELITE_MONSTER_SCALE = 1.1f;

		public const string ELITE_MONSTER_EFFECT_PATTERN = "Monster_Elite_01";

		public const string FROZEN_DIE_EFFECT_PATTERN = "Frozen_Die";

		public const float WALL_STEER_ANGLE_FACTOR = 0.01f;

		public const float WALL_GROUND_CONTACT_DEGREE_THRESHOLD = 20f;

		public const float MONSTER_THROW_MASS_RATIO = 0.1f;

		public const float MONSTER_FAST_KILL_ANI_DAMAGE_RATIO = 0.9f;

		public const float MONSTER_FAST_KILL_HOLD_DURATION = 0.3f;

		public const float MONSTER_THROW_FAST_KILL_WAIT_DURATION = 0.1f;

		public static Dictionary<string, ConfigOverrideGroup> monsterGroupMap;

		private static List<string> _configPathList;

		private static Action<string> _loadJsonConfigCallback = null;

		private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();

		public static int MONSTER_FREEZE_DIR_TAG;

		public static int MONSTER_IDLESUB_TAG;

		public static int MONSTER_MOVESUB_TAG;

		public static int MONSTER_HIT_TAG;

		public static int MONSTER_DIE_TAG;

		public static int MONSTER_ATKBS_TAG;

		public static int MONSTER_ATK_TAG;

		public static int MONSTER_SKL_TAG;

		public static int MONSTER_THROWSUB_TAG;

		public static HashSet<int>[] MONSTER_TAG_GROUPS;

		public static Shader MONSTER_OPAQUE_SHADER;

		public static Shader MONSTER_TRANSPARENT_SHADER;

		public static Shader MONSTER_ELITE_SHADER;

		private static bool _monsterDataInited = false;

		public static string GetPrefabResPath(string monsterName, string typeName, bool useLow = false)
		{
			MonsterConfigMetaData monsterConfigMetaDataByKey = MonsterConfigMetaDataReader.GetMonsterConfigMetaDataByKey(monsterName, typeName);
			string categoryName = monsterConfigMetaDataByKey.categoryName;
			string subTypeName = monsterConfigMetaDataByKey.subTypeName;
			ConfigMonster monsterConfig = GetMonsterConfig(monsterConfigMetaDataByKey.monsterName, monsterConfigMetaDataByKey.typeName, string.Empty);
			if (useLow && monsterConfig.CommonArguments.HasLowPrefab)
			{
				return "Entities/Monster/" + categoryName + "/" + subTypeName + "/" + subTypeName + "_Low";
			}
			return "Entities/Monster/" + categoryName + "/" + subTypeName + "/" + subTypeName;
		}

		public static void ReloadFromFile()
		{
			MonsterConfigMetaDataReader.LoadFromFile();
			UniqueMonsterMetaDataReader.LoadFromFile();
			LoadAllMonsterConfig();
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
		{
			_loadJsonConfigCallback = finishCallback;
			_configPathList = new List<string>();
			MonsterConfigMetaDataReader.LoadFromFile();
			yield return null;
			UniqueMonsterMetaDataReader.LoadFromFile();
			yield return null;
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(LoadAllMonsterConfigAsync(progressSpan, moveOneStepCallback));
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (ConfigOverrideGroup value in monsterGroupMap.Values)
			{
				HashUtils.TryHashObject(value.Default, ref lastHash);
				if (value.Overrides == null)
				{
					continue;
				}
				foreach (KeyValuePair<string, object> @override in value.Overrides)
				{
					HashUtils.TryHashObject(@override, ref lastHash);
				}
			}
			return lastHash;
		}

		public static void LoadAllMonsterConfig()
		{
			monsterGroupMap = new Dictionary<string, ConfigOverrideGroup>();
			List<MonsterConfigMetaData> itemList = MonsterConfigMetaDataReader.GetItemList();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (MonsterConfigMetaData item in itemList)
			{
				string configFile = item.configFile;
				hashSet.Add(configFile);
			}
			foreach (string item2 in hashSet)
			{
				ConfigOverrideGroup value = ConfigUtil.LoadJSONConfig<ConfigOverrideGroup>("Data/MonsterConfig/" + item2);
				monsterGroupMap.Add(item2, value);
			}
		}

		public static IEnumerator LoadAllMonsterConfigAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			monsterGroupMap = new Dictionary<string, ConfigOverrideGroup>();
			List<MonsterConfigMetaData> itemList = MonsterConfigMetaDataReader.GetItemList();
			HashSet<string> ConfigFiles = new HashSet<string>();
			if (itemList.Count == 0)
			{
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("MonsterData");
					_loadJsonConfigCallback = null;
				}
				yield break;
			}
			foreach (MonsterConfigMetaData metaData in itemList)
			{
				string configPath = metaData.configFile;
				ConfigFiles.Add(configPath);
			}
			foreach (string configFile in ConfigFiles)
			{
				string path = "Data/MonsterConfig/" + configFile;
				_configPathList.Add(path);
			}
			float step = progressSpan / (float)ConfigFiles.Count;
			_loadDataBackGroundWorker.StartBackGroundWork("MonsterData");
			foreach (string configFile2 in ConfigFiles)
			{
				string path2 = "Data/MonsterConfig/" + configFile2;
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync(path2);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null monsterPath :" + path2);
				if (asyncRequest != null)
				{
					yield return asyncRequest.operation;
					if (moveOneStepCallback != null)
					{
						moveOneStepCallback(step);
					}
					ConfigUtil.LoadJSONStrConfigMultiThread<ConfigOverrideGroup>(asyncRequest.asset.ToString(), _loadDataBackGroundWorker, OnLoadOneJsonConfigFinish, configFile2);
				}
			}
		}

		private static void OnLoadOneJsonConfigFinish(ConfigOverrideGroup configGroup, string configFile)
		{
			string item = "Data/MonsterConfig/" + configFile;
			_configPathList.Remove(item);
			monsterGroupMap.Add(configFile, configGroup);
			if (_configPathList.Count == 0)
			{
				_loadDataBackGroundWorker.StopBackGroundWork(false);
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("MonsterData");
					_loadJsonConfigCallback = null;
				}
			}
		}

		public static MonsterConfigMetaData GetMonsterConfigMetaData(string monsterName, string typeName)
		{
			return MonsterConfigMetaDataReader.GetMonsterConfigMetaDataByKey(monsterName, typeName);
		}

		public static UniqueMonsterMetaData GetUniqueMonsterMetaData(uint uniqueMonsterID)
		{
			return UniqueMonsterMetaDataReader.GetUniqueMonsterMetaDataByKey(uniqueMonsterID);
		}

		public static ConfigMonster GetMonsterConfig(string monsterName, string typeName, string configType = "")
		{
			MonsterConfigMetaData monsterConfigMetaDataByKey = MonsterConfigMetaDataReader.GetMonsterConfigMetaDataByKey(monsterName, typeName);
			string configFile = monsterConfigMetaDataByKey.configFile;
			string text = configType;
			if (text == string.Empty)
			{
				text = monsterConfigMetaDataByKey.configType;
			}
			return monsterGroupMap[configFile].GetConfig<ConfigMonster>(text);
		}

		public static ConfigMonster GetFirstMonsterConfigBySubTypeName(string subTypeName)
		{
			foreach (MonsterConfigMetaData item in MonsterConfigMetaDataReader.GetItemList())
			{
				if (item.subTypeName == subTypeName)
				{
					string configFile = item.configFile;
					string configType = item.configType;
					return monsterGroupMap[configFile].GetConfig<ConfigMonster>(configType);
				}
			}
			return null;
		}

		public static void InitMonsterData()
		{
			if (!_monsterDataInited)
			{
				AllMonsterConfigOnLevelLoaded();
				MONSTER_FREEZE_DIR_TAG = Animator.StringToHash("MONSTER_FREEZE_DIR");
				MONSTER_IDLESUB_TAG = Animator.StringToHash("MONSTER_IDLESUB");
				MONSTER_MOVESUB_TAG = Animator.StringToHash("MONSTER_MOVESUB");
				MONSTER_HIT_TAG = Animator.StringToHash("MONSTER_HITSUB");
				MONSTER_DIE_TAG = Animator.StringToHash("MONSTER_DIESUB");
				MONSTER_ATKBS_TAG = Animator.StringToHash("MONSTER_ATKBS");
				MONSTER_ATK_TAG = Animator.StringToHash("MONSTER_ATK");
				MONSTER_SKL_TAG = Animator.StringToHash("MONSTER_SKL");
				MONSTER_THROWSUB_TAG = Animator.StringToHash("MONSTER_THROWSUB");
				MONSTER_TAG_GROUPS = new HashSet<int>[8]
				{
					new HashSet<int> { MONSTER_HIT_TAG },
					new HashSet<int> { MONSTER_HIT_TAG, MONSTER_ATK_TAG, MONSTER_DIE_TAG, MONSTER_THROWSUB_TAG, MONSTER_FREEZE_DIR_TAG },
					new HashSet<int> { MONSTER_THROWSUB_TAG },
					new HashSet<int> { MONSTER_MOVESUB_TAG },
					new HashSet<int> { MONSTER_MOVESUB_TAG, MONSTER_IDLESUB_TAG },
					new HashSet<int> { MONSTER_MOVESUB_TAG, MONSTER_IDLESUB_TAG, MONSTER_SKL_TAG, MONSTER_ATK_TAG, MONSTER_HIT_TAG, MONSTER_FREEZE_DIR_TAG },
					new HashSet<int> { MONSTER_IDLESUB_TAG },
					new HashSet<int> { MONSTER_ATK_TAG, MONSTER_ATKBS_TAG, MONSTER_SKL_TAG }
				};
				MONSTER_OPAQUE_SHADER = Shader.Find("miHoYo/Character/Simple_Emission_Opaque");
				MONSTER_TRANSPARENT_SHADER = Shader.Find("miHoYo/Character/Simple_Emission");
				MONSTER_ELITE_SHADER = Shader.Find("miHoYo/Character/Simple_Emission_Elite");
			}
		}

		public static List<MonsterConfigMetaData> GetAllMonsterConfigMetaData()
		{
			return MonsterConfigMetaDataReader.GetItemList();
		}

		private static void AllMonsterConfigOnLevelLoaded()
		{
			foreach (ConfigOverrideGroup value in monsterGroupMap.Values)
			{
				((ConfigMonster)value.Default).OnLevelLoaded();
				if (value.Overrides == null)
				{
					continue;
				}
				foreach (object value2 in value.Overrides.Values)
				{
					((ConfigMonster)value2).OnLevelLoaded();
				}
			}
		}
	}
}
