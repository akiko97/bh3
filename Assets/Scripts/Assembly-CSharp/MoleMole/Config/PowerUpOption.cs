namespace MoleMole.Config
{
	public class PowerUpOption : ConfigAbilityStateOption
	{
		public DynamicFloat AttackRatio;

		public DynamicFloat AniDamageRatio = DynamicFloat.ZERO;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.PowerUp;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.Properties.Add("Actor_AddedAttackRatio", AttackRatio);
			modifier.Properties.Add("Actor_AniDamageDelta", AniDamageRatio);
		}
	}
}
