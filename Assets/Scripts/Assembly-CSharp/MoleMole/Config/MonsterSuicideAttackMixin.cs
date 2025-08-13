namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterSuicideAttackMixin : ConfigAbilityMixin, IHashable
	{
		public string SuicideHitAnimEventID;

		public string SuicideHitAlliedAnimEventID;

		public string KilledHitAnimEventID;

		public string KilledHitAlliedAnimEventID;

		public MixinEffect SuicideEffect;

		public bool IsTouchExplode;

		public string OnTouchTriggerID;

		public string WarningAudioPattern;

		public MixinEffect KilledEffect;

		public DynamicFloat SuicideCountDownDuration;

		public string SuicicdeModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(SuicideHitAnimEventID, ref lastHash);
			HashUtils.ContentHashOnto(SuicideHitAlliedAnimEventID, ref lastHash);
			HashUtils.ContentHashOnto(KilledHitAnimEventID, ref lastHash);
			HashUtils.ContentHashOnto(KilledHitAlliedAnimEventID, ref lastHash);
			if (SuicideEffect != null)
			{
				HashUtils.ContentHashOnto(SuicideEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(SuicideEffect.AudioPattern, ref lastHash);
			}
			HashUtils.ContentHashOnto(IsTouchExplode, ref lastHash);
			HashUtils.ContentHashOnto(OnTouchTriggerID, ref lastHash);
			HashUtils.ContentHashOnto(WarningAudioPattern, ref lastHash);
			if (KilledEffect != null)
			{
				HashUtils.ContentHashOnto(KilledEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(KilledEffect.AudioPattern, ref lastHash);
			}
			if (SuicideCountDownDuration != null)
			{
				HashUtils.ContentHashOnto(SuicideCountDownDuration.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(SuicideCountDownDuration.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(SuicideCountDownDuration.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(SuicicdeModifierName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterSuicideAttack(instancedAbility, instancedModifier, this);
		}
	}
}
