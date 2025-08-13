namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DefendWithDirectionMixin : ConfigAbilityMixin, IHashable
	{
		public string[] DefendSkillIDs;

		public bool AlwaysDefend;

		public float DefendNormalizedTimeStart;

		public float DefendNormalizedTimeStop = 1f;

		public float DefendAngle = 90f;

		public bool ReverseAngle;

		public float BreakDefendAniDamageRatio = 10f;

		public float DefendDamageReduce = 1f;

		public bool DefendElemental = true;

		public AttackResult.AnimatorHitEffect DefendSuccessHitEffect;

		public ConfigAbilityAction[] DefendSuccessActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] DefendFailActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityPredicate[] DefendPredicates = ConfigAbilityPredicate.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
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
				foreach (ConfigAbilityAction configAbilityAction in defendSuccessActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (DefendFailActions != null)
			{
				ConfigAbilityAction[] defendFailActions = DefendFailActions;
				foreach (ConfigAbilityAction configAbilityAction2 in defendFailActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
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
			return new ConfigAbilityAction[2][] { DefendSuccessActions, DefendFailActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityDefendWithDirectionMixin(instancedAbility, instancedModifier, this);
		}
	}
}
