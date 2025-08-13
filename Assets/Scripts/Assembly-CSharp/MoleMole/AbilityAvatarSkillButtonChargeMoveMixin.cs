using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAvatarSkillButtonChargeMoveMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			InLoop = 1,
			After = 2
		}

		private const float SMOOTH_LENGTH = 1f;

		public AvatarSkillButtonHoldChargeMoveMixin config;

		private BaseMonoAvatar _avatar;

		private State _state;

		private HashSet<string> _chargeLoopSkillIDs;

		private float _moveSpeed;

		private Vector3 _moveDirection = Vector3.zero;

		private float _smoothTimer;

		private bool _isSteer;

		private float _steerSpeed;

		private bool hasSteerBefore;

		private bool inStoping;

		public AbilityAvatarSkillButtonChargeMoveMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarSkillButtonHoldChargeMoveMixin)config;
			_avatar = (BaseMonoAvatar)entity;
			_chargeLoopSkillIDs = new HashSet<string>(this.config.ChargeLoopSkillIDs);
			_moveSpeed = instancedAbility.Evaluate(this.config.MoveSpeed);
			_isSteer = this.config.IsSteer;
			_steerSpeed = this.config.SteerSpeed;
		}

		public override void OnAdded()
		{
			_state = State.Idle;
			BaseMonoAvatar avatar = _avatar;
			avatar.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(avatar.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
		}

		public override void OnRemoved()
		{
			BaseMonoAvatar avatar = _avatar;
			avatar.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(avatar.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			_avatar.SetHasAdditiveVelocity(false);
			_avatar.SetAdditiveVelocity(Vector3.zero);
		}

		public override void Core()
		{
			if (_state != State.InLoop)
			{
				return;
			}
			AvatarControlData activeControlData = _avatar.GetActiveControlData();
			if (activeControlData.hasSteer)
			{
				if (_isSteer)
				{
					Vector3 steerDirection = activeControlData.steerDirection;
					_avatar.SteerFaceDirectionTo(Vector3.Lerp(_avatar.FaceDirection, steerDirection, _steerSpeed * Time.deltaTime));
				}
				else
				{
					_avatar.SetHasAdditiveVelocity(true);
					_avatar.SetAdditiveVelocity(activeControlData.steerDirection * _moveSpeed);
				}
				_moveDirection = activeControlData.steerDirection;
				hasSteerBefore = true;
				inStoping = false;
			}
			else
			{
				if (_isSteer)
				{
					return;
				}
				if (hasSteerBefore)
				{
					hasSteerBefore = false;
					inStoping = true;
					_smoothTimer = 0f;
				}
				if (inStoping)
				{
					_smoothTimer += Time.deltaTime * entity.TimeScale;
					_avatar.SetHasAdditiveVelocity(true);
					_avatar.SetAdditiveVelocity(_moveDirection * Mathf.Lerp(0f, _moveSpeed, 1f - _smoothTimer / 1f));
					if (_smoothTimer > 1f)
					{
						inStoping = false;
						_smoothTimer = 0f;
					}
				}
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (_chargeLoopSkillIDs.Contains(to))
			{
				_state = State.InLoop;
			}
			if (_state == State.InLoop && !_chargeLoopSkillIDs.Contains(to))
			{
				_state = State.After;
				_avatar.SetHasAdditiveVelocity(false);
				_avatar.SetAdditiveVelocity(Vector3.zero);
				_moveDirection = Vector3.zero;
			}
		}
	}
}
