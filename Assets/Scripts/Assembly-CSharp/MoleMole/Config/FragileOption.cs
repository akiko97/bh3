namespace MoleMole.Config
{
	public class FragileOption : ConfigAbilityStateOption
	{
		public DynamicFloat DefenceRatio = DynamicFloat.ZERO;

		public DynamicFloat DamageTakeRatio = DynamicFloat.ZERO;

		public DynamicFloat AniDefenceRatio = DynamicFloat.ZERO;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.Fragile;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.Properties.Add("Actor_DefenceRatio", DefenceRatio);
			modifier.Properties.Add("Actor_DamageTakeRatio", DamageTakeRatio);
			modifier.Properties.Add("Actor_AniDefenceDelta", AniDefenceRatio);
		}
	}
}
