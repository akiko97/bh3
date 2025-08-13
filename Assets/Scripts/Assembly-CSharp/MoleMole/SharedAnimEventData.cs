using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public class SharedAnimEventData
	{
		public const char SHARED_ANIM_EVENT_PREFIX = '#';

		private static List<string> _configPathList;

		private static Action<string> _loadJsonConfigCallback = null;

		private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();

		private static Dictionary<string, ConfigEntityAnimEvent> _sharedAnimEventDict;

		public static void ReloadFromData()
		{
			_sharedAnimEventDict = new Dictionary<string, ConfigEntityAnimEvent>();
			string[] sharedAnimEventGroupPathes = GlobalDataManager.metaConfig.sharedAnimEventGroupPathes;
			foreach (string jsonPath in sharedAnimEventGroupPathes)
			{
				ConfigSharedAnimEventGroup configSharedAnimEventGroup = ConfigUtil.LoadJSONConfig<ConfigSharedAnimEventGroup>(jsonPath);
				foreach (KeyValuePair<string, ConfigEntityAnimEvent> animEvent in configSharedAnimEventGroup.AnimEvents)
				{
					_sharedAnimEventDict.Add(animEvent.Key, animEvent.Value);
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
		{
			_loadJsonConfigCallback = finishCallback;
			_configPathList = new List<string>();
			_sharedAnimEventDict = new Dictionary<string, ConfigEntityAnimEvent>();
			string[] pathes = GlobalDataManager.metaConfig.sharedAnimEventGroupPathes;
			if (pathes.Length == 0)
			{
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("SharedAnimEventData");
					_loadJsonConfigCallback = null;
				}
				yield break;
			}
			string[] array = pathes;
			foreach (string jsonPath in array)
			{
				_configPathList.Add(jsonPath);
			}
			float step = progressSpan / (float)pathes.Length;
			_loadDataBackGroundWorker.StartBackGroundWork("SharedAnimEventData");
			string[] array2 = pathes;
			foreach (string jsonPath2 in array2)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync(jsonPath2);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null sharedAnimEventPath :" + jsonPath2);
				if (asyncRequest != null)
				{
					yield return asyncRequest.operation;
					if (moveOneStepCallback != null)
					{
						moveOneStepCallback(step);
					}
					ConfigUtil.LoadJSONStrConfigMultiThread<ConfigSharedAnimEventGroup>(asyncRequest.asset.ToString(), _loadDataBackGroundWorker, OnLoadOneJsonConfigFinish, jsonPath2);
				}
			}
		}

		private static void OnLoadOneJsonConfigFinish(ConfigSharedAnimEventGroup animEventGroup, string configPath)
		{
			_configPathList.Remove(configPath);
			foreach (KeyValuePair<string, ConfigEntityAnimEvent> animEvent in animEventGroup.AnimEvents)
			{
				_sharedAnimEventDict.Add(animEvent.Key, animEvent.Value);
			}
			if (_configPathList.Count == 0)
			{
				_loadDataBackGroundWorker.StopBackGroundWork(false);
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("SharedAnimEventData");
					_loadJsonConfigCallback = null;
				}
			}
		}

		public static bool IsSharedAnimEventID(string animEventID)
		{
			return animEventID[0] == '#';
		}

		public static ConfigAvatarAnimEvent ResolveAnimEvent(ConfigAvatar config, string animEventID)
		{
			if (animEventID == null)
			{
				return null;
			}
			if (animEventID[0] == '#')
			{
				return _sharedAnimEventDict[animEventID] as ConfigAvatarAnimEvent;
			}
			ConfigAvatarAnimEvent value;
			config.AnimEvents.TryGetValue(animEventID, out value);
			return value;
		}

		public static ConfigMonsterAnimEvent ResolveAnimEvent(ConfigMonster config, string animEventID)
		{
			if (animEventID == null)
			{
				return null;
			}
			if (animEventID[0] == '#')
			{
				return _sharedAnimEventDict[animEventID] as ConfigMonsterAnimEvent;
			}
			ConfigMonsterAnimEvent value;
			config.AnimEvents.TryGetValue(animEventID, out value);
			return value;
		}

		public static ConfigPropAnimEvent ResolveAnimEvent(ConfigPropObject config, string animEventID)
		{
			if (animEventID == null)
			{
				return null;
			}
			if (animEventID[0] == '#')
			{
				return _sharedAnimEventDict[animEventID] as ConfigPropAnimEvent;
			}
			ConfigPropAnimEvent value;
			config.AnimEvents.TryGetValue(animEventID, out value);
			return value;
		}

		public static ConfigEntityAnimEvent ResolveAnimEvent(IEntityConfig config, string animEventID)
		{
			if (animEventID == null)
			{
				return null;
			}
			if (animEventID[0] == '#')
			{
				return _sharedAnimEventDict[animEventID];
			}
			return config.TryGetAnimEvent(animEventID);
		}
	}
}
