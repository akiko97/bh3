namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByAttackFromAnimEvent : ConfigAbilityPredicate, IHashable
	{
		public bool IsAnimEventAttack;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(IsAnimEventAttack, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByAttackIsAnimEventHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
