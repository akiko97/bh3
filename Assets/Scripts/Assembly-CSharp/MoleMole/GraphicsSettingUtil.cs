using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class GraphicsSettingUtil
	{
		public static Resolution _originScreenResolution;

		public static bool _UsingNativeResolution = true;

		public static int _UsingResolutionX;

		public static int _UsingResolutionY;

		private static bool _hasSettingScreenResolution;

		public static Action<bool> onPostFXChanged;

		public static void ApplyResolution(Dictionary<ResolutionQualityGrade, int> resolutionPercentage, ResolutionQualityGrade resolutionQuality, int recommendResX, int recommendResY)
		{
			int currentScreenResolutionWidth = 0;
			int currentScreenResolutionHeight = 0;
			SetScreenResolution(resolutionQuality, ref currentScreenResolutionWidth, ref currentScreenResolutionHeight);
			int num = resolutionPercentage[resolutionQuality];
			int resX = Mathf.RoundToInt((float)(currentScreenResolutionWidth * num) / 100f);
			int resY = Mathf.RoundToInt((float)(currentScreenResolutionHeight * num) / 100f);
			if (resolutionQuality == ResolutionQualityGrade.Middle || resolutionQuality == ResolutionQualityGrade.Low)
			{
				GetResolutionKeepScale(recommendResX, recommendResY, currentScreenResolutionWidth, currentScreenResolutionHeight, ref resX, ref resY);
				if (resolutionQuality == ResolutionQualityGrade.Low)
				{
					resX = Mathf.RoundToInt((float)(resX * num) / 100f);
					resY = Mathf.RoundToInt((float)(resY * num) / 100f);
				}
			}
			_UsingNativeResolution = resX == currentScreenResolutionWidth && resY == currentScreenResolutionHeight;
			_UsingResolutionX = resX;
			_UsingResolutionY = resY;
			PostFXWithResScale postFXWithResScale = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
			if (postFXWithResScale != null && postFXWithResScale.enabled)
			{
				postFXWithResScale.CameraResWidth = resX;
				postFXWithResScale.CameraResHeight = resY;
			}
			else
			{
				Screen.SetResolution(resX, resY, Screen.fullScreen);
			}
			GraphicsUtils.RebindAllRenderTexturesToCamera();
		}

		private static void GetResolutionKeepScale(int recommendResX, int recommendResY, int currentScreenResolutionWidth, int currentScreenResolutionHeight, ref int resX, ref int resY)
		{
			if (currentScreenResolutionWidth <= recommendResX || currentScreenResolutionHeight <= recommendResY)
			{
				resX = currentScreenResolutionWidth;
				resY = currentScreenResolutionHeight;
				return;
			}
			float num = (float)recommendResX / (float)currentScreenResolutionWidth;
			float num2 = (float)recommendResY / (float)currentScreenResolutionHeight;
			float num3 = (float)currentScreenResolutionWidth / (float)currentScreenResolutionHeight;
			if (num2 <= num)
			{
				resX = Mathf.RoundToInt((float)recommendResY * num3);
				resY = recommendResY;
			}
			else
			{
				resX = recommendResX;
				resY = Mathf.RoundToInt((float)recommendResX / num3);
			}
		}

		private static void SetScreenResolution(ResolutionQualityGrade resolutionQuality, ref int currentScreenResolutionWidth, ref int currentScreenResolutionHeight)
		{
			if (!_hasSettingScreenResolution)
			{
				_originScreenResolution = Screen.currentResolution;
				_hasSettingScreenResolution = true;
			}
			currentScreenResolutionWidth = _originScreenResolution.width;
			currentScreenResolutionHeight = _originScreenResolution.height;
			if (resolutionQuality != ResolutionQualityGrade.High)
			{
				int resX = 0;
				int resY = 0;
				GetResolutionKeepScale(1280, 720, _originScreenResolution.width, _originScreenResolution.height, ref resX, ref resY);
				Screen.SetResolution(resX, resY, Screen.fullScreen);
				currentScreenResolutionWidth = resX;
				currentScreenResolutionHeight = resY;
			}
			else
			{
				Screen.SetResolution(_originScreenResolution.width, _originScreenResolution.height, Screen.fullScreen);
				currentScreenResolutionWidth = _originScreenResolution.width;
				currentScreenResolutionHeight = _originScreenResolution.height;
			}
		}

		public static void SetPostEffectBufferSizeByQuality(Dictionary<PostEffectQualityGrade, int> postFxGradeBufferSize, PostEffectQualityGrade quality)
		{
			PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				int value = 0;
				postFxGradeBufferSize.TryGetValue(quality, out value);
				postFX.internalBufferSize = (PostFXBase.InternalBufferSizeEnum)value;
			}
		}

		public static void EnablePostFX(bool enabled, bool forceWhenDisable = false)
		{
			PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				if (!enabled && forceWhenDisable)
				{
					postFX.enabled = false;
				}
				else
				{
					postFX.enabled = true;
					postFX.OnlyResScale = !enabled;
				}
				postFX.FastMode = !enabled;
				postFX.originalEnabled = postFX.enabled;
				if (onPostFXChanged != null)
				{
					onPostFXChanged(postFX.enabled);
				}
			}
		}

		public static void EnableHDR(bool enabled)
		{
			PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				postFX.HDRBuffer = enabled;
			}
		}

		public static void EnableFXAA(bool enabled)
		{
			PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				postFX.FXAA = enabled;
			}
		}

		public static void EnableDistortion(bool enabled)
		{
			PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				postFX.UseDistortion = enabled;
			}
		}

		public static void EnableColorGrading(bool enabled)
		{
			PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				postFX.UseColorGrading = enabled;
			}
		}

		public static void EnableReflection(bool enabled)
		{
			GlobalVars.USE_REFLECTION = enabled;
			ReflectionBase[] array = UnityEngine.Object.FindObjectsOfType<ReflectionBase>();
			ReflectionBase[] array2 = array;
			foreach (ReflectionBase reflectionBase in array2)
			{
				reflectionBase.SetFastMode(!enabled);
			}
		}

		public static void SetPostFXContrast(float contrastDelta)
		{
			PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				if (Singleton<LevelManager>.Instance != null)
				{
					postFX.constrast = 2f + contrastDelta;
				}
				else
				{
					postFX.constrast = 2.1f + contrastDelta;
				}
			}
		}

		public static void SetTargetFrameRate(int targetFrameRate)
		{
			Application.targetFrameRate = targetFrameRate;
		}

		public static void EnableDynamicBone(bool enabled)
		{
			EnableAvatarsDynamicBone(enabled);
			EnableMonstersDynamicBone(enabled);
		}

		public static void EnableUIAvatarsDynamicBone(bool enabled)
		{
			GlobalVars.UI_AVATAR_USE_DYNAMIC_BONE = enabled;
			BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
			if (sceneCanvas == null || (!(sceneCanvas is MonoMainCanvas) && !(sceneCanvas is MonoTestUI) && !(sceneCanvas is MonoGameEntry)))
			{
				return;
			}
			Avatar3dModelContext avatar3dModelContext = ((sceneCanvas is MonoMainCanvas) ? ((MonoMainCanvas)sceneCanvas).avatar3dModelContext : ((!(sceneCanvas is MonoTestUI)) ? ((MonoGameEntry)sceneCanvas).avatar3dModelContext : ((MonoTestUI)sceneCanvas).avatar3dModelContext));
			if (avatar3dModelContext == null)
			{
				return;
			}
			List<Transform> allAvatars = avatar3dModelContext.GetAllAvatars();
			foreach (Transform item in allAvatars)
			{
				DynamicBone[] componentsInChildren = item.gameObject.GetComponentsInChildren<DynamicBone>(true);
				DynamicBone[] array = componentsInChildren;
				foreach (DynamicBone dynamicBone in array)
				{
					dynamicBone.enabled = enabled;
				}
			}
		}

		public static void EnableAvatarsDynamicBone(bool enabled)
		{
			GlobalVars.AVATAR_USE_DYNAMIC_BONE = enabled;
			if (Singleton<AvatarManager>.Instance == null)
			{
				return;
			}
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
			foreach (BaseMonoAvatar item in allAvatars)
			{
				DynamicBone[] componentsInChildren = item.gameObject.GetComponentsInChildren<DynamicBone>(true);
				DynamicBone[] array = componentsInChildren;
				foreach (DynamicBone dynamicBone in array)
				{
					dynamicBone.enabled = enabled;
				}
			}
		}

		public static void EnableMonstersDynamicBone(bool enabled)
		{
			GlobalVars.MONSTER_USE_DYNAMIC_BONE = enabled;
			if (Singleton<MonsterManager>.Instance == null)
			{
				return;
			}
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			foreach (BaseMonoMonster item in allMonsters)
			{
				DynamicBone[] componentsInChildren = item.gameObject.GetComponentsInChildren<DynamicBone>(true);
				DynamicBone[] array = componentsInChildren;
				foreach (DynamicBone dynamicBone in array)
				{
					dynamicBone.enabled = enabled;
				}
			}
		}

		public static void EnableStaticCloudMode(bool enabled)
		{
			GlobalVars.STATIC_CLOUD_MODE = enabled;
		}
	}
}
