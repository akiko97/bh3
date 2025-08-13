namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class OnKillEnemyMixin : ConfigAbilityMixin, IHashable
	{
		public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (Actions == null)
			{
				return;
			}
			ConfigAbilityAction[] actions = Actions;
			foreach (ConfigAbilityAction configAbilityAction in actions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityOnKillEnemyMixin(instancedAbility, instancedModifier, this);
		}
	}
}
