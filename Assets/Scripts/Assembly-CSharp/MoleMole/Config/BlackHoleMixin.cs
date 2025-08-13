namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class BlackHoleMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat CreationZOffset;

		public DynamicFloat Radius;

		public DynamicFloat Duration;

		public bool ApplyAttackerWitchTimeRatio;

		public DynamicFloat PullVelocity;

		public string ExplodeAnimEventID;

		public MixinEffect CreationEffect;

		public MixinEffect DestroyEffect;

		public string[] ModifierNames;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (CreationZOffset != null)
			{
				HashUtils.ContentHashOnto(CreationZOffset.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(CreationZOffset.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(CreationZOffset.dynamicKey, ref lastHash);
			}
			if (Radius != null)
			{
				HashUtils.ContentHashOnto(Radius.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Radius.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Radius.dynamicKey, ref lastHash);
			}
			if (Duration != null)
			{
				HashUtils.ContentHashOnto(Duration.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Duration.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Duration.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(ApplyAttackerWitchTimeRatio, ref lastHash);
			if (PullVelocity != null)
			{
				HashUtils.ContentHashOnto(PullVelocity.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PullVelocity.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PullVelocity.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(ExplodeAnimEventID, ref lastHash);
			if (CreationEffect != null)
			{
				HashUtils.ContentHashOnto(CreationEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(CreationEffect.AudioPattern, ref lastHash);
			}
			if (DestroyEffect != null)
			{
				HashUtils.ContentHashOnto(DestroyEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(DestroyEffect.AudioPattern, ref lastHash);
			}
			if (ModifierNames != null)
			{
				string[] modifierNames = ModifierNames;
				foreach (string value in modifierNames)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityBlackHoleMixin(instancedAbility, instancedModifier, this);
		}
	}
}
