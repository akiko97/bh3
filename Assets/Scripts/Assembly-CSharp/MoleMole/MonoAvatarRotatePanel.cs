using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	public class MonoAvatarRotatePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public enum AvatarModelAutoRotateType
		{
			RotateToBack = 0,
			RotateToFront = 1,
			RotateToOrigin = 2
		}

		private const float _deltaTimePerFrame = 0.016f;

		private const float _stopRotateThreshold = 0.001f;

		private const float _autoRotateThreshold = 0.001f;

		private AvatarDataItem _avatarDataItem;

		public float maxRotateSpeed = 20f;

		public float speedLerpRatio = 15f;

		public int lastFrameCountForInertia = 5;

		[NonSerialized]
		public bool enableDrawGizmos = true;

		[NonSerialized]
		public bool enableManualRotate = true;

		private bool _isRotateWhenDragOutPanel = true;

		private float _rotateSpeedRatio = 100f;

		private float _currentRotateSpeed;

		private bool _isPointerDown;

		private bool _isPointerInPanel;

		private bool _rotateRight;

		private List<float> _lastSeveralFramesSpeed = new List<float>();

		private bool _inSlowDown;

		private bool _inAutoRotate;

		private Vector3 _autoRotateTargetDirection;

		private Transform _avatarModel;

		public float maxAutoRotateSpeed = 0.5f;

		public float minAutoRotateSpeed = 0.03f;

		public float autoRotateSpeedLerpRatio = 10f;

		private float _currentAutoRotateSpeed;

		public void SetupView(AvatarDataItem avatarDataItem)
		{
			_avatarDataItem = avatarDataItem;
			BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
			if (sceneCanvas is MonoMainCanvas)
			{
				_avatarModel = ((MonoMainCanvas)sceneCanvas).avatar3dModelContext.GetAvatarById(_avatarDataItem.avatarID);
			}
			else
			{
				_avatarModel = ((MonoTestUI)sceneCanvas).avatar3dModelContext.GetAvatarById(_avatarDataItem.avatarID);
			}
		}

		public void StartAutoRotateModel(AvatarModelAutoRotateType rotateType, MiscData.PageInfoKey pageKey = MiscData.PageInfoKey.AvatarDetailPage, string tabName = "Default")
		{
			switch (rotateType)
			{
			case AvatarModelAutoRotateType.RotateToFront:
			{
				GameObject gameObject2 = GameObject.Find("MainCamera");
				Vector3 forward2 = gameObject2.transform.forward;
				_autoRotateTargetDirection = new Vector3(0f - forward2.x, 0f, 0f - forward2.z);
				break;
			}
			case AvatarModelAutoRotateType.RotateToBack:
			{
				GameObject gameObject = GameObject.Find("MainCamera");
				Vector3 forward = gameObject.transform.forward;
				_autoRotateTargetDirection = new Vector3(forward.x, 0f, forward.z);
				break;
			}
			case AvatarModelAutoRotateType.RotateToOrigin:
			{
				ConfigAvatarShowInfo avatarShowInfo = UIUtil.GetAvatarShowInfo(_avatarDataItem, pageKey, tabName);
				Quaternion identity = Quaternion.identity;
				identity.eulerAngles = avatarShowInfo.Avatar.EulerAngle;
				_autoRotateTargetDirection = identity * Vector3.forward;
				break;
			}
			}
			_currentAutoRotateSpeed = maxAutoRotateSpeed;
			_inAutoRotate = true;
		}

		public void OnDrag(PointerEventData data)
		{
			if (!_inAutoRotate && _isPointerDown && enableManualRotate && (_isRotateWhenDragOutPanel || _isPointerInPanel))
			{
				Vector2 delta = data.delta;
				Vector2 right = Vector2.right;
				float num = Vector2.Dot(right, delta);
				float dpi = Screen.dpi;
				float f = num / dpi;
				_rotateRight = ((num > 0f) ? true : false);
				_currentRotateSpeed = Mathf.Abs(f) * _rotateSpeedRatio;
				if (_currentRotateSpeed > maxRotateSpeed)
				{
					_currentRotateSpeed = maxRotateSpeed;
				}
				AddRotateSpeedToList(_currentRotateSpeed);
				if (_rotateRight)
				{
					_avatarModel.Rotate(Vector3.up, 0f - _currentRotateSpeed);
				}
				else
				{
					_avatarModel.Rotate(Vector3.up, _currentRotateSpeed);
				}
			}
		}

		private void AddRotateSpeedToList(float rotateSpeed)
		{
			if (_lastSeveralFramesSpeed.Count < lastFrameCountForInertia)
			{
				_lastSeveralFramesSpeed.Add(rotateSpeed);
				return;
			}
			_lastSeveralFramesSpeed.Add(rotateSpeed);
			_lastSeveralFramesSpeed.RemoveAt(0);
		}

		public void OnPointerDown(PointerEventData data)
		{
			_isPointerDown = true;
			ResetStatus();
		}

		public void OnPointerUp(PointerEventData data)
		{
			_isPointerDown = false;
			_currentRotateSpeed = SampleLastSeveralFrameRotateSpeed();
			if (CheckBeginStopGradually(data.position))
			{
				_inSlowDown = true;
			}
		}

		public void OnPointerEnter(PointerEventData data)
		{
			_isPointerInPanel = true;
		}

		public void OnPointerExit(PointerEventData data)
		{
			_isPointerInPanel = false;
			if (!_isRotateWhenDragOutPanel)
			{
				ResetStatus();
			}
		}

		private void Update()
		{
			DoStopGradually();
			DoAutoRotate();
		}

		private void DoStopGradually()
		{
			if (!CheckNeedStopGradually())
			{
				return;
			}
			float value = 0.016f * speedLerpRatio;
			_currentRotateSpeed = Mathf.Lerp(_currentRotateSpeed, 0f, Mathf.Clamp01(value));
			TryToStopSlowDown();
			if (_inSlowDown)
			{
				if (_rotateRight)
				{
					_avatarModel.Rotate(Vector3.up, 0f - _currentRotateSpeed);
				}
				else
				{
					_avatarModel.Rotate(Vector3.up, _currentRotateSpeed);
				}
			}
		}

		private bool CheckNeedStopGradually()
		{
			return enableManualRotate && !_inAutoRotate && _inSlowDown && _currentRotateSpeed > 0f;
		}

		private void TryToStopSlowDown()
		{
			if (_inSlowDown && Mathf.Abs(_currentRotateSpeed) < 0.001f)
			{
				_inSlowDown = false;
			}
		}

		private void OnDrawGizmos()
		{
		}

		private void DrawAvatarRotatePanelGizmos()
		{
			RectTransform component = GetComponent<RectTransform>();
			Vector3[] array = new Vector3[4];
			component.GetWorldCorners(array);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(array[0], array[1]);
			Gizmos.DrawLine(array[1], array[2]);
			Gizmos.DrawLine(array[2], array[3]);
			Gizmos.DrawLine(array[3], array[0]);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(array[0], 0.05f);
			Gizmos.DrawSphere(array[1], 0.05f);
			Gizmos.DrawSphere(array[2], 0.05f);
			Gizmos.DrawSphere(array[3], 0.05f);
		}

		private bool CheckBeginStopGradually(Vector2 pointerPosition)
		{
			if (!_isPointerDown && _currentRotateSpeed > 0f)
			{
				if (!_isRotateWhenDragOutPanel)
				{
					return _isPointerInPanel;
				}
				return true;
			}
			return false;
		}

		private void ResetStatus()
		{
			_currentRotateSpeed = 0f;
			_inSlowDown = false;
			_lastSeveralFramesSpeed.Clear();
		}

		private float SampleLastSeveralFrameRotateSpeed()
		{
			if (_lastSeveralFramesSpeed.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			foreach (float item in _lastSeveralFramesSpeed)
			{
				float num2 = item;
				num += num2;
			}
			return num / (float)_lastSeveralFramesSpeed.Count;
		}

		private bool CheckNeedAutoRotate()
		{
			return _inAutoRotate;
		}

		private void TryToStopAutoRotate()
		{
			if (_inAutoRotate && Mathf.Abs(Vector3.Dot(_avatarModel.forward, _autoRotateTargetDirection) - 1f) < 0.001f)
			{
				_inAutoRotate = false;
			}
		}

		private void DoAutoRotate()
		{
			if (CheckNeedAutoRotate())
			{
				float value = 0.016f * autoRotateSpeedLerpRatio;
				_currentAutoRotateSpeed = Mathf.Lerp(_currentAutoRotateSpeed, minAutoRotateSpeed, Mathf.Clamp01(value));
				Vector3 forward = Vector3.RotateTowards(_avatarModel.forward, _autoRotateTargetDirection, _currentAutoRotateSpeed, 0f);
				_avatarModel.forward = forward;
				TryToStopAutoRotate();
			}
		}
	}
}
