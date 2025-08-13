using FullInspector;
using UnityEngine;

namespace MoleMole
{
	[fiInspectorOnly]
	public class BaseMainCameraState : State<MonoMainCamera>
	{
		public Vector3 cameraPosition;

		public Vector3 cameraForward;

		public float cameraFOV;

		public bool muteCameraShake;

		public bool lerpDirectionalLight;

		public float cameraShakeRatio = 1f;

		public BaseMainCameraState(MonoMainCamera camera)
			: base(camera)
		{
		}
	}
}
