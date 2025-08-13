namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByIsLocalAvatar : ConfigAbilityPredicate, IHashable
	{
		public void ObjectContentHashOnto(ref int lastHash)
		{
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByIsLocalAvatarHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
