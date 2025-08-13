using MoleMole.Config;

namespace MoleMole
{
	public class AbilityDebugMixin : BaseAbilityMixin
	{
		private bool _hasAdded;

		public AbilityDebugMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			_hasAdded = false;
		}

		public override void OnAdded()
		{
			if (!_hasAdded)
			{
				_hasAdded = true;
			}
		}
	}
}
