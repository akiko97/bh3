namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByAttackCategoryTag : ConfigAbilityPredicate, IHashable
	{
		public AttackResult.AttackCategoryTag CategoryTag;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)CategoryTag, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByAttackCategoryTagHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
