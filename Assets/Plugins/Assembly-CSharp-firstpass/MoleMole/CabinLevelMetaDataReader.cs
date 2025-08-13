using System.Collections.Generic;

namespace MoleMole
{
	public class CabinLevelMetaDataReader
	{
		private static List<CabinLevelMetaData> _itemList;

		private static Dictionary<KeyValuePair<int, int>, CabinLevelMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CabinLevelData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<KeyValuePair<int, int>, CabinLevelMetaData>();
			_itemList = new List<CabinLevelMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				List<CabinLevelMetaData.CabinUpLevelNeedItem> list2 = new List<CabinLevelMetaData.CabinUpLevelNeedItem>();
				foreach (string item in CommonUtils.GetStringListFromString(array2[5]))
				{
					list2.Add(new CabinLevelMetaData.CabinUpLevelNeedItem(item));
				}
				CabinLevelMetaData cabinLevelMetaData = new CabinLevelMetaData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), list2, int.Parse(array2[6]));
				if (!_itemDict.ContainsKey(new KeyValuePair<int, int>(cabinLevelMetaData.cabinType, cabinLevelMetaData.level)))
				{
					_itemList.Add(cabinLevelMetaData);
					_itemDict.Add(new KeyValuePair<int, int>(cabinLevelMetaData.cabinType, cabinLevelMetaData.level), cabinLevelMetaData);
				}
			}
		}

		public static CabinLevelMetaData GetCabinLevelMetaDataByKey(int cabinType, int level)
		{
			CabinLevelMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<int, int>(cabinType, level), out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinLevelMetaData TryGetCabinLevelMetaDataByKey(int cabinType, int level)
		{
			CabinLevelMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<int, int>(cabinType, level), out value);
			return value;
		}

		public static List<CabinLevelMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinLevelMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
