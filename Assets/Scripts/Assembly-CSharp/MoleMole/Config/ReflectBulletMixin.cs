namespace MoleMole.Config
{
	public class ReflectBulletMixin : ConfigAbilityMixin
	{
		public bool IsReflectToLauncher;

		public DynamicFloat DamageRatio;

		public bool ResetAliveDuration;

		public float NewAliveDuration;

		public float Angle = 30f;

		public ConfigAbilityAction[] ReflectSuccessActions = ConfigAbilityAction.EMPTY;

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityReflectBulletMixin(instancedAbility, instancedModifier, this);
		}
	}
}
