using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using MoleMole.Config;

namespace MoleMole
{
	public static class AIData
	{
		public const string THREAT_RETARGET_AI_EVENT = "AIThreatRetarget_1";

		public const string TARGET_ATTACK_START_AI_EVENT = "AITargetAttackStart_0";

		public const int DEFAULT_LOCAL_AVATAR_BE_ATTACK_MAX_NUM = 4;

		private const string GROUP_AI_GRID_CONFIG_PATH = "Data/AI/GroupAIGrid";

		private static Dictionary<string, ConfigGroupAIGridEntry> _allGridEntries;

		public static ConfigGroupAIGridRepository GroupAIGrid;

		private static List<string> _configPathList;

		private static Action<string> _loadJsonConfigCallback = null;

		private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();

		public static void ReloadFromFile()
		{
			_allGridEntries = new Dictionary<string, ConfigGroupAIGridEntry>();
			string[] groupAIGridPathes = GlobalDataManager.metaConfig.groupAIGridPathes;
			foreach (string jsonPath in groupAIGridPathes)
			{
				ConfigGroupAIGridRepository configGroupAIGridRepository = ConfigUtil.LoadJSONConfig<ConfigGroupAIGridRepository>(jsonPath);
				foreach (ConfigGroupAIGridEntry item in configGroupAIGridRepository)
				{
					_allGridEntries.Add(item.Name, item);
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
		{
			_loadJsonConfigCallback = finishCallback;
			_configPathList = new List<string>();
			_allGridEntries = new Dictionary<string, ConfigGroupAIGridEntry>();
			string[] pathes = GlobalDataManager.metaConfig.groupAIGridPathes;
			if (pathes.Length == 0)
			{
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("AIData");
					_loadJsonConfigCallback = null;
				}
				yield break;
			}
			string[] array = pathes;
			foreach (string gridFilePath in array)
			{
				_configPathList.Add(gridFilePath);
			}
			float step = progressSpan / (float)pathes.Length;
			_loadDataBackGroundWorker.StartBackGroundWork("AIData");
			string[] array2 = pathes;
			foreach (string gridFilePath2 in array2)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync(gridFilePath2);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null gridFilePath :" + gridFilePath2);
				if (asyncRequest != null)
				{
					yield return asyncRequest.operation;
					if (moveOneStepCallback != null)
					{
						moveOneStepCallback(step);
					}
					ConfigUtil.LoadJSONStrConfigMultiThread<ConfigGroupAIGridRepository>(asyncRequest.asset.ToString(), _loadDataBackGroundWorker, OnLoadOneJsonConfigFinish, gridFilePath2);
				}
			}
		}

		private static void OnLoadOneJsonConfigFinish(ConfigGroupAIGridRepository gridAIEntries, string configPath)
		{
			_configPathList.Remove(configPath);
			foreach (ConfigGroupAIGridEntry gridAIEntry in gridAIEntries)
			{
				_allGridEntries.Add(gridAIEntry.Name, gridAIEntry);
			}
			if (_configPathList.Count == 0)
			{
				_loadDataBackGroundWorker.StopBackGroundWork(false);
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("AIData");
					_loadJsonConfigCallback = null;
				}
			}
		}

		public static ConfigGroupAIGridEntry GetGroupAIGridEntry(string name)
		{
			return _allGridEntries[name];
		}

		public static void SetSharedVariableCompat(BehaviorTree btree, ConfigDynamicArguments aiParams)
		{
			foreach (KeyValuePair<string, object> aiParam in aiParams)
			{
				SharedVariable variable = btree.GetVariable(aiParam.Key);
				if (variable is SharedFloat || variable is SharedSafeFloat)
				{
					if (aiParam.Value is int)
					{
						variable.SetValue((float)(int)aiParam.Value);
					}
					else
					{
						variable.SetValue((float)aiParam.Value);
					}
				}
				else if (variable is SharedInt || variable is SharedSafeInt)
				{
					variable.SetValue((int)aiParam.Value);
				}
				else if (variable is SharedBool)
				{
					variable.SetValue((bool)aiParam.Value);
				}
				else if (variable is SharedString)
				{
					variable.SetValue((string)aiParam.Value);
				}
				else if (variable is SharedGroupMoveType)
				{
					variable.SetValue(Enum.Parse(typeof(ConfigGroupAIMinionOld.MoveType), (string)aiParam.Value));
				}
				else if (variable is SharedGroupAttackType)
				{
					variable.SetValue(Enum.Parse(typeof(ConfigGroupAIMinionOld.AttackType), (string)aiParam.Value));
				}
			}
		}

		public static void SetSharedVariableOld(BehaviorTree btree, ConfigGroupAIMinionParamOld[] AIParams)
		{
			for (int i = 0; i < AIParams.Length; i++)
			{
				switch (AIParams[i].Type)
				{
				case ConfigGroupAIMinionParamOld.ParamType.Bool:
					btree.SetVariableValue(AIParams[i].Name, AIParams[i].BoolValue);
					continue;
				case ConfigGroupAIMinionParamOld.ParamType.Int:
					btree.SetVariableValue(AIParams[i].Name, AIParams[i].IntValue);
					continue;
				case ConfigGroupAIMinionParamOld.ParamType.Float:
					btree.SetVariableValue(AIParams[i].Name, AIParams[i].FloatValue);
					continue;
				case ConfigGroupAIMinionParamOld.ParamType.AttackType:
					btree.SetVariableValue(AIParams[i].Name, AIParams[i].AttackTypeValue);
					continue;
				case ConfigGroupAIMinionParamOld.ParamType.MoveType:
					btree.SetVariableValue(AIParams[i].Name, AIParams[i].MoveTypeValue);
					continue;
				}
				if (AIParams[i].Interruption)
				{
					btree.SendEvent("Interruption", (object)true);
					btree.SendEvent("Interruption", (object)false);
					btree.SetVariableValue("Group_TriggerAttack", false);
				}
				if (AIParams[i].TriggerAttack)
				{
					if (AIParams[i].TriggerAttackDelay > 0f)
					{
						btree.SetVariableValue("Group_TriggerAttackDelay", AIParams[i].TriggerAttackDelay);
					}
					else
					{
						btree.SetVariableValue("Group_TriggerAttack", true);
					}
				}
			}
		}
	}
}
