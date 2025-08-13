namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DistanceBeyondMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat HitDistanceBeyond = DynamicFloat.ZERO;

		public bool Reverse;

		public DynamicFloat Distance = DynamicFloat.ZERO;

		public DynamicFloat DamagePercentageUp = DynamicFloat.ZERO;

		public DynamicFloat AttackRatio = DynamicFloat.ZERO;

		public DynamicFloat AniDamageRatioUp = DynamicFloat.ZERO;

		public DynamicFloat CriticalChanceRatioUp = DynamicFloat.ZERO;

		public DynamicFloat CriticalDamageRatioUp = DynamicFloat.ZERO;

		public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (HitDistanceBeyond != null)
			{
				HashUtils.ContentHashOnto(HitDistanceBeyond.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(HitDistanceBeyond.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(HitDistanceBeyond.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(Reverse, ref lastHash);
			if (Distance != null)
			{
				HashUtils.ContentHashOnto(Distance.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Distance.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Distance.dynamicKey, ref lastHash);
			}
			if (DamagePercentageUp != null)
			{
				HashUtils.ContentHashOnto(DamagePercentageUp.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DamagePercentageUp.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DamagePercentageUp.dynamicKey, ref lastHash);
			}
			if (AttackRatio != null)
			{
				HashUtils.ContentHashOnto(AttackRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AttackRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AttackRatio.dynamicKey, ref lastHash);
			}
			if (AniDamageRatioUp != null)
			{
				HashUtils.ContentHashOnto(AniDamageRatioUp.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AniDamageRatioUp.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AniDamageRatioUp.dynamicKey, ref lastHash);
			}
			if (CriticalChanceRatioUp != null)
			{
				HashUtils.ContentHashOnto(CriticalChanceRatioUp.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(CriticalChanceRatioUp.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(CriticalChanceRatioUp.dynamicKey, ref lastHash);
			}
			if (CriticalDamageRatioUp != null)
			{
				HashUtils.ContentHashOnto(CriticalDamageRatioUp.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(CriticalDamageRatioUp.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(CriticalDamageRatioUp.dynamicKey, ref lastHash);
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
			return new AbilityDistanceBeyondMixin(instancedAbility, instancedModifier, this);
		}
	}
}
