namespace MoleMole
{
	public class IslandCameraFocusingState : IslandCameraBaseState
	{
		private MonoIslandBuilding _building;

		public IslandCameraFocusingState(MonoIslandCameraSM sm)
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

		public MonoIslandBuilding GetBuilding()
		{
			return _building;
		}
	}
}
