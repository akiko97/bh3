namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ForceInterruptMixin : ConfigAbilityMixin, IHashable
	{
		public string[] SkillIDs;

		public float TimeThreshold;

		public ConfigAbilityAction[] InterruptActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (SkillIDs != null)
			{
				string[] skillIDs = SkillIDs;
				foreach (string value in skillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(TimeThreshold, ref lastHash);
			if (InterruptActions == null)
			{
				return;
			}
			ConfigAbilityAction[] interruptActions = InterruptActions;
			foreach (ConfigAbilityAction configAbilityAction in interruptActions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[1][] { InterruptActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityForceInterruptMixin(instancedAbility, instancedModifier, this);
		}
	}
}
