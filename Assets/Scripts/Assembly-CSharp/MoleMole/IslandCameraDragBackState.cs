using UnityEngine;

namespace MoleMole
{
	public class IslandCameraDragBackState : IslandCameraBaseState
	{
		private DragBackPoint _dragBackPoint;

		private Vector3 _startPos;

		private Vector3 _toPos;

		private float _startTime;

		private float _totalTime;

		public IslandCameraDragBackState(MonoIslandCameraSM sm)
		{
			_sm = sm;
		}

		public override void Update()
		{
			if (_totalTime <= 0f)
			{
				_sm.SetPivot(_toPos);
				_sm.GotoState(E_IslandCameraState.Swipe);
			}
			float num = (Time.time - _startTime) / _totalTime;
			if (num >= 1f)
			{
				_sm.SetPivot(_toPos);
				_sm.GotoState(E_IslandCameraState.Swipe);
			}
			else
			{
				float value = 2f * num - num * num;
				value = Mathf.Clamp(value, 0f, 1f);
				_sm.SetPivot(Vector3.Lerp(_startPos, _toPos, value));
			}
		}

		public override void Enter(IslandCameraBaseState lastState, object param = null)
		{
			base.Enter(lastState, param);
			_dragBackPoint = param as DragBackPoint;
			_startPos = _sm.GetPivot();
			_toPos = _dragBackPoint._finalPos;
			_startTime = Time.time;
			_totalTime = (_toPos - _startPos).magnitude / _sm._dragBack_speed;
		}

		public override void Exit(IslandCameraBaseState nextState)
		{
		}

		public override void OnTouchStart(Gesture gesture)
		{
			_sm.GotoState(E_IslandCameraState.Swipe, gesture);
		}
	}
}
