namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ShieldMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat ShieldHPRatio;

		public float BetweenAttackResumeCD;

		public float BetweenAttackResumeRatio;

		public float ForceResumeCD;

		public float MinForceResumeCD = 2f;

		public bool UseLevelTimeScale;

		public float ForceResumeRatio;

		public float ThrowForceResumeTimeRatio = 0.3f;

		public float ControlledForceResumeTimeRatio = 0.1f;

		public AbilityState[] ControlledAbilityStates;

		public DynamicFloat ForceResumeByDamageHPRatio = DynamicFloat.ZERO;

		public float ShieldResumeTimeSpan;

		public DynamicInt ShowShieldBar;

		public float ShieldDisplayRatioFloor;

		public float ShieldDisplayRatioCeiling = 1f;

		public float DamagePower = 1f;

		public float AniDamagePower = 1f;

		public float ShieldDamagePower = 1f;

		public ConfigEntityAttackEffect ShieldSuccessEffect;

		public ConfigAbilityAction[] ShieldSuccessActions = ConfigAbilityAction.EMPTY;

		public float ShieldBrokenTimeSlow;

		public MixinEffect ShieldBrokenEffect;

		public ConfigAbilityAction[] ShiedlBrokenActions = ConfigAbilityAction.EMPTY;

		public int ShieldSuccessAddFrameHalt;

		public bool MuteHitEffect = true;

		public string ShieldOnModifierName;

		public string ShieldOffModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (ShieldHPRatio != null)
			{
				HashUtils.ContentHashOnto(ShieldHPRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ShieldHPRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ShieldHPRatio.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(BetweenAttackResumeCD, ref lastHash);
			HashUtils.ContentHashOnto(BetweenAttackResumeRatio, ref lastHash);
			HashUtils.ContentHashOnto(ForceResumeCD, ref lastHash);
			HashUtils.ContentHashOnto(MinForceResumeCD, ref lastHash);
			HashUtils.ContentHashOnto(UseLevelTimeScale, ref lastHash);
			HashUtils.ContentHashOnto(ForceResumeRatio, ref lastHash);
			HashUtils.ContentHashOnto(ThrowForceResumeTimeRatio, ref lastHash);
			HashUtils.ContentHashOnto(ControlledForceResumeTimeRatio, ref lastHash);
			if (ControlledAbilityStates != null)
			{
				AbilityState[] controlledAbilityStates = ControlledAbilityStates;
				foreach (AbilityState value in controlledAbilityStates)
				{
					HashUtils.ContentHashOnto((int)value, ref lastHash);
				}
			}
			if (ForceResumeByDamageHPRatio != null)
			{
				HashUtils.ContentHashOnto(ForceResumeByDamageHPRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ForceResumeByDamageHPRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ForceResumeByDamageHPRatio.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(ShieldResumeTimeSpan, ref lastHash);
			if (ShowShieldBar != null)
			{
				HashUtils.ContentHashOnto(ShowShieldBar.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ShowShieldBar.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ShowShieldBar.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(ShieldDisplayRatioFloor, ref lastHash);
			HashUtils.ContentHashOnto(ShieldDisplayRatioCeiling, ref lastHash);
			HashUtils.ContentHashOnto(DamagePower, ref lastHash);
			HashUtils.ContentHashOnto(AniDamagePower, ref lastHash);
			HashUtils.ContentHashOnto(ShieldDamagePower, ref lastHash);
			if (ShieldSuccessEffect != null)
			{
				HashUtils.ContentHashOnto(ShieldSuccessEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(ShieldSuccessEffect.SwitchName, ref lastHash);
				HashUtils.ContentHashOnto(ShieldSuccessEffect.MuteAttackEffect, ref lastHash);
				HashUtils.ContentHashOnto((int)ShieldSuccessEffect.AttackEffectTriggerPos, ref lastHash);
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
			if (ShiedlBrokenActions != null)
			{
				ConfigAbilityAction[] shiedlBrokenActions = ShiedlBrokenActions;
				foreach (ConfigAbilityAction configAbilityAction2 in shiedlBrokenActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
					}
				}
			}
			HashUtils.ContentHashOnto(ShieldSuccessAddFrameHalt, ref lastHash);
			HashUtils.ContentHashOnto(MuteHitEffect, ref lastHash);
			HashUtils.ContentHashOnto(ShieldOnModifierName, ref lastHash);
			HashUtils.ContentHashOnto(ShieldOffModifierName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityShieldMixin(instancedAbility, instancedModifier, this);
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[2][] { ShieldSuccessActions, ShiedlBrokenActions };
		}
	}
}
