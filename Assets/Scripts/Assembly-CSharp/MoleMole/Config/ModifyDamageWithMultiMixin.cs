namespace MoleMole.Config
{
	public class ModifyDamageWithMultiMixin : ModifyDamageMixin
	{
		public enum DamageMultipleType
		{
			BySelfCurrentSPAmount = 0,
			BySelfMaxSPAmount = 1,
			ByTargetAbilityState = 2,
			ByTargetDistance = 3,
			ByLevelCurrentCombo = 4
		}

		public DamageMultipleType MultipleType;

		public float BaseMultiple;

		public DynamicFloat MaxMultiple = DynamicFloat.ZERO;

		public bool ClearAllSP = true;

		public AbilityState TargetAbilityState;

		public MixinTargetting Targetting;

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityModifiyDamageWithMultiMixin(instancedAbility, instancedModifier, this);
		}
	}
}
