namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByAny : ConfigAbilityPredicate, IHashable
	{
		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Predicates == null)
			{
				return;
			}
			ConfigAbilityPredicate[] predicates = Predicates;
			foreach (ConfigAbilityPredicate configAbilityPredicate in predicates)
			{
				if (configAbilityPredicate is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityPredicate, ref lastHash);
				}
			}
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByAnyHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
