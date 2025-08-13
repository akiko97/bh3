using UnityEngine;

namespace MoleMole
{
	public class FollowLookAtPosition : BaseFollowShortState
	{
		private Vector3 _targetPosition;

		private float MAX_FOLLOW_TIME = 2f;

		private float _timer;

		private bool _mute;

		public FollowLookAtPosition(MainCameraFollowState followState)
			: base(followState)
		{
			base.isSkippingBaseState = false;
		}

		public void SetLookAtTarget(Vector3 targetPosition, bool mute = false)
		{
			_targetPosition = targetPosition;
			_mute = mute;
		}

		public override void Enter()
		{
			if (Singleton<LevelDesignManager>.Instance.IsPointInCameraFov(_targetPosition))
			{
				End();
			}
			_timer = MAX_FOLLOW_TIME;
			if (_owner.recoverState.active)
			{
				_owner.recoverState.CancelLerpRatioRecovering();
			}
			if (_mute)
			{
				Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(false);
				Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
			}
		}

		public override void PostUpdate()
		{
			_timer -= Time.deltaTime;
			Vector3 vector = _targetPosition - _owner.cameraPosition;
			float num = Miscs.AngleFromToIgnoreY(-_owner.cameraForward, -vector);
			if (Mathf.Abs(num) < 20f || _timer < 0f)
			{
				End();
				return;
			}
			float b = _owner.anchorPolar - num;
			_owner.anchorPolar = Mathf.Lerp(_owner.anchorPolar, b, Time.deltaTime * 5f / 5f);
			_owner.posLerpRatio = 2f;
			_owner.forwardLerpRatio = 2f;
			_owner.needLerpPositionThisFrame = true;
			_owner.needLerpForwardThisFrame = true;
		}

		public override void Exit()
		{
			_owner.posLerpRatio = 1f;
			_owner.forwardLerpRatio = 1f;
			if (_mute)
			{
				Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(true);
				Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
			}
		}
	}
}
