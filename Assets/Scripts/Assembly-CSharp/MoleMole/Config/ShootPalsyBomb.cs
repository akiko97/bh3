namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ShootPalsyBomb : ConfigAbilityAction, IHashable
	{
		public DynamicString PropName;

		public float BombSpeed;

		public string BombAttackID;

		public string AttachPoint;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (PropName != null)
			{
				HashUtils.ContentHashOnto(PropName.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PropName.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PropName.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(BombSpeed, ref lastHash);
			HashUtils.ContentHashOnto(BombAttackID, ref lastHash);
			HashUtils.ContentHashOnto(AttachPoint, ref lastHash);
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
			abilityPlugin.ShootPalsyBombHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
