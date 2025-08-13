using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class TouchPatternData
	{
		private static Dictionary<string, List<TouchPatternItem>> _touchPatternDict;

		public static void ReloadFromFile()
		{
			_touchPatternDict = new Dictionary<string, List<TouchPatternItem>>();
			string[] touchPatternPathes = GlobalDataManager.metaConfig.touchPatternPathes;
			for (int i = 0; i < touchPatternPathes.Length; i++)
			{
				ConfigTouchPattern configTouchPattern = ConfigUtil.LoadConfig<ConfigTouchPattern>(touchPatternPathes[i]);
				if (_touchPatternDict.ContainsKey(configTouchPattern.name))
				{
					Debug.LogError(string.Format("duplicate touch pattern name : {0}", configTouchPattern.name));
				}
				else if (configTouchPattern.touchPatternItems != null)
				{
					List<TouchPatternItem> list = new List<TouchPatternItem>();
					for (int j = 0; j < configTouchPattern.touchPatternItems.Length; j++)
					{
						list.Add(configTouchPattern.touchPatternItems[j]);
					}
					_touchPatternDict[configTouchPattern.name] = list;
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			_touchPatternDict = new Dictionary<string, List<TouchPatternItem>>();
			string[] touchPatternPathes = GlobalDataManager.metaConfig.touchPatternPathes;
			float step = progressSpan / (float)touchPatternPathes.Length;
			for (int ix = 0; ix < touchPatternPathes.Length; ix++)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync(touchPatternPathes[ix]);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null touchPatternPath :" + touchPatternPathes[ix]);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				ConfigTouchPattern patternConfig = (ConfigTouchPattern)asyncRequest.asset;
				SuperDebug.VeryImportantAssert(patternConfig != null, "assetRequest is null touchPatternPath :" + touchPatternPathes[ix]);
				if (patternConfig == null)
				{
					continue;
				}
				if (_touchPatternDict.ContainsKey(patternConfig.name))
				{
					Debug.LogError(string.Format("duplicate touch pattern name : {0}", patternConfig.name));
				}
				else if (patternConfig.touchPatternItems != null)
				{
					List<TouchPatternItem> patternList = new List<TouchPatternItem>();
					for (int jx = 0; jx < patternConfig.touchPatternItems.Length; jx++)
					{
						patternList.Add(patternConfig.touchPatternItems[jx]);
					}
					_touchPatternDict[patternConfig.name] = patternList;
				}
			}
		}

		public static List<TouchPatternItem> GetTouchPatternList(string characterName)
		{
			if (_touchPatternDict.ContainsKey(characterName))
			{
				return _touchPatternDict[characterName];
			}
			return null;
		}
	}
}
