using MoleMole.Config;

namespace MoleMole
{
	public class AbilityModifyAnimEventAttackMixin : BaseAbilityMixin
	{
		private ModifyAnimEventAttackMixin config;

		public AbilityModifyAnimEventAttackMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ModifyAnimEventAttackMixin)config;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtHittingOther)
			{
				return OnHittingOtherResolve((EvtHittingOther)evt);
			}
			return false;
		}

		private bool OnHittingOtherResolve(EvtHittingOther evt)
		{
			bool flag = false;
			flag = config.ModifyAllAnimEvents || Miscs.ArrayContains(config.AnimEventIDs, evt.animEventID);
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt))
			{
				return false;
			}
			if (flag)
			{
				evt.attackData.attackerCritChance += instancedAbility.Evaluate(config.CritChanceDelta);
				evt.attackData.attackerCritDamageRatio += instancedAbility.Evaluate(config.CritDamageRatioDelta);
				evt.attackData.attackerAniDamageRatio += instancedAbility.Evaluate(config.AnimDamageRatioDelta);
				evt.attackData.attackerAttackPercentage += instancedAbility.Evaluate(config.DamagePercentageDelta);
				evt.attackData.attackerShieldDamageRatio += instancedAbility.Evaluate(config.ShieldDamageDelta);
				evt.attackData.attackerAttackValue += instancedAbility.Evaluate(config.AttackValueDelta);
				actor.abilityPlugin.HandleActionTargetDispatch(config.Actions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt);
			}
			return true;
		}
	}
}
