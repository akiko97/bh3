namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterTeleportMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat BaselineDistance;

		public MixinEffect TeleportFromEffect;

		public float TeleportInverval = 0.2f;

		public bool towardsTarget = true;

		public MixinEffect TeleportToEffect;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (BaselineDistance != null)
			{
				HashUtils.ContentHashOnto(BaselineDistance.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(BaselineDistance.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(BaselineDistance.dynamicKey, ref lastHash);
			}
			if (TeleportFromEffect != null)
			{
				HashUtils.ContentHashOnto(TeleportFromEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(TeleportFromEffect.AudioPattern, ref lastHash);
			}
			HashUtils.ContentHashOnto(TeleportInverval, ref lastHash);
			HashUtils.ContentHashOnto(towardsTarget, ref lastHash);
			if (TeleportToEffect != null)
			{
				HashUtils.ContentHashOnto(TeleportToEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(TeleportToEffect.AudioPattern, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterTeleportMixin(instancedAbility, instancedModifier, this);
		}
	}
}
