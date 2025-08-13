namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class SkillIDChangeWithNormalizedTimeMixin : ConfigAbilityMixin, IHashable
	{
		public string SkillIDFrom;

		public string SkillIDTo;

		public float NormalizedTimeStart;

		public float NormalizedTimeStop = 1f;

		public ConfigAbilityAction[] NormalizedTimeStartActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] SkillIDChangeActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(SkillIDFrom, ref lastHash);
			HashUtils.ContentHashOnto(SkillIDTo, ref lastHash);
			HashUtils.ContentHashOnto(NormalizedTimeStart, ref lastHash);
			HashUtils.ContentHashOnto(NormalizedTimeStop, ref lastHash);
			if (NormalizedTimeStartActions != null)
			{
				ConfigAbilityAction[] normalizedTimeStartActions = NormalizedTimeStartActions;
				foreach (ConfigAbilityAction configAbilityAction in normalizedTimeStartActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (SkillIDChangeActions == null)
			{
				return;
			}
			ConfigAbilityAction[] skillIDChangeActions = SkillIDChangeActions;
			foreach (ConfigAbilityAction configAbilityAction2 in skillIDChangeActions)
			{
				if (configAbilityAction2 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[2][] { NormalizedTimeStartActions, SkillIDChangeActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilitySkillIDChangeWithNormalizedTimeMixin(instancedAbility, instancedModifier, this);
		}
	}
}
