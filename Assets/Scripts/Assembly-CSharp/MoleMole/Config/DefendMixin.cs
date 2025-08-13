namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DefendMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat DefendWindow = DynamicFloat.ONE;

		public DynamicFloat DefendPerfectStartTime = DynamicFloat.ZERO;

		public DynamicFloat DefendPerfectEndTime = DynamicFloat.ONE;

		public string DefendDurationModifierName;

		public string DefendPerfectDurationModifierName;

		public ConfigEntityAttackEffect DefendReplaceAttackEffect;

		public ConfigAbilityAction[] DefendStartActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] DefendSuccessActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] DefendSuccessPerfectActions = ConfigAbilityAction.EMPTY;

		public DefendMixin()
		{
			isUnique = true;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (DefendWindow != null)
			{
				HashUtils.ContentHashOnto(DefendWindow.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DefendWindow.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DefendWindow.dynamicKey, ref lastHash);
			}
			if (DefendPerfectStartTime != null)
			{
				HashUtils.ContentHashOnto(DefendPerfectStartTime.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DefendPerfectStartTime.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DefendPerfectStartTime.dynamicKey, ref lastHash);
			}
			if (DefendPerfectEndTime != null)
			{
				HashUtils.ContentHashOnto(DefendPerfectEndTime.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DefendPerfectEndTime.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DefendPerfectEndTime.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(DefendDurationModifierName, ref lastHash);
			HashUtils.ContentHashOnto(DefendPerfectDurationModifierName, ref lastHash);
			if (DefendReplaceAttackEffect != null)
			{
				HashUtils.ContentHashOnto(DefendReplaceAttackEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(DefendReplaceAttackEffect.SwitchName, ref lastHash);
				HashUtils.ContentHashOnto(DefendReplaceAttackEffect.MuteAttackEffect, ref lastHash);
				HashUtils.ContentHashOnto((int)DefendReplaceAttackEffect.AttackEffectTriggerPos, ref lastHash);
			}
			if (DefendStartActions != null)
			{
				ConfigAbilityAction[] defendStartActions = DefendStartActions;
				foreach (ConfigAbilityAction configAbilityAction in defendStartActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (DefendSuccessActions != null)
			{
				ConfigAbilityAction[] defendSuccessActions = DefendSuccessActions;
				foreach (ConfigAbilityAction configAbilityAction2 in defendSuccessActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
					}
				}
			}
			if (DefendSuccessPerfectActions == null)
			{
				return;
			}
			ConfigAbilityAction[] defendSuccessPerfectActions = DefendSuccessPerfectActions;
			foreach (ConfigAbilityAction configAbilityAction3 in defendSuccessPerfectActions)
			{
				if (configAbilityAction3 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction3, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[3][] { DefendStartActions, DefendSuccessActions, DefendSuccessPerfectActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityDefendMixin(instancedAbility, instancedModifier, this);
		}
	}
}
