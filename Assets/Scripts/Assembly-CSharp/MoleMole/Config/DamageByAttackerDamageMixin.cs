namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DamageByAttackerDamageMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat DamagePercentage = DynamicFloat.ZERO;

		public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (DamagePercentage != null)
			{
				HashUtils.ContentHashOnto(DamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DamagePercentage.dynamicKey, ref lastHash);
			}
			if (Actions != null)
			{
				ConfigAbilityAction[] actions = Actions;
				foreach (ConfigAbilityAction configAbilityAction in actions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (Predicates == null)
			{
				return;
			}
			ConfigAbilityPredicate[] predicates = Predicates;
			foreach (ConfigAbilityPredicate configAbilityPredicate in predicates)
			{
				if (configAbilityPredicate is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityPredicate, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityDamageByAttackerDamageMixin(instancedAbility, instancedModifier, this);
		}
	}
}
