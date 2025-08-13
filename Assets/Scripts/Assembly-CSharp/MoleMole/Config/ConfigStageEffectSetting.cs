namespace MoleMole.Config
{
	public class ConfigStageEffectSetting
	{
		public static ConfigStageEffectSetting EMPTY = new ConfigStageEffectSetting();

		public string[] LocalAvatarEffectPredicates = Miscs.EMPTY_STRINGS;

		public ColorOverrideEntry[] AvatarColorOverrides = ColorOverrideEntry.EMPTY;
	}
}
