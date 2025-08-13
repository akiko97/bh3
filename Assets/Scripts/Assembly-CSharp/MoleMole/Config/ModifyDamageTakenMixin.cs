namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ModifyDamageTakenMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat DamageTakenRatio = DynamicFloat.ZERO;

		public DynamicFloat DamageTakenDelta = DynamicFloat.ZERO;

		public DynamicFloat AddAttackeeAniDefenceRatio = DynamicFloat.ZERO;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (DamageTakenRatio != null)
			{
				HashUtils.ContentHashOnto(DamageTakenRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DamageTakenRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DamageTakenRatio.dynamicKey, ref lastHash);
			}
			if (DamageTakenDelta != null)
			{
				HashUtils.ContentHashOnto(DamageTakenDelta.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DamageTakenDelta.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DamageTakenDelta.dynamicKey, ref lastHash);
			}
			if (AddAttackeeAniDefenceRatio != null)
			{
				HashUtils.ContentHashOnto(AddAttackeeAniDefenceRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AddAttackeeAniDefenceRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AddAttackeeAniDefenceRatio.dynamicKey, ref lastHash);
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
			if (Actions == null)
			{
				return;
			}
			ConfigAbilityAction[] actions = Actions;
			foreach (ConfigAbilityAction configAbilityAction in actions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityModifyDamageTakenMixin(instancedAbility, instancedModifier, this);
		}
	}
}
