using System;
using System.Collections.Generic;
using System.Diagnostics;
using FullInspector;
using UnityEngine;

namespace MoleMole
{
	[fiInspectorOnly]
	public sealed class MonoMainCamera : BaseMonoCamera
	{
		private class CameraShakeEntry
		{
			public float timer;

			public float duration;

			public float range;

			public float angle;

			public int stepFrame;

			public int stepFrameCounter;

			public bool isAngleDirected;

			public override string ToString()
			{
				return string.Format("duration:{0} range:{1} frame:{2}, timer:{3}", duration, range, Time.frameCount, timer);
			}
		}

		private class CameraExposureArgument
		{
			public float timer;

			public float exposureTime;

			public float keepTime;

			public float recoverTime;

			public float maxExposureRate;

			public float currentExposureRate;

			public float originalExposure;

			public float deltaExposureRate;

			public float currentGlareThresRate;

			public float originalGlareThres;

			public float deltaGlareThresRate;
		}

		private enum CameraExposureState
		{
			Idle = 0,
			Exposure = 1,
			Keep = 2,
			Recover = 3
		}

		private enum CameraGlareState
		{
			Idle = 0,
			Glare = 1,
			Keep = 2,
			Recover = 3
		}

		private class CameraGlareArgument
		{
			public float timer;

			public float glareTime;

			public float keepTime;

			public float recoverTime;

			public float targetRate;

			public float deltaRate;

			public CameraGlareState state;

			public static float originalValue;
		}

		private enum CameraDOFCustomState
		{
			Active = 0,
			Inactive = 1,
			Done = 2
		}

		private const float TRANSITION_LERP_SPAN = 0.5f;

		public Light directionalLight;

		private BaseMainCameraState _nextState;

		private BaseMainCameraState _state;

		private int _shakeStepFrame;

		private int _shakeFrameCounter;

		private float _shakeTotalTime;

		private float _shakeTimer;

		private float _shakeRange;

		private float _shakeDirectedRatio;

		private Vector3 _directedShakeOffset;

		private bool _isAlongDirected;

		private Vector3 _calculatedShakeOffset;

		private Quaternion _directionalLightFollowRotation;

		private bool _needUpdateDirectionalLight = true;

		private bool _doingTransitionLerp;

		private int _transitionLerpStep;

		private float _transitionLerpSpeedRatio = 1f;

		private float _transitionLerpTimer;

		private Vector3 _transitionFromPos;

		private Vector3 _transitionFromForward;

		private float _transitionFromFov;

		private Plane[] _frustumPlanes = new Plane[6];

		private bool _muteManualControl;

		private List<CameraShakeEntry> _cameraShakeLs = new List<CameraShakeEntry>();

		private CameraShakeEntry _largestShakeEntryThisFrame;

		private CameraExposureArgument _camExposureArg;

		private CameraExposureState _camExposureState;

		private LinkedList<CameraGlareArgument> _camGlareArgs = new LinkedList<CameraGlareArgument>();

		private AnimationCurve _dofAnimationCurve;

		private float _dofCustomTimer;

		private float _dofCustomDuration;

		private CameraDOFCustomState _dofCustomState = CameraDOFCustomState.Inactive;

		private Color failColor = new Color(0.5f, 0.5f, 0.5f, 1f);

		private Color normalColor = new Color(0.5f, 0.5f, 0.5f, 0f);

		private bool _debugIsNear = true;

		public MainCameraStaticState staticState { get; private set; }

		public MainCameraFollowState followState { get; private set; }

		public MainCameraLevelAnimState levelAnimState { get; private set; }

		public MainCameraAvatarAnimState avatarAnimState { get; private set; }

		public MainCameraCinemaState cinemaState { get; private set; }

		public MainCameraStoryState storyState { get; private set; }

		public float originalNearClip { get; private set; }

		public float originalFOV { get; private set; }

		public Camera cameraComponent { get; private set; }

		public static Vector3 CameraForwardLerp(Vector3 a, Vector3 b, float t)
		{
			a.Normalize();
			b.Normalize();
			Vector3 forward = a;
			a.y = 0f;
			Vector3 forward2 = b;
			b.y = 0f;
			Quaternion a2 = Quaternion.LookRotation(forward);
			Quaternion b2 = Quaternion.LookRotation(forward2);
			Quaternion quaternion = Quaternion.Slerp(a2, b2, t);
			float a3 = Mathf.Asin(a.y);
			float b3 = Mathf.Asin(b.y);
			float y = Mathf.Lerp(a3, b3, t);
			return Quaternion.Euler(0f, y, 0f) * quaternion * Vector3.forward;
		}

