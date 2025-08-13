using System.Collections.Generic;

namespace MoleMole
{
	public class EquipSkillMetaDataReader
	{
		private static List<EquipSkillMetaData> _itemList;

		private static Dictionary<int, EquipSkillMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/EquipmentSkillData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, EquipSkillMetaData>();
			_itemList = new List<EquipSkillMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				EquipSkillMetaData equipSkillMetaData = new EquipSkillMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), array2[3].Trim());
				if (!_itemDict.ContainsKey(equipSkillMetaData.ID))
				{
					_itemList.Add(equipSkillMetaData);
					_itemDict.Add(equipSkillMetaData.ID, equipSkillMetaData);
				}
			}
		}

		public static EquipSkillMetaData GetEquipSkillMetaDataByKey(int ID)
		{
			EquipSkillMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static EquipSkillMetaData TryGetEquipSkillMetaDataByKey(int ID)
		{
			EquipSkillMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<EquipSkillMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (EquipSkillMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
