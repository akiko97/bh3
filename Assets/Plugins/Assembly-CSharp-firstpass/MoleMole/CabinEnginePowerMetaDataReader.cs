using System.Collections.Generic;

namespace MoleMole
{
	public class CabinEnginePowerMetaDataReader
	{
		private static List<CabinEnginePowerMetaData> _itemList;

		private static Dictionary<int, CabinEnginePowerMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/PowerCostData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, CabinEnginePowerMetaData>();
			_itemList = new List<CabinEnginePowerMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				CabinEnginePowerMetaData cabinEnginePowerMetaData = new CabinEnginePowerMetaData(int.Parse(array2[0]), int.Parse(array2[1]));
				if (!_itemDict.ContainsKey(cabinEnginePowerMetaData.level))
				{
					_itemList.Add(cabinEnginePowerMetaData);
					_itemDict.Add(cabinEnginePowerMetaData.level, cabinEnginePowerMetaData);
				}
			}
		}

		public static CabinEnginePowerMetaData GetCabinEnginePowerMetaDataByKey(int level)
		{
			CabinEnginePowerMetaData value;
			_itemDict.TryGetValue(level, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinEnginePowerMetaData TryGetCabinEnginePowerMetaDataByKey(int level)
		{
			CabinEnginePowerMetaData value;
			_itemDict.TryGetValue(level, out value);
			return value;
		}

		public static List<CabinEnginePowerMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinEnginePowerMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