		public override void Awake()
		{
			base.Awake();
			cameraComponent = GetComponent<Camera>();
			directionalLight = base.transform.Find("DirLight").GetComponent<Light>();
			_directionalLightFollowRotation = directionalLight.transform.rotation;
		}

		public void Init(uint runtimeID)
		{
			Init(1u, runtimeID);
			originalNearClip = cameraComponent.nearClipPlane;
			originalFOV = cameraComponent.fieldOfView;
			staticState = new MainCameraStaticState(this);
			followState = new MainCameraFollowState(this);
			levelAnimState = new MainCameraLevelAnimState(this);
			avatarAnimState = new MainCameraAvatarAnimState(this);
			cinemaState = new MainCameraCinemaState(this);
			storyState = new MainCameraStoryState(this);
			_state = staticState;
			_state.Enter();
			_camExposureState = CameraExposureState.Idle;
			_camExposureArg = new CameraExposureArgument
			{
				timer = 0f,
				exposureTime = 0f,
				keepTime = 0f,
				recoverTime = 0f,
				maxExposureRate = 1f,
				currentExposureRate = 1f,
				originalExposure = GetComponent<PostFXBase>().Exposure,
				deltaExposureRate = 0f,
				currentGlareThresRate = 1f,
				originalGlareThres = GetComponent<PostFXBase>().glareThreshold,
				deltaGlareThresRate = 0f
			};
			CameraGlareArgument.originalValue = GetComponent<PostFXBase>().glareIntensity;
			Singleton<EventManager>.Instance.CreateActor<MainCameraActor>(this);
		}

		public void Transit(BaseMainCameraState to)
		{
			if (_nextState != null)
			{
				ClearNextState();
			}
			if (_nextState != null)
			{
			}
			if (IsInTransitionLerp())
			{
				InterruptTransitionLerp(_nextState);
			}
			else
			{
				_nextState = to;
			}
		}

		public void TransitWithLerp(BaseMainCameraState to, float transitionLerpSpeedRatio = 1f)
		{
			if (IsInTransitionLerp())
			{
				InterruptTransitionLerp(_nextState);
				return;
			}
			_nextState = to;
			_transitionLerpStep = 0;
			_transitionLerpTimer = 0f;
			_transitionLerpSpeedRatio = transitionLerpSpeedRatio;
			_doingTransitionLerp = true;
		}

		public bool IsInTransitionLerp()
		{
			return _doingTransitionLerp;
		}

		public void InterruptTransitionLerp(BaseMainCameraState nextState)
		{
			_doingTransitionLerp = false;
			_nextState = null;
			Transit(nextState);
		}

		public void ClearNextState()
		{
			_doingTransitionLerp = false;
			_nextState = null;
		}

		private int SeekLargestShakeEntryIndex()
		{
			int result = -1;
			float num = 0f;
			for (int i = 0; i < _cameraShakeLs.Count; i++)
			{
				if (_cameraShakeLs[i] != null)
				{
					CameraShakeEntry cameraShakeEntry = _cameraShakeLs[i];
					if (cameraShakeEntry.range * (cameraShakeEntry.timer / cameraShakeEntry.duration) > num)
					{
						result = i;
					}
				}
			}
			return result;
		}

		public void ActShakeEffect(float time, float range, float angle, int stepFrame, bool isAngleDirected, bool clearPreviousShake)
		{
			if (_largestShakeEntryThisFrame != null && range <= _largestShakeEntryThisFrame.range)
			{
				return;
			}
			CameraShakeEntry cameraShakeEntry = new CameraShakeEntry();
			cameraShakeEntry.timer = time;
			cameraShakeEntry.duration = time;
			cameraShakeEntry.range = range;
			cameraShakeEntry.angle = angle;
			cameraShakeEntry.stepFrame = stepFrame;
			cameraShakeEntry.stepFrameCounter = 1;
			cameraShakeEntry.isAngleDirected = isAngleDirected;
			CameraShakeEntry cameraShakeEntry2 = cameraShakeEntry;
			int num = _cameraShakeLs.SeekAddPosition();
			_cameraShakeLs[num] = cameraShakeEntry2;
			int num2 = SeekLargestShakeEntryIndex();
			if (num2 == num || clearPreviousShake)
			{
				if (clearPreviousShake)
				{
					_cameraShakeLs.Clear();
				}
				SetupShake(cameraShakeEntry2);
			}
			_largestShakeEntryThisFrame = cameraShakeEntry2;
		}

