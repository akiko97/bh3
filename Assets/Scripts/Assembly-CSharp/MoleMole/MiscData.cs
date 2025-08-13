using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MiscData
	{
		public enum PageInfoKey
		{
			GameEntryPage = 0,
			MainPage = 1,
			AvatarOverviewPage = 2,
			AvatarDetailPage = 3
		}

		private static Dictionary<int, string> _featureUnlockLevelDict;

		private static Dictionary<int, string> _currencyIconPathDict;

		private static Dictionary<int, string> _gachaTicketIconPathDict;

		public static ConfigMisc Config { get; private set; }

		public static void LoadFromFile()
		{
			Config = ConfigUtil.LoadJSONConfig<ConfigMisc>("Data/MiscData");
			_featureUnlockLevelDict = new Dictionary<int, string>();
			foreach (KeyValuePair<string, object> item in Config.FeatureUnlockLevel)
			{
				_featureUnlockLevelDict[int.Parse(item.Key)] = item.Value.ToString();
			}
			_currencyIconPathDict = new Dictionary<int, string>();
			foreach (KeyValuePair<string, object> item2 in Config.CurrencyIconPath)
			{
				_currencyIconPathDict[int.Parse(item2.Key)] = item2.Value.ToString();
			}
			_gachaTicketIconPathDict = new Dictionary<int, string>();
			foreach (KeyValuePair<string, object> item3 in Config.GachaTicketIconPath)
			{
				_gachaTicketIconPathDict[int.Parse(item3.Key)] = item3.Value.ToString();
			}
		}

		public static Color GetColor(string key)
		{
			string hexString = Config.Color[key].ToString();
			return Miscs.ParseColor(hexString);
		}

		public static string AddColor(string key, string text)
		{
			Color color = GetColor(key);
			string arg = ColorUtility.ToHtmlStringRGBA(color);
			return string.Format("<color=#{0}>{1}</color>", arg, text);
		}

		public static int GetEquipPowerUpResultIndex(int boostRate)
		{
			for (int i = 0; i < Config.EquipPowerUpBoostRateResult.Count; i++)
			{
				if (boostRate >= Config.EquipPowerUpBoostRateResult[i] && (i == Config.EquipPowerUpBoostRateResult.Count - 1 || boostRate < Config.EquipPowerUpBoostRateResult[i + 1]))
				{
					return i + 1;
				}
			}
			return 1;
		}

		public static List<string> GetNewFeatures(int levelBefore, int levelNow)
		{
			List<string> list = new List<string>();
			if (levelBefore < levelNow)
			{
				foreach (KeyValuePair<int, string> item in _featureUnlockLevelDict)
				{
					if (item.Key > levelBefore && item.Key <= levelNow)
					{
						list.Add(item.Value);
					}
				}
			}
			return list;
		}

		public static string GetCurrencyIconPath(int metaID)
		{
			if (!_currencyIconPathDict.ContainsKey(metaID))
			{
				return null;
			}
			return _currencyIconPathDict[metaID];
		}

		public static string GetGachaTicketIconPath(int metaID)
		{
			if (!_gachaTicketIconPathDict.ContainsKey(metaID))
			{
				return null;
			}
			return _gachaTicketIconPathDict[metaID];
		}

		public static ConfigPageAvatarShowInfo GetPageAvatarShowInfo(PageInfoKey pageKey)
		{
			return Config.PageAvatarShowInfo[(int)pageKey];
		}

		public static ConfigPlotAvatarCameraPosInfo GetPlotAvatarCameraPosInfo()
		{
			return Config.PlotAvatarCameraPosInfo;
		}
	}
}
