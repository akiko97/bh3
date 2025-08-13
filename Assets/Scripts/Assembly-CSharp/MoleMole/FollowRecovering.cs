using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class FollowRecovering : State<MainCameraFollowState>
	{
		private class RecoverParameter
		{
			public static MainCameraFollowState followState;

			public bool needRecover;

			public float originalValue;

			private float _startValue;

			private float _durationRatio;

			private float _duration;

			private float _timer;

			public RecoverParameter(float durationRatio)
			{
				_durationRatio = durationRatio;
			}

			public void SetupRecover(float startValue)
			{
				if (needRecover && _timer < _duration)
				{
					float num = Mathf.Lerp(_startValue, originalValue, _timer / _duration);
					startValue = num;
				}
				if (startValue == originalValue)
				{
					needRecover = false;
					return;
				}
				needRecover = true;
				_startValue = startValue;
				_duration = _durationRatio * Mathf.Abs(_startValue - originalValue);
				_timer = 0f;
			}

			public bool IsDone()
			{
				return !needRecover || _timer >= _duration;
			}

			public void LerpStep(ref float target)
			{
				if (!(_timer > _duration))
				{
					_timer += Time.deltaTime * followState.mainCamera.TimeScale;
					if (_timer < _duration)
					{
						target = Mathf.Lerp(_startValue, originalValue, _timer / _duration);
					}
					else
					{
						target = originalValue;
					}
				}
			}

			public void RecoverImmediately(ref float target)
			{
				target = originalValue;
				needRecover = false;
			}
		}

		private const float POS_RECOVER_DURATION_RATIO = 0.3f;

		private const float ELEVATION_ANGLE_RECOVER_DURATION_RATIO = 0.01f;

		private const float ANGLE_RECOVER_DURATION_RATIO = 0.04f;

		private const float LERPRATIO_RECOVER_DURATION_RATIO = 0.05f;

		private const float FOV_RECOVER_DURATION_RATIO = 0.1f;

		[ShowInInspector]
		private RecoverParameter _recoverRadius;

		[ShowInInspector]
		private RecoverParameter _recoverElevation;

		[ShowInInspector]
		private RecoverParameter _recoverCenterY;

		[ShowInInspector]
		private RecoverParameter _recoverForwardDelta;

		[ShowInInspector]
		private RecoverParameter _recoverLerpPosRatio;

		[ShowInInspector]
		private RecoverParameter _recoverLerpForwardRatio;

		[ShowInInspector]
		private RecoverParameter _recoverFOV;

		public FollowRecovering(MainCameraFollowState owner)
			: base(owner)
		{
			SetActive(false);
			RecoverParameter.followState = _owner;
			_recoverRadius = new RecoverParameter(0.3f);
			_recoverCenterY = new RecoverParameter(0.3f);
			_recoverElevation = new RecoverParameter(0.01f);
			_recoverForwardDelta = new RecoverParameter(0.04f);
			_recoverLerpPosRatio = new RecoverParameter(0.05f);
			_recoverLerpForwardRatio = new RecoverParameter(0.05f);
			_recoverFOV = new RecoverParameter(0.1f);
		}

		public void SetupRecoverRadius(float origRadius)
		{
			_recoverRadius.originalValue = origRadius;
		}

		public float GetOriginalRadius()
		{
			return _recoverRadius.originalValue;
		}

		public float GetOriginalElevation()
		{
			return _recoverElevation.originalValue;
		}

		public float GetOriginalCenterY()
		{
			return _recoverCenterY.originalValue;
		}

		public void SetupRecoverElevation(float origElevation)
		{
			_recoverElevation.originalValue = origElevation;
		}

		public void SetupRecoverCenterY(float origCenterY)
		{
			_recoverCenterY.originalValue = origCenterY;
		}

		public void SetupRecoverForwardDelta(float origForwardDelta)
		{
			_recoverForwardDelta.originalValue = origForwardDelta;
		}

		public void SetupRecoverLerpPosRatio(float origLerpPosRatio)
		{
			_recoverLerpPosRatio.originalValue = origLerpPosRatio;
		}

		public void SetupRecoverLerpForwardRatio(float origLerpForwardRatio)
		{
			_recoverLerpForwardRatio.originalValue = origLerpForwardRatio;
		}

		public float GetOriginalFOV()
		{
			return _recoverFOV.originalValue;
		}

		public void SetupRecoverFOV(float origFOV)
		{
			_recoverFOV.originalValue = origFOV;
		}

		public void TryRecover()
		{
			_recoverRadius.SetupRecover(_owner.anchorRadius);
			_recoverElevation.SetupRecover(_owner.anchorElevation);
			_recoverCenterY.SetupRecover(_owner.followCenterY);
			_recoverForwardDelta.SetupRecover(_owner.forwardDeltaAngle);
			_recoverLerpPosRatio.SetupRecover(_owner.posLerpRatio);
			_recoverLerpForwardRatio.SetupRecover(_owner.forwardLerpRatio);
			_recoverFOV.SetupRecover(_owner.cameraFOV);
			if (_recoverRadius.needRecover || _recoverElevation.needRecover || _recoverCenterY.needRecover || _recoverForwardDelta.needRecover || _recoverLerpPosRatio.needRecover || _recoverLerpForwardRatio.needRecover || _recoverFOV.needRecover)
			{
				Enter();
			}
		}

		public void CancelPosAndForwardRecover()
		{
			_recoverRadius.needRecover = false;
			_recoverElevation.needRecover = false;
			_recoverCenterY.needRecover = false;
			_recoverForwardDelta.needRecover = false;
			_recoverLerpForwardRatio.needRecover = false;
			_recoverLerpPosRatio.needRecover = false;
		}

		public void CancelAndJumpToOriginalState()
		{
			_owner.anchorRadius = _recoverRadius.originalValue;
			_owner.anchorElevation = _recoverElevation.originalValue;
			_owner.followCenterY = _recoverCenterY.originalValue;
			_owner.forwardDeltaAngle = _recoverForwardDelta.originalValue;
			_owner.posLerpRatio = _recoverLerpPosRatio.originalValue;
			_owner.forwardLerpRatio = _recoverLerpForwardRatio.originalValue;
			_owner.cameraFOV = _recoverFOV.originalValue;
			Exit();
		}

		public void CancelElevationRecover()
		{
			_recoverElevation.needRecover = false;
		}

		public void CancelAndStopAtCurrentState()
		{
			Exit();
		}

		public void CancelLerpRatioRecovering()
		{
			_recoverLerpForwardRatio.needRecover = false;
			_recoverLerpPosRatio.needRecover = false;
			_owner.posLerpRatio = 1f;
			_owner.forwardLerpRatio = 1f;
		}

		public void CancelForwardDeltaAngleRecover()
		{
			_recoverForwardDelta.needRecover = false;
		}

		public void RecoverImmediately()
		{
			_recoverRadius.RecoverImmediately(ref _owner.anchorRadius);
			_recoverElevation.RecoverImmediately(ref _owner.anchorElevation);
			_recoverCenterY.RecoverImmediately(ref _owner.followCenterY);
			_recoverForwardDelta.RecoverImmediately(ref _owner.forwardDeltaAngle);
			_recoverLerpPosRatio.RecoverImmediately(ref _owner.posLerpRatio);
			_recoverLerpForwardRatio.RecoverImmediately(ref _owner.forwardLerpRatio);
			_recoverFOV.RecoverImmediately(ref _owner.cameraFOV);
		}

		public override void Enter()
		{
			SetActive(true);
		}

		public override void Update()
		{
			if (_recoverRadius.needRecover)
			{
				_recoverRadius.LerpStep(ref _owner.anchorRadius);
			}
			if (_recoverElevation.needRecover)
			{
				_recoverElevation.LerpStep(ref _owner.anchorElevation);
			}
			if (_recoverCenterY.needRecover)
			{
				_recoverCenterY.LerpStep(ref _owner.followCenterY);
			}
			if (_recoverForwardDelta.needRecover)
			{
				_recoverForwardDelta.LerpStep(ref _owner.forwardDeltaAngle);
			}
			if (_recoverLerpPosRatio.needRecover)
			{
				_recoverLerpPosRatio.LerpStep(ref _owner.posLerpRatio);
			}
			if (_recoverLerpForwardRatio.needRecover)
			{
				_recoverLerpForwardRatio.LerpStep(ref _owner.forwardLerpRatio);
			}
			if (_recoverFOV.needRecover)
			{
				_recoverFOV.LerpStep(ref _owner.cameraFOV);
			}
			if (_recoverRadius.IsDone() && _recoverElevation.IsDone() && _recoverCenterY.IsDone() && _recoverForwardDelta.IsDone() && _recoverLerpPosRatio.IsDone() && _recoverLerpForwardRatio.IsDone() && _recoverFOV.IsDone())
			{
				_owner.recoverState.Exit();
			}
		}

		public override void Exit()
		{
			_recoverRadius.needRecover = false;
			_recoverElevation.needRecover = false;
			_recoverCenterY.needRecover = false;
			_recoverForwardDelta.needRecover = false;
			_recoverLerpForwardRatio.needRecover = false;
			_recoverLerpPosRatio.needRecover = false;
			_recoverFOV.needRecover = false;
			SetActive(false);
		}
	}
}
