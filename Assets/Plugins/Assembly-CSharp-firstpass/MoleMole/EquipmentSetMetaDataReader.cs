using System.Collections.Generic;

namespace MoleMole
{
	public class EquipmentSetMetaDataReader
	{
		private static List<EquipmentSetMetaData> _itemList;

		private static Dictionary<int, EquipmentSetMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/EquipmentSetData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, EquipmentSetMetaData>();
			_itemList = new List<EquipmentSetMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				EquipmentSetMetaData equipmentSetMetaData = new EquipmentSetMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), int.Parse(array2[3]), int.Parse(array2[4]), float.Parse(array2[5]), float.Parse(array2[6]), float.Parse(array2[7]), float.Parse(array2[8]), float.Parse(array2[9]), float.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]), float.Parse(array2[13]), float.Parse(array2[14]), float.Parse(array2[15]), float.Parse(array2[16]), float.Parse(array2[17]), float.Parse(array2[18]), int.Parse(array2[19]), int.Parse(array2[20]), float.Parse(array2[21]), float.Parse(array2[22]), float.Parse(array2[23]), float.Parse(array2[24]), float.Parse(array2[25]), float.Parse(array2[26]));
				if (!_itemDict.ContainsKey(equipmentSetMetaData.ID))
				{
					_itemList.Add(equipmentSetMetaData);
					_itemDict.Add(equipmentSetMetaData.ID, equipmentSetMetaData);
				}
			}
		}

		public static EquipmentSetMetaData GetEquipmentSetMetaDataByKey(int ID)
		{
			EquipmentSetMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static EquipmentSetMetaData TryGetEquipmentSetMetaDataByKey(int ID)
		{
			EquipmentSetMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<EquipmentSetMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (EquipmentSetMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
