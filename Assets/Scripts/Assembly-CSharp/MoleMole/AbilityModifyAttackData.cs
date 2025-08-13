using MoleMole.Config;

namespace MoleMole
{
	public class AbilityModifyAttackData : BaseAbilityMixin
	{
		private ModifyAttackData config;

		public AbilityModifyAttackData(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ModifyAttackData)config;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtHittingOther)
			{
				return OnPostHittingOther((EvtHittingOther)evt);
			}
			return false;
		}

		private bool OnPostHittingOther(EvtHittingOther evt)
		{
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt))
			{
				return false;
			}
			if (config.NoTriggerEvadeAndDefend)
			{
				evt.attackData.noTriggerEvadeAndDefend = config.NoTriggerEvadeAndDefend;
			}
			return false;
		}
	}
}
