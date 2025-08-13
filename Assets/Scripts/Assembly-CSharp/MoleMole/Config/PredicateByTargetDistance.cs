namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class PredicateByTargetDistance : ConfigAbilityAction, IHashable
	{
		public float Distance;

		public ConfigAbilityAction[] InActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OutActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(Distance, ref lastHash);
			if (InActions != null)
			{
				ConfigAbilityAction[] inActions = InActions;
				foreach (ConfigAbilityAction configAbilityAction in inActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (OutActions != null)
			{
				ConfigAbilityAction[] outActions = OutActions;
				foreach (ConfigAbilityAction configAbilityAction2 in outActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
					}
				}
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

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[2][] { InActions, OutActions };
		}

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.PredicateByTargetDistanceHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
