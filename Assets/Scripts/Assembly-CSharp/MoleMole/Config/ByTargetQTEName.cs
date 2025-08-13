namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByTargetQTEName : ConfigAbilityPredicate, IHashable
	{
		public string targetQTEName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(targetQTEName, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByTargetQTENameHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
