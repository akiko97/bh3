using UnityEngine;

namespace MoleMole
{
	public class FollowAvatarStoryState : BaseFollowBaseState
	{
		private Vector3 _targetCameraPos;

		public FollowAvatarStoryState(MainCameraFollowState followState)
			: base(followState)
		{
		}

		public void SetEnteringLerpTarget(float targetPolar)
		{
			maskedShortStates.Add(_owner.rotateToAvatarFacingState);
		}

		public override void Enter()
		{
			_targetCameraPos = _owner.cameraPosition + new Vector3(1f, 0f, 0f);
		}

		public override void Update()
		{
			_owner.cameraPosition = _targetCameraPos;
		}

		public void StopStoryState()
		{
			Exit();
		}

		public override void Exit()
		{
			if (maskedShortStates.Contains(_owner.rotateToAvatarFacingState))
			{
				maskedShortStates.Remove(_owner.rotateToAvatarFacingState);
			}
			_owner.forwardDeltaAngle = 0f;
		}

		public override void ResetState()
		{
		}
	}
}
