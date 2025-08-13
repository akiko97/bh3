using System.Collections.Generic;

namespace MoleMole
{
	public class MonsterSkillMetaDataReader
	{
		private static List<MonsterSkillMetaData> _itemList;

		private static Dictionary<int, MonsterSkillMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/MonsterSkillData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, MonsterSkillMetaData>();
			_itemList = new List<MonsterSkillMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				MonsterSkillMetaData monsterSkillMetaData = new MonsterSkillMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim());
				if (!_itemDict.ContainsKey(monsterSkillMetaData.monsterSkillID))
				{
					_itemList.Add(monsterSkillMetaData);
					_itemDict.Add(monsterSkillMetaData.monsterSkillID, monsterSkillMetaData);
				}
			}
		}

		public static MonsterSkillMetaData GetMonsterSkillMetaDataByKey(int monsterSkillID)
		{
			MonsterSkillMetaData value;
			_itemDict.TryGetValue(monsterSkillID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static MonsterSkillMetaData TryGetMonsterSkillMetaDataByKey(int monsterSkillID)
		{
			MonsterSkillMetaData value;
			_itemDict.TryGetValue(monsterSkillID, out value);
			return value;
		}

		public static List<MonsterSkillMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (MonsterSkillMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
