namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ReplaceAttackData : ConfigAbilityAction, IHashable
	{
		public int FrameHalt;

		public bool ReplaceFrameHalt;

		public float AttackerAniDamageRatio;

		public bool ReplaceAttackerAniDamageRatio;

		public float AddAttackeeAniDefenceRatio;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(FrameHalt, ref lastHash);
			HashUtils.ContentHashOnto(ReplaceFrameHalt, ref lastHash);
			HashUtils.ContentHashOnto(AttackerAniDamageRatio, ref lastHash);
			HashUtils.ContentHashOnto(ReplaceAttackerAniDamageRatio, ref lastHash);
			HashUtils.ContentHashOnto(AddAttackeeAniDefenceRatio, ref lastHash);
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
			abilityPlugin.ReplaceAttackDataHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
