namespace MoleMole.Config
{
	public class PoisonOption : ConfigAbilityStateOption
	{
		public DynamicFloat PoisonCD;

		public DynamicFloat PoisonDamage = DynamicFloat.ZERO;

		public DynamicFloat DamagePercentage = DynamicFloat.ZERO;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.Poisoned;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			modifier.ThinkInterval = PoisonCD;
			DamageByAttackValue damageByAttackValue = new DamageByAttackValue();
			damageByAttackValue.Target = AbilityTargetting.Self;
			damageByAttackValue.DamagePercentage = DamagePercentage;
			damageByAttackValue.AddedDamageValue = PoisonDamage;
			DamageByAttackValue element = damageByAttackValue;
			Miscs.ArrayAppend(ref modifier.OnThinkInterval, element);
		}
	}
}
