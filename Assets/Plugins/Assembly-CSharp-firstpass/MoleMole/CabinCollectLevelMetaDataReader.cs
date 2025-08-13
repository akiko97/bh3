using System.Collections.Generic;

namespace MoleMole
{
	public class CabinCollectLevelMetaDataReader
	{
		private static List<CabinCollectLevelDataMetaData> _itemList;

		private static Dictionary<int, CabinCollectLevelDataMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CabinCollectCabinData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, CabinCollectLevelDataMetaData>();
			_itemList = new List<CabinCollectLevelDataMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				CabinCollectLevelDataMetaData cabinCollectLevelDataMetaData = new CabinCollectLevelDataMetaData(int.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]), float.Parse(array2[3]), int.Parse(array2[4]), float.Parse(array2[5]));
				if (!_itemDict.ContainsKey(cabinCollectLevelDataMetaData.level))
				{
					_itemList.Add(cabinCollectLevelDataMetaData);
					_itemDict.Add(cabinCollectLevelDataMetaData.level, cabinCollectLevelDataMetaData);
				}
			}
		}

		public static CabinCollectLevelDataMetaData GetCabinCollectLevelDataMetaDataByKey(int level)
		{
			CabinCollectLevelDataMetaData value;
			_itemDict.TryGetValue(level, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinCollectLevelDataMetaData TryGetCabinCollectLevelDataMetaDataByKey(int level)
		{
			CabinCollectLevelDataMetaData value;
			_itemDict.TryGetValue(level, out value);
			return value;
		}

		public static List<CabinCollectLevelDataMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinCollectLevelDataMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
