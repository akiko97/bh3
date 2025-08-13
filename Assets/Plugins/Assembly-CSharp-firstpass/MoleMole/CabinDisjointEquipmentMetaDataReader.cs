using System.Collections.Generic;

namespace MoleMole
{
	public class CabinDisjointEquipmentMetaDataReader
	{
		private static List<CabinDisjointEquipmentMetaData> _itemList;

		private static Dictionary<int, CabinDisjointEquipmentMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CabinDisjointEquipmentData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, CabinDisjointEquipmentMetaData>();
			_itemList = new List<CabinDisjointEquipmentMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				List<CabinDisjointEquipmentMetaData.CabinDisjointOutputItem> list2 = new List<CabinDisjointEquipmentMetaData.CabinDisjointOutputItem>();
				foreach (string item in CommonUtils.GetStringListFromString(array2[2]))
				{
					list2.Add(new CabinDisjointEquipmentMetaData.CabinDisjointOutputItem(item));
				}
				CabinDisjointEquipmentMetaData cabinDisjointEquipmentMetaData = new CabinDisjointEquipmentMetaData(int.Parse(array2[0]), int.Parse(array2[1]), list2);
				if (!_itemDict.ContainsKey(cabinDisjointEquipmentMetaData.EquipmentID))
				{
					_itemList.Add(cabinDisjointEquipmentMetaData);
					_itemDict.Add(cabinDisjointEquipmentMetaData.EquipmentID, cabinDisjointEquipmentMetaData);
				}
			}
		}

		public static CabinDisjointEquipmentMetaData GetCabinDisjointEquipmentMetaDataByKey(int EquipmentID)
		{
			CabinDisjointEquipmentMetaData value;
			_itemDict.TryGetValue(EquipmentID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinDisjointEquipmentMetaData TryGetCabinDisjointEquipmentMetaDataByKey(int EquipmentID)
		{
			CabinDisjointEquipmentMetaData value;
			_itemDict.TryGetValue(EquipmentID, out value);
			return value;
		}

		public static List<CabinDisjointEquipmentMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinDisjointEquipmentMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
