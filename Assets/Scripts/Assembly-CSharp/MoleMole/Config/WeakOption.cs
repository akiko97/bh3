namespace MoleMole.Config
{
	public class WeakOption : ConfigAbilityStateOption
	{
		public DynamicFloat AttackRatio;

		public DynamicFloat AniDamageRatio = DynamicFloat.ZERO;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.Weak;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.Properties.Add("Actor_AttackRatio", AttackRatio);
			modifier.Properties.Add("Actor_AniDamageDelta", AniDamageRatio);
		}
	}
}
