using UnityEngine;

namespace MoleMole
{
	public class IslandCameraToFocusState : IslandCameraBaseState
	{
		private Vector3 _startPos;

		private Vector3 _focusPos;

		private float _startPitch;

		private float _focusPitch;

		private float _startTime;

		private float _totalTime;

		private bool _fire_prefocus;

		private MonoIslandBuilding _building;

		public IslandCameraToFocusState(MonoIslandCameraSM sm)
		{
			_sm = sm;
		}

		public override void Update()
		{
			if (_totalTime <= 0f)
			{
				_sm.SetPivot(_focusPos);
				_sm.SetLookAtPitch(_focusPitch);
				_sm.GotoState(E_IslandCameraState.Focusing, _building);
				return;
			}
			float num = (Time.time - _startTime) / _totalTime;
			if (num >= 1f)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraFocus, _building));
				_sm.SetPivot(_focusPos);
				_sm.SetLookAtPitch(_focusPitch);
				_sm.GotoState(E_IslandCameraState.Focusing, _building);
				return;
			}
			float value = 2f * num - num * num;
			value = Mathf.Clamp(value, 0f, 1f);
			_sm.SetPivot(Vector3.Lerp(_startPos, _focusPos, value));
			_sm.SetLookAtPitch(Mathf.Lerp(_startPitch, _focusPitch, value));
			if (!_fire_prefocus && _startTime + _totalTime - Time.time < _sm._pre_focus_time)
			{
				_fire_prefocus = true;
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraPreFocus, _building));
			}
		}

		public override void Enter(IslandCameraBaseState lastState, object param = null)
		{
			base.Enter(lastState, param);
			_building = param as MonoIslandBuilding;
			if (_building != null)
			{
				_startPos = _building.GetLandedPos();
				_focusPos = _building.GetFocusPos();
				_startPitch = _sm.GetLookAtPitch();
				_focusPitch = _building.GetFocusPitch();
				_startTime = Time.time;
				_totalTime = (_focusPos - _startPos).magnitude / _sm._to_focus_speed;
				_fire_prefocus = false;
				(Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).TriggerFullScreenBlock(true);
			}
		}

		public override void Exit(IslandCameraBaseState nextState)
		{
			(Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).TriggerFullScreenBlock(false);
		}
	}
}
