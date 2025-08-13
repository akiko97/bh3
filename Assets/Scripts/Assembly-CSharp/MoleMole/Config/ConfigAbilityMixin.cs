using System;

namespace MoleMole.Config
{
	[CheckForHashable]
	public abstract class ConfigAbilityMixin : BaseActionContainer
	{
		public static ConfigAbilityMixin[] EMPTY = new ConfigAbilityMixin[0];

		[NonSerialized]
		public bool isUnique;

		public abstract BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier);

		public virtual BaseAbilityMixin MPCreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return null;
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return ConfigAbilityAction.EMPTY_SUBS;
		}
	}
}
