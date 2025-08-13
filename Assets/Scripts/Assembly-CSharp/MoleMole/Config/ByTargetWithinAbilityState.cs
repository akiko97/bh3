namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByTargetWithinAbilityState : ConfigAbilityPredicate, IHashable
	{
		public AbilityState TargetState;

		public AbilityState[] TargetStates;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)TargetState, ref lastHash);
			if (TargetStates != null)
			{
				AbilityState[] targetStates = TargetStates;
				foreach (AbilityState value in targetStates)
				{
					HashUtils.ContentHashOnto((int)value, ref lastHash);
				}
			}
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByTargetWithinAbilityStateHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
