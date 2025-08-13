namespace MoleMole.Config
{
	public class ByTargetDistance : ConfigAbilityPredicate
	{
		public DynamicFloat Distance;

		public MixinPredicate Logic;

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByTargetDistanceHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
