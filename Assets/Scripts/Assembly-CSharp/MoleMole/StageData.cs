using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public static class StageData
	{
		public const float ORTH_STAGE_LIFT_RATIO = 0.06f;

		public const string STAGE_REG_FILE_PATH = "Stage/StageRegistry";

		private static Dictionary<string, StageEntry> _stageEntryDict;

		public static void ReloadFromFile()
		{
			_stageEntryDict = new Dictionary<string, StageEntry>();
			string[] stageEntryPathes = GlobalDataManager.metaConfig.stageEntryPathes;
			for (int i = 0; i < stageEntryPathes.Length; i++)
			{
				ConfigStageRegistry configStageRegistry = ConfigUtil.LoadConfig<ConfigStageRegistry>(stageEntryPathes[i]);
				if (configStageRegistry.entries != null)
				{
					for (int j = 0; j < configStageRegistry.entries.Length; j++)
					{
						StageEntry stageEntry = configStageRegistry.entries[j];
						_stageEntryDict.Add(stageEntry.TypeName, stageEntry);
					}
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			_stageEntryDict = new Dictionary<string, StageEntry>();
			string[] stageEntryPathes = GlobalDataManager.metaConfig.stageEntryPathes;
			float step = progressSpan / (float)stageEntryPathes.Length;
			for (int ix = 0; ix < stageEntryPathes.Length; ix++)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync(stageEntryPathes[ix]);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null stagePath :" + stageEntryPathes[ix]);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				ConfigStageRegistry stageRegistryConfig = (ConfigStageRegistry)asyncRequest.asset;
				SuperDebug.VeryImportantAssert(stageRegistryConfig != null, "stageRegistryConfig is null stagePath :" + stageEntryPathes[ix]);
				if (!(stageRegistryConfig == null) && stageRegistryConfig.entries != null)
				{
					for (int jx = 0; jx < stageRegistryConfig.entries.Length; jx++)
					{
						StageEntry stage = stageRegistryConfig.entries[jx];
						_stageEntryDict.Add(stage.TypeName, stage);
					}
				}
			}
		}

		public static StageEntry GetStageEntryByName(string typeName)
		{
			return _stageEntryDict[typeName];
		}

		public static Dictionary<string, StageEntry> GetAllStageEntries()
		{
			return _stageEntryDict;
		}

		public static StageEntry GetFirstStageEntryByPrefabPathAndLocatorName(string perpStagePrefabPath, string locatorName)
		{
			foreach (StageEntry value in _stageEntryDict.Values)
			{
				if (value.GetPerpStagePrefabPath() == perpStagePrefabPath && value.LocationPointName.EndsWith(locatorName))
				{
					return value;
				}
			}
			return null;
		}
	}
}
