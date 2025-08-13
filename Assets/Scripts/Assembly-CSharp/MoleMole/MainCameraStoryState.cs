using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MainCameraStoryState : BaseMainCameraState
	{
		public const string DEFAULT_RENDER_TEXTURE_PATH = "Rendering/Texture/TestRenderTexture";

		public const float DEFAULT_FADE_DURATION = 0.5f;

		private BaseMainCameraState _nextState;

		private bool _exitTransitionLerp;

		private bool _backFollowState = true;

		private bool _quitWithFadeIn = true;

		private bool _pauseLevel;

		private int _currentPlotID;

		public float anchorRadius = 6f;

		public BaseMonoAvatar avatar;

		public float yaw;

		public float pitch;

		public float yOffset = 1f;

		public float xOffset;

		public float fov = 60f;

		public float yawOffset;

		public float pitchOffset;

		private Gyroscope _gyro;

		private Vector3 _baseGravity;

		private Quaternion _baseAttitude;

		public float ParallexRange = 5f;

		public float ParallexSensitivity = 0.1f;

		public float ParallexBoundHardness = 0.5f;

		private bool blurDataLoaded;

		public AnimationCurve blurEnterCurve;

		public AnimationCurve blurExitCurve;

		public float blurEnterDuration;

		public float blurExitDuration;

		private uint _storyScreenID;

		private int originalCullingMask;

		public MainCameraStoryState(MonoMainCamera camera)
			: base(camera)
		{
			LoadBlurData();
		}

		public override void Enter()
		{
			Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(false);
			avatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			SetOtherAvatarsVisibility(false);
			avatar.PlayState("StandBy");
			avatar.SetLocomotionBool("IsStoryMode", true);
			anchorRadius = avatar.config.StoryCameraSetting.anchorRadius;
			yaw = avatar.config.StoryCameraSetting.yaw;
			pitch = avatar.config.StoryCameraSetting.pitch;
			yOffset = avatar.config.StoryCameraSetting.yOffset;
			xOffset = avatar.config.StoryCameraSetting.xOffset;
			fov = avatar.config.StoryCameraSetting.fov;
			Vector3 vector = Vector3.Cross(avatar.FaceDirection, Vector3.up);
			cameraForward = Quaternion.AngleAxis(yaw, Vector3.up) * avatar.FaceDirection;
			cameraForward = Quaternion.AngleAxis(pitch, vector) * cameraForward;
			cameraPosition = avatar.XZPosition - cameraForward * anchorRadius + Vector3.up * yOffset + vector * xOffset;
			cameraFOV = fov;
			_storyScreenID = Singleton<DynamicObjectManager>.Instance.CreateStoryScreen(avatar.GetRuntimeID(), "StoryScreen", avatar.XZPosition + avatar.FaceDirection, avatar.FaceDirection, _currentPlotID);
			avatar.SetLocomotionBool("IsStoryMode", true);
			Singleton<LevelDesignManager>.Instance.SetMuteAvatarVoice(true);
			MonoStoryScreen monoStoryScreen = (MonoStoryScreen)Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(_storyScreenID);
			if (monoStoryScreen != null)
			{
				monoStoryScreen.onOpenAnimationChange = (Action<bool>)Delegate.Combine(monoStoryScreen.onOpenAnimationChange, new Action<bool>(OnOpenAnimationChange));
			}
			if (blurDataLoaded)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().SetCameraFakeDOFCustommed(blurEnterCurve, blurEnterDuration);
			}
			_gyro = Input.gyro;
			_gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
			SetAllOtherDynamicObjectsAndEfffectsVisibility(false);
			_baseAttitude = _gyro.attitude;
			_baseGravity = _gyro.gravity;
			if (_pauseLevel)
			{
				Singleton<LevelManager>.Instance.SetPause(true);
			}
		}

		private void SetAllOtherDynamicObjectsAndEfffectsVisibility(bool visible)
		{
			Singleton<EffectManager>.Instance.SetAllAliveEffectPause(!visible);
			Singleton<DynamicObjectManager>.Instance.SetDynamicObjectsVisibilityExept<MonoStoryScreen>(visible);
		}

		private void SetStoryModeCullingMaskEnable(bool enable)
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (!(mainCamera != null))
			{
				return;
			}
			PostFXWithResScale component = mainCamera.GetComponent<PostFXWithResScale>();
			int[] layers = new int[8] { 0, 4, 8, 9, 12, 23, 24, 26 };
			int cullingMaskByLayers = mainCamera.GetCullingMaskByLayers(layers);
			if (component != null && component.isActiveAndEnabled)
			{
				if (enable)
				{
					originalCullingMask = component.cullingMask;
					component.cullingMask = cullingMaskByLayers;
				}
				else
				{
					component.cullingMask = originalCullingMask;
				}
			}
			else if (enable)
			{
				originalCullingMask = mainCamera.cameraComponent.cullingMask;
				mainCamera.SetMainCameraCullingMask(cullingMaskByLayers);
			}
			else
			{
				mainCamera.SetMainCameraCullingMask(originalCullingMask);
			}
		}

		private void LoadBlurData()
		{
			string filePath = "FakeDOF/DOFEnterPlot";
			ConfigCameraFakeDOF configCameraFakeDOF = ConfigUtil.LoadConfig<ConfigCameraFakeDOF>(filePath);
			blurEnterCurve = configCameraFakeDOF.Curve;
			blurEnterDuration = configCameraFakeDOF.Duration;
			string filePath2 = "FakeDOF/DOFExitPlot";
			ConfigCameraFakeDOF configCameraFakeDOF2 = ConfigUtil.LoadConfig<ConfigCameraFakeDOF>(filePath2);
			blurExitCurve = configCameraFakeDOF2.Curve;
			blurExitDuration = configCameraFakeDOF2.Duration;
			blurDataLoaded = true;
		}

		private float GetFixedEulerAngle(float angle)
		{
			float num = angle;
			if (num > 360f)
			{
				num -= 360f;
			}
			if (num < 0f)
			{
				num += 360f;
			}
			return (!(num > 180f)) ? num : (num - 360f);
		}

		public override void Update()
		{
			if (_gyro != null)
			{
				yawOffset = Mathf.Lerp(yawOffset, (_baseGravity.x - _gyro.gravity.x) * ParallexRange, ParallexSensitivity);
				pitchOffset = Mathf.Lerp(pitchOffset, (_baseGravity.y - _gyro.gravity.y) * ParallexRange, ParallexSensitivity);
			}
			Vector3 vector = Vector3.Cross(avatar.FaceDirection, Vector3.up);
			cameraForward = Quaternion.AngleAxis(yaw + yawOffset, Vector3.up) * avatar.FaceDirection;
			cameraForward = Quaternion.AngleAxis(pitch + pitchOffset, vector) * cameraForward;
			cameraPosition = avatar.XZPosition - cameraForward * anchorRadius + Vector3.up * yOffset + vector * xOffset;
			cameraFOV = fov;
		}

		public void QuitStoryStateAsDefault()
		{
			if (_exitTransitionLerp)
			{
				bool backFollowState = _backFollowState;
				QuitStoryStateWithLerp(false, 1f, backFollowState);
			}
			else
			{
				QuitStoryStateWithFade(0.5f, _backFollowState, _quitWithFadeIn);
			}
		}

		public void QuitStoryStateWithLerp(bool instant = false, float lerpRatio = 1f, bool backFollow = true)
		{
			ClearStoryScreen();
			if (backFollow)
			{
				TransitToFollow(instant, lerpRatio);
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtStoryState(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), EvtStoryState.State.Finish));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnPlotFinished));
		}

		public void QuitStoryStateWithFade(float duration, bool backFollow = true, bool needFadeIn = false)
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel(duration, false,fadeEndCallback: delegate
			{
				ClearStoryScreen();
				if (backFollow)
				{
					TransitToFollow(true);
				}
				if (needFadeIn)
				{
					Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(duration,fadeEndCallback: delegate
					{
						Singleton<EventManager>.Instance.FireEvent(new EvtStoryState(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), EvtStoryState.State.Finish));
						Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnPlotFinished));
					});
				}
				else
				{
					Singleton<EventManager>.Instance.FireEvent(new EvtStoryState(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), EvtStoryState.State.Finish));
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnPlotFinished));
				}
			});
		}

		public void SetCurrentPlotSetting(int plotID, bool lerpOut = true, bool needFadeIn = true, bool backFollow = true, bool pauseLevel = false)
		{
			_currentPlotID = plotID;
			_exitTransitionLerp = lerpOut;
			_backFollowState = backFollow;
			_quitWithFadeIn = needFadeIn;
			_pauseLevel = pauseLevel;
		}

		public void TransitToFollow(bool instant, float lerpRatio = 1f)
		{
			SetNextState(_owner.followState, !instant);
			if (_exitTransitionLerp)
			{
				_owner.TransitWithLerp(_nextState, lerpRatio);
			}
			else
			{
				_owner.Transit(_nextState);
			}
		}

		public void StartQuit()
		{
			MonoStoryScreen monoStoryScreen = (MonoStoryScreen)Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(_storyScreenID);
			if (monoStoryScreen != null)
			{
				monoStoryScreen.StartDie();
			}
		}

		private void OnOpenAnimationChange(bool openState)
		{
			if (!openState)
			{
				MonoStoryScreen monoStoryScreen = (MonoStoryScreen)Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(_storyScreenID);
				if (monoStoryScreen != null)
				{
					monoStoryScreen.onOpenAnimationChange = (Action<bool>)Delegate.Remove(monoStoryScreen.onOpenAnimationChange, new Action<bool>(OnOpenAnimationChange));
				}
				QuitStoryStateAsDefault();
			}
		}

		private void ClearStoryScreen()
		{
			avatar.SetLocomotionBool("IsStoryMode", false);
			Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(true);
		}

		private void SetOtherAvatarsVisibility(bool visible)
		{
			List<BaseMonoAvatar> list = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars().FindAll((BaseMonoAvatar x) => !Singleton<AvatarManager>.Instance.IsLocalAvatar(x.GetRuntimeID()));
			foreach (BaseMonoAvatar item in list)
			{
				Singleton<AvatarManager>.Instance.SetAvatarVisibility(visible, item);
				if (!visible)
				{
					item.PushTimeScale(0f, 7);
				}
				else
				{
					item.PopTimeScale(7);
				}
			}
		}

		private void DoExit(float exitDuration = 0.5f)
		{
			SetNextState(_owner.followState, true);
			if (_exitTransitionLerp)
			{
				_owner.TransitWithLerp(_nextState, 1f);
			}
			else
			{
				_owner.Transit(_nextState);
			}
		}

		public void SetNextState(BaseMainCameraState nextState, bool exitTransitionLerp)
		{
			_nextState = nextState;
			_exitTransitionLerp = exitTransitionLerp;
		}

		public override void Exit()
		{
			avatar.SetLocomotionBool("IsStoryMode", false);
			SetOtherAvatarsVisibility(true);
			SetAllOtherDynamicObjectsAndEfffectsVisibility(true);
			if (blurDataLoaded)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().SetCameraFakeDOFCustommed(blurExitCurve, blurExitDuration);
			}
			if (Singleton<LevelManager>.Instance.IsPaused())
			{
				Singleton<LevelManager>.Instance.SetPause(false);
			}
		}
	}
}
