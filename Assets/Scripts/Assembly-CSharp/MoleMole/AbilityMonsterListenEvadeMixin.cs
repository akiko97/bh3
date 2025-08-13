using MoleMole.Config;

namespace MoleMole
{
	public class AbilityMonsterListenEvadeMixin : BaseAbilityMixin
	{
		private MonsterListenEvadeMixin config;

		public AbilityMonsterListenEvadeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterListenEvadeMixin)config;
			Singleton<EventManager>.Instance.RegisterEventListener<EvtEvadeSuccess>(actor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtEvadeSuccess)
			{
				return OnEvadeSuccess((EvtEvadeSuccess)evt);
			}
			return false;
		}

		public override void OnAdded()
		{
		}

		public override void OnRemoved()
		{
		}

		private bool OnEvadeSuccess(EvtEvadeSuccess evt)
		{
			if (evt.attackerID == entity.GetRuntimeID())
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.BeEvadeSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt);
			}
			return true;
		}
	}
}
