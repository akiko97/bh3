namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByTargetIsSelf : ConfigAbilityPredicate, IHashable
	{
		public bool IsSelf = true;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(IsSelf, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByTargetIsSelfHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
