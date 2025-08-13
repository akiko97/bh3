using System.Collections.Generic;

namespace MoleMole
{
	public class CabinLevelUpTimePriceMetaDataReader
	{
		private static List<CabinLevelUpTimePriceMetaData> _itemList;

		private static Dictionary<int, CabinLevelUpTimePriceMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CabinTimePrice");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, CabinLevelUpTimePriceMetaData>();
			_itemList = new List<CabinLevelUpTimePriceMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				CabinLevelUpTimePriceMetaData cabinLevelUpTimePriceMetaData = new CabinLevelUpTimePriceMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]));
				if (!_itemDict.ContainsKey(cabinLevelUpTimePriceMetaData.timeMin))
				{
					_itemList.Add(cabinLevelUpTimePriceMetaData);
					_itemDict.Add(cabinLevelUpTimePriceMetaData.timeMin, cabinLevelUpTimePriceMetaData);
				}
			}
		}

		public static CabinLevelUpTimePriceMetaData GetCabinLevelUpTimePriceMetaDataByKey(int timeMin)
		{
			CabinLevelUpTimePriceMetaData value;
			_itemDict.TryGetValue(timeMin, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinLevelUpTimePriceMetaData TryGetCabinLevelUpTimePriceMetaDataByKey(int timeMin)
		{
			CabinLevelUpTimePriceMetaData value;
			_itemDict.TryGetValue(timeMin, out value);
			return value;
		}

		public static List<CabinLevelUpTimePriceMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinLevelUpTimePriceMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
