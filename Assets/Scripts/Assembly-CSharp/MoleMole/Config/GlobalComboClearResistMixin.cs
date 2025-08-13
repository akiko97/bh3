namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class GlobalComboClearResistMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat ResumeTimeSpan;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (ResumeTimeSpan != null)
			{
				HashUtils.ContentHashOnto(ResumeTimeSpan.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ResumeTimeSpan.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ResumeTimeSpan.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityGlobalComboClearResistMixin(instancedAbility, instancedModifier, this);
		}
	}
}
