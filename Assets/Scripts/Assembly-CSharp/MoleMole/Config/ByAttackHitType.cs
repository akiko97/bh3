namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByAttackHitType : ConfigAbilityPredicate, IHashable
	{
		public AttackResult.ActorHitType HitType;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)HitType, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByAttackHitTypeHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
