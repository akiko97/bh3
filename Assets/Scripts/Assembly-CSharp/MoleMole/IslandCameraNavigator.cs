using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Camera))]
	public class IslandCameraNavigator : MonoBehaviour
	{
		public float ParallexRange = 2.5f;

		public float ParallexSensitivity = 0.05f;

		public float ParallexBoundHardness = 0.5f;

		protected Quaternion baseAttitude;

		private Quaternion referenceAttitude = Quaternion.identity;

		private Quaternion referanceRotation = Quaternion.identity;

		private void Start()
		{
			Input.gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
			baseAttitude = Input.gyro.attitude;
			referenceAttitude = Quaternion.Euler(GameObject.Find("IslandCameraGroup").transform.eulerAngles.x, 0f, 0f);
		}

		private void FixedUpdate()
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, referenceAttitude * ConvertRotation(Quaternion.Inverse(baseAttitude) * referanceRotation * Input.gyro.attitude), ParallexSensitivity);
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.z = 0f;
			base.transform.rotation = Quaternion.Euler(eulerAngles);
			if (Quaternion.Angle(Input.gyro.attitude, baseAttitude) > ParallexRange)
			{
				baseAttitude = Quaternion.Slerp(baseAttitude, Input.gyro.attitude, ParallexBoundHardness);
			}
		}

		private static Quaternion ConvertRotation(Quaternion q)
		{
			return new Quaternion(q.x, q.y, 0f - q.z, 0f - q.w);
		}
	}
}
