namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class HitExplodeBulletMixin : ConfigAbilityMixin, IHashable
	{
		public string BulletTypeName;

		public MixinTargetting Targetting = MixinTargetting.Enemy;

		public DynamicFloat BulletSpeed;

		public bool IgnoreTimeScale;

		public DynamicFloat AliveDuration = new DynamicFloat
		{
			fixedValue = -1f
		};

		public DynamicFloat HitExplodeRadius;

		public bool IsFixedHeight;

		public MixinEffect BulletEffect;

		public bool ApplyDistinctHitExplodeEffectPattern;

		public DynamicFloat DistinctHitExplodeHeight = DynamicFloat.ONE;

		public MixinEffect HitExplodeEffect;

		public MixinEffect HitExplodeEffectAir;

		public MixinEffect HitExplodeEffectGround;

		public MixinEffect SelfExplodeEffect;

		public bool MuteSelfHitExplodeActions;

		public bool IsHitChangeTargetDirection;

		public string HitAnimEventID;

		public bool FaceTarget;

		public BulletClearBehavior RemoveClearType;

		public BulletHitBehavior BulletHitType;

		public bool BulletEffectGround = true;

		public bool ExplodeEffectGround = true;

		public float ResetTime = 0.4f;

		public ConfigAbilityAction[] HitExplodeActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(BulletTypeName, ref lastHash);
			HashUtils.ContentHashOnto((int)Targetting, ref lastHash);
			if (BulletSpeed != null)
			{
				HashUtils.ContentHashOnto(BulletSpeed.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(BulletSpeed.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(BulletSpeed.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(IgnoreTimeScale, ref lastHash);
			if (AliveDuration != null)
			{
				HashUtils.ContentHashOnto(AliveDuration.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AliveDuration.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AliveDuration.dynamicKey, ref lastHash);
			}
			if (HitExplodeRadius != null)
			{
				HashUtils.ContentHashOnto(HitExplodeRadius.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(HitExplodeRadius.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(HitExplodeRadius.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(IsFixedHeight, ref lastHash);
			if (BulletEffect != null)
			{
				HashUtils.ContentHashOnto(BulletEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(BulletEffect.AudioPattern, ref lastHash);
			}
			HashUtils.ContentHashOnto(ApplyDistinctHitExplodeEffectPattern, ref lastHash);
			if (DistinctHitExplodeHeight != null)
			{
				HashUtils.ContentHashOnto(DistinctHitExplodeHeight.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DistinctHitExplodeHeight.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DistinctHitExplodeHeight.dynamicKey, ref lastHash);
			}
			if (HitExplodeEffect != null)
			{
				HashUtils.ContentHashOnto(HitExplodeEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(HitExplodeEffect.AudioPattern, ref lastHash);
			}
			if (HitExplodeEffectAir != null)
			{
				HashUtils.ContentHashOnto(HitExplodeEffectAir.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(HitExplodeEffectAir.AudioPattern, ref lastHash);
			}
			if (HitExplodeEffectGround != null)
			{
				HashUtils.ContentHashOnto(HitExplodeEffectGround.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(HitExplodeEffectGround.AudioPattern, ref lastHash);
			}
			if (SelfExplodeEffect != null)
			{
				HashUtils.ContentHashOnto(SelfExplodeEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(SelfExplodeEffect.AudioPattern, ref lastHash);
			}
			HashUtils.ContentHashOnto(MuteSelfHitExplodeActions, ref lastHash);
			HashUtils.ContentHashOnto(IsHitChangeTargetDirection, ref lastHash);
			HashUtils.ContentHashOnto(HitAnimEventID, ref lastHash);
			HashUtils.ContentHashOnto(FaceTarget, ref lastHash);
			HashUtils.ContentHashOnto((int)RemoveClearType, ref lastHash);
			HashUtils.ContentHashOnto((int)BulletHitType, ref lastHash);
			HashUtils.ContentHashOnto(BulletEffectGround, ref lastHash);
			HashUtils.ContentHashOnto(ExplodeEffectGround, ref lastHash);
			HashUtils.ContentHashOnto(ResetTime, ref lastHash);
			if (HitExplodeActions == null)
			{
				return;
			}
			ConfigAbilityAction[] hitExplodeActions = HitExplodeActions;
			foreach (ConfigAbilityAction configAbilityAction in hitExplodeActions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[1][] { HitExplodeActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityHitExplodeBulletMixin(instancedAbility, instancedModifier, this);
		}

		public override BaseAbilityMixin MPCreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new MPAbilityHitExplodeBulletMixin_TotallyLocal(instancedAbility, instancedModifier, this);
		}
	}
}
