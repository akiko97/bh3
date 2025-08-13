using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public static class AvatarData
	{
		public enum AvatarTagGroup
		{
			ShowHit = 0,
			MuteJoyStickInput = 1,
			UseJoyStickDirectionForMove = 2,
			AttackWithNoTarget = 3,
			AttackSteerOnEnter = 4,
			AttackTargetLeadDirection = 5,
			Stable = 6,
			AllowTriggerInput = 7,
			Movement = 8,
			AttackOrSkill = 9,
			Throw = 10
		}

		private const string AVATAR_REGISTRY_PATH = "Data/AvatarConfig/AvatarRegistry";

		private const string AVATAR_PREFAB_PATH = "Entities/Avatar/";

		private const string AVATAR_CONFIG_PATH = "Data/AvatarConfig";

		private const string AVATAR_PREFAB_PREFIX = "Avatar_";

		private const string AVATAR_PREFAB_LOW_SUFFIX = "_Low";

		public const int SKILL_NUM = 3;

		public const int SKILL_WEPON_INDEX = 3;

		public const string WEAPON_SKILL_NAME = "SKL_WEAPON";

		public const float HIT_HEAVY_THRESHOLD = 0.8f;

		public const float TIME_SLOW_ATTACK_TARGET_RADIUS = 2f;

		public const float AVATAR_CAMERA_PULL_Z_FURTHER_RATIO = 1.9f;

		public const float AVATAR_CAMERA_PULL_Z_FAR_RATIO = 1.3f;

		public const float AVATAR_CAMERA_PUSH_Z_NEAR_RATIO = 0.8f;

		public const float AVATAR_TARGET_FADE_OFF_TIME = 0.5f;

		public const float AVATAR_DEFAULT_RIGIDBODY_MASS = 1f;

		public const float AVATAR_ATTACK_EXIT_STEER_LERPING_RATIO = 1.3f;

		private static ConfigAvatarRegistry _avatarRegistry;

		private static List<string> _configPathList;

		private static Action<string> _loadJsonConfigCallback = null;

		private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();

		public static int AVATAR_APPEAR_TAG;

		public static int AVATAR_IDLESUB_TAG;

		public static int AVATAR_MOVESUB_TAG;

		public static int AVATAR_HIT_TAG;

		public static int AVATAR_DIE_TAG;

		public static int AVATAR_EVADE_TAG;

		public static int AVATAR_ATK_TAG;

		public static int AVATAR_SKL_TAG;

		public static int AVATAR_SKL_MOVE_TAG;

		public static int AVATAR_SKL_NO_TARGET_TAG;

		public static int AVATAR_THROW_TAG;

		public static int AVATAR_SWITCH_TAG;

		public static HashSet<int>[] AVATAR_TAG_GROUPS;

		private static bool _avatarDataInited = false;

		public static bool NO_TARGET_SKILL_CLEAR_AVATAR_TARGET = true;

		public static bool NO_TARGET_SKILL_RESET_AVATAR_TARGET_FADE_OFF_TIME = true;

		public static bool RUN_CLEAR_ATTACK_TARGET = true;

		public static string GetPrefabResPath(string type, bool useLow = false)
		{
			AvatarRegistryEntry value;
			_avatarRegistry.AvatarRegistry.TryGetValue(type, out value);
			if (value != null)
			{
				if (useLow && value.Config.CommonArguments.HasLowPrefab)
				{
					return "Entities/Avatar/" + type + "/Avatar_" + type + "_Low";
				}
				return "Entities/Avatar/" + type + "/Avatar_" + type;
			}
			throw new Exception("Invalid Type or State!: " + type);
		}

		public static ConfigAvatar GetAvatarConfig(string type)
		{
			if (_avatarRegistry.AvatarRegistry.ContainsKey(type))
			{
				return _avatarRegistry.AvatarRegistry[type].Config;
			}
			return null;
		}

		public static void ReloadFromFile()
		{
			_avatarRegistry = ConfigUtil.LoadJSONConfig<ConfigAvatarRegistry>("Data/AvatarConfig/AvatarRegistry");
			foreach (string key in _avatarRegistry.AvatarRegistry.Keys)
			{
				_avatarRegistry.AvatarRegistry[key].Config = ConfigUtil.LoadJSONConfig<ConfigAvatar>(string.Format("{0}/{1}{2}_Config", "Data/AvatarConfig", "Avatar_", key));
			}
			ReloadAvatarConfig();
		}

		private static void PrintAvatarSubSkills()
		{
			string text = string.Empty;
			foreach (string key in _avatarRegistry.AvatarRegistry.Keys)
			{
				ConfigAvatarAbilityUnlock[] abilitiesUnlock = _avatarRegistry.AvatarRegistry[key].Config.AbilitiesUnlock;
				for (int i = 0; i < abilitiesUnlock.Length; i++)
				{
					string text2 = text;
					text = text2 + abilitiesUnlock[i].UnlockBySubSkillID + "\t" + abilitiesUnlock[i].AbilityName + "\n";
				}
			}
			Debug.Log(text);
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarRegistryEntry value in _avatarRegistry.AvatarRegistry.Values)
			{
				HashUtils.TryHashObject(value.Config, ref lastHash);
			}
			return lastHash;
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
		{
			_loadJsonConfigCallback = finishCallback;
			_configPathList = new List<string>();
			AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync("Data/AvatarConfig/AvatarRegistry");
			yield return asyncRequest.operation;
			_avatarRegistry = ConfigUtil.LoadJSONStrConfig<ConfigAvatarRegistry>(asyncRequest.asset.ToString());
			if (_avatarRegistry.AvatarRegistry.Count == 0)
			{
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("AvatarData");
					_loadJsonConfigCallback = null;
				}
				yield break;
			}
			foreach (string avatarType in _avatarRegistry.AvatarRegistry.Keys)
			{
				string path = string.Format("{0}/{1}{2}_Config", "Data/AvatarConfig", "Avatar_", avatarType);
				_configPathList.Add(path);
			}
			float step = progressSpan / (float)_avatarRegistry.AvatarRegistry.Count;
			_loadDataBackGroundWorker.StartBackGroundWork("AvatarData");
			foreach (string avatarType2 in _avatarRegistry.AvatarRegistry.Keys)
			{
				string path2 = string.Format("{0}/{1}{2}_Config", "Data/AvatarConfig", "Avatar_", avatarType2);
				asyncRequest = ConfigUtil.LoadJsonConfigAsync(path2);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null avatarPath :" + path2);
				if (asyncRequest != null)
				{
					yield return asyncRequest.operation;
					if (moveOneStepCallback != null)
					{
						moveOneStepCallback(step);
					}
					ConfigUtil.LoadJSONStrConfigMultiThread<ConfigAvatar>(asyncRequest.asset.ToString(), _loadDataBackGroundWorker, OnLoadOneJsonConfigFinish, avatarType2);
				}
			}
		}

		private static void OnLoadOneJsonConfigFinish(ConfigAvatar configAvatar, string avatarType)
		{
			string item = string.Format("{0}/{1}{2}_Config", "Data/AvatarConfig", "Avatar_", avatarType);
			_configPathList.Remove(item);
			_avatarRegistry.AvatarRegistry[avatarType].Config = configAvatar;
			if (_configPathList.Count == 0)
			{
				_loadDataBackGroundWorker.StopBackGroundWork(false);
				ReloadAvatarConfig();
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("AvatarData");
					_loadJsonConfigCallback = null;
				}
			}
		}

		private static void ReloadAvatarConfig()
		{
			foreach (AvatarMetaData item in AvatarMetaDataReader.GetItemList())
			{
				ConfigAvatar config = _avatarRegistry.AvatarRegistry[item.avatarRegistryKey].Config;
				config.CommonArguments.Nature = (EntityNature)item.attribute;
				if (config.Skills.ContainsKey("SKL01"))
				{
					ConfigAvatarSkill configAvatarSkill = config.Skills["SKL01"];
					configAvatarSkill.SkillCD = item.SKL01CD;
					configAvatarSkill.SPCost = item.SKL01SP;
					configAvatarSkill.SPNeed = item.SKL01SPNeed;
					configAvatarSkill.ChargesCount = item.SKL01Charges;
				}
				if (config.Skills.ContainsKey("SKL02"))
				{
					ConfigAvatarSkill configAvatarSkill2 = config.Skills["SKL02"];
					configAvatarSkill2.SkillCD = item.SKL02CD;
					configAvatarSkill2.SPCost = item.SKL02SP;
					configAvatarSkill2.SPNeed = item.SKL02SPNeed;
					configAvatarSkill2.ChargesCount = item.SKL02Charges;
				}
				if (config.Skills.ContainsKey("SKL03"))
				{
					ConfigAvatarSkill configAvatarSkill3 = config.Skills["SKL03"];
					configAvatarSkill3.SkillCD = item.SKL03CD;
					configAvatarSkill3.SPCost = item.SKL03SP;
					configAvatarSkill3.SPNeed = item.SKL03SPNeed;
					configAvatarSkill3.ChargesCount = item.SKL03Charges;
				}
			}
		}

		public static uint GetAvatarTypeIDByName(string typeName)
		{
			return _avatarRegistry.AvatarRegistry[typeName].ID;
		}

		public static void InitAvatarData()
		{
			if (_avatarDataInited)
			{
				return;
			}
			foreach (AvatarRegistryEntry value in _avatarRegistry.AvatarRegistry.Values)
			{
				value.Config.OnLevelLoaded();
			}
			AVATAR_APPEAR_TAG = Animator.StringToHash("AVATAR_APPEAR");
			AVATAR_IDLESUB_TAG = Animator.StringToHash("AVATAR_IDLESUB");
			AVATAR_MOVESUB_TAG = Animator.StringToHash("AVATAR_MOVESUB");
			AVATAR_HIT_TAG = Animator.StringToHash("AVATAR_HITSUB");
			AVATAR_DIE_TAG = Animator.StringToHash("AVATAR_DIESUB");
			AVATAR_EVADE_TAG = Animator.StringToHash("AVATAR_EVADESUB");
			AVATAR_ATK_TAG = Animator.StringToHash("AVATAR_ATK");
			AVATAR_SKL_TAG = Animator.StringToHash("AVATAR_SKL");
			AVATAR_SKL_MOVE_TAG = Animator.StringToHash("AVATAR_SKL_MOVE");
			AVATAR_SKL_NO_TARGET_TAG = Animator.StringToHash("AVATAR_SKL_NO_TARGET");
			AVATAR_THROW_TAG = Animator.StringToHash("AVATAR_THROW");
			AVATAR_SWITCH_TAG = Animator.StringToHash("AVATAR_SWITCHSUB");
			AVATAR_TAG_GROUPS = new HashSet<int>[11]
			{
				new HashSet<int> { AVATAR_HIT_TAG, AVATAR_THROW_TAG },
				new HashSet<int> { AVATAR_APPEAR_TAG, AVATAR_DIE_TAG, AVATAR_SKL_TAG, AVATAR_SKL_NO_TARGET_TAG, AVATAR_HIT_TAG, AVATAR_THROW_TAG },
				new HashSet<int> { AVATAR_MOVESUB_TAG, AVATAR_IDLESUB_TAG, AVATAR_SKL_MOVE_TAG },
				new HashSet<int> { AVATAR_SWITCH_TAG, AVATAR_SKL_NO_TARGET_TAG, AVATAR_SKL_MOVE_TAG },
				new HashSet<int> { AVATAR_SKL_TAG },
				new HashSet<int> { AVATAR_ATK_TAG },
				new HashSet<int> { AVATAR_IDLESUB_TAG, AVATAR_APPEAR_TAG },
				new HashSet<int> { AVATAR_MOVESUB_TAG, AVATAR_IDLESUB_TAG, AVATAR_ATK_TAG, AVATAR_SKL_TAG, AVATAR_SKL_MOVE_TAG, AVATAR_SKL_NO_TARGET_TAG, AVATAR_HIT_TAG, AVATAR_THROW_TAG, AVATAR_SWITCH_TAG },
				new HashSet<int> { AVATAR_MOVESUB_TAG },
				new HashSet<int> { AVATAR_ATK_TAG, AVATAR_SKL_TAG, AVATAR_SKL_MOVE_TAG, AVATAR_SKL_NO_TARGET_TAG },
				new HashSet<int> { AVATAR_THROW_TAG }
			};
		}

		public static Dictionary<string, AvatarRegistryEntry> GetAllAvatarData()
		{
			return _avatarRegistry.AvatarRegistry;
		}

		public static void UnlockAvatarAbilities(AvatarDataItem avatarDataItem, AvatarActor avatarActor, bool useLeaderSkill)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (KeyValuePair<string, ConfigEntityAbilityEntry> ability in avatarActor.config.Abilities)
			{
				avatarActor.abilityIDMap.Add(ability.Key, ability.Value.AbilityName);
				dictionary.Add(ability.Key, false);
			}
			for (int i = 0; i < avatarActor.config.AbilitiesUnlock.Length; i++)
			{
				bool flag = false;
				ConfigAvatarAbilityUnlock configAvatarAbilityUnlock = avatarActor.config.AbilitiesUnlock[i];
				AvatarSubSkillDataItem outSubSkillItem = null;
				if ((!configAvatarAbilityUnlock.IsUnlockBySkill) ? CheckUnlockBySubSkillIDAndAddParam(configAvatarAbilityUnlock, avatarDataItem, out outSubSkillItem, useLeaderSkill) : CheckUnlockBySkillID(configAvatarAbilityUnlock, avatarDataItem, useLeaderSkill))
				{
					if (outSubSkillItem != null)
					{
						AddUnlockedAbility(avatarActor, configAvatarAbilityUnlock, dictionary, outSubSkillItem.SkillParam_1, outSubSkillItem.SkillParam_2, outSubSkillItem.SkillParam_3);
					}
					else
					{
						AddUnlockedAbility(avatarActor, configAvatarAbilityUnlock, dictionary, 0f, 0f, 0f);
					}
				}
			}
			foreach (KeyValuePair<string, ConfigEntityAbilityEntry> ability2 in avatarActor.config.Abilities)
			{
				if (!dictionary[ability2.Key] && ability2.Value.AbilityName != "Noop")
				{
					avatarActor.CreateAppliedAbility(AbilityData.GetAbilityConfig(ability2.Value.AbilityName, ability2.Value.AbilityOverride));
				}
			}
		}

		private static bool CheckUnlockBySkillID(ConfigAvatarAbilityUnlock unlockConfig, AvatarDataItem avatarDataItem, bool useLeaderSkill)
		{
			for (int i = 0; i < avatarDataItem.skillDataList.Count; i++)
			{
				AvatarSkillDataItem avatarSkillDataItem = avatarDataItem.skillDataList[i];
				if (avatarSkillDataItem.skillID == unlockConfig.UnlockBySkillID && avatarSkillDataItem.UnLocked)
				{
					if (avatarSkillDataItem.IsLeaderSkill)
					{
						return useLeaderSkill;
					}
					return true;
				}
			}
			return false;
		}

		private static bool CheckUnlockBySubSkillIDAndAddParam(ConfigAvatarAbilityUnlock unlockConfig, AvatarDataItem avatarDataItem, out AvatarSubSkillDataItem outSubSkillItem, bool useLeaderSkill)
		{
			for (int i = 0; i < avatarDataItem.skillDataList.Count; i++)
			{
				AvatarSkillDataItem avatarSkillDataItem = avatarDataItem.skillDataList[i];
				if (avatarSkillDataItem.IsLeaderSkill && !useLeaderSkill)
				{
					continue;
				}
				for (int j = 0; j < avatarSkillDataItem.avatarSubSkillList.Count; j++)
				{
					AvatarSubSkillDataItem avatarSubSkillDataItem = avatarSkillDataItem.avatarSubSkillList[j];
					if (avatarSubSkillDataItem.subSkillID == unlockConfig.UnlockBySubSkillID && avatarSubSkillDataItem.UnLocked)
					{
						outSubSkillItem = avatarSubSkillDataItem;
						return true;
					}
				}
			}
			outSubSkillItem = null;
			return false;
		}

		private static void AddUnlockedAbility(AvatarActor avatarActor, ConfigAvatarAbilityUnlock unlockConfig, Dictionary<string, bool> defaultReplaceMap, float skillParam1 = 0f, float skillParam2 = 0f, float skillParam3 = 0f)
		{
			ConfigAbility abilityConfig = AbilityData.GetAbilityConfig(unlockConfig.AbilityName, unlockConfig.AbilityOverride);
			Dictionary<string, object> dictionary = null;
			bool flag = false;
			for (int i = 0; i < avatarActor.appliedAbilities.Count; i++)
			{
				if (avatarActor.appliedAbilities[i].Item1.AbilityName == unlockConfig.AbilityName)
				{
					dictionary = avatarActor.appliedAbilities[i].Item2;
					if (avatarActor.appliedAbilities[i].Item1 != abilityConfig)
					{
						avatarActor.appliedAbilities[i] = Tuple.Create(abilityConfig, dictionary);
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				dictionary = avatarActor.CreateAppliedAbility(abilityConfig);
				if (unlockConfig.AbilityReplaceID != null)
				{
					defaultReplaceMap[unlockConfig.AbilityReplaceID] = true;
					avatarActor.abilityIDMap[unlockConfig.AbilityReplaceID] = unlockConfig.AbilityName;
				}
			}
			if (unlockConfig.ParamSpecial1 != null)
			{
				AbilityData.SetupParamSpecial(abilityConfig, dictionary, unlockConfig.ParamSpecial1, unlockConfig.ParamMethod1, skillParam1);
			}
			if (unlockConfig.ParamSpecial2 != null)
			{
				AbilityData.SetupParamSpecial(abilityConfig, dictionary, unlockConfig.ParamSpecial2, unlockConfig.ParamMethod2, skillParam2);
			}
			if (unlockConfig.ParamSpecial3 != null)
			{
				AbilityData.SetupParamSpecial(abilityConfig, dictionary, unlockConfig.ParamSpecial3, unlockConfig.ParamMethod3, skillParam3);
			}
		}
	}
}
