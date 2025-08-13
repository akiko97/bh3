using UnityEngine;

namespace MoleMole
{
	public class IslandCameraSwipeState : IslandCameraBaseState
	{
		private GameObject _selectObj;

		private int _fingerIndex = -2;

		private bool _bSwipe = true;

		public IslandCameraSwipeState(MonoIslandCameraSM sm)
		{
			_sm = sm;
			_bSwipe = true;
		}

		public override void Update()
		{
		}

		public override void Enter(IslandCameraBaseState lastState, object param = null)
		{
			base.Enter(lastState, param);
			if (param != null)
			{
				Gesture gesture = param as Gesture;
				if (gesture != null)
				{
					OnTouchStart(gesture);
				}
			}
		}

		public override void Exit(IslandCameraBaseState nextState)
		{
			_selectObj = null;
			_fingerIndex = -2;
		}

		public override void OnSwipe(Gesture gesture)
		{
			if (_bSwipe)
			{
				_sm.SwipeToWorldSpeed(gesture.deltaPosition);
				_sm.SwipeMoveHandler();
			}
		}

		public override void OnSwipeEnd(Gesture gesture)
		{
			if (_bSwipe)
			{
				if (_sm._swipeNextState == E_SwipeNextState.SwipeToDamping)
				{
					_sm.GotoState(E_IslandCameraState.Damping);
				}
				else if (_sm._swipeNextState == E_SwipeNextState.SwipeToDragBack)
				{
					_sm.GotoState(E_IslandCameraState.DragBack, _sm.GetDragBackPoint());
				}
			}
		}

		public override void OnDrag(Gesture gesture)
		{
			if (_bSwipe)
			{
				_sm.SwipeToWorldSpeed(gesture.deltaPosition);
				_sm.SwipeMoveHandler();
			}
		}

		public override void OnDragEnd(Gesture gesture)
		{
			if (_bSwipe)
			{
				if (_sm._swipeNextState == E_SwipeNextState.SwipeToDamping)
				{
					_sm.GotoState(E_IslandCameraState.Damping);
				}
				else if (_sm._swipeNextState == E_SwipeNextState.SwipeToDragBack)
				{
					_sm.GotoState(E_IslandCameraState.DragBack, _sm.GetDragBackPoint());
				}
			}
		}

		public override void OnTouchStart(Gesture gesture)
		{
			if (gesture.pickedObject != null)
			{
				_selectObj = gesture.pickedObject;
				_fingerIndex = gesture.fingerIndex;
			}
		}

		public override void OnTouchUp(Gesture gesture)
		{
			if (_selectObj != null && _selectObj == gesture.pickedObject && _fingerIndex == gesture.fingerIndex)
			{
				MonoIslandBuilding component = _selectObj.GetComponent<MonoIslandBuilding>();
				if (component != null)
				{
					_sm.GotoState(E_IslandCameraState.ToLanded, component);
				}
			}
		}
	}
}
