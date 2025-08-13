using MoleMole.Config;

namespace MoleMole
{
	public class AbilityKeepAttackSameTargetMixin : BaseAbilityMixin
	{
		private KeepAttackSameTargetMixin config;

		private uint _lastTargetID;

		private EntityTimer _fadeTimer;

		public AbilityKeepAttackSameTargetMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (KeepAttackSameTargetMixin)config;
			_fadeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.TargetFadeWindow));
		}

		public override void OnAdded()
		{
			_fadeTimer.Reset(false);
			_lastTargetID = 0u;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtHittingOther)
			{
				return OnHittingOther((EvtHittingOther)evt);
			}
			return false;
		}

		public override void Core()
		{
			_fadeTimer.Core(1f);
			if (_fadeTimer.isTimeUp)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.OnTargetFadeOrChanged, instancedAbility, instancedModifier, null, null);
				_fadeTimer.Reset(false);
			}
		}

		private bool OnHittingOther(EvtHittingOther evt)
		{
			if (string.IsNullOrEmpty(evt.animEventID))
			{
				return true;
			}
			if (evt.toID != _lastTargetID)
			{
				if (_lastTargetID != 0)
				{
					actor.abilityPlugin.HandleActionTargetDispatch(config.OnTargetFadeOrChanged, instancedAbility, instancedModifier, null, null);
				}
				_lastTargetID = evt.toID;
			}
			else
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.OnAttackSameTarget, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt);
				_fadeTimer.Reset(true);
			}
			return true;
		}
	}
}
