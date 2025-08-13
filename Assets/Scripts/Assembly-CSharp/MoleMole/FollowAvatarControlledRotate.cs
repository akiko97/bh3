using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class FollowAvatarControlledRotate : BaseFollowBaseState
	{
		private enum State
		{
			Entering = 0,
			Controlling = 1
		}

		private const float DRAG_ELEVATION_MIN = -10f;

		private const float DRAG_ELEVATION_MAX = 50f;

		private const float ANGULAR_VELOCITY_DRAG_RATIO = 16f;

		private const float TO_EXIT_DURATION = 0.75f;

		private const float DELTA_TO_ANGULAR_POLAR_RATIO = 2000f;

		private const float DELTA_TO_ANGULAR_ELEVATION_RATIO = 400f;

		private const float POS_LERP_INCREASE_RATIO = 9f;

		private const float FORWARD_LERP_INCREASE_RATIO = 9f;

		private const float DELTA_TO_ZOOM_RATIO = 3f;

		private const float DELTA_TO_ZOOM_END_RATIO = 10f;

		private const float MAX_ZOOM_RADIUS = 12f;

		private const float MIN_ZOOM_RADIUS = 4f;

		[ShowInInspector]
		private bool _hasDragDataThisFrame;

		[ShowInInspector]
		private bool _hasZoomDataThisFrame;

		[ShowInInspector]
		private Vector2 _pointerDragDelta;

		[ShowInInspector]
		private float _polarAngularVelcity;

		[ShowInInspector]
		private float _elevationAngularVelocity;

		private float _zoomDelta;

		private float _zoomVelocity;

		private float _lerpTimer;

		[ShowInInspector]
		private EntityTimer _exitTimer;

		[ShowInInspector]
		private State _state;

		public FollowAvatarControlledRotate(MainCameraFollowState followState)
			: base(followState)
		{
			maskedShortStates.Add(_owner.standByState);
			maskedShortStates.Add(_owner.lookAtPositionState);
			maskedShortStates.Add(_owner.timedPullZState);
			_exitTimer = new EntityTimer(0.75f, _owner.mainCamera);
		}

		public override void Enter()
		{
			_owner.recoverState.TryRecover();
			_owner.recoverState.CancelElevationRecover();
			_state = State.Entering;
			_lerpTimer = 0f;
			_exitTimer.Reset(false);
			_owner.forwardLerpRatio = 1f;
			_owner.posLerpRatio = 1f;
		}

		public override void Update()
		{
			if (_state == State.Entering)
			{
				UpdateControlled();
				_lerpTimer += Time.deltaTime * _owner.mainCamera.TimeScale;
				if (_owner.lerpPositionOvershootLastFrame && _owner.lerpForwardOvershootLastFrame)
				{
					_owner.forwardLerpRatio = 1f;
					_owner.posLerpRatio = 1f;
					_owner.needLerpPositionThisFrame = false;
					_owner.needLerpForwardThisFrame = false;
					_state = State.Controlling;
				}
				else
				{
					_owner.forwardLerpRatio = _lerpTimer * 9f;
					_owner.posLerpRatio = _lerpTimer * 9f;
					_owner.needLerpPositionThisFrame = true;
					_owner.needLerpForwardThisFrame = true;
				}
			}
			else
			{
				UpdateControlled();
				_owner.needLerpPositionThisFrame = false;
				_owner.needLerpForwardThisFrame = false;
				_exitTimer.Core(1f);
				if (_exitTimer.isTimeUp && !Singleton<CameraManager>.Instance.controlledRotateKeepManual)
				{
					_owner.TryToTransitToOtherBaseState(true);
				}
			}
		}

		public void SetExitingControl(bool exiting)
		{
			if (exiting)
			{
				_exitTimer.Reset(true);
			}
			else
			{
				_exitTimer.Reset(false);
			}
		}

		private void UpdateControlled()
		{
			if (_hasDragDataThisFrame)
			{
				_hasDragDataThisFrame = false;
				_polarAngularVelcity += _pointerDragDelta.x * 2000f;
				_elevationAngularVelocity += _pointerDragDelta.y * 400f;
			}
			if (_hasZoomDataThisFrame)
			{
				_hasZoomDataThisFrame = false;
				_zoomVelocity = _zoomDelta * 3f;
			}
			_zoomVelocity = Mathf.Lerp(_zoomVelocity, 0f, Time.deltaTime * _owner.mainCamera.TimeScale * 10f);
			_owner.anchorRadius += _zoomVelocity * _owner.mainCamera.TimeScale * Time.deltaTime;
			_owner.anchorRadius = Mathf.Clamp(_owner.anchorRadius, 4f, 12f);
			_owner.anchorPolar += Time.deltaTime * _owner.mainCamera.TimeScale * _polarAngularVelcity;
			_owner.anchorElevation += Time.deltaTime * _owner.mainCamera.TimeScale * _elevationAngularVelocity;
			_owner.anchorElevation = Mathf.Clamp(_owner.anchorElevation, -10f, 50f);
			_polarAngularVelcity = Mathf.Lerp(_polarAngularVelcity, 0f, Time.deltaTime * _owner.mainCamera.TimeScale * 16f);
			_elevationAngularVelocity = Mathf.Lerp(_elevationAngularVelocity, 0f, Time.deltaTime * _owner.mainCamera.TimeScale * 16f);
		}

		public override void Exit()
		{
			_owner.forwardLerpRatio = 8f;
			_owner.posLerpRatio = 8f;
			_owner.recoverState.TryRecover();
		}

		public void SetDragDelta(Vector2 delta)
		{
			_state = ((_owner.lerpPositionOvershootLastFrame && _owner.lerpForwardOvershootLastFrame) ? State.Controlling : State.Entering);
			_hasDragDataThisFrame = true;
			_pointerDragDelta.x = 0f - delta.x;
			_pointerDragDelta.y = 0f - delta.y;
		}

		public void SetZoomDelta(float zoomDelta)
		{
			_state = ((_owner.lerpPositionOvershootLastFrame && _owner.lerpForwardOvershootLastFrame) ? State.Controlling : State.Entering);
			_hasZoomDataThisFrame = true;
			_zoomDelta = zoomDelta;
		}

		public override void ResetState()
		{
		}

		public bool IsExiting()
		{
			return _exitTimer.isActive;
		}
	}
}
