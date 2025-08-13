using System.Collections.Generic;

namespace MoleMole
{
	public class MonsterUIMetaDataReader
	{
		private static List<MonsterUIMetaData> _itemList;

		private static Dictionary<int, MonsterUIMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/MonsterUIData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, MonsterUIMetaData>();
			_itemList = new List<MonsterUIMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				MonsterUIMetaData monsterUIMetaData = new MonsterUIMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), array2[3].Trim(), int.Parse(array2[4]), array2[5].Trim(), float.Parse(array2[6]), float.Parse(array2[7]), float.Parse(array2[8]), float.Parse(array2[9]), float.Parse(array2[10]), CommonUtils.GetIntListFromString(array2[11].Trim()), array2[12].Trim());
				if (!_itemDict.ContainsKey(monsterUIMetaData.monsterID))
				{
					_itemList.Add(monsterUIMetaData);
					_itemDict.Add(monsterUIMetaData.monsterID, monsterUIMetaData);
				}
			}
		}

		public static MonsterUIMetaData GetMonsterUIMetaDataByKey(int monsterID)
		{
			MonsterUIMetaData value;
			_itemDict.TryGetValue(monsterID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static MonsterUIMetaData TryGetMonsterUIMetaDataByKey(int monsterID)
		{
			MonsterUIMetaData value;
			_itemDict.TryGetValue(monsterID, out value);
			return value;
		}

		public static List<MonsterUIMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (MonsterUIMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
