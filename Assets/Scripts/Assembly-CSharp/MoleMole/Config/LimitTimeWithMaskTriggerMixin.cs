namespace MoleMole.Config
{
	public class LimitTimeWithMaskTriggerMixin : ConfigAbilityMixin
	{
		public float CountTime;

		public string MaskTriggerID;

		public float MaskDuration;

		public string SkillID;

		public DynamicInt EvadeLimitCount;

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityLimitWithMaskTriggerMixin(instancedAbility, instancedModifier, this);
		}
	}
}
