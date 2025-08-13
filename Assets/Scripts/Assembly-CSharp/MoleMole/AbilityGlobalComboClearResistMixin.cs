using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityGlobalComboClearResistMixin : BaseAbilityMixin
	{
		private enum State
		{
			Effect = 0,
			Resuming = 1
		}

		private GlobalComboClearResistMixin config;

		private EntityTimer _resumeTimer;

		private LevelActor _levelActor;

		private State _state;

		public AbilityGlobalComboClearResistMixin(ActorAbility instancedAbility, ActorModifier instancedModifer, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifer, config)
		{
			this.config = (GlobalComboClearResistMixin)config;
			_resumeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.ResumeTimeSpan));
			_levelActor = Singleton<LevelManager>.Instance.levelActor;
		}

		public override void OnAdded()
		{
			_state = State.Effect;
			LevelActor levelActor = _levelActor;
			levelActor.comboTimeUPCallback = (Action)Delegate.Combine(levelActor.comboTimeUPCallback, new Action(OnComboTimeUp));
			_resumeTimer.SetActive(false);
		}

		public override void OnRemoved()
		{
			LevelActor levelActor = _levelActor;
			levelActor.comboTimeUPCallback = (Action)Delegate.Remove(levelActor.comboTimeUPCallback, new Action(OnComboTimeUp));
			_resumeTimer.SetActive(false);
		}

		public override void Core()
		{
			if ((bool)actor.isAlive)
			{
				_resumeTimer.Core(1f);
				State state = _state;
				if (state != State.Effect && state == State.Resuming && _resumeTimer.isTimeUp)
				{
					_resumeTimer.timespan = instancedAbility.Evaluate(config.ResumeTimeSpan);
					_resumeTimer.Reset(false);
					_state = State.Effect;
					LevelActor levelActor = _levelActor;
					levelActor.comboTimeUPCallback = (Action)Delegate.Combine(levelActor.comboTimeUPCallback, new Action(OnComboTimeUp));
				}
			}
		}

		private void OnComboTimeUp()
		{
			if (_state == State.Effect)
			{
				_state = State.Resuming;
				_levelActor.ResetComboTimer();
				LevelActor levelActor = _levelActor;
				levelActor.comboTimeUPCallback = (Action)Delegate.Remove(levelActor.comboTimeUPCallback, new Action(OnComboTimeUp));
				_resumeTimer.timespan = instancedAbility.Evaluate(config.ResumeTimeSpan);
				_resumeTimer.SetActive(true);
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ResitComboClear));
			}
		}
	}
}
