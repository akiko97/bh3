using UnityEngine;

namespace MoleMole
{
	public class IslandCameraDampingState : IslandCameraBaseState
	{
		public IslandCameraDampingState(MonoIslandCameraSM sm)
		{
			_sm = sm;
		}

		public override void Update()
		{
			_sm.ToDampingSpeed();
			if (Miscs.IsAlmostZero(_sm.SwipeMoveHandler().magnitude))
			{
				_sm.SetSwipeSpeed(Vector2.zero);
				_sm.GotoState(E_IslandCameraState.Swipe);
			}
		}

		public override void Enter(IslandCameraBaseState lastState, object param = null)
		{
			base.Enter(lastState, param);
		}

		public override void Exit(IslandCameraBaseState nextState)
		{
		}

		public override void OnTouchStart(Gesture gesture)
		{
			_sm.GotoState(E_IslandCameraState.Swipe, gesture);
		}

		public override void OnTouchUp(Gesture gesture)
		{
		}
	}
}
