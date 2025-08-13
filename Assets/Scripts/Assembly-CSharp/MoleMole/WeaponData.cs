using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class WeaponData
	{
		public static Dictionary<int, ConfigWeapon> _weaponIDMap;

		private static List<string> _configPathList;

		private static Action<string> _loadJsonConfigCallback = null;

		private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();

		public static void ReloadFromFile()
		{
			_weaponIDMap = new Dictionary<int, ConfigWeapon>();
			string[] weaponRegistryPathes = GlobalDataManager.metaConfig.weaponRegistryPathes;
			foreach (string jsonPath in weaponRegistryPathes)
			{
				ConfigWeaponRegistry configWeaponRegistry = ConfigUtil.LoadJSONConfig<ConfigWeaponRegistry>(jsonPath);
				foreach (ConfigWeapon item in configWeaponRegistry)
				{
					_weaponIDMap.Add(item.WeaponID, item);
				}
			}
			ReloadWeaponConfig();
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
		{
			_loadJsonConfigCallback = finishCallback;
			_configPathList = new List<string>();
			_weaponIDMap = new Dictionary<int, ConfigWeapon>();
			string[] pathes = GlobalDataManager.metaConfig.weaponRegistryPathes;
			if (pathes.Length == 0)
			{
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("WeaponData");
					_loadJsonConfigCallback = null;
				}
				yield break;
			}
			string[] array = pathes;
			foreach (string weaponRegistryPath in array)
			{
				_configPathList.Add(weaponRegistryPath);
			}
			float step = progressSpan / (float)pathes.Length;
			_loadDataBackGroundWorker.StartBackGroundWork("WeaponData");
			string[] array2 = pathes;
			foreach (string weaponRegistryPath2 in array2)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync(weaponRegistryPath2);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null weaponRegistryPath :" + weaponRegistryPath2);
				if (asyncRequest != null)
				{
					yield return asyncRequest.operation;
					if (moveOneStepCallback != null)
					{
						moveOneStepCallback(step);
					}
					ConfigUtil.LoadJSONStrConfigMultiThread<ConfigWeaponRegistry>(asyncRequest.asset.ToString(), _loadDataBackGroundWorker, OnLoadOneJsonConfigFinish, weaponRegistryPath2);
				}
			}
		}

		private static void OnLoadOneJsonConfigFinish(ConfigWeaponRegistry weaponList, string configPath)
		{
			_configPathList.Remove(configPath);
			foreach (ConfigWeapon weapon in weaponList)
			{
				_weaponIDMap.Add(weapon.WeaponID, weapon);
			}
			if (_configPathList.Count == 0)
			{
				_loadDataBackGroundWorker.StopBackGroundWork(false);
				ReloadWeaponConfig();
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("WeaponData");
					_loadJsonConfigCallback = null;
				}
			}
		}

		private static void ReloadWeaponConfig()
		{
			foreach (ConfigWeapon value in _weaponIDMap.Values)
			{
				WeaponMetaData weaponMetaData = WeaponMetaDataReader.TryGetWeaponMetaDataByKey(value.WeaponID);
				if (weaponMetaData != null)
				{
					value.Attach.PrefabPath = weaponMetaData.bodyMod;
					value.Meta = weaponMetaData;
				}
			}
		}

		public static ConfigWeapon GetWeaponConfig(int weaponID)
		{
			return _weaponIDMap[weaponID];
		}

		public static Dictionary<int, ConfigWeapon> GetAllWeaponConfigs()
		{
			return _weaponIDMap;
		}

		public static void AddAvatarWeaponAdditionalAbilities(int weaponID, AvatarActor avatar)
		{
			ConfigWeapon weaponConfig = GetWeaponConfig(weaponID);
			for (int i = 0; i < weaponConfig.AdditionalAbilities.Length; i++)
			{
				ConfigAbility abilityConfig = AbilityData.GetAbilityConfig(weaponConfig.AdditionalAbilities[i].AbilityName, weaponConfig.AdditionalAbilities[i].AbilityOverride);
				avatar.CreateAppliedAbility(abilityConfig);
				if (!string.IsNullOrEmpty(weaponConfig.AdditionalAbilities[i].AbilityReplaceID))
				{
					avatar.abilityIDMap[weaponConfig.AdditionalAbilities[i].AbilityReplaceID] = abilityConfig.AbilityName;
				}
			}
		}

		public static void WeaponModelAndEffectAttach(int weaponID, string avatarType, BaseMonoAnimatorEntity entity)
		{
			ConfigWeapon weaponConfig = GetWeaponConfig(weaponID);
			Transform transform = Miscs.LoadResource<GameObject>(weaponConfig.Attach.PrefabPath).transform;
			WeaponAttach.AttachWeaponMesh(weaponConfig, entity, transform, avatarType);
			MonoEffectOverrideSetting component = transform.GetComponent<MonoEffectOverrideSetting>();
			if (component == null && weaponConfig.EffectOverlays == null)
			{
				return;
			}
			MonoEffectOverride monoEffectOverride = entity.GetComponent<MonoEffectOverride>();
			if (monoEffectOverride == null)
			{
				monoEffectOverride = entity.gameObject.AddComponent<MonoEffectOverride>();
			}
			if (component != null)
			{
				for (int i = 0; i < component.materialOverrides.Length; i++)
				{
					MaterialOverrideEntry materialOverrideEntry = component.materialOverrides[i];
					monoEffectOverride.materialOverrides.Add(materialOverrideEntry.materialOverrideKey, materialOverrideEntry.material);
				}
				for (int j = 0; j < component.colorOverrides.Length; j++)
				{
					ColorOverrideEntry colorOverrideEntry = component.colorOverrides[j];
					monoEffectOverride.colorOverrides.Add(colorOverrideEntry.colorOverrideKey, colorOverrideEntry.color);
				}
				for (int k = 0; k < component.floatOverrides.Length; k++)
				{
					FloatOverrideEntry floatOverrideEntry = component.floatOverrides[k];
					monoEffectOverride.floatOverrides.Add(floatOverrideEntry.floatOverrideKey, floatOverrideEntry.value);
				}
			}
			if (weaponConfig.EffectOverlays.Length > 0)
			{
				for (int l = 0; l < weaponConfig.EffectOverlays.Length; l++)
				{
					monoEffectOverride.effectOverlays.Add(weaponConfig.EffectOverlays[l].EffectOverrideKey, weaponConfig.EffectOverlays[l].EffectPattern);
				}
			}
			if (weaponConfig.EffectOverrides.Length > 0)
			{
				for (int m = 0; m < weaponConfig.EffectOverrides.Length; m++)
				{
					monoEffectOverride.effectOverrides.Add(weaponConfig.EffectOverrides[m].EffectOverrideKey, weaponConfig.EffectOverrides[m].EffectPattern);
				}
			}
			weaponConfig.Attach.GetRuntimeWeaponAttachHandler()(weaponConfig, transform, entity, avatarType);
		}

		public static int GetFirstWeaponIDForRole(EntityRoleName role)
		{
			foreach (ConfigWeapon value in _weaponIDMap.Values)
			{
				if (value.OwnerRole == role)
				{
					return value.WeaponID;
				}
			}
			return 0;
		}
	}
}
