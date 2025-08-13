namespace MoleMole.Config
{
	public class AttackSpeedDownOption : ConfigAbilityStateOption
	{
		public DynamicFloat AttackSpeedRatio;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.AttackSpeedDown;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.Properties.Add("Entity_AttackSpeed", AttackSpeedRatio);
		}
	}
}
