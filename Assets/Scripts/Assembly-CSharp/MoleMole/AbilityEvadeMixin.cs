using MoleMole.Config;

namespace MoleMole
{
	public class AbilityEvadeMixin : BaseAbilityMixin
	{
		protected enum State
		{
			Idle = 0,
			Evading = 1,
			EvadeSuccessed = 2
		}

		protected EvadeMixin config;

		protected EntityTimer _evadeTimer;

		protected EntityTimer _extendedBlockAttackTimer;

		protected State _state;

		protected EvadeEntityDummy _dummyActor;

		public AbilityEvadeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (EvadeMixin)config;
			_evadeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.EvadeWindow));
			_evadeTimer.SetActive(false);
			_extendedBlockAttackTimer = new EntityTimer(instancedAbility.Evaluate(this.config.EvadeSuccessExtendedInvincibleWindow));
			_extendedBlockAttackTimer.SetActive(false);
			_state = State.Idle;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtEvadeSuccess)
			{
				return OnEvadeSuccess((EvtEvadeSuccess)evt);
			}
			return false;
		}

		public override void OnRemoved()
		{
			if (_state == State.Evading || _state == State.EvadeSuccessed)
			{
				entity.SetCountedIsGhost(false);
			}
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (_dummyActor != null && _dummyActor.IsEntityExists())
			{
				_dummyActor.Kill();
			}
			uint runtimeID = Singleton<DynamicObjectManager>.Instance.CreateEvadeDummy(actor.runtimeID, config.EvadeDummyName, actor.entity.XZPosition, actor.entity.transform.forward);
			_dummyActor = Singleton<EventManager>.Instance.GetActor<EvadeEntityDummy>(runtimeID);
			actor.abilityPlugin.HandleActionTargetDispatch(config.EvadeStartActions, instancedAbility, instancedModifier, null, null);
			Singleton<EventManager>.Instance.FireEvent(new EvtEvadeStart(actor.runtimeID));
			if (_state == State.Idle)
			{
				actor.AddAbilityState(AbilityState.BlockAnimEventAttack, true);
				entity.SetCountedIsGhost(true);
			}
			_evadeTimer.Reset(true);
			_extendedBlockAttackTimer.Reset(true);
			_state = State.Evading;
		}

		public override void Core()
		{
			if (_state == State.Evading)
			{
				_evadeTimer.Core(1f);
				if (_evadeTimer.isTimeUp)
				{
					EvadeFail();
				}
			}
			else if (_state == State.EvadeSuccessed)
			{
				_extendedBlockAttackTimer.Core(1f);
				if (_extendedBlockAttackTimer.isTimeUp)
				{
					actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
					entity.SetCountedIsGhost(false);
					_state = State.Idle;
				}
			}
		}

		protected virtual bool OnEvadeSuccess(EvtEvadeSuccess evt)
		{
			if (_state != State.Evading)
			{
				return false;
			}
			_state = State.EvadeSuccessed;
			_extendedBlockAttackTimer.Reset(true);
			instancedAbility.CurrentTriggerEvent = evt;
			actor.abilityPlugin.HandleActionTargetDispatch(config.EvadeSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackerID), evt);
			return true;
		}

		protected virtual void EvadeFail()
		{
			actor.abilityPlugin.HandleActionTargetDispatch(config.EvadeFailActions, instancedAbility, instancedModifier, null, null);
			_evadeTimer.Reset(false);
			_dummyActor.Kill();
			actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
			entity.SetCountedIsGhost(false);
			_state = State.Idle;
		}
	}
}
