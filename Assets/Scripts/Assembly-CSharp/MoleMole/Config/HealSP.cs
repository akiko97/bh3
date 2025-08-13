namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class HealSP : ConfigAbilityAction, IHashable
	{
		public DynamicFloat Amount;

		public bool MuteHealEffect;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Amount != null)
			{
				HashUtils.ContentHashOnto(Amount.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Amount.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Amount.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(MuteHealEffect, ref lastHash);
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
			abilityPlugin.HealSPHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		public override MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return mpAbilityPlugin.HealSP_AuthorityHandler;
		}

		public override MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return mpAbilityPlugin.HealSP_RemoteHandler;
		}
	}
}
