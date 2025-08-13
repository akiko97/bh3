namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterAreaLastingBuffMixin : ConfigAbilityMixin, IHashable
	{
		public enum AreaLastingHitBreakType
		{
			Normal = 0,
			ConvertAllHitsToLightHit = 1,
			BreakingHitCancels = 2
		}

		public DynamicFloat Radius = DynamicFloat.ONE;

		public DynamicFloat Duration = DynamicFloat.ZERO;

		public MixinTargetting Target = MixinTargetting.Allied;

		public string BuffDurationEndTrigger;

		public bool TriggerOnAdded;

		public AreaLastingHitBreakType HitBreakType;

		public bool IncludeSelf = true;

		public string SelfLastingModifierName;

		public string[] ModifierNames;

		public string BuffTimeRatioAnimatorParam;

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
			HashUtils.ContentHashOnto(BuffDurationEndTrigger, ref lastHash);
			HashUtils.ContentHashOnto(TriggerOnAdded, ref lastHash);
			HashUtils.ContentHashOnto((int)HitBreakType, ref lastHash);
			HashUtils.ContentHashOnto(IncludeSelf, ref lastHash);
			HashUtils.ContentHashOnto(SelfLastingModifierName, ref lastHash);
			if (ModifierNames != null)
			{
				string[] modifierNames = ModifierNames;
				foreach (string value in modifierNames)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(BuffTimeRatioAnimatorParam, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterAreaLastingBuffMixin(instancedAbility, instancedModifier, this);
		}
	}
}
