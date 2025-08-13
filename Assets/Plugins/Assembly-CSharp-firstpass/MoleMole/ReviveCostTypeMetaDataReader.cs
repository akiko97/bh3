using System.Collections.Generic;

namespace MoleMole
{
	public class ReviveCostTypeMetaDataReader
	{
		private static List<ReviveCostTypeMetaData> _itemList;

		private static Dictionary<int, ReviveCostTypeMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/StageReviveCostData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, ReviveCostTypeMetaData>();
			_itemList = new List<ReviveCostTypeMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				ReviveCostTypeMetaData reviveCostTypeMetaData = new ReviveCostTypeMetaData(int.Parse(array2[0]), new List<int>
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
				if (!_itemDict.ContainsKey(reviveCostTypeMetaData.reviveTimes))
				{
					_itemList.Add(reviveCostTypeMetaData);
					_itemDict.Add(reviveCostTypeMetaData.reviveTimes, reviveCostTypeMetaData);
				}
			}
		}

		public static ReviveCostTypeMetaData GetReviveCostTypeMetaDataByKey(int reviveTimes)
		{
			ReviveCostTypeMetaData value;
			_itemDict.TryGetValue(reviveTimes, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static ReviveCostTypeMetaData TryGetReviveCostTypeMetaDataByKey(int reviveTimes)
		{
			ReviveCostTypeMetaData value;
			_itemDict.TryGetValue(reviveTimes, out value);
			return value;
		}

		public static List<ReviveCostTypeMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (ReviveCostTypeMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
