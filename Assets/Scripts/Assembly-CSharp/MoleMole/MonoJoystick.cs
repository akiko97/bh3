using FullInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IDragHandler
	{
		private const int POINTER_NONE = -253;

		private Button _button;

		private RectTransform _imageTrans;

		private float _currentAngle;

		private BaseAvatarInputController _controller;

		private Camera _uiCamera;

		private bool _useVirtualJoyStick;

		private bool _isPointerDown;

		private bool _pointerLeaveTrigger;

		private Vector2 _currentScreenHitPos;

		private float _lastAnalogInputX;

		private float _lastAnalogInputY;

		private bool _updateAfterEnable;

		[ShowInInspector]
		private int _controlPointerID = -253;

		public bool IsMoving { get; private set; }

		public float CurrentAngleV2
		{
			get
			{
				return (!(_currentAngle < -180f)) ? _currentAngle : (_currentAngle + 360f);
			}
		}

		public void Update()
		{
			if (_updateAfterEnable)
			{
				_updateAfterEnable = false;
				OnUnactive();
			}
			if (_isPointerDown && Input.touchCount == 0)
			{
				_controlPointerID = -253;
				_isPointerDown = false;
				_pointerLeaveTrigger = true;
			}
			if (_isPointerDown)
			{
				OnActive(_currentScreenHitPos);
			}
			else if (_pointerLeaveTrigger)
			{
				_pointerLeaveTrigger = false;
				OnUnactive();
			}
		}

		public void InitJoystick(BaseAvatarInputController controller)
		{
			_controller = controller;
			base.gameObject.SetActive(true);
		}

		public void OnDrag(PointerEventData data)
		{
			if (_controlPointerID != -253 && data.pointerId == _controlPointerID && _isPointerDown)
			{
				_currentScreenHitPos = data.position;
				OnActive(_currentScreenHitPos);
			}
		}

		public void OnPointerDown(PointerEventData data)
		{
			if (_controlPointerID == -253)
			{
				_controlPointerID = data.pointerId;
			}
			else if (_controlPointerID != data.pointerId)
			{
				return;
			}
			_isPointerDown = true;
			_currentScreenHitPos = data.position;
		}

		public void OnPointerUp(PointerEventData data)
		{
			if (_controlPointerID != -253 && _controlPointerID == data.pointerId)
			{
				_controlPointerID = -253;
				_isPointerDown = false;
				_pointerLeaveTrigger = true;
			}
		}

		private void Awake()
		{
			_button = GetComponent<Button>();
			_uiCamera = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().GetComponent<Canvas>().worldCamera;
			_imageTrans = base.transform.Find("Image") as RectTransform;
		}

		private void OnEnable()
		{
			_updateAfterEnable = true;
		}

		private void OnDisable()
		{
			_isPointerDown = false;
			_pointerLeaveTrigger = false;
			_currentAngle = 0f;
			_controlPointerID = -253;
			_lastAnalogInputX = 0f;
			_lastAnalogInputY = 0f;
			IsMoving = false;
			ResetGraphicRotate();
		}

		private void OnActive(Vector2 hitPos)
		{
			_currentAngle = SetGraphicRotate(hitPos);
			IsMoving = true;
			_controller.TryMove(IsMoving, CurrentAngleV2);
			_button.image.overrideSprite = _button.spriteState.pressedSprite;
			_useVirtualJoyStick = true;
		}

		private void OnUnactive()
		{
			_isPointerDown = false;
			_pointerLeaveTrigger = false;
			_currentAngle = 0f;
			_controlPointerID = -253;
			_lastAnalogInputX = 0f;
			_lastAnalogInputY = 0f;
			IsMoving = false;
			ResetGraphicRotate();
			_controller.TryMove(IsMoving, CurrentAngleV2);
			_useVirtualJoyStick = false;
		}

		private float SetGraphicRotate(Vector2 hitPos)
		{
			Vector3 vector = RectTransformUtility.WorldToScreenPoint(_uiCamera, base.transform.position);
			float x = hitPos.x - vector.x;
			float y = hitPos.y - vector.y;
			float num = Mathf.Atan2(y, x) * 57.29578f - 90f;
			_imageTrans.SetLocalEulerAnglesZ(num);
			return num;
		}

		private void ResetGraphicRotate()
		{
			_imageTrans.SetLocalEulerAnglesZ(0f);
		}

		private void UpdateRealJoyStick()
		{
			if (Singleton<LevelManager>.Instance.IsPaused())
			{
				return;
			}
			Button component = GetComponent<Button>();
			float axisRaw = Input.GetAxisRaw("Horizontal");
			float axisRaw2 = Input.GetAxisRaw("Vertical");
			if (axisRaw == 0f && axisRaw2 == 0f)
			{
				if (axisRaw == _lastAnalogInputX && axisRaw2 == _lastAnalogInputY)
				{
					return;
				}
				_imageTrans.SetLocalEulerAnglesZ(0f);
				_currentAngle = 0f;
				IsMoving = false;
				_controller.TryMove(IsMoving, CurrentAngleV2);
				component.image.overrideSprite = null;
			}
			else
			{
				if (axisRaw > 0f && axisRaw2 > 0f)
				{
					_currentAngle = -45f;
				}
				else if (axisRaw > 0f && axisRaw2 < 0f)
				{
					_currentAngle = -135f;
				}
				else if (axisRaw > 0f && axisRaw2 == 0f)
				{
					_currentAngle = -90f;
				}
				else if (axisRaw < 0f && axisRaw2 > 0f)
				{
					_currentAngle = 45f;
				}
				else if (axisRaw < 0f && axisRaw2 < 0f)
				{
					_currentAngle = -225f;
				}
				else if (axisRaw < 0f && axisRaw2 == 0f)
				{
					_currentAngle = -270f;
				}
				else if (axisRaw == 0f && axisRaw2 > 0f)
				{
					_currentAngle = 0f;
				}
				else if (axisRaw == 0f && axisRaw2 < 0f)
				{
					_currentAngle = -180f;
				}
				_imageTrans.SetLocalEulerAnglesZ(_currentAngle);
				IsMoving = true;
				_controller.TryMove(IsMoving, CurrentAngleV2);
				component.image.overrideSprite = component.spriteState.pressedSprite;
			}
			_lastAnalogInputX = axisRaw;
			_lastAnalogInputY = axisRaw2;
		}
	}
}
