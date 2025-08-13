using MoleMole.Config;

namespace MoleMole
{
	public class AbilityDelayMixin : BaseAbilityMixin
	{
		private DelayMixin config;

		private EntityTimer _waitTimer;

		public AbilityDelayMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DelayMixin)config;
			_waitTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Delay));
		}

		public override void OnAdded()
		{
			_waitTimer.Reset(true);
			_waitTimer.SetActive(true);
		}

		public override void Core()
		{
			_waitTimer.Core(1f);
			if (_waitTimer.isTimeUp)
			{
				OnTimeUp();
			}
		}

		public void OnTimeUp()
		{
			actor.abilityPlugin.HandleActionTargetDispatch(config.OnTimeUp, instancedAbility, instancedModifier, actor, null);
			_waitTimer.Reset(false);
		}
	}
}
