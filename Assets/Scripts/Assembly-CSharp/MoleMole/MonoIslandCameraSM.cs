using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoIslandCameraSM : MonoBehaviour
	{
		public enum E_SwipeType
		{
			Normal = 0,
			Lerp = 1
		}

		public class FingerUtil
		{
			private List<int> _fingerIDList;

			public FingerUtil()
			{
				_fingerIDList = new List<int>();
			}

			public bool AddFinger(int id)
			{
				if (ContainFinger(id))
				{
					return false;
				}
				_fingerIDList.Add(id);
				return true;
			}

			public bool RemoveFinger(int id)
			{
				if (!ContainFinger(id))
				{
					return false;
				}
				_fingerIDList.Remove(id);
				return true;
			}

			public int GetFingerNum()
			{
				return _fingerIDList.Count;
			}

			public bool ContainFinger(int id)
			{
				for (int i = 0; i < _fingerIDList.Count; i++)
				{
					if (_fingerIDList[i] == id)
					{
						return true;
					}
				}
				return false;
			}
		}

		[SerializeField]
		private float _swipe_to_world_speed_ratio;

		[SerializeField]
		private float _swipe_lerp_ratio = 0.08f;

		public float _swipe_damping_ratio = 10f;

		public float _to_landed_speed;

		public float _to_focus_speed;

		public float _backto_base_speed;

		public float _backto_landed_speed;

		public float _pre_landed_time;

		public float _pre_focus_time;

		public float _tension_speed_ratio;

		public float _dragBack_speed;

		[SerializeField]
		private Transform _cameraBasePos;

		[SerializeField]
		private float _up_bound;

		[SerializeField]
		private float _down_bound;

		[SerializeField]
		private float _left_bound;

		[SerializeField]
		private float _right_bound;

		[SerializeField]
		private float _up_bound_inner;

		[SerializeField]
		private float _down_bound_inner;

		[SerializeField]
		private float _left_bound_inner;

		[SerializeField]
		private float _right_bound_inner;

		[SerializeField]
		private Transform _debugLine_left;

		[SerializeField]
		private Transform _debugLine_right;

		[SerializeField]
		private Transform _debugLine_up;

		[SerializeField]
		private Transform _debugLine_bottom;

		private Vector2 _swipeSpeed;

		private Transform _camera;

		private Transform _mainCameraTran;

		private IslandCameraBaseState _currentState;

		private IslandCameraSwipeState _swipeState;

		private IslandCameraDampingState _dampingState;

		private IslandCameraToLandedState _toLandedState;

		private IslandCameraToFocusState _toFocusState;

		private IslandCameraBackToLandedState _backToLandedState;

		private IslandCameraFocusingState _focusingState;

		private IslandCameraBackToBaseState _backToBaseState;

		private IslandCameraDragBackState _dragBackState;

		private IslandCameraLandingState _landingState;

		private Dictionary<E_IslandCameraState, IslandCameraBaseState> _stateDict;

		[HideInInspector]
		public MonoIslandBuildingsUtil _buildingsUtil;

		private Vector3 _deltaPos;

		[SerializeField]
		private float _coverLerpDuration;

		[SerializeField]
		private float _grayCoverAlpha;

		[SerializeField]
		private AnimationCurve _cameraMoveNormalizedCurve;

		private Transform _cover;

		private float _startTimeCoverAlpha;

		private AlphaLerpMaterialPropetyBlock _mpb;

		private E_AlphaLerpDir _coverLerpingDir;

		private Vector3 _vPivot;

		private Vector3 _lookAtPos;

		private Vector3 _lookAtDir;

		private float _lookAtPitch;

		private float _lookAtDist = 150f;

		private IslandCameraNavigatorNew _gyroManager;

		private DragBackPoint _dragBackPoint;

		public E_SwipeNextState _swipeNextState;

		private MonoIslandBuilding _engine;

		private System.Random _random = new System.Random();

		private int _next_frame;

		public E_SwipeType _swipeType;

		private Vector3 _targetPivot;

		private FingerUtil _fingerUtil;

		private void Awake()
		{
			_camera = base.transform;
			_mainCameraTran = base.transform.Find("MainCamera");
			_buildingsUtil = GameObject.Find("IslandWorld").GetComponent<MonoIslandBuildingsUtil>();
			_cover = base.transform.Find("MainCamera/Cover");
			_cameraBasePos.position = base.transform.position;
			_cameraBasePos.rotation = base.transform.rotation;
			_gyroManager = GetComponent<IslandCameraNavigatorNew>();
			_engine = GameObject.Find("IslandWorld/Engine").GetComponent<MonoIslandBuilding>();
		}

		private void Start()
		{
			_fingerUtil = new FingerUtil();
			_swipeState = new IslandCameraSwipeState(this);
			_dampingState = new IslandCameraDampingState(this);
			_toLandedState = new IslandCameraToLandedState(this);
			_toFocusState = new IslandCameraToFocusState(this);
			_backToLandedState = new IslandCameraBackToLandedState(this);
			_focusingState = new IslandCameraFocusingState(this);
			_backToBaseState = new IslandCameraBackToBaseState(this);
			_dragBackState = new IslandCameraDragBackState(this);
			_landingState = new IslandCameraLandingState(this);
			_stateDict = new Dictionary<E_IslandCameraState, IslandCameraBaseState>
			{
				{
					E_IslandCameraState.Swipe,
					_swipeState
				},
				{
					E_IslandCameraState.Damping,
					_dampingState
				},
				{
					E_IslandCameraState.ToLanded,
					_toLandedState
				},
				{
					E_IslandCameraState.ToFocus,
					_toFocusState
				},
				{
					E_IslandCameraState.BackToLanded,
					_backToLandedState
				},
				{
					E_IslandCameraState.Focusing,
					_focusingState
				},
				{
					E_IslandCameraState.BackToBase,
					_backToBaseState
				},
				{
					E_IslandCameraState.DragBack,
					_dragBackState
				},
				{
					E_IslandCameraState.Landing,
					_landingState
				}
			};
			_mpb = new AlphaLerpMaterialPropetyBlock(_cover.GetComponent<MeshRenderer>(), "_Color", 0f, _grayCoverAlpha);
			_mpb.SetAlpha(0f);
			_currentState = null;
			GotoState(E_IslandCameraState.Swipe);
			SetPivot(_cameraBasePos.position);
			_targetPivot = _cameraBasePos.position;
			_lookAtPitch = _cameraBasePos.eulerAngles.x;
			_lookAtDir = _cameraBasePos.forward;
			_dragBackPoint = new DragBackPoint(_cameraBasePos.position);
		}

		public void Update()
		{
			_currentState.Update();
			UpdateCamera();
			UpdateCoverLerping();
		}

		private void UpdateLowFPS()
		{
			if (_next_frame <= 0)
			{
				_next_frame = Time.frameCount;
			}
			if (_next_frame == Time.frameCount)
			{
				for (int i = 0; i < 300000; i++)
				{
					RaycastHit hitInfo;
					Physics.Raycast(Vector3.zero, Vector3.up * _next_frame, out hitInfo, 10f, 1 << InLevelData.PROP_LAYER);
				}
				_next_frame += _random.Next(10, 15);
			}
		}

		private void UpdateCamera()
		{
			_camera.position = GetPivot() + GetGyroOffset();
			_camera.LookAt(GetLookAtPos());
		}

		public Vector3 GetPivot()
		{
			return _vPivot;
		}

		public void SetPivot(Vector3 pos)
		{
			_vPivot = pos;
		}

		public IslandCameraNavigatorNew GetGyroManager()
		{
			return _gyroManager;
		}

		private Vector3 GetGyroOffset()
		{
			return _gyroManager.GetOffset();
		}

		private Vector3 GetLookAtPos()
		{
			_lookAtPos = GetPivot() + GetLookAtDir() * _lookAtDist;
			return _lookAtPos;
		}

		public float GetLookAtPitch()
		{
			return _lookAtPitch;
		}

		public Vector3 GetLookAtDir()
		{
			_lookAtDir = Quaternion.Euler(_lookAtPitch, 0f, 0f) * Vector3.forward;
			return _lookAtDir;
		}

		public void SetLookAtPitch(float pitch)
		{
			_lookAtPitch = pitch;
		}

		public Transform GetCameraBasePos()
		{
			return _cameraBasePos;
		}

		public float GetSwipeLayerY()
		{
			return _cameraBasePos.position.y;
		}

		private void LerpPivot(Vector3 targetPos, Vector3 delta)
		{
			float num = delta.magnitude / Time.deltaTime;
			float value = Time.deltaTime * _swipe_lerp_ratio * (1f + num);
			value = Mathf.Clamp01(value);
			_vPivot = Vector3.Lerp(_vPivot, targetPos, value);
			_targetPivot = targetPos;
		}

		public Vector2 SwipeMoveHandler()
		{
			_deltaPos.x = _swipeSpeed.x;
			_deltaPos.y = 0f;
			_deltaPos.z = _swipeSpeed.y;
			if (_swipeType == E_SwipeType.Normal)
			{
				Vector3 cameraPos = GetPivot() + _deltaPos;
				cameraPos = CheckSwipeCameraPos(cameraPos);
				SetPivot(cameraPos);
			}
			else
			{
				Vector3 cameraPos2 = _targetPivot + _deltaPos;
				cameraPos2 = CheckSwipeCameraPos(cameraPos2);
				LerpPivot(cameraPos2, _deltaPos);
			}
			return _swipeSpeed;
		}

		public void SwipeToWorldSpeed(Vector2 deltaPosition)
		{
			float num = _swipe_to_world_speed_ratio;
			float num2 = _swipe_to_world_speed_ratio;
			int cameraOutInfo = GetCameraOutInfo(_camera.position);
			if ((cameraOutInfo & 1) != 0 && deltaPosition.x > 0f)
			{
				float outRotio = GetOutRotio(E_IslandCameraOut.Left, _camera.position);
				num = _swipe_to_world_speed_ratio * GetRatioCurve(outRotio);
			}
			if ((cameraOutInfo & 4) != 0 && deltaPosition.x < 0f)
			{
				float outRotio2 = GetOutRotio(E_IslandCameraOut.Right, _camera.position);
				num = _swipe_to_world_speed_ratio * GetRatioCurve(outRotio2);
			}
			if ((cameraOutInfo & 8) != 0 && deltaPosition.y > 0f)
			{
				float outRotio3 = GetOutRotio(E_IslandCameraOut.Bottom, _camera.position);
				num2 = _swipe_to_world_speed_ratio * GetRatioCurve(outRotio3);
			}
			if ((cameraOutInfo & 2) != 0 && deltaPosition.y < 0f)
			{
				float outRotio4 = GetOutRotio(E_IslandCameraOut.Top, _camera.position);
				num2 = _swipe_to_world_speed_ratio * GetRatioCurve(outRotio4);
			}
			_swipeSpeed.x = (0f - deltaPosition.x) / (float)((Screen.width <= Screen.height) ? Screen.height : Screen.width) * num;
			_swipeSpeed.y = (0f - deltaPosition.y) / (float)((Screen.width <= Screen.height) ? Screen.height : Screen.width) * num2;
			_swipeSpeed = _swipeSpeed * Time.smoothDeltaTime / Time.deltaTime;
		}

		private float GetRatioCurve(float ratio)
		{
			return (1f - ratio) * (1f - ratio);
		}

		public void ToDampingSpeed()
		{
			int cameraOutInfo = GetCameraOutInfo(_camera.position);
			if (cameraOutInfo > 0)
			{
				_swipeSpeed = Vector2.zero;
				if ((cameraOutInfo & 1) != 0)
				{
					float outRotio = GetOutRotio(E_IslandCameraOut.Left, _camera.position);
					float x = _tension_speed_ratio * outRotio;
					_swipeSpeed.x = x;
				}
				if ((cameraOutInfo & 4) != 0)
				{
					float outRotio2 = GetOutRotio(E_IslandCameraOut.Right, _camera.position);
					float x2 = (0f - _tension_speed_ratio) * outRotio2;
					_swipeSpeed.x = x2;
				}
				if ((cameraOutInfo & 8) != 0)
				{
					float outRotio3 = GetOutRotio(E_IslandCameraOut.Bottom, _camera.position);
					float y = _tension_speed_ratio * outRotio3;
					_swipeSpeed.y = y;
				}
				if ((cameraOutInfo & 2) != 0)
				{
					float outRotio4 = GetOutRotio(E_IslandCameraOut.Top, _camera.position);
					float y2 = (0f - _tension_speed_ratio) * outRotio4;
					_swipeSpeed.y = y2;
				}
			}
			else
			{
				_swipeSpeed = Vector2.Lerp(_swipeSpeed, Vector2.zero, Time.deltaTime * _swipe_damping_ratio);
			}
		}

		public void SetSwipeSpeed(Vector2 _speed)
		{
			_swipeSpeed = _speed;
		}

		public Transform GetCamera()
		{
			return _camera;
		}

		public void GotoState(E_IslandCameraState newStateName, object param = null)
		{
			IslandCameraBaseState state = GetState(newStateName);
			if (_currentState != null)
			{
				_currentState.Exit(state);
			}
			state.Enter(_currentState, param);
			_currentState = state;
		}

		public void CameraToBasePos()
		{
			_camera.position = GetCameraBasePos().position;
			_camera.rotation = GetCameraBasePos().rotation;
		}

		public void ExitFocusing()
		{
			if (_currentState is IslandCameraFocusingState)
			{
				GotoState(E_IslandCameraState.BackToLanded, (_currentState as IslandCameraFocusingState).GetBuilding());
			}
		}

		public void BackToBase()
		{
			MonoIslandBuilding monoIslandBuilding = null;
			if (_currentState == _focusingState)
			{
				monoIslandBuilding = _focusingState.GetBuilding();
			}
			else
			{
				if (_currentState != _landingState)
				{
					return;
				}
				monoIslandBuilding = _landingState.GetBuilding();
			}
			GotoState(E_IslandCameraState.BackToBase, monoIslandBuilding);
		}

		public void ToFocusing()
		{
			if (_currentState == _landingState)
			{
				GotoState(E_IslandCameraState.ToFocus, _landingState.GetBuilding());
			}
		}

		private IslandCameraBaseState GetState(E_IslandCameraState stateName)
		{
			return _stateDict[stateName];
		}

		public DragBackPoint GetDragBackPoint()
		{
			return _dragBackPoint;
		}

		private void Debug_DrawBorderInfo()
		{
			int cameraOutInfo = GetCameraOutInfo(_camera.position);
			_debugLine_right.gameObject.SetActive((cameraOutInfo & 1) != 0);
			_debugLine_left.gameObject.SetActive((cameraOutInfo & 4) != 0);
			_debugLine_up.gameObject.SetActive((cameraOutInfo & 8) != 0);
			_debugLine_bottom.gameObject.SetActive((cameraOutInfo & 2) != 0);
		}

		private void UpdateCoverLerping()
		{
			if (_coverLerpingDir == E_AlphaLerpDir.None)
			{
				return;
			}
			if (_coverLerpingDir == E_AlphaLerpDir.ToLarge && !_cover.gameObject.activeSelf)
			{
				_cover.gameObject.SetActive(true);
			}
			float num = (Time.time - _startTimeCoverAlpha) / _coverLerpDuration;
			if (num > 1f)
			{
				if (_coverLerpingDir == E_AlphaLerpDir.ToLittle)
				{
					_cover.gameObject.SetActive(false);
				}
				TriggerCover(E_AlphaLerpDir.None);
			}
			else
			{
				_mpb.LerpAlpha(num);
			}
		}

		public void TriggerCover(E_AlphaLerpDir dir)
		{
			_coverLerpingDir = dir;
			_mpb.SetDir(dir);
			_startTimeCoverAlpha = Time.time;
		}

		public void TriggerCameraObj(bool enable)
		{
			_mainCameraTran.gameObject.SetActive(enable);
		}

		public float GetLandedSpeedFinal(float dist)
		{
			float magnitude = (_cameraBasePos.position - _engine.GetLandedPos()).magnitude;
			float num = magnitude * 2.5f;
			float b = 2f;
			dist = Mathf.Clamp(dist, magnitude, num);
			float t = (dist - magnitude) / (num - magnitude);
			t = Mathf.Lerp(1f, b, t);
			return _to_landed_speed * t;
		}

		public float GetBackToBaseSpeed(float dist)
		{
			float magnitude = (_cameraBasePos.position - _engine.GetFocusPos()).magnitude;
			float num = magnitude * 2f;
			float b = 2f;
			dist = Mathf.Clamp(dist, magnitude, num);
			float t = (dist - magnitude) / (num - magnitude);
			t = Mathf.Lerp(1f, b, t);
			return _backto_base_speed * t;
		}

		public float GetNomalizedCurvePos(float t)
		{
			return _cameraMoveNormalizedCurve.Evaluate(t);
		}

		public Vector3 CheckSwipeCameraPos(Vector3 cameraPos)
		{
			cameraPos.y = GetSwipeLayerY();
			float max = _cameraBasePos.position.z + _up_bound;
			float min = _cameraBasePos.position.z - _down_bound;
			float min2 = _cameraBasePos.position.x - _left_bound;
			float max2 = _cameraBasePos.position.x + _right_bound;
			cameraPos.x = Mathf.Clamp(cameraPos.x, min2, max2);
			cameraPos.z = Mathf.Clamp(cameraPos.z, min, max);
			return cameraPos;
		}

		public int GetCameraOutInfo(Vector3 cameraPos)
		{
			int num = 0;
			float num2 = _cameraBasePos.position.z + _up_bound_inner;
			float num3 = _cameraBasePos.position.z - _down_bound_inner;
			float num4 = _cameraBasePos.position.x - _left_bound_inner;
			float num5 = _cameraBasePos.position.x + _right_bound_inner;
			if (cameraPos.x < num4)
			{
				num |= 1;
			}
			if (cameraPos.x > num5)
			{
				num |= 4;
			}
			if (cameraPos.z < num3)
			{
				num |= 8;
			}
			if (cameraPos.z > num2)
			{
				num |= 2;
			}
			return num;
		}

		public float GetOutRotio(E_IslandCameraOut type, Vector3 cameraPos)
		{
			float num = _cameraBasePos.position.z + _up_bound;
			float num2 = _cameraBasePos.position.z - _down_bound;
			float num3 = _cameraBasePos.position.x - _left_bound;
			float num4 = _cameraBasePos.position.x + _right_bound;
			float num5 = _cameraBasePos.position.z + _up_bound_inner;
			float num6 = _cameraBasePos.position.z - _down_bound_inner;
			float num7 = _cameraBasePos.position.x - _left_bound_inner;
			float num8 = _cameraBasePos.position.x + _right_bound_inner;
			float result = 0f;
			switch (type)
			{
			case E_IslandCameraOut.Left:
				if (cameraPos.x < num7)
				{
					result = (num7 - cameraPos.x) / (num7 - num3);
					result = Mathf.Clamp01(result);
				}
				break;
			case E_IslandCameraOut.Right:
				if (cameraPos.x > num8)
				{
					result = (cameraPos.x - num8) / (num4 - num8);
					result = Mathf.Clamp01(result);
				}
				break;
			case E_IslandCameraOut.Bottom:
				if (cameraPos.z < num6)
				{
					result = (num6 - cameraPos.z) / (num6 - num2);
					result = Mathf.Clamp01(result);
				}
				break;
			case E_IslandCameraOut.Top:
				if (cameraPos.z > num5)
				{
					result = (cameraPos.z - num5) / (num - num5);
					result = Mathf.Clamp01(result);
				}
				break;
			}
			return result;
		}

		private void OnTouchStart(Gesture gesture)
		{
			bool flag = _fingerUtil.AddFinger(gesture.fingerIndex);
			_currentState.OnTouchStart(gesture);
		}

		private void OnTouchUp(Gesture gesture)
		{
			bool flag = _fingerUtil.RemoveFinger(gesture.fingerIndex);
			_currentState.OnTouchUp(gesture);
		}

		private void OnSwipeStart(Gesture gesture)
		{
		}

		private void OnSwipe(Gesture gesture)
		{
			if (_fingerUtil.GetFingerNum() == 1 && _fingerUtil.ContainFinger(gesture.fingerIndex))
			{
				_currentState.OnSwipe(gesture);
			}
		}

		private void OnSwipeEnd(Gesture gesture)
		{
			if (_fingerUtil.GetFingerNum() == 1 && _fingerUtil.ContainFinger(gesture.fingerIndex))
			{
				_currentState.OnSwipeEnd(gesture);
			}
		}

		private void OnDrag(Gesture gesture)
		{
			if (_fingerUtil.GetFingerNum() == 1 && _fingerUtil.ContainFinger(gesture.fingerIndex))
			{
				_currentState.OnDrag(gesture);
			}
		}

		private void OnDragStart(Gesture gesture)
		{
			if (_fingerUtil.GetFingerNum() == 1 && _fingerUtil.ContainFinger(gesture.fingerIndex))
			{
				_currentState.OnDragStart(gesture);
			}
		}

		private void OnDragEnd(Gesture gesture)
		{
			if (_fingerUtil.GetFingerNum() == 1 && _fingerUtil.ContainFinger(gesture.fingerIndex))
			{
				_currentState.OnDragEnd(gesture);
			}
		}

		public void OnEnable()
		{
			SubscribeEvent();
		}

		public void OnDisable()
		{
			UnsubscribeEvent();
		}

		public void OnDestroy()
		{
			UnsubscribeEvent();
		}

		public void SubscribeEvent()
		{
			EasyTouch.On_TouchStart += OnTouchStart;
			EasyTouch.On_TouchUp += OnTouchUp;
			EasyTouch.On_SwipeStart += OnSwipeStart;
			EasyTouch.On_SwipeEnd += OnSwipeEnd;
			EasyTouch.On_Swipe += OnSwipe;
			EasyTouch.On_Drag += OnDrag;
			EasyTouch.On_DragStart += OnDragStart;
			EasyTouch.On_DragEnd += OnDragEnd;
		}

		public void UnsubscribeEvent()
		{
			EasyTouch.On_TouchStart -= OnTouchStart;
			EasyTouch.On_TouchUp -= OnTouchUp;
			EasyTouch.On_SwipeStart -= OnSwipeStart;
			EasyTouch.On_SwipeEnd -= OnSwipeEnd;
			EasyTouch.On_Swipe -= OnSwipe;
			EasyTouch.On_Drag -= OnDrag;
			EasyTouch.On_DragStart -= OnDragStart;
			EasyTouch.On_DragEnd -= OnDragEnd;
		}
	}
}
