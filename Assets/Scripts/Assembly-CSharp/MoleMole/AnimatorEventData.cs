using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public static class AnimatorEventData
	{
		private static Dictionary<string, AnimatorEventPattern> _animatorEventPatternDict;

		public static void ReloadFromFile()
		{
			_animatorEventPatternDict = new Dictionary<string, AnimatorEventPattern>();
			string[] animatorEventPatternPathes = GlobalDataManager.metaConfig.animatorEventPatternPathes;
			for (int i = 0; i < animatorEventPatternPathes.Length; i++)
			{
				ConfigAnimatorEventPattern configAnimatorEventPattern = ConfigUtil.LoadConfig<ConfigAnimatorEventPattern>(animatorEventPatternPathes[i]);
				if (configAnimatorEventPattern.patterns != null)
				{
					for (int j = 0; j < configAnimatorEventPattern.patterns.Length; j++)
					{
						AnimatorEventPattern animatorEventPattern = configAnimatorEventPattern.patterns[j];
						_animatorEventPatternDict.Add(animatorEventPattern.name, animatorEventPattern);
					}
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			_animatorEventPatternDict = new Dictionary<string, AnimatorEventPattern>();
			string[] animatorEventPatternPathes = GlobalDataManager.metaConfig.animatorEventPatternPathes;
			float step = progressSpan / (float)animatorEventPatternPathes.Length;
			for (int ix = 0; ix < animatorEventPatternPathes.Length; ix++)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync(animatorEventPatternPathes[ix]);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null animatorEventPatternPath :" + animatorEventPatternPathes[ix]);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				ConfigAnimatorEventPattern patternConfig = (ConfigAnimatorEventPattern)asyncRequest.asset;
				SuperDebug.VeryImportantAssert(patternConfig != null, "patternConfig is null animatorEventPatternPath :" + animatorEventPatternPathes[ix]);
				if (!(patternConfig == null) && patternConfig.patterns != null)
				{
					for (int jx = 0; jx < patternConfig.patterns.Length; jx++)
					{
						AnimatorEventPattern pattern = patternConfig.patterns[jx];
						_animatorEventPatternDict.Add(pattern.name, pattern);
					}
				}
			}
		}

		public static ConfigAnimatorEventPattern[] GetConfigs()
		{
			List<ConfigAnimatorEventPattern> list = new List<ConfigAnimatorEventPattern>();
			string[] animatorEventPatternPathes = GlobalDataManager.metaConfig.animatorEventPatternPathes;
			for (int i = 0; i < animatorEventPatternPathes.Length; i++)
			{
				ConfigAnimatorEventPattern item = ConfigUtil.LoadConfig<ConfigAnimatorEventPattern>(animatorEventPatternPathes[i]);
				list.Add(item);
			}
			return list.ToArray();
		}

		public static AnimatorEventPattern GetAnimatorEventPattern(string patternName)
		{
			if (!_animatorEventPatternDict.ContainsKey(patternName))
			{
				return null;
			}
			return _animatorEventPatternDict[patternName];
		}

		public static Dictionary<string, AnimatorEventPattern> GetAllAnimatorEventPatterns()
		{
			return _animatorEventPatternDict;
		}
	}
}
