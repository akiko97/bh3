namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByIsPlayerAvatar : ConfigAbilityPredicate, IHashable
	{
		public void ObjectContentHashOnto(ref int lastHash)
		{
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByIsPlayerAvatarHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
