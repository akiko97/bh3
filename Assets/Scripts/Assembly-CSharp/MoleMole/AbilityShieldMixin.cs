using FullInspector;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityShieldMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			ShieldBroken = 1,
			Resuming = 2
		}

		private ShieldMixin config;

		public float maxShield;

		public float shield;

		private EntityTimer _betweenAttackResumeTimer;

		private EntityTimer _forceResumeTimer;

		private EntityTimer _minForceResumeTimer;

		private EntityTimer _shieldResumeTimer;

		private float _shieldResumeRatio = 1f;

		private float _shieldResumeStart;

		private float _shieldResumeTarget;

		private float _forceResumeDamage;

		private bool _showShieldBar;

		private DisplayValue<float> _shieldDisplay;

		private float _displayFloor;

		[ShowInInspector]
		private State _state;

		private MonsterActor _monsterActor;

		private BaseMonoMonster _monsterEntity;

		private AbilityState _controlledAbilityState;

		public AbilityShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ShieldMixin)config;
			if (entity is BaseMonoMonster)
			{
				_monsterEntity = (BaseMonoMonster)entity;
				_monsterActor = (MonsterActor)actor;
			}
			_showShieldBar = instancedAbility.Evaluate(this.config.ShowShieldBar) != 0;
			if (this.config.UseLevelTimeScale)
			{
				_betweenAttackResumeTimer = new EntityTimer(this.config.BetweenAttackResumeCD);
				_forceResumeTimer = new EntityTimer(this.config.ForceResumeCD);
				_minForceResumeTimer = new EntityTimer(this.config.MinForceResumeCD);
				_shieldResumeTimer = new EntityTimer(this.config.ShieldResumeTimeSpan);
			}
			else
			{
				_betweenAttackResumeTimer = new EntityTimer(this.config.BetweenAttackResumeCD, entity);
				_forceResumeTimer = new EntityTimer(this.config.ForceResumeCD, entity);
				_minForceResumeTimer = new EntityTimer(this.config.MinForceResumeCD, entity);
				_shieldResumeTimer = new EntityTimer(this.config.ShieldResumeTimeSpan, entity);
			}
		}

		private void OnShieldChanged(float from, float to)
		{
			if (_shieldDisplay != null)
			{
				_shieldDisplay.Pub(Mathf.Lerp(_displayFloor, config.ShieldDisplayRatioCeiling, to / maxShield));
			}
		}

		public override void OnAdded()
		{
			_state = State.Idle;
			shield = (maxShield = (float)actor.baseMaxHP * instancedAbility.Evaluate(config.ShieldHPRatio));
			_betweenAttackResumeTimer.SetActive(true);
			_forceResumeTimer.SetActive(false);
			_minForceResumeTimer.SetActive(false);
			_shieldResumeTimer.SetActive(false);
			actor.abilityPlugin.ApplyModifier(instancedAbility, config.ShieldOnModifierName);
			if (_showShieldBar)
			{
				_shieldDisplay = actor.abilityPlugin.CreateOrGetDisplayFloat("Shield", 0f, 1f, config.ShieldDisplayRatioCeiling);
				if (_shieldDisplay.value < config.ShieldDisplayRatioCeiling)
				{
					_state = State.Resuming;
					_displayFloor = _shieldDisplay.value;
					_shieldResumeTimer.timespan = 0.3f;
					_shieldResumeTimer.Reset(true);
					_shieldResumeStart = 0f;
					_shieldResumeTarget = maxShield;
				}
				else
				{
					_displayFloor = config.ShieldDisplayRatioFloor;
				}
			}
			if (config.ControlledAbilityStates != null)
			{
				for (int i = 0; i < config.ControlledAbilityStates.Length; i++)
				{
					_controlledAbilityState |= config.ControlledAbilityStates[i];
				}
			}
		}

		public override void OnRemoved()
		{
			actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ShieldOnModifierName);
			actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ShieldOffModifierName);
		}

		public override void Core()
		{
			_betweenAttackResumeTimer.Core(1f);
			_forceResumeTimer.Core(_shieldResumeRatio);
			_minForceResumeTimer.Core(1f);
			_shieldResumeTimer.Core(1f);
			if (_state == State.Idle)
			{
				if (shield <= 0f)
				{
					_state = State.ShieldBroken;
					_forceResumeTimer.SetActive(true);
					_minForceResumeTimer.SetActive(true);
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ShieldOnModifierName);
					actor.abilityPlugin.ApplyModifier(instancedAbility, config.ShieldOffModifierName);
				}
				else if (_betweenAttackResumeTimer.isTimeUp)
				{
					_state = State.Resuming;
					_displayFloor = config.ShieldDisplayRatioFloor;
					_shieldResumeStart = shield;
					_shieldResumeTarget = Mathf.Min(shield + maxShield * config.BetweenAttackResumeCD, maxShield);
					_shieldResumeTimer.Reset(true);
					_betweenAttackResumeTimer.Reset(true);
				}
			}
			else if (_state == State.ShieldBroken)
			{
				_shieldResumeRatio = GetShieldResumeRatio();
				if (_forceResumeTimer.isTimeUp && _minForceResumeTimer.isTimeUp)
				{
					_state = State.Resuming;
					_displayFloor = config.ShieldDisplayRatioFloor;
					_shieldResumeStart = shield;
					_shieldResumeTarget = Mathf.Min(shield + maxShield * config.ForceResumeRatio, maxShield);
					_shieldResumeTimer.Reset(true);
					_forceResumeTimer.Reset(false);
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ShieldOffModifierName);
					actor.abilityPlugin.ApplyModifier(instancedAbility, config.ShieldOnModifierName);
				}
			}
			else
			{
				if (_state != State.Resuming)
				{
					return;
				}
				float newValue = Mathf.Lerp(_shieldResumeStart, _shieldResumeTarget, _shieldResumeTimer.GetTimingRatio());
				DelegateUtils.UpdateField(ref shield, newValue, OnShieldChanged);
				if (_shieldResumeTimer.isTimeUp)
				{
					_state = State.Idle;
					_shieldResumeTimer.timespan = config.ShieldResumeTimeSpan;
					_displayFloor = config.ShieldDisplayRatioFloor;
					if (_monsterActor.isElite)
					{
						_monsterEntity.SwitchEliteShader(true);
					}
				}
			}
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return OnKilled((EvtKilled)evt);
			}
			return false;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnPostBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnKilled(EvtKilled evt)
		{
			DelegateUtils.UpdateField(ref shield, 0f, OnShieldChanged);
			return true;
		}

		private bool OnPostBeingHit(EvtBeingHit evt)
		{
			if (!evt.attackData.isAnimEventAttack)
			{
				return false;
			}
			if (evt.attackData.rejected)
			{
				return false;
			}
			if (actor.abilityState.ContainsState(AbilityState.Invincible) || actor.abilityState.ContainsState(AbilityState.Undamagable))
			{
				return false;
			}
			_betweenAttackResumeTimer.Reset();
			float num = evt.attackData.attackerAniDamageRatio;
			if (num < 0f)
			{
				num = 0f;
			}
			float num2 = Mathf.Pow(evt.attackData.damage, config.DamagePower);
			num2 *= Mathf.Pow(evt.attackData.attackerShieldDamageRatio, config.ShieldDamagePower);
			num2 *= Mathf.Pow(num, config.AniDamagePower);
			num2 += evt.attackData.attackerShieldDamageDelta;
			float num3 = shield - num2;
			if (num3 <= 0f)
			{
				num3 = 0f;
			}
			if (num3 > 0f && config.ShieldSuccessEffect != null)
			{
				evt.attackData.beHitEffectPattern = config.ShieldSuccessEffect;
			}
			if (shield > 0f && num3 == 0f && _state != State.Resuming)
			{
				if (config.ShieldBrokenTimeSlow > 0f)
				{
					Singleton<LevelManager>.Instance.levelActor.TimeSlow(config.ShieldBrokenTimeSlow);
				}
				FireMixinEffect(config.ShieldBrokenEffect, entity);
				actor.abilityPlugin.HandleActionTargetDispatch(config.ShiedlBrokenActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
				evt.attackData.frameHalt = 0;
				evt.attackData.attackerAniDamageRatio = 10f;
				Singleton<EventManager>.Instance.FireEvent(new EvtShieldBroken(actor.runtimeID));
				evt.attackData.AddHitFlag(AttackResult.ActorHitFlag.ShieldBroken);
				_forceResumeDamage = 0f;
				if (_monsterActor.isElite)
				{
					_monsterEntity.SwitchEliteShader(false);
				}
			}
			if (_state != State.Resuming)
			{
				DelegateUtils.UpdateField(ref shield, num3, OnShieldChanged);
			}
			if (shield > 0f && evt.attackData.attackeeAniDefenceRatio > evt.attackData.attackerAniDamageRatio)
			{
				evt.attackData.frameHalt += config.ShieldSuccessAddFrameHalt;
				if (config.MuteHitEffect)
				{
					evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Mute;
				}
				else
				{
					evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
				}
				actor.abilityPlugin.HandleActionTargetDispatch(config.ShieldSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
			}
			if (_state == State.ShieldBroken && shield == 0f && instancedAbility.Evaluate(config.ForceResumeByDamageHPRatio) > 0f)
			{
				_forceResumeDamage += evt.attackData.GetTotalDamage() * _shieldResumeRatio;
				if (_forceResumeDamage / (float)actor.baseMaxHP >= instancedAbility.Evaluate(config.ForceResumeByDamageHPRatio))
				{
					_forceResumeTimer.isTimeUp = true;
				}
			}
			return true;
		}

		private float GetShieldResumeRatio()
		{
			float result = 1f;
			if (_monsterEntity != null && _monsterEntity.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
			{
				result = config.ThrowForceResumeTimeRatio;
			}
			if ((_controlledAbilityState & actor.abilityState) != AbilityState.None)
			{
				result = config.ControlledForceResumeTimeRatio;
			}
			return result;
		}
	}
}
