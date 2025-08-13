using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public static class EffectData
	{
		private const string EFFECT_PREFAB_PATH = "Effect/";

		private static Dictionary<string, EffectPattern> _effectPatternDict;

		private static Dictionary<string, EffectPattern[]> _effectGroupDict;

		public static string GetPrefabResPath(string effectPath)
		{
			return "Effect/" + effectPath;
		}

		public static void ReloadFromFile()
		{
			_effectPatternDict = new Dictionary<string, EffectPattern>();
			_effectGroupDict = new Dictionary<string, EffectPattern[]>();
			string[] effectPatternPathes = GlobalDataManager.metaConfig.effectPatternPathes;
			for (int i = 0; i < effectPatternPathes.Length; i++)
			{
				ConfigEffectPattern configEffectPattern = ConfigUtil.LoadConfig<ConfigEffectPattern>(effectPatternPathes[i]);
				if (configEffectPattern.patterns != null)
				{
					_effectGroupDict.Add(configEffectPattern.groupName, configEffectPattern.patterns);
					for (int j = 0; j < configEffectPattern.patterns.Length; j++)
					{
						EffectPattern effectPattern = configEffectPattern.patterns[j];
						_effectPatternDict.Add(effectPattern.name, effectPattern);
					}
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			_effectPatternDict = new Dictionary<string, EffectPattern>();
			_effectGroupDict = new Dictionary<string, EffectPattern[]>();
			string[] effectPatternPathes = GlobalDataManager.metaConfig.effectPatternPathes;
			float step = progressSpan / (float)effectPatternPathes.Length;
			for (int ix = 0; ix < effectPatternPathes.Length; ix++)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync(effectPatternPathes[ix]);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null effectPath :" + effectPatternPathes[ix]);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				ConfigEffectPattern patternConfig = (ConfigEffectPattern)asyncRequest.asset;
				SuperDebug.VeryImportantAssert(patternConfig != null, "patternConfig is null effectPath :" + effectPatternPathes[ix]);
				if (!(patternConfig == null) && patternConfig.patterns != null)
				{
					_effectGroupDict.Add(patternConfig.groupName, patternConfig.patterns);
					for (int jx = 0; jx < patternConfig.patterns.Length; jx++)
					{
						EffectPattern pattern = patternConfig.patterns[jx];
						_effectPatternDict.Add(pattern.name, pattern);
					}
				}
			}
		}

		public static EffectPattern GetEffectPattern(string patternName)
		{
			return _effectPatternDict[patternName];
		}

		public static bool HasEffectPattern(string patternName)
		{
			return _effectGroupDict.ContainsKey(patternName);
		}

		public static Dictionary<string, EffectPattern> GetAllEffectPatterns()
		{
			return _effectPatternDict;
		}

		public static EffectPattern[] GetEffectGroupPatterns(string effectGroupName)
		{
			return _effectGroupDict[effectGroupName];
		}
	}
}
