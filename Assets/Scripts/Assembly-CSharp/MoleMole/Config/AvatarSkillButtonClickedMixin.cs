namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSkillButtonClickedMixin : ConfigAbilityMixin, IHashable
	{
		public string SkillButtonID;

		public ConfigAbilityAction[] OnClickedActions = ConfigAbilityAction.EMPTY;

		public bool ConsumeClick;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(SkillButtonID, ref lastHash);
			if (OnClickedActions != null)
			{
				ConfigAbilityAction[] onClickedActions = OnClickedActions;
				foreach (ConfigAbilityAction configAbilityAction in onClickedActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			HashUtils.ContentHashOnto(ConsumeClick, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarSkillButtonClickedMixin(instancedAbility, instancedModifier, this);
		}
	}
}
