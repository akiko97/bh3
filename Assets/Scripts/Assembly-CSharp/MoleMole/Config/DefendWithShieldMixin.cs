namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DefendWithShieldMixin : DefendWithDirectionMixin, IHashable
	{
		public DynamicFloat ShieldHPRatio;

		public float ShieldResumeSpeedByRatio;

		public float ShieldAniDamageRatioPow = 1f;

		public string ShieldRatioAnimatorParam;

		public ConfigAbilityAction[] ShieldBrokenActions = ConfigAbilityAction.EMPTY;

		public new void ObjectContentHashOnto(ref int lastHash)
		{
			if (ShieldHPRatio != null)
			{
				HashUtils.ContentHashOnto(ShieldHPRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ShieldHPRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ShieldHPRatio.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(ShieldResumeSpeedByRatio, ref lastHash);
			HashUtils.ContentHashOnto(ShieldAniDamageRatioPow, ref lastHash);
			HashUtils.ContentHashOnto(ShieldRatioAnimatorParam, ref lastHash);
			if (ShieldBrokenActions != null)
			{
				ConfigAbilityAction[] shieldBrokenActions = ShieldBrokenActions;
				foreach (ConfigAbilityAction configAbilityAction in shieldBrokenActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (DefendSkillIDs != null)
			{
				string[] defendSkillIDs = DefendSkillIDs;
				foreach (string value in defendSkillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(AlwaysDefend, ref lastHash);
			HashUtils.ContentHashOnto(DefendNormalizedTimeStart, ref lastHash);
			HashUtils.ContentHashOnto(DefendNormalizedTimeStop, ref lastHash);
			HashUtils.ContentHashOnto(DefendAngle, ref lastHash);
			HashUtils.ContentHashOnto(ReverseAngle, ref lastHash);
			HashUtils.ContentHashOnto(BreakDefendAniDamageRatio, ref lastHash);
			HashUtils.ContentHashOnto(DefendDamageReduce, ref lastHash);
			HashUtils.ContentHashOnto(DefendElemental, ref lastHash);
			HashUtils.ContentHashOnto((int)DefendSuccessHitEffect, ref lastHash);
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
			if (DefendFailActions != null)
			{
				ConfigAbilityAction[] defendFailActions = DefendFailActions;
				foreach (ConfigAbilityAction configAbilityAction3 in defendFailActions)
				{
					if (configAbilityAction3 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction3, ref lastHash);
					}
				}
			}
			if (DefendPredicates == null)
			{
				return;
			}
			ConfigAbilityPredicate[] defendPredicates = DefendPredicates;
			foreach (ConfigAbilityPredicate configAbilityPredicate in defendPredicates)
			{
				if (configAbilityPredicate is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityPredicate, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[1][] { ShieldBrokenActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityDefendWithShieldMixin(instancedAbility, instancedModifier, this);
		}
	}
}
