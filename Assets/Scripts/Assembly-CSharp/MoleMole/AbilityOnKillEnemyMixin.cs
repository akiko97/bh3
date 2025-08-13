using MoleMole.Config;

namespace MoleMole
{
	public class AbilityOnKillEnemyMixin : BaseAbilityMixin
	{
		private OnKillEnemyMixin config;

		public AbilityOnKillEnemyMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (OnKillEnemyMixin)config;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(actor.runtimeID);
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(actor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return OnKilled((EvtKilled)evt);
			}
			return false;
		}

		private bool OnKilled(EvtKilled evt)
		{
			if (evt.killerID == actor.runtimeID)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.Actions, instancedAbility, instancedModifier, null, null);
			}
			return true;
		}
	}
}
