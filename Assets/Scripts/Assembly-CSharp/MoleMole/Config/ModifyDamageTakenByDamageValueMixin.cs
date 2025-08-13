namespace MoleMole.Config
{
	public class ModifyDamageTakenByDamageValueMixin : ModifyDamageTakenMixin
	{
		public enum LogicType
		{
			MoreThan = 0,
			LessThanOrEqual = 1
		}

		public LogicType CompareType;

		public DynamicFloat ByDamageValue;

		public DynamicFloat ReplaceDamageValue;

		public bool UseReplaceAniDamageRatio;

		public float ReplaceAniDamageRatio;

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityModifyDamageTakenByDamageValueMixin(instancedAbility, instancedModifier, this);
		}
	}
}
