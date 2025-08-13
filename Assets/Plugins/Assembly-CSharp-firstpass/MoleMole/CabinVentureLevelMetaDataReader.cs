using System.Collections.Generic;

namespace MoleMole
{
	public class CabinVentureLevelMetaDataReader
	{
		private static List<CabinVentureLevelDataMetaData> _itemList;

		private static Dictionary<int, CabinVentureLevelDataMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/VentureCabinBaseData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, CabinVentureLevelDataMetaData>();
			_itemList = new List<CabinVentureLevelDataMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				List<CabinVentureLevelDataMetaData.UnlockVentureItem> list2 = new List<CabinVentureLevelDataMetaData.UnlockVentureItem>();
				foreach (string item in CommonUtils.GetStringListFromString(array2[3]))
				{
					list2.Add(new CabinVentureLevelDataMetaData.UnlockVentureItem(item));
				}
				CabinVentureLevelDataMetaData cabinVentureLevelDataMetaData = new CabinVentureLevelDataMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), list2, int.Parse(array2[4]));
				if (!_itemDict.ContainsKey(cabinVentureLevelDataMetaData.level))
				{
					_itemList.Add(cabinVentureLevelDataMetaData);
					_itemDict.Add(cabinVentureLevelDataMetaData.level, cabinVentureLevelDataMetaData);
				}
			}
		}

		public static CabinVentureLevelDataMetaData GetCabinVentureLevelDataMetaDataByKey(int level)
		{
			CabinVentureLevelDataMetaData value;
			_itemDict.TryGetValue(level, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinVentureLevelDataMetaData TryGetCabinVentureLevelDataMetaDataByKey(int level)
		{
			CabinVentureLevelDataMetaData value;
			_itemDict.TryGetValue(level, out value);
			return value;
		}

		public static List<CabinVentureLevelDataMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinVentureLevelDataMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
