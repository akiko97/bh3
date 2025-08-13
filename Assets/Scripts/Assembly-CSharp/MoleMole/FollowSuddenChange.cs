using UnityEngine;

namespace MoleMole
{
	public class FollowSuddenChange : BaseFollowShortState
	{
		private BaseMonoAvatar _nextAvatar;

		public FollowSuddenChange(MainCameraFollowState followState)
			: base(followState)
		{
			base.isSkippingBaseState = true;
		}

		public void SetSuddenChangeTarget(BaseMonoAvatar avatar)
		{
			_nextAvatar = avatar;
		}

		public override void Enter()
		{
			_owner.TransitBaseState(_owner.followAvatarState);
			_owner.SetupFollowAvatar(_nextAvatar.GetRuntimeID());
			_owner.followCenterXZPosition = _nextAvatar.XZPosition;
		}

		public override void PostUpdate()
		{
			Vector3 faceDirection = _owner.avatar.FaceDirection;
			_owner.anchorPolar = Mathf.Atan2(0f - faceDirection.z, 0f - faceDirection.x) * 57.29578f;
			_owner.needLerpForwardThisFrame = false;
			_owner.needLerpPositionThisFrame = false;
			_owner.needSmoothFollowCenterThisFrame = false;
			End();
		}

		public override void Exit()
		{
		}
	}
}
