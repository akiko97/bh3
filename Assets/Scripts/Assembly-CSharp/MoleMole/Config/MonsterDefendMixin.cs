namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterDefendMixin : ConfigAbilityMixin, IHashable
	{
		public string[] DefendSkillIDs;

		public string DefendTriggerID;

		public DynamicFloat DefendActionChance = DynamicFloat.ZERO;

		public bool DefendMelee = true;

		public bool DefendRange = true;

		public DynamicFloat BreakDefendAniDamageRatio = new DynamicFloat
		{
			fixedValue = 2f
		};

		public DynamicFloat DefendAngle = new DynamicFloat
		{
			fixedValue = 180f
		};

		public DynamicFloat DefendActionRange = DynamicFloat.ONE;

		public MixinEffect DefendEffect;

		public MixinEffect DefendSuccessEffect;

		public DynamicFloat DefendActionCD = DynamicFloat.ZERO;

		public DynamicFloat InDefendMaxTime = DynamicFloat.ZERO;

		public bool AllowLayerLightControl;

		public int ShieldLightLayer = 3;

		public float ShieldLightMax = 1f;

		public float ShieldLightMin;

		public string[] ResetCDSkillIDs;

		public ConfigAbilityAction[] DefendStartActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] DefendActionReadyActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] DefendSuccessActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (DefendSkillIDs != null)
			{
				string[] defendSkillIDs = DefendSkillIDs;
				foreach (string value in defendSkillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(DefendTriggerID, ref lastHash);
			if (DefendActionChance != null)
			{
				HashUtils.ContentHashOnto(DefendActionChance.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DefendActionChance.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DefendActionChance.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(DefendMelee, ref lastHash);
			HashUtils.ContentHashOnto(DefendRange, ref lastHash);
			if (BreakDefendAniDamageRatio != null)
			{
				HashUtils.ContentHashOnto(BreakDefendAniDamageRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(BreakDefendAniDamageRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(BreakDefendAniDamageRatio.dynamicKey, ref lastHash);
			}
			if (DefendAngle != null)
			{
				HashUtils.ContentHashOnto(DefendAngle.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DefendAngle.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DefendAngle.dynamicKey, ref lastHash);
			}
			if (DefendActionRange != null)
			{
				HashUtils.ContentHashOnto(DefendActionRange.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DefendActionRange.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DefendActionRange.dynamicKey, ref lastHash);
			}
			if (DefendEffect != null)
			{
				HashUtils.ContentHashOnto(DefendEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(DefendEffect.AudioPattern, ref lastHash);
			}
			if (DefendSuccessEffect != null)
			{
				HashUtils.ContentHashOnto(DefendSuccessEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(DefendSuccessEffect.AudioPattern, ref lastHash);
			}
			if (DefendActionCD != null)
			{
				HashUtils.ContentHashOnto(DefendActionCD.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DefendActionCD.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DefendActionCD.dynamicKey, ref lastHash);
			}
			if (InDefendMaxTime != null)
			{
				HashUtils.ContentHashOnto(InDefendMaxTime.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(InDefendMaxTime.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(InDefendMaxTime.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(AllowLayerLightControl, ref lastHash);
			HashUtils.ContentHashOnto(ShieldLightLayer, ref lastHash);
			HashUtils.ContentHashOnto(ShieldLightMax, ref lastHash);
			HashUtils.ContentHashOnto(ShieldLightMin, ref lastHash);
			if (ResetCDSkillIDs != null)
			{
				string[] resetCDSkillIDs = ResetCDSkillIDs;
				foreach (string value2 in resetCDSkillIDs)
				{
					HashUtils.ContentHashOnto(value2, ref lastHash);
				}
			}
			if (DefendStartActions != null)
			{
				ConfigAbilityAction[] defendStartActions = DefendStartActions;
				foreach (ConfigAbilityAction configAbilityAction in defendStartActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (DefendActionReadyActions != null)
			{
				ConfigAbilityAction[] defendActionReadyActions = DefendActionReadyActions;
				foreach (ConfigAbilityAction configAbilityAction2 in defendActionReadyActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
					}
				}
			}
			if (DefendSuccessActions != null)
			{
				ConfigAbilityAction[] defendSuccessActions = DefendSuccessActions;
				foreach (ConfigAbilityAction configAbilityAction3 in defendSuccessActions)
				{
					if (configAbilityAction3 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction3, ref lastHash);
					}
				}
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

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterDefendMixin(instancedAbility, instancedModifier, this);
		}
	}
}
