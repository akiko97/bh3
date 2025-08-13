namespace MoleMole.Config
{
	public class BurnOption : ConfigAbilityStateOption
	{
		public DynamicFloat BurnCD;

		public DynamicFloat BurnDamage = DynamicFloat.ZERO;

		public DynamicFloat DamagePercentage = DynamicFloat.ZERO;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.Burn;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.ThinkInterval = BurnCD;
			DamageByAttackValue damageByAttackValue = new DamageByAttackValue();
			damageByAttackValue.Target = AbilityTargetting.Self;
			damageByAttackValue.FireDamagePercentage = DamagePercentage;
			damageByAttackValue.FireDamage = BurnDamage;
			DamageByAttackValue element = damageByAttackValue;
			Miscs.ArrayAppend(ref modifier.OnThinkInterval, element);
		}
	}
}
