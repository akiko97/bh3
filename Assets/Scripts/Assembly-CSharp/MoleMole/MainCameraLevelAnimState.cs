using System;
using UnityEngine;

namespace MoleMole
{
	public class MainCameraLevelAnimState : BaseMainCameraState
	{
		private Animation _animation;

		private BaseMainCameraState _nextState;

		private bool _exitTransitionLerp;

		private bool _isFirstFrame;

		private bool _ignoreTimeScale;

		private bool _pauseLevel;

		private MonoSimpleAnimation _simpleAnimationComponent;

		private CameraAnimationCullingType _cullType;

		private Action _levelAnimStartCallback;

		private Action _levelAnimEndCallback;

		private Vector2 _origRadialBlurCenter;

		private float _origRadialBlurStrenth;

		private float _origRadialBlurScatterScale;

		public MainCameraLevelAnimState(MonoMainCamera camera)
			: base(camera)
		{
			_nextState = camera.staticState;
			_exitTransitionLerp = false;
			muteCameraShake = false;
		}

		public override void Enter()
		{
			Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
			_isFirstFrame = true;
			if (_simpleAnimationComponent != null)
			{
				if (_simpleAnimationComponent.initClipZNear > 0f)
				{
					_owner.cameraComponent.nearClipPlane = Mathf.Max(0.01f, _simpleAnimationComponent.initClipZNear);
				}
				if (_simpleAnimationComponent.initFOV > 0f)
				{
					_owner.cameraComponent.fieldOfView = _simpleAnimationComponent.initFOV;
				}
				_simpleAnimationComponent.selfUpdateKeyedRotation = false;
				if (_simpleAnimationComponent.useKeyedDirectionalLightRotation)
				{
					_owner.SetNeedLerpDirectionalLight(false);
				}
			}
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(false, false, true);
			PostFX component = _owner.GetComponent<PostFX>();
			if (component != null)
			{
				_origRadialBlurCenter = component.RadialBlurCenter;
				_origRadialBlurStrenth = component.RadialBlurStrenth;
				_origRadialBlurScatterScale = component.RadialBlurScatterScale;
			}
			if (_pauseLevel)
			{
				Singleton<LevelManager>.Instance.SetPause(true);
			}
			if (_cullType == CameraAnimationCullingType.CullAvatars)
			{
				Singleton<AvatarManager>.Instance.SetAllAvatarVisibility(false);
			}
			if (_levelAnimStartCallback != null)
			{
				_levelAnimStartCallback();
			}
		}

		private void DoExit()
		{
			if (_exitTransitionLerp)
			{
				_owner.TransitWithLerp(_nextState, 0.3f);
				return;
			}
			_owner.Transit(_nextState);
			Singleton<EventManager>.Instance.FireEvent(new EvtCamearaAnimState(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), EvtCamearaAnimState.State.Finish));
		}

		public override void Update()
		{
			if (_simpleAnimationComponent != null)
			{
				_simpleAnimationComponent.SyncRotation();
			}
			if (_isFirstFrame)
			{
				SyncByAnimation();
				_isFirstFrame = false;
				return;
			}
			if (_animation != null && _animation.isPlaying)
			{
				SyncByAnimation();
				if (_ignoreTimeScale)
				{
					if (!_pauseLevel)
					{
						if (Singleton<LevelManager>.Instance.IsPaused())
						{
							_animation[_animation.clip.name].speed = 0f;
						}
						else
						{
							_animation[_animation.clip.name].speed = 1f;
						}
					}
				}
				else
				{
					_animation[_animation.clip.name].speed = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				}
			}
			else
			{
				DoExit();
			}
			Debug.DrawLine(_owner.transform.position, _owner.transform.position + _owner.transform.forward * 10f, Color.red, 3f);
		}

		public override void Exit()
		{
			if (_levelAnimEndCallback != null)
			{
				_levelAnimEndCallback();
			}
			if (_animation != null)
			{
				_animation.Stop();
				UnityEngine.Object.Destroy(_animation.gameObject);
			}
			_owner.cameraComponent.nearClipPlane = _owner.originalNearClip;
			_owner.cameraComponent.fieldOfView = _owner.originalFOV;
			if (_simpleAnimationComponent.useKeyedRadialBlur)
			{
				_simpleAnimationComponent.useKeyedRadialBlur = false;
				PostFX component = _owner.GetComponent<PostFX>();
				if (component != null)
				{
					component.RadialBlurCenter = _origRadialBlurCenter;
					component.RadialBlurStrenth = _origRadialBlurStrenth;
					component.RadialBlurScatterScale = _origRadialBlurScatterScale;
				}
			}
			if (_pauseLevel)
			{
				Singleton<LevelManager>.Instance.SetPause(false);
			}
			if (_cullType == CameraAnimationCullingType.CullAvatars)
			{
				Singleton<AvatarManager>.Instance.SetAllAvatarVisibility(true);
			}
			Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(true, false, true);
			_owner.SetNeedLerpDirectionalLight(true);
		}

		public void SetupFollowAnim(Animation anim, bool ignoreTimeScale, bool pauseLevel, CameraAnimationCullingType cullType = CameraAnimationCullingType.CullNothing, Action levelAnimStartCallback = null, Action levelAnimEndCallback = null)
		{
			_animation = anim;
			_pauseLevel = pauseLevel;
			_ignoreTimeScale = ignoreTimeScale;
			_simpleAnimationComponent = anim.GetComponent<MonoSimpleAnimation>();
			_levelAnimStartCallback = levelAnimStartCallback;
			_levelAnimEndCallback = levelAnimEndCallback;
			_cullType = cullType;
		}

		public void SetNextState(BaseMainCameraState nextState, bool exitTransitionLerp)
		{
			_nextState = nextState;
			_exitTransitionLerp = exitTransitionLerp;
		}

		private void SyncByAnimation()
		{
			cameraPosition = _animation.transform.position;
			cameraForward = _animation.transform.forward;
			cameraFOV = _owner.originalFOV;
			if (_simpleAnimationComponent != null)
			{
				if (_simpleAnimationComponent.useKeyedFOV)
				{
					cameraFOV = _simpleAnimationComponent.keyedFOV;
				}
				if (_simpleAnimationComponent.useKeyedDirectionalLightRotation)
				{
					_owner.directionalLight.transform.rotation = _simpleAnimationComponent.GetLightRotation();
				}
			}
			if (_simpleAnimationComponent != null && _simpleAnimationComponent.useKeyedRadialBlur)
			{
				PostFX component = _owner.GetComponent<PostFX>();
				if (!(component == null))
				{
					component.RadialBlurCenter = _simpleAnimationComponent.radialBlurCenter;
					component.RadialBlurScatterScale = _simpleAnimationComponent.radialBlurScatterScale;
					component.RadialBlurStrenth = _simpleAnimationComponent.radialBlurStrenth;
				}
			}
		}
	}
}