		private void SetupShake(CameraShakeEntry shakeEntry)
		{
			_shakeTimer = shakeEntry.timer;
			_shakeTotalTime = shakeEntry.duration;
			_shakeStepFrame = shakeEntry.stepFrame;
			_shakeFrameCounter = shakeEntry.stepFrameCounter;
			_shakeRange = shakeEntry.range * _state.cameraShakeRatio;
			_shakeDirectedRatio = ((!shakeEntry.isAngleDirected) ? 0f : 0.8f);
			if (shakeEntry.isAngleDirected)
			{
				_directedShakeOffset = Quaternion.AngleAxis(shakeEntry.angle, Vector3.forward) * Vector3.right;
				_directedShakeOffset *= shakeEntry.range * _shakeDirectedRatio;
				_isAlongDirected = true;
			}
			else
			{
				_directedShakeOffset = Vector3.zero;
				_isAlongDirected = false;
			}
		}

		public void ActExposureEffect(float exposureTime, float keepTime, float recoverTime, float maxRate)
		{
			_camExposureArg.exposureTime = exposureTime;
			_camExposureArg.keepTime = keepTime;
			_camExposureArg.recoverTime = recoverTime;
			_camExposureArg.maxExposureRate = Mathf.Max(_camExposureArg.maxExposureRate, maxRate);
			_camExposureArg.deltaExposureRate = (maxRate - _camExposureArg.currentExposureRate) / _camExposureArg.exposureTime;
			_camExposureArg.deltaGlareThresRate = _camExposureArg.currentGlareThresRate / _camExposureArg.exposureTime;
			_camExposureArg.timer = exposureTime;
			_camExposureState = CameraExposureState.Exposure;
		}

		public void ActGlareEffect(float glareTime, float keepTime, float recoverTime, float targetRate)
		{
			CameraGlareArgument cameraGlareArgument = new CameraGlareArgument();
			cameraGlareArgument.glareTime = glareTime;
			cameraGlareArgument.keepTime = keepTime;
			cameraGlareArgument.recoverTime = recoverTime;
			cameraGlareArgument.targetRate = targetRate;
			cameraGlareArgument.deltaRate = 0f;
			cameraGlareArgument.timer = cameraGlareArgument.glareTime;
			cameraGlareArgument.state = CameraGlareState.Glare;
			_camGlareArgs.AddLast(cameraGlareArgument);
		}

		public void SetFailPostFX(bool enabled)
		{
			PostFXBase component = GetComponent<PostFXBase>();
			if (component != null)
			{
				if (enabled)
				{
					component.enabled = true;
				}
				else
				{
					component.enabled = component.originalEnabled;
				}
				component.SepiaColor = ((!enabled) ? normalColor : failColor);
			}
		}

		private void LateUpdate()
		{
			if (_nextState != null)
			{
				if (_doingTransitionLerp)
				{
					if (_transitionLerpStep == 0)
					{
						_transitionFromPos = _state.cameraPosition;
						_transitionFromForward = _state.cameraForward;
						_transitionFromFov = _state.cameraFOV;
						_state.Exit();
						_state.SetActive(false);
						_state = _nextState;
						_state.Enter();
						_state.SetActive(true);
						_state.Update();
						_transitionLerpTimer += Time.unscaledDeltaTime * _transitionLerpSpeedRatio;
						float num = _transitionLerpTimer / 0.5f;
						num *= 2f - num;
						base.transform.position = Vector3.Lerp(_transitionFromPos, _state.cameraPosition, num);
						base.transform.forward = CameraForwardLerp(_transitionFromForward, _state.cameraForward, Time.deltaTime * 5f * _transitionLerpSpeedRatio);
						cameraComponent.fieldOfView = Mathf.Lerp(_transitionFromFov, _state.cameraFOV, num);
						_transitionLerpStep = 1;
					}
					else if (_transitionLerpStep == 1)
					{
						_state.Update();
						_transitionLerpTimer += Time.unscaledDeltaTime * _transitionLerpSpeedRatio;
						float num2 = _transitionLerpTimer / 0.5f;
						if (num2 > 1f)
						{
							base.transform.position = _state.cameraPosition;
							base.transform.forward = _state.cameraForward;
							cameraComponent.fieldOfView = _state.cameraFOV;
							_nextState = null;
							_doingTransitionLerp = false;
						}
						else
						{
							num2 *= 2f - num2;
							base.transform.position = Vector3.Lerp(_transitionFromPos, _state.cameraPosition, num2);
							base.transform.forward = CameraForwardLerp(_transitionFromForward, _state.cameraForward, num2);
							cameraComponent.fieldOfView = Mathf.Lerp(_transitionFromFov, _state.cameraFOV, num2);
						}
					}
				}
				else
				{
					_state.Exit();
					_state.SetActive(false);
					_state = _nextState;
					_state.Enter();
					_state.SetActive(true);
					_nextState = null;
					_state.Update();
					_cameraTrans.position = _state.cameraPosition;
					_cameraTrans.forward = _state.cameraForward;
					cameraComponent.fieldOfView = _state.cameraFOV;
				}
			}
			else
			{
				_state.Update();
				_cameraTrans.position = _state.cameraPosition;
				_cameraTrans.forward = _state.cameraForward;
				cameraComponent.fieldOfView = _state.cameraFOV;
			}
			float num3 = ((!_state.lerpDirectionalLight) ? 8f : 1f);
			if (_needUpdateDirectionalLight)
			{
				_directionalLightFollowRotation = Quaternion.Slerp(_directionalLightFollowRotation, _cameraTrans.rotation, Time.deltaTime * num3);
				directionalLight.transform.rotation = Quaternion.Euler(45f, _directionalLightFollowRotation.eulerAngles.y, _directionalLightFollowRotation.eulerAngles.z);
			}
			UpdateCameraShake();
			UpdateCameraExposure();
			UpdateCameraGlare();
			UpdateCameraDofCustom();
			GeometryUtilityUser.CalculateFrustumPlanes(cameraComponent, ref _frustumPlanes);
		}

