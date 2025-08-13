using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class CameraData
	{
		public const uint MAIN_CAMERA_TYPE = 1u;

		public const uint IN_LEVEL_UI_CAMERA_TYPE = 2u;

		public const uint STAGE_ORTH_CAMERA_TYPE = 3u;

		private const string MAIN_CAMERA_PREFAB_PATH = "Entities/Camera/MainCamera";

		private const string IN_LEVEL_UI_CAMERA_PREFAB_PATH = "Entities/Camera/InLevelUICamera";

		private const string STAGE_ORTH_CAMERA_PREFAB_PATH = "Entities/Camera/OrthStageCamera";

		private static Dictionary<string, AnimationCurve> _cameraCurveDict;

		public static string GetPrefabResPath(uint type)
		{
			switch (type)
			{
			case 1u:
				return "Entities/Camera/MainCamera";
			case 2u:
				return "Entities/Camera/InLevelUICamera";
			case 3u:
				return "Entities/Camera/OrthStageCamera";
			default:
				throw new Exception("Invalid Type or State!");
			}
		}

		public static void ReloadFromFile()
		{
			_cameraCurveDict = new Dictionary<string, AnimationCurve>();
			string[] cameraCurvePatternPathes = GlobalDataManager.metaConfig.cameraCurvePatternPathes;
			for (int i = 0; i < cameraCurvePatternPathes.Length; i++)
			{
				ConfigCameraCurvePattern configCameraCurvePattern = ConfigUtil.LoadConfig<ConfigCameraCurvePattern>(cameraCurvePatternPathes[i]);
				if (!(configCameraCurvePattern == null))
				{
					for (int j = 0; j < configCameraCurvePattern.patterns.Length; j++)
					{
						CameraCurvePattern cameraCurvePattern = configCameraCurvePattern.patterns[j];
						_cameraCurveDict.Add(cameraCurvePattern.name, cameraCurvePattern.animationCurve);
					}
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			_cameraCurveDict = new Dictionary<string, AnimationCurve>();
			string[] cameraCurvePatternPathes = GlobalDataManager.metaConfig.cameraCurvePatternPathes;
			float step = progressSpan / (float)cameraCurvePatternPathes.Length;
			for (int ix = 0; ix < cameraCurvePatternPathes.Length; ix++)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync(cameraCurvePatternPathes[ix]);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null cameraCurvePatternPath :" + cameraCurvePatternPathes[ix]);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				ConfigCameraCurvePattern cameraCurvePatternConfig = (ConfigCameraCurvePattern)asyncRequest.asset;
				SuperDebug.VeryImportantAssert(cameraCurvePatternConfig != null, "cameraCurvePatternConfig is null cameraCurvePatternPath :" + cameraCurvePatternPathes[ix]);
				if (!(cameraCurvePatternConfig == null) && cameraCurvePatternConfig.patterns != null)
				{
					for (int jx = 0; jx < cameraCurvePatternConfig.patterns.Length; jx++)
					{
						CameraCurvePattern pattern = cameraCurvePatternConfig.patterns[jx];
						_cameraCurveDict.Add(pattern.name, pattern.animationCurve);
					}
				}
			}
		}

		public static AnimationCurve GetCameraCurveByName(string name)
		{
			if (!string.IsNullOrEmpty(name) && _cameraCurveDict.ContainsKey(name))
			{
				return _cameraCurveDict[name];
			}
			return null;
		}
	}
}
