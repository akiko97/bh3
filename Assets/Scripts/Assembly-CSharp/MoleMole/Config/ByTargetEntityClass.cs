namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByTargetEntityClass : ConfigAbilityPredicate, IHashable
	{
		public EntityClass TargetClass;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)TargetClass, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByTargetClassHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