		private void UpdateCameraShake()
		{
			float timeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
			if (timeScale == 0f)
			{
				return;
			}
			if (_shakeTimer > 0f)
			{
				_shakeFrameCounter--;
				if (_shakeFrameCounter == 0)
				{
					_shakeFrameCounter = _shakeStepFrame;
					Vector3 vector = UnityEngine.Random.insideUnitCircle.normalized * _shakeRange * (1f - _shakeDirectedRatio);
					Vector3 directedShakeOffset = _directedShakeOffset;
					if (!_isAlongDirected)
					{
						directedShakeOffset *= 0.7f;
					}
					Vector3 direction = _directedShakeOffset + vector;
					float num = Mathf.Clamp(_shakeTimer / _shakeTotalTime, 0.2f, 0.9f);
					direction *= num * num;
					Vector3 calculatedShakeOffset = base.transform.TransformDirection(direction);
					_calculatedShakeOffset = calculatedShakeOffset;
					_directedShakeOffset *= -1f;
					_isAlongDirected = !_isAlongDirected;
				}
				if (!_state.muteCameraShake)
				{
					base.transform.position = _calculatedShakeOffset + base.transform.position;
				}
			}
			_shakeTimer -= Time.unscaledDeltaTime;
			bool flag = false;
			for (int i = 0; i < _cameraShakeLs.Count; i++)
			{
				if (_cameraShakeLs[i] != null)
				{
					CameraShakeEntry cameraShakeEntry = _cameraShakeLs[i];
					cameraShakeEntry.timer -= Time.deltaTime * timeScale;
					if (cameraShakeEntry.timer <= 0f)
					{
						_cameraShakeLs[i] = null;
						flag = true;
					}
				}
			}
			if (flag)
			{
				int num2 = SeekLargestShakeEntryIndex();
				if (num2 >= 0)
				{
					SetupShake(_cameraShakeLs[num2]);
				}
			}
			_largestShakeEntryThisFrame = null;
		}

		private void UpdateCameraExposure()
		{
			float timeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
			if (timeScale == 0f || _camExposureState == CameraExposureState.Idle)
			{
				return;
			}
			if (_camExposureState == CameraExposureState.Exposure)
			{
				_camExposureArg.timer -= Time.deltaTime * timeScale;
				_camExposureArg.currentExposureRate += _camExposureArg.deltaExposureRate * Time.deltaTime * timeScale;
				_camExposureArg.currentGlareThresRate -= _camExposureArg.deltaGlareThresRate * Time.deltaTime * timeScale;
				if (_camExposureArg.timer <= 0f)
				{
					_camExposureArg.timer = _camExposureArg.keepTime;
					_camExposureState = CameraExposureState.Keep;
				}
			}
			else if (_camExposureState == CameraExposureState.Keep)
			{
				_camExposureArg.timer -= Time.deltaTime * timeScale;
				if (_camExposureArg.timer <= 0f)
				{
					_camExposureArg.timer = _camExposureArg.recoverTime;
					_camExposureArg.deltaExposureRate = (_camExposureArg.maxExposureRate - 1f) / _camExposureArg.recoverTime;
					_camExposureArg.deltaGlareThresRate = 1f / _camExposureArg.recoverTime;
					_camExposureState = CameraExposureState.Recover;
				}
			}
			else
			{
				_camExposureArg.timer -= Time.deltaTime * timeScale;
				_camExposureArg.currentExposureRate -= _camExposureArg.deltaExposureRate * Time.deltaTime * timeScale;
				_camExposureArg.currentGlareThresRate += _camExposureArg.deltaGlareThresRate * Time.deltaTime * timeScale;
				if (_camExposureArg.timer <= 0f)
				{
					_camExposureArg.currentExposureRate = 1f;
					_camExposureState = CameraExposureState.Idle;
				}
			}
			_camExposureArg.currentExposureRate = Mathf.Clamp(_camExposureArg.currentExposureRate, 1f, _camExposureArg.maxExposureRate);
			_camExposureArg.currentGlareThresRate = Mathf.Clamp(_camExposureArg.currentGlareThresRate, 0f, 1f);
			GetComponent<PostFXBase>().Exposure = _camExposureArg.currentExposureRate * _camExposureArg.originalExposure;
			GetComponent<PostFXBase>().glareThreshold = _camExposureArg.currentGlareThresRate * _camExposureArg.originalGlareThres;
		}

