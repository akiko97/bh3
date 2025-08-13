using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class StageManager
	{
		private MonoBasePerpStage _perpStage;

		private MonoStageEnv _stageEnv;

		private StageEntry _activeStageEntry;

		private FixedStack<ConfigStageEffectSetting> _stageEffectSettingStack;

		private bool _transitOnChange;

		private string[] _localAvatarPredicates = Miscs.EMPTY_STRINGS;

		private StageManager()
		{
		}

		public void InitAtAwake()
		{
			InitStageEffectSettings();
		}

		public void InitAtStart()
		{
		}

		public void Core()
		{
		}

		public MonoBasePerpStage GetPerpStage()
		{
			return _perpStage;
		}

		public MonoStageEnv GetStageEnv()
		{
			return _stageEnv;
		}

		public string GetStageTypeName()
		{
			return _activeStageEntry.TypeName;
		}

		public StageEntry GetActiveStageEntry()
		{
			return _activeStageEntry;
		}

		public void CreateStage(string typeName, List<string> avatarSpawnNameList, string baseWeatherName, bool isContinued = false)
		{
			StageEntry stageEntryByName = StageData.GetStageEntryByName(typeName);
			bool isBorn = _activeStageEntry == null;
			Vector3 preStagePos = Vector3.zero;
			MonoBasePerpStage.ContinueWeatherDataSettings continueData = null;
			MonoBasePerpStage perpStage;
			if (_activeStageEntry == null)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Miscs.LoadResource(stageEntryByName.GetPerpStagePrefabPath()));
				perpStage = gameObject.GetComponent<MonoBasePerpStage>();
				SetPerpstageNodeVisibility(perpStage, stageEntryByName, false, false);
				SetPerpstageNodeVisibility(perpStage, stageEntryByName, true, true);
			}
			else if (_activeStageEntry.PerpStagePrefabPath != stageEntryByName.PerpStagePrefabPath)
			{
				preStagePos = _perpStage.transform.position;
				continueData = _perpStage.GetContinueWeatherDataSetup();
				UnityEngine.Object.DestroyImmediate(_perpStage.gameObject);
				Resources.UnloadUnusedAssets();
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Miscs.LoadResource(stageEntryByName.GetPerpStagePrefabPath()));
				perpStage = gameObject2.GetComponent<MonoBasePerpStage>();
				SetPerpstageNodeVisibility(perpStage, stageEntryByName, false, false);
				SetPerpstageNodeVisibility(perpStage, stageEntryByName, true, true);
			}
			else
			{
				SetPerpstageNodeVisibility(_perpStage, _activeStageEntry, false, true);
				preStagePos = -_perpStage.transform.Find(_activeStageEntry.LocationPointName).localPosition;
				continueData = _perpStage.GetContinueWeatherDataSetup();
				perpStage = _perpStage;
				SetPerpstageNodeVisibility(perpStage, stageEntryByName, false, false);
				SetPerpstageNodeVisibility(perpStage, stageEntryByName, true, true);
			}
			Vector3 offset = InitAfterCreateStage(stageEntryByName, perpStage, preStagePos, isBorn, baseWeatherName, continueData, isContinued);
			Singleton<AvatarManager>.Instance.PreloadTeamAvatars();
			Singleton<EventManager>.Instance.FireEvent(new EvtStageCreated(avatarSpawnNameList, isBorn, offset));
		}

		private Vector3 InitAfterCreateStage(StageEntry stageEntry, MonoBasePerpStage perpStage, Vector3 preStagePos, bool isBorn, string baseWeatherName, MonoBasePerpStage.ContinueWeatherDataSettings continueData, bool isContinued)
		{
			Vector3 zero = Vector3.zero;
			Vector3 result = Vector3.zero;
			if (!string.IsNullOrEmpty(stageEntry.LocationPointName))
			{
				perpStage.transform.position = Vector3.zero;
				perpStage.transform.position = -perpStage.transform.Find(stageEntry.LocationPointName).localPosition;
				zero = perpStage.transform.position;
				result = zero - preStagePos;
			}
			if (_activeStageEntry == null)
			{
				perpStage.Init(stageEntry, baseWeatherName);
			}
			else
			{
				float num = 0f;
				float num2 = 0f;
				ConfigWeatherData fromWeatherData = null;
				ConfigWeatherData configWeatherData = null;
				string text = null;
				if (continueData != null)
				{
					num = continueData.renderingDataContinueTimer;
					num2 = continueData.weatherContinueTimer;
					fromWeatherData = continueData.currentWeatherData;
					configWeatherData = continueData.continueWeatherData;
					text = continueData.continueWeatherName;
				}
				if (_activeStageEntry.PerpStagePrefabPath != stageEntry.PerpStagePrefabPath)
				{
					if (isContinued)
					{
						if (num > 0f)
						{
							if (text != null)
							{
								perpStage.Init(stageEntry, fromWeatherData, text, num, num2);
							}
							else
							{
								perpStage.Init(stageEntry, fromWeatherData, configWeatherData, num, num2);
							}
						}
						else if (text != null)
						{
							perpStage.Init(stageEntry, text);
						}
						else
						{
							perpStage.Init(stageEntry, configWeatherData);
						}
					}
					else
					{
						perpStage.Init(stageEntry, baseWeatherName);
					}
				}
				else if (isContinued)
				{
					if (num > 0f)
					{
						perpStage.TransitWeatherData(configWeatherData, num, num2);
					}
					else
					{
						perpStage.Reset(stageEntry, configWeatherData);
					}
				}
				else
				{
					perpStage.Reset(stageEntry, WeatherData.GetWeatherDataConfig(baseWeatherName));
				}
				CleanForStageTransit();
			}
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Miscs.LoadResource(stageEntry.GetEnvPrefabPath()));
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			MonoStageEnv component = gameObject.GetComponent<MonoStageEnv>();
			Vector3 vector = new Vector3(0f, -0.05f, 0f);
			Transform transform = Miscs.FindFirstChildGivenLayerAndCollider(gameObject.transform, LayerMask.NameToLayer("StageCollider"));
			if (transform != null)
			{
				transform.position += vector;
			}
			RegisterStage(stageEntry, perpStage, component);
			return result;
		}

		private void CleanForStageTransit()
		{
			Singleton<DynamicObjectManager>.Instance.CleanWhenStageChange();
			Singleton<PropObjectManager>.Instance.CleanWhenStageChange();
			UnityEngine.Object.Destroy(_stageEnv.gameObject);
		}

		public void RegisterStage(StageEntry stageEntry, MonoBasePerpStage perpStage, MonoStageEnv stageEnv)
		{
			_activeStageEntry = stageEntry;
			_perpStage = perpStage;
			_stageEnv = stageEnv;
		}

		public static void SetPerpstageNodeVisibility(MonoBasePerpStage perpStage, StageEntry stageEntry, bool useShowNodes, bool visible)
		{
			if (!useShowNodes)
			{
				if (!string.IsNullOrEmpty(stageEntry.HideNodeNames))
				{
					SetPerpstageNodeVisibilityByNode(perpStage, stageEntry.HideNodeNames, visible);
				}
				if (!string.IsNullOrEmpty(stageEntry.HideNodePrefabPaths))
				{
					SetPerpstageNodeVisibilityByPrefab(perpStage, stageEntry.HideNodePrefabPaths, visible);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(stageEntry.ShowNodeNames))
				{
					SetPerpstageNodeVisibilityByNode(perpStage, stageEntry.ShowNodeNames, visible);
				}
				if (!string.IsNullOrEmpty(stageEntry.ShowNodePrefabPaths))
				{
					SetPerpstageNodeVisibilityByPrefab(perpStage, stageEntry.ShowNodePrefabPaths, visible);
				}
			}
		}

		public static void SetPerpstageNodeVisibilityByNode(MonoBasePerpStage perpStage, string names, bool visible)
		{
			if (!string.IsNullOrEmpty(names))
			{
				string[] array = names.Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					Transform transform = perpStage.transform.Find(array[i]);
					transform.gameObject.SetActive(visible);
				}
				perpStage.transform.gameObject.SetActive(true);
			}
		}

		public static void SetPerpstageNodeVisibilityByPrefab(MonoBasePerpStage perpStage, string prefabPathes, bool visible)
		{
			if (string.IsNullOrEmpty(prefabPathes))
			{
				return;
			}
			string[] array = prefabPathes.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split('/');
				string name = array2[array2.Length - 1];
				if (!visible)
				{
					Transform transform = perpStage.transform.Find(name);
					if (transform != null)
					{
						transform.gameObject.SetActive(visible);
					}
					continue;
				}
				Transform transform2 = perpStage.transform.Find(name);
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(visible);
					continue;
				}
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Miscs.LoadResource(array[i]));
				gameObject.name = name;
				gameObject.transform.SetParent(perpStage.transform, false);
			}
			perpStage.transform.gameObject.SetActive(true);
		}

		public void InitStageEffectSettings()
		{
			AvatarManager instance = Singleton<AvatarManager>.Instance;
			instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>)Delegate.Combine(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(OnLocalAvatarChanged));
			_stageEffectSettingStack = new FixedStack<ConfigStageEffectSetting>(7, OnStageEffectSettingChanged);
			_stageEffectSettingStack.Push(ConfigStageEffectSetting.EMPTY, true);
		}

		public void SetBaseStageEffectSetting(ConfigStageEffectSetting setting)
		{
			if (setting != null)
			{
				_stageEffectSettingStack.Set(0, setting);
			}
		}

		public void ApplyActiveStageEffectSettingAndStartCheckingForChange()
		{
			ApplyStageEffectSetting(_stageEffectSettingStack.value);
			_transitOnChange = true;
		}

		public void OnStageEffectSettingChanged(ConfigStageEffectSetting fromSetting, int oldIx, ConfigStageEffectSetting toSetting, int newIx)
		{
			if (_transitOnChange)
			{
				UnApplyStageEffectSettings(fromSetting);
				ApplyStageEffectSetting(toSetting);
			}
		}

		public int PushStageSettingData(ConfigStageEffectSetting setting)
		{
			return _stageEffectSettingStack.Push(setting);
		}

		public void PopStageSettingData(int ix)
		{
			_stageEffectSettingStack.Pop(ix);
		}

		private void OnLocalAvatarChanged(BaseMonoAvatar from, BaseMonoAvatar to)
		{
			RemoveEntityEffectPredicates(from, _localAvatarPredicates);
			AddEntityEffectPredicates(to, _localAvatarPredicates);
		}

		private void AddEntityEffectPredicates(BaseMonoAnimatorEntity entity, string[] predicates)
		{
			for (int i = 0; i < predicates.Length; i++)
			{
				entity.AddAnimEventPredicate(predicates[i]);
			}
		}

		private void RemoveEntityEffectPredicates(BaseMonoAnimatorEntity entity, string[] predicates)
		{
			for (int i = 0; i < predicates.Length; i++)
			{
				entity.RemoveAnimEventPredicate(predicates[i]);
			}
		}

		private void ApplyStageEffectSetting(ConfigStageEffectSetting setting)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			AddEntityEffectPredicates(localAvatar, setting.LocalAvatarEffectPredicates);
			_localAvatarPredicates = setting.LocalAvatarEffectPredicates;
			if (setting.AvatarColorOverrides.Length <= 0)
			{
				return;
			}
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			for (int i = 0; i < allPlayerAvatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = allPlayerAvatars[i];
				MonoEffectOverride monoEffectOverride = baseMonoAvatar.GetComponent<MonoEffectOverride>();
				if (monoEffectOverride == null)
				{
					monoEffectOverride = baseMonoAvatar.gameObject.AddComponent<MonoEffectOverride>();
				}
				for (int j = 0; j < setting.AvatarColorOverrides.Length; j++)
				{
					ColorOverrideEntry colorOverrideEntry = setting.AvatarColorOverrides[j];
					monoEffectOverride.colorOverrides.Add(colorOverrideEntry.colorOverrideKey, colorOverrideEntry.color);
				}
			}
		}

		private void UnApplyStageEffectSettings(ConfigStageEffectSetting setting)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			RemoveEntityEffectPredicates(localAvatar, setting.LocalAvatarEffectPredicates);
			_localAvatarPredicates = Miscs.EMPTY_STRINGS;
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			for (int i = 0; i < allPlayerAvatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = allPlayerAvatars[i];
				MonoEffectOverride component = baseMonoAvatar.GetComponent<MonoEffectOverride>();
				if (!(component == null))
				{
					for (int j = 0; j < setting.AvatarColorOverrides.Length; j++)
					{
						ColorOverrideEntry colorOverrideEntry = setting.AvatarColorOverrides[j];
						component.colorOverrides.Remove(colorOverrideEntry.colorOverrideKey);
					}
				}
			}
		}
	}
}
