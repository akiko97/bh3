namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ModifyAnimEventAttackMixin : ConfigAbilityMixin, IHashable
	{
		public string[] AnimEventIDs;

		public bool ModifyAllAnimEvents;

		public DynamicFloat Dmage = DynamicFloat.ZERO;

		public DynamicFloat AnimDamageRatioDelta = DynamicFloat.ZERO;

		public DynamicFloat CritChanceDelta = DynamicFloat.ZERO;

		public DynamicFloat CritDamageRatioDelta = DynamicFloat.ZERO;

		public DynamicFloat DamagePercentageDelta = DynamicFloat.ZERO;

		public DynamicFloat ShieldDamageDelta = DynamicFloat.ZERO;

		public DynamicFloat AttackValueDelta = DynamicFloat.ZERO;

		public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (AnimEventIDs != null)
			{
				string[] animEventIDs = AnimEventIDs;
				foreach (string value in animEventIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(ModifyAllAnimEvents, ref lastHash);
			if (Dmage != null)
			{
				HashUtils.ContentHashOnto(Dmage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Dmage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Dmage.dynamicKey, ref lastHash);
			}
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
			if (DamagePercentageDelta != null)
			{
				HashUtils.ContentHashOnto(DamagePercentageDelta.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DamagePercentageDelta.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DamagePercentageDelta.dynamicKey, ref lastHash);
			}
			if (ShieldDamageDelta != null)
			{
				HashUtils.ContentHashOnto(ShieldDamageDelta.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ShieldDamageDelta.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ShieldDamageDelta.dynamicKey, ref lastHash);
			}
			if (AttackValueDelta != null)
			{
				HashUtils.ContentHashOnto(AttackValueDelta.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AttackValueDelta.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AttackValueDelta.dynamicKey, ref lastHash);
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
			return new AbilityModifyAnimEventAttackMixin(instancedAbility, instancedModifier, this);
		}
	}
}
