namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByHitDirection : ConfigAbilityPredicate, IHashable
	{
		public float Angle;

		public bool ReverseAngle;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(Angle, ref lastHash);
			HashUtils.ContentHashOnto(ReverseAngle, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByHitDirectionHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
