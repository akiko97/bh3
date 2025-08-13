namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterBlurDistanceBeyondMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat Distance = DynamicFloat.ZERO;

		public string[] ModifierNames;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Distance != null)
			{
				HashUtils.ContentHashOnto(Distance.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Distance.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Distance.dynamicKey, ref lastHash);
			}
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
			return new AbilityMonsterBlurDistanceBeyondMixin(instancedAbility, instancedModifier, this);
		}
	}
}
