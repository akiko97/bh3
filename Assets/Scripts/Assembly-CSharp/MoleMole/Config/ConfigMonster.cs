using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	[GeneratePartialHash]
	public class ConfigMonster : ConfigAnimatorEntity, IHashable, IEntityConfig, IOnLoaded
	{
		public static Dictionary<string, ConfigAbilityPropertyEntry> EMTPY_PROPERTIES = new Dictionary<string, ConfigAbilityPropertyEntry>();

		public ConfigMonsterCommonArguments CommonArguments;

		public ConfigMonsterStateMachinePattern StateMachinePattern;

		public ConfigDynamicArguments DynamicArguments = new ConfigDynamicArguments();

		public ConfigMonsterEliteArguments EliteArguments;

		public ConfigMonsterAIArguments AIArguments;

		public Dictionary<string, ConfigAbilityPropertyEntry> EntityProperties = EMTPY_PROPERTIES;

		public Dictionary<string, ConfigEntityAbilityEntry> Abilities = new Dictionary<string, ConfigEntityAbilityEntry>();

		public Dictionary<string, ConfigMonsterSkill> Skills = new Dictionary<string, ConfigMonsterSkill>();

		public Dictionary<string, ConfigNamedState> NamedStates = new Dictionary<string, ConfigNamedState>();

		public Dictionary<string, ConfigMonsterAnimEvent> AnimEvents = new Dictionary<string, ConfigMonsterAnimEvent>();

		public Dictionary<string, ConfigMultiAnimEvent> MultiAnimEvents = new Dictionary<string, ConfigMultiAnimEvent>();

		public Dictionary<string, bool> AnimatorConfig = new Dictionary<string, bool>();

		public List<List<string>> ATKRatioNames = new List<List<string>>();

		public ConfigDebuffResistance DebuffResistance = new ConfigDebuffResistance();

		[NonSerialized]
		[ShowInInspector]
		public Dictionary<int, string> StateToSkillIDMap;

		[NonSerialized]
		[ShowInInspector]
		public Dictionary<int, string> StateToNamedStateMap;

		ConfigEntityAnimEvent IEntityConfig.TryGetAnimEvent(string animEventID)
		{
			ConfigMonsterAnimEvent value;
			AnimEvents.TryGetValue(animEventID, out value);
			return value;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (CommonArguments != null)
			{
				HashUtils.ContentHashOnto(CommonArguments.HP, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.Attack, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.Defence, ref lastHash);
				if (CommonArguments.FadeInHeight.HasValue)
				{
					HashUtils.ContentHashOnto(CommonArguments.FadeInHeight.Value, ref lastHash);
				}
				HashUtils.ContentHashOnto(CommonArguments.BePushedSpeedRatio, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.BePushedSpeedRatioThrow, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.HitboxInactiveDelay, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.UseTransparentShaderDistanceThreshold, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.UseSwitchShader, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.UseEliteShader, ref lastHash);
				HashUtils.ContentHashOnto((int)CommonArguments.Nature, ref lastHash);
				HashUtils.ContentHashOnto((int)CommonArguments.Class, ref lastHash);
				HashUtils.ContentHashOnto((int)CommonArguments.RoleName, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.DefaultAnimEventPredicate, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CreatePosYOffset, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CreateCollisionRadius, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CreateCollisionHeight, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CollisionLevel, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CollisionRadius, ref lastHash);
				if (CommonArguments.PreloadEffectPatternGroups != null)
				{
					string[] preloadEffectPatternGroups = CommonArguments.PreloadEffectPatternGroups;
					foreach (string value in preloadEffectPatternGroups)
					{
						HashUtils.ContentHashOnto(value, ref lastHash);
					}
				}
				if (CommonArguments.RequestSoundBankNames != null)
				{
					string[] requestSoundBankNames = CommonArguments.RequestSoundBankNames;
					foreach (string value2 in requestSoundBankNames)
					{
						HashUtils.ContentHashOnto(value2, ref lastHash);
					}
				}
				if (CommonArguments.EffectPredicates != null)
				{
					string[] effectPredicates = CommonArguments.EffectPredicates;
					foreach (string value3 in effectPredicates)
					{
						HashUtils.ContentHashOnto(value3, ref lastHash);
					}
				}
				HashUtils.ContentHashOnto(CommonArguments.HasLowPrefab, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CameraMinAngleRatio, ref lastHash);
			}
			if (StateMachinePattern != null)
			{
				HashUtils.ContentHashOnto(StateMachinePattern.AIMode, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ThrowAnimDefenceRatio, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ThrowDieEffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.FastDieEffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.FastDieAnimationWaitDuration, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ThrowUpNamedState, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ThrowUpNamedStateRetreatStopNormalizedTime, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ThrowBlowNamedState, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ThrowBlowDieNamedState, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ThrowBlowAirNamedState, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ThrowBlowAirNamedStateRetreatStopNormalizedTime, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.RetreatToVelocityScaleRatio, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.RetreatBlowVelocityRatio, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.HeavyRetreatThreshold, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.UseRandomLeftRightHitEffectAsNormal, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.UseBackHitAngleCheck, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.BackHitDegreeThreshold, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.UseLeftRightHitAngleCheck, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.LeftRightHitAngleRange, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.UseStandByWalkSteer, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.WalkSteerTimeThreshold, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.WalkSteerAnimatorStateName, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.KeepHitboxStanding, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.KeepHitboxStandingMinHeight, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.UseAbsMoveSpeed, ref lastHash);
				if (StateMachinePattern.BeHitEffect != null)
				{
					HashUtils.ContentHashOnto(StateMachinePattern.BeHitEffect.EffectPattern, ref lastHash);
					HashUtils.ContentHashOnto(StateMachinePattern.BeHitEffect.SwitchName, ref lastHash);
					HashUtils.ContentHashOnto(StateMachinePattern.BeHitEffect.MuteAttackEffect, ref lastHash);
					HashUtils.ContentHashOnto((int)StateMachinePattern.BeHitEffect.AttackEffectTriggerPos, ref lastHash);
				}
				if (StateMachinePattern.BeHitEffectMid != null)
				{
					HashUtils.ContentHashOnto(StateMachinePattern.BeHitEffectMid.EffectPattern, ref lastHash);
					HashUtils.ContentHashOnto(StateMachinePattern.BeHitEffectMid.SwitchName, ref lastHash);
					HashUtils.ContentHashOnto(StateMachinePattern.BeHitEffectMid.MuteAttackEffect, ref lastHash);
					HashUtils.ContentHashOnto((int)StateMachinePattern.BeHitEffectMid.AttackEffectTriggerPos, ref lastHash);
				}
				if (StateMachinePattern.BeHitEffectBig != null)
				{
					HashUtils.ContentHashOnto(StateMachinePattern.BeHitEffectBig.EffectPattern, ref lastHash);
					HashUtils.ContentHashOnto(StateMachinePattern.BeHitEffectBig.SwitchName, ref lastHash);
					HashUtils.ContentHashOnto(StateMachinePattern.BeHitEffectBig.MuteAttackEffect, ref lastHash);
					HashUtils.ContentHashOnto((int)StateMachinePattern.BeHitEffectBig.AttackEffectTriggerPos, ref lastHash);
				}
				HashUtils.ContentHashOnto(StateMachinePattern.DieAnimEventID, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ConstMoveSpeed, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.AniMinSpeedRatio, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.AniMaxSpeedRatio, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ChangeDirLerpRatioForMove, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.DefaultAnimDefenceRatio, ref lastHash);
			}
			if (DynamicArguments != null)
			{
				foreach (KeyValuePair<string, object> dynamicArgument in DynamicArguments)
				{
					HashUtils.ContentHashOnto(dynamicArgument.Key, ref lastHash);
					HashUtils.ContentHashOntoFallback(dynamicArgument.Value, ref lastHash);
				}
			}
			if (EliteArguments != null)
			{
				HashUtils.ContentHashOnto(EliteArguments.HPRatio, ref lastHash);
				HashUtils.ContentHashOnto(EliteArguments.DefenseRatio, ref lastHash);
				HashUtils.ContentHashOnto(EliteArguments.AttackRatio, ref lastHash);
				HashUtils.ContentHashOnto(EliteArguments.DebuffResistanceRatio, ref lastHash);
				HashUtils.ContentHashOnto(EliteArguments.HexColorElite1, ref lastHash);
				HashUtils.ContentHashOnto(EliteArguments.EliteEmissionScaler1, ref lastHash);
				HashUtils.ContentHashOnto(EliteArguments.EliteNormalDisplacement1, ref lastHash);
				HashUtils.ContentHashOnto(EliteArguments.HexColorElite2, ref lastHash);
				HashUtils.ContentHashOnto(EliteArguments.EliteEmissionScaler2, ref lastHash);
				HashUtils.ContentHashOnto(EliteArguments.EliteNormalDisplacement2, ref lastHash);
			}
			if (AIArguments != null)
			{
				HashUtils.ContentHashOnto(AIArguments.AttackRange, ref lastHash);
			}
			if (EntityProperties != null)
			{
				foreach (KeyValuePair<string, ConfigAbilityPropertyEntry> entityProperty in EntityProperties)
				{
					HashUtils.ContentHashOnto(entityProperty.Key, ref lastHash);
					HashUtils.ContentHashOnto((int)entityProperty.Value.Type, ref lastHash);
					HashUtils.ContentHashOnto(entityProperty.Value.Default, ref lastHash);
					HashUtils.ContentHashOnto(entityProperty.Value.Ceiling, ref lastHash);
					HashUtils.ContentHashOnto(entityProperty.Value.Floor, ref lastHash);
					HashUtils.ContentHashOnto((int)entityProperty.Value.Stacking, ref lastHash);
				}
			}
			if (Abilities != null)
			{
				foreach (KeyValuePair<string, ConfigEntityAbilityEntry> ability in Abilities)
				{
					HashUtils.ContentHashOnto(ability.Key, ref lastHash);
					HashUtils.ContentHashOnto(ability.Value.AbilityName, ref lastHash);
					HashUtils.ContentHashOnto(ability.Value.AbilityOverride, ref lastHash);
				}
			}
			if (Skills != null)
			{
				foreach (KeyValuePair<string, ConfigMonsterSkill> skill in Skills)
				{
					HashUtils.ContentHashOnto(skill.Key, ref lastHash);
					if (skill.Value.AnimatorStateNames != null)
					{
						string[] animatorStateNames = skill.Value.AnimatorStateNames;
						foreach (string value4 in animatorStateNames)
						{
							HashUtils.ContentHashOnto(value4, ref lastHash);
						}
					}
					HashUtils.ContentHashOnto(skill.Value.AnimatorEventPattern, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AnimDefenceRatio, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AnimDefenceNormalizedTimeStart, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AnimDefenceNormalizedTimeStop, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AttackNormalizedTimeStart, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AttackNormalizedTimeStop, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.HighSpeedMovement, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.SteerToTargetOnEnter, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.MassRatio, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.NeedClearEffect, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.Unselectable, ref lastHash);
				}
			}
			if (NamedStates != null)
			{
				foreach (KeyValuePair<string, ConfigNamedState> namedState in NamedStates)
				{
					HashUtils.ContentHashOnto(namedState.Key, ref lastHash);
					if (namedState.Value.AnimatorStateNames != null)
					{
						string[] animatorStateNames2 = namedState.Value.AnimatorStateNames;
						foreach (string value5 in animatorStateNames2)
						{
							HashUtils.ContentHashOnto(value5, ref lastHash);
						}
					}
					HashUtils.ContentHashOnto(namedState.Value.HighSpeedMovement, ref lastHash);
				}
			}
			if (AnimEvents != null)
			{
				foreach (KeyValuePair<string, ConfigMonsterAnimEvent> animEvent in AnimEvents)
				{
					HashUtils.ContentHashOnto(animEvent.Key, ref lastHash);
					if (animEvent.Value.AttackHint != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.AttackHint.InnerStartDelay, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackHint.InnerInflateDuration, ref lastHash);
						HashUtils.ContentHashOnto((int)animEvent.Value.AttackHint.OffsetBase, ref lastHash);
					}
					if (animEvent.Value.PhysicsProperty != null)
					{
					}
					HashUtils.ContentHashOnto(animEvent.Value.Predicate, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.Predicate2, ref lastHash);
					if (animEvent.Value.AttackPattern != null)
					{
					}
					if (animEvent.Value.AttackProperty != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.DamagePercentage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.AddedDamageValue, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.NormalDamage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.NormalDamagePercentage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.FireDamage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.FireDamagePercentage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.ThunderDamage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.ThunderDamagePercentage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.IceDamage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.IceDamagePercentage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.AlienDamage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.AlienDamagePercentage, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.AniDamageRatio, ref lastHash);
						HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.HitType, ref lastHash);
						HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.HitEffect, ref lastHash);
						HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.HitEffectAux, ref lastHash);
						HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.KillEffect, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.FrameHalt, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.RetreatVelocity, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.IsAnimEventAttack, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.IsInComboCount, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.SPRecover, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.WitchTimeRatio, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.NoTriggerEvadeAndDefend, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.NoBreakFrameHaltAdd, ref lastHash);
						HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.AttackTargetting, ref lastHash);
						if (animEvent.Value.AttackProperty.CategoryTag != null)
						{
							AttackResult.AttackCategoryTag[] categoryTag = animEvent.Value.AttackProperty.CategoryTag;
							foreach (AttackResult.AttackCategoryTag value6 in categoryTag)
							{
								HashUtils.ContentHashOnto((int)value6, ref lastHash);
							}
						}
					}
					if (animEvent.Value.CameraShake != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeOnNotHit, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeRange, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeTime, ref lastHash);
						if (animEvent.Value.CameraShake.ShakeAngle.HasValue)
						{
							HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeAngle.Value, ref lastHash);
						}
						HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeStepFrame, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ClearPreviousShake, ref lastHash);
					}
					if (animEvent.Value.AttackEffect != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.AttackEffect.EffectPattern, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackEffect.SwitchName, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.AttackEffect.MuteAttackEffect, ref lastHash);
						HashUtils.ContentHashOnto((int)animEvent.Value.AttackEffect.AttackEffectTriggerPos, ref lastHash);
					}
					if (animEvent.Value.TriggerAbility != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.TriggerAbility.ID, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.TriggerAbility.Name, ref lastHash);
					}
					if (animEvent.Value.TriggerEffectPattern != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.TriggerEffectPattern.EffectPattern, ref lastHash);
					}
					if (animEvent.Value.TimeSlow != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.TimeSlow.Force, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.TimeSlow.Duration, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.TimeSlow.SlowRatio, ref lastHash);
					}
					if (animEvent.Value.TriggerTintCamera != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.TriggerTintCamera.RenderDataName, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.TriggerTintCamera.Duration, ref lastHash);
						HashUtils.ContentHashOnto(animEvent.Value.TriggerTintCamera.TransitDuration, ref lastHash);
					}
				}
			}
			if (MultiAnimEvents != null)
			{
				foreach (KeyValuePair<string, ConfigMultiAnimEvent> multiAnimEvent in MultiAnimEvents)
				{
					HashUtils.ContentHashOnto(multiAnimEvent.Key, ref lastHash);
					if (multiAnimEvent.Value.AnimEventNames != null)
					{
						string[] animEventNames = multiAnimEvent.Value.AnimEventNames;
						foreach (string value7 in animEventNames)
						{
							HashUtils.ContentHashOnto(value7, ref lastHash);
						}
					}
				}
			}
			if (AnimatorConfig != null)
			{
				foreach (KeyValuePair<string, bool> item in AnimatorConfig)
				{
					HashUtils.ContentHashOnto(item.Key, ref lastHash);
					HashUtils.ContentHashOnto(item.Value, ref lastHash);
				}
			}
			if (ATKRatioNames != null)
			{
				foreach (List<string> aTKRatioName in ATKRatioNames)
				{
					if (aTKRatioName == null)
					{
						continue;
					}
					foreach (string item2 in aTKRatioName)
					{
						HashUtils.ContentHashOnto(item2, ref lastHash);
					}
				}
			}
			if (DebuffResistance != null)
			{
				if (DebuffResistance.ImmuneStates != null)
				{
					foreach (AbilityState immuneState in DebuffResistance.ImmuneStates)
					{
						HashUtils.ContentHashOnto((int)immuneState, ref lastHash);
					}
				}
				HashUtils.ContentHashOnto(DebuffResistance.ResistanceRatio, ref lastHash);
				HashUtils.ContentHashOnto(DebuffResistance.DurationRatio, ref lastHash);
			}
			if (AnimatorStateParamBinds != null)
			{
				ConfigBindAnimatorStateToParameter[] animatorStateParamBinds = AnimatorStateParamBinds;
				foreach (ConfigBindAnimatorStateToParameter configBindAnimatorStateToParameter in animatorStateParamBinds)
				{
					if (configBindAnimatorStateToParameter.AnimatorStateNames != null)
					{
						string[] animatorStateNames3 = configBindAnimatorStateToParameter.AnimatorStateNames;
						foreach (string value8 in animatorStateNames3)
						{
							HashUtils.ContentHashOnto(value8, ref lastHash);
						}
					}
					if (configBindAnimatorStateToParameter.ParameterConfig != null)
					{
						HashUtils.ContentHashOnto(configBindAnimatorStateToParameter.ParameterConfig.ParameterID, ref lastHash);
						HashUtils.ContentHashOnto(configBindAnimatorStateToParameter.ParameterConfig.ParameterIDSub, ref lastHash);
						HashUtils.ContentHashOnto(configBindAnimatorStateToParameter.ParameterConfig.NormalizedTimeStart, ref lastHash);
						HashUtils.ContentHashOnto(configBindAnimatorStateToParameter.ParameterConfig.NormalizedTimeStop, ref lastHash);
					}
				}
			}
			if (MPArguments == null)
			{
				return;
			}
			HashUtils.ContentHashOnto(MPArguments.SyncSendInterval, ref lastHash);
			HashUtils.ContentHashOnto((int)MPArguments.RemoteMode, ref lastHash);
			if (MPArguments.MuteSyncAnimatorTags != null)
			{
				string[] muteSyncAnimatorTags = MPArguments.MuteSyncAnimatorTags;
				foreach (string value9 in muteSyncAnimatorTags)
				{
					HashUtils.ContentHashOnto(value9, ref lastHash);
				}
			}
		}

		public void OnLoaded()
		{
			if (AIArguments == null)
			{
				AIArguments = new ConfigMonsterAIArguments();
			}
			if (MPArguments == null)
			{
				MPArguments = MPData.MONSTER_DEFAULT_MP_SETTINGS;
			}
			CommonConfig = new ConfigCommonEntity
			{
				EntityProperties = EntityProperties,
				CommonArguments = CommonArguments,
				MPArguments = MPArguments
			};
		}

		public override void OnLevelLoaded()
		{
			base.OnLevelLoaded();
			StateToSkillIDMap = new Dictionary<int, string>();
			StateToNamedStateMap = new Dictionary<int, string>();
			foreach (KeyValuePair<string, ConfigMonsterSkill> skill in Skills)
			{
				string key = skill.Key;
				ConfigMonsterSkill value = skill.Value;
				if (value.AnimatorStateNames != null)
				{
					int i = 0;
					for (int num = value.AnimatorStateNames.Length; i < num; i++)
					{
						StateToSkillIDMap.Add(Animator.StringToHash(value.AnimatorStateNames[i]), key);
					}
				}
			}
			foreach (KeyValuePair<string, ConfigNamedState> namedState in NamedStates)
			{
				string key2 = namedState.Key;
				ConfigNamedState value2 = namedState.Value;
				for (int j = 0; j < value2.AnimatorStateNames.Length; j++)
				{
					int key3 = Animator.StringToHash(value2.AnimatorStateNames[j]);
					StateToNamedStateMap.Add(key3, key2);
				}
			}
			ColorUtility.TryParseHtmlString(EliteArguments.HexColorElite1, out EliteArguments.EliteColor1);
			ColorUtility.TryParseHtmlString(EliteArguments.HexColorElite2, out EliteArguments.EliteColor2);
		}
	}
}
