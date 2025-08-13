namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin : BaseAvatarSkillButtonHoldChargeAnimatorMixin, IHashable
	{
		public float[] ChargeLoopNormalizeTimeEnds;

		public float[][] ChargeEndNormalizeTimeThershold;

		public ConfigAbilityAction[][][] ChargeEndNormalizeTimeActions;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (ChargeLoopNormalizeTimeEnds != null)
			{
				float[] chargeLoopNormalizeTimeEnds = ChargeLoopNormalizeTimeEnds;
				foreach (float value in chargeLoopNormalizeTimeEnds)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (ChargeEndNormalizeTimeThershold != null)
			{
				float[][] chargeEndNormalizeTimeThershold = ChargeEndNormalizeTimeThershold;
				foreach (float[] array in chargeEndNormalizeTimeThershold)
				{
					if (array != null)
					{
						float[] array2 = array;
						foreach (float value2 in array2)
						{
							HashUtils.ContentHashOnto(value2, ref lastHash);
						}
					}
				}
			}
			if (ChargeEndNormalizeTimeActions != null)
			{
				ConfigAbilityAction[][][] chargeEndNormalizeTimeActions = ChargeEndNormalizeTimeActions;
				foreach (ConfigAbilityAction[][] array3 in chargeEndNormalizeTimeActions)
				{
					if (array3 == null)
					{
						continue;
					}
					ConfigAbilityAction[][] array4 = array3;
					foreach (ConfigAbilityAction[] array5 in array4)
					{
						if (array5 == null)
						{
							continue;
						}
						ConfigAbilityAction[] array6 = array5;
						foreach (ConfigAbilityAction configAbilityAction in array6)
						{
							if (configAbilityAction is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
							}
						}
					}
				}
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
			return new AbilityAvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin(instancedAbility, instancedModifier, this);
		}
	}
}
