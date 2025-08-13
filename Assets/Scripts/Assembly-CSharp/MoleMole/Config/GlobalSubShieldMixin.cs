namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class GlobalSubShieldMixin : ConfigAbilityMixin, IHashable
	{
		public float[] ShieldEffectRanges;

		public MixinEffect[] ShieldEffects;

		public MixinEffect ShieldSuccessEffect;

		public ConfigAbilityAction[] ShieldSuccessActions = ConfigAbilityAction.EMPTY;

		public float ShieldBrokenTimeSlow;

		public MixinEffect ShieldBrokenEffect;

		public ConfigAbilityAction[] ShieldBrokenActions = ConfigAbilityAction.EMPTY;

		public int ShieldSuccessAddFrameHalt;

		public string ShieldOffModifierName;

		public DynamicFloat ShieldDefenceRatio;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (ShieldEffectRanges != null)
			{
				float[] shieldEffectRanges = ShieldEffectRanges;
				foreach (float value in shieldEffectRanges)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (ShieldEffects != null)
			{
				MixinEffect[] shieldEffects = ShieldEffects;
				foreach (MixinEffect mixinEffect in shieldEffects)
				{
					HashUtils.ContentHashOnto(mixinEffect.EffectPattern, ref lastHash);
					HashUtils.ContentHashOnto(mixinEffect.AudioPattern, ref lastHash);
				}
			}
			if (ShieldSuccessEffect != null)
			{
				HashUtils.ContentHashOnto(ShieldSuccessEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(ShieldSuccessEffect.AudioPattern, ref lastHash);
			}
			if (ShieldSuccessActions != null)
			{
				ConfigAbilityAction[] shieldSuccessActions = ShieldSuccessActions;
				foreach (ConfigAbilityAction configAbilityAction in shieldSuccessActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			HashUtils.ContentHashOnto(ShieldBrokenTimeSlow, ref lastHash);
			if (ShieldBrokenEffect != null)
			{
				HashUtils.ContentHashOnto(ShieldBrokenEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(ShieldBrokenEffect.AudioPattern, ref lastHash);
			}
			if (ShieldBrokenActions != null)
			{
				ConfigAbilityAction[] shieldBrokenActions = ShieldBrokenActions;
				foreach (ConfigAbilityAction configAbilityAction2 in shieldBrokenActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
					}
				}
			}
			HashUtils.ContentHashOnto(ShieldSuccessAddFrameHalt, ref lastHash);
			HashUtils.ContentHashOnto(ShieldOffModifierName, ref lastHash);
			if (ShieldDefenceRatio != null)
			{
				HashUtils.ContentHashOnto(ShieldDefenceRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ShieldDefenceRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ShieldDefenceRatio.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityGlobalSubShieldMixin(instancedAbility, instancedModifier, this);
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[2][] { ShieldSuccessActions, ShieldBrokenActions };
		}
	}
}
