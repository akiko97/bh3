namespace MoleMole.Config
{
	public class ActCameraExposure : ConfigAbilityAction
	{
		public float MaxExposure;

		public float ExposureTime;

		public float KeepTime;

		public float RecoverTime;

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.CameraExposureHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
