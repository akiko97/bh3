using MoleMole.Config;

namespace MoleMole
{
	public class AbilityModifyDamageByAttackeeMixin : BaseAbilityMixin
	{
		private ModifyDamageByAttackeeMixin config;

		public AbilityModifyDamageByAttackeeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ModifyDamageByAttackeeMixin)config;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		protected virtual bool OnBeingHit(EvtBeingHit evt)
		{
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt))
			{
				return false;
			}
			actor.abilityPlugin.HandleActionTargetDispatch(config.Actions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
			evt.attackData.attackeeAddedDamageTakeRatio += instancedAbility.Evaluate(config.AddedDamageTakeRatio);
			return true;
		}
	}
}
