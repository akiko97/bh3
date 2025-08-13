namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class SetAIParamBool : ConfigAbilityAction, IHashable
	{
		public DynamicString Param;

		public bool Value;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Param != null)
			{
				HashUtils.ContentHashOnto(Param.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Param.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Param.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(Value, ref lastHash);
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
			abilityPlugin.SetAIParamBoolHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
