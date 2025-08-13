namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterListenEvadeMixin : ConfigAbilityMixin, IHashable
	{
		public ConfigAbilityAction[] BeEvadeSuccessActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (BeEvadeSuccessActions == null)
			{
				return;
			}
			ConfigAbilityAction[] beEvadeSuccessActions = BeEvadeSuccessActions;
			foreach (ConfigAbilityAction configAbilityAction in beEvadeSuccessActions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterListenEvadeMixin(instancedAbility, instancedModifier, this);
		}
	}
}
