namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class SPTHresholdMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat Threshold = DynamicFloat.ZERO;

		public MixinPredicate Predicate;

		public string ModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Threshold != null)
			{
				HashUtils.ContentHashOnto(Threshold.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Threshold.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Threshold.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto((int)Predicate, ref lastHash);
			HashUtils.ContentHashOnto(ModifierName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilitySPThresholdMixin(instancedAbility, instancedModifier, this);
		}
	}
}
