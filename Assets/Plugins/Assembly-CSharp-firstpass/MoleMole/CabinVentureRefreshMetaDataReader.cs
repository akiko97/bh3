using System.Collections.Generic;

namespace MoleMole
{
	public class CabinVentureRefreshMetaDataReader
	{
		private static List<CabinVentureRefreshDataMetaData> _itemList;

		private static Dictionary<int, CabinVentureRefreshDataMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CabinVentureRefreshData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, CabinVentureRefreshDataMetaData>();
			_itemList = new List<CabinVentureRefreshDataMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				CabinVentureRefreshDataMetaData cabinVentureRefreshDataMetaData = new CabinVentureRefreshDataMetaData(int.Parse(array2[0]), new List<int>
				{
					0,
					int.Parse(array2[1]),
					int.Parse(array2[2]),
					int.Parse(array2[3]),
					int.Parse(array2[4]),
					int.Parse(array2[5]),
					int.Parse(array2[6]),
					int.Parse(array2[7]),
					int.Parse(array2[8]),
					int.Parse(array2[9]),
					int.Parse(array2[10])
				});
				if (!_itemDict.ContainsKey(cabinVentureRefreshDataMetaData.level))
				{
					_itemList.Add(cabinVentureRefreshDataMetaData);
					_itemDict.Add(cabinVentureRefreshDataMetaData.level, cabinVentureRefreshDataMetaData);
				}
			}
		}

		public static CabinVentureRefreshDataMetaData GetCabinVentureRefreshDataMetaDataByKey(int level)
		{
			CabinVentureRefreshDataMetaData value;
			_itemDict.TryGetValue(level, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinVentureRefreshDataMetaData TryGetCabinVentureRefreshDataMetaDataByKey(int level)
		{
			CabinVentureRefreshDataMetaData value;
			_itemDict.TryGetValue(level, out value);
			return value;
		}

		public static List<CabinVentureRefreshDataMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinVentureRefreshDataMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
