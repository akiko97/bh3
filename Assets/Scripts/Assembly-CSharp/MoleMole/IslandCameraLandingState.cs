namespace MoleMole
{
	public class IslandCameraLandingState : IslandCameraBaseState
	{
		private MonoIslandBuilding _building;

		public IslandCameraLandingState(MonoIslandCameraSM sm)
		{
			_sm = sm;
		}

		public override void Enter(IslandCameraBaseState lastState, object param = null)
		{
			base.Enter(lastState, param);
			_building = param as MonoIslandBuilding;
			(Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).TriggerFullScreenBlock(false);
		}

		public override void Exit(IslandCameraBaseState nextState)
		{
			_building = null;
		}

		public override void OnTouchUp(Gesture gesture)
		{
			if ((!(gesture.pickedObject != null) || !(gesture.pickedObject == _building.gameObject)) && Singleton<MainUIManager>.Instance.CurrentPageContext != null)
			{
				Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
			}
		}

		public MonoIslandBuilding GetBuilding()
		{
			return _building;
		}
	}
}
