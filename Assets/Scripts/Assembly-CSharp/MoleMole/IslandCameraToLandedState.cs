using UnityEngine;

namespace MoleMole
{
	public class IslandCameraToLandedState : IslandCameraBaseState
	{
		private Vector3 _startPos;

		private Vector3 _landedPos;

		private float _startPitch;

		private float _landedPitch;

		private float _startTime;

		private float _totalTime;

		private bool _fire_prelanded;

		private MonoIslandBuilding _building;

		public IslandCameraToLandedState(MonoIslandCameraSM sm)
		{
			_sm = sm;
		}

		public override void Update()
		{
			if (_totalTime <= 0f)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraPreLanded, _building));
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraLanded, _building));
				_sm.TriggerCover(E_AlphaLerpDir.ToLarge);
				_building.SetRenderQueue(E_IslandRenderQueue.Front);
				_building.TriggerHighLight(E_AlphaLerpDir.ToLarge);
				(Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).SetBuildingEffect(_building, false);
				_sm.SetPivot(_landedPos);
				_sm.SetLookAtPitch(_landedPitch);
				_sm.GotoState(E_IslandCameraState.Landing, _building);
				return;
			}
			float num = (Time.time - _startTime) / _totalTime;
			if (num >= 1f)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraLanded, _building));
				_sm.SetPivot(_landedPos);
				_sm.SetLookAtPitch(_landedPitch);
				_sm.GotoState(E_IslandCameraState.Landing, _building);
				return;
			}
			float nomalizedCurvePos = _sm.GetNomalizedCurvePos(num);
			nomalizedCurvePos = Mathf.Clamp(nomalizedCurvePos, 0f, 1f);
			_sm.SetPivot(Vector3.Lerp(_startPos, _landedPos, nomalizedCurvePos));
			_sm.SetLookAtPitch(Mathf.Lerp(_startPitch, _landedPitch, nomalizedCurvePos));
			if (!_fire_prelanded && _startTime + _totalTime - Time.time < _sm._pre_landed_time)
			{
				_fire_prelanded = true;
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraPreLanded, _building));
				_sm.TriggerCover(E_AlphaLerpDir.ToLarge);
				_building.SetRenderQueue(E_IslandRenderQueue.Front);
				_building.TriggerHighLight(E_AlphaLerpDir.ToLarge);
				(Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).SetBuildingEffect(_building, false);
			}
		}

		public override void Enter(IslandCameraBaseState lastState, object param = null)
		{
			base.Enter(lastState, param);
			_building = param as MonoIslandBuilding;
			if (!(_building != null) || !(_sm != null))
			{
				return;
			}
			_startPos = _sm.GetPivot();
			_landedPos = _building.GetLandedPos();
			_startTime = Time.time;
			float magnitude = (_landedPos - _startPos).magnitude;
			_totalTime = magnitude / _sm.GetLandedSpeedFinal(magnitude);
			_fire_prelanded = false;
			_startPitch = _sm.GetLookAtPitch();
			_landedPitch = _building.GetLandedPitch();
			if ((bool)_sm && _sm.GetGyroManager() != null)
			{
				_sm.GetGyroManager().SetEnable(false);
			}
			if (Singleton<MainUIManager>.Instance != null)
			{
				MonoIslandUICanvas monoIslandUICanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas;
				if (monoIslandUICanvas != null)
				{
					monoIslandUICanvas.TriggerFullScreenBlock(true);
				}
			}
		}

		public override void Exit(IslandCameraBaseState nextState)
		{
			_building = null;
			if (Singleton<MainUIManager>.Instance != null)
			{
				MonoIslandUICanvas monoIslandUICanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas;
				if (monoIslandUICanvas != null)
				{
					monoIslandUICanvas.TriggerFullScreenBlock(false);
				}
			}
		}
	}
}
