namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class OnSwitchChargeMixin : ConfigAbilityMixin, IHashable
	{
		public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

		public string[] AfterSkillIDs;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Actions != null)
			{
				ConfigAbilityAction[] actions = Actions;
				foreach (ConfigAbilityAction configAbilityAction in actions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (AfterSkillIDs != null)
			{
				string[] afterSkillIDs = AfterSkillIDs;
				foreach (string value in afterSkillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityOnSwitchChargeMixin(instancedAbility, instancedModifier, this);
		}
	}
}
