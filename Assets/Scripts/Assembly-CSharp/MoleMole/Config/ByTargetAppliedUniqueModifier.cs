namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByTargetAppliedUniqueModifier : ConfigAbilityPredicate, IHashable
	{
		public string UniquModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(UniquModifierName, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByTargetAppliedUniqueModifierHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
