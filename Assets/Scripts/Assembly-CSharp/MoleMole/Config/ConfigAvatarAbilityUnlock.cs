namespace MoleMole.Config
{
	public class ConfigAvatarAbilityUnlock
	{
		public static ConfigAvatarAbilityUnlock[] EMTPY = new ConfigAvatarAbilityUnlock[0];

		public bool IsUnlockBySkill;

		public int UnlockBySkillID;

		public int UnlockBySubSkillID;

		public string AbilityName;

		public string AbilityOverride = "Default";

		public string AbilityReplaceID;

		public string ParamSpecial1;

		public ParamMethod ParamMethod1;

		public string ParamSpecial2;

		public ParamMethod ParamMethod2;

		public string ParamSpecial3;

		public ParamMethod ParamMethod3;
	}
}
