namespace MoleMole.Config
{
	public class ShowLevelDisplayText : ConfigAbilityAction
	{
		public string Text;

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.ShowLevelDisplayTextHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
