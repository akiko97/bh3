namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class SetSelfAttackTarget : ConfigAbilityAction, IHashable
	{
		public bool SteerToTargetImmediately;

		public SetSelfAttackTarget()
		{
			Target = AbilityTargetting.Target;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(SteerToTargetImmediately, ref lastHash);
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
			abilityPlugin.SetSelfAttackTargetHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		public override void OnLoaded()
		{
		}

		public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
		{
			output = string.Format("{0} 通过技能设置攻击目标为 {1}", Miscs.GetDebugActorName(instancedAbility.caster), Miscs.GetDebugActorName(target));
			return true;
		}
	}
}
