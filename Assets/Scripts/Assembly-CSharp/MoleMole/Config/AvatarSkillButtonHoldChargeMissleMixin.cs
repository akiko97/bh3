namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSkillButtonHoldChargeMissleMixin : AvatarSkillButtonHoldChargeAnimatorMixin, IHashable
	{
		public string AbilityName;

		public string AbilityNameSub;

		public int[] ChargeMissleAmount;

		public new void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(AbilityName, ref lastHash);
			HashUtils.ContentHashOnto(AbilityNameSub, ref lastHash);
			if (ChargeMissleAmount != null)
			{
				int[] chargeMissleAmount = ChargeMissleAmount;
				foreach (int value in chargeMissleAmount)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (ChargeLoopDurations != null)
			{
				float[] chargeLoopDurations = ChargeLoopDurations;
				foreach (float value2 in chargeLoopDurations)
				{
					HashUtils.ContentHashOnto(value2, ref lastHash);
				}
			}
			if (ChargeTimeRatio != null)
			{
				HashUtils.ContentHashOnto(ChargeTimeRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ChargeTimeRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ChargeTimeRatio.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(AllowHoldLockDirection, ref lastHash);
			HashUtils.ContentHashOnto(SkillButtonID, ref lastHash);
			HashUtils.ContentHashOnto(NextLoopTriggerID, ref lastHash);
			HashUtils.ContentHashOnto(AfterSkillTriggerID, ref lastHash);
			if (BeforeSkillIDs != null)
			{
				string[] beforeSkillIDs = BeforeSkillIDs;
				foreach (string value3 in beforeSkillIDs)
				{
					HashUtils.ContentHashOnto(value3, ref lastHash);
				}
			}
			if (ChargeLoopSkillIDs != null)
			{
				string[] chargeLoopSkillIDs = ChargeLoopSkillIDs;
				foreach (string value4 in chargeLoopSkillIDs)
				{
					HashUtils.ContentHashOnto(value4, ref lastHash);
				}
			}
			if (AfterSkillIDs != null)
			{
				string[] afterSkillIDs = AfterSkillIDs;
				foreach (string value5 in afterSkillIDs)
				{
					HashUtils.ContentHashOnto(value5, ref lastHash);
				}
			}
			if (TransientSkillIDs != null)
			{
				string[] transientSkillIDs = TransientSkillIDs;
				foreach (string value6 in transientSkillIDs)
				{
					HashUtils.ContentHashOnto(value6, ref lastHash);
				}
			}
			if (ChargeSubTargetAmount != null)
			{
				int[] chargeSubTargetAmount = ChargeSubTargetAmount;
				foreach (int value7 in chargeSubTargetAmount)
				{
					HashUtils.ContentHashOnto(value7, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(SubTargetModifierName, ref lastHash);
			HashUtils.ContentHashOnto(ChargeTimeRatioAIKey, ref lastHash);
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
				foreach (string value8 in chargeLoopAudioPatterns)
				{
					HashUtils.ContentHashOnto(value8, ref lastHash);
				}
			}
			if (ChargeSwitchAudioPatterns != null)
			{
				string[] chargeSwitchAudioPatterns = ChargeSwitchAudioPatterns;
				foreach (string value9 in chargeSwitchAudioPatterns)
				{
					HashUtils.ContentHashOnto(value9, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(ImmediatelyDetachLoopEffect, ref lastHash);
			HashUtils.ContentHashOnto(ChargeSwitchWindow, ref lastHash);
			HashUtils.ContentHashOnto(DisallowReleaseButtonInBS, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarSkillButtonChargeMissleMixin(instancedAbility, instancedModifier, this);
		}
	}
}
