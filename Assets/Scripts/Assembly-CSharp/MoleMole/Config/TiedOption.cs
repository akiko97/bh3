namespace MoleMole.Config
{
	public class TiedOption : ConfigAbilityStateOption
	{
		public DynamicFloat UntieSteerAmount;

		public override AbilityState GetMatchingAbilityState()
		{
			return AbilityState.Tied;
		}

		public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
		{
			AvatarTiedMixin avatarTiedMixin = new AvatarTiedMixin();
			avatarTiedMixin.UntieSteerAmount = UntieSteerAmount;
			AvatarTiedMixin element = avatarTiedMixin;
			Miscs.ArrayAppend(ref modifier.ModifierMixins, element);
		}
	}
}
