namespace MoleMole.Config
{
	public class MoveSpeedUpOption : ConfigAbilityStateOption
	{
		public DynamicFloat MoveSpeedRatio;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.MoveSpeedUp;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.Properties.Add("Animator_MoveSpeedRatio", MoveSpeedRatio);
		}
	}
}
