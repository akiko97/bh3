namespace MoleMole.Config
{
	public class AttackSpeedUpOption : ConfigAbilityStateOption
	{
		public DynamicFloat AttackSpeedRatio;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.AttackSpeedUp;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.Properties.Add("Entity_AttackSpeed", AttackSpeedRatio);
		}
	}
}
