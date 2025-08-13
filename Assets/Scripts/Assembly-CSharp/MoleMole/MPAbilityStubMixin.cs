using MoleMole.Config;

namespace MoleMole
{
	public class MPAbilityStubMixin : BaseAbilityMixin
	{
		public MPAbilityStubMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
		}
	}
}
