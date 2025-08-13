using System.Collections.Generic;

namespace MoleMole
{
	public class CabinPowerCostMetaDataReader
	{
		private static List<CabinPowerCostMetaData> _itemList;

		private static Dictionary<int, CabinPowerCostMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CabinPowerCostData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, CabinPowerCostMetaData>();
			_itemList = new List<CabinPowerCostMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				CabinPowerCostMetaData cabinPowerCostMetaData = new CabinPowerCostMetaData(int.Parse(array2[0]), int.Parse(array2[1]));
				if (!_itemDict.ContainsKey(cabinPowerCostMetaData.Level))
				{
					_itemList.Add(cabinPowerCostMetaData);
					_itemDict.Add(cabinPowerCostMetaData.Level, cabinPowerCostMetaData);
				}
			}
		}

		public static CabinPowerCostMetaData GetCabinPowerCostMetaDataByKey(int Level)
		{
			CabinPowerCostMetaData value;
			_itemDict.TryGetValue(Level, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinPowerCostMetaData TryGetCabinPowerCostMetaDataByKey(int Level)
		{
			CabinPowerCostMetaData value;
			_itemDict.TryGetValue(Level, out value);
			return value;
		}

		public static List<CabinPowerCostMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinPowerCostMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