		private void UpdateCameraDofCustom()
		{
			FakeDOF component = GetComponent<FakeDOF>();
			if (component == null)
			{
				return;
			}
			if (_dofCustomState != CameraDOFCustomState.Active)
			{
				if (component.backgroundBlurFactor < 0.01f)
				{
					component.enabled = false;
				}
				return;
			}
			component.enabled = true;
			if (_dofAnimationCurve != null && !(_dofCustomDuration <= 0f) && _dofCustomDuration > 0f && _dofAnimationCurve != null)
			{
				_dofCustomTimer += Time.deltaTime;
				float num = _dofCustomTimer / _dofCustomDuration;
				float num2 = _dofAnimationCurve.Evaluate(num);
				component.backgroundBlurFactor = num2 * 5f;
				if (num > 1f)
				{
					_dofCustomState = CameraDOFCustomState.Done;
				}
			}
		}

		private void UpdateCameraGlare()
		{
			float timeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
			if (timeScale == 0f)
			{
				return;
			}
			float num = 1f;
			LinkedListNode<CameraGlareArgument> linkedListNode = null;
			LinkedListNode<CameraGlareArgument> linkedListNode2 = _camGlareArgs.First;
			while (linkedListNode2 != null)
			{
				linkedListNode = linkedListNode2;
				linkedListNode2 = linkedListNode.Next;
				CameraGlareArgument value = linkedListNode.Value;
				if (value.state == CameraGlareState.Idle)
				{
					_camGlareArgs.Remove(value);
					continue;
				}
				if (value.state == CameraGlareState.Glare)
				{
					value.timer -= Time.deltaTime * timeScale;
					value.deltaRate = (1f - value.timer / value.glareTime) * (value.targetRate - 1f);
					if (value.timer <= 0f)
					{
						value.deltaRate = value.targetRate - 1f;
						value.timer = value.keepTime;
						value.state = CameraGlareState.Keep;
					}
				}
				else if (value.state == CameraGlareState.Keep)
				{
					value.timer -= Time.deltaTime * timeScale;
					if (value.timer <= 0f)
					{
						value.timer = value.recoverTime;
						value.state = CameraGlareState.Recover;
					}
				}
				else
				{
					value.timer -= Time.deltaTime * timeScale;
					value.deltaRate = value.timer / value.recoverTime * (value.targetRate - 1f);
					if (value.timer <= 0f)
					{
						value.deltaRate = 0f;
						value.state = CameraGlareState.Idle;
					}
				}
				num += value.deltaRate;
			}
			GetComponent<PostFXBase>().glareIntensity = num * CameraGlareArgument.originalValue;
		}

		public Vector3 WorldToUIPoint(Vector3 pos)
		{
			Vector3 position = cameraComponent.WorldToScreenPoint(pos);
			Camera component = Singleton<CameraManager>.Instance.GetInLevelUICamera().GetComponent<Camera>();
			position.z = Mathf.Clamp(position.z, component.nearClipPlane, component.farClipPlane);
			return component.ScreenToWorldPoint(position);
		}

		public void FollowLookAtPosition(Vector3 position, bool mute = false, bool force = false)
		{
			if (!force && followState.slowMotionKillState.active)
			{
				followState.slowMotionKillState.SetFollowingLookAtPosition(position, mute);
				return;
			}
			followState.TryRemoveShortState();
			followState.lookAtPositionState.SetLookAtTarget(position, mute);
			followState.AddShortState(followState.lookAtPositionState);
		}

		public void SetFollowAnchorRadius(float anchorRadius)
		{
			float anchorRadius2 = Mathf.Clamp(anchorRadius, 6f, 8.5f);
			followState.anchorRadius = anchorRadius2;
		}

