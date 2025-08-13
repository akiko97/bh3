namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByAvatarHasChargesLeft : ConfigAbilityPredicate, IHashable
	{
		public string CDSkillID;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(CDSkillID, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByAvatarHasChargesLeftHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
