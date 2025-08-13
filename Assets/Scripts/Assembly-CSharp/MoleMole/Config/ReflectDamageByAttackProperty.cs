namespace MoleMole.Config
{
	public class ReflectDamageByAttackProperty : DamageByAttackProperty
	{
		public DynamicFloat ReflectRatio = DynamicFloat.ONE;

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.ReflectDamageByAttackPropertyHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
