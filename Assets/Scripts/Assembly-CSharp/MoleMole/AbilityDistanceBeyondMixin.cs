using MoleMole.Config;

namespace MoleMole
{
	public class AbilityDistanceBeyondMixin : BaseAbilityMixin
	{
		private DistanceBeyondMixin config;

		public AbilityDistanceBeyondMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DistanceBeyondMixin)config;
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
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID);
			if (baseAbilityActor == null || baseAbilityActor.entity == null)
			{
				return false;
			}
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt))
			{
				return false;
			}
			bool flag = (actor.entity.transform.position - baseAbilityActor.entity.transform.position).magnitude > instancedAbility.Evaluate(config.HitDistanceBeyond);
			if (config.Reverse)
			{
				flag = !flag;
			}
			if (flag)
			{
				evt.attackData.attackerCritChance += instancedAbility.Evaluate(config.CriticalChanceRatioUp);
				evt.attackData.attackerCritDamageRatio += instancedAbility.Evaluate(config.CriticalDamageRatioUp);
				evt.attackData.attackerAttackPercentage += instancedAbility.Evaluate(config.DamagePercentageUp);
				evt.attackData.attackerAniDamageRatio += instancedAbility.Evaluate(config.AniDamageRatioUp);
				evt.attackData.attackerAttackValue *= 1f + instancedAbility.Evaluate(config.AttackRatio);
				actor.abilityPlugin.HandleActionTargetDispatch(config.Actions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt);
			}
			return true;
		}
	}
}