		public void SetFollowRange(MainCameraFollowState.RangeState rangeState, bool force = false)
		{
			if (force || !followState.slowMotionKillState.active)
			{
				followState.TryRemoveShortState();
				followState.rangeTransitState.SetRange(rangeState);
				followState.AddShortState(followState.rangeTransitState);
			}
		}

		public void SetTimedPullZ(float radiusRatio, float elevationAngles, float centerYOffset, float fovOffset, float time, float lerpTimer = 0f, string lerpCurveName = "", bool force = false)
		{
			if (force || !followState.slowMotionKillState.active)
			{
				followState.TryRemoveShortState();
				followState.timedPullZState.SetTimedPullZ(radiusRatio, elevationAngles, centerYOffset, fovOffset, time, lerpTimer, lerpCurveName);
				followState.AddShortState(followState.timedPullZState);
			}
		}

		public void SetSlowMotionKill(ConfigCameraSlowMotionKill config, float distTarget, float distCamera)
		{
			if (followState.slowMotionKillState.active)
			{
				followState.slowMotionKillState.SetSlowMotionKill(config, distTarget, distCamera);
				return;
			}
			followState.TryRemoveShortState();
			followState.slowMotionKillState.SetSlowMotionKill(config, distTarget, distCamera);
			followState.AddShortState(followState.slowMotionKillState);
		}

		public void SetUserDefinedCameraLocateRatio(float cameraLocateRatio)
		{
			followState.isCameraLocateRatioUserDefined = true;
			followState.cameraLocateRatio = cameraLocateRatio;
		}

		public void SetCameraLocateRatio(float cameraLocateRatio)
		{
			followState.cameraLocateRatio = cameraLocateRatio;
		}

		public void SuddenSwitchFollowAvatar(uint avatarID, bool force = false)
		{
			if (force || !followState.slowMotionKillState.active)
			{
				followState.TryRemoveShortState();
				followState.suddenChangeState.SetSuddenChangeTarget(Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(avatarID));
				followState.AddShortState(followState.suddenChangeState);
			}
		}

		public void SuddenRecover()
		{
			if (followState.active)
			{
				followState.TryRemoveShortState();
				followState.AddShortState(followState.suddenRecoverState);
			}
		}

		public void SetMuteManualCameraControl(bool mute)
		{
			_muteManualControl = mute;
			if (mute)
			{
				if (followState.rotateToAvatarFacingState.active)
				{
					followState.TryRemoveShortState();
				}
				if (followState.followAvatarControlledRotate.active)
				{
					followState.TransitBaseState(followState.followAvatarState);
				}
			}
		}

		public void FollowControlledRotateStart()
		{
			if (followState.active && !_muteManualControl)
			{
				if (followState.followAvatarControlledRotate.active)
				{
					followState.followAvatarControlledRotate.SetExitingControl(false);
				}
				else
				{
					followState.TransitBaseState(followState.followAvatarControlledRotate);
				}
				if (followState.rotateToAvatarFacingState.active)
				{
					followState.TryRemoveShortState();
				}
			}
		}

		public void SetCameraFakeDOFCustommed(AnimationCurve cureve, float duration)
		{
			_dofAnimationCurve = cureve;
			_dofCustomTimer = 0f;
			_dofCustomDuration = duration;
			_dofCustomState = CameraDOFCustomState.Active;
		}

		public void SetFollowControledRotationData(Vector2 delta)
		{
			if (followState.active && followState.followAvatarControlledRotate.active && !_muteManualControl)
			{
				followState.followAvatarControlledRotate.SetDragDelta(delta);
			}
		}

		public void SetFollowControledZoomingData(float zoomDelta)
		{
			if (followState.active && followState.followAvatarControlledRotate.active && !_muteManualControl)
			{
				followState.followAvatarControlledRotate.SetZoomDelta(zoomDelta);
			}
		}

		public void FollowControlledRotateEnableExitTimer()
		{
			if (followState.active && followState.followAvatarControlledRotate.active && !_muteManualControl)
			{
				followState.followAvatarControlledRotate.SetExitingControl(true);
			}
		}

		public void FollowControlledRotateStop()
		{
			if (followState.active && followState.followAvatarControlledRotate.active && !_muteManualControl)
			{
				followState.TryToTransitToOtherBaseState(false);
			}
		}

		public void SetRotateToFaceDirection()
		{
			if (followState.active && followState.rotateToAvatarFacingState.CanRotate() && !_muteManualControl)
			{
				followState.AddOrReplaceShortState(followState.rotateToAvatarFacingState);
			}
		}

		public void SetupFollowAvatar(uint avatarID)
		{
			followState.SetupFollowAvatar(avatarID);
		}

