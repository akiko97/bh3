namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class GlobalMainShieldMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat ShieldHPRatio;

		public DynamicFloat ShieldHP;

		public DynamicFloat BetweenAttackResumeCD;

		public float ForceResumeCD;

		public float MinForceResumeCD = 2f;

		public bool UseLevelTimeScale;

		public float ForceResumeRatio;

		public float ThrowForceResumeTimeRatio = 0.3f;

		public float ControlledForceResumeTimeRatio = 0.1f;

		public AbilityState[] ControlledAbilityStates;

		public DynamicFloat ForceResumeByDamageHPRatio = DynamicFloat.ZERO;

		public float ShieldResumeTimeSpan;

		public float ShieldBrokenTimeSlow;

		public MixinEffect ShieldResumeEffect;

		public ConfigAbilityAction[] ShieldResumeActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] ShieldBrokenActions = ConfigAbilityAction.EMPTY;

		public string ChildShieldModifierName;

		public string ShieldOffModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (ShieldHPRatio != null)
			{
				HashUtils.ContentHashOnto(ShieldHPRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ShieldHPRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ShieldHPRatio.dynamicKey, ref lastHash);
			}
			if (ShieldHP != null)
			{
				HashUtils.ContentHashOnto(ShieldHP.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ShieldHP.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ShieldHP.dynamicKey, ref lastHash);
			}
			if (BetweenAttackResumeCD != null)
			{
				HashUtils.ContentHashOnto(BetweenAttackResumeCD.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(BetweenAttackResumeCD.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(BetweenAttackResumeCD.dynamicKey, ref lastHash);
			}
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
			HashUtils.ContentHashOnto(ShieldBrokenTimeSlow, ref lastHash);
			if (ShieldResumeEffect != null)
			{
				HashUtils.ContentHashOnto(ShieldResumeEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(ShieldResumeEffect.AudioPattern, ref lastHash);
			}
			if (ShieldResumeActions != null)
			{
				ConfigAbilityAction[] shieldResumeActions = ShieldResumeActions;
				foreach (ConfigAbilityAction configAbilityAction in shieldResumeActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
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
			HashUtils.ContentHashOnto(ChildShieldModifierName, ref lastHash);
			HashUtils.ContentHashOnto(ShieldOffModifierName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityGlobalMainShieldMixin(instancedAbility, instancedModifier, this);
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[2][] { ShieldResumeActions, ShieldBrokenActions };
		}
	}
}
