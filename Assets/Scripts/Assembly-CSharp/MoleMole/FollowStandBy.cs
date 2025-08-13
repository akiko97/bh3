using UnityEngine;

namespace MoleMole
{
	public class FollowStandBy : BaseFollowShortState
	{
		private enum State
		{
			Entering = 0,
			StandBy = 1,
			Exiting = 2
		}

		private const float RADIUS_LINEAR_LERP_ENTER_TIME = 1.5f;

		private const float RADIUS_LINEAR_LERP_EXIT_TIME = 0.3f;

		private float _origRadius;

		private float _targetRadius;

		private float _origCenterOffsetY;

		private float _targetCenterOffsetY;

		private float _timer;

		private State _state;

		public FollowStandBy(MainCameraFollowState followState)
			: base(followState)
		{
			base.isSkippingBaseState = true;
		}

		public override void Enter()
		{
			if (_owner.recoverState.active)
			{
				_owner.recoverState.CancelPosAndForwardRecover();
			}
			_origRadius = _owner.anchorRadius;
			_targetRadius = 3.84f;
			_origCenterOffsetY = _owner.followCenterY;
			_targetCenterOffsetY = _owner.followCenterY * 0.85f;
			_timer = 0f;
			_state = State.Entering;
			_owner.posLerpRatio = 0.2f;
			_owner.forwardLerpRatio = 0.2f;
		}

		public override void Update()
		{
			if (_state == State.Entering)
			{
				if (!_owner.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Stable))
				{
					_timer = 0f;
					_owner.posLerpRatio = 1f;
					_owner.forwardLerpRatio = 1f;
					_state = State.Exiting;
					return;
				}
				_timer += Time.deltaTime * _owner.mainCamera.TimeScale;
				if (_timer < 1.5f)
				{
					float t = _timer / 1.5f;
					_owner.anchorRadius = Mathf.Lerp(_origRadius, _targetRadius, t);
					_owner.followCenterY = Mathf.Lerp(_origCenterOffsetY, _targetCenterOffsetY, t);
				}
				else
				{
					_owner.anchorRadius = _targetRadius;
					_owner.followCenterY = _targetCenterOffsetY;
					_state = State.StandBy;
				}
			}
			else if (_state == State.StandBy)
			{
				if (!_owner.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Stable))
				{
					_owner.posLerpRatio = 1f;
					_owner.forwardLerpRatio = 1f;
					_timer = 0f;
					_state = State.Exiting;
				}
			}
			else if (_state == State.Exiting)
			{
				_timer += Time.deltaTime * _owner.mainCamera.TimeScale;
				if (_timer < 0.3f)
				{
					float t2 = _timer / 0.3f;
					_owner.anchorRadius = Mathf.Lerp(_targetRadius, _origRadius, t2);
					_owner.followCenterY = Mathf.Lerp(_targetCenterOffsetY, _origCenterOffsetY, t2);
				}
				else
				{
					_owner.anchorRadius = _origRadius;
					_owner.followCenterY = _origCenterOffsetY;
					End();
				}
			}
		}

		public override void Exit()
		{
			_owner.recoverState.TryRecover();
			_owner.posLerpRatio = 1f;
			_owner.forwardLerpRatio = 1f;
		}
	}
}
