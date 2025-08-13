using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public static class AuxObjectData
	{
		private static Dictionary<string, string> _auxObjectDict;

		public static void ReloadFromFile()
		{
			_auxObjectDict = new Dictionary<string, string>();
			string[] auxEntryPathes = GlobalDataManager.metaConfig.auxEntryPathes;
			for (int i = 0; i < auxEntryPathes.Length; i++)
			{
				ConfigAuxObjectRegistry configAuxObjectRegistry = ConfigUtil.LoadConfig<ConfigAuxObjectRegistry>(auxEntryPathes[i]);
				if (configAuxObjectRegistry.entries != null)
				{
					for (int j = 0; j < configAuxObjectRegistry.entries.Length; j++)
					{
						AuxObjectEntry auxObjectEntry = configAuxObjectRegistry.entries[j];
						_auxObjectDict.Add(auxObjectEntry.name, auxObjectEntry.GetPrefabPath());
					}
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			_auxObjectDict = new Dictionary<string, string>();
			string[] auxRegistryPathes = GlobalDataManager.metaConfig.auxEntryPathes;
			float step = progressSpan / (float)auxRegistryPathes.Length;
			for (int ix = 0; ix < auxRegistryPathes.Length; ix++)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync(auxRegistryPathes[ix]);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null auxRegistryPath :" + auxRegistryPathes[ix]);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				ConfigAuxObjectRegistry auxRegistryConfig = (ConfigAuxObjectRegistry)asyncRequest.asset;
				SuperDebug.VeryImportantAssert(auxRegistryConfig != null, "auxRegistryConfig is null auxRegistryPath :" + auxRegistryPathes[ix]);
				if (!(auxRegistryConfig == null) && auxRegistryConfig.entries != null)
				{
					for (int jx = 0; jx < auxRegistryConfig.entries.Length; jx++)
					{
						AuxObjectEntry entry = auxRegistryConfig.entries[jx];
						_auxObjectDict.Add(entry.name, entry.GetPrefabPath());
					}
				}
			}
		}

		public static string GetAuxObjectPrefabPath(string auxObjectName)
		{
			return _auxObjectDict[auxObjectName];
		}

		public static bool ContainAuxObjectPrefabPath(string auxObjectName)
		{
			return _auxObjectDict.ContainsKey(auxObjectName);
		}
	}
}