		public void PlayAvatarCameraAnimationThenStay(Animation anim, BaseMonoAvatar avatar)
		{
			avatarAnimState.SetupFollowAvatarAnim(anim, avatar);
			if (IsInTransitionLerp())
			{
				InterruptTransitionLerp(avatarAnimState);
			}
			else
			{
				Transit(avatarAnimState);
			}
		}

		public void PlayAvatarCameraAnimationThenTransitToFollow(Animation anim, BaseMonoAvatar avatar, MainCameraFollowState.EnterPolarMode enterPolarMode, bool exitTransitionLerp)
		{
			float polar = 0f;
			if (enterPolarMode == MainCameraFollowState.EnterPolarMode.AlongTargetPolar)
			{
				polar = followState.anchorPolar;
			}
			followState.SetEnterPolarMode(enterPolarMode, polar);
			avatarAnimState.SetupFollowAvatarAnim(anim, avatar);
			avatarAnimState.SetNextState(followState, exitTransitionLerp);
			if (IsInTransitionLerp())
			{
				InterruptTransitionLerp(avatarAnimState);
			}
			else
			{
				Transit(avatarAnimState);
			}
		}

		public void PlayLevelAnimationThenTransitBack(Animation anim, bool ignoreTimeScale, bool enterLerp, bool exitLerp, bool pauseLevel, CameraAnimationCullingType cullType = CameraAnimationCullingType.CullNothing, Action startCallback = null, Action endCallback = null)
		{
			levelAnimState.SetupFollowAnim(anim, ignoreTimeScale, pauseLevel, cullType, startCallback, endCallback);
			levelAnimState.SetNextState((_state != staticState) ? _state : followState, exitLerp);
			if (_state == avatarAnimState)
			{
				avatarAnimState.SetInterrupt();
			}
			if (enterLerp)
			{
				TransitWithLerp(levelAnimState, 0.1f);
			}
			else
			{
				Transit(levelAnimState);
			}
		}

		public void PlayStoryCameraState(int plotID, bool enterLerp = true, bool exitLerp = true, bool needFadeIn = true, bool backFollow = true, bool pauseLevel = false)
		{
			if (_state != storyState)
			{
				MainCameraStoryState mainCameraStoryState = storyState;
				bool backFollow2 = backFollow;
				mainCameraStoryState.SetCurrentPlotSetting(plotID, exitLerp, needFadeIn, backFollow2, pauseLevel);
				if (enterLerp)
				{
					TransitWithLerp(storyState, 0.5f);
				}
				else
				{
					Transit(storyState);
				}
			}
		}

		public void TransitToStatic()
		{
			Transit(staticState);
		}

		public void TransitToFollow()
		{
			Transit(followState);
		}

		public void TransitToCinema(ICinema cinema)
		{
			cinemaState.SetCinema(cinema);
			Transit(cinemaState);
		}

		public void TransitToStory()
		{
			Transit(storyState);
		}

		public void SetNeedLerpDirectionalLight(bool needLerp)
		{
			_needUpdateDirectionalLight = needLerp;
		}

		public int GetCullingMaskByLayers(int[] layers)
		{
			int num = 0;
			foreach (int num2 in layers)
			{
				num |= 1 << num2;
			}
			return num;
		}

		public void SetMainCameraCullingMask(int mask)
		{
			cameraComponent.cullingMask = mask;
		}

		public bool IsEntityVisible(BaseMonoEntity entity)
		{
			Collider component = entity.GetComponent<Collider>();
			if (component != null && component.enabled)
			{
				return GeometryUtility.TestPlanesAABB(_frustumPlanes, component.bounds);
			}
			Vector3 vector = cameraComponent.WorldToViewportPoint(entity.XZPosition);
			return vector.z > 0f && Miscs.IsFloatInRange(vector.x, 0f, 1f) && Miscs.IsFloatInRange(vector.y, 0f, 1f);
		}

		public bool IsEntityVisibleInCustomOffset(BaseMonoEntity entity, float fovOffset, float nearOffset, float farOffset)
		{
			bool flag = false;
			float fieldOfView = cameraComponent.fieldOfView;
			float nearClipPlane = cameraComponent.nearClipPlane;
			float farClipPlane = cameraComponent.farClipPlane;
			cameraComponent.fieldOfView += fovOffset;
			cameraComponent.nearClipPlane += nearOffset;
			cameraComponent.farClipPlane += farOffset;
			Matrix4x4 projectionMatrix = cameraComponent.projectionMatrix;
			Collider component = entity.GetComponent<Collider>();
			if (component != null && component.enabled)
			{
				Plane[] planes = GeometryUtility.CalculateFrustumPlanes(projectionMatrix);
				flag = GeometryUtility.TestPlanesAABB(planes, component.bounds);
			}
			Vector3 vector = cameraComponent.WorldToViewportPoint(entity.XZPosition);
			flag = vector.z > 0f && Miscs.IsFloatInRange(vector.x, 0f, 1f) && Miscs.IsFloatInRange(vector.y, 0f, 1f);
			cameraComponent.fieldOfView = fieldOfView;
			cameraComponent.nearClipPlane = nearClipPlane;
			cameraComponent.farClipPlane = farClipPlane;
			return flag;
		}

