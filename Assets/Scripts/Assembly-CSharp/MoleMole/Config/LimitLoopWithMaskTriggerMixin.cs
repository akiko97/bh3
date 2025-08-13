namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class LimitLoopWithMaskTriggerMixin : ConfigAbilityMixin, IHashable
	{
		public string MaskTriggerID;

		public float MaskDuration;

		public string SkillID;

		public DynamicInt LoopLimitCount;

		public bool UseOverCount;

		public float ResetOverCountTime;

		public int OverCountResetLoopLimitCount;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(MaskTriggerID, ref lastHash);
			HashUtils.ContentHashOnto(MaskDuration, ref lastHash);
			HashUtils.ContentHashOnto(SkillID, ref lastHash);
			if (LoopLimitCount != null)
			{
				HashUtils.ContentHashOnto(LoopLimitCount.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(LoopLimitCount.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(LoopLimitCount.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(UseOverCount, ref lastHash);
			HashUtils.ContentHashOnto(ResetOverCountTime, ref lastHash);
			HashUtils.ContentHashOnto(OverCountResetLoopLimitCount, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityLimitLoopWithMaskTriggerMixin(instancedAbility, instancedModifier, this);
		}
	}
}
