namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ModifyDamageMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat AnimDamageRatioDelta = DynamicFloat.ZERO;

		public DynamicFloat CritChanceDelta = DynamicFloat.ZERO;

		public DynamicFloat CritDamageRatioDelta = DynamicFloat.ZERO;

		public DynamicFloat AddedAttackRatio = DynamicFloat.ZERO;

		public DynamicFloat AddedDamageValue = DynamicFloat.ZERO;

		public DynamicFloat AddedDamageRatio = DynamicFloat.ZERO;

		public DynamicFloat AddedDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat AllDamageReduceRatio = DynamicFloat.ZERO;

		public DynamicFloat NormalDamage = DynamicFloat.ZERO;

		public DynamicFloat FireDamage = DynamicFloat.ZERO;

		public DynamicFloat ThunderDamage = DynamicFloat.ZERO;

		public DynamicFloat IceDamage = DynamicFloat.ZERO;

		public DynamicFloat AllienDamage = DynamicFloat.ZERO;

		public DynamicFloat NormalDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat FireDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat ThunderDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat IceDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat AllienDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat AniDamageRatio = DynamicFloat.ZERO;

		public DynamicFloat ModifyChance = DynamicFloat.ONE;

		public bool IncludeNonAnimEventAttacks;

		public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] BreakActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (AnimDamageRatioDelta != null)
			{
				HashUtils.ContentHashOnto(AnimDamageRatioDelta.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AnimDamageRatioDelta.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AnimDamageRatioDelta.dynamicKey, ref lastHash);
			}
			if (CritChanceDelta != null)
			{
				HashUtils.ContentHashOnto(CritChanceDelta.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(CritChanceDelta.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(CritChanceDelta.dynamicKey, ref lastHash);
			}
			if (CritDamageRatioDelta != null)
			{
				HashUtils.ContentHashOnto(CritDamageRatioDelta.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(CritDamageRatioDelta.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(CritDamageRatioDelta.dynamicKey, ref lastHash);
			}
			if (AddedAttackRatio != null)
			{
				HashUtils.ContentHashOnto(AddedAttackRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AddedAttackRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AddedAttackRatio.dynamicKey, ref lastHash);
			}
			if (AddedDamageValue != null)
			{
				HashUtils.ContentHashOnto(AddedDamageValue.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AddedDamageValue.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AddedDamageValue.dynamicKey, ref lastHash);
			}
			if (AddedDamageRatio != null)
			{
				HashUtils.ContentHashOnto(AddedDamageRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AddedDamageRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AddedDamageRatio.dynamicKey, ref lastHash);
			}
			if (AddedDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(AddedDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AddedDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AddedDamagePercentage.dynamicKey, ref lastHash);
			}
			if (AllDamageReduceRatio != null)
			{
				HashUtils.ContentHashOnto(AllDamageReduceRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AllDamageReduceRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AllDamageReduceRatio.dynamicKey, ref lastHash);
			}
			if (NormalDamage != null)
			{
				HashUtils.ContentHashOnto(NormalDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(NormalDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(NormalDamage.dynamicKey, ref lastHash);
			}
			if (FireDamage != null)
			{
				HashUtils.ContentHashOnto(FireDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(FireDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(FireDamage.dynamicKey, ref lastHash);
			}
			if (ThunderDamage != null)
			{
				HashUtils.ContentHashOnto(ThunderDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ThunderDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ThunderDamage.dynamicKey, ref lastHash);
			}
			if (IceDamage != null)
			{
				HashUtils.ContentHashOnto(IceDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(IceDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(IceDamage.dynamicKey, ref lastHash);
			}
			if (AllienDamage != null)
			{
				HashUtils.ContentHashOnto(AllienDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AllienDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AllienDamage.dynamicKey, ref lastHash);
			}
			if (NormalDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(NormalDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(NormalDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(NormalDamagePercentage.dynamicKey, ref lastHash);
			}
			if (FireDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(FireDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(FireDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(FireDamagePercentage.dynamicKey, ref lastHash);
			}
			if (ThunderDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(ThunderDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ThunderDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ThunderDamagePercentage.dynamicKey, ref lastHash);
			}
			if (IceDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(IceDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(IceDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(IceDamagePercentage.dynamicKey, ref lastHash);
			}
			if (AllienDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(AllienDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AllienDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AllienDamagePercentage.dynamicKey, ref lastHash);
			}
			if (AniDamageRatio != null)
			{
				HashUtils.ContentHashOnto(AniDamageRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AniDamageRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AniDamageRatio.dynamicKey, ref lastHash);
			}
			if (ModifyChance != null)
			{
				HashUtils.ContentHashOnto(ModifyChance.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ModifyChance.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ModifyChance.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(IncludeNonAnimEventAttacks, ref lastHash);
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
			if (BreakActions != null)
			{
				ConfigAbilityAction[] breakActions = BreakActions;
				foreach (ConfigAbilityAction configAbilityAction2 in breakActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
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
			return new AbilityModifiyDamageMixin(instancedAbility, instancedModifier, this);
		}
	}
}
