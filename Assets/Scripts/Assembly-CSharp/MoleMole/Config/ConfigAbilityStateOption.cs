namespace MoleMole.Config
{
	public abstract class ConfigAbilityStateOption
	{
		public abstract AbilityState GetMatchingAbilityState();

		public abstract void ChangeModifierConfig(ConfigAbilityModifier modifier);
	}
}
