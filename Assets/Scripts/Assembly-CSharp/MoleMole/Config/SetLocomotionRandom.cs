namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class SetLocomotionRandom : ConfigAbilityAction, IHashable
	{
		public DynamicInt Range;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Range != null)
			{
				HashUtils.ContentHashOnto(Range.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Range.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Range.dynamicKey, ref lastHash);
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
			abilityPlugin.SetLocomotionRandomHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
