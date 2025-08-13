namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByAttackTargetAnimState : ConfigAbilityPredicate, IHashable
	{
		public enum AnimState
		{
			Throw = 0
		}

		public AnimState State;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)State, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByAttackTargetAnimStateHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
