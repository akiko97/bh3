using UnityEngine;

namespace MoleMole
{
	public sealed class MonoStageOrthCamera : BaseMonoCamera
	{
		private Vector3 _origOrthCameraPosition;

		public void Init(uint runtimeID)
		{
			Init(3u, runtimeID);
			_cameraTrans.SetPositionY(-100f);
			_origOrthCameraPosition = _cameraTrans.transform.position;
		}

		public override void Update()
		{
			base.Update();
		}

		public void SetShake(Vector3 shakeOffset)
		{
			Vector3 vector = base.transform.TransformDirection(shakeOffset);
			_cameraTrans.position = _origOrthCameraPosition + vector * 0.15f;
		}

		public void ResumePosition()
		{
			base.transform.position = _origOrthCameraPosition;
		}
	}
}
