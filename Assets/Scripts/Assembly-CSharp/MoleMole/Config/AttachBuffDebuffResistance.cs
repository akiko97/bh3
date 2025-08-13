namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachBuffDebuffResistance : ConfigAbilityAction, IHashable
	{
		public AbilityState[] ResistanceBuffDebuffs;

		public float ResistanceRatio;

		public float ResistanceDurationRatio;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (ResistanceBuffDebuffs != null)
			{
				AbilityState[] resistanceBuffDebuffs = ResistanceBuffDebuffs;
				foreach (AbilityState value in resistanceBuffDebuffs)
				{
					HashUtils.ContentHashOnto((int)value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(ResistanceRatio, ref lastHash);
			HashUtils.ContentHashOnto(ResistanceDurationRatio, ref lastHash);
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
			abilityPlugin.AttachBuffDebufResistanceHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
