namespace MoleMole.Config
{
	public class ActCameraGlare : ConfigAbilityAction
	{
		public float TargetRate;

		public float GlareTime;

		public float KeepTime;

		public float RecoverTime;

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.CameraGlareHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
