namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class PullTargetMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat PullRadius;

		public DynamicFloat PullVelocity;

		public DynamicFloat StopDistance = DynamicFloat.ONE;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (PullRadius != null)
			{
				HashUtils.ContentHashOnto(PullRadius.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PullRadius.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PullRadius.dynamicKey, ref lastHash);
			}
			if (PullVelocity != null)
			{
				HashUtils.ContentHashOnto(PullVelocity.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PullVelocity.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PullVelocity.dynamicKey, ref lastHash);
			}
			if (StopDistance != null)
			{
				HashUtils.ContentHashOnto(StopDistance.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(StopDistance.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(StopDistance.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityPullTargetMixin(instancedAbility, instancedModifier, this);
		}
	}
}
