namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DefendChargeMixin : ConfigAbilityMixin, IHashable
	{
		public string DefendBSSkillID;

		public string DefendLoopSkillID;

		public string DefnedASSkillID;

		public float DefendBSNormalizedStartTime;

		public float DefendASNormalizedEndTime;

		public DynamicFloat DefendPerfectStartTime = DynamicFloat.ZERO;

		public DynamicFloat DefendPerfectEndTime = DynamicFloat.ZERO;

		public string DefendDurationModifierName;

		public string DefendPerfectDurationModifierName;

		public ConfigEntityAttackEffect DefendReplaceAttackEffect;

		public ConfigAbilityAction[] DefendSuccessActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] DefendSuccessPerfectActions = ConfigAbilityAction.EMPTY;

		public DefendChargeMixin()
		{
			isUnique = true;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(DefendBSSkillID, ref lastHash);
			HashUtils.ContentHashOnto(DefendLoopSkillID, ref lastHash);
			HashUtils.ContentHashOnto(DefnedASSkillID, ref lastHash);
			HashUtils.ContentHashOnto(DefendBSNormalizedStartTime, ref lastHash);
			HashUtils.ContentHashOnto(DefendASNormalizedEndTime, ref lastHash);
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
			if (DefendSuccessActions != null)
			{
				ConfigAbilityAction[] defendSuccessActions = DefendSuccessActions;
				foreach (ConfigAbilityAction configAbilityAction in defendSuccessActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (DefendSuccessPerfectActions == null)
			{
				return;
			}
			ConfigAbilityAction[] defendSuccessPerfectActions = DefendSuccessPerfectActions;
			foreach (ConfigAbilityAction configAbilityAction2 in defendSuccessPerfectActions)
			{
				if (configAbilityAction2 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[2][] { DefendSuccessActions, DefendSuccessPerfectActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityDefendChargeMixin(instancedAbility, instancedModifier, this);
		}
	}
}
