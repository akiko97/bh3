namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterSkillIDChargeAnimatorMixin : ConfigAbilityMixin, IHashable
	{
		public string NextLoopTriggerID;

		public string AfterSkillTriggerID;

		public string[] BeforeSkillIDs;

		public string[] ChargeLoopSkillIDs;

		public string[] AfterSkillIDs;

		public string[] TransientSkillIDs = Miscs.EMPTY_STRINGS;

		public float[] ChargeLoopDurations;

		public DynamicFloat ChargeTimeRatio = DynamicFloat.ONE;

		public MixinEffect[] ChargeLoopEffects;

		public MixinEffect[] ChargeSwitchEffects;

		public string[] ChargeLoopAudioPatterns;

		public string[] ChargeSwitchAudioPatterns;

		public float ChargeSwitchWindow = 0.4f;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(NextLoopTriggerID, ref lastHash);
			HashUtils.ContentHashOnto(AfterSkillTriggerID, ref lastHash);
			if (BeforeSkillIDs != null)
			{
				string[] beforeSkillIDs = BeforeSkillIDs;
				foreach (string value in beforeSkillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (ChargeLoopSkillIDs != null)
			{
				string[] chargeLoopSkillIDs = ChargeLoopSkillIDs;
				foreach (string value2 in chargeLoopSkillIDs)
				{
					HashUtils.ContentHashOnto(value2, ref lastHash);
				}
			}
			if (AfterSkillIDs != null)
			{
				string[] afterSkillIDs = AfterSkillIDs;
				foreach (string value3 in afterSkillIDs)
				{
					HashUtils.ContentHashOnto(value3, ref lastHash);
				}
			}
			if (TransientSkillIDs != null)
			{
				string[] transientSkillIDs = TransientSkillIDs;
				foreach (string value4 in transientSkillIDs)
				{
					HashUtils.ContentHashOnto(value4, ref lastHash);
				}
			}
			if (ChargeLoopDurations != null)
			{
				float[] chargeLoopDurations = ChargeLoopDurations;
				foreach (float value5 in chargeLoopDurations)
				{
					HashUtils.ContentHashOnto(value5, ref lastHash);
				}
			}
			if (ChargeTimeRatio != null)
			{
				HashUtils.ContentHashOnto(ChargeTimeRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ChargeTimeRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ChargeTimeRatio.dynamicKey, ref lastHash);
			}
			if (ChargeLoopEffects != null)
			{
				MixinEffect[] chargeLoopEffects = ChargeLoopEffects;
				foreach (MixinEffect mixinEffect in chargeLoopEffects)
				{
					HashUtils.ContentHashOnto(mixinEffect.EffectPattern, ref lastHash);
					HashUtils.ContentHashOnto(mixinEffect.AudioPattern, ref lastHash);
				}
			}
			if (ChargeSwitchEffects != null)
			{
				MixinEffect[] chargeSwitchEffects = ChargeSwitchEffects;
				foreach (MixinEffect mixinEffect2 in chargeSwitchEffects)
				{
					HashUtils.ContentHashOnto(mixinEffect2.EffectPattern, ref lastHash);
					HashUtils.ContentHashOnto(mixinEffect2.AudioPattern, ref lastHash);
				}
			}
			if (ChargeLoopAudioPatterns != null)
			{
				string[] chargeLoopAudioPatterns = ChargeLoopAudioPatterns;
				foreach (string value6 in chargeLoopAudioPatterns)
				{
					HashUtils.ContentHashOnto(value6, ref lastHash);
				}
			}
			if (ChargeSwitchAudioPatterns != null)
			{
				string[] chargeSwitchAudioPatterns = ChargeSwitchAudioPatterns;
				foreach (string value7 in chargeSwitchAudioPatterns)
				{
					HashUtils.ContentHashOnto(value7, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(ChargeSwitchWindow, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterSkillIDChargeAnimatorMixin(instancedAbility, instancedModifier, this);
		}
	}
}
