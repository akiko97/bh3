using System.Collections.Generic;

namespace MoleMole
{
	public class LevelResetCostMetaDataReader
	{
		private static List<LevelResetCostMetaData> _itemList;

		private static Dictionary<int, LevelResetCostMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/StageResetCostData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, LevelResetCostMetaData>();
			_itemList = new List<LevelResetCostMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				LevelResetCostMetaData levelResetCostMetaData = new LevelResetCostMetaData(int.Parse(array2[0]), new List<int>
				{
					0,
					int.Parse(array2[1]),
					int.Parse(array2[2]),
					int.Parse(array2[3]),
					int.Parse(array2[4]),
					int.Parse(array2[5]),
					int.Parse(array2[6]),
					int.Parse(array2[7]),
					int.Parse(array2[8]),
					int.Parse(array2[9]),
					int.Parse(array2[10])
				});
				if (!_itemDict.ContainsKey(levelResetCostMetaData.times))
				{
					_itemList.Add(levelResetCostMetaData);
					_itemDict.Add(levelResetCostMetaData.times, levelResetCostMetaData);
				}
			}
		}

		public static LevelResetCostMetaData GetLevelResetCostMetaDataByKey(int times)
		{
			LevelResetCostMetaData value;
			_itemDict.TryGetValue(times, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static LevelResetCostMetaData TryGetLevelResetCostMetaDataByKey(int times)
		{
			LevelResetCostMetaData value;
			_itemDict.TryGetValue(times, out value);
			return value;
		}

		public static List<LevelResetCostMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (LevelResetCostMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
