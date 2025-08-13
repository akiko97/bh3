namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ByAttackAnimEventID : ConfigAbilityPredicate, IHashable
	{
		public string[] AnimEventIDs = Miscs.EMPTY_STRINGS;

		public bool ByAnyEventID;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (AnimEventIDs != null)
			{
				string[] animEventIDs = AnimEventIDs;
				foreach (string value in animEventIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(ByAnyEventID, ref lastHash);
		}

		public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			return abilityPlugin.ByAttackAnimEventIDHandler(this, instancedAbility, instancedModifier, target, evt);
		}
	}
}
