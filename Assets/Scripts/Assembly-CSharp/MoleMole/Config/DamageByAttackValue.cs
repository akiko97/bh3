namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DamageByAttackValue : ConfigAbilityAction, IHashable
	{
		public DynamicFloat DamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat AddedDamageValue = DynamicFloat.ZERO;

		public DynamicFloat PlainDamage = DynamicFloat.ZERO;

		public DynamicFloat PlainDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat FireDamage = DynamicFloat.ZERO;

		public DynamicFloat FireDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat ThunderDamage = DynamicFloat.ZERO;

		public DynamicFloat ThunderDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat IceDamage = DynamicFloat.ZERO;

		public DynamicFloat IceDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat AlienDamage = DynamicFloat.ZERO;

		public DynamicFloat AlienDamagePercentage = DynamicFloat.ZERO;

		public DynamicFloat AniDamageRatio = DynamicFloat.ZERO;

		public DynamicFloat RetreatVelocity = DynamicFloat.ZERO;

		public DynamicInt FrameHalt = DynamicInt.ZERO;

		public AttackResult.AnimatorHitEffect HitEffect = AttackResult.AnimatorHitEffect.Normal;

		public ConfigEntityAttackEffect AttackEffect;

		public ConfigEntityCameraShake CameraShake;

		public bool IsAnimEventAttack;

		public bool IsInComboCount;

		public AttackResult.ActorHitLevel HitLevel;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (DamagePercentage != null)
			{
				HashUtils.ContentHashOnto(DamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DamagePercentage.dynamicKey, ref lastHash);
			}
			if (AddedDamageValue != null)
			{
				HashUtils.ContentHashOnto(AddedDamageValue.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AddedDamageValue.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AddedDamageValue.dynamicKey, ref lastHash);
			}
			if (PlainDamage != null)
			{
				HashUtils.ContentHashOnto(PlainDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PlainDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PlainDamage.dynamicKey, ref lastHash);
			}
			if (PlainDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(PlainDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PlainDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PlainDamagePercentage.dynamicKey, ref lastHash);
			}
			if (FireDamage != null)
			{
				HashUtils.ContentHashOnto(FireDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(FireDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(FireDamage.dynamicKey, ref lastHash);
			}
			if (FireDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(FireDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(FireDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(FireDamagePercentage.dynamicKey, ref lastHash);
			}
			if (ThunderDamage != null)
			{
				HashUtils.ContentHashOnto(ThunderDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ThunderDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ThunderDamage.dynamicKey, ref lastHash);
			}
			if (ThunderDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(ThunderDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ThunderDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ThunderDamagePercentage.dynamicKey, ref lastHash);
			}
			if (IceDamage != null)
			{
				HashUtils.ContentHashOnto(IceDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(IceDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(IceDamage.dynamicKey, ref lastHash);
			}
			if (IceDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(IceDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(IceDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(IceDamagePercentage.dynamicKey, ref lastHash);
			}
			if (AlienDamage != null)
			{
				HashUtils.ContentHashOnto(AlienDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AlienDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AlienDamage.dynamicKey, ref lastHash);
			}
			if (AlienDamagePercentage != null)
			{
				HashUtils.ContentHashOnto(AlienDamagePercentage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AlienDamagePercentage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AlienDamagePercentage.dynamicKey, ref lastHash);
			}
			if (AniDamageRatio != null)
			{
				HashUtils.ContentHashOnto(AniDamageRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(AniDamageRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(AniDamageRatio.dynamicKey, ref lastHash);
			}
			if (RetreatVelocity != null)
			{
				HashUtils.ContentHashOnto(RetreatVelocity.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(RetreatVelocity.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(RetreatVelocity.dynamicKey, ref lastHash);
			}
			if (FrameHalt != null)
			{
				HashUtils.ContentHashOnto(FrameHalt.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(FrameHalt.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(FrameHalt.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto((int)HitEffect, ref lastHash);
			if (AttackEffect != null)
			{
				HashUtils.ContentHashOnto(AttackEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(AttackEffect.SwitchName, ref lastHash);
				HashUtils.ContentHashOnto(AttackEffect.MuteAttackEffect, ref lastHash);
				HashUtils.ContentHashOnto((int)AttackEffect.AttackEffectTriggerPos, ref lastHash);
			}
			if (CameraShake != null)
			{
				HashUtils.ContentHashOnto(CameraShake.ShakeOnNotHit, ref lastHash);
				HashUtils.ContentHashOnto(CameraShake.ShakeRange, ref lastHash);
				HashUtils.ContentHashOnto(CameraShake.ShakeTime, ref lastHash);
				if (CameraShake.ShakeAngle.HasValue)
				{
					HashUtils.ContentHashOnto(CameraShake.ShakeAngle.Value, ref lastHash);
				}
				HashUtils.ContentHashOnto(CameraShake.ShakeStepFrame, ref lastHash);
				HashUtils.ContentHashOnto(CameraShake.ClearPreviousShake, ref lastHash);
			}
			HashUtils.ContentHashOnto(IsAnimEventAttack, ref lastHash);
			HashUtils.ContentHashOnto(IsInComboCount, ref lastHash);
			HashUtils.ContentHashOnto((int)HitLevel, ref lastHash);
			HashUtils.ContentHashOnto((int)Target, ref lastHash);
			if (TargetOption != null && TargetOption.Range != null)
			{
				HashUtils.ContentHashOnto(TargetOption.Range.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(TargetOption.Range.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(TargetOption.Range.dynamicKey, ref lastHash);
			}
			if (Predicates == null)
			{
				return;
			}
			ConfigAbilityPredicate[] predicates = Predicates;
			foreach (ConfigAbilityPredicate configAbilityPredicate in predicates)
			{
				if (configAbilityPredicate is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityPredicate, ref lastHash);
				}
			}
		}

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.DamageByAttackValueHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		public override MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return mpAbilityPlugin.DamageByAttackValue_AuthorityHandler;
		}

		public override MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return MPActorAbilityPlugin.STUB_RemoteMute;
		}
	}
}
