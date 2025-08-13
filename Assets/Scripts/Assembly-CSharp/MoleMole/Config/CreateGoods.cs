namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CreateGoods : ConfigAbilityAction, IHashable
	{
		public string GoodType;

		public string GoodAbility;

		public DynamicFloat GoodArgument;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(GoodType, ref lastHash);
			HashUtils.ContentHashOnto(GoodAbility, ref lastHash);
			if (GoodArgument != null)
			{
				HashUtils.ContentHashOnto(GoodArgument.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(GoodArgument.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(GoodArgument.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto((int)Target, ref lastHash);
			if (TargetOption != null && TargetOption.Range != null)
			{
				HashUtils.ContentHashOnto(TargetOption.Range.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(TargetOption.Range.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(TargetOption.Range.dynamicKey, ref lastHash);
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

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.CreateGoodsHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
