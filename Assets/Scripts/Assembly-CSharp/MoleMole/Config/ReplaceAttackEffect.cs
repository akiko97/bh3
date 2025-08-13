namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ReplaceAttackEffect : ConfigAbilityAction, IHashable
	{
		public ConfigEntityAttackEffect AttackEffect;

		public ConfigEntityAttackEffect BeHitEffect;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (AttackEffect != null)
			{
				HashUtils.ContentHashOnto(AttackEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(AttackEffect.SwitchName, ref lastHash);
				HashUtils.ContentHashOnto(AttackEffect.MuteAttackEffect, ref lastHash);
				HashUtils.ContentHashOnto((int)AttackEffect.AttackEffectTriggerPos, ref lastHash);
			}
			if (BeHitEffect != null)
			{
				HashUtils.ContentHashOnto(BeHitEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(BeHitEffect.SwitchName, ref lastHash);
				HashUtils.ContentHashOnto(BeHitEffect.MuteAttackEffect, ref lastHash);
				HashUtils.ContentHashOnto((int)BeHitEffect.AttackEffectTriggerPos, ref lastHash);
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
			abilityPlugin.ReplaceAttackEffectHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
