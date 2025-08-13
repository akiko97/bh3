namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByControlDataHasSteer : ConfigAbilityPredicate, IHashable
	{
		public bool HasSteer = true;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(HasSteer, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByControlDataHasSteerHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
