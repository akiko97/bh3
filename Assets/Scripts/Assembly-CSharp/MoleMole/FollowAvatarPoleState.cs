using UnityEngine;

namespace MoleMole
{
	public class FollowAvatarPoleState : BaseFollowBaseState
	{
		private float _enteringLerpDuration;

		private bool _enteringLerp;

		private float _startPolar;

		private float _startForwardDelta;

		private float _targetPolar;

		private float _targetForwardDelta;

		private float _lerpTimer;

		public FollowAvatarPoleState(MainCameraFollowState followState)
			: base(followState)
		{
		}

		public void SetEnteringLerpTarget(float targetPolar)
		{
			_targetPolar = targetPolar;
			maskedShortStates.Add(_owner.rotateToAvatarFacingState);
		}

		public void CancelEnteringLerp()
		{
			_enteringLerp = false;
		}

		public override void Enter()
		{
			if (_owner.lastBaseState == _owner.followAvatarAndTargetState)
			{
				_enteringLerp = true;
				_lerpTimer = 0f;
				_startPolar = _owner.anchorPolar;
				_startForwardDelta = _owner.forwardDeltaAngle;
				_targetForwardDelta = 0f;
				_enteringLerpDuration = Miscs.AbsAngleDiff(_startPolar, _targetPolar) * 0.1f;
				base.cannotBeSkipped = true;
			}
		}

		private float GetPoleFollowAnchor()
		{
			Vector3 vector = _owner.followData.anchorPosition - _owner.followCenterXZPosition;
			return Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		}

		public override void Update()
		{
			if (_enteringLerp)
			{
				_lerpTimer += Time.deltaTime * _owner.mainCamera.TimeScale;
				if (_lerpTimer > _enteringLerpDuration)
				{
					_owner.anchorPolar = _targetPolar;
					_owner.forwardDeltaAngle = _targetForwardDelta;
					_enteringLerp = false;
					maskedShortStates.Remove(_owner.rotateToAvatarFacingState);
					base.cannotBeSkipped = false;
				}
				else
				{
					float t = _lerpTimer / _enteringLerpDuration;
					_owner.anchorPolar = Mathf.Lerp(_startPolar, _targetPolar, t);
					_owner.forwardDeltaAngle = Mathf.Lerp(_startForwardDelta, _targetForwardDelta, t);
				}
			}
			else
			{
				_owner.anchorPolar = GetPoleFollowAnchor();
			}
		}

		public override void Exit()
		{
			if (maskedShortStates.Contains(_owner.rotateToAvatarFacingState))
			{
				maskedShortStates.Remove(_owner.rotateToAvatarFacingState);
			}
			_enteringLerp = false;
			_owner.forwardDeltaAngle = 0f;
		}

		public override void ResetState()
		{
			_enteringLerp = false;
		}
	}
}
