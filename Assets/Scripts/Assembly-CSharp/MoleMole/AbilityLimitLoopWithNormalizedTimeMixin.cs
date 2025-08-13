using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityLimitLoopWithNormalizedTimeMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			WaitingForLoop = 1,
			Looping = 2,
			Exiting = 3
		}

		private State _state;

		public LimitLoopWithNormalizedTime config;

		private int _countLeft;

		public AbilityLimitLoopWithNormalizedTimeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (LimitLoopWithNormalizedTime)config;
		}

		public override void OnAdded()
		{
			_countLeft = instancedAbility.Evaluate(config.LoopLimitCount);
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, true);
			_state = State.Idle;
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			entity.RemovePersistentAnimatorBool(config.AllowLoopBoolID);
		}

		public override void Core()
		{
			if (_state == State.WaitingForLoop)
			{
				float currentNormalizedTime = entity.GetCurrentNormalizedTime();
				if (currentNormalizedTime > config.NormalizedTimeStart)
				{
					_state = State.Looping;
					if (_countLeft <= 0)
					{
						entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, false);
					}
				}
				else if (currentNormalizedTime > config.NormalizedTimeStop)
				{
					_state = State.Exiting;
					entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, true);
				}
			}
			else if (_state == State.Looping && entity.GetCurrentNormalizedTime() > config.NormalizedTimeStop)
			{
				_state = State.Exiting;
				entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, true);
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (_state == State.Idle)
			{
				if (to == config.SkillID)
				{
					_state = State.WaitingForLoop;
					_countLeft--;
					entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, true);
				}
			}
			else if (to == config.SkillID)
			{
				_state = State.WaitingForLoop;
				_countLeft--;
				entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, true);
			}
			else if (to != config.SkillID)
			{
				_state = State.Idle;
				_countLeft = instancedAbility.Evaluate(config.LoopLimitCount);
				entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, true);
			}
		}
	}
}
