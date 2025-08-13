namespace MoleMole.Config
{
	public class ModifyDamageByAttackeeMixin : ConfigAbilityMixin
	{
		public DynamicFloat AddedDamageTakeRatio = DynamicFloat.ZERO;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityModifyDamageByAttackeeMixin(instancedAbility, instancedModifier, this);
		}
	}
}
