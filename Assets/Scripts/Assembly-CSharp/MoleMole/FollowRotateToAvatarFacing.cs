using UnityEngine;

namespace MoleMole
{
	public class FollowRotateToAvatarFacing : BaseFollowShortState
	{
		private float _polar;

		private float _targetPolar;

		public FollowRotateToAvatarFacing(MainCameraFollowState followState)
			: base(followState)
		{
			base.isSkippingBaseState = true;
		}

		public bool CanRotate()
		{
			float f = Miscs.AngleFromToIgnoreY(-_owner.cameraForward, -_owner.avatar.FaceDirection);
			float num = Mathf.Abs(f);
			return num > 10f;
		}

		public override void Enter()
		{
			if (_owner.followAvatarControlledRotate.active)
			{
				_owner.followAvatarControlledRotate.SetExitingControl(false);
			}
			float num = Miscs.AngleFromToIgnoreY(-_owner.cameraForward, -_owner.avatar.FaceDirection);
			float num2 = Mathf.Abs(num);
			if (num2 < 10f)
			{
				_targetPolar = _owner.anchorPolar;
			}
			else if (num2 > 90f && !_owner.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Stable))
			{
				_targetPolar = _owner.anchorPolar - Mathf.Sign(num) * 90f;
			}
			else
			{
				_targetPolar = _owner.anchorPolar - num;
			}
			_polar = _owner.anchorPolar;
		}

		public override void PostUpdate()
		{
			if (Mathf.Abs(_polar - _targetPolar) < 2f)
			{
				End();
			}
			else
			{
				_polar = Mathf.Lerp(_polar, _targetPolar, Time.deltaTime * 5f);
				_owner.anchorPolar = _polar;
			}
			_owner.needLerpPositionThisFrame = false;
			_owner.needLerpForwardThisFrame = false;
		}

		public override void Exit()
		{
			if (_owner.followAvatarControlledRotate.active)
			{
				_owner.followAvatarControlledRotate.SetExitingControl(true);
				return;
			}
			_owner.forwardLerpRatio = 8f;
			_owner.posLerpRatio = 8f;
			_owner.recoverState.TryRecover();
		}
	}
}
