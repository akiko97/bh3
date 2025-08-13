namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarModifyPropertyByCombo : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat Initial;

		public DynamicFloat MinValue = DynamicFloat.ZERO;

		public DynamicFloat MaxValue;

		public DynamicInt MaxValueCombo;

		public DynamicFloat PerComboDelta;

		public string Property;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Initial != null)
			{
				HashUtils.ContentHashOnto(Initial.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Initial.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Initial.dynamicKey, ref lastHash);
			}
			if (MinValue != null)
			{
				HashUtils.ContentHashOnto(MinValue.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(MinValue.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(MinValue.dynamicKey, ref lastHash);
			}
			if (MaxValue != null)
			{
				HashUtils.ContentHashOnto(MaxValue.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(MaxValue.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(MaxValue.dynamicKey, ref lastHash);
			}
			if (MaxValueCombo != null)
			{
				HashUtils.ContentHashOnto(MaxValueCombo.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(MaxValueCombo.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(MaxValueCombo.dynamicKey, ref lastHash);
			}
			if (PerComboDelta != null)
			{
				HashUtils.ContentHashOnto(PerComboDelta.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PerComboDelta.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PerComboDelta.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(Property, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarModifyPropertyByComboMixin(instancedAbility, instancedModifier, this);
		}
	}
}
