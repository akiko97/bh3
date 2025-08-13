namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSwitchRoleMixin : ConfigAbilityMixin, IHashable
	{
		public ConfigAbilityAction[] SwitchInActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] SwitchOutActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (SwitchInActions != null)
			{
				ConfigAbilityAction[] switchInActions = SwitchInActions;
				foreach (ConfigAbilityAction configAbilityAction in switchInActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (SwitchOutActions == null)
			{
				return;
			}
			ConfigAbilityAction[] switchOutActions = SwitchOutActions;
			foreach (ConfigAbilityAction configAbilityAction2 in switchOutActions)
			{
				if (configAbilityAction2 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarSwitchRoleMixin(instancedAbility, instancedModifier, this);
		}
	}
}
