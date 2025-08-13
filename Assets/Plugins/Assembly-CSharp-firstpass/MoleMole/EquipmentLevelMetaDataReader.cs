using System.Collections.Generic;

namespace MoleMole
{
	public class EquipmentLevelMetaDataReader
	{
		private static List<EquipmentLevelMetaData> _itemList;

		private static Dictionary<int, EquipmentLevelMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/EquipmentLevelData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, EquipmentLevelMetaData>();
			_itemList = new List<EquipmentLevelMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				EquipmentLevelMetaData equipmentLevelMetaData = new EquipmentLevelMetaData(int.Parse(array2[0]), new List<int>
				{
					0,
					int.Parse(array2[1]),
					int.Parse(array2[2]),
					int.Parse(array2[3]),
					int.Parse(array2[4]),
					int.Parse(array2[5]),
					int.Parse(array2[6]),
					int.Parse(array2[7])
				}, int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]));
				if (!_itemDict.ContainsKey(equipmentLevelMetaData.level))
				{
					_itemList.Add(equipmentLevelMetaData);
					_itemDict.Add(equipmentLevelMetaData.level, equipmentLevelMetaData);
				}
			}
		}

		public static EquipmentLevelMetaData GetEquipmentLevelMetaDataByKey(int level)
		{
			EquipmentLevelMetaData value;
			_itemDict.TryGetValue(level, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static EquipmentLevelMetaData TryGetEquipmentLevelMetaDataByKey(int level)
		{
			EquipmentLevelMetaData value;
			_itemDict.TryGetValue(level, out value);
			return value;
		}

		public static List<EquipmentLevelMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (EquipmentLevelMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
