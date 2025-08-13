using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public static class EquipmentSkillData
	{
		public static Dictionary<int, ConfigEquipmentSkillEntry> _equipSkillMap;

		private static List<string> _configPathList;

		private static Action<string> _loadJsonConfigCallback = null;

		private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();

		public static void ReloadFromFile()
		{
			_equipSkillMap = new Dictionary<int, ConfigEquipmentSkillEntry>();
			string[] equipmentSkillRegistryPathes = GlobalDataManager.metaConfig.equipmentSkillRegistryPathes;
			foreach (string jsonPath in equipmentSkillRegistryPathes)
			{
				ConfigEquipmentSkillRegistry configEquipmentSkillRegistry = ConfigUtil.LoadJSONConfig<ConfigEquipmentSkillRegistry>(jsonPath);
				foreach (ConfigEquipmentSkillEntry item in configEquipmentSkillRegistry)
				{
					_equipSkillMap.Add(item.EquipmentSkillID, item);
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
		{
			_loadJsonConfigCallback = finishCallback;
			_configPathList = new List<string>();
			_equipSkillMap = new Dictionary<int, ConfigEquipmentSkillEntry>();
			string[] pathes = GlobalDataManager.metaConfig.equipmentSkillRegistryPathes;
			if (pathes.Length == 0)
			{
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("EquipmentSkillData");
					_loadJsonConfigCallback = null;
				}
				yield break;
			}
			string[] array = pathes;
			foreach (string equipRegistryPath in array)
			{
				_configPathList.Add(equipRegistryPath);
			}
			float step = progressSpan / (float)pathes.Length;
			_loadDataBackGroundWorker.StartBackGroundWork("EquipmentSkillData");
			string[] array2 = pathes;
			foreach (string equipRegistryPath2 in array2)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync(equipRegistryPath2);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null equipRegistryPath :" + equipRegistryPath2);
				if (asyncRequest != null)
				{
					yield return asyncRequest.operation;
					if (moveOneStepCallback != null)
					{
						moveOneStepCallback(step);
					}
					ConfigUtil.LoadJSONStrConfigMultiThread<ConfigEquipmentSkillRegistry>(asyncRequest.asset.ToString(), _loadDataBackGroundWorker, OnLoadOneJsonConfigFinish, equipRegistryPath2);
				}
			}
		}

		private static void OnLoadOneJsonConfigFinish(ConfigEquipmentSkillRegistry weaponList, string configPath)
		{
			_configPathList.Remove(configPath);
			foreach (ConfigEquipmentSkillEntry weapon in weaponList)
			{
				_equipSkillMap.Add(weapon.EquipmentSkillID, weapon);
			}
			if (_configPathList.Count == 0)
			{
				_loadDataBackGroundWorker.StopBackGroundWork(false);
				if (_loadJsonConfigCallback != null)
				{
					_loadJsonConfigCallback("EquipmentSkillData");
					_loadJsonConfigCallback = null;
				}
			}
		}

		public static ConfigEquipmentSkillEntry GetEquipmentSkillConfig(int equipSkillID)
		{
			return _equipSkillMap[equipSkillID];
		}

		public static void ApplyEquipSkillEntry(AvatarActor avatarActor, int equipSkillID, float calculatedParam1, float calculatedParam2, float calculatedParam3, out ConfigEquipmentSkillEntry skillConfig)
		{
			skillConfig = null;
			if (equipSkillID > 0 && _equipSkillMap.ContainsKey(equipSkillID))
			{
				skillConfig = GetEquipmentSkillConfig(equipSkillID);
				ConfigAbility abilityConfig = AbilityData.GetAbilityConfig(skillConfig.AbilityName, skillConfig.AbilityOverride);
				Dictionary<string, object> overrideMap = avatarActor.CreateAppliedAbility(abilityConfig);
				if (skillConfig.ParamSpecial1 != null)
				{
					AbilityData.SetupParamSpecial(abilityConfig, overrideMap, skillConfig.ParamSpecial1, skillConfig.ParamMethod1, calculatedParam1);
				}
				if (skillConfig.ParamSpecial2 != null)
				{
					AbilityData.SetupParamSpecial(abilityConfig, overrideMap, skillConfig.ParamSpecial2, skillConfig.ParamMethod2, calculatedParam2);
				}
				if (skillConfig.ParamSpecial3 != null)
				{
					AbilityData.SetupParamSpecial(abilityConfig, overrideMap, skillConfig.ParamSpecial3, skillConfig.ParamMethod3, calculatedParam3);
				}
			}
		}

		public static void AddAvatarWeaponEquipSkillAbilities(AvatarDataItem avatarDataItem, AvatarActor avatarActor, ref List<ConfigEquipmentSkillEntry> skillEntryList)
		{
			WeaponDataItem weapon = avatarDataItem.GetWeapon();
			List<EquipSkillDataItem> skills = weapon.skills;
			for (int i = 0; i < skills.Count; i++)
			{
				EquipSkillDataItem equipSkillDataItem = skills[i];
				ConfigEquipmentSkillEntry skillConfig = null;
				ApplyEquipSkillEntry(avatarActor, equipSkillDataItem.ID, equipSkillDataItem.GetSkillParam1(weapon.level), equipSkillDataItem.GetSkillParam2(weapon.level), equipSkillDataItem.GetSkillParam3(weapon.level), out skillConfig);
				if (skillConfig != null)
				{
					skillEntryList.Add(skillConfig);
				}
			}
			WeaponData.AddAvatarWeaponAdditionalAbilities(weapon.ID, avatarActor);
		}

		public static void AddAvatarStigmataEquipSkillAbilities(AvatarDataItem avatarDataItem, AvatarActor avatarActor, ref List<ConfigEquipmentSkillEntry> skillEntryList)
		{
			List<StigmataDataItem> list = avatarDataItem.GetStigmataList().FindAll((StigmataDataItem x) => x != null);
			int num = 0;
			for (int count = list.Count; num < count; num++)
			{
				List<EquipSkillDataItem> skillsWithAffix = list[num].GetSkillsWithAffix();
				for (int num2 = 0; num2 < skillsWithAffix.Count; num2++)
				{
					EquipSkillDataItem equipSkillDataItem = skillsWithAffix[num2];
					ConfigEquipmentSkillEntry skillConfig = null;
					ApplyEquipSkillEntry(avatarActor, equipSkillDataItem.ID, equipSkillDataItem.GetSkillParam1(list[num].level), equipSkillDataItem.GetSkillParam2(list[num].level), equipSkillDataItem.GetSkillParam3(list[num].level), out skillConfig);
					if (skillConfig != null)
					{
						skillEntryList.Add(skillConfig);
					}
				}
			}
		}

		public static void AddAvatarSetEquipSkillAbilities(AvatarDataItem avatarDataItem, AvatarActor avatarActor, ref List<ConfigEquipmentSkillEntry> skillEntryList)
		{
			EquipSetDataItem ownEquipSetData = avatarDataItem.GetOwnEquipSetData();
			if (ownEquipSetData == null)
			{
				return;
			}
			Dictionary<int, EquipSkillDataItem>.ValueCollection values = avatarDataItem.GetOwnEquipSetData().GetOwnSetSkills().Values;
			foreach (EquipSkillDataItem item in values)
			{
				ConfigEquipmentSkillEntry skillConfig = null;
				ApplyEquipSkillEntry(avatarActor, item.ID, item.GetSkillParam1(1), item.GetSkillParam2(1), item.GetSkillParam3(1), out skillConfig);
				if (skillConfig != null)
				{
					skillEntryList.Add(skillConfig);
				}
			}
		}
	}
}
