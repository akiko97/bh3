using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class RenderingData
	{
		private static Dictionary<string, ConfigBaseRenderingData> _renderingDataDict;

		public static void ReloadFromFile()
		{
			_renderingDataDict = new Dictionary<string, ConfigBaseRenderingData>();
			string[] renderEntryPathes = GlobalDataManager.metaConfig.renderEntryPathes;
			for (int i = 0; i < renderEntryPathes.Length; i++)
			{
				ConfigRenderingDataRegistry configRenderingDataRegistry = ConfigUtil.LoadConfig<ConfigRenderingDataRegistry>(renderEntryPathes[i]);
				if (configRenderingDataRegistry.entries != null)
				{
					for (int j = 0; j < configRenderingDataRegistry.entries.Length; j++)
					{
						RenderingDataEntry renderingDataEntry = configRenderingDataRegistry.entries[j];
						_renderingDataDict.Add(renderingDataEntry.name, Miscs.LoadResource<ConfigBaseRenderingData>(renderingDataEntry.GetDataPath()));
					}
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			_renderingDataDict = new Dictionary<string, ConfigBaseRenderingData>();
			string[] renderingDataRegistryPathes = GlobalDataManager.metaConfig.renderEntryPathes;
			float step = progressSpan / (float)renderingDataRegistryPathes.Length;
			for (int ix = 0; ix < renderingDataRegistryPathes.Length; ix++)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync(renderingDataRegistryPathes[ix]);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null renderingDataRegistryPath :" + renderingDataRegistryPathes[ix]);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				ConfigRenderingDataRegistry renderingRegistry = (ConfigRenderingDataRegistry)asyncRequest.asset;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				SuperDebug.VeryImportantAssert(renderingRegistry != null, "renderingRegistry is null renderingDataRegistryPath :" + renderingDataRegistryPathes[ix]);
				if (!(renderingRegistry == null) && renderingRegistry.entries != null)
				{
					for (int jx = 0; jx < renderingRegistry.entries.Length; jx++)
					{
						RenderingDataEntry entry = renderingRegistry.entries[jx];
						_renderingDataDict.Add(entry.name, Miscs.LoadResource<ConfigBaseRenderingData>(entry.GetDataPath()));
					}
				}
			}
		}

		public static T GetRenderingDataConfig<T>(string name) where T : ConfigBaseRenderingData
		{
			return (T)_renderingDataDict[name];
		}

		public static void ApplyRenderingData(ConfigBaseRenderingData renderingData, Material mat)
		{
			for (int i = 0; i < renderingData.properties.Length; i++)
			{
				BaseRenderingProperty baseRenderingProperty = renderingData.properties[i];
				baseRenderingProperty.SimpleApplyOnMaterial(mat);
			}
		}
	}
}
