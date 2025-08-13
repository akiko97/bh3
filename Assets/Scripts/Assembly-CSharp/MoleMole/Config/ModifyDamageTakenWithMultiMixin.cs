namespace MoleMole.Config
{
	public class ModifyDamageTakenWithMultiMixin : ModifyDamageTakenMixin
	{
		public enum DamageMultipleType
		{
			ByTargetDistance = 0
		}

		public DamageMultipleType MultipleType;

		public float BaseMultiple;

		public DynamicFloat MaxMultiple = DynamicFloat.ZERO;

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityModifyDamageTakenWithMultiMixin(instancedAbility, instancedModifier, this);
		}
	}
}
