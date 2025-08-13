namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class Retargeted : BaseUtilityAction, IHashable
	{
		public AbilityTargetting Retarget;

		public TargettingOption RetargetOption;

		public bool RandomedTarget;

		public bool IgnoreSelf;

		public ConfigAbilityAction[] RetargetedActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)Retarget, ref lastHash);
			if (RetargetOption != null && RetargetOption.Range != null)
			{
				HashUtils.ContentHashOnto(RetargetOption.Range.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(RetargetOption.Range.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(RetargetOption.Range.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(RandomedTarget, ref lastHash);
			HashUtils.ContentHashOnto(IgnoreSelf, ref lastHash);
			if (RetargetedActions != null)
			{
				ConfigAbilityAction[] retargetedActions = RetargetedActions;
				foreach (ConfigAbilityAction configAbilityAction in retargetedActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
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
			return new ConfigAbilityAction[1][] { RetargetedActions };
		}

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.RetargetedHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
