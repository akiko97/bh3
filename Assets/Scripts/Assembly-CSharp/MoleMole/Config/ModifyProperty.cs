namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ModifyProperty : ConfigAbilityAction, IHashable
	{
		public string Property;

		public DynamicFloat Delta;

		public DynamicFloat Min;

		public DynamicFloat Max;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(Property, ref lastHash);
			if (Delta != null)
			{
				HashUtils.ContentHashOnto(Delta.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Delta.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Delta.dynamicKey, ref lastHash);
			}
			if (Min != null)
			{
				HashUtils.ContentHashOnto(Min.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Min.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Min.dynamicKey, ref lastHash);
			}
			if (Max != null)
			{
				HashUtils.ContentHashOnto(Max.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Max.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Max.dynamicKey, ref lastHash);
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
			abilityPlugin.ModifyPropertyHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
		{
			output = string.Format("{0} 对 {1} 更改属性 {2}:{3}", Miscs.GetDebugActorName(instancedAbility.caster), Miscs.GetDebugActorName(target), Property, instancedAbility.Evaluate(Delta));
			return true;
		}
	}
}
