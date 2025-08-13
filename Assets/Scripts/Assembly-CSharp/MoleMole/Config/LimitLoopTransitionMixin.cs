namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class LimitLoopTransitionMixin : ConfigAbilityMixin, IHashable
	{
		public string AllowLoopBoolID;

		public string SkillID;

		public DynamicInt LoopLimitCount;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(AllowLoopBoolID, ref lastHash);
			HashUtils.ContentHashOnto(SkillID, ref lastHash);
			if (LoopLimitCount != null)
			{
				HashUtils.ContentHashOnto(LoopLimitCount.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(LoopLimitCount.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(LoopLimitCount.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityLimitLoopTransitionMixin(instancedAbility, instancedModifier, this);
		}
	}
}
