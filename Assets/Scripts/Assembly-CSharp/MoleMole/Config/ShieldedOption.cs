namespace MoleMole.Config
{
	public class ShieldedOption : ConfigAbilityStateOption
	{
		public DynamicFloat DefenceRatio;

		public DynamicFloat AniDefenceRatio = DynamicFloat.ZERO;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.Shielded;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.Properties.Add("Actor_DefenceRatio", DefenceRatio);
			modifier.Properties.Add("Actor_AniDefenceDelta", AniDefenceRatio);
		}
	}
}
