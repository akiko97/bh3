using UnityEngine;

namespace MoleMole
{
	public class MainCameraCinemaState : BaseMainCameraState
	{
		private ICinema _cinema;

		public MainCameraCinemaState(MonoMainCamera camera)
			: base(camera)
		{
		}

		public void SetCinema(ICinema cinema)
		{
			_cinema = cinema;
		}

		public override void Enter()
		{
			cameraPosition = _owner.transform.position;
			cameraForward = _owner.transform.forward;
			cameraFOV = _owner.originalFOV;
			if (_cinema.GetInitCameraClipZNear() > 0f)
			{
				_owner.cameraComponent.nearClipPlane = Mathf.Max(0.01f, _cinema.GetInitCameraClipZNear());
			}
			if (_cinema.GetInitCameraFOV() > 0f)
			{
				_owner.cameraComponent.fieldOfView = _cinema.GetInitCameraFOV();
			}
			Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
		}

		public override void Update()
		{
			Transform cameraTransform = _cinema.GetCameraTransform();
			if (cameraTransform != null)
			{
				cameraPosition = cameraTransform.position;
				cameraForward = cameraTransform.forward;
			}
		}

		public override void Exit()
		{
			_owner.cameraComponent.nearClipPlane = _owner.originalNearClip;
			_owner.cameraComponent.fieldOfView = _owner.originalFOV;
			Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
		}
	}
}
