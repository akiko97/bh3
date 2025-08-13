using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAvatarSwitchRoleMixin : BaseAbilityMixin
	{
		private AvatarSwitchRoleMixin config;

		public AbilityAvatarSwitchRoleMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarSwitchRoleMixin)config;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapInEnd>(actor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(actor.runtimeID);
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapInEnd>(actor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(actor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtAvatarSwapInEnd)
			{
				return OnSwitchInEnd((EvtAvatarSwapInEnd)evt);
			}
			if (evt is EvtAvatarSwapOutStart)
			{
				return OnSwitchOutStart((EvtAvatarSwapOutStart)evt);
			}
			return false;
		}

		private bool OnSwitchInEnd(EvtAvatarSwapInEnd evt)
		{
			if (actor.runtimeID == evt.targetID)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.SwitchInActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt);
			}
			return true;
		}

		private bool OnSwitchOutStart(EvtAvatarSwapOutStart evt)
		{
			if (actor.runtimeID == evt.targetID)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.SwitchOutActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt);
			}
			return true;
		}
	}
}
