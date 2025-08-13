namespace MoleMole.Config
{
	public class BleedOption : ConfigAbilityStateOption
	{
		public DynamicFloat BleedCD;

		public DynamicFloat BleedDamage = DynamicFloat.ZERO;

		public DynamicFloat DamagePercentage = DynamicFloat.ZERO;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.Bleed;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.ThinkInterval = BleedCD;
			DamageByAttackValue damageByAttackValue = new DamageByAttackValue();
			damageByAttackValue.Target = AbilityTargetting.Self;
			damageByAttackValue.DamagePercentage = DamagePercentage;
			damageByAttackValue.AddedDamageValue = BleedDamage;
			DamageByAttackValue element = damageByAttackValue;
			Miscs.ArrayAppend(ref modifier.OnThinkInterval, element);
		}
	}
}
