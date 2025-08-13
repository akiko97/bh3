namespace MoleMole.Config
{
	public class TargetLockedOption : ConfigAbilityStateOption
	{
		public DynamicFloat TakeExtraDamageRatio;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.TargetLocked;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			ModifyDamageByAttackeeMixin modifyDamageByAttackeeMixin = new ModifyDamageByAttackeeMixin();
			modifyDamageByAttackeeMixin.AddedDamageTakeRatio = TakeExtraDamageRatio;
			modifyDamageByAttackeeMixin.Predicates = new ConfigAbilityPredicate[0];
			ModifyDamageByAttackeeMixin element = modifyDamageByAttackeeMixin;
			Miscs.ArrayAppend(ref modifier.ModifierMixins, element);
		}
	}
}
