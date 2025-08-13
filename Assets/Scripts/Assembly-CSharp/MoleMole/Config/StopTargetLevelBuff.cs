namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class StopTargetLevelBuff : ConfigAbilityAction, IHashable
	{
		public LevelBuffType LevelBuff;

		public bool stopOtherSide;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)LevelBuff, ref lastHash);
			HashUtils.ContentHashOnto(stopOtherSide, ref lastHash);
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
			abilityPlugin.StopTargetLevelBuffHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
		{
			output = string.Format("{0} 停止 LevelBuff {1}, 停止对面 side: {2}", Miscs.GetDebugActorName(instancedAbility.caster), LevelBuff, stopOtherSide);
			return true;
		}
	}
}
