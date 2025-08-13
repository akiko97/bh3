namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class Predicated : BaseUtilityAction, IHashable
	{
		public ConfigAbilityPredicate[] TargetPredicates = ConfigAbilityPredicate.EMPTY;

		public ConfigAbilityAction[] SuccessActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] FailActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (TargetPredicates != null)
			{
				ConfigAbilityPredicate[] targetPredicates = TargetPredicates;
				foreach (ConfigAbilityPredicate configAbilityPredicate in targetPredicates)
				{
					if (configAbilityPredicate is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityPredicate, ref lastHash);
					}
				}
			}
			if (SuccessActions != null)
			{
				ConfigAbilityAction[] successActions = SuccessActions;
				foreach (ConfigAbilityAction configAbilityAction in successActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (FailActions != null)
			{
				ConfigAbilityAction[] failActions = FailActions;
				foreach (ConfigAbilityAction configAbilityAction2 in failActions)
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
			foreach (ConfigAbilityPredicate configAbilityPredicate2 in predicates)
			{
				if (configAbilityPredicate2 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityPredicate2, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[2][] { SuccessActions, FailActions };
		}

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.PredicatedHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		public override MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return mpAbilityPlugin.Predicated_AuthorityHandler;
		}

		public override MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return MPActorAbilityPlugin.STUB_RemoteMute;
		}
	}
}
