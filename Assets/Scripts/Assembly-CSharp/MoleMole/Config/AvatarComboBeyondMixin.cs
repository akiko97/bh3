namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarComboBeyondMixin : ConfigAbilityMixin, IHashable
	{
		public string[] ModifierNames;

		public DynamicInt[] ComboSteps;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (ModifierNames != null)
			{
				string[] modifierNames = ModifierNames;
				foreach (string value in modifierNames)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (ComboSteps != null)
			{
				DynamicInt[] comboSteps = ComboSteps;
				foreach (DynamicInt dynamicInt in comboSteps)
				{
					HashUtils.ContentHashOnto(dynamicInt.isDynamic, ref lastHash);
					HashUtils.ContentHashOnto(dynamicInt.fixedValue, ref lastHash);
					HashUtils.ContentHashOnto(dynamicInt.dynamicKey, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarComboBeyondMixin(instancedAbility, instancedModifier, this);
		}
	}
}
