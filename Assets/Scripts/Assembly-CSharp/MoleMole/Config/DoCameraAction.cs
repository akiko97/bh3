namespace MoleMole.Config
{
	public class DoCameraAction : ConfigAbilityAction
	{
		public ConfigAvatarCameraAction CameraAction;

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.CameraActionHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
