namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DefendModeMonsterMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat TestNumber = DynamicFloat.ZERO;

		public DynamicFloat MaxHatred = DynamicFloat.ONE;

		public DynamicFloat HatredAddRateByDamage = new DynamicFloat
		{
			fixedValue = 0.2f
		};

		public DynamicFloat HatredAddThreholdRatioByDamage = new DynamicFloat
		{
			fixedValue = 0.05f
		};

		public DynamicFloat HatredDecreaseInterval = DynamicFloat.ONE;

		public DynamicFloat HatredDecreateRateByInterval = new DynamicFloat
		{
			fixedValue = 0.05f
		};

		public float[] hatredAIAreaSections = new float[1] { 0.5f };

		public int[] hatredAIValues = new int[2] { 2, 3 };

		public int DefaultAIValue = 2;

		public DynamicFloat MinAISwitchDuration = new DynamicFloat
		{
			fixedValue = 0.5f
		};

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (TestNumber != null)
			{
				HashUtils.ContentHashOnto(TestNumber.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(TestNumber.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(TestNumber.dynamicKey, ref lastHash);
			}
			if (MaxHatred != null)
			{
				HashUtils.ContentHashOnto(MaxHatred.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(MaxHatred.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(MaxHatred.dynamicKey, ref lastHash);
			}
			if (HatredAddRateByDamage != null)
			{
				HashUtils.ContentHashOnto(HatredAddRateByDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(HatredAddRateByDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(HatredAddRateByDamage.dynamicKey, ref lastHash);
			}
			if (HatredAddThreholdRatioByDamage != null)
			{
				HashUtils.ContentHashOnto(HatredAddThreholdRatioByDamage.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(HatredAddThreholdRatioByDamage.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(HatredAddThreholdRatioByDamage.dynamicKey, ref lastHash);
			}
			if (HatredDecreaseInterval != null)
			{
				HashUtils.ContentHashOnto(HatredDecreaseInterval.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(HatredDecreaseInterval.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(HatredDecreaseInterval.dynamicKey, ref lastHash);
			}
			if (HatredDecreateRateByInterval != null)
			{
				HashUtils.ContentHashOnto(HatredDecreateRateByInterval.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(HatredDecreateRateByInterval.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(HatredDecreateRateByInterval.dynamicKey, ref lastHash);
			}
			if (hatredAIAreaSections != null)
			{
				float[] array = hatredAIAreaSections;
				foreach (float value in array)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (hatredAIValues != null)
			{
				int[] array2 = hatredAIValues;
				foreach (int value2 in array2)
				{
					HashUtils.ContentHashOnto(value2, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(DefaultAIValue, ref lastHash);
			if (MinAISwitchDuration != null)
			{
				HashUtils.ContentHashOnto(MinAISwitchDuration.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(MinAISwitchDuration.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(MinAISwitchDuration.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityDefendModeMonsterMixin(instancedAbility, instancedModifier, this);
		}
	}
}
