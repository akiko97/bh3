namespace MoleMole.Config
{
	public class DebugMixin : ConfigAbilityMixin
	{
		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityDebugMixin(instancedAbility, instancedModifier, this);
		}
	}
}
