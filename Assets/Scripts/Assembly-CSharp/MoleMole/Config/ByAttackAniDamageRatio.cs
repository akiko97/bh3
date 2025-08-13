namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByAttackAniDamageRatio : ConfigAbilityPredicate, IHashable
	{
		public enum LogicType
		{
			MoreThan = 0,
			LessThan = 1
		}

		public float AniDamageRatio;

		public LogicType CompareType;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(AniDamageRatio, ref lastHash);
			HashUtils.ContentHashOnto((int)CompareType, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByAttackAniDamageRatioHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
