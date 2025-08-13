using MoleMole.Config;

namespace MoleMole
{
	public class AbilityModifyDamageTakenMixin : BaseAbilityMixin
	{
		private ModifyDamageTakenMixin config;

		public AbilityModifyDamageTakenMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ModifyDamageTakenMixin)config;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnPostBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		protected virtual bool OnPostBeingHit(EvtBeingHit evt)
		{
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt))
			{
				return false;
			}
			actor.abilityPlugin.HandleActionTargetDispatch(config.Actions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
			ModifyDamage(evt, 1f);
			evt.attackData.attackeeAniDefenceRatio += instancedAbility.Evaluate(config.AddAttackeeAniDefenceRatio);
			return true;
		}

		protected virtual void ModifyDamage(EvtBeingHit evt, float multiple = 1f)
		{
			evt.attackData.damage = evt.attackData.damage * (1f + instancedAbility.Evaluate(config.DamageTakenRatio) * multiple) + instancedAbility.Evaluate(config.DamageTakenDelta) * multiple;
			if (evt.attackData.damage < 0f)
			{
				evt.attackData.damage = 0f;
			}
		}
	}
}
