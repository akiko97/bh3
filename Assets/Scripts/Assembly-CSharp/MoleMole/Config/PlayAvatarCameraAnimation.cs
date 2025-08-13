namespace MoleMole.Config
{
	public class PlayAvatarCameraAnimation : ConfigAvatarCameraAction
	{
		public string CameraAnimName;

		public bool ExitTransitionLerp = true;

		public MainCameraFollowState.EnterPolarMode EnterPolarMode;
	}
}
