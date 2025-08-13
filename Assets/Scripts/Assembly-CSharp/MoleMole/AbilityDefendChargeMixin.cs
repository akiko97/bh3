using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityDefendChargeMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			Before = 1,
			InLoop = 2,
			After = 3
		}

		private enum PerfectDefendState
		{
			Idle = 0,
			WaitToStart = 1,
			PerfectDefend = 2
		}

		public DefendChargeMixin config;

		private State _state;

		private PerfectDefendState _perfectDefendState;

		private float _perfectDefendDuration;

		private EntityTimer _perfectDefendTimer;

		private ActorModifier _defendAttachedModifier;

		private ActorModifier _defendPerfectAttachedModifier;

		private bool _isInDefend;

		public AbilityDefendChargeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DefendChargeMixin)config;
			_perfectDefendTimer = new EntityTimer(0f, entity);
			_perfectDefendTimer.SetActive(false);
			_perfectDefendDuration = instancedAbility.Evaluate(this.config.DefendPerfectEndTime) - instancedAbility.Evaluate(this.config.DefendPerfectStartTime);
		}

		public override void OnAdded()
		{
			_state = State.Idle;
			_perfectDefendState = PerfectDefendState.Idle;
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
		}

		public override void Core()
		{
			if (_perfectDefendState == PerfectDefendState.WaitToStart)
			{
				_perfectDefendTimer.Core(1f);
				if (_perfectDefendTimer.isTimeUp)
				{
					_perfectDefendTimer.timespan = _perfectDefendDuration;
					_perfectDefendTimer.Reset(true);
					_perfectDefendState = PerfectDefendState.PerfectDefend;
					if (config.DefendPerfectDurationModifierName != null)
					{
						_defendPerfectAttachedModifier = actor.abilityPlugin.ApplyModifier(instancedAbility, config.DefendPerfectDurationModifierName);
					}
				}
			}
			else if (_perfectDefendState == PerfectDefendState.PerfectDefend)
			{
				_perfectDefendTimer.Core(1f);
				if (_perfectDefendTimer.isTimeUp)
				{
					_perfectDefendTimer.Reset(false);
					_perfectDefendState = PerfectDefendState.Idle;
					if (_defendPerfectAttachedModifier != null)
					{
						actor.abilityPlugin.TryRemoveModifier(_defendPerfectAttachedModifier);
						_defendPerfectAttachedModifier = null;
					}
				}
			}
			if (_state == State.Before)
			{
				if (!_isInDefend && entity.GetCurrentNormalizedTime() > config.DefendBSNormalizedStartTime)
				{
					SetInDefend(true);
				}
			}
			else if (_state == State.After && _isInDefend && entity.GetCurrentNormalizedTime() > config.DefendASNormalizedEndTime)
			{
				SetInDefend(false);
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (_state == State.Idle)
			{
				if (to == config.DefendBSSkillID)
				{
					_state = State.Before;
				}
			}
			else if (_state == State.Before)
			{
				if (to == config.DefendLoopSkillID)
				{
					_state = State.InLoop;
					return;
				}
				if (_isInDefend)
				{
					SetInDefend(false);
				}
				_state = State.Idle;
			}
			else if (_state == State.InLoop)
			{
				if (to == config.DefendBSSkillID)
				{
					_state = State.After;
					return;
				}
				SetInDefend(false);
				_state = State.Idle;
			}
			else
			{
				if (_state != State.After)
				{
					return;
				}
				if (to == config.DefendBSSkillID)
				{
					if (_isInDefend)
					{
						SetInDefend(false);
					}
					_state = State.Before;
				}
				else
				{
					if (_isInDefend)
					{
						SetInDefend(false);
					}
					_state = State.Idle;
				}
			}
		}

		private void SetInDefend(bool inDefend)
		{
			if (inDefend)
			{
				_perfectDefendState = PerfectDefendState.WaitToStart;
				_perfectDefendTimer.timespan = instancedAbility.Evaluate(config.DefendPerfectStartTime);
				_perfectDefendTimer.Reset(true);
				if (config.DefendDurationModifierName != null)
				{
					_defendAttachedModifier = actor.abilityPlugin.ApplyModifier(instancedAbility, config.DefendDurationModifierName);
				}
			}
			else if (_defendAttachedModifier != null)
			{
				actor.abilityPlugin.TryRemoveModifier(_defendAttachedModifier);
				_defendAttachedModifier = null;
			}
			_isInDefend = inDefend;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			if (_isInDefend)
			{
				if (config.DefendReplaceAttackEffect != null)
				{
					evt.attackData.attackEffectPattern = config.DefendReplaceAttackEffect;
				}
				actor.abilityPlugin.HandleActionTargetDispatch(config.DefendSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
				if (_perfectDefendState == PerfectDefendState.PerfectDefend)
				{
					actor.abilityPlugin.HandleActionTargetDispatch(config.DefendSuccessPerfectActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
				}
				return true;
			}
			return false;
		}
	}
}
