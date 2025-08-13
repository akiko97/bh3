using System;
using System.Collections.Generic;
using System.Diagnostics;
using FullInspector;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class ActorAbilityPlugin : BaseActorPlugin
	{
		public enum OnKillBehavior
		{
			RemoveAll = 0,
			DoNotRemoveUntilDestroyed = 1,
			RemoveAllDebuffsAndDurationed = 2
		}

		public delegate bool AbilityPredicateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt);

		private static ConfigEntityAttackProperty _attackProperty = new ConfigEntityAttackProperty();

		private static Dictionary<string, object> EMPTY_OVERRIDE_MAP = new Dictionary<string, object>();

		protected BaseAbilityActor _owner;

		protected LevelActor _levelActor;

		[ShowInInspector]
		[InspectorCollapsedFoldout]
		protected List<ActorAbility> _appliedAbilities;

		[InspectorCollapsedFoldout]
		[ShowInInspector]
		protected List<ActorModifier> _appliedModifiers;

		private List<ActorModifier> _deadModifiers;

		private Dictionary<string, DisplayValue<float>> _displayValueMap;

		private Dictionary<string, DynamicActorValue<float>> _dynamicValueMap;

		private List<Tuple<ActorModifier, EntityTimer>> _modifierDurationTimers;

		private List<Tuple<ActorModifier, EntityTimer>> _modifierThinkTimers;

		private List<ConfigAbility> _additionalAbilities;

		private bool _isKilled;

		private bool _isMuted;

		private bool _waitForStageReady;

		public OnKillBehavior onKillBehavior;

		public bool muteEvents;

		public bool IsImmuneDebuff;

		protected ActorAbilityPlugin(BaseAbilityActor abilityActor)
		{
			_owner = abilityActor;
			_appliedAbilities = new List<ActorAbility>();
			_appliedModifiers = new List<ActorModifier>();
			_deadModifiers = new List<ActorModifier>();
			_displayValueMap = new Dictionary<string, DisplayValue<float>>();
			_dynamicValueMap = new Dictionary<string, DynamicActorValue<float>>();
			_modifierDurationTimers = new List<Tuple<ActorModifier, EntityTimer>>();
			_modifierThinkTimers = new List<Tuple<ActorModifier, EntityTimer>>();
			_additionalAbilities = new List<ConfigAbility>();
			_levelActor = Singleton<LevelManager>.Instance.levelActor;
		}

		public void FireEffectHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			FireEffect fireEffect = (FireEffect)actionConfig;
			if (fireEffect.OwnedByLevel)
			{
				Singleton<LevelManager>.Instance.levelActor.entity.FireEffect(fireEffect.EffectPattern, target.entity.XZPosition, target.entity.transform.forward);
			}
			else if (target.entity != null && target.entity.gameObject.activeSelf)
			{
				target.entity.FireEffect(fireEffect.EffectPattern);
			}
		}

		public void FireEffectToTargetHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			FireEffectToTarget fireEffectToTarget = (FireEffectToTarget)actionConfig;
			if (instancedAbility.caster.entity != null && target.entity != null)
			{
				if (fireEffectToTarget.Reverse)
				{
					target.entity.FireEffectTo(fireEffectToTarget.EffectPattern, instancedAbility.caster.entity);
				}
				else
				{
					instancedAbility.caster.entity.FireEffectTo(fireEffectToTarget.EffectPattern, target.entity);
				}
			}
		}

		public void ApplyModifierHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ApplyModifier applyModifier = (ApplyModifier)actionConfig;
			if (evt is EvtEvadeSuccess)
			{
				instancedAbility.CurrentTriggerEvent = evt;
			}
			target.abilityPlugin.ApplyModifier(instancedAbility, applyModifier.ModifierName);
		}

		public void RemoveModifierHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			RemoveModifier removeModifier = (RemoveModifier)actionConfig;
			target.abilityPlugin.TryRemoveModifier(instancedAbility, removeModifier.ModifierName);
		}

		public void RemoveUniqueModifierHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			RemoveUniqueModifier removeUniqueModifier = (RemoveUniqueModifier)actionConfig;
			ActorModifier firstUniqueModifier = target.abilityPlugin.GetFirstUniqueModifier(removeUniqueModifier.ModifierName);
			if (firstUniqueModifier != null)
			{
				target.abilityPlugin.TryRemoveModifier(firstUniqueModifier);
			}
		}

		public void AttachModifierHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachModifier attachModifier = (AttachModifier)actionConfig;
			ActorModifier actorModifier = target.abilityPlugin.ApplyModifier(instancedAbility, attachModifier.ModifierName);
			if (actorModifier != null)
			{
				if (instancedModifier != null)
				{
					instancedModifier.AttachModifier(actorModifier);
				}
				else
				{
					instancedAbility.AttachModifier(actorModifier);
				}
			}
		}

		public void AttachEffectHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachEffect attachEffect = (AttachEffect)actionConfig;
			int patternIx = target.entity.AttachEffect(attachEffect.EffectPattern);
			if (instancedModifier != null)
			{
				instancedModifier.AttachEffectPatternIndex(patternIx);
			}
			else
			{
				instancedAbility.AttachEffectPatternIndex(patternIx);
			}
		}

		public void AttachShaderHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachShader attachShader = (AttachShader)actionConfig;
			float enableDuration = ((!attachShader.UsePrefabEnableDurtion) ? instancedAbility.Evaluate(attachShader.Duration) : (-1f));
			target.entity.SetShaderDataLerp(attachShader.ShaderType, true, enableDuration, -1f, attachShader.UseNewTexture);
		}

		public void DetachShaderHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			DetachShader detachShader = (DetachShader)actionConfig;
			float disableDuration = ((!detachShader.UsePrefabDisableDurtion) ? detachShader.Duration : (-1f));
			target.entity.SetShaderDataLerp(detachShader.ShaderType, false, -1f, disableDuration);
		}

		public void AttachAnimEventPredicateHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachAnimEventPredicate attachAnimEventPredicate = (AttachAnimEventPredicate)actionConfig;
			target.entity.AddAnimEventPredicate(attachAnimEventPredicate.AnimEventPredicate);
			if (instancedModifier != null)
			{
				instancedModifier.AttachAnimEventPredicate(target.entity, attachAnimEventPredicate.AnimEventPredicate);
			}
			else
			{
				instancedAbility.AttachAnimEventPredicate(target.entity, attachAnimEventPredicate.AnimEventPredicate);
			}
		}

		public void DetachAnimEventPredicateHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			DetachAnimEventPredicate detachAnimEventPredicate = (DetachAnimEventPredicate)actionConfig;
			target.entity.RemoveAnimEventPredicate(detachAnimEventPredicate.AnimEventPredicate);
			if (instancedModifier != null)
			{
				instancedModifier.DetachAnimEventPredicate(target.entity, detachAnimEventPredicate.AnimEventPredicate);
			}
			else
			{
				instancedAbility.DetachAnimEventPredicate(target.entity, detachAnimEventPredicate.AnimEventPredicate);
			}
		}

		public void HealHPHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			HealHP healHP = (HealHP)actionConfig;
			float num = 0f;
			if (healHP.Amount != null)
			{
				num += instancedAbility.Evaluate(healHP.Amount);
			}
			if (healHP.AmountByCasterMaxHPRatio != null)
			{
				num += (float)instancedAbility.caster.maxHP * instancedAbility.Evaluate(healHP.AmountByCasterMaxHPRatio);
			}
			if (healHP.AmountByTargetMaxHPRatio != null)
			{
				num += (float)target.maxHP * instancedAbility.Evaluate(healHP.AmountByTargetMaxHPRatio);
			}
			if ((bool)target.isAlive || (target.abilityState & AbilityState.Limbo) != AbilityState.None)
			{
				target.HealHP(num * healHP.HealRatio);
				if (!healHP.MuteHealEffect && num > 0f && target != null && target.entity.gameObject.activeSelf)
				{
					target.entity.FireEffect("Ability_HealHP");
				}
			}
		}

		public void HealSPHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			HealSP healSP = (HealSP)actionConfig;
			if ((bool)target.isAlive || (target.abilityState & AbilityState.Limbo) != AbilityState.None)
			{
				float num = instancedAbility.Evaluate(healSP.Amount);
				target.HealSP(num);
				if (!healSP.MuteHealEffect && num > 0f && target != null && target.entity.gameObject.activeSelf)
				{
					target.entity.FireEffect("Ability_HealSP");
				}
			}
		}

		public void MuteAdditiveVelocityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			MuteAdditiveVelocity muteAdditiveVelocity = (MuteAdditiveVelocity)actionConfig;
			if (target != null && (bool)target.isAlive)
			{
				BaseMonoAnimatorEntity baseMonoAnimatorEntity = target.entity as BaseMonoAnimatorEntity;
				if (baseMonoAnimatorEntity != null)
				{
					baseMonoAnimatorEntity.MuteAdditiveVelocity = muteAdditiveVelocity.Mute;
				}
			}
		}

		public void CameraShakeHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ActCameraShake actCameraShake = (ActCameraShake)actionConfig;
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(target.runtimeID))
			{
				AttackPattern.ActCameraShake(actCameraShake.CameraShake);
			}
		}

		public void CameraExposureHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ActCameraExposure actCameraExposure = (ActCameraExposure)actionConfig;
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (mainCamera != null)
			{
				mainCamera.ActExposureEffect(actCameraExposure.ExposureTime, actCameraExposure.KeepTime, actCameraExposure.RecoverTime, actCameraExposure.MaxExposure);
			}
		}

		public void CameraGlareHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ActCameraGlare actCameraGlare = (ActCameraGlare)actionConfig;
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (mainCamera != null)
			{
				mainCamera.ActGlareEffect(actCameraGlare.GlareTime, actCameraGlare.KeepTime, actCameraGlare.RecoverTime, actCameraGlare.TargetRate);
			}
		}

		public void CameraActionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			DoCameraAction doCameraAction = (DoCameraAction)actionConfig;
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(target.runtimeID))
			{
				BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(target.runtimeID);
				avatarByRuntimeID.DoCameraAction(doCameraAction.CameraAction);
			}
		}

		public void ShowLevelDisplayTextHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ShowLevelDisplayText showLevelDisplayText = (ShowLevelDisplayText)actionConfig;
			string text = LocalizationGeneralLogic.GetText(showLevelDisplayText.Text);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLevelDisplayText, text));
		}

		public void SetAIParamHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			SetAIParam setAIParam = (SetAIParam)actionConfig;
			switch (setAIParam.LogicType)
			{
			case ParamLogicType.Replace:
				if (target.entity is BaseMonoMonster)
				{
					BaseMonoMonster baseMonoMonster2 = target.entity as BaseMonoMonster;
					if (baseMonoMonster2.IsAIControllerActive())
					{
						((BTreeMonsterAIController)(target.entity as BaseMonoMonster).GetActiveAIController()).SetBehaviorVariable(instancedAbility.Evaluate(setAIParam.Param), setAIParam.Value);
					}
				}
				break;
			case ParamLogicType.Add:
				if (target.entity is BaseMonoMonster)
				{
					BaseMonoMonster baseMonoMonster = target.entity as BaseMonoMonster;
					if (baseMonoMonster.IsAIControllerActive())
					{
						((BTreeMonsterAIController)(target.entity as BaseMonoMonster).GetActiveAIController()).AddBehaviorVariableFloat(instancedAbility.Evaluate(setAIParam.Param), setAIParam.Value);
					}
				}
				break;
			}
		}

		public void SetAIParamBoolHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			SetAIParamBool setAIParamBool = (SetAIParamBool)actionConfig;
			if (target.entity is BaseMonoMonster)
			{
				BaseMonoMonster baseMonoMonster = target.entity as BaseMonoMonster;
				if (baseMonoMonster.IsAIControllerActive())
				{
					((BTreeMonsterAIController)(target.entity as BaseMonoMonster).GetActiveAIController()).SetBehaviorVariable(instancedAbility.Evaluate(setAIParamBool.Param), setAIParamBool.Value);
				}
			}
		}

		public void SetAIParamStringHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			SetAIParamString setAIParamString = (SetAIParamString)actionConfig;
			if (target.entity is BaseMonoMonster)
			{
				BaseMonoMonster baseMonoMonster = target.entity as BaseMonoMonster;
				if (baseMonoMonster.IsAIControllerActive())
				{
					((BTreeMonsterAIController)(target.entity as BaseMonoMonster).GetActiveAIController()).SetBehaviorVariable(instancedAbility.Evaluate(setAIParamString.Param), instancedAbility.Evaluate(setAIParamString.Value));
				}
			}
		}

		public void TimeSlowHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ActTimeSlow actTimeSlow = (ActTimeSlow)actionConfig;
			ConfigTimeSlow timeSlow = actTimeSlow.TimeSlow;
			Singleton<LevelManager>.Instance.levelActor.TimeSlow(timeSlow.Duration, timeSlow.SlowRatio, null);
		}

		public void ReplaceAttackEffectHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ReplaceAttackEffect replaceAttackEffect = (ReplaceAttackEffect)actionConfig;
			EvtBeingHit evtBeingHit = evt as EvtBeingHit;
			if (replaceAttackEffect.AttackEffect != null)
			{
				evtBeingHit.attackData.attackEffectPattern = replaceAttackEffect.AttackEffect;
			}
			if (replaceAttackEffect.BeHitEffect != null)
			{
				evtBeingHit.attackData.beHitEffectPattern = replaceAttackEffect.BeHitEffect;
			}
		}

		public void ReplaceAttackDataHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ReplaceAttackData replaceAttackData = (ReplaceAttackData)actionConfig;
			EvtBeingHit evtBeingHit = evt as EvtBeingHit;
			if (replaceAttackData.ReplaceFrameHalt)
			{
				evtBeingHit.attackData.frameHalt = replaceAttackData.FrameHalt;
			}
			if (replaceAttackData.ReplaceAttackerAniDamageRatio)
			{
				evtBeingHit.attackData.attackerAniDamageRatio = replaceAttackData.AttackerAniDamageRatio;
			}
			if (replaceAttackData.AddAttackeeAniDefenceRatio != 0f)
			{
				evtBeingHit.attackData.attackeeAniDefenceRatio += replaceAttackData.AddAttackeeAniDefenceRatio;
			}
		}

		protected float _Internal_CalculateApplyLevelBuffDuration(ApplyLevelBuff config, ActorAbility instancedAbility, BaseEvent evt)
		{
			float num = instancedAbility.Evaluate(config.Duration);
			switch (config.LevelBuffSpecial)
			{
			case LevelBuffSpecial.WitchTimeDurationScaledByEvadedAttack:
				if (evt is EvtEvadeSuccess)
				{
					EvtEvadeSuccess evtEvadeSuccess = (EvtEvadeSuccess)evt;
					BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtEvadeSuccess.attackerID);
					if (actor == null)
					{
						break;
					}
					if (actor is MonsterActor)
					{
						ConfigMonsterAnimEvent configMonsterAnimEvent = SharedAnimEventData.ResolveAnimEvent((actor as MonsterActor).config, evtEvadeSuccess.skillID);
						if (configMonsterAnimEvent != null)
						{
							num *= configMonsterAnimEvent.AttackProperty.WitchTimeRatio;
						}
					}
					else if (actor is PropObjectActor)
					{
						ConfigEntityAnimEvent configEntityAnimEvent = SharedAnimEventData.ResolveAnimEvent((actor as PropObjectActor).config, evtEvadeSuccess.skillID);
						if (configEntityAnimEvent != null)
						{
							num *= configEntityAnimEvent.AttackProperty.WitchTimeRatio;
						}
					}
				}
				else
				{
					if (!(evt is EvtDefendSuccess))
					{
						break;
					}
					EvtDefendSuccess evtDefendSuccess = (EvtDefendSuccess)evt;
					MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evtDefendSuccess.attackerID);
					if (actor2 != null)
					{
						ConfigMonsterAnimEvent configMonsterAnimEvent2 = SharedAnimEventData.ResolveAnimEvent(actor2.config, evtDefendSuccess.skillID);
						if (configMonsterAnimEvent2 != null)
						{
							num *= configMonsterAnimEvent2.AttackProperty.WitchTimeRatio;
						}
					}
				}
				break;
			case LevelBuffSpecial.InfiniteDuration:
				num = 999999f;
				break;
			}
			return num;
		}

		public void ApplyLevelBuffHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ApplyLevelBuff applyLevelBuff = (ApplyLevelBuff)actionConfig;
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			bool flag = true;
			if (_levelActor.IsLevelBuffActive(applyLevelBuff.LevelBuff))
			{
				flag = false;
			}
			float duration = _Internal_CalculateApplyLevelBuffDuration(applyLevelBuff, instancedAbility, evt);
			uint runtimeID = instancedAbility.caster.runtimeID;
			LevelBuffSide side = ((!applyLevelBuff.UseOverrideCurSide) ? CalculateLevelBuffSide(runtimeID) : applyLevelBuff.OverrideCurSide);
			switch (applyLevelBuff.LevelBuff)
			{
			case LevelBuffType.WitchTime:
			{
				Singleton<LevelManager>.Instance.levelActor.StartLevelBuff(levelActor.witchTimeLevelBuff, duration, applyLevelBuff.LevelBuffAllowRefresh, applyLevelBuff.EnteringTimeSlow, side, runtimeID, applyLevelBuff.NotStartEffect);
				BaseLevelBuff baseLevelBuff = levelActor.witchTimeLevelBuff;
				break;
			}
			case LevelBuffType.StopWorld:
			{
				Singleton<LevelManager>.Instance.levelActor.StartLevelBuff(levelActor.stopWorldLevelBuff, duration, applyLevelBuff.LevelBuffAllowRefresh, applyLevelBuff.EnteringTimeSlow, side, runtimeID, applyLevelBuff.NotStartEffect);
				BaseLevelBuff baseLevelBuff = levelActor.stopWorldLevelBuff;
				break;
			}
			default:
			{
				BaseLevelBuff baseLevelBuff = null;
				break;
			}
			}
			if (!flag)
			{
				return;
			}
			List<ActorModifier> list = new List<ActorModifier>();
			for (int i = 0; i < applyLevelBuff.AttachModifiers.Length; i++)
			{
				AttachModifier attachModifier = applyLevelBuff.AttachModifiers[i];
				BaseAbilityActor outTarget;
				BaseAbilityActor[] outTargetLs;
				bool needHandleTargetOnNull;
				ResolveTarget(attachModifier.Target, attachModifier.TargetOption, instancedAbility, target, out outTarget, out outTargetLs, out needHandleTargetOnNull);
				if (outTarget != null || needHandleTargetOnNull)
				{
					ActorModifier actorModifier = outTarget.abilityPlugin.ApplyModifier(instancedAbility, attachModifier.ModifierName);
					if (actorModifier != null)
					{
						list.Add(actorModifier);
					}
				}
				else
				{
					if (outTargetLs == null)
					{
						continue;
					}
					for (int j = 0; j < outTargetLs.Length; j++)
					{
						if (outTargetLs[j] != null)
						{
							ActorModifier actorModifier2 = outTargetLs[j].abilityPlugin.ApplyModifier(instancedAbility, attachModifier.ModifierName);
							if (actorModifier2 != null)
							{
								list.Add(actorModifier2);
							}
						}
					}
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				_levelActor.GetPlugin<LevelAbilityHelperPlugin>().AddLevelBuffModifier(applyLevelBuff.LevelBuff, list[k]);
			}
			if (applyLevelBuff.AttachLevelEffectPattern != null)
			{
				_levelActor.GetPlugin<LevelAbilityHelperPlugin>().AttachLevelBuffEffect(applyLevelBuff.LevelBuff, applyLevelBuff.AttachLevelEffectPattern);
			}
		}

		public void StopTargetLevelBuffHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			StopTargetLevelBuff stopTargetLevelBuff = (StopTargetLevelBuff)actionConfig;
			if (!_levelActor.IsLevelBuffActive(stopTargetLevelBuff.LevelBuff))
			{
				return;
			}
			LevelBuffSide levelBuffSide = CalculateLevelBuffSide(instancedAbility.caster.runtimeID);
			if (stopTargetLevelBuff.LevelBuff == LevelBuffType.WitchTime)
			{
				LevelBuffWitchTime witchTimeLevelBuff = _levelActor.witchTimeLevelBuff;
				if ((!stopTargetLevelBuff.stopOtherSide) ? (levelBuffSide == witchTimeLevelBuff.levelBuffSide) : (levelBuffSide != witchTimeLevelBuff.levelBuffSide))
				{
					_levelActor.StopLevelBuff(witchTimeLevelBuff);
				}
			}
			else if (stopTargetLevelBuff.LevelBuff == LevelBuffType.StopWorld)
			{
				LevelBuffStopWorld stopWorldLevelBuff = _levelActor.stopWorldLevelBuff;
				if ((!stopTargetLevelBuff.stopOtherSide) ? (levelBuffSide == stopWorldLevelBuff.levelBuffSide) : (levelBuffSide != stopWorldLevelBuff.levelBuffSide))
				{
					_levelActor.StopLevelBuff(stopWorldLevelBuff);
				}
			}
		}

		public void RefreshTargetLevelBuffHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			RefreshTargetLevelBuff refreshTargetLevelBuff = (RefreshTargetLevelBuff)actionConfig;
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (_levelActor.IsLevelBuffActive(refreshTargetLevelBuff.LevelBuff) && refreshTargetLevelBuff.LevelBuff == LevelBuffType.WitchTime)
			{
				levelActor.witchTimeLevelBuff.ApplyWitchTimeSlowedBySideWithRuntimeID(target.runtimeID);
			}
		}

		public void AttachMaterialGroupHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachMaterialGroup attachMaterialGroup = (AttachMaterialGroup)actionConfig;
			target.entity.PushMaterialGroup(attachMaterialGroup.MaterialGroupName);
			if (instancedModifier != null)
			{
				instancedModifier.AttachPushMaterialGroup(target.entity);
			}
			else
			{
				instancedAbility.AttachPushMaterialGroup(target.entity);
			}
		}

		public void PredicateByTargetDistanceHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateByTargetDistance predicateByTargetDistance = (PredicateByTargetDistance)actionConfig;
			BaseActor caster = instancedAbility.caster;
			if (caster != null)
			{
				if (target == null || predicateByTargetDistance.Distance < 0f || (target.gameObject.transform.position - caster.gameObject.transform.position).magnitude < predicateByTargetDistance.Distance)
				{
					HandleActionTargetDispatch(predicateByTargetDistance.InActions, instancedAbility, instancedModifier, target, evt);
				}
				else
				{
					HandleActionTargetDispatch(predicateByTargetDistance.OutActions, instancedAbility, instancedModifier, target, evt);
				}
			}
		}

		public void PredicateByHitTypeHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateByHitType predicateByHitType = (PredicateByHitType)actionConfig;
			EvtBeingHit evtBeingHit = evt as EvtBeingHit;
			if (evtBeingHit != null)
			{
				if (evtBeingHit.attackData.hitType == AttackResult.ActorHitType.Melee)
				{
					HandleActionTargetDispatch(predicateByHitType.MeleeActions, instancedAbility, instancedModifier, target, evt);
				}
				else if (evtBeingHit.attackData.hitType == AttackResult.ActorHitType.Ranged)
				{
					HandleActionTargetDispatch(predicateByHitType.RangeActions, instancedAbility, instancedModifier, target, evt);
				}
			}
		}

		public void SpawnPropObjectHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			SpawnPropObject spawnPropObject = (SpawnPropObject)actionConfig;
			if (target.entity != null)
			{
				uint runtimeID = Singleton<PropObjectManager>.Instance.CreatePropObject(_owner.runtimeID, spawnPropObject.PropObjectName, 0f, target.attack, (!spawnPropObject.FollowRootNode) ? target.entity.XZPosition : target.entity.GetAttachPoint("RootNode").position, target.entity.transform.forward);
				PropObjectActor actor = Singleton<EventManager>.Instance.GetActor<PropObjectActor>(runtimeID);
				if (spawnPropObject.ModifierName != null)
				{
					actor.abilityPlugin.ApplyModifier(instancedAbility, spawnPropObject.ModifierName);
				}
			}
		}

		public void ShootPalsyBombHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ShootPalsyBomb shootPalsyBomb = (ShootPalsyBomb)actionConfig;
			DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(Singleton<EventManager>.Instance.GetActor(instancedAbility.caster.runtimeID), shootPalsyBomb.BombAttackID);
			uint runtimeID = Singleton<LevelDesignManager>.Instance.CreatePropObject("Spawn01", instancedAbility.Evaluate(shootPalsyBomb.PropName));
			MonoPalsyBombProp monoPalsyBombProp = (MonoPalsyBombProp)Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
			Transform attachPoint = instancedAbility.caster.entity.GetAttachPoint(shootPalsyBomb.AttachPoint);
			Vector3 vector = Vector3.RotateTowards(instancedAbility.caster.entity.transform.forward, Vector3.up, (float)Math.PI / 3f, 0f);
			monoPalsyBombProp.StartParabolaBorn(bornVelocity: new Vector3(vector.x * shootPalsyBomb.BombSpeed, vector.y * 10f, vector.z * shootPalsyBomb.BombSpeed), bornPosition: attachPoint.position, bornAcceleration: new Vector3(0f, -20f, 0f));
		}

		public void CreateUnitFieldHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			CreateUnitField createUnitField = (CreateUnitField)actionConfig;
			string propName = "Trap_Slow";
			Vector3 xZPosition = instancedAbility.caster.entity.XZPosition;
			uint runtimeID = Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, propName, 1f, 1f, xZPosition, instancedAbility.caster.entity.transform.forward);
			MonoTriggerUnitFieldProp monoTriggerUnitFieldProp = (MonoTriggerUnitFieldProp)Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
			monoTriggerUnitFieldProp.InitUnitFieldPropRange(createUnitField.numberX, createUnitField.numberZ);
			monoTriggerUnitFieldProp.EnableProp();
		}

		public void AttachMaskAnimEventIDsHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachMaskAnimEventIDs attachMaskAnimEventIDs = (AttachMaskAnimEventIDs)actionConfig;
			BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
			for (int i = 0; i < attachMaskAnimEventIDs.AnimEventIDs.Length; i++)
			{
				string text = attachMaskAnimEventIDs.AnimEventIDs[i];
				target.entity.MaskAnimEvent(text);
				baseActorActionContext.AttachMaskedAnimEventID(target.entity, text);
			}
		}

		public void RandomedHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			Randomed randomed = (Randomed)actionConfig;
			float value = UnityEngine.Random.value;
			float num = instancedAbility.Evaluate(randomed.Chance);
			bool flag = value < num;
			if (value < num)
			{
				HandleActionTargetDispatch(randomed.SuccessActions, instancedAbility, instancedModifier, target, evt);
			}
			else
			{
				HandleActionTargetDispatch(randomed.FailActions, instancedAbility, instancedModifier, target, evt);
			}
		}

		public void PredicatedHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			Predicated predicated = (Predicated)actionConfig;
			if (EvaluateAbilityPredicate(predicated.TargetPredicates, instancedAbility, instancedModifier, target, evt))
			{
				HandleActionTargetDispatch(predicated.SuccessActions, instancedAbility, instancedModifier, target, evt);
			}
			else
			{
				HandleActionTargetDispatch(predicated.FailActions, instancedAbility, instancedModifier, target, evt);
			}
		}

		public void RetargetedHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			Retargeted retargeted = (Retargeted)actionConfig;
			BaseAbilityActor outTarget;
			BaseAbilityActor[] outTargetLs;
			bool needHandleTargetOnNull;
			ResolveTarget(retargeted.Retarget, retargeted.RetargetOption, instancedAbility, target, out outTarget, out outTargetLs, out needHandleTargetOnNull);
			if (outTarget != null || needHandleTargetOnNull)
			{
				HandleActionTargetDispatch(retargeted.RetargetedActions, instancedAbility, instancedModifier, outTarget, evt);
			}
			else
			{
				if (outTargetLs == null || outTargetLs.Length <= 0)
				{
					return;
				}
				if (retargeted.RandomedTarget)
				{
					int num = UnityEngine.Random.Range(0, outTargetLs.Length);
					if (outTargetLs[num] != null)
					{
						HandleActionTargetDispatch(retargeted.RetargetedActions, instancedAbility, instancedModifier, outTargetLs[num], evt);
					}
					return;
				}
				for (int i = 0; i < outTargetLs.Length; i++)
				{
					if (outTargetLs[i] != null && (!retargeted.IgnoreSelf || outTargetLs[i] != instancedAbility.caster))
					{
						HandleActionTargetDispatch(retargeted.RetargetedActions, instancedAbility, instancedModifier, outTargetLs[i], evt);
					}
				}
			}
		}

		public void PredicateByTargetClassHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateByTargetClass config = (PredicateByTargetClass)actionConfig;
			for (int i = 0; i < config.Actions.Length; i++)
			{
				HandleActionTargetDispatch(config.Actions[i], instancedAbility, instancedModifier, target, evt, (BaseAbilityActor actionTarget) => actionTarget != null && actionTarget.commonConfig.CommonArguments.Class == config.EntityClass);
			}
		}

		public void PredicateByTargetNatureHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateByTargetNature config = (PredicateByTargetNature)actionConfig;
			for (int i = 0; i < config.Actions.Length; i++)
			{
				HandleActionTargetDispatch(config.Actions[i], instancedAbility, instancedModifier, target, evt, (BaseAbilityActor actionTarget) => actionTarget != null && actionTarget.commonConfig.CommonArguments.Nature == config.EntityNature);
			}
		}

		public void PredicateBySpecialHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateBySpecial predicateBySpecial = (PredicateBySpecial)actionConfig;
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			switch (predicateBySpecial.Special)
			{
			case PredicateBySpecial.SpecialType.IsEveryAvatarHasDifferenctClass:
			{
				bool flag3 = true;
				for (int m = 0; m < allPlayerAvatars.Count; m++)
				{
					for (int n = 0; n < allPlayerAvatars.Count; n++)
					{
						if (m != n && allPlayerAvatars[m].config.CommonArguments.Class == allPlayerAvatars[n].config.CommonArguments.Class)
						{
							flag3 = false;
						}
					}
				}
				if (flag3)
				{
					HandleActionTargetDispatch(predicateBySpecial.Actions, instancedAbility, instancedModifier, target, evt);
				}
				break;
			}
			case PredicateBySpecial.SpecialType.IsEveryAvatarHasDifferenctRoleName:
			{
				bool flag4 = true;
				for (int num = 0; num < allPlayerAvatars.Count; num++)
				{
					for (int num2 = 0; num2 < allPlayerAvatars.Count; num2++)
					{
						if (num != num2 && allPlayerAvatars[num].config.CommonArguments.RoleName == allPlayerAvatars[num2].config.CommonArguments.RoleName)
						{
							flag4 = false;
						}
					}
				}
				if (flag4)
				{
					HandleActionTargetDispatch(predicateBySpecial.Actions, instancedAbility, instancedModifier, target, evt);
				}
				break;
			}
			case PredicateBySpecial.SpecialType.IsEveryAvatarHasDifferenctNature:
			{
				bool flag2 = true;
				for (int k = 0; k < allPlayerAvatars.Count; k++)
				{
					for (int l = 0; l < allPlayerAvatars.Count; l++)
					{
						if (k != l && allPlayerAvatars[k].config.CommonArguments.Nature == allPlayerAvatars[l].config.CommonArguments.Nature)
						{
							flag2 = false;
						}
					}
				}
				if (flag2)
				{
					HandleActionTargetDispatch(predicateBySpecial.Actions, instancedAbility, instancedModifier, target, evt);
				}
				break;
			}
			case PredicateBySpecial.SpecialType.IsEveryAvatarHasSameNature:
			{
				bool flag = true;
				for (int i = 0; i < allPlayerAvatars.Count; i++)
				{
					for (int j = 0; j < allPlayerAvatars.Count; j++)
					{
						if (allPlayerAvatars[i].config.CommonArguments.Nature != predicateBySpecial.SameNature)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					HandleActionTargetDispatch(predicateBySpecial.Actions, instancedAbility, instancedModifier, target, evt);
				}
				break;
			}
			}
		}

		public void PredicateByActorPresenceHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateByActorPresence predicateByActorPresence = (PredicateByActorPresence)actionConfig;
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			bool flag = true;
			for (int i = 0; i < predicateByActorPresence.ActorNames.Length; i++)
			{
				bool flag2 = false;
				for (int j = 0; j < allPlayerAvatars.Count; j++)
				{
					if (predicateByActorPresence.ActorNames[i] == allPlayerAvatars[j].config.CommonArguments.RoleName)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				HandleActionTargetDispatch(predicateByActorPresence.Actions, instancedAbility, instancedModifier, target, evt);
			}
		}

		public void ModifyPropertyHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ModifyProperty modifyProperty = (ModifyProperty)actionConfig;
			int num = instancedModifier.config.Properties.IndexOfKey(modifyProperty.Property);
			int stackIx = instancedModifier.stackIndices[num];
			float propertyByStackIndex = target.GetPropertyByStackIndex(modifyProperty.Property, stackIx);
			float value = propertyByStackIndex + instancedAbility.Evaluate(modifyProperty.Delta);
			value = Mathf.Clamp(value, instancedAbility.Evaluate(modifyProperty.Min), instancedAbility.Evaluate(modifyProperty.Max));
			target.SetPropertyByStackIndex(modifyProperty.Property, stackIx, value);
		}

		public void ResetPropertyHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ResetProperty resetProperty = (ResetProperty)actionConfig;
			int num = instancedModifier.config.Properties.IndexOfKey(resetProperty.Property);
			int stackIx = instancedModifier.stackIndices[num];
			target.SetPropertyByStackIndex(resetProperty.Property, stackIx, instancedAbility.Evaluate(instancedModifier.config.Properties[resetProperty.Property]));
		}

		public void SetAnimatorBoolHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			SetAnimatorBool setAnimatorBool = (SetAnimatorBool)actionConfig;
			target.entity.SetPersistentAnimatorBool(setAnimatorBool.BoolID, setAnimatorBool.Value);
		}

		public void SetAnimatorIntHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			SetAnimatorInt setAnimatorInt = (SetAnimatorInt)actionConfig;
			target.entity.SetPersistentAnimatoInt(setAnimatorInt.IntID, setAnimatorInt.Value);
		}

		public void SetLocomotionRandomHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			SetLocomotionRandom setLocomotionRandom = (SetLocomotionRandom)actionConfig;
			(target.entity as BaseMonoAnimatorEntity).SetLocomotionRandom(instancedAbility.Evaluate(setLocomotionRandom.Range));
		}

		public void SetLocomotionFloatHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			SetLocomotionFloat setLocomotionFloat = (SetLocomotionFloat)actionConfig;
			(target.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(setLocomotionFloat.Param, setLocomotionFloat.Value);
		}

		public void DamageByAnimEventIDHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			DamageByAnimEventID damageByAnimEventID = (DamageByAnimEventID)actionConfig;
			BaseActor caster = instancedAbility.caster;
			AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(caster, damageByAnimEventID.AnimEventID);
			bool forceSkipAttackerResolve = !caster.IsEntityExists();
			AttackPattern.SendHitEvent(caster.runtimeID, target.runtimeID, damageByAnimEventID.AnimEventID, null, attackData, forceSkipAttackerResolve);
		}

		public void DamageByAttackPropertyHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			DamageByAttackProperty damageByAttackProperty = (DamageByAttackProperty)actionConfig;
			BaseActor caster = instancedAbility.caster;
			AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackProperty(caster, damageByAttackProperty.AttackProperty, damageByAttackProperty.AttackEffect, damageByAttackProperty.CameraShake);
			attackData.hitType = AttackResult.ActorHitType.Ailment;
			bool forceSkipAttackerResolve = !caster.IsEntityExists();
			AttackPattern.SendHitEvent(caster.runtimeID, target.runtimeID, null, null, attackData, forceSkipAttackerResolve);
		}

		public void ReflectDamageByAttackPropertyHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ReflectDamageByAttackProperty reflectDamageByAttackProperty = (ReflectDamageByAttackProperty)actionConfig;
			BaseActor caster = instancedAbility.caster;
			EvtBeingHit evtBeingHit = evt as EvtBeingHit;
			if (!evtBeingHit.GetAttackResult().rejected)
			{
				ConfigEntityAttackProperty attackProperty = reflectDamageByAttackProperty.AttackProperty;
				attackProperty.AddedDamageValue = evtBeingHit.GetAttackResult().GetTotalDamage() * instancedAbility.Evaluate(reflectDamageByAttackProperty.ReflectRatio);
				AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackProperty(caster, reflectDamageByAttackProperty.AttackProperty, reflectDamageByAttackProperty.AttackEffect, reflectDamageByAttackProperty.CameraShake);
				attackData.hitType = AttackResult.ActorHitType.Ailment;
				bool forceSkipAttackerResolve = !caster.IsEntityExists();
				AttackPattern.SendHitEvent(caster.runtimeID, target.runtimeID, null, null, attackData, forceSkipAttackerResolve);
			}
		}

		public void DamageByAttackValueHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			DamageByAttackValue damageByAttackValue = (DamageByAttackValue)actionConfig;
			BaseActor caster = instancedAbility.caster;
			_attackProperty.DamagePercentage = instancedAbility.Evaluate(damageByAttackValue.DamagePercentage);
			_attackProperty.AddedDamageValue = instancedAbility.Evaluate(damageByAttackValue.AddedDamageValue);
			_attackProperty.NormalDamage = instancedAbility.Evaluate(damageByAttackValue.PlainDamage);
			_attackProperty.NormalDamagePercentage = instancedAbility.Evaluate(damageByAttackValue.PlainDamagePercentage);
			_attackProperty.FireDamage = instancedAbility.Evaluate(damageByAttackValue.FireDamage);
			_attackProperty.FireDamagePercentage = instancedAbility.Evaluate(damageByAttackValue.FireDamagePercentage);
			_attackProperty.ThunderDamage = instancedAbility.Evaluate(damageByAttackValue.ThunderDamage);
			_attackProperty.ThunderDamagePercentage = instancedAbility.Evaluate(damageByAttackValue.ThunderDamagePercentage);
			_attackProperty.IceDamage = instancedAbility.Evaluate(damageByAttackValue.IceDamage);
			_attackProperty.IceDamagePercentage = instancedAbility.Evaluate(damageByAttackValue.IceDamagePercentage);
			_attackProperty.AlienDamage = instancedAbility.Evaluate(damageByAttackValue.AlienDamage);
			_attackProperty.AlienDamagePercentage = instancedAbility.Evaluate(damageByAttackValue.AlienDamagePercentage);
			_attackProperty.AniDamageRatio = instancedAbility.Evaluate(damageByAttackValue.AniDamageRatio);
			_attackProperty.FrameHalt = instancedAbility.Evaluate(damageByAttackValue.FrameHalt);
			_attackProperty.HitType = AttackResult.ActorHitType.Ailment;
			_attackProperty.HitEffect = damageByAttackValue.HitEffect;
			_attackProperty.RetreatVelocity = instancedAbility.Evaluate(damageByAttackValue.RetreatVelocity);
			_attackProperty.IsAnimEventAttack = damageByAttackValue.IsAnimEventAttack;
			_attackProperty.IsInComboCount = damageByAttackValue.IsInComboCount;
			AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackProperty(caster, _attackProperty, damageByAttackValue.AttackEffect, damageByAttackValue.CameraShake);
			attackData.hitLevel = damageByAttackValue.HitLevel;
			bool forceSkipAttackerResolve = !caster.IsEntityExists();
			AttackPattern.SendHitEvent(caster.runtimeID, target.runtimeID, null, null, attackData, forceSkipAttackerResolve);
		}

		public void PredicateByAnimEventIDHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateByAnimEventID predicateByAnimEventID = (PredicateByAnimEventID)actionConfig;
			string text = null;
			if (evt is EvtBeingHit)
			{
				EvtBeingHit evtBeingHit = (EvtBeingHit)evt;
				text = evtBeingHit.animEventID;
			}
			else if (evt is EvtHittingOther)
			{
				EvtHittingOther evtHittingOther = (EvtHittingOther)evt;
				text = evtHittingOther.animEventID;
			}
			else if (evt is EvtAttackLanded)
			{
				EvtAttackLanded evtAttackLanded = (EvtAttackLanded)evt;
				text = evtAttackLanded.animEventID;
			}
			if (text == null)
			{
				return;
			}
			for (int i = 0; i < predicateByAnimEventID.AnimEventIDs.Length; i++)
			{
				if (text == predicateByAnimEventID.AnimEventIDs[i])
				{
					HandleActionTargetDispatch(predicateByAnimEventID.Actions, instancedAbility, instancedModifier, target, evt);
					break;
				}
			}
		}

		public void PredicateByParamHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateByParam predicateByParam = (PredicateByParam)actionConfig;
			if (instancedAbility.HasParam(predicateByParam.Param))
			{
				HandleActionTargetDispatch(predicateByParam.Actions, instancedAbility, instancedModifier, target, evt);
			}
		}

		public void PredicateByParamNotZeroHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateByParamNotZero predicateByParamNotZero = (PredicateByParamNotZero)actionConfig;
			if (instancedAbility.GetFloatParam(predicateByParamNotZero.Param) != 0f)
			{
				HandleActionTargetDispatch(predicateByParamNotZero.Actions, instancedAbility, instancedModifier, target, evt);
			}
		}

		public void PredicateByHasEnemyAroundHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			PredicateByHasEnemyAround predicateByHasEnemyAround = (PredicateByHasEnemyAround)actionConfig;
			List<CollisionResult> list = CollisionDetectPattern.CircleCollisionDetectBySphere(_owner.entity.XZPosition + Vector3.up, 0f, _owner.entity.transform.forward, instancedAbility.Evaluate(predicateByHasEnemyAround.Range), Singleton<EventManager>.Instance.GetAbilityTargettingMask(_owner.runtimeID, MixinTargetting.Enemy));
			if (list.Count > 0)
			{
				HandleActionTargetDispatch(predicateByHasEnemyAround.Actions, instancedAbility, instancedModifier, target, evt);
			}
		}

		public void TriggerAbilityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			MoleMole.Config.TriggerAbility triggerAbility = (MoleMole.Config.TriggerAbility)actionConfig;
			EvtAbilityStart evtAbilityStart = new EvtAbilityStart(target.runtimeID, instancedAbility.CurrentTriggerEvent);
			evtAbilityStart.abilityID = triggerAbility.AbilityID;
			evtAbilityStart.abilityName = triggerAbility.AbilityName;
			evtAbilityStart.otherID = GetOtherID(evt);
			evtAbilityStart.hitCollision = GetHitCollision(evt);
			evtAbilityStart.abilityArgument = triggerAbility.Argument;
			Singleton<EventManager>.Instance.FireEvent(evtAbilityStart);
		}

		public void ClearComboHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.ResetCombo();
		}

		public void AttachImmuneAbilityStateHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachImmuneAbilityState attachImmuneAbilityState = (AttachImmuneAbilityState)actionConfig;
			if (attachImmuneAbilityState.ClearAppliedState && (target.abilityState & attachImmuneAbilityState.ImmuneState) != AbilityState.None)
			{
				target.RemoveAbilityState(attachImmuneAbilityState.ImmuneState);
			}
			target.SetAbilityStateImmune(attachImmuneAbilityState.ImmuneState, true);
			if (instancedModifier != null)
			{
				instancedModifier.AttachImmuneAbilityState(target, attachImmuneAbilityState.ImmuneState);
			}
			else
			{
				instancedAbility.AttachImmuneAbilityState(target, attachImmuneAbilityState.ImmuneState);
			}
		}

		public void AttachBuffDebufResistanceHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (target.IsEntityExists())
			{
				BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
				AttachBuffDebuffResistance resistance = (AttachBuffDebuffResistance)actionConfig;
				baseActorActionContext.AttachBuffDebuffResistance(target, resistance);
			}
		}

		public void DebugLogHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
		}

		public void MissionTriggerAbilityActionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			Singleton<MissionModule>.Instance.TryToUpdateTriggerAbilityAction((uint)((MissionTriggerAbilityAction)actionConfig).FinishParaInt);
		}

		public void FireAudioHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
		}

		public void AttachTintHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachStageTint tintConfig = (AttachStageTint)actionConfig;
			BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
			baseActorActionContext.AttachStageTint(tintConfig);
		}

		public void SetAnimatorTriggerHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			SetAnimatorTrigger setAnimatorTrigger = (SetAnimatorTrigger)actionConfig;
			target.entity.SetTrigger(setAnimatorTrigger.TriggerID);
		}

		public void ResetAnimatorTriggerHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ResetAnimatorTrigger resetAnimatorTrigger = (ResetAnimatorTrigger)actionConfig;
			target.entity.ResetTrigger(resetAnimatorTrigger.TriggerID);
		}

		public void CreateGoodsHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			CreateGoods createGoods = (CreateGoods)actionConfig;
			Singleton<DynamicObjectManager>.Instance.CreateGood(562036737u, createGoods.GoodType, createGoods.GoodAbility, instancedAbility.Evaluate(createGoods.GoodArgument), target.entity.XZPosition, target.entity.transform.forward, true);
		}

		public void AttachDisableHitboxHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (target.IsEntityExists())
			{
				BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
				AttachIsGhost attachIsGhost = (AttachIsGhost)actionConfig;
				baseActorActionContext.AttachIsGhost(target.entity, attachIsGhost.IsGhost);
			}
		}

		public void AttachNoCollisionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (target.IsEntityExists())
			{
				BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
				baseActorActionContext.AttachNoCollision(target.entity);
			}
		}

		public void AttachAllowSelectionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (target.IsEntityExists())
			{
				BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
				AttachAllowSelection attachAllowSelection = (AttachAllowSelection)actionConfig;
				baseActorActionContext.AttachAllowSelected(target.entity, attachAllowSelection.AllowSelection);
			}
		}

		public void AttachOpacityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (target.IsEntityExists())
			{
				BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
				AttachOpacity attachOpacity = (AttachOpacity)actionConfig;
				baseActorActionContext.AttachOpacity(target.entity, instancedAbility.Evaluate(attachOpacity.Opacity));
			}
		}

		public void SetSelfAttackTargetHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (target.IsEntityExists() && _owner.IsEntityExists())
			{
				SetSelfAttackTarget setSelfAttackTarget = (SetSelfAttackTarget)actionConfig;
				_owner.entity.SetAttackTarget(target.entity);
				if (setSelfAttackTarget.SteerToTargetImmediately)
				{
					Vector3 forward = target.entity.XZPosition - _owner.entity.XZPosition;
					_owner.entity.SteerFaceDirectionTo(forward);
				}
			}
		}

		public void SetAllowSwitchOther(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (target.IsEntityExists())
			{
				BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
				AttachAllowSwitchOther attachAllowSwitchOther = (AttachAllowSwitchOther)actionConfig;
				baseActorActionContext.AttachAllowSwitchOther(target.entity, attachAllowSwitchOther.AllowSwitchOther);
			}
		}

		public void SetMuteOtherQTE(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (target.IsEntityExists())
			{
				BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
				AttachMuteOtherQTE attachMuteOtherQTE = (AttachMuteOtherQTE)actionConfig;
				baseActorActionContext.AttachMuteOtherQTE(target.entity, attachMuteOtherQTE.MuteOtherQTE);
			}
		}

		public void AttachImmuneDebuffHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachImmuneDebuff attachImmuneDebuff = (AttachImmuneDebuff)actionConfig;
			if (attachImmuneDebuff.ClearAppliedDebuff)
			{
				target.abilityPlugin.RemoveAllDebuffModifiers();
			}
			target.SetImmuneDebuff(true);
			BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
			baseActorActionContext.AttachImmuneDebuff(target);
		}

		public void AttachEffectOverrideHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttachEffectOverride attachEffectOverride = (AttachEffectOverride)actionConfig;
			target.entity.AddEffectOverride(attachEffectOverride.EffectOverrideKey, attachEffectOverride.EffectPattern);
			BaseActorActionContext baseActorActionContext = ((instancedModifier == null) ? ((BaseActorActionContext)instancedAbility) : ((BaseActorActionContext)instancedModifier));
			baseActorActionContext.AttachEffectOverride(target.entity, attachEffectOverride.EffectOverrideKey);
		}

		public void AvatarSkillStartHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AvatarSkillStart avatarSkillStart = (AvatarSkillStart)actionConfig;
			if (instancedAbility.caster is AvatarActor)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtSkillStart(target.runtimeID, avatarSkillStart.CDSkillID));
			}
		}

		public void FireEventHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			FireEvent fireEvent = (FireEvent)actionConfig;
			EventCategory eventCategory = (EventCategory)(int)Enum.Parse(typeof(EventCategory), fireEvent.EvtCategory);
			if (eventCategory == EventCategory.EvtDefendStart)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtDefendStart(_owner.runtimeID));
			}
			if (eventCategory == EventCategory.EvtDefendSuccess && evt != null && evt is EvtBeingHit)
			{
				EvtBeingHit evtBeingHit = evt as EvtBeingHit;
				Singleton<EventManager>.Instance.FireEvent(new EvtDefendSuccess(_owner.runtimeID, evtBeingHit.sourceID, evtBeingHit.animEventID));
			}
		}

		public void TriggerAttackPatternHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			TriggerAttackPattern triggerAttackPattern = (TriggerAttackPattern)actionConfig;
			if (target.IsActive())
			{
				if (triggerAttackPattern.IgnoreEvade)
				{
					target.entity.TriggerAttackPattern(triggerAttackPattern.AnimEventID, Singleton<EventManager>.Instance.GetAbilityTargettingMask(target.runtimeID, triggerAttackPattern.Targetting));
				}
				else
				{
					target.entity.TriggerAttackPattern(triggerAttackPattern.AnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(target.runtimeID, triggerAttackPattern.Targetting));
				}
			}
		}

		public void TriggerAnimEventHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			TriggerAnimEvent triggerAnimEvent = (TriggerAnimEvent)actionConfig;
			if (target.IsActive())
			{
				(target.entity as BaseMonoAnimatorEntity).AnimEventHandler(triggerAnimEvent.AnimEventID);
			}
		}

		public void ForceKillHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ForceKill forceKill = (ForceKill)actionConfig;
			if ((bool)target.isAlive)
			{
				target.ForceKill(_owner.runtimeID, forceKill.KillEffect);
			}
		}

		public void RestartMainAIHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if ((bool)target.isAlive && target.HasPlugin<MonsterAIPlugin>())
			{
				target.GetPlugin<MonsterAIPlugin>().RestartMainAI();
			}
		}

		protected LevelBuffSide CalculateLevelBuffSide(uint ownerID)
		{
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 3)
			{
				return LevelBuffSide.FromAvatar;
			}
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 4)
			{
				return LevelBuffSide.FromMonster;
			}
			return LevelBuffSide.FromLevel;
		}

		public bool ByTargetClassHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return target != null && ((ByTargetEntityClass)predConfig).TargetClass == target.commonConfig.CommonArguments.Class;
		}

		public bool ByTargetNatureHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return target != null && ((ByTargetEntityNature)predConfig).TargetNature == target.commonConfig.CommonArguments.Nature;
		}

		public bool ByTargetDistanceHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByTargetDistance byTargetDistance = (ByTargetDistance)predConfig;
			if (target == null)
			{
				return false;
			}
			float magnitude = (target.entity.XZPosition - instancedAbility.caster.entity.XZPosition).magnitude;
			switch (byTargetDistance.Logic)
			{
			case MixinPredicate.Greater:
			case MixinPredicate.GreaterOrEqual:
				return magnitude > instancedAbility.Evaluate(byTargetDistance.Distance);
			case MixinPredicate.Lesser:
				return magnitude < instancedAbility.Evaluate(byTargetDistance.Distance);
			default:
				return false;
			}
		}

		public bool ByTargetInLevelAnimHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByTargetInLevelAnim byTargetInLevelAnim = (ByTargetInLevelAnim)predConfig;
			if (target == null)
			{
				return false;
			}
			return target.isInLevelAnim == byTargetInLevelAnim.InLevelAnim;
		}

		public bool ByTargetAnimStateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByTargetAnimState byTargetAnimState = (ByTargetAnimState)predConfig;
			if (target == null)
			{
				return false;
			}
			bool result = false;
			if (byTargetAnimState.State == ByTargetAnimState.AnimState.Throw && target is MonsterActor)
			{
				return ((MonsterActor)target).monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw);
			}
			return result;
		}

		public bool ByAttackTargetAnimStateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByAttackTargetAnimState byAttackTargetAnimState = (ByAttackTargetAnimState)predConfig;
			if (target == null)
			{
				return false;
			}
			bool result = false;
			if (byAttackTargetAnimState.State == ByAttackTargetAnimState.AnimState.Throw)
			{
				AvatarActor avatarActor = target as AvatarActor;
				if (avatarActor != null && avatarActor.avatar != null && avatarActor.avatar.AttackTarget is BaseMonoMonster)
				{
					return ((BaseMonoMonster)avatarActor.avatar.AttackTarget).IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw);
				}
			}
			return result;
		}

		public bool ByAttackCategoryTagHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByAttackCategoryTag byAttackCategoryTag = (ByAttackCategoryTag)predConfig;
			if (target == null)
			{
				return false;
			}
			IEvtWithAttackResult evtWithAttackResult = evt as IEvtWithAttackResult;
			if (evtWithAttackResult == null)
			{
				return false;
			}
			AttackResult attackResult = evtWithAttackResult.GetAttackResult();
			if (attackResult == null)
			{
				return false;
			}
			return attackResult.attackCategoryTag.ContainsTag(byAttackCategoryTag.CategoryTag);
		}

		public bool ByTargetWithinAbilityStateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByTargetWithinAbilityState byTargetWithinAbilityState = (ByTargetWithinAbilityState)predConfig;
			if (byTargetWithinAbilityState.TargetStates != null)
			{
				AbilityState abilityState = AbilityState.None;
				for (int i = 0; i < byTargetWithinAbilityState.TargetStates.Length; i++)
				{
					abilityState |= byTargetWithinAbilityState.TargetStates[i];
				}
				return target != null && (abilityState & target.abilityState) != 0;
			}
			return target != null && (byTargetWithinAbilityState.TargetState & target.abilityState) != 0;
		}

		public bool ByTargetContainAnimEventPredicateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByTargetContainAnimEventPredicate byTargetContainAnimEventPredicate = (ByTargetContainAnimEventPredicate)predConfig;
			if (byTargetContainAnimEventPredicate.ForceByCaster)
			{
				target = instancedAbility.caster;
			}
			if (target == null)
			{
				return false;
			}
			return target.entity.ContainAnimEventPredicate(byTargetContainAnimEventPredicate.AnimEventPredicate);
		}

		public bool ByTargetAppliedUniqueModifierHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByTargetAppliedUniqueModifier byTargetAppliedUniqueModifier = (ByTargetAppliedUniqueModifier)predConfig;
			if (target == null)
			{
				return false;
			}
			return target.abilityPlugin.GetFirstUniqueModifier(byTargetAppliedUniqueModifier.UniquModifierName) != null;
		}

		public bool ByAttackHitTypeHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttackResult attackResult = GetAttackResult(evt);
			if (attackResult != null)
			{
				return attackResult.hitType == ((ByAttackHitType)predConfig).HitType;
			}
			return false;
		}

		public bool ByAttackHitFlagHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttackResult attackResult = GetAttackResult(evt);
			if (attackResult != null)
			{
				return attackResult.ContainHitFlag(((ByAttackHitFlag)predConfig).HitFlag);
			}
			return false;
		}

		public bool ByAttackAniDamageRatioHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByAttackAniDamageRatio byAttackAniDamageRatio = (ByAttackAniDamageRatio)predConfig;
			AttackData attackData = GetAttackResult(evt) as AttackData;
			if (attackData != null)
			{
				switch (byAttackAniDamageRatio.CompareType)
				{
				case ByAttackAniDamageRatio.LogicType.MoreThan:
					return (attackData.attackerAniDamageRatio > byAttackAniDamageRatio.AniDamageRatio) ? true : false;
				case ByAttackAniDamageRatio.LogicType.LessThan:
					return (attackData.attackerAniDamageRatio < byAttackAniDamageRatio.AniDamageRatio) ? true : false;
				}
			}
			return false;
		}

		public bool ByAttackIsAnimEventHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttackResult attackResult = GetAttackResult(evt);
			if (attackResult != null)
			{
				return attackResult.isAnimEventAttack == ((ByAttackFromAnimEvent)predConfig).IsAnimEventAttack;
			}
			return false;
		}

		public bool ByAttackInComboCountHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			AttackResult attackResult = GetAttackResult(evt);
			if (attackResult != null)
			{
				return attackResult.isInComboCount == ((ByAttackInComboCount)predConfig).InComboCount;
			}
			return false;
		}

		public bool ByHitDirectionHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			EvtBeingHit evtBeingHit = evt as EvtBeingHit;
			if (evtBeingHit.attackData != null)
			{
				if (!evtBeingHit.attackData.isAnimEventAttack)
				{
					return false;
				}
				if (evtBeingHit.attackData.rejected)
				{
					return false;
				}
				BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtBeingHit.sourceID);
				float num = Vector3.Angle(instancedAbility.caster.entity.transform.forward, actor.entity.transform.position - instancedAbility.caster.entity.transform.position);
				bool flag = num < ((ByHitDirection)predConfig).Angle;
				if (((ByHitDirection)predConfig).ReverseAngle)
				{
					flag = !flag;
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public bool ByAttackAnimEventIDHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			string animEventID = GetAnimEventID(evt);
			if (animEventID != null)
			{
				if (((ByAttackAnimEventID)predConfig).ByAnyEventID)
				{
					return true;
				}
				return Miscs.ArrayContains(((ByAttackAnimEventID)predConfig).AnimEventIDs, animEventID);
			}
			return false;
		}

		public bool ByTargetQTENameHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByTargetQTEName byTargetQTEName = predConfig as ByTargetQTEName;
			AvatarActor avatarActor = target as AvatarActor;
			if (avatarActor != null && byTargetQTEName != null)
			{
				return avatarActor.CurrentQTEName == byTargetQTEName.targetQTEName;
			}
			return false;
		}

		public bool ByAttackerCategoryHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByAttackerCategory byAttackerCategory = predConfig as ByAttackerCategory;
			EvtBeingHit evtBeingHit = evt as EvtBeingHit;
			if (evtBeingHit != null)
			{
				BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtBeingHit.sourceID);
				if (actor != null)
				{
					ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(actor.runtimeID);
					if (byAttackerCategory.Category == num)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool ByAttackDataTypeHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByAttackDataType byAttackDataType = predConfig as ByAttackDataType;
			AttackResult attackResult = GetAttackResult(evt);
			if (attackResult == null)
			{
				return false;
			}
			if (byAttackDataType.Type == ByAttackDataType.AttackDataType.Breakable)
			{
				return attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light;
			}
			if (byAttackDataType.Type == ByAttackDataType.AttackDataType.EvadeDefendable)
			{
				return !attackResult.noTriggerEvadeAndDefend;
			}
			return false;
		}

		public bool ByAnyHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByAny byAny = predConfig as ByAny;
			for (int i = 0; i < byAny.Predicates.Length; i++)
			{
				if (EvaluateAbilityPredicate(byAny.Predicates[i], instancedAbility, instancedModifier, target, evt))
				{
					return true;
				}
			}
			return false;
		}

		public bool ByNotHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByNot byNot = predConfig as ByNot;
			for (int i = 0; i < byNot.Predicates.Length; i++)
			{
				if (EvaluateAbilityPredicate(byNot.Predicates[i], instancedAbility, instancedModifier, target, evt))
				{
					return false;
				}
			}
			return true;
		}

		public bool ByAvatarHasChargesLeftHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByAvatarHasChargesLeft byAvatarHasChargesLeft = (ByAvatarHasChargesLeft)predConfig;
			AvatarActor avatarActor = _owner as AvatarActor;
			return avatarActor.HasChargesLeft(byAvatarHasChargesLeft.CDSkillID);
		}

		public bool ByIsLocalAvatarHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return Singleton<AvatarManager>.Instance.IsLocalAvatar(_owner.runtimeID);
		}

		public bool ByIsPlayerAvatarHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return Singleton<AvatarManager>.Instance.IsPlayerAvatar(_owner.runtimeID);
		}

		public bool ByControlDataHasSteerHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByControlDataHasSteer byControlDataHasSteer = predConfig as ByControlDataHasSteer;
			BaseMonoAvatar baseMonoAvatar = _owner.entity as BaseMonoAvatar;
			return (!byControlDataHasSteer.HasSteer) ? (!baseMonoAvatar.GetActiveControlData().hasSteer) : baseMonoAvatar.GetActiveControlData().hasSteer;
		}

		public bool ByAnimatorBoolTrueHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByAnimatorBoolTrue byAnimatorBoolTrue = predConfig as ByAnimatorBoolTrue;
			if (target == null)
			{
				return false;
			}
			return (target.entity as BaseMonoAnimatorEntity).GetLocomotionBool(byAnimatorBoolTrue.Param);
		}

		public bool ByTargetIsSelfHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			ByTargetIsSelf byTargetIsSelf = predConfig as ByTargetIsSelf;
			if (target == null)
			{
				return false;
			}
			return !((target.runtimeID == _owner.runtimeID) ^ byTargetIsSelf.IsSelf);
		}

		public static void PostInitAbilityActorPlugin(BaseAbilityActor actor)
		{
			if (Singleton<LevelManager>.Instance.gameMode is NetworkedMP_Default_GameMode)
			{
				actor.mpAbilityPlugin = new MPActorAbilityPlugin(actor);
				actor.abilityPlugin = actor.mpAbilityPlugin;
				actor.AddPluginAs<ActorAbilityPlugin>(actor.abilityPlugin);
			}
			else
			{
				actor.abilityPlugin = new ActorAbilityPlugin(actor);
				actor.AddPlugin(actor.abilityPlugin);
			}
		}

		public override void Core()
		{
			for (int i = 0; i < _appliedAbilities.Count; i++)
			{
				ActorAbility actorAbility = _appliedAbilities[i];
				if (actorAbility != null)
				{
					CoreMixins(actorAbility.instancedMixins);
				}
			}
			for (int j = 0; j < _appliedModifiers.Count; j++)
			{
				ActorModifier actorModifier = _appliedModifiers[j];
				if (actorModifier != null)
				{
					CoreMixins(actorModifier.instancedMixins);
				}
			}
			for (int k = 0; k < _modifierDurationTimers.Count; k++)
			{
				if (_modifierDurationTimers[k] != null)
				{
					_modifierDurationTimers[k].Item2.Core(1f);
					if (_modifierDurationTimers[k].Item2.isTimeUp)
					{
						TryRemoveModifier(_modifierDurationTimers[k].Item1);
						_modifierDurationTimers[k] = null;
					}
				}
			}
			for (int l = 0; l < _modifierThinkTimers.Count; l++)
			{
				if (_modifierThinkTimers[l] == null)
				{
					continue;
				}
				_modifierThinkTimers[l].Item2.Core(1f);
				if (_modifierThinkTimers[l].Item2.isTimeUp)
				{
					HandleModifierActions(_modifierThinkTimers[l].Item1, null, null, (ConfigAbilityModifier config) => config.OnThinkInterval);
					_modifierThinkTimers[l].Item2.Reset(true);
				}
			}
		}

		public DisplayValue<float> CreateOrGetDisplayFloat(string key, float floor, float ceiling, float value)
		{
			if (_displayValueMap.ContainsKey(key))
			{
				return _displayValueMap[key];
			}
			DisplayValue<float> displayValue = new DisplayValue<float>(floor, ceiling, value);
			_displayValueMap.Add(key, displayValue);
			return displayValue;
		}

		public bool HasDisplayFloat(string key)
		{
			return _displayValueMap.ContainsKey(key);
		}

		public void SubAttachDisplayFloat(string key, Action<float, float> cb, ref float curValue, ref float floor, ref float ceiling)
		{
			DisplayValue<float> displayValue = _displayValueMap[key];
			displayValue.SubAttach(cb, ref curValue, ref floor, ref ceiling);
		}

		public void SubDetachDisplayFloat(string key, Action<float, float> cb)
		{
			DisplayValue<float> displayValue = _displayValueMap[key];
			displayValue.SubDetach(cb);
		}

		public DynamicActorValue<float> CreateOrGetDynamicFloat(string key, float value)
		{
			if (_dynamicValueMap.ContainsKey(key))
			{
				return _dynamicValueMap[key];
			}
			DynamicActorValue<float> dynamicActorValue = new DynamicActorValue<float>(value);
			_dynamicValueMap.Add(key, dynamicActorValue);
			return dynamicActorValue;
		}

		public bool HasDynamicFloat(string key)
		{
			return _dynamicValueMap.ContainsKey(key);
		}

		public void SubAttachDynamicFloat(string key, Action<float, float> cb, ref float curValue)
		{
			DynamicActorValue<float> dynamicActorValue = _dynamicValueMap[key];
			dynamicActorValue.SubAttach(cb, ref curValue);
		}

		public void SubDetachDynamicFloat(string key, Action<float, float> cb)
		{
			DynamicActorValue<float> dynamicActorValue = _dynamicValueMap[key];
			dynamicActorValue.SubDetach(cb);
		}

		private bool IsMuted()
		{
			return _isKilled || _isMuted;
		}

		private ActorModifier TryRecycleDeadModifier(ActorAbility ownerAbility, ConfigAbilityModifier config)
		{
			for (int i = 0; i < _deadModifiers.Count; i++)
			{
				if (_deadModifiers[i] != null && _deadModifiers[i].config == config && _deadModifiers[i].parentAbility == ownerAbility)
				{
					ActorModifier result = _deadModifiers[i];
					_deadModifiers[i] = null;
					return result;
				}
			}
			return null;
		}

		public List<ActorAbility> GetAppliedAbilities()
		{
			return (_appliedAbilities == null) ? new List<ActorAbility>() : _appliedAbilities;
		}

		public ActorAbility AddAbility(ConfigAbility abilityConfig)
		{
			return AddAbility(abilityConfig, EMPTY_OVERRIDE_MAP);
		}

		public virtual ActorAbility AddAbility(ConfigAbility abilityConfig, Dictionary<string, object> overrideMap)
		{
			if (IsMuted())
			{
				return null;
			}
			ActorAbility actorAbility = new ActorAbility(_owner, abilityConfig, overrideMap);
			_appliedAbilities.Add(actorAbility);
			int num = _appliedAbilities.Count - 1;
			actorAbility.instancedAbilityID = num + 1;
			HandleAbilityActions(actorAbility, null, null, (ConfigAbility config) => config.OnAdded);
			actorAbility.Attach();
			AddInstancedMixins(actorAbility.instancedMixins);
			return _appliedAbilities[num];
		}

		protected virtual void RemoveAbility(ActorAbility instancedAbility)
		{
			for (int i = 0; i < _appliedAbilities.Count; i++)
			{
				if (_appliedAbilities[i] == instancedAbility)
				{
					ActorAbility actorAbility = _appliedAbilities[i];
					HandleAbilityActions(actorAbility, null, null, (ConfigAbility config) => config.OnRemoved);
					RemoveInstancedMixins(actorAbility.instancedMixins);
					actorAbility.Detach();
					_appliedAbilities[i] = null;
				}
			}
		}

		public ActorModifier ApplyModifier(ActorAbility instancedAbility, string modifierName)
		{
			ConfigAbilityModifier modifierConfig = instancedAbility.config.Modifiers[modifierName];
			return ApplyModifier(instancedAbility, modifierConfig);
		}

		protected virtual ActorModifier ApplyModifier(ActorAbility instancedAbility, ConfigAbilityModifier modifierConfig)
		{
			if (IsMuted())
			{
				return null;
			}
			string modifierName = modifierConfig.ModifierName;
			if (modifierConfig.State != AbilityState.None && _owner.IsImmuneAbilityState(modifierConfig.State))
			{
				return null;
			}
			if (AbilityData.IsModifierDebuff(modifierConfig))
			{
				if (AbilityData.IsModifierDebuff(modifierConfig) && IsImmuneDebuff)
				{
					return null;
				}
				if (_owner is MonsterActor)
				{
					MonsterActor monsterActor = (MonsterActor)_owner;
					ConfigDebuffResistance debuffResistance = monsterActor.config.DebuffResistance;
					float resistanceRatio = debuffResistance.ResistanceRatio;
					float debuffResistanceRatio = monsterActor.config.EliteArguments.DebuffResistanceRatio;
					float resistanceRatio2 = monsterActor.GetResistanceRatio(modifierConfig.State);
					float value = UnityEngine.Random.value;
					float num = (1f - resistanceRatio) * (1f - debuffResistanceRatio) * resistanceRatio2;
					if (value >= num)
					{
						return null;
					}
				}
				else if (_owner is AvatarActor)
				{
					AvatarActor avatarActor = (AvatarActor)_owner;
					ConfigDebuffResistance debuffResistance2 = avatarActor.config.DebuffResistance;
					float resistanceRatio3 = debuffResistance2.ResistanceRatio;
					float resistanceRatio4 = avatarActor.GetResistanceRatio(modifierConfig.State);
					float value2 = UnityEngine.Random.value;
					float num2 = (1f - resistanceRatio3) * resistanceRatio4;
					if (value2 >= num2)
					{
						return null;
					}
				}
			}
			ActorModifier appliedModifier = GetAppliedModifier(modifierConfig, instancedAbility);
			if (appliedModifier != null)
			{
				if (modifierConfig.Stacking == ConfigAbilityModifier.ModifierStacking.Unique)
				{
					return null;
				}
				if (modifierConfig.Stacking != ConfigAbilityModifier.ModifierStacking.Multiple)
				{
					for (int i = 0; i < _modifierDurationTimers.Count; i++)
					{
						if (_modifierDurationTimers[i] != null && _modifierDurationTimers[i].Item1 == appliedModifier)
						{
							EntityTimer item = _modifierDurationTimers[i].Item2;
							float num3 = instancedAbility.Evaluate(modifierConfig.Duration);
							if (AbilityData.IsModifierDebuff(modifierConfig))
							{
								num3 *= 1f + _owner.GetProperty("Actor_DebuffDurationRatioDelta");
							}
							if (AbilityData.IsModifierBuff(modifierConfig) || AbilityData.IsModifierDebuff(modifierConfig))
							{
								num3 *= _owner.GetAbilityStateDurationRatio(modifierConfig.State);
							}
							if (modifierConfig.Stacking == ConfigAbilityModifier.ModifierStacking.Prolong)
							{
								item.timespan = item.timespan - item.timer + num3;
								item.Reset();
							}
							else if (modifierConfig.Stacking == ConfigAbilityModifier.ModifierStacking.Refresh)
							{
								item.Reset();
							}
						}
					}
					return null;
				}
			}
			if (modifierConfig.State != AbilityState.None)
			{
				AbilityState[] precedenceTrack = null;
				int stateIx = -1;
				AbilityData.GetStateIndiceInPrecedenceMap(modifierConfig.State, out precedenceTrack, out stateIx);
				for (int j = 0; j < AbilityData.ABILITY_STATE_PRECEDENCE_MAP.Length; j++)
				{
					AbilityState[] array = AbilityData.ABILITY_STATE_PRECEDENCE_MAP[j];
					for (int k = 0; k < array.Length; k++)
					{
						if (array[k] != modifierConfig.State)
						{
							continue;
						}
						precedenceTrack = array;
						stateIx = k;
						for (int l = 0; l < stateIx; l++)
						{
							int modifierIndexByState = GetModifierIndexByState(precedenceTrack[l]);
							ActorModifier actorModifier = ((modifierIndexByState >= 0) ? _appliedModifiers[modifierIndexByState] : null);
							if (actorModifier != null)
							{
								return null;
							}
						}
						for (int m = stateIx + 1; m < precedenceTrack.Length; m++)
						{
							RemoveModifierByState(precedenceTrack[m]);
						}
					}
				}
			}
			bool flag = false;
			if (modifierConfig.State != AbilityState.None && modifierConfig.IsDebuff)
			{
				int modifierIndexByState2 = GetModifierIndexByState(modifierConfig.State);
				ActorModifier actorModifier2 = ((modifierIndexByState2 >= 0) ? _appliedModifiers[modifierIndexByState2] : null);
				if (actorModifier2 != null)
				{
					if (!actorModifier2.config.IsDebuff)
					{
						return null;
					}
					_owner.AddAbilityState(modifierConfig.State, actorModifier2.config.MuteStateDisplayEffect);
					flag = true;
					RemoveModifier(actorModifier2, modifierIndexByState2);
				}
			}
			int index = _appliedModifiers.SeekAddPosition();
			appliedModifier = AddModifierOnIndex(instancedAbility, modifierConfig, index);
			if (flag)
			{
				_owner.RemoveAbilityState(modifierConfig.State);
			}
			BaseMonoEntity timeScaleEntity = null;
			if (modifierConfig.TimeScale == ConfigAbilityModifier.ModifierTimeScale.Owner)
			{
				timeScaleEntity = _owner.entity;
			}
			else if (modifierConfig.TimeScale == ConfigAbilityModifier.ModifierTimeScale.Caster)
			{
				timeScaleEntity = instancedAbility.caster.entity;
			}
			else if (modifierConfig.TimeScale == ConfigAbilityModifier.ModifierTimeScale.Level)
			{
				timeScaleEntity = _levelActor.levelEntity;
			}
			float num4 = instancedAbility.Evaluate(appliedModifier.config.Duration);
			if (modifierConfig.ApplyAttackerWitchTimeRatio && instancedAbility.CurrentTriggerEvent is EvtEvadeSuccess)
			{
				EvtEvadeSuccess evtEvadeSuccess = instancedAbility.CurrentTriggerEvent as EvtEvadeSuccess;
				if (evtEvadeSuccess != null)
				{
					MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evtEvadeSuccess.attackerID);
					if (actor != null)
					{
						ConfigMonsterAnimEvent configMonsterAnimEvent = SharedAnimEventData.ResolveAnimEvent(actor.config, evtEvadeSuccess.skillID);
						if (configMonsterAnimEvent != null)
						{
							num4 *= configMonsterAnimEvent.AttackProperty.WitchTimeRatio;
						}
					}
				}
			}
			if (num4 != 0f)
			{
				if (AbilityData.IsModifierDebuff(modifierConfig))
				{
					num4 *= 1f + _owner.GetProperty("Actor_DebuffDurationRatioDelta");
				}
				if (AbilityData.IsModifierBuff(modifierConfig) || AbilityData.IsModifierDebuff(modifierConfig))
				{
					num4 *= _owner.GetAbilityStateDurationRatio(modifierConfig.State);
				}
				int index2 = _modifierDurationTimers.SeekAddPosition();
				_modifierDurationTimers[index2] = Tuple.Create(appliedModifier, new EntityTimer(num4, timeScaleEntity));
				_modifierDurationTimers[index2].Item2.SetActive(true);
			}
			float num5 = instancedAbility.Evaluate(appliedModifier.config.ThinkInterval);
			if (num5 != 0f)
			{
				int index3 = _modifierThinkTimers.SeekAddPosition();
				_modifierThinkTimers[index3] = Tuple.Create(appliedModifier, new EntityTimer(num5, timeScaleEntity));
				_modifierThinkTimers[index3].Item2.SetActive(true);
			}
			HandleModifierActions(appliedModifier, null, null, (ConfigAbilityModifier config) => config.OnAdded);
			if (appliedModifier.config.OnMonsterCreated.Length > 0)
			{
				Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(_owner.runtimeID);
			}
			if (appliedModifier.config.OnAvatarCreated.Length > 0)
			{
				Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(_owner.runtimeID);
			}
			return appliedModifier;
		}

		private ActorModifier GetAppliedModifier(ConfigAbilityModifier modifierConfig, ActorAbility ownerAbility)
		{
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null && _appliedModifiers[i].config == modifierConfig && _appliedModifiers[i].parentAbility == ownerAbility)
				{
					return _appliedModifiers[i];
				}
			}
			return null;
		}

		protected virtual void RemoveModifier(ActorModifier modifier, int index)
		{
			HandleModifierActions(modifier, null, null, (ConfigAbilityModifier config) => config.OnRemoved);
			RemoveModifierOnIndex(modifier, index);
			for (int num = 0; num < _modifierThinkTimers.Count; num++)
			{
				if (_modifierThinkTimers[num] != null && _modifierThinkTimers[num].Item1 == modifier)
				{
					_modifierThinkTimers[num] = null;
				}
			}
			for (int num2 = 0; num2 < _modifierDurationTimers.Count; num2++)
			{
				if (_modifierDurationTimers[num2] != null && _modifierDurationTimers[num2].Item1 == modifier)
				{
					_modifierDurationTimers[num2] = null;
				}
			}
			if (modifier.config.OnMonsterCreated.Length > 0)
			{
				Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(_owner.runtimeID);
			}
			if (modifier.config.OnAvatarCreated.Length > 0)
			{
				Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarCreated>(_owner.runtimeID);
			}
		}

		public bool TryRemoveModifier(ActorModifier modifier)
		{
			if (modifier == null)
			{
				return false;
			}
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] == modifier)
				{
					RemoveModifier(modifier, i);
					return true;
				}
			}
			return false;
		}

		public bool TryRemoveModifier(ActorAbility instancedAbility, string modifierName)
		{
			ConfigAbilityModifier configAbilityModifier = instancedAbility.config.Modifiers[modifierName];
			bool result = false;
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null && _appliedModifiers[i].config == configAbilityModifier)
				{
					RemoveModifier(_appliedModifiers[i], i);
					if (configAbilityModifier.Stacking != ConfigAbilityModifier.ModifierStacking.Multiple)
					{
						return true;
					}
					result = true;
				}
			}
			return result;
		}

		public bool HasAbility(string abilityName)
		{
			if (string.IsNullOrEmpty(abilityName))
			{
				return false;
			}
			if (_appliedAbilities == null)
			{
				return false;
			}
			for (int i = 0; i < _appliedAbilities.Count; i++)
			{
				if (_appliedAbilities[i] != null && _appliedAbilities[i].config != null && _appliedAbilities[i].config.AbilityName == abilityName)
				{
					return true;
				}
			}
			return false;
		}

		public override void OnAdded()
		{
			if (_levelActor.levelState == LevelActor.LevelState.LevelRunning)
			{
				AddAppliedAbilities();
				return;
			}
			Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(_owner.runtimeID);
			_waitForStageReady = true;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (muteEvents)
			{
				return false;
			}
			bool flag = false;
			string text = null;
			EvtAbilityStart evtAbilityStart = null;
			if (evt is EvtAbilityStart)
			{
				evtAbilityStart = (EvtAbilityStart)evt;
				if (evtAbilityStart.abilityName == null)
				{
					evtAbilityStart.abilityName = _owner.GetAbilityNameByID(evtAbilityStart.abilityID);
				}
				text = evtAbilityStart.abilityName;
				if (text == "Noop")
				{
					return false;
				}
			}
			for (int i = 0; i < _appliedAbilities.Count; i++)
			{
				ActorAbility actorAbility = _appliedAbilities[i];
				if (actorAbility == null)
				{
					continue;
				}
				bool flag2 = actorAbility.config.AbilityName == text;
				for (int j = 0; j < actorAbility.instancedMixins.Length; j++)
				{
					flag |= EventInstancedMixin(actorAbility.instancedMixins[j], evt);
					if (flag2)
					{
						flag = true;
						if (actorAbility.config.UseAbilityArgumentAsSpecialKey != null)
						{
							actorAbility.argumentRecieved = true;
							actorAbility.argumentSpecialValue = (float)evtAbilityStart.abilityArgument;
						}
						else if (actorAbility.config.SetAbilityArgumentToOverrideMap != null)
						{
							actorAbility.SetOverrideMapValue(actorAbility.config.SetAbilityArgumentToOverrideMap, evtAbilityStart.abilityArgument);
						}
						AbilityStartInstancedMixin(actorAbility.instancedMixins[j], evtAbilityStart);
						actorAbility.argumentRecieved = false;
					}
				}
			}
			for (int k = 0; k < _appliedModifiers.Count; k++)
			{
				ActorModifier actorModifier = _appliedModifiers[k];
				if (actorModifier == null)
				{
					continue;
				}
				bool flag3 = actorModifier.parentAbility.config.AbilityName == text;
				for (int l = 0; l < actorModifier.instancedMixins.Length; l++)
				{
					flag |= EventInstancedMixin(actorModifier.instancedMixins[l], evt);
					if (flag3)
					{
						flag = true;
						AbilityStartInstancedMixin(actorModifier.instancedMixins[l], evtAbilityStart);
					}
				}
			}
			if (evt is EvtAbilityStart)
			{
				for (int m = 0; m < _appliedAbilities.Count; m++)
				{
					ActorAbility actorAbility2 = _appliedAbilities[m];
					if (actorAbility2 != null && actorAbility2.config.AbilityName == text)
					{
						if (actorAbility2.config.UseAbilityArgumentAsSpecialKey != null)
						{
							actorAbility2.argumentRecieved = true;
							actorAbility2.argumentSpecialValue = (float)evtAbilityStart.abilityArgument;
						}
						else if (actorAbility2.config.SetAbilityArgumentToOverrideMap != null)
						{
							actorAbility2.SetOverrideMapValue(actorAbility2.config.SetAbilityArgumentToOverrideMap, evtAbilityStart.abilityArgument);
						}
						flag |= HandleAbilityActions(actorAbility2, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtAbilityStart.otherID), evt, (ConfigAbility config) => config.OnAbilityStart);
						actorAbility2.argumentRecieved = false;
					}
				}
			}
			else if (evt is EvtBeingHit)
			{
				EvtBeingHit evtBeingHit = (EvtBeingHit)evt;
				flag |= HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtBeingHit.sourceID), evt, (ConfigAbilityModifier config) => config.OnBeingHit);
			}
			else if (evt is EvtAttackLanded)
			{
				EvtAttackLanded evtAttackLanded = (EvtAttackLanded)evt;
				flag |= HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtAttackLanded.attackeeID), evt, (ConfigAbilityModifier config) => config.OnAttackLanded);
			}
			else if (evt is EvtEvadeStart)
			{
				flag |= HandleModifierActions(null, evt, (ConfigAbilityModifier config) => config.OnEvadeStart);
			}
			else if (evt is EvtEvadeSuccess)
			{
				EvtEvadeSuccess evtEvadeSuccess = (EvtEvadeSuccess)evt;
				flag |= HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtEvadeSuccess.attackerID), evt, (ConfigAbilityModifier config) => config.OnEvadeSuccess);
			}
			else if (evt is EvtDefendStart)
			{
				flag |= HandleModifierActions(null, evt, (ConfigAbilityModifier config) => config.OnDefendStart);
			}
			else if (evt is EvtDefendSuccess)
			{
				EvtDefendSuccess evtDefendSuccess = (EvtDefendSuccess)evt;
				flag |= HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtDefendSuccess.attackerID), evt, (ConfigAbilityModifier config) => config.OnDefendSuccess);
			}
			else if (evt is EvtFieldEnter)
			{
				EvtFieldEnter evtFieldEnter = (EvtFieldEnter)evt;
				BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtFieldEnter.otherID);
				if (actor != null)
				{
					flag |= HandleAbilityActions(actor, evt, (ConfigAbility config) => config.OnFieldEnter);
				}
			}
			else if (evt is EvtFieldExit)
			{
				EvtFieldExit evtFieldExit = (EvtFieldExit)evt;
				BaseAbilityActor actor2 = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtFieldExit.otherID);
				if (actor2 != null)
				{
					flag |= HandleAbilityActions(actor2, evt, (ConfigAbility config) => config.OnFieldExit);
				}
			}
			else if (evt is EvtKilled)
			{
				BaseAbilityActor actor3 = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(((EvtKilled)evt).killerID);
				HandleModifierActions(actor3, evt, (ConfigAbilityModifier config) => config.OnKilled);
				HandleAbilityActions(actor3, evt, (ConfigAbility config) => config.OnKilled);
				return OnKilled((EvtKilled)evt);
			}
			return flag;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			bool flag = false;
			for (int i = 0; i < _appliedAbilities.Count; i++)
			{
				ActorAbility actorAbility = _appliedAbilities[i];
				if (actorAbility != null)
				{
					for (int j = 0; j < actorAbility.instancedMixins.Length; j++)
					{
						flag |= actorAbility.instancedMixins[j].ListenEvent(evt);
					}
				}
			}
			for (int k = 0; k < _appliedModifiers.Count; k++)
			{
				ActorModifier actorModifier = _appliedModifiers[k];
				if (actorModifier != null)
				{
					for (int l = 0; l < actorModifier.instancedMixins.Length; l++)
					{
						flag |= actorModifier.instancedMixins[l].ListenEvent(evt);
					}
				}
			}
			if (evt is EvtMonsterCreated)
			{
				EvtMonsterCreated evtMonsterCreated = (EvtMonsterCreated)evt;
				HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtMonsterCreated.monsterID), evt, (ConfigAbilityModifier config) => config.OnMonsterCreated);
			}
			else if (evt is EvtAvatarCreated)
			{
				EvtAvatarCreated evtAvatarCreated = (EvtAvatarCreated)evt;
				HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtAvatarCreated.avatarID), evt, (ConfigAbilityModifier config) => config.OnAvatarCreated);
			}
			else if (evt is EvtStageReady)
			{
				ListenStageReady((EvtStageReady)evt);
			}
			return flag;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (muteEvents)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < _appliedAbilities.Count; i++)
			{
				ActorAbility actorAbility = _appliedAbilities[i];
				if (actorAbility != null)
				{
					for (int j = 0; j < actorAbility.instancedMixins.Length; j++)
					{
						flag |= PostEventInstancedMixin(actorAbility.instancedMixins[j], evt);
					}
				}
			}
			for (int k = 0; k < _appliedModifiers.Count; k++)
			{
				ActorModifier actorModifier = _appliedModifiers[k];
				if (actorModifier != null)
				{
					for (int l = 0; l < actorModifier.instancedMixins.Length; l++)
					{
						flag |= PostEventInstancedMixin(actorModifier.instancedMixins[l], evt);
					}
				}
			}
			return flag;
		}

		public override bool OnResolvedEvent(BaseEvent evt)
		{
			if (muteEvents)
			{
				return false;
			}
			bool flag = false;
			if (evt is EvtBeingHit)
			{
				EvtBeingHit evtBeingHit = (EvtBeingHit)evt;
				flag |= HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtBeingHit.sourceID), evt, (ConfigAbilityModifier config) => config.OnBeingHitResolved);
			}
			return flag;
		}

		private bool OnKilled(EvtKilled evt)
		{
			_isKilled = true;
			if (onKillBehavior == OnKillBehavior.RemoveAll)
			{
				RemoveAllNonOnDestroyAbilities();
				RemoveAllModifies();
			}
			else if (onKillBehavior == OnKillBehavior.RemoveAllDebuffsAndDurationed)
			{
				RemoveAllDurationedOrDebuffs();
			}
			else if (onKillBehavior != OnKillBehavior.DoNotRemoveUntilDestroyed)
			{
			}
			return true;
		}

		private bool ListenStageReady(EvtStageReady evt)
		{
			if (_waitForStageReady)
			{
				AddAppliedAbilities();
				Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(_owner.runtimeID);
				_waitForStageReady = false;
			}
			return true;
		}

		public void InsertPreInitAbility(ConfigAbility abilityConfig)
		{
			_additionalAbilities.Add(abilityConfig);
		}

		protected virtual void AddAppliedAbilities()
		{
			List<Tuple<ConfigAbility, Dictionary<string, object>>> appliedAbilities = _owner.appliedAbilities;
			for (int i = 0; i < appliedAbilities.Count; i++)
			{
				AddAbility(appliedAbilities[i].Item1, appliedAbilities[i].Item2);
			}
			for (int j = 0; j < _additionalAbilities.Count; j++)
			{
				AddAbility(_additionalAbilities[j]);
			}
			_additionalAbilities = null;
		}

		protected virtual void RemoveAllAbilities()
		{
			for (int i = 0; i < _appliedAbilities.Count; i++)
			{
				if (_appliedAbilities[i] != null)
				{
					RemoveAbility(_appliedAbilities[i]);
				}
			}
		}

		protected virtual void RemoveAllNonOnDestroyAbilities()
		{
			for (int i = 0; i < _appliedAbilities.Count; i++)
			{
				if (_appliedAbilities[i] != null && _appliedAbilities[i].config.OnDestroy.Length <= 0)
				{
					RemoveAbility(_appliedAbilities[i]);
				}
			}
		}

		protected virtual void RemoveAllModifies()
		{
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null)
				{
					RemoveModifier(_appliedModifiers[i], i);
				}
			}
		}

		public void ResetKilled()
		{
			_isKilled = false;
		}

		public override void OnRemoved()
		{
			if (Singleton<LevelManager>.Instance.levelActor.levelState != LevelActor.LevelState.LevelRunning)
			{
				return;
			}
			HandleAbilityActions(null, null, (ConfigAbility config) => config.OnDestroy);
			RemoveAllModifies();
			RemoveAllAbilities();
			foreach (DisplayValue<float> value in _displayValueMap.Values)
			{
				value.Dispose();
			}
		}

		public void StopAndDropAll()
		{
			RemoveAllModifies();
			RemoveAllAbilities();
			_isMuted = true;
		}

		protected virtual bool HandleAbilityActions(ActorAbility instancedAbility, BaseAbilityActor other, BaseEvent evt, Func<ConfigAbility, ConfigAbilityAction[]> actionsGetter)
		{
			ConfigAbilityAction[] array = actionsGetter(instancedAbility.config);
			for (int i = 0; i < array.Length; i++)
			{
				HandleActionTargetDispatch(array[i], instancedAbility, null, other, evt);
			}
			return array.Length > 0;
		}

		protected virtual bool HandleAbilityActions(BaseAbilityActor other, BaseEvent evt, Func<ConfigAbility, ConfigAbilityAction[]> actionsGetter)
		{
			bool flag = false;
			for (int i = 0; i < _appliedAbilities.Count; i++)
			{
				if (_appliedAbilities[i] != null)
				{
					ActorAbility actorAbility = _appliedAbilities[i];
					ConfigAbilityAction[] array = actionsGetter(actorAbility.config);
					flag |= array.Length > 0;
					for (int j = 0; j < array.Length; j++)
					{
						HandleActionTargetDispatch(array[j], actorAbility, null, other, evt);
					}
				}
			}
			return flag;
		}

		protected virtual bool HandleModifierActions(ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt, Func<ConfigAbilityModifier, ConfigAbilityAction[]> actionsGetter)
		{
			ConfigAbilityAction[] array = actionsGetter(instancedModifier.config);
			for (int i = 0; i < array.Length; i++)
			{
				HandleActionTargetDispatch(array[i], instancedModifier.parentAbility, instancedModifier, other, evt);
			}
			return array.Length > 0;
		}

		protected virtual bool HandleModifierActions(BaseAbilityActor other, BaseEvent evt, Func<ConfigAbilityModifier, ConfigAbilityAction[]> actionsGetter)
		{
			bool flag = false;
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null)
				{
					ActorModifier actorModifier = _appliedModifiers[i];
					ConfigAbilityAction[] array = actionsGetter(actorModifier.config);
					flag |= array.Length > 0;
					for (int j = 0; j < array.Length; j++)
					{
						HandleActionTargetDispatch(array[j], actorModifier.parentAbility, actorModifier, other, evt);
					}
				}
			}
			return flag;
		}

		public void HandleActionTargetDispatch(ConfigAbilityAction[] actionConfigs, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt)
		{
			for (int i = 0; i < actionConfigs.Length; i++)
			{
				HandleActionTargetDispatch(actionConfigs[i], instancedAbility, instancedModifier, other, evt);
			}
		}

		private static bool YesTargetPredicate(BaseAbilityActor target)
		{
			return true;
		}

		protected virtual void HandleActionTargetDispatch(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt, Func<BaseAbilityActor, bool> targetPredicate)
		{
			BaseAbilityActor outTarget;
			BaseAbilityActor[] outTargetLs;
			bool needHandleTargetOnNull;
			ResolveTarget(actionConfig.Target, actionConfig.TargetOption, instancedAbility, other, out outTarget, out outTargetLs, out needHandleTargetOnNull);
			if ((outTarget != null && targetPredicate(outTarget)) || needHandleTargetOnNull)
			{
				HandleAction(actionConfig, instancedAbility, instancedModifier, outTarget, evt);
			}
			else
			{
				if (outTargetLs == null)
				{
					return;
				}
				for (int i = 0; i < outTargetLs.Length; i++)
				{
					if (outTargetLs[i] != null && targetPredicate(outTargetLs[i]))
					{
						HandleAction(actionConfig, instancedAbility, instancedModifier, outTargetLs[i], evt);
					}
				}
			}
		}

		private void HandleActionTargetDispatch(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt)
		{
			HandleActionTargetDispatch(actionConfig, instancedAbility, instancedModifier, other, evt, (BaseAbilityActor actionTarget) => YesTargetPredicate(actionTarget));
		}

		private BaseAbilityActor CheckTargetAvailable(BaseAbilityActor target, bool includeGhost = false)
		{
			if (target != null && target.entity != null)
			{
				if (!includeGhost && target.entity.isGhost)
				{
					return null;
				}
				return target;
			}
			return null;
		}

		public BaseAbilityActor[] FilterTargetArray(BaseAbilityActor[] targets, bool includeGhost = false)
		{
			for (int i = 0; i < targets.Length; i++)
			{
				targets[i] = CheckTargetAvailable(targets[i], includeGhost);
			}
			return targets;
		}

		protected void ResolveTarget(AbilityTargetting targetting, TargettingOption option, ActorAbility instancedAbility, BaseAbilityActor other, out BaseAbilityActor outTarget, out BaseAbilityActor[] outTargetLs, out bool needHandleTargetOnNull)
		{
			BaseAbilityActor caster = instancedAbility.caster;
			BaseAbilityActor owner = _owner;
			BaseAbilityActor baseAbilityActor = ((caster == owner) ? other : owner);
			switch (targetting)
			{
			case AbilityTargetting.Self:
				outTarget = owner;
				outTargetLs = null;
				needHandleTargetOnNull = false;
				break;
			case AbilityTargetting.Caster:
				outTarget = caster;
				outTargetLs = null;
				needHandleTargetOnNull = false;
				break;
			case AbilityTargetting.Target:
				outTarget = CheckTargetAvailable(baseAbilityActor, true);
				outTargetLs = null;
				needHandleTargetOnNull = false;
				break;
			case AbilityTargetting.Creator:
				outTarget = CheckTargetAvailable(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(owner.ownerID), true);
				outTargetLs = null;
				needHandleTargetOnNull = false;
				break;
			case AbilityTargetting.CasterAllied:
				outTarget = null;
				outTargetLs = FilterTargetArray(Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(caster), true);
				needHandleTargetOnNull = false;
				break;
			case AbilityTargetting.TargetAllied:
				outTarget = null;
				outTargetLs = FilterTargetArray(Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(baseAbilityActor));
				needHandleTargetOnNull = false;
				break;
			case AbilityTargetting.EnemyAllied:
				outTarget = null;
				outTargetLs = FilterTargetArray(Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(caster));
				needHandleTargetOnNull = false;
				break;
			case AbilityTargetting.Other:
				outTarget = other;
				outTargetLs = null;
				needHandleTargetOnNull = true;
				break;
			case AbilityTargetting.CasterCenteredAllied:
				outTarget = null;
				if (!caster.IsActive())
				{
					outTargetLs = null;
				}
				else
				{
					outTargetLs = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(caster);
					for (int j = 0; j < outTargetLs.Length; j++)
					{
						if (!outTargetLs[j].IsActive() || CheckTargetAvailable(outTargetLs[j], true) == null || Miscs.DistancForVec3IgnoreY(outTargetLs[j].entity.XZPosition, caster.entity.XZPosition) > instancedAbility.Evaluate(option.Range))
						{
							outTargetLs[j] = null;
						}
					}
				}
				needHandleTargetOnNull = false;
				break;
			case AbilityTargetting.CasterCenteredEnemies:
				outTarget = null;
				if (!caster.IsActive())
				{
					outTargetLs = null;
				}
				else
				{
					outTargetLs = Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(caster);
					for (int l = 0; l < outTargetLs.Length; l++)
					{
						if (!outTargetLs[l].IsActive() || CheckTargetAvailable(outTargetLs[l]) == null || Miscs.DistancForVec3IgnoreY(outTargetLs[l].entity.XZPosition, caster.entity.XZPosition) > instancedAbility.Evaluate(option.Range))
						{
							outTargetLs[l] = null;
						}
					}
				}
				needHandleTargetOnNull = false;
				break;
			case AbilityTargetting.TargetCenteredAllied:
			{
				needHandleTargetOnNull = false;
				outTarget = null;
				if (baseAbilityActor == null || !baseAbilityActor.IsActive())
				{
					outTargetLs = null;
					break;
				}
				outTargetLs = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(caster);
				for (int k = 0; k < outTargetLs.Length; k++)
				{
					if (!outTargetLs[k].IsActive() || (CheckTargetAvailable(outTargetLs[k], true) == null && Miscs.DistancForVec3IgnoreY(outTargetLs[k].entity.XZPosition, baseAbilityActor.entity.XZPosition) > instancedAbility.Evaluate(option.Range)))
					{
						outTargetLs[k] = null;
					}
				}
				break;
			}
			case AbilityTargetting.TargetCenteredEnemies:
			{
				needHandleTargetOnNull = false;
				outTarget = null;
				if (baseAbilityActor == null || !baseAbilityActor.IsActive())
				{
					outTargetLs = null;
					break;
				}
				outTargetLs = Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(caster);
				for (int i = 0; i < outTargetLs.Length; i++)
				{
					if (!outTargetLs[i].IsActive() || CheckTargetAvailable(outTargetLs[i]) == null || Miscs.DistancForVec3IgnoreY(outTargetLs[i].entity.XZPosition, baseAbilityActor.entity.XZPosition) > instancedAbility.Evaluate(option.Range))
					{
						outTargetLs[i] = null;
					}
				}
				break;
			}
			default:
				outTarget = null;
				outTargetLs = null;
				needHandleTargetOnNull = false;
				break;
			}
		}

		public bool EvaluateAbilityPredicate(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return predConfig.Call(this, instancedAbility, instancedModifier, target, evt);
		}

		public bool EvaluateAbilityPredicate(ConfigAbilityPredicate[] predConfigs, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			bool flag = true;
			for (int i = 0; i < predConfigs.Length; i++)
			{
				flag &= EvaluateAbilityPredicate(predConfigs[i], instancedAbility, instancedModifier, target, evt);
			}
			return flag;
		}

		protected virtual void HandleAction(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (EvaluateAbilityPredicate(actionConfig.Predicates, instancedAbility, instancedModifier, target, evt))
			{
				actionConfig.Call(this, actionConfig, instancedAbility, instancedModifier, target, evt);
			}
		}

		private uint GetOtherID(BaseEvent evt)
		{
			IEvtWithOtherID evtWithOtherID = evt as IEvtWithOtherID;
			if (evtWithOtherID != null)
			{
				return evtWithOtherID.GetOtherID();
			}
			return 0u;
		}

		private AttackResult GetAttackResult(BaseEvent evt)
		{
			IEvtWithAttackResult evtWithAttackResult = evt as IEvtWithAttackResult;
			if (evtWithAttackResult != null)
			{
				return evtWithAttackResult.GetAttackResult();
			}
			return null;
		}

		private string GetAnimEventID(BaseEvent evt)
		{
			IEvtWithAnimEventID evtWithAnimEventID = evt as IEvtWithAnimEventID;
			if (evtWithAnimEventID != null)
			{
				return evtWithAnimEventID.GetAnimEventID();
			}
			return null;
		}

		private AttackResult.HitCollsion GetHitCollision(BaseEvent evt)
		{
			IEvtWithHitCollision evtWithHitCollision = evt as IEvtWithHitCollision;
			if (evtWithHitCollision != null)
			{
				return evtWithHitCollision.GetHitCollision();
			}
			return null;
		}

		public void RemoveAllDebuffModifiers()
		{
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null && AbilityData.IsModifierDebuff(_appliedModifiers[i].config))
				{
					RemoveModifier(_appliedModifiers[i], i);
				}
			}
		}

		private bool IsModifierDurationed(ActorModifier modifier)
		{
			for (int i = 0; i < _modifierDurationTimers.Count; i++)
			{
				if (_modifierDurationTimers[i] != null && _modifierDurationTimers[i].Item1 == modifier)
				{
					return true;
				}
			}
			return false;
		}

		private void RemoveAllDurationedOrDebuffs()
		{
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null && (IsModifierDurationed(_appliedModifiers[i]) || AbilityData.IsModifierDebuff(_appliedModifiers[i].config)))
				{
					RemoveModifier(_appliedModifiers[i], i);
				}
			}
		}

		private int GetModifierIndexByState(AbilityState state)
		{
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null && _appliedModifiers[i].config.State == state)
				{
					return i;
				}
			}
			return -1;
		}

		public void RemoveModifierByState(AbilityState state)
		{
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null && _appliedModifiers[i].config.State == state)
				{
					RemoveModifier(_appliedModifiers[i], i);
				}
			}
		}

		public ActorModifier GetFirstUniqueModifier(string uniqueModifierName)
		{
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null && _appliedModifiers[i].config.IsUnique && _appliedModifiers[i].config.ModifierName == uniqueModifierName)
				{
					return _appliedModifiers[i];
				}
			}
			return null;
		}

		public void AddOrGetAbilityAndTriggerOnTarget(ConfigAbility abilityConfig, uint targetID, object abilityArgument)
		{
			if (abilityConfig == null)
			{
				SuperDebug.VeryImportantError("AbilityConfig is Empty in AddOrGetAbilityAndTriggerOnTarget");
				return;
			}
			if (string.IsNullOrEmpty(abilityConfig.AbilityName))
			{
				SuperDebug.VeryImportantError("AbilityConfig should have a abilityName");
				return;
			}
			if (!string.IsNullOrEmpty(abilityConfig.AbilityName) && !HasAbility(abilityConfig.AbilityName))
			{
				AddAbility(abilityConfig);
			}
			EvtAbilityStart evtAbilityStart = new EvtAbilityStart(_owner.runtimeID, targetID);
			evtAbilityStart.abilityName = abilityConfig.AbilityName;
			evtAbilityStart.abilityArgument = abilityArgument;
			Singleton<EventManager>.Instance.FireEvent(evtAbilityStart);
		}

		public EntityTimer DebugGetModifierTimer(ActorModifier instancedModifier)
		{
			foreach (Tuple<ActorModifier, EntityTimer> modifierDurationTimer in _modifierDurationTimers)
			{
				if (modifierDurationTimer == null || modifierDurationTimer.Item1 != instancedModifier)
				{
					continue;
				}
				return modifierDurationTimer.Item2;
			}
			return null;
		}

		[Conditional("NG_HSOD_DEBUG")]
		[Conditional("UNITY_EDITOR")]
		protected void DebugLogAbility(ActorAbility instancedAbility, string format, params object[] arguments)
		{
		}

		public virtual BaseAbilityMixin CreateInstancedAbilityMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
		{
			return config.CreateInstancedMixin(instancedAbility, instancedModifier);
		}

		protected virtual void AddInstancedMixins(BaseAbilityMixin[] mixins)
		{
			for (int i = 0; i < mixins.Length; i++)
			{
				mixins[i].OnAdded();
			}
		}

		protected virtual void RemoveInstancedMixins(BaseAbilityMixin[] mixins)
		{
			for (int i = 0; i < mixins.Length; i++)
			{
				mixins[i].OnRemoved();
			}
		}

		protected virtual void AbilityStartInstancedMixin(BaseAbilityMixin mixin, EvtAbilityStart evt)
		{
			mixin.OnAbilityTriggered(evt);
		}

		protected virtual bool EventInstancedMixin(BaseAbilityMixin mixin, BaseEvent evt)
		{
			return mixin.OnEvent(evt);
		}

		protected virtual bool PostEventInstancedMixin(BaseAbilityMixin mixin, BaseEvent evt)
		{
			return mixin.OnPostEvent(evt);
		}

		protected virtual void CoreMixins(BaseAbilityMixin[] mixins)
		{
			for (int i = 0; i < mixins.Length; i++)
			{
				mixins[i].Core();
			}
		}

		protected virtual ActorModifier AddModifierOnIndex(ActorAbility instancedAbility, ConfigAbilityModifier modifierConfig, int index)
		{
			string modifierName = modifierConfig.ModifierName;
			ActorModifier actorModifier = TryRecycleDeadModifier(instancedAbility, modifierConfig);
			if (actorModifier == null)
			{
				actorModifier = new ActorModifier(instancedAbility, _owner, instancedAbility.config.Modifiers[modifierName]);
			}
			_appliedModifiers.ExpandToInclude(index);
			_appliedModifiers[index] = actorModifier;
			actorModifier.instancedModifierID = index + 1;
			actorModifier.Attach();
			AddInstancedMixins(actorModifier.instancedMixins);
			return actorModifier;
		}

		protected virtual void RemoveModifierOnIndex(ActorModifier modifier, int index)
		{
			RemoveInstancedMixins(modifier.instancedMixins);
			modifier.Detach();
			_appliedModifiers[index] = null;
			int index2 = _deadModifiers.SeekAddPosition();
			_deadModifiers[index2] = modifier;
			modifier.instancedModifierID = 0;
		}
	}
}
