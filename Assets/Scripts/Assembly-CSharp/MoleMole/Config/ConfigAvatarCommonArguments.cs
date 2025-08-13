namespace MoleMole.Config
{
	public class ConfigAvatarCommonArguments : ConfigEntityCommonArguments
	{
		public float GoodsAttractRadius;

		public string[] MaskedSkillButtons = Miscs.EMPTY_STRINGS;

		public float SwitchInCD = 4f;

		public float QTESwitchInCDRatio = 2f;

		public float AttackSPRecoverRatio = 3f;
	}
}
