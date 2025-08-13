namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class StealHPMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat HPStealRatio = DynamicFloat.ZERO;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (HPStealRatio != null)
			{
				HashUtils.ContentHashOnto(HPStealRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(HPStealRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(HPStealRatio.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityStealMixin(instancedAbility, instancedModifier, this);
		}
	}
}
