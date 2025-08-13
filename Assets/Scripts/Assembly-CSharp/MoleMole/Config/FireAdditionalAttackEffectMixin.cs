namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class FireAdditionalAttackEffectMixin : ConfigAbilityMixin, IHashable
	{
		public ConfigEntityAttackEffect AttackEffect;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (AttackEffect != null)
			{
				HashUtils.ContentHashOnto(AttackEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(AttackEffect.SwitchName, ref lastHash);
				HashUtils.ContentHashOnto(AttackEffect.MuteAttackEffect, ref lastHash);
				HashUtils.ContentHashOnto((int)AttackEffect.AttackEffectTriggerPos, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityFireAdditionalAttackEffectMixin(instancedAbility, instancedModifier, this);
		}
	}
}
