namespace MoleMole.Config
{
	[CheckForHashable]
	public abstract class ConfigAbilityPredicate
	{
		public static ConfigAbilityPredicate[] EMPTY = new ConfigAbilityPredicate[0];

		public abstract bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt);
	}
}
