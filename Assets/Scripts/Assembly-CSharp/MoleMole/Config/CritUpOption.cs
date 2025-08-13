namespace MoleMole.Config
{
	public class CritUpOption : ConfigAbilityStateOption
	{
		public DynamicFloat CritChanceDelta;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.CritUp;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.Properties.Add("Actor_CriticalChanceDelta", CritChanceDelta);
		}
	}
}
