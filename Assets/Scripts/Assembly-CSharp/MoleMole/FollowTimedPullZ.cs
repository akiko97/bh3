using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class FollowTimedPullZ : BaseFollowShortState
	{
		private enum State
		{
			Entering = 0,
			PullZ = 1,
			Exiting = 2
		}

		[ShowInInspector]
		private float _origRadius;

		[ShowInInspector]
		private float _targetRadius;

		[ShowInInspector]
		private float _origElevation;

		[ShowInInspector]
		private float _targetElevation;

		[ShowInInspector]
		private float _origCenterY;

		[ShowInInspector]
		private float _targetCenterY;

		[ShowInInspector]
		private float _origFOV;

		[ShowInInspector]
		private float _targetFOV;

		[ShowInInspector]
		private float _pullZTimer;

		[ShowInInspector]
		private float _timer;

		[ShowInInspector]
		private float _exitDuration;

		[ShowInInspector]
		private float _lerpTimer;

		[ShowInInspector]
		private float _lerpTotalTime;

		[ShowInInspector]
		private string _lerpCurveName;

		[ShowInInspector]
		private AnimationCurve _lerpCurve;

		[ShowInInspector]
		private State _state;

		public FollowTimedPullZ(MainCameraFollowState followState)
			: base(followState)
		{
			base.isSkippingBaseState = false;
		}

		public void SetTimedPullZ(float radiusRatio, float elevationAngles, float centerYOffset, float fovOffset, float time, float lerpTime, string lerpCurveName)
		{
			_targetRadius = radiusRatio * _owner.recoverState.GetOriginalRadius();
			_targetElevation = _owner.recoverState.GetOriginalElevation() + elevationAngles;
			_targetCenterY = _owner.recoverState.GetOriginalCenterY() + centerYOffset;
			_targetFOV = Mathf.Max(0f, _owner.recoverState.GetOriginalFOV() + fovOffset);
			_pullZTimer = time;
			_lerpTimer = lerpTime;
			_lerpTotalTime = lerpTime;
			_lerpCurveName = lerpCurveName;
			_lerpCurve = CameraData.GetCameraCurveByName(_lerpCurveName);
		}

		public override void Enter()
		{
			if (_owner.recoverState.active)
			{
				_owner.recoverState.CancelAndStopAtCurrentState();
			}
			_origRadius = _owner.recoverState.GetOriginalRadius();
			_origElevation = _owner.recoverState.GetOriginalElevation();
			_origCenterY = _owner.recoverState.GetOriginalCenterY();
			_origFOV = _owner.mainCamera.cameraComponent.fieldOfView;
			_timer = 0f;
			_state = State.Entering;
		}

		public override void Update()
		{
			if (_state == State.Entering)
			{
				_lerpTimer -= Time.deltaTime * _owner.mainCamera.TimeScale;
				if (_lerpTimer > 0f)
				{
					float t = ((_lerpCurve != null) ? _lerpCurve.Evaluate(1f - _lerpTimer / _lerpTotalTime) : (1f - _lerpTimer / _lerpTotalTime));
					_owner.anchorRadius = Mathf.Clamp(Mathf.Lerp(_origRadius, _targetRadius, t), 4f, 11f);
					_owner.anchorElevation = Mathf.Lerp(_origElevation, _targetElevation, t);
					_owner.followCenterY = Mathf.Lerp(_origCenterY, _targetCenterY, t);
					_owner.cameraFOV = Mathf.Lerp(_origFOV, _targetFOV, t);
				}
				else
				{
					_owner.anchorRadius = Mathf.Clamp(_targetRadius, 4f, 11f);
					_owner.anchorElevation = _targetElevation;
					_owner.followCenterY = _targetCenterY;
					_owner.cameraFOV = _targetFOV;
					_state = State.PullZ;
				}
			}
			else if (_state == State.PullZ)
			{
				_pullZTimer -= Time.deltaTime * _owner.mainCamera.TimeScale;
				if (_pullZTimer <= 0f)
				{
					float a = 0.1f * Mathf.Abs(_targetFOV - _origFOV);
					float b = 0.35f * Mathf.Abs(_targetRadius - _origRadius);
					_exitDuration = Mathf.Max(a, b);
					_state = State.Exiting;
				}
			}
			else if (_state == State.Exiting)
			{
				_timer += Time.deltaTime * _owner.mainCamera.TimeScale;
				if (_timer < _exitDuration)
				{
					_owner.anchorRadius = Mathf.Clamp(Mathf.Lerp(_targetRadius, _origRadius, _timer / _exitDuration), 4f, 11f);
					_owner.anchorElevation = Mathf.Lerp(_targetElevation, _origElevation, _timer / _exitDuration);
					_owner.followCenterY = Mathf.Lerp(_targetCenterY, _origCenterY, _timer / _exitDuration);
					_owner.cameraFOV = Mathf.Lerp(_targetFOV, _origFOV, _timer / _exitDuration);
				}
				else
				{
					_owner.anchorRadius = Mathf.Clamp(_origRadius, 4f, 11f);
					_owner.anchorElevation = _origElevation;
					_owner.followCenterY = _origCenterY;
					_owner.cameraFOV = _origFOV;
					End();
				}
			}
		}

		public void ForceToExitState()
		{
			_pullZTimer = 0f;
			float a = 0.1f * Mathf.Abs(_targetFOV - _origFOV);
			float b = 0.35f * Mathf.Abs(_targetRadius - _origRadius);
			_exitDuration = Mathf.Max(a, b);
			_state = State.Exiting;
		}

		public override void Exit()
		{
			_owner.recoverState.TryRecover();
		}
	}
}
