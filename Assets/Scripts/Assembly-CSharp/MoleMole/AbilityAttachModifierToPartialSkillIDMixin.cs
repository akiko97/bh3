using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAttachModifierToPartialSkillIDMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			InSkill = 1,
			InPartial = 2,
			OutPartial = 3
		}

		private AttachModifierToPartialSkillIDMixin config;

		private State _state;

		private ActorModifier _attachedModifier;

		private State _oldState;

		public AbilityAttachModifierToPartialSkillIDMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachModifierToPartialSkillIDMixin)config;
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

		private void Transit(State nextState)
		{
			if (nextState == State.InPartial)
			{
				_attachedModifier = actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierName);
			}
			else if (_state == State.InPartial)
			{
				actor.abilityPlugin.TryRemoveModifier(_attachedModifier);
				_attachedModifier = null;
			}
			_state = nextState;
		}

		public override void Core()
		{
			if (_oldState != _state)
			{
				_oldState = _state;
			}
			if (_state == State.InSkill)
			{
				float currentNormalizedTime = entity.GetCurrentNormalizedTime();
				if (currentNormalizedTime > config.NormalizedTimeStart)
				{
					Transit(State.InPartial);
				}
			}
			else if (_state == State.InPartial)
			{
				if (entity.GetCurrentNormalizedTime() > config.NormalizedTimeStop)
				{
					Transit(State.OutPartial);
				}
			}
			else if (_state != State.OutPartial)
			{
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (_state == State.Idle)
			{
				if (to == config.SkillID)
				{
					if (entity.GetCurrentNormalizedTime() > config.NormalizedTimeStart)
					{
						Transit(State.InPartial);
					}
					else
					{
						Transit(State.InSkill);
					}
				}
			}
			else if (_state == State.InSkill)
			{
				if (to == config.SkillID)
				{
					if (entity.GetCurrentNormalizedTime() > config.NormalizedTimeStart)
					{
						Transit(State.InPartial);
					}
				}
				else
				{
					Transit(State.Idle);
				}
			}
			else if (_state == State.InPartial)
			{
				if (to != config.SkillID)
				{
					Transit(State.Idle);
				}
			}
			else
			{
				if (_state != State.OutPartial)
				{
					return;
				}
				if (to != config.SkillID)
				{
					Transit(State.Idle);
				}
				else if (to == config.SkillID)
				{
					if (entity.GetCurrentNormalizedTime() > config.NormalizedTimeStart)
					{
						Transit(State.InPartial);
					}
					else
					{
						Transit(State.InSkill);
					}
				}
			}
		}
	}
}
