namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterSwordDodgeMixin : ConfigAbilityMixin, IHashable
	{
		public float DodgeRatio;

		public float NoDodgeAttackRatio;

		public ConfigAbilityAction[] DodgeActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(DodgeRatio, ref lastHash);
			HashUtils.ContentHashOnto(NoDodgeAttackRatio, ref lastHash);
			if (DodgeActions == null)
			{
				return;
			}
			ConfigAbilityAction[] dodgeActions = DodgeActions;
			foreach (ConfigAbilityAction configAbilityAction in dodgeActions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterSwordDodgeMixin(instancedAbility, instancedModifier, this);
		}
	}
}
