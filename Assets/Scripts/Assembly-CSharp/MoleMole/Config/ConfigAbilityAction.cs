namespace MoleMole.Config
{
	[CheckForHashable]
	public abstract class ConfigAbilityAction : BaseActionContainer, IOnLoaded
	{
		public static ConfigAbilityAction[] EMPTY = new ConfigAbilityAction[0];

		public static ConfigAbilityAction[][] EMPTY_SUBS = new ConfigAbilityAction[0][];

		public AbilityTargetting Target;

		public TargettingOption TargetOption;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

		public virtual void OnLoaded()
		{
		}

		public abstract void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt);

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return EMPTY_SUBS;
		}

		public virtual bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
		{
			return false;
		}

		public virtual MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return MPActorAbilityPlugin.STUB_AuthorityMute;
		}

		public virtual MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return MPActorAbilityPlugin.STUB_RemoteMute;
		}
	}
}
