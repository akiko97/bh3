namespace MoleMole.Config
{
	public abstract class BaseAvatarSkillButtonHoldChargeAnimatorMixin : ConfigAbilityMixin
	{
		public bool AllowHoldLockDirection;

		public string SkillButtonID;

		public string NextLoopTriggerID;

		public string AfterSkillTriggerID;

		public string[] BeforeSkillIDs;

		public string[] ChargeLoopSkillIDs;

		public string[] AfterSkillIDs;

		public string[] TransientSkillIDs = Miscs.EMPTY_STRINGS;

		public int[] ChargeSubTargetAmount;

		public string SubTargetModifierName;

		public string ChargeTimeRatioAIKey;

		public MixinEffect[] ChargeLoopEffects;

		public MixinEffect[] ChargeSwitchEffects;

		public string[] ChargeLoopAudioPatterns;

		public string[] ChargeSwitchAudioPatterns;

		public bool ImmediatelyDetachLoopEffect;

		public float ChargeSwitchWindow = 0.4f;

		public bool DisallowReleaseButtonInBS;
	}
}
