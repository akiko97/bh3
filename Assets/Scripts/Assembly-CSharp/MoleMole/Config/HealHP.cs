namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class HealHP : ConfigAbilityAction, IHashable
	{
		public DynamicFloat Amount;

		public DynamicFloat AmountByCasterMaxHPRatio;

		public DynamicFloat AmountByTargetMaxHPRatio;

		public bool MuteHealEffect;

		public float HealRatio = 1f;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Amount != null)
			{
				HashUtils.ContentHashOnto(Amount.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Amount.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Amount.dynamicKey, ref lastHash);
			}
			if (AmountByCasterMaxHPRatio != null)
			{
				HashUtils.ContentHashOnto(AmountByCasterMaxHPRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AmountByCasterMaxHPRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AmountByCasterMaxHPRatio.dynamicKey, ref lastHash);
			}
			if (AmountByTargetMaxHPRatio != null)
			{
				HashUtils.ContentHashOnto(AmountByTargetMaxHPRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AmountByTargetMaxHPRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AmountByTargetMaxHPRatio.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(MuteHealEffect, ref lastHash);
			HashUtils.ContentHashOnto(HealRatio, ref lastHash);
			HashUtils.ContentHashOnto((int)Target, ref lastHash);
			if (TargetOption != null && TargetOption.Range != null)
			{
				HashUtils.ContentHashOnto(TargetOption.Range.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(TargetOption.Range.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(TargetOption.Range.dynamicKey, ref lastHash);
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

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.HealHPHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
