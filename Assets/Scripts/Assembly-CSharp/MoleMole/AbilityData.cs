using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class AbilityData
	{
		public const AbilityState ABILITY_STATE_DEBUFF = AbilityState.Bleed | AbilityState.Stun | AbilityState.Paralyze | AbilityState.Burn | AbilityState.Poisoned | AbilityState.Frozen | AbilityState.MoveSpeedDown | AbilityState.AttackSpeedDown | AbilityState.Weak | AbilityState.Fragile | AbilityState.TargetLocked | AbilityState.Tied;

		public const AbilityState ABILITY_STATE_BUFF = AbilityState.Endure | AbilityState.MoveSpeedUp | AbilityState.AttackSpeedUp | AbilityState.PowerUp | AbilityState.Shielded | AbilityState.CritUp | AbilityState.Immune | AbilityState.MaxMoveSpeed | AbilityState.Undamagable;

		public const string ENTITY_TIME_SCALE_DELTA = "Entity_TimeScaleDelta";

		public const string ENTITY_ATTACK_SPEED_RATIO = "Entity_AttackSpeed";

		public const string ENTITY_ATTACK_MOVE_RATIO = "Entity_AttackMoveRatio";

		public const string ENTITY_MASS_RATIO = "Entity_MassRatio";

		public const string ANIMATOR_MOVE_SPEED = "Animator_MoveSpeedRatio";

		public const string ANIMATOR_OVERALL_SPEED = "Animator_OverallSpeedRatio";

		public const string ANIMATOR_OVERALL_SPEED_MULTIPLIED = "Animator_OverallSpeedRatioMultiplied";

		public const string ANIMATOR_RIGIDBODY_VELOCITY_RATIO = "Animator_RigidBodyVelocityRatio";

		public const string AI_ATTACK_CD_RATIO = "AI_AttackCDRatio";

		public const string AI_IGNORE_MAX_ATTACK_NUM_CHANCE = "AI_IgnoreMaxAttackNumChance";

		public const string AI_CAN_TELEPORT = "AI_CanTeleport";

		public const string ACTOR_ANI_DAMAGE_DELTA = "Actor_AniDamageDelta";

		public const string ACTOR_ANI_DEFENCE_DELTA = "Actor_AniDefenceDelta";

		public const string ACTOR_THROW_ANI_DEFENCE_DELTA = "Actor_ThrowAniDefenceDelta";

		public const string ACTOR_CRITICAL_RATIO = "Actor_CriticalRatio";

		public const string ACTOR_CRITICAL_DELTA = "Actor_CriticalDelta";

		public const string ACTOR_DEFENCE_RATIO = "Actor_DefenceRatio";

		public const string ACTOR_DEFENCE_DELTA = "Actor_DefenceDelta";

		public const string ACTOR_CRITICAL_CHANCE_DELTA = "Actor_CriticalChanceDelta";

		public const string ACTOR_CRITICAL_DAMAGE_RATIO = "Actor_CriticalDamageRatio";

		public const string ACTOR_SHIELD_DAMAGE_RATIO = "Actor_ShieldDamageRatio";

		public const string ACTOR_SHIELD_DAMAGE_DELTA = "Actor_ShieldDamageDelta";

		public const string ACTOR_MAX_HP_RATIO = "Actor_MaxHPRatio";

		public const string ACTOR_MAX_HP_DELTA = "Actor_MaxHPDelta";

		public const string ACTOR_MAX_SP_RATIO = "Actor_MaxSPRatio";

		public const string ACTOR_MAX_SP_DELTA = "Actor_MaxSPDelta";

		public const string ACTOR_SP_COST_DELTA = "Actor_SkillSPCostDelta";

		public const string ACTOR_SP_COST_RATIO = "Actor_SkillSPCostRatio";

		public const string ACTOR_SP_RECOVER_RATIO = "Actor_SPRecoverRatio";

		public const string ACTOR_RETREAT_RATIO = "Actor_RetreatRatio";

		public const string ACTOR_COMBO_TIMER_RATIO = "Actor_ComboTimerRatio";

		public const string ACTOR_COMBO_TIMER_DELTA = "Actor_ComboTimerDelta";

		public const string ACTOR_DEBUFF_DURATION_RATIO_DELTA = "Actor_DebuffDurationRatioDelta";

		public const string ACTOR_ATTACK_STEAL_HP_RATIO = "Actor_AttackStealHPRatio";

		public const string ACTOR_ATTACK_DELTA = "Actor_AttackDelta";

		public const string ACTOR_ATTACK_RATIO = "Actor_AttackRatio";

		public const string ACTOR_ADDED_DAMAGE_RATIO = "Actor_AddedDamageRatio";

		public const string ACTOR_ADDED_ATTACK_RATIO = "Actor_AddedAttackRatio";

		public const string ACTOR_ADDED_NORMAL_ATTACK_RATIO = "Actor_NormalAttackRatio";

		public const string ACTOR_ADDED_FIRE_ATTACK_RATIO = "Actor_FireAttackRatio";

		public const string ACTOR_ADDED_THUNDER_ATTACK_RATIO = "Actor_ThunderAttackRatio";

		public const string ACTOR_ADDED_ICE_ATTACK_RATIO = "Actor_IceAttackRatio";

		public const string ACTOR_ADDED_ALLIEN_ATTACK_RATIO = "Actor_AllienAttackRatio";

		public const string ACTOR_DAMAGE_REDUCE_RATIO = "Actor_DamageReduceRatio";

		public const string ACTOR_RESIST_ALL_ELEMENT_ATTACK_RATIO = "Actor_ResistAllElementAttackRatio";

		public const string ACTOR_RESIST_NORMAL_ATTACK_RATIO = "Actor_ResistNormalAttackRatio";

		public const string ACTOR_RESIST_FIRE_ATTACK_RATIO = "Actor_ResistFireAttackRatio";

		public const string ACTOR_RESIST_THUNDER_ATTACK_RATIO = "Actor_ResistThunderAttackRatio";

		public const string ACTOR_RESIST_ICE_ATTACK_RATIO = "Actor_ResistIceAttackRatio";

		public const string ACTOR_RESIST_ALLIEN_ATTACK_RATIO = "Actor_ResistAllienAttackRatio";

		public const string ACTOR_ADDED_DAMAGE_TAKE_RATIO = "Actor_DamageTakeRatio";

		public const string ACTOR_ADDED_NORMAL_ATTACK_TAKE_RATIO = "Actor_NormalAttackTakeRatio";

		public const string ACTOR_ADDED_FIRE_ATTACK_TAKE_RATIO = "Actor_FireAttackTakeRatio";

		public const string ACTOR_ADDED_THUNDER_ATTACK_TAKE_RATIO = "Actor_ThunderAttackTakeRatio";

		public const string ACTOR_ADDED_ICE_ATTACK_TAKE_RATIO = "Actor_IceAttackTakeRatio";

		public const string ACTOR_ADDED_ALLIEN_ATTACK_TAKE_RATIO = "Actor_AllienAttackTakeRatio";

		public const string ACTOR_SKL01_CD_RATIO = "Actor_SKL01CDRatio";

		public const string ACTOR_SKL02_CD_RATIO = "Actor_SKL02CDRatio";

		public const string ACTOR_GOODS_ATTRACT_RATIUS = "Actor_GoodsAttrackRadius";

		public const string ACTOR_BE_RETREAT_RATIO = "Actor_BeRetreatRatio";

		public const float SHORT_PARALYZE_RESUME_DURATION = 0.35f;

		public const float LONG_PARALYZE_RESUME_DURATION = 0.5f;

		public const float WITCH_TIME_RESUME_DURATION = 0.5f;

		public const float AI_CAN_TELEPORT_MIN_DISTANCE = 0.5f;

		public static AbilityState[] ABILITY_STATE_CONTROL_DEBUFFS;

		public static AbilityState ABILITY_STATE_CONTROL_DEBUFFS_MASK;

		public static AbilityState[][] ABILITY_STATE_PRECEDENCE_MAP;

		public static AbilityState[] EMPTY;

		public static Dictionary<string, object> EMPTY_OVERRIDE_MAP;

		private static Dictionary<string, ConfigOverrideGroup> _abilityGroupMap;

		private static List<string> _configPathList;

		private static Action<string> _loadJsonConfigCallback;

		private static BackGroundWorker _loadDataBackGroundWorker;

		public static Dictionary<string, ConfigAbilityPropertyEntry> PROPERTIES;

		static AbilityData()
		{
			EMPTY = new AbilityState[0];
			EMPTY_OVERRIDE_MAP = new Dictionary<string, object>();
			_loadJsonConfigCallback = null;
			_loadDataBackGroundWorker = new BackGroundWorker();
			ABILITY_STATE_CONTROL_DEBUFFS = new AbilityState[3]
			{
				AbilityState.Stun,
				AbilityState.Paralyze,
				AbilityState.Frozen
			};
			for (int i = 0; i < ABILITY_STATE_CONTROL_DEBUFFS.Length; i++)
			{
				ABILITY_STATE_CONTROL_DEBUFFS_MASK |= ABILITY_STATE_CONTROL_DEBUFFS[i];
			}
			ABILITY_STATE_PRECEDENCE_MAP = new AbilityState[3][]
			{
				new AbilityState[3]
				{
					AbilityState.Frozen,
					AbilityState.Paralyze,
					AbilityState.Stun
				},
				new AbilityState[2]
				{
					AbilityState.MaxMoveSpeed,
					AbilityState.MoveSpeedUp
				},
				new AbilityState[2]
				{
					AbilityState.MaxMoveSpeed,
					AbilityState.MoveSpeedDown
				}
			};
			PROPERTIES = new Dictionary<string, ConfigAbilityPropertyEntry>();
			DefineEntityProperty("Entity_TimeScaleDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineEntityProperty("Entity_AttackSpeed", 0f, FixedFloatStack.StackMethod.Sum);
			DefineEntityProperty("Entity_AttackMoveRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineEntityProperty("Entity_MassRatio", 0f, FixedFloatStack.StackMethod.Sum, 0f, 100f);
			DefineEntityProperty("Animator_MoveSpeedRatio", 0f, FixedFloatStack.StackMethod.Sum, -0.8f, 1f);
			DefineEntityProperty("Animator_OverallSpeedRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineEntityProperty("Animator_OverallSpeedRatioMultiplied", 1f, FixedFloatStack.StackMethod.Multiplied);
			DefineEntityProperty("AI_AttackCDRatio", 1f, FixedFloatStack.StackMethod.Multiplied);
			DefineEntityProperty("AI_IgnoreMaxAttackNumChance", 0f, FixedFloatStack.StackMethod.Sum);
			DefineEntityProperty("AI_CanTeleport", 0f, FixedFloatStack.StackMethod.Sum);
			DefineEntityProperty("Animator_RigidBodyVelocityRatio", 0f, FixedFloatStack.StackMethod.Sum, -1f, 3f);
			DefineEntityProperty("Actor_GoodsAttrackRadius", 1f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_AniDamageDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_AniDefenceDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_ThrowAniDefenceDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_CriticalDamageRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_DefenceRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_DefenceDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_CriticalChanceDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_CriticalRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_CriticalDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_ShieldDamageRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_ShieldDamageDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_MaxHPRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_MaxHPDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_MaxSPRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_SkillSPCostRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_SkillSPCostDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_MaxSPDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_SPRecoverRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_RetreatRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_ComboTimerRatio", 1f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_ComboTimerDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_AttackStealHPRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_DebuffDurationRatioDelta", 0f, FixedFloatStack.StackMethod.OneMinusMultiplied);
			DefineActorProperty("Actor_AttackDelta", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_AttackRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_AddedDamageRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_AddedAttackRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_NormalAttackRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_FireAttackRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_ThunderAttackRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_IceAttackRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_AllienAttackRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_DamageReduceRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied);
			DefineActorProperty("Actor_ResistAllElementAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied);
			DefineActorProperty("Actor_ResistNormalAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied);
			DefineActorProperty("Actor_ResistFireAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied);
			DefineActorProperty("Actor_ResistThunderAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied);
			DefineActorProperty("Actor_ResistIceAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied);
			DefineActorProperty("Actor_ResistAllienAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied);
			DefineActorProperty("Actor_DamageTakeRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_NormalAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_FireAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_ThunderAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_IceAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_AllienAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_SKL01CDRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_SKL02CDRatio", 0f, FixedFloatStack.StackMethod.Sum);
			DefineActorProperty("Actor_BeRetreatRatio", 0f, FixedFloatStack.StackMethod.Sum);
		}

		public static bool IsModifierDebuff(ConfigAbilityModifier config)
		{
			return config.IsDebuff || (config.State & (AbilityState.Bleed | AbilityState.Stun | AbilityState.Paralyze | AbilityState.Burn | AbilityState.Poisoned | AbilityState.Frozen | AbilityState.MoveSpeedDown | AbilityState.AttackSpeedDown | AbilityState.Weak | AbilityState.Fragile | AbilityState.TargetLocked | AbilityState.Tied)) != 0;
		}

		public static bool IsModifierBuff(ConfigAbilityModifier config)
		{
			return config.IsBuff || (config.State & (AbilityState.Endure | AbilityState.MoveSpeedUp | AbilityState.AttackSpeedUp | AbilityState.PowerUp | AbilityState.Shielded | AbilityState.CritUp | AbilityState.Immune | AbilityState.MaxMoveSpeed | AbilityState.Undamagable)) != 0;
		}

		public static void ReloadFromFile()
		{
			_abilityGroupMap = new Dictionary<string, ConfigOverrideGroup>();
			string[] abilityRegistryPathes = GlobalDataManager.metaConfig.abilityRegistryPathes;
			foreach (string text in abilityRegistryPathes)
			{
				ConfigAbilityRegistry configAbilityRegistry = ConfigUtil.LoadJSONConfig<ConfigAbilityRegistry>(text);
				foreach (ConfigOverrideGroup item in configAbilityRegistry)
				{
					try
					{
						ConfigAbility config = item.GetConfig<ConfigAbility>("Default");
						_abilityGroupMap.Add(config.AbilityName, item);
					}
					catch
					{
						Debug.LogError("Error during loading ability file: " + text);
						throw;
					}
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
		{
			_loadJsonConfigCallback = finishCallback;
			_configPathList = new List<string>();
			_abilityGroupMap = new Dictionary<string, ConfigOverrideGroup>();
			string[] pathes = GlobalDataManager.metaConfig.abilityRegistryPathes;
			if (pathes.Length == 0)
			{
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("AbilityData");
					_loadJsonConfigCallback = null;
				}
				yield break;
			}
			string[] array = pathes;
			foreach (string abilityRegistryPath in array)
			{
				_configPathList.Add(abilityRegistryPath);
			}
			float step = progressSpan / (float)pathes.Length;
			_loadDataBackGroundWorker.StartBackGroundWork("AbilityData");
			string[] array2 = pathes;
			foreach (string abilityRegistryPath2 in array2)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync(abilityRegistryPath2);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null abilityPath :" + abilityRegistryPath2);
				if (asyncRequest != null)
				{
					yield return asyncRequest.operation;
					if (moveOneStepCallback != null)
					{
						moveOneStepCallback(step);
					}
					ConfigUtil.LoadJSONStrConfigMultiThread<ConfigAbilityRegistry>(asyncRequest.asset.ToString(), _loadDataBackGroundWorker, OnLoadOneJsonConfigFinish, abilityRegistryPath2);
				}
			}
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (ConfigOverrideGroup value in _abilityGroupMap.Values)
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

		private static void OnLoadOneJsonConfigFinish(ConfigAbilityRegistry abilityGroupList, string configPath)
		{
			_configPathList.Remove(configPath);
			foreach (ConfigOverrideGroup abilityGroup in abilityGroupList)
			{
				try
				{
					ConfigAbility config = abilityGroup.GetConfig<ConfigAbility>("Default");
					_abilityGroupMap.Add(config.AbilityName, abilityGroup);
				}
				catch
				{
					Debug.LogError("Error during loading ability file: " + configPath);
					throw;
				}
			}
			if (_configPathList.Count == 0)
			{
				_loadDataBackGroundWorker.StopBackGroundWork(false);
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("AbilityData");
					_loadJsonConfigCallback = null;
				}
			}
		}

		public static Dictionary<string, ConfigOverrideGroup> GetAbilityGroupMap()
		{
			return _abilityGroupMap;
		}

		public static ConfigAbility GetAbilityConfig(string abilityName)
		{
			return GetAbilityConfig(abilityName, "Default");
		}

		public static ConfigAbility GetAbilityConfig(string abilityName, string overrideName)
		{
			return _abilityGroupMap[abilityName].GetConfig<ConfigAbility>(overrideName);
		}

		public static string[] GetAllAbilityNames()
		{
			if (_abilityGroupMap == null)
			{
				return new string[0];
			}
			string[] array = new string[_abilityGroupMap.Count];
			_abilityGroupMap.Keys.CopyTo(array, 0);
			return array;
		}

		private static void DefineEntityProperty(string propertyName, float defaultValue, FixedFloatStack.StackMethod valueType, float floor = float.MinValue, float ceiling = float.MaxValue)
		{
			PROPERTIES.Add(propertyName, new ConfigAbilityPropertyEntry
			{
				Type = ConfigAbilityPropertyEntry.PropertyType.Entity,
				Default = defaultValue,
				Stacking = valueType,
				Floor = floor,
				Ceiling = ceiling
			});
		}

		private static void DefineActorProperty(string propertyName, float defaultValue, FixedFloatStack.StackMethod valueType, float floor = float.MinValue, float ceiling = float.MaxValue)
		{
			PROPERTIES.Add(propertyName, new ConfigAbilityPropertyEntry
			{
				Type = ConfigAbilityPropertyEntry.PropertyType.Actor,
				Default = defaultValue,
				Stacking = valueType,
				Floor = floor,
				Ceiling = ceiling
			});
		}

		public static void GetStateIndiceInPrecedenceMap(AbilityState state, out AbilityState[] precedenceTrack, out int stateIx)
		{
			AbilityState[] array = null;
			stateIx = 0;
			for (int i = 0; i < ABILITY_STATE_PRECEDENCE_MAP.Length; i++)
			{
				array = ABILITY_STATE_PRECEDENCE_MAP[i];
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] == state)
					{
						precedenceTrack = array;
						stateIx = j;
						return;
					}
				}
			}
			precedenceTrack = null;
		}

		public static void SetupParamSpecial(ConfigAbility abilityConfig, Dictionary<string, object> overrideMap, string paramSpecial, ParamMethod paramMethod, float paramValue)
		{
			float num = (float)abilityConfig.AbilitySpecials[paramSpecial];
			float num2;
			switch (paramMethod)
			{
			case ParamMethod.Replace:
				num2 = paramValue;
				break;
			case ParamMethod.Add:
				num2 = num + paramValue;
				break;
			case ParamMethod.Minus:
				num2 = num - paramValue;
				break;
			case ParamMethod.OneAddMultipled:
				num2 = num * (1f + paramValue);
				break;
			case ParamMethod.Negative:
				num2 = 0f - paramValue;
				break;
			default:
				num2 = 0f;
				break;
			}
			overrideMap.Add(paramSpecial, num2);
		}
	}
}
