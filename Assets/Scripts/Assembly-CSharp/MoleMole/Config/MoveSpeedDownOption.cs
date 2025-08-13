namespace MoleMole.Config
{
	public class MoveSpeedDownOption : ConfigAbilityStateOption
	{
		public DynamicFloat MoveSpeedRatio;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.MoveSpeedDown;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.Properties.Add("Animator_MoveSpeedRatio", MoveSpeedRatio);
		}
	}
}
