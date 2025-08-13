using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterDefendMixin : BaseAbilityMixin
	{
		private MonsterDefendMixin config;

		private float _defendAngle;

		private EntityTimer _defendActionTimer;

		private bool _defendCDAllowed;

		private bool _isInDefendState;

		private EntityTimer _inDefendStateTimer;

		private bool _allowDefendActions;

		private float _breakDefendAniDamageRatio = 1f;

		private float _defendActionRange = 1f;

		private float _defendActionChance = 1f;

		private int _shieldLightLayer = 3;

		private bool _allowLayerControl;

		private float _shieldLightMax = 1f;

		private float _shieldLightMin;

		private float _layerWeightTransitionDuration = 0.3f;

		private BaseMonoAnimatorEntity _animatorEntity;

		public AbilityMonsterDefendMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterDefendMixin)config;
			_defendAngle = instancedAbility.Evaluate(this.config.DefendAngle);
			_defendActionTimer = new EntityTimer(instancedAbility.Evaluate(this.config.DefendActionCD));
			_inDefendStateTimer = new EntityTimer(instancedAbility.Evaluate(this.config.InDefendMaxTime));
			_breakDefendAniDamageRatio = instancedAbility.Evaluate(this.config.BreakDefendAniDamageRatio);
			_defendActionRange = instancedAbility.Evaluate(this.config.DefendActionRange);
			_defendActionChance = instancedAbility.Evaluate(this.config.DefendActionChance);
			_defendActionChance = Mathf.Clamp(_defendActionChance, 0f, 1f);
			_defendActionTimer.SetActive(this.config.DefendSuccessActions.Length > 0);
			_shieldLightLayer = this.config.ShieldLightLayer;
			_shieldLightMax = Mathf.Clamp(this.config.ShieldLightMax, 0f, 1f);
			_shieldLightMin = Mathf.Clamp(this.config.ShieldLightMin, 0f, 1f);
			_allowLayerControl = this.config.AllowLayerLightControl;
			_animatorEntity = (BaseMonoAnimatorEntity)entity;
		}

		public override void OnAdded()
		{
			if (config.ResetCDSkillIDs != null)
			{
				BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
				baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			}
			StartDefend();
		}

		public override void OnRemoved()
		{
			if (config.ResetCDSkillIDs != null)
			{
				BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
				baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			}
		}

		public override void Core()
		{
			_defendActionTimer.Core(1f);
			_inDefendStateTimer.Core(1f);
			if (_defendActionTimer.isTimeUp || _inDefendStateTimer.isTimeUp)
			{
				_defendCDAllowed = true;
				_defendActionTimer.Reset(false);
				_inDefendStateTimer.Reset(false);
				DefendActionReady();
			}
			actor.entity.ResetTrigger(config.DefendTriggerID);
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (_defendActionTimer != null)
			{
				_defendActionTimer.Reset(true);
			}
			StartDefend();
		}

		private void StartDefend()
		{
			if (config != null && config.DefendStartActions != null)
			{
				if (config.DefendStartActions.Length > 0)
				{
					actor.abilityPlugin.HandleActionTargetDispatch(config.DefendStartActions, instancedAbility, instancedModifier, actor, null);
				}
				if (_allowLayerControl && _animatorEntity != null)
				{
					_animatorEntity.StartFadeAnimatorLayerWeight(_shieldLightLayer, _shieldLightMin, _layerWeightTransitionDuration);
				}
			}
		}

		private void DefendActionReady()
		{
			if (config.DefendActionReadyActions.Length > 0)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.DefendActionReadyActions, instancedAbility, instancedModifier, actor, null);
			}
			if (_allowLayerControl)
			{
				_animatorEntity.StartFadeAnimatorLayerWeight(_shieldLightLayer, _shieldLightMax, _layerWeightTransitionDuration);
			}
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
			MonsterActor monsterActor = actor as MonsterActor;
			BaseMonoMonster baseMonoMonster = entity as BaseMonoMonster;
			if (baseAbilityActor != null && baseMonoMonster != null)
			{
				baseMonoMonster.hasArmor = false;
			}
			if (monsterActor != null && baseAbilityActor != null)
			{
				if (evt.attackData.attackerAniDamageRatio > _breakDefendAniDamageRatio)
				{
					return true;
				}
				string currentSkillID = monsterActor.monster.CurrentSkillID;
				if (!string.IsNullOrEmpty(currentSkillID))
				{
					bool flag = false;
					for (int i = 0; i < config.DefendSkillIDs.Length; i++)
					{
						if (config.DefendSkillIDs[i] == currentSkillID)
						{
							flag = true;
							break;
						}
					}
					bool flag2 = false;
					float num = Vector3.Angle(actor.entity.transform.forward, baseAbilityActor.entity.transform.position - actor.entity.transform.position);
					bool flag3 = num < _defendAngle;
					if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, actor, null))
					{
						flag2 = false;
					}
					else if (flag3)
					{
						flag2 = true;
					}
					bool flag4 = false;
					if (evt.attackData.hitType == AttackResult.ActorHitType.Melee && config.DefendMelee)
					{
						flag4 = true;
					}
					if (evt.attackData.hitType == AttackResult.ActorHitType.Ranged && config.DefendRange)
					{
						flag4 = true;
					}
					if (flag && flag2 && flag4)
					{
						if (!_isInDefendState)
						{
							_isInDefendState = true;
							_inDefendStateTimer.Reset(instancedAbility.Evaluate(config.InDefendMaxTime) > 0f);
						}
						if (baseMonoMonster != null)
						{
							baseMonoMonster.hasArmor = true;
						}
						actor.entity.SetTrigger(config.DefendTriggerID);
						evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
						evt.attackData.hitEffectPattern = AttackResult.HitEffectPattern.OnlyAttack;
						float num2 = Vector3.Distance(baseAbilityActor.entity.transform.position, actor.entity.transform.position);
						bool flag5 = num2 < _defendActionRange;
						_allowDefendActions = UnityEngine.Random.value < _defendActionChance && _defendCDAllowed;
						if (flag3)
						{
							evt.attackData.damage = 0f;
							if (evt.attackData.GetElementalDamage() > 0f)
							{
								evt.attackData.hitLevel = AttackResult.ActorHitLevel.Normal;
							}
							else
							{
								evt.attackData.hitLevel = AttackResult.ActorHitLevel.Mute;
							}
							if (_allowDefendActions && flag5)
							{
								if (config.DefendSuccessEffect != null)
								{
									FireMixinEffect(config.DefendSuccessEffect, entity);
								}
							}
							else if (config.DefendEffect != null)
							{
								FireMixinEffect(config.DefendEffect, entity);
							}
						}
						if (_allowDefendActions && flag5)
						{
							actor.abilityPlugin.HandleActionTargetDispatch(config.DefendSuccessActions, instancedAbility, instancedModifier, actor, evt);
							_defendCDAllowed = false;
							_defendActionTimer.Reset(true);
							_isInDefendState = false;
							_inDefendStateTimer.Reset(false);
							StartDefend();
						}
					}
				}
			}
			return true;
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			for (int i = 0; i < config.ResetCDSkillIDs.Length; i++)
			{
				if (to == config.ResetCDSkillIDs[i])
				{
					_defendCDAllowed = false;
					_defendActionTimer.Reset(true);
					_isInDefendState = false;
					_inDefendStateTimer.Reset(false);
					StartDefend();
				}
			}
		}
	}
}
