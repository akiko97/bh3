using MoleMole.Config;

namespace MoleMole
{
	public class AbilityCriticalAttackMixin : BaseAbilityMixin
	{
		private CriticalAttackMixin config;

		public AbilityCriticalAttackMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (CriticalAttackMixin)config;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtAttackLanded)
			{
				return OnAttackLanded((EvtAttackLanded)evt);
			}
			return false;
		}

		private bool OnAttackLanded(EvtAttackLanded evt)
		{
			if (evt.attackResult.hitLevel == AttackResult.ActorHitLevel.Critical)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.OnCriticalAttackActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID), evt);
			}
			return true;
		}
	}
}
