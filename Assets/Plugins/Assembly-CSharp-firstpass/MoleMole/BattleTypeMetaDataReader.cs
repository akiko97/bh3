using System.Collections.Generic;

namespace MoleMole
{
	public class BattleTypeMetaDataReader
	{
		private static List<BattleTypeMetaData> _itemList;

		private static Dictionary<int, BattleTypeMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/BattleTypeData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, BattleTypeMetaData>();
			_itemList = new List<BattleTypeMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				BattleTypeMetaData battleTypeMetaData = new BattleTypeMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim());
				if (!_itemDict.ContainsKey(battleTypeMetaData.battleTypeId))
				{
					_itemList.Add(battleTypeMetaData);
					_itemDict.Add(battleTypeMetaData.battleTypeId, battleTypeMetaData);
				}
			}
		}

		public static BattleTypeMetaData GetBattleTypeMetaDataByKey(int battleTypeId)
		{
			BattleTypeMetaData value;
			_itemDict.TryGetValue(battleTypeId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static BattleTypeMetaData TryGetBattleTypeMetaDataByKey(int battleTypeId)
		{
			BattleTypeMetaData value;
			_itemDict.TryGetValue(battleTypeId, out value);
			return value;
		}

		public static List<BattleTypeMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (BattleTypeMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
