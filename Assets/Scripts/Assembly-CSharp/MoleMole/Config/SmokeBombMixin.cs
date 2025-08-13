namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class SmokeBombMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat Radius = DynamicFloat.ONE;

		public DynamicFloat Duration = DynamicFloat.ZERO;

		public MixinTargetting Target = MixinTargetting.Allied;

		public MixinEffect SmokeOnEffect;

		public MixinEffect SmokeOffEffect;

		public string[] Modifiers;

		public string[] InSmokeModifiers;

		public void ObjectContentHashOnto(ref int lastHash)
		{
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
			HashUtils.ContentHashOnto((int)Target, ref lastHash);
			if (SmokeOnEffect != null)
			{
				HashUtils.ContentHashOnto(SmokeOnEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(SmokeOnEffect.AudioPattern, ref lastHash);
			}
			if (SmokeOffEffect != null)
			{
				HashUtils.ContentHashOnto(SmokeOffEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(SmokeOffEffect.AudioPattern, ref lastHash);
			}
			if (Modifiers != null)
			{
				string[] modifiers = Modifiers;
				foreach (string value in modifiers)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (InSmokeModifiers != null)
			{
				string[] inSmokeModifiers = InSmokeModifiers;
				foreach (string value2 in inSmokeModifiers)
				{
					HashUtils.ContentHashOnto(value2, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilitySmokeBombMixin(instancedAbility, instancedModifier, this);
		}
	}
}
