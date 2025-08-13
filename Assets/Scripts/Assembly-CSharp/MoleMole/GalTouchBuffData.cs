using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public class GalTouchBuffData
	{
		public static Dictionary<int, ConfigGalTouchBuffEntry> _galTouchBuffMap;

		public static void ReloadFromFile()
		{
			_galTouchBuffMap = new Dictionary<int, ConfigGalTouchBuffEntry>();
			string[] galTouchBuffRegistryPathes = GlobalDataManager.metaConfig.galTouchBuffRegistryPathes;
			foreach (string jsonPath in galTouchBuffRegistryPathes)
			{
				ConfigGalTouchBuffRegistry configGalTouchBuffRegistry = ConfigUtil.LoadJSONConfig<ConfigGalTouchBuffRegistry>(jsonPath);
				foreach (ConfigGalTouchBuffEntry item in configGalTouchBuffRegistry)
				{
					_galTouchBuffMap.Add(item.GalTouchBuffID, item);
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			_galTouchBuffMap = new Dictionary<int, ConfigGalTouchBuffEntry>();
			float step = progressSpan / (float)GlobalDataManager.metaConfig.galTouchBuffRegistryPathes.Length;
			string[] galTouchBuffRegistryPathes = GlobalDataManager.metaConfig.galTouchBuffRegistryPathes;
			foreach (string path in galTouchBuffRegistryPathes)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync(path);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null galTouchBuffRegistryPath :" + path);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				ConfigGalTouchBuffRegistry buffList = ConfigUtil.LoadJSONStrConfig<ConfigGalTouchBuffRegistry>(asyncRequest.asset.ToString());
				foreach (ConfigGalTouchBuffEntry buff in buffList)
				{
					_galTouchBuffMap.Add(buff.GalTouchBuffID, buff);
				}
			}
		}

		public static ConfigGalTouchBuffEntry GetGalTouchBuffEntry(int buffId)
		{
			if (_galTouchBuffMap == null)
			{
				return null;
			}
			if (!_galTouchBuffMap.ContainsKey(buffId))
			{
				return null;
			}
			return _galTouchBuffMap[buffId];
		}

		public static void ApplyGalTouchBuffEntry(AvatarActor avatarActor, int buffId, float calculatedParam1, float calculatedParam2, float calculatedParam3)
		{
			if (buffId > 0)
			{
				ConfigGalTouchBuffEntry galTouchBuffEntry = GetGalTouchBuffEntry(buffId);
				ConfigAbility abilityConfig = AbilityData.GetAbilityConfig(galTouchBuffEntry.AbilityName, galTouchBuffEntry.AbilityOverride);
				Dictionary<string, object> overrideMap = avatarActor.CreateAppliedAbility(abilityConfig);
				if (galTouchBuffEntry.ParamSpecial1 != null)
				{
					AbilityData.SetupParamSpecial(abilityConfig, overrideMap, galTouchBuffEntry.ParamSpecial1, galTouchBuffEntry.ParamMethod1, calculatedParam1);
				}
				if (galTouchBuffEntry.ParamSpecial2 != null)
				{
					AbilityData.SetupParamSpecial(abilityConfig, overrideMap, galTouchBuffEntry.ParamSpecial2, galTouchBuffEntry.ParamMethod2, calculatedParam2);
				}
				if (galTouchBuffEntry.ParamSpecial3 != null)
				{
					AbilityData.SetupParamSpecial(abilityConfig, overrideMap, galTouchBuffEntry.ParamSpecial3, galTouchBuffEntry.ParamMethod3, calculatedParam3);
				}
			}
		}

		public static float GetCalculatedParam(float baseParam, float addParam, int level)
		{
			return baseParam + addParam * (float)(level - 1);
		}
	}
}
