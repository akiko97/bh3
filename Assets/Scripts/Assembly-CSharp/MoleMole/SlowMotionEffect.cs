using UnityEngine;

namespace MoleMole
{
	internal class SlowMotionEffect
	{
		public class OutParams
		{
			public float timeScale;

			public float anchorDeltaPolar;

			public float anchorElevationOffset;

			public float anchorRadiusOffset;

			public float forwardDeltaAngle;
		}

		private float _origForwardDeltaAngle;

		private float _duringTimer;

		private float _duration;

		private AnimationCurve _timeScaleCurve;

		private AnimationCurve _cameraSpeedCurve;

		private AnimationCurve _cameraRadiusOffsetCurve;

		private float _cameraRotateSpeed;

		private float _minCameraRotateAngle;

		private float _maxCameraRotateAngle;

		private float _minCameraElevationOffset;

		private float _maxCameraElevationOffset;

		private float _minCameraRadiusOffset;

		private float _maxCameraRadiusOffset;

		private Vector4 _distanceAttenuationFactorsForRotateAngle;

		private Vector4 _distanceAttenuationFactorsForElevationOffset;

		private float _distTarget;

		private float _cameraSpeedCurveIntegral;

		private AnimationCurve _cameraElevationCurve;

		private float _polarVelSign;

		private float _cameraElevationOffset;

		private float _cameraRadiusOffset;

		private bool _nearCase;

		private bool _cameraNearCase;

		private float _targetForwardDeltaAngle;

		protected bool _hasUserControled;

		public OutParams outParams { get; private set; }

		public bool active { get; set; }

		private static float CalcCurveIntegral01(AnimationCurve curve)
		{
			float num = 0f;
			for (int i = 0; i < 60; i++)
			{
				num += curve.Evaluate(1f / 60f * (float)i);
			}
			return num * (1f / 60f);
		}

		public void Set(ConfigCameraSlowMotionKill config, float distTarget, float distCamera)
		{
			outParams = new OutParams();
			_duration = config.Duration;
			_timeScaleCurve = config.TimeScaleCurve;
			_cameraSpeedCurve = config.CameraSpeedCurve;
			_cameraRadiusOffsetCurve = config.CameraRadiusOffsetCurve;
			_minCameraRotateAngle = config.MinCameraRotateAngle;
			_maxCameraRotateAngle = config.MaxCameraRotateAngle;
			_minCameraElevationOffset = config.MinCameraElevationOffset;
			_maxCameraElevationOffset = config.MaxCameraElevationOffset;
			_minCameraRadiusOffset = config.MinCameraRadiusOffset;
			_maxCameraRadiusOffset = config.MaxCameraRadiusOffset;
			_distanceAttenuationFactorsForRotateAngle = config.DistanceAttenuationFactorsForRotateAngle;
			_distanceAttenuationFactorsForElevationOffset = config.DistanceAttenuationFactorsForElevationOffset;
			_distTarget = distTarget;
			_nearCase = distTarget < config.DistanceThreshold;
			_cameraNearCase = distCamera < config.CameraDistanceThreshold;
			_cameraSpeedCurveIntegral = CalcCurveIntegral01(_cameraSpeedCurve);
			_cameraElevationCurve = AnimationCurve.EaseInOut(0f, 0f, 0.5f, 1f);
			_cameraElevationCurve.postWrapMode = WrapMode.PingPong;
		}

		public void Enter(MainCameraFollowState cameraState)
		{
			_origForwardDeltaAngle = cameraState.forwardDeltaAngle;
			_duringTimer = 0f;
			_hasUserControled = false;
			float num = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(cameraState.cameraForward, cameraState.avatar.FaceDirection)));
			if (_nearCase)
			{
				_polarVelSign = ((!(Random.value > 0.3f)) ? (0f - num) : num);
			}
			else
			{
				_polarVelSign = 0f - num;
			}
			float num2 = Random.Range(_minCameraRotateAngle, _maxCameraRotateAngle);
			_cameraRotateSpeed = num2 / _duration / _cameraSpeedCurveIntegral;
			float value = Random.value;
			_cameraElevationOffset = Mathf.Lerp(_maxCameraElevationOffset, _minCameraElevationOffset, value);
			_cameraRadiusOffset = Random.Range(_minCameraRadiusOffset, _maxCameraRadiusOffset);
			if (_cameraNearCase)
			{
				_cameraRadiusOffset = Random.Range(0f, _maxCameraRadiusOffset);
			}
			Vector4 distanceAttenuationFactorsForRotateAngle = _distanceAttenuationFactorsForRotateAngle;
			float b = distanceAttenuationFactorsForRotateAngle.x + _distTarget * distanceAttenuationFactorsForRotateAngle.y + _distTarget * _distTarget * distanceAttenuationFactorsForRotateAngle.z + _distTarget * _distTarget * _distTarget * distanceAttenuationFactorsForRotateAngle.w;
			b = 1f / Mathf.Max(1f, b);
			_cameraRotateSpeed *= b;
			distanceAttenuationFactorsForRotateAngle = _distanceAttenuationFactorsForElevationOffset;
			float b2 = distanceAttenuationFactorsForRotateAngle.x + _distTarget * distanceAttenuationFactorsForRotateAngle.y + _distTarget * _distTarget * distanceAttenuationFactorsForRotateAngle.z + _distTarget * _distTarget * _distTarget * distanceAttenuationFactorsForRotateAngle.w;
			b2 = 1f / Mathf.Max(1f, b2);
			_cameraElevationOffset *= b2;
			if (!cameraState.followAvatarControlledRotate.active)
			{
				cameraState.TransitBaseState(cameraState.followAvatarState);
			}
		}

		public bool OverDuration(float progress)
		{
			return _duringTimer / _duration > progress;
		}

		public float GetProgress()
		{
			return _duringTimer / _duration;
		}

		public void Update()
		{
			_duringTimer += Time.unscaledDeltaTime;
			float num = _duringTimer / _duration;
			outParams.timeScale = _timeScaleCurve.Evaluate(num);
			outParams.anchorDeltaPolar = _polarVelSign * _cameraSpeedCurve.Evaluate(num) * _cameraRotateSpeed * Time.unscaledDeltaTime;
			outParams.anchorElevationOffset = _cameraElevationCurve.Evaluate(num) * _cameraElevationOffset;
			outParams.anchorRadiusOffset = _cameraRadiusOffsetCurve.Evaluate(num) * _cameraRadiusOffset;
			outParams.forwardDeltaAngle = Mathf.Lerp(_origForwardDeltaAngle, _targetForwardDeltaAngle, num);
			if (_duringTimer > _duration)
			{
				active = false;
			}
		}
	}
}
