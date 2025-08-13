namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByTargetContainAnimEventPredicate : ConfigAbilityPredicate, IHashable
	{
		public string AnimEventPredicate;

		public bool ForceByCaster;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(AnimEventPredicate, ref lastHash);
			HashUtils.ContentHashOnto(ForceByCaster, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByTargetContainAnimEventPredicateHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
