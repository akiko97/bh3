using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityForceInterruptMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			Running = 1
		}

		private ForceInterruptMixin config;

		private State _state;

		private float _timer;

		public AbilityForceInterruptMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ForceInterruptMixin)config;
		}

		public override void OnAdded()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			_state = State.Idle;
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
		}

		public override void Core()
		{
			if (_state == State.Running)
			{
				_timer -= Time.deltaTime * entity.TimeScale;
				if (_timer <= 0f)
				{
					actor.abilityPlugin.HandleActionTargetDispatch(config.InterruptActions, instancedAbility, instancedModifier, null, null);
					_state = State.Idle;
				}
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < config.SkillIDs.Length; i++)
			{
				if (config.SkillIDs[i] == from)
				{
					flag = true;
				}
				if (config.SkillIDs[i] == to)
				{
					flag2 = true;
				}
			}
			if (!flag && flag2)
			{
				if (_state == State.Idle)
				{
					_state = State.Running;
					_timer = config.TimeThreshold;
				}
			}
			else if (flag && !flag2)
			{
				_state = State.Idle;
			}
		}
	}
}
