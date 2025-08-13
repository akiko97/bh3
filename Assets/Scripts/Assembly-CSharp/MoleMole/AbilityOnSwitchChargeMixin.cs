using MoleMole.Config;

namespace MoleMole
{
	public class AbilityOnSwitchChargeMixin : BaseAbilityMixin
	{
		private OnSwitchChargeMixin config;

		public AbilityOnSwitchChargeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (OnSwitchChargeMixin)config;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtChargeRelease)
			{
				return OnChargeRelease((EvtChargeRelease)evt);
			}
			return false;
		}

		private bool OnChargeRelease(EvtChargeRelease evt)
		{
			if (evt.isSwitchRelease && Miscs.ArrayContains(config.AfterSkillIDs, evt.releaseSkillID))
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.Actions, instancedAbility, instancedModifier, null, evt);
				return true;
			}
			return false;
		}
	}
}
