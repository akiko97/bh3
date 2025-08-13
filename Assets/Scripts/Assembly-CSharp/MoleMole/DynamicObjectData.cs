using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public static class DynamicObjectData
	{
		public static Dictionary<string, string> dynamicObjectDict;

		public static void ReloadFromFile()
		{
			dynamicObjectDict = new Dictionary<string, string>();
			string[] dynamicObjectRegistryPathes = GlobalDataManager.metaConfig.dynamicObjectRegistryPathes;
			for (int i = 0; i < dynamicObjectRegistryPathes.Length; i++)
			{
				ConfigDynamicObjectRegistry configDynamicObjectRegistry = ConfigUtil.LoadConfig<ConfigDynamicObjectRegistry>(dynamicObjectRegistryPathes[i]);
				if (configDynamicObjectRegistry.entries != null)
				{
					for (int j = 0; j < configDynamicObjectRegistry.entries.Length; j++)
					{
						DynamicObjectEntry dynamicObjectEntry = configDynamicObjectRegistry.entries[j];
						dynamicObjectDict.Add(dynamicObjectEntry.name, dynamicObjectEntry.prefabPath);
					}
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			dynamicObjectDict = new Dictionary<string, string>();
			string[] dynamicObjectPathes = GlobalDataManager.metaConfig.dynamicObjectRegistryPathes;
			float step = progressSpan / (float)dynamicObjectPathes.Length;
			for (int ix = 0; ix < dynamicObjectPathes.Length; ix++)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync(dynamicObjectPathes[ix]);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "patternConfig is null dynamicObjectPathes :" + dynamicObjectPathes[ix]);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				ConfigDynamicObjectRegistry dynamicObjectRegistryConfig = (ConfigDynamicObjectRegistry)asyncRequest.asset;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				SuperDebug.VeryImportantAssert(dynamicObjectRegistryConfig != null, "dynamicObjectRegistryConfig is null dynamicObjectPathes :" + dynamicObjectPathes[ix]);
				if (!(dynamicObjectRegistryConfig == null) && dynamicObjectRegistryConfig.entries != null)
				{
					for (int jx = 0; jx < dynamicObjectRegistryConfig.entries.Length; jx++)
					{
						DynamicObjectEntry entry = dynamicObjectRegistryConfig.entries[jx];
						dynamicObjectDict.Add(entry.name, entry.prefabPath);
					}
				}
			}
		}

		public static ConfigDynamicObjectRegistry GetDynamicObjectRegistry(string registryPath)
		{
			string[] dynamicObjectRegistryPathes = GlobalDataManager.metaConfig.dynamicObjectRegistryPathes;
			for (int i = 0; i < dynamicObjectRegistryPathes.Length; i++)
			{
				if (dynamicObjectRegistryPathes[i] == registryPath)
				{
					return ConfigUtil.LoadConfig<ConfigDynamicObjectRegistry>(dynamicObjectRegistryPathes[i]);
				}
			}
			return null;
		}
	}
}
