namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterDodgeTeleportMixin : DodgeTeleportMixin, IHashable
	{
		public new void ObjectContentHashOnto(ref int lastHash)
		{
			if (TeleportSkillIDs != null)
			{
				string[] teleportSkillIDs = TeleportSkillIDs;
				foreach (string value in teleportSkillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(CanHitTrigger, ref lastHash);
			HashUtils.ContentHashOnto(Distance, ref lastHash);
			HashUtils.ContentHashOnto(TeleportTime, ref lastHash);
			HashUtils.ContentHashOnto((int)DirectionMode, ref lastHash);
			if (Angle != null)
			{
				HashUtils.ContentHashOnto(Angle.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Angle.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Angle.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(SpawnPoint, ref lastHash);
			if (CDTime != null)
			{
				HashUtils.ContentHashOnto(CDTime.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(CDTime.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(CDTime.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(NeedFade, ref lastHash);
			if (TeleportActions != null)
			{
				ConfigAbilityAction[] teleportActions = TeleportActions;
				foreach (ConfigAbilityAction configAbilityAction in teleportActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
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

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterDodgeTeleportMixin(instancedAbility, instancedModifier, this);
		}
	}
}
