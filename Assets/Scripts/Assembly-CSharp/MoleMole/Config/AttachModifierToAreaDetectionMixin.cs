namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachModifierToAreaDetectionMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat Radius = DynamicFloat.ZERO;

		public float Delay = 1f;

		public bool IsInvert;

		public string[] ModifierNames;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Radius != null)
			{
				HashUtils.ContentHashOnto(Radius.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Radius.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Radius.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(Delay, ref lastHash);
			HashUtils.ContentHashOnto(IsInvert, ref lastHash);
			if (ModifierNames != null)
			{
				string[] modifierNames = ModifierNames;
				foreach (string value in modifierNames)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAttachModifierToAreaDetectionMixin(instancedAbility, instancedModifier, this);
		}
	}
}
