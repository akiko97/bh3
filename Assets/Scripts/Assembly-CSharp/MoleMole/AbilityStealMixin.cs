using MoleMole.Config;

namespace MoleMole
{
	public class AbilityStealMixin : BaseAbilityMixin
	{
		private StealHPMixin config;

		public AbilityStealMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (StealHPMixin)config;
		}

		public override void OnAdded()
		{
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtHittingOther)
			{
				return OnHittingOther((EvtHittingOther)evt);
			}
			return false;
		}

		private bool OnHittingOther(EvtHittingOther evt)
		{
			float amount = evt.attackData.attackerAttackValue * instancedAbility.Evaluate(config.HPStealRatio);
			actor.HealHP(amount);
			return true;
		}
	}
}
