namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByAttackDataType : ConfigAbilityPredicate, IHashable
	{
		public enum AttackDataType
		{
			None = 0,
			Breakable = 1,
			EvadeDefendable = 2
		}

		public AttackDataType Type;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)Type, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByAttackDataTypeHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