		private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
		{
			float value = 2f * near / (right - left);
			float value2 = 2f * near / (top - bottom);
			float value3 = (right + left) / (right - left);
			float value4 = (top + bottom) / (top - bottom);
			float value5 = (0f - (far + near)) / (far - near);
			float value6 = (0f - 2f * far * near) / (far - near);
			float value7 = -1f;
			Matrix4x4 result = default(Matrix4x4);
			result[0, 0] = value;
			result[0, 1] = 0f;
			result[0, 2] = value3;
			result[0, 3] = 0f;
			result[1, 0] = 0f;
			result[1, 1] = value2;
			result[1, 2] = value4;
			result[1, 3] = 0f;
			result[2, 0] = 0f;
			result[2, 1] = 0f;
			result[2, 2] = value5;
			result[2, 3] = value6;
			result[3, 0] = 0f;
			result[3, 1] = 0f;
			result[3, 2] = value7;
			result[3, 3] = 0f;
			return result;
		}

		public int GetVisibleMonstersCount()
		{
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			int num = 0;
			for (int i = 0; i < allMonsters.Count; i++)
			{
				if (IsEntityVisible(allMonsters[i]))
				{
					num++;
				}
			}
			return num;
		}

		public int GetVisibleMonstersCountWithOffset(float fovOffset, float nearOffset, float farOffset)
		{
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			int num = 0;
			for (int i = 0; i < allMonsters.Count; i++)
			{
				if (IsEntityVisibleInCustomOffset(allMonsters[i], fovOffset, nearOffset, farOffset))
				{
					num++;
				}
			}
			return num;
		}

		[Conditional("NG_HSOD_DEBUG")]
		[Conditional("UNITY_EDITOR")]
		private void DebugUpdate()
		{
			if (Input.GetKeyUp(KeyCode.U))
			{
				if (followState.active)
				{
					FollowLookAtPosition(Vector3.zero);
				}
			}
			else if (Input.GetKeyUp(KeyCode.Y))
			{
				if (followState.active)
				{
					if (_debugIsNear)
					{
						SetFollowRange(MainCameraFollowState.RangeState.Far);
						_debugIsNear = false;
					}
					else
					{
						SetFollowRange(MainCameraFollowState.RangeState.Near);
						_debugIsNear = true;
					}
				}
			}
			else if (Input.GetKeyUp(KeyCode.I))
			{
				if (followState.active)
				{
					SetTimedPullZ(0.8f, 0f, 0f, -10f, 1.5f, 1f, "BinlaoTest");
				}
			}
			else if (Input.GetKeyUp(KeyCode.O))
			{
				if (followState.active)
				{
					SuddenSwitchFollowAvatar(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
				}
			}
			else if (Input.GetKeyUp(KeyCode.P))
			{
				Singleton<LevelDesignManager>.Instance.PlayCameraAnimationOnEnv("Ver1_12_1", false, true, false);
			}
			else if (Input.GetKeyUp(KeyCode.F2))
			{
				ActShakeEffect(0.1f, 0.5f, 90f, 3, true, false);
			}
			else if (Input.GetKeyUp(KeyCode.F3))
			{
				ActShakeEffect(10f, 0.2f, 0f, 3, true, false);
			}
			else if (Input.GetKeyUp(KeyCode.R))
			{
				SuddenRecover();
			}
			else if (Input.GetKeyUp(KeyCode.Z))
			{
				BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
				if (currentPageContext != null && currentPageContext is InLevelMainPageContext)
				{
					InLevelMainPageContext inLevelMainPageContext = currentPageContext as InLevelMainPageContext;
					inLevelMainPageContext.SetInLevelMainPageActive(true);
				}
			}
			else if (Input.GetKeyUp(KeyCode.X))
			{
				BasePageContext currentPageContext2 = Singleton<MainUIManager>.Instance.CurrentPageContext;
				if (currentPageContext2 != null && currentPageContext2 is InLevelMainPageContext)
				{
					InLevelMainPageContext inLevelMainPageContext2 = currentPageContext2 as InLevelMainPageContext;
					inLevelMainPageContext2.SetInLevelMainPageActive(false);
				}
			}
		}
	}
}
