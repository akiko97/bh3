using System.Collections.Generic;

namespace MoleMole
{
	public class MonsterConfigMetaDataReader
	{
		private static List<MonsterConfigMetaData> _itemList;

		private static Dictionary<KeyValuePair<string, string>, MonsterConfigMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/MonsterConfigData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<KeyValuePair<string, string>, MonsterConfigMetaData>();
			_itemList = new List<MonsterConfigMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				MonsterConfigMetaData monsterConfigMetaData = new MonsterConfigMetaData(array2[0].Trim(), array2[1].Trim(), array2[2].Trim(), array2[3].Trim(), float.Parse(array2[4]), float.Parse(array2[5]), float.Parse(array2[6]), int.Parse(array2[7]), array2[8].Trim(), array2[9].Trim(), array2[10].Trim());
				if (!_itemDict.ContainsKey(new KeyValuePair<string, string>(monsterConfigMetaData.monsterName, monsterConfigMetaData.typeName)))
				{
					_itemList.Add(monsterConfigMetaData);
					_itemDict.Add(new KeyValuePair<string, string>(monsterConfigMetaData.monsterName, monsterConfigMetaData.typeName), monsterConfigMetaData);
				}
			}
		}

		public static MonsterConfigMetaData GetMonsterConfigMetaDataByKey(string monsterName, string typeName)
		{
			MonsterConfigMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<string, string>(monsterName, typeName), out value);
			if (value == null)
			{
			}
			return value;
		}

		public static MonsterConfigMetaData TryGetMonsterConfigMetaDataByKey(string monsterName, string typeName)
		{
			MonsterConfigMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<string, string>(monsterName, typeName), out value);
			return value;
		}

		public static List<MonsterConfigMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (MonsterConfigMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
