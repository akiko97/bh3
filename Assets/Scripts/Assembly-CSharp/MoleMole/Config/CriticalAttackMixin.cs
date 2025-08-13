namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CriticalAttackMixin : ConfigAbilityMixin, IHashable
	{
		public ConfigAbilityAction[] OnCriticalAttackActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (OnCriticalAttackActions == null)
			{
				return;
			}
			ConfigAbilityAction[] onCriticalAttackActions = OnCriticalAttackActions;
			foreach (ConfigAbilityAction configAbilityAction in onCriticalAttackActions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[1][] { OnCriticalAttackActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityCriticalAttackMixin(instancedAbility, instancedModifier, this);
		}
	}
}
