using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	[GeneratePartialHash]
	public class ConfigAvatar : ConfigAnimatorEntity, IHashable, IEntityConfig, IOnLoaded
	{
		public ConfigAvatarCommonArguments CommonArguments;

		public ConfigAvatarStateMachinePattern StateMachinePattern;

		public AvatarAttackTargetSelect AttackTargetSelectPattern;

		public ConfigAvatarAIArguments AIArguments;

		public Dictionary<string, ConfigAbilityPropertyEntry> EntityProperties = new Dictionary<string, ConfigAbilityPropertyEntry>();

		public ConfigAvatarAbilityUnlock[] AbilitiesUnlock = ConfigAvatarAbilityUnlock.EMTPY;

		public Dictionary<string, ConfigEntityAbilityEntry> Abilities = new Dictionary<string, ConfigEntityAbilityEntry>();

		public Dictionary<string, ConfigAvatarSkill> Skills = new Dictionary<string, ConfigAvatarSkill>();

		public Dictionary<string, ConfigNamedState> NamedStates = new Dictionary<string, ConfigNamedState>();

		public Dictionary<string, ConfigAvatarAnimEvent> AnimEvents = new Dictionary<string, ConfigAvatarAnimEvent>();

		public Dictionary<string, ConfigMultiAnimEvent> MultiAnimEvents = new Dictionary<string, ConfigMultiAnimEvent>();

		public Dictionary<AvatarCinemaType, string> CinemaPaths = new Dictionary<AvatarCinemaType, string>();

		public ConfigDebuffResistance DebuffResistance = new ConfigDebuffResistance();

		public ConfigLevelEndAnimation LevelEndAnimation = new ConfigLevelEndAnimation();

		public ConfigStoryStateSetting StoryCameraSetting = new ConfigStoryStateSetting();

		[NonSerialized]
		[ShowInInspector]
		public Dictionary<int, string> StateToSkillIDMap;

		[NonSerialized]
		[ShowInInspector]
		public Dictionary<int, string> StateToNamedStateMap;

		ConfigEntityAnimEvent IEntityConfig.TryGetAnimEvent(string animEventID)
		{
			ConfigAvatarAnimEvent value;
			AnimEvents.TryGetValue(animEventID, out value);
			return value;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (CommonArguments != null)
			{
				HashUtils.ContentHashOnto(CommonArguments.GoodsAttractRadius, ref lastHash);
				if (CommonArguments.MaskedSkillButtons != null)
				{
					string[] maskedSkillButtons = CommonArguments.MaskedSkillButtons;
					foreach (string value in maskedSkillButtons)
					{
						HashUtils.ContentHashOnto(value, ref lastHash);
					}
				}
				HashUtils.ContentHashOnto(CommonArguments.SwitchInCD, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.QTESwitchInCDRatio, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.AttackSPRecoverRatio, ref lastHash);
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
					foreach (string value2 in preloadEffectPatternGroups)
					{
						HashUtils.ContentHashOnto(value2, ref lastHash);
					}
				}
				if (CommonArguments.RequestSoundBankNames != null)
				{
					string[] requestSoundBankNames = CommonArguments.RequestSoundBankNames;
					foreach (string value3 in requestSoundBankNames)
					{
						HashUtils.ContentHashOnto(value3, ref lastHash);
					}
				}
				if (CommonArguments.EffectPredicates != null)
				{
					string[] effectPredicates = CommonArguments.EffectPredicates;
					foreach (string value4 in effectPredicates)
					{
						HashUtils.ContentHashOnto(value4, ref lastHash);
					}
				}
				HashUtils.ContentHashOnto(CommonArguments.HasLowPrefab, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CameraMinAngleRatio, ref lastHash);
			}
			if (StateMachinePattern != null)
			{
				HashUtils.ContentHashOnto(StateMachinePattern.IdleCD, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.SwitchInAnimatorStateName, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.SwitchOutAnimatorStateName, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ConstMoveSpeed, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.AniMinSpeedRatio, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.AniMaxSpeedRatio, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.ChangeDirLerpRatioForMove, ref lastHash);
				HashUtils.ContentHashOnto(StateMachinePattern.DefaultAnimDefenceRatio, ref lastHash);
			}
			if (AttackTargetSelectPattern == null || AttackTargetSelectPattern.selectMethod != null)
			{
			}
			if (AIArguments != null)
			{
				HashUtils.ContentHashOnto(AIArguments.AttackDistance, ref lastHash);
				HashUtils.ContentHashOnto(AIArguments.SupporterAI, ref lastHash);
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
			if (AbilitiesUnlock != null)
			{
				ConfigAvatarAbilityUnlock[] abilitiesUnlock = AbilitiesUnlock;
				foreach (ConfigAvatarAbilityUnlock configAvatarAbilityUnlock in abilitiesUnlock)
				{
					HashUtils.ContentHashOnto(configAvatarAbilityUnlock.IsUnlockBySkill, ref lastHash);
					HashUtils.ContentHashOnto(configAvatarAbilityUnlock.UnlockBySkillID, ref lastHash);
					HashUtils.ContentHashOnto(configAvatarAbilityUnlock.UnlockBySubSkillID, ref lastHash);
					HashUtils.ContentHashOnto(configAvatarAbilityUnlock.AbilityName, ref lastHash);
					HashUtils.ContentHashOnto(configAvatarAbilityUnlock.AbilityOverride, ref lastHash);
					HashUtils.ContentHashOnto(configAvatarAbilityUnlock.AbilityReplaceID, ref lastHash);
					HashUtils.ContentHashOnto(configAvatarAbilityUnlock.ParamSpecial1, ref lastHash);
					HashUtils.ContentHashOnto((int)configAvatarAbilityUnlock.ParamMethod1, ref lastHash);
					HashUtils.ContentHashOnto(configAvatarAbilityUnlock.ParamSpecial2, ref lastHash);
					HashUtils.ContentHashOnto((int)configAvatarAbilityUnlock.ParamMethod2, ref lastHash);
					HashUtils.ContentHashOnto(configAvatarAbilityUnlock.ParamSpecial3, ref lastHash);
					HashUtils.ContentHashOnto((int)configAvatarAbilityUnlock.ParamMethod3, ref lastHash);
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
				foreach (KeyValuePair<string, ConfigAvatarSkill> skill in Skills)
				{
					HashUtils.ContentHashOnto(skill.Key, ref lastHash);
					if (skill.Value.AnimatorStateNames != null)
					{
						string[] animatorStateNames = skill.Value.AnimatorStateNames;
						foreach (string value5 in animatorStateNames)
						{
							HashUtils.ContentHashOnto(value5, ref lastHash);
						}
					}
					HashUtils.ContentHashOnto(skill.Value.AnimatorEventPattern, ref lastHash);
					HashUtils.ContentHashOnto((int)skill.Value.SkillType, ref lastHash);
					if (skill.Value.SPCostDelta != null)
					{
						HashUtils.ContentHashOnto(skill.Value.SPCostDelta.isDynamic, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.SPCostDelta.fixedValue, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.SPCostDelta.dynamicKey, ref lastHash);
					}
					if (skill.Value.SkillCDDelta != null)
					{
						HashUtils.ContentHashOnto(skill.Value.SkillCDDelta.isDynamic, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.SkillCDDelta.fixedValue, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.SkillCDDelta.dynamicKey, ref lastHash);
					}
					HashUtils.ContentHashOnto(skill.Value.CanHold, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.MuteHighlighted, ref lastHash);
					if (skill.Value.ChargesCountDelta != null)
					{
						HashUtils.ContentHashOnto(skill.Value.ChargesCountDelta.isDynamic, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.ChargesCountDelta.fixedValue, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.ChargesCountDelta.dynamicKey, ref lastHash);
					}
					HashUtils.ContentHashOnto(skill.Value.HaveBranch, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.IsInstantTrigger, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.InstantTriggerEvent, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.ForceMuteSteer, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.BranchHighlightNormalizedTimeStart, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.BranchHighlightNormalizedTimeStop, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AnimDefenceRatio, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AnimDefenceNormalizedTimeStart, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AnimDefenceNormalizedTimeStop, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.ComboTimerPauseNormalizedTimeStart, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.ComboTimerPauseNormalizedTimeStop, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.LastKillCameraAnimation, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AttackNormalizedTimeStart, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.AttackNormalizedTimeStop, ref lastHash);
					HashUtils.ContentHashOnto((int)skill.Value.EnterSteer, ref lastHash);
					if (skill.Value.EnterSteerOption != null)
					{
						HashUtils.ContentHashOnto((int)skill.Value.EnterSteerOption.SteerType, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.EnterSteerOption.MaxSteeringAngle, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.EnterSteerOption.SteerLerpRatio, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.EnterSteerOption.MaxSteerNormalizedTimeStart, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.EnterSteerOption.MaxSteerNormalizedTimeEnd, ref lastHash);
						HashUtils.ContentHashOnto(skill.Value.EnterSteerOption.MuteSteerWhenNoEnemy, ref lastHash);
					}
					HashUtils.ContentHashOnto(skill.Value.HighSpeedMovement, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.MassRatio, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.NeedClearEffect, ref lastHash);
					HashUtils.ContentHashOnto(skill.Value.MuteCameraControl, ref lastHash);
					HashUtils.ContentHashOnto((int)skill.Value.ReviveCDAction, ref lastHash);
					if (skill.Value.SkillCategoryTag != null)
					{
						AttackResult.AttackCategoryTag[] skillCategoryTag = skill.Value.SkillCategoryTag;
						foreach (AttackResult.AttackCategoryTag value6 in skillCategoryTag)
						{
							HashUtils.ContentHashOnto((int)value6, ref lastHash);
						}
					}
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
						foreach (string value7 in animatorStateNames2)
						{
							HashUtils.ContentHashOnto(value7, ref lastHash);
						}
					}
					HashUtils.ContentHashOnto(namedState.Value.HighSpeedMovement, ref lastHash);
				}
			}
			if (AnimEvents != null)
			{
				foreach (KeyValuePair<string, ConfigAvatarAnimEvent> animEvent in AnimEvents)
				{
					HashUtils.ContentHashOnto(animEvent.Key, ref lastHash);
					if (animEvent.Value.PhysicsProperty != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.PhysicsProperty.IsFreezeDirection, ref lastHash);
					}
					if (animEvent.Value.CameraAction != null)
					{
					}
					if (animEvent.Value.LastKillCameraAnimation != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.LastKillCameraAnimation.AnimationName, ref lastHash);
					}
					if (animEvent.Value.WitchTimeResume != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.WitchTimeResume.ResumeTime, ref lastHash);
					}
					if (animEvent.Value.MissionSpecificKill != null)
					{
						HashUtils.ContentHashOnto(animEvent.Value.MissionSpecificKill.FinishParaInt, ref lastHash);
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
							foreach (AttackResult.AttackCategoryTag value8 in categoryTag)
							{
								HashUtils.ContentHashOnto((int)value8, ref lastHash);
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
						foreach (string value9 in animEventNames)
						{
							HashUtils.ContentHashOnto(value9, ref lastHash);
						}
					}
				}
			}
			if (CinemaPaths != null)
			{
				foreach (KeyValuePair<AvatarCinemaType, string> cinemaPath in CinemaPaths)
				{
					HashUtils.ContentHashOnto((int)cinemaPath.Key, ref lastHash);
					HashUtils.ContentHashOnto(cinemaPath.Value, ref lastHash);
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
			if (LevelEndAnimation != null)
			{
				HashUtils.ContentHashOnto(LevelEndAnimation.LevelWinAnim, ref lastHash);
				HashUtils.ContentHashOnto(LevelEndAnimation.LevelLoseAnim, ref lastHash);
			}
			if (StoryCameraSetting != null)
			{
				HashUtils.ContentHashOnto(StoryCameraSetting.anchorRadius, ref lastHash);
				HashUtils.ContentHashOnto(StoryCameraSetting.yaw, ref lastHash);
				HashUtils.ContentHashOnto(StoryCameraSetting.pitch, ref lastHash);
				HashUtils.ContentHashOnto(StoryCameraSetting.yOffset, ref lastHash);
				HashUtils.ContentHashOnto(StoryCameraSetting.xOffset, ref lastHash);
				HashUtils.ContentHashOnto(StoryCameraSetting.fov, ref lastHash);
				HashUtils.ContentHashOnto(StoryCameraSetting.screenZOffset, ref lastHash);
				HashUtils.ContentHashOnto(StoryCameraSetting.screenYOffset, ref lastHash);
				HashUtils.ContentHashOnto(StoryCameraSetting.screenXOffset, ref lastHash);
				HashUtils.ContentHashOnto(StoryCameraSetting.screenScale, ref lastHash);
			}
			if (AnimatorStateParamBinds != null)
			{
				ConfigBindAnimatorStateToParameter[] animatorStateParamBinds = AnimatorStateParamBinds;
				foreach (ConfigBindAnimatorStateToParameter configBindAnimatorStateToParameter in animatorStateParamBinds)
				{
					if (configBindAnimatorStateToParameter.AnimatorStateNames != null)
					{
						string[] animatorStateNames3 = configBindAnimatorStateToParameter.AnimatorStateNames;
						foreach (string value10 in animatorStateNames3)
						{
							HashUtils.ContentHashOnto(value10, ref lastHash);
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
				foreach (string value11 in muteSyncAnimatorTags)
				{
					HashUtils.ContentHashOnto(value11, ref lastHash);
				}
			}
		}

		public void OnLoaded()
		{
			if (MPArguments == null)
			{
				MPArguments = MPData.AVATAR_DEFAULT_MP_SETTINGS;
			}
			CommonConfig = new ConfigCommonEntity
			{
				CommonArguments = CommonArguments,
				EntityProperties = EntityProperties,
				MPArguments = MPArguments
			};
		}

		public override void OnLevelLoaded()
		{
			base.OnLevelLoaded();
			StateToSkillIDMap = new Dictionary<int, string>();
			StateToNamedStateMap = new Dictionary<int, string>();
			foreach (KeyValuePair<string, ConfigAvatarSkill> skill in Skills)
			{
				string key = skill.Key;
				ConfigAvatarSkill value = skill.Value;
				for (int i = 0; i < value.AnimatorStateNames.Length; i++)
				{
					int key2 = Animator.StringToHash(value.AnimatorStateNames[i]);
					StateToSkillIDMap.Add(key2, key);
				}
			}
			foreach (KeyValuePair<string, ConfigNamedState> namedState in NamedStates)
			{
				string key3 = namedState.Key;
				ConfigNamedState value2 = namedState.Value;
				for (int j = 0; j < value2.AnimatorStateNames.Length; j++)
				{
					int key4 = Animator.StringToHash(value2.AnimatorStateNames[j]);
					StateToNamedStateMap.Add(key4, key3);
				}
			}
			StateMachinePattern.SwitchInAnimatorStateHash = Animator.StringToHash(StateMachinePattern.SwitchInAnimatorStateName);
			StateMachinePattern.SwitchOutAnimatorStateHash = Animator.StringToHash(StateMachinePattern.SwitchOutAnimatorStateName);
		}
	}
}
