using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	public class MonoViewPointTouch : MonoBehaviour
	{
		private enum CameraControlType
		{
			None = 0,
			OnePoint = 1,
			TwoPoint = 2
		}

		private class CameraControllPointer
		{
			public int pointerId;

			public bool isDrag;

			public Vector2 enterPoint;

			public Vector2 offsetPoint;
		}

		private const float VIEWPOINT_DRAG_MOVEMENT_THRESHOLD = 0.06f;

		private const float ZOOMING_DRAG_MOVEMENT_THRESHOLD = 20f;

		private const float ZOOMING_SCALE_FACTOR = 4f;

		private Dictionary<int, CameraControllPointer> _cameraControlPointers = new Dictionary<int, CameraControllPointer>();

		private CameraControlType _preControlType;

		private CameraControlType _currentControlType;

		private Vector2 _dragOffset = Vector2.zero;

		private float _screenWidthInInch;

		private float _screenHeightInInch;

		private void Start()
		{
			if (GraphicsSettingUtil._originScreenResolution.width > 0)
			{
				_screenWidthInInch = (float)GraphicsSettingUtil._originScreenResolution.width / Screen.dpi;
				_screenHeightInInch = (float)GraphicsSettingUtil._originScreenResolution.height / Screen.dpi;
			}
			else
			{
				_screenWidthInInch = (float)Screen.width / Screen.dpi;
				_screenHeightInInch = (float)Screen.height / Screen.dpi;
			}
		}

		public void OnViewPointPanelPointerDown(BaseEventData baseData)
		{
			PointerEventData pointerEventData = (PointerEventData)baseData;
			if (!_cameraControlPointers.ContainsKey(pointerEventData.pointerId))
			{
				CameraControllPointer cameraControllPointer = new CameraControllPointer();
				cameraControllPointer.pointerId = pointerEventData.pointerId;
				cameraControllPointer.isDrag = false;
				cameraControllPointer.enterPoint = pointerEventData.position;
				cameraControllPointer.offsetPoint = pointerEventData.delta;
				_cameraControlPointers.Add(cameraControllPointer.pointerId, cameraControllPointer);
				RefreshControlType();
			}
		}

		public void OnViewPointPanelPointerUp(BaseEventData baseData)
		{
			PointerEventData pointerEventData = (PointerEventData)baseData;
			if (!_cameraControlPointers.ContainsKey(pointerEventData.pointerId))
			{
				return;
			}
			CameraControllPointer cameraControllPointer = _cameraControlPointers[pointerEventData.pointerId];
			if (_currentControlType == CameraControlType.OnePoint)
			{
				if (cameraControllPointer.isDrag)
				{
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateEnableExitTimer();
					if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDamageStasticsPlugin>() && Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDamageStasticsPlugin>().isUpdating)
					{
						Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDamageStasticsPlugin>().AddScreenRotateTimes();
					}
				}
				else
				{
					if (Singleton<CameraManager>.Instance.controlledRotateKeepManual)
					{
						Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStop();
					}
					if (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning && Singleton<MonsterManager>.Instance.LivingMonsterCount() > 0 && Singleton<AvatarManager>.Instance.GetLocalAvatar().IsAlive())
					{
						Singleton<AvatarManager>.Instance.GetLocalAvatar().SelectTarget();
						Singleton<AvatarManager>.Instance.GetLocalAvatar().ClearAttackTargetTimed(1.2f);
					}
					else
					{
						Singleton<CameraManager>.Instance.GetMainCamera().SetRotateToFaceDirection();
					}
				}
			}
			_cameraControlPointers.Remove(pointerEventData.pointerId);
			RefreshControlType();
			if (_currentControlType == CameraControlType.None)
			{
				_dragOffset = Vector2.zero;
			}
		}

		public void OnViewPointPanelDrag(BaseEventData baseData)
		{
			PointerEventData pointerEventData = (PointerEventData)baseData;
			if (!_cameraControlPointers.ContainsKey(pointerEventData.pointerId))
			{
				return;
			}
			CameraControllPointer cameraControllPointer = _cameraControlPointers[pointerEventData.pointerId];
			cameraControllPointer.offsetPoint = pointerEventData.delta;
			CameraControllPointer cameraControllPointer2 = cameraControllPointer;
			CameraControlType currentTouchType = GetCurrentTouchType();
			switch (currentTouchType)
			{
			case CameraControlType.TwoPoint:
			{
				foreach (KeyValuePair<int, CameraControllPointer> cameraControlPointer in _cameraControlPointers)
				{
					if (cameraControlPointer.Key != pointerEventData.pointerId)
					{
						cameraControllPointer2 = cameraControlPointer.Value;
					}
				}
				float num4 = Vector2.Distance(cameraControllPointer.enterPoint, cameraControllPointer2.enterPoint);
				float num5 = Vector2.Distance(cameraControllPointer.offsetPoint + cameraControllPointer.enterPoint, cameraControllPointer2.offsetPoint + cameraControllPointer2.enterPoint);
				float num6 = num4 - num5;
				float followControledZoomingData = num6 * Time.deltaTime * 4f;
				if (!cameraControllPointer.isDrag && Mathf.Abs(num6) > 20f)
				{
					cameraControllPointer.isDrag = true;
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
				}
				if (cameraControllPointer.isDrag)
				{
					_dragOffset = pointerEventData.delta;
					Singleton<CameraManager>.Instance.GetMainCamera().SetFollowControledZoomingData(followControledZoomingData);
				}
				return;
			}
			case CameraControlType.OnePoint:
				if (_preControlType != CameraControlType.None)
				{
					break;
				}
				if (!cameraControllPointer.isDrag)
				{
					float num = (pointerEventData.position.x - cameraControllPointer.enterPoint.x) / (float)Screen.width * _screenWidthInInch;
					float num2 = (pointerEventData.position.y - cameraControllPointer.enterPoint.y) / (float)Screen.height * _screenHeightInInch;
					float num3 = num * num + num2 * num2;
					if (num3 > 0.0036f)
					{
						cameraControllPointer.isDrag = true;
						Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
					}
				}
				if (cameraControllPointer.isDrag)
				{
					Vector2 delta = pointerEventData.delta;
					delta.x = delta.x / (float)Screen.width * _screenWidthInInch;
					delta.y = delta.y / (float)Screen.height * _screenHeightInInch;
					Singleton<CameraManager>.Instance.GetMainCamera().SetFollowControledRotationData(delta);
				}
				return;
			}
			if (currentTouchType == CameraControlType.OnePoint && _preControlType == CameraControlType.TwoPoint)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().SetFollowControledRotationData(pointerEventData.delta - _dragOffset);
			}
		}

		public void OnViewPointPanelInitializePotentialDrag(BaseEventData baseData)
		{
			PointerEventData pointerEventData = (PointerEventData)baseData;
			pointerEventData.useDragThreshold = false;
		}

		private CameraControlType GetCurrentTouchType()
		{
			switch (_cameraControlPointers.Count)
			{
			case 1:
				return CameraControlType.OnePoint;
			case 2:
				return CameraControlType.TwoPoint;
			default:
				return CameraControlType.None;
			}
		}

		private void RefreshControlType()
		{
			_preControlType = _currentControlType;
			_currentControlType = GetCurrentTouchType();
		}
	}
}
