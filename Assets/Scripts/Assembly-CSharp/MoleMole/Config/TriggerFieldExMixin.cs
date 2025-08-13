namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class TriggerFieldExMixin : ConfigAbilityMixin, IHashable
	{
		public enum PositionType
		{
			Caster = 0,
			AttackTarget = 1,
			Target = 2
		}

		public MixinEffect CreationEffect;

		public float CreateEffectDelay;

		public MixinEffect DurationEffect;

		public MixinEffect DestroyEffect;

		public bool DestoryAfterSwitch;

		public PositionType TriggerPositionType = PositionType.Target;

		public DynamicFloat NoAttackTargetZOffset = DynamicFloat.ZERO;

		public DynamicFloat CreationZOffset = DynamicFloat.ZERO;

		public DynamicFloat CreationXOffset = DynamicFloat.ZERO;

		public DynamicFloat Radius;

		public MixinTargetting Targetting = MixinTargetting.Enemy;

		public DynamicFloat Duration;

		public bool Follow;

		public bool IncludeSelf;

		public bool TriggerOnAdded;

		public bool ApplyAttackerWitchTimeRatio;

		public string[] TargetModifiers = Miscs.EMPTY_STRINGS;

		public ConfigAbilityAction[] OnStartCasterActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnDestroyCasterActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnDestroyTargetActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (CreationEffect != null)
			{
				HashUtils.ContentHashOnto(CreationEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(CreationEffect.AudioPattern, ref lastHash);
			}
			HashUtils.ContentHashOnto(CreateEffectDelay, ref lastHash);
			if (DurationEffect != null)
			{
				HashUtils.ContentHashOnto(DurationEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(DurationEffect.AudioPattern, ref lastHash);
			}
			if (DestroyEffect != null)
			{
				HashUtils.ContentHashOnto(DestroyEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(DestroyEffect.AudioPattern, ref lastHash);
			}
			HashUtils.ContentHashOnto(DestoryAfterSwitch, ref lastHash);
			HashUtils.ContentHashOnto((int)TriggerPositionType, ref lastHash);
			if (NoAttackTargetZOffset != null)
			{
				HashUtils.ContentHashOnto(NoAttackTargetZOffset.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(NoAttackTargetZOffset.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(NoAttackTargetZOffset.dynamicKey, ref lastHash);
			}
			if (CreationZOffset != null)
			{
				HashUtils.ContentHashOnto(CreationZOffset.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(CreationZOffset.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(CreationZOffset.dynamicKey, ref lastHash);
			}
			if (CreationXOffset != null)
			{
				HashUtils.ContentHashOnto(CreationXOffset.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(CreationXOffset.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(CreationXOffset.dynamicKey, ref lastHash);
			}
			if (Radius != null)
			{
				HashUtils.ContentHashOnto(Radius.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Radius.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Radius.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto((int)Targetting, ref lastHash);
			if (Duration != null)
			{
				HashUtils.ContentHashOnto(Duration.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Duration.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Duration.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(Follow, ref lastHash);
			HashUtils.ContentHashOnto(IncludeSelf, ref lastHash);
			HashUtils.ContentHashOnto(TriggerOnAdded, ref lastHash);
			HashUtils.ContentHashOnto(ApplyAttackerWitchTimeRatio, ref lastHash);
			if (TargetModifiers != null)
			{
				string[] targetModifiers = TargetModifiers;
				foreach (string value in targetModifiers)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (OnStartCasterActions != null)
			{
				ConfigAbilityAction[] onStartCasterActions = OnStartCasterActions;
				foreach (ConfigAbilityAction configAbilityAction in onStartCasterActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (OnDestroyCasterActions != null)
			{
				ConfigAbilityAction[] onDestroyCasterActions = OnDestroyCasterActions;
				foreach (ConfigAbilityAction configAbilityAction2 in onDestroyCasterActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
					}
				}
			}
			if (OnDestroyTargetActions == null)
			{
				return;
			}
			ConfigAbilityAction[] onDestroyTargetActions = OnDestroyTargetActions;
			foreach (ConfigAbilityAction configAbilityAction3 in onDestroyTargetActions)
			{
				if (configAbilityAction3 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction3, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityTriggerFieldExMixin(instancedAbility, instancedModifier, this);
		}
	}
}
