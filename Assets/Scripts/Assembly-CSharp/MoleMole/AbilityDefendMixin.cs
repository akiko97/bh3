using MoleMole.Config;

namespace MoleMole
{
	public class AbilityDefendMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			Defending = 1
		}

		private DefendMixin config;

		private float _defendWindow;

		private float _defendPerfectStartTime;

		private float _defendPerfectEndTime;

		private EntityTimer _defendFailTimer;

		private EntityTimer _defendPerfectFailTimer;

		private State _state;

		private bool _isDefendingPerfect;

		private ActorModifier _defendDurationModifier;

		private ActorModifier _defendPerfectDurationModifier;

		public AbilityDefendMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DefendMixin)config;
			_defendWindow = instancedAbility.Evaluate(this.config.DefendWindow);
			_defendPerfectStartTime = instancedAbility.Evaluate(this.config.DefendPerfectStartTime);
			_defendPerfectEndTime = instancedAbility.Evaluate(this.config.DefendPerfectEndTime);
			_defendFailTimer = new EntityTimer(_defendWindow);
			_defendPerfectFailTimer = new EntityTimer(_defendPerfectStartTime);
			_isDefendingPerfect = false;
			_defendFailTimer.SetActive(false);
			_defendPerfectFailTimer.SetActive(false);
			_state = State.Idle;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			actor.abilityPlugin.HandleActionTargetDispatch(config.DefendStartActions, instancedAbility, instancedModifier, null, null);
			Singleton<EventManager>.Instance.FireEvent(new EvtDefendStart(actor.runtimeID));
			_defendFailTimer.Reset(true);
			if (!string.IsNullOrEmpty(config.DefendDurationModifierName))
			{
				_defendDurationModifier = actor.abilityPlugin.ApplyModifier(instancedAbility, config.DefendDurationModifierName);
			}
			if (_state == State.Idle)
			{
				actor.AddAbilityState(AbilityState.BlockAnimEventAttack, true);
			}
			_state = State.Defending;
		}

		public override void Core()
		{
			_defendFailTimer.Core(1f);
			if (_isDefendingPerfect)
			{
				_defendPerfectFailTimer.Core(1f);
			}
			if (_defendFailTimer.timer > _defendPerfectStartTime && !_isDefendingPerfect)
			{
				_isDefendingPerfect = true;
				_defendPerfectFailTimer.Reset(true);
				if (config.DefendPerfectDurationModifierName != null)
				{
					_defendPerfectDurationModifier = actor.abilityPlugin.ApplyModifier(instancedAbility, config.DefendPerfectDurationModifierName);
				}
			}
			if (_defendPerfectFailTimer.timer > _defendPerfectEndTime && _isDefendingPerfect)
			{
				_isDefendingPerfect = false;
				_defendPerfectFailTimer.Reset(false);
				if (_defendPerfectDurationModifier != null)
				{
					actor.abilityPlugin.TryRemoveModifier(_defendPerfectDurationModifier);
					_defendPerfectDurationModifier = null;
				}
				if (_state == State.Defending)
				{
					actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
					_state = State.Idle;
				}
			}
			if (_defendFailTimer.isTimeUp)
			{
				if (_defendDurationModifier != null)
				{
					actor.abilityPlugin.TryRemoveModifier(_defendDurationModifier);
					_defendDurationModifier = null;
				}
				_isDefendingPerfect = false;
				_defendFailTimer.Reset(false);
				_defendPerfectFailTimer.Reset(false);
				if (_state == State.Defending)
				{
					actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
					_state = State.Idle;
				}
			}
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			if (_defendFailTimer.isActive && !_defendFailTimer.isTimeUp)
			{
				if (config.DefendReplaceAttackEffect != null)
				{
					evt.attackData.attackEffectPattern = config.DefendReplaceAttackEffect;
				}
				actor.abilityPlugin.HandleActionTargetDispatch(config.DefendSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
				if (_isDefendingPerfect)
				{
					actor.abilityPlugin.HandleActionTargetDispatch(config.DefendSuccessPerfectActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
				}
			}
			return true;
		}
	}
}
