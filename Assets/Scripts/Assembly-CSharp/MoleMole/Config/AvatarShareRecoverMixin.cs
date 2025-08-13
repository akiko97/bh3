namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarShareRecoverMixin : ConfigAbilityMixin, IHashable
	{
		public bool ShareSP;

		public DynamicFloat ShareSPRatio = DynamicFloat.ONE;

		public bool ShareHP;

		public DynamicFloat ShareHPRatio = DynamicFloat.ONE;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ShareSP, ref lastHash);
			if (ShareSPRatio != null)
			{
				HashUtils.ContentHashOnto(ShareSPRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ShareSPRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ShareSPRatio.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(ShareHP, ref lastHash);
			if (ShareHPRatio != null)
			{
				HashUtils.ContentHashOnto(ShareHPRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ShareHPRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ShareHPRatio.dynamicKey, ref lastHash);
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
			return new AbilityAvatarShareRecoverMixin(instancedAbility, instancedModifier, this);
		}
	}
}
