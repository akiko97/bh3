using UnityEngine;

namespace MoleMole
{
	public class IslandCameraBackToLandedState : IslandCameraBaseState
	{
		private Vector3 _startPos;

		private Vector3 _finalPos;

		private float _startPitch;

		private float _finalPitch;

		private float _startTime;

		private float _totalTime;

		private MonoIslandBuilding _building;

		public IslandCameraBackToLandedState(MonoIslandCameraSM sm)
		{
			_sm = sm;
		}

		public override void Update()
		{
			if (_totalTime <= 0f)
			{
				_sm.SetPivot(_finalPos);
				_sm.SetLookAtPitch(_finalPitch);
				_sm.GotoState(E_IslandCameraState.Swipe);
				return;
			}
			float num = (Time.time - _startTime) / _totalTime;
			if (num >= 1f)
			{
				_sm.SetPivot(_finalPos);
				_sm.SetLookAtPitch(_finalPitch);
				_sm.GotoState(E_IslandCameraState.Swipe);
			}
			else
			{
				float value = 2f * num - num * num;
				value = Mathf.Clamp(value, 0f, 1f);
				_sm.SetPivot(Vector3.Lerp(_startPos, _finalPos, value));
				_sm.SetLookAtPitch(Mathf.Lerp(_startPitch, _finalPitch, value));
			}
		}

		public override void Enter(IslandCameraBaseState lastState, object param = null)
		{
			base.Enter(lastState, param);
			_building = param as MonoIslandBuilding;
			if (_building != null)
			{
				_startPos = _building.GetFocusPos();
				_finalPos = _building.GetLandedPos();
				_startPitch = _building.GetFocusPitch();
				_finalPitch = _sm.GetCameraBasePos().eulerAngles.x;
				_startTime = Time.time;
				_totalTime = (_finalPos - _startPos).magnitude / _sm._backto_landed_speed;
			}
		}

		public override void Exit(IslandCameraBaseState nextState)
		{
			_building = null;
			if (_sm != null && _sm.GetGyroManager() != null)
			{
				_sm.GetGyroManager().SetEnable(true);
			}
		}
	}
}
