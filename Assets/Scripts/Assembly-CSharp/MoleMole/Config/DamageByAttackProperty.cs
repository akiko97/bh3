namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DamageByAttackProperty : ConfigAbilityAction, IHashable
	{
		public ConfigEntityAttackProperty AttackProperty;

		public ConfigEntityAttackEffect AttackEffect;

		public ConfigEntityCameraShake CameraShake;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (AttackProperty != null)
			{
				HashUtils.ContentHashOnto(AttackProperty.DamagePercentage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.AddedDamageValue, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.NormalDamage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.NormalDamagePercentage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.FireDamage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.FireDamagePercentage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.ThunderDamage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.ThunderDamagePercentage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.IceDamage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.IceDamagePercentage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.AlienDamage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.AlienDamagePercentage, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.AniDamageRatio, ref lastHash);
				HashUtils.ContentHashOnto((int)AttackProperty.HitType, ref lastHash);
				HashUtils.ContentHashOnto((int)AttackProperty.HitEffect, ref lastHash);
				HashUtils.ContentHashOnto((int)AttackProperty.HitEffectAux, ref lastHash);
				HashUtils.ContentHashOnto((int)AttackProperty.KillEffect, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.FrameHalt, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.RetreatVelocity, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.IsAnimEventAttack, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.IsInComboCount, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.SPRecover, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.WitchTimeRatio, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.NoTriggerEvadeAndDefend, ref lastHash);
				HashUtils.ContentHashOnto(AttackProperty.NoBreakFrameHaltAdd, ref lastHash);
				HashUtils.ContentHashOnto((int)AttackProperty.AttackTargetting, ref lastHash);
				if (AttackProperty.CategoryTag != null)
				{
					AttackResult.AttackCategoryTag[] categoryTag = AttackProperty.CategoryTag;
					foreach (AttackResult.AttackCategoryTag value in categoryTag)
					{
						HashUtils.ContentHashOnto((int)value, ref lastHash);
					}
				}
			}
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
			abilityPlugin.DamageByAttackPropertyHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
		{
			output = string.Format("{0} 对 {1} 开始通过技能造成攻击 AttackProperty {2}", Miscs.GetDebugActorName(instancedAbility.caster), Miscs.GetDebugActorName(target), AttackProperty.GetDebugOutput());
			return true;
		}
	}
}
