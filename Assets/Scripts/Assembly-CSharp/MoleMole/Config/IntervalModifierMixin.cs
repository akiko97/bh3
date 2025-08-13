namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class IntervalModifierMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat Interval = DynamicFloat.ZERO;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public string ModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Interval != null)
			{
				HashUtils.ContentHashOnto(Interval.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Interval.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Interval.dynamicKey, ref lastHash);
			}
			if (Predicates != null)
			{
				ConfigAbilityPredicate[] predicates = Predicates;
				foreach (ConfigAbilityPredicate configAbilityPredicate in predicates)
				{
					if (configAbilityPredicate is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityPredicate, ref lastHash);
					}
				}
			}
			HashUtils.ContentHashOnto(ModifierName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityIntervalModifierMixin(instancedAbility, instancedModifier, this);
		}
	}
}
