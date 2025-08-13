using UnityEngine;

namespace MoleMole
{
	public class ConfigCameraSlowMotionKill : ScriptableObject
	{
		public float Duration;

		public AnimationCurve TimeScaleCurve;

		public AnimationCurve CameraSpeedCurve;

		public AnimationCurve CameraRadiusOffsetCurve;

		public float MinCameraRotateAngle;

		public float MaxCameraRotateAngle;

		public float MinCameraElevationOffset;

		public float MaxCameraElevationOffset;

		public float MinCameraRadiusOffset;

		public float MaxCameraRadiusOffset;

		public Vector4 DistanceAttenuationFactorsForRotateAngle;

		public Vector4 DistanceAttenuationFactorsForElevationOffset;

		public float DistanceThreshold;

		public float CameraDistanceThreshold;
	}
}
