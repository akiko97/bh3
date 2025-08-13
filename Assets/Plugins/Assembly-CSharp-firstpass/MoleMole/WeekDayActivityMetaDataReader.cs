using System.Collections.Generic;

namespace MoleMole
{
	public class WeekDayActivityMetaDataReader
	{
		private static List<WeekDayActivityMetaData> _itemList;

		private static Dictionary<int, WeekDayActivityMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/WeekDayActivity");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, WeekDayActivityMetaData>();
			_itemList = new List<WeekDayActivityMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				WeekDayActivityMetaData weekDayActivityMetaData = new WeekDayActivityMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), array2[3].Trim(), array2[4].Trim(), array2[5].Trim(), array2[6].Trim(), array2[7].Trim(), array2[8].Trim(), array2[9].Trim(), CommonUtils.GetIntListFromString(array2[10].Trim()), int.Parse(array2[11]), int.Parse(array2[12]), int.Parse(array2[13]), int.Parse(array2[14]));
				if (!_itemDict.ContainsKey(weekDayActivityMetaData.weekDayActivityID))
				{
					_itemList.Add(weekDayActivityMetaData);
					_itemDict.Add(weekDayActivityMetaData.weekDayActivityID, weekDayActivityMetaData);
				}
			}
		}

		public static WeekDayActivityMetaData GetWeekDayActivityMetaDataByKey(int weekDayActivityID)
		{
			WeekDayActivityMetaData value;
			_itemDict.TryGetValue(weekDayActivityID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static WeekDayActivityMetaData TryGetWeekDayActivityMetaDataByKey(int weekDayActivityID)
		{
			WeekDayActivityMetaData value;
			_itemDict.TryGetValue(weekDayActivityID, out value);
			return value;
		}

		public static List<WeekDayActivityMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (WeekDayActivityMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
