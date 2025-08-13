using System.Collections.Generic;

namespace MoleMole
{
	public class PlayerLevelMetaDataReader
	{
		private static List<PlayerLevelMetaData> _itemList;

		private static Dictionary<int, PlayerLevelMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/PlayerLevelData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, PlayerLevelMetaData>();
			_itemList = new List<PlayerLevelMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				PlayerLevelMetaData playerLevelMetaData = new PlayerLevelMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]));
				if (!_itemDict.ContainsKey(playerLevelMetaData.level))
				{
					_itemList.Add(playerLevelMetaData);
					_itemDict.Add(playerLevelMetaData.level, playerLevelMetaData);
				}
			}
		}

		public static PlayerLevelMetaData GetPlayerLevelMetaDataByKey(int level)
		{
			PlayerLevelMetaData value;
			_itemDict.TryGetValue(level, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static PlayerLevelMetaData TryGetPlayerLevelMetaDataByKey(int level)
		{
			PlayerLevelMetaData value;
			_itemDict.TryGetValue(level, out value);
			return value;
		}

		public static List<PlayerLevelMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (PlayerLevelMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
