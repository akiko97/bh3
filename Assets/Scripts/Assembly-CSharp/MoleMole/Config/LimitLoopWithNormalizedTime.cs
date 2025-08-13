namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class LimitLoopWithNormalizedTime : ConfigAbilityMixin, IHashable
	{
		public string AllowLoopBoolID;

		public string SkillID;

		public float NormalizedTimeStart;

		public float NormalizedTimeStop;

		public DynamicInt LoopLimitCount;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(AllowLoopBoolID, ref lastHash);
			HashUtils.ContentHashOnto(SkillID, ref lastHash);
			HashUtils.ContentHashOnto(NormalizedTimeStart, ref lastHash);
			HashUtils.ContentHashOnto(NormalizedTimeStop, ref lastHash);
			if (LoopLimitCount != null)
			{
				HashUtils.ContentHashOnto(LoopLimitCount.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(LoopLimitCount.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(LoopLimitCount.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityLimitLoopWithNormalizedTimeMixin(instancedAbility, instancedModifier, this);
		}
	}
}
