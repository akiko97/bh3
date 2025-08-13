using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public static class PropObjectData
	{
		public static Dictionary<string, ConfigPropObject> _propMap;

		private static List<string> _configPathList;

		private static Action<string> _loadJsonConfigCallback = null;

		private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();

		public static void ReloadFromFile()
		{
			_propMap = new Dictionary<string, ConfigPropObject>();
			string[] propObjectRegistryPathes = GlobalDataManager.metaConfig.propObjectRegistryPathes;
			foreach (string jsonPath in propObjectRegistryPathes)
			{
				ConfigPropObjectRegistry configPropObjectRegistry = ConfigUtil.LoadJSONConfig<ConfigPropObjectRegistry>(jsonPath);
				foreach (ConfigPropObject item in configPropObjectRegistry)
				{
					_propMap.Add(item.Name, item);
				}
			}
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (ConfigPropObject value in _propMap.Values)
			{
				HashUtils.TryHashObject(value, ref lastHash);
			}
			return lastHash;
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
		{
			_loadJsonConfigCallback = finishCallback;
			_configPathList = new List<string>();
			_propMap = new Dictionary<string, ConfigPropObject>();
			string[] pathes = GlobalDataManager.metaConfig.propObjectRegistryPathes;
			if (pathes.Length == 0)
			{
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("PropObjectData");
					_loadJsonConfigCallback = null;
				}
				yield break;
			}
			string[] array = pathes;
			foreach (string propRegistryPath in array)
			{
				_configPathList.Add(propRegistryPath);
			}
			float step = progressSpan / (float)pathes.Length;
			_loadDataBackGroundWorker.StartBackGroundWork("PropObjectData");
			string[] array2 = pathes;
			foreach (string propRegistryPath2 in array2)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync(propRegistryPath2);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null propRegistryPath :" + propRegistryPath2);
				if (asyncRequest != null)
				{
					yield return asyncRequest.operation;
					if (moveOneStepCallback != null)
					{
						moveOneStepCallback(step);
					}
					ConfigUtil.LoadJSONStrConfigMultiThread<ConfigPropObjectRegistry>(asyncRequest.asset.ToString(), _loadDataBackGroundWorker, OnLoadOneJsonConfigFinish, propRegistryPath2);
				}
			}
		}

		private static void OnLoadOneJsonConfigFinish(ConfigPropObjectRegistry propList, string configPath)
		{
			_configPathList.Remove(configPath);
			foreach (ConfigPropObject prop in propList)
			{
				_propMap.Add(prop.Name, prop);
			}
			if (_configPathList.Count == 0)
			{
				_loadDataBackGroundWorker.StopBackGroundWork(false);
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("PropObjectData");
					_loadJsonConfigCallback = null;
				}
			}
		}

		public static ConfigPropObject GetPropObjectConfig(string propName)
		{
			return _propMap[propName];
		}
	}
}
