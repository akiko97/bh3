namespace MoleMole
{
	public class MainCameraStaticState : BaseMainCameraState
	{
		public MainCameraStaticState(MonoMainCamera camera)
			: base(camera)
		{
		}

		public override void Enter()
		{
			cameraPosition = _owner.transform.position;
			cameraForward = _owner.transform.forward;
			cameraFOV = _owner.originalFOV;
		}

		public override void Update()
		{
		}

		public override void Exit()
		{
		}
	}
}
