using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityDynamicDistanceMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			Wait = 1,
			Runing = 2,
			End = 3
		}

		private DynamicDistanceMixin config;

		private BaseMonoAbilityEntity _entity;

		private State _state;

		private int _animatorMoveStackIx;

		public AbilityDynamicDistanceMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DynamicDistanceMixin)config;
			_entity = entity;
		}

		public override void OnAdded()
		{
			_state = State.Idle;
			if (config.SkillID != null)
			{
				BaseMonoAbilityEntity baseMonoAbilityEntity = _entity;
				baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			}
			else
			{
				SetDynamicSpeed();
			}
		}

		public override void OnRemoved()
		{
			if (config.SkillID != null)
			{
				BaseMonoAbilityEntity baseMonoAbilityEntity = _entity;
				baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			}
			ResetDynamicSpeed();
		}

		public override void Core()
		{
			if (_state == State.Wait)
			{
				if (GetCurrentNormalizedTime() >= config.NormalizedTimeStart)
				{
					SetDynamicSpeed();
				}
			}
			else if (_state == State.Runing && config.NormalizedTimeStop < 1f && GetCurrentNormalizedTime() > config.NormalizedTimeStop)
			{
				ResetDynamicSpeed();
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (from != config.SkillID && to == config.SkillID)
			{
				_state = State.Idle;
				SetDynamicSpeed();
			}
			else if (from == config.SkillID && to != config.SkillID)
			{
				ResetDynamicSpeed();
			}
		}

		private float GetCurrentNormalizedTime()
		{
			return (!(entity.GetCurrentNormalizedTime() > 1f)) ? entity.GetCurrentNormalizedTime() : 1f;
		}

		private bool CheckNormalizedTime()
		{
			float currentNormalizedTime = GetCurrentNormalizedTime();
			return (currentNormalizedTime >= config.NormalizedTimeStart && currentNormalizedTime <= config.NormalizedTimeStop) ? true : false;
		}

		private void SetDynamicSpeed()
		{
			if (CheckNormalizedTime())
			{
				if ((bool)_entity.GetAttackTarget())
				{
					float num = Vector3.Distance(_entity.GetAttackTarget().XZPosition, _entity.XZPosition);
					if (num > config.MaxDynamicDistanceDistance)
					{
						num = config.MaxDynamicDistanceDistance;
					}
					if (num < config.MinDynamicDistanceDistance)
					{
						num = config.MinDynamicDistanceDistance;
					}
					_animatorMoveStackIx = _entity.PushProperty("Animator_RigidBodyVelocityRatio", num / config.DefaultDistance - 1f);
					entity.PushHighspeedMovement();
				}
				else if (config.NoTargetDistance != 0f)
				{
					_animatorMoveStackIx = _entity.PushProperty("Animator_RigidBodyVelocityRatio", config.NoTargetDistance / config.DefaultDistance - 1f);
					entity.PushHighspeedMovement();
				}
				else
				{
					_animatorMoveStackIx = 0;
				}
				_state = State.Runing;
			}
			else if (GetCurrentNormalizedTime() < config.NormalizedTimeStart)
			{
				_state = State.Wait;
			}
			else
			{
				_state = State.End;
			}
		}

		private void ResetDynamicSpeed()
		{
			_state = State.End;
			if (_animatorMoveStackIx != 0)
			{
				entity.PopHighspeedMovement();
				_entity.PopProperty("Animator_RigidBodyVelocityRatio", _animatorMoveStackIx);
				_animatorMoveStackIx = 0;
			}
		}
	}
}
