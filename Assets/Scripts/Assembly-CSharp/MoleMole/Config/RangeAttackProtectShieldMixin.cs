namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class RangeAttackProtectShieldMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat ProtectRange = new DynamicFloat
		{
			fixedValue = 3f
		};

		public DynamicFloat DamageReduceRatio = DynamicFloat.ZERO;

		public ConfigAbilityAction[] OnRangeAttackProtectShieldSuccessActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (ProtectRange != null)
			{
				HashUtils.ContentHashOnto(ProtectRange.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ProtectRange.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ProtectRange.dynamicKey, ref lastHash);
			}
			if (DamageReduceRatio != null)
			{
				HashUtils.ContentHashOnto(DamageReduceRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DamageReduceRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DamageReduceRatio.dynamicKey, ref lastHash);
			}
			if (OnRangeAttackProtectShieldSuccessActions == null)
			{
				return;
			}
			ConfigAbilityAction[] onRangeAttackProtectShieldSuccessActions = OnRangeAttackProtectShieldSuccessActions;
			foreach (ConfigAbilityAction configAbilityAction in onRangeAttackProtectShieldSuccessActions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityRangeAttackProtectShieldMixin(instancedAbility, instancedModifier, this);
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[1][] { OnRangeAttackProtectShieldSuccessActions };
		}
	}
}
