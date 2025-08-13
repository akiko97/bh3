using System;
using System.Collections.Generic;

namespace MoleMole
{
	public class MissionDataReader
	{
		private static List<MissionData> _itemList;

		private static Dictionary<int, MissionData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/MissionData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, MissionData>();
			_itemList = new List<MissionData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				MissionData missionData = new MissionData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), int.Parse(array2[3]), array2[4].Trim(), array2[5].Trim(), array2[6].Trim(), int.Parse(array2[7]), int.Parse(array2[8]), array2[9].Trim(), int.Parse(array2[10]), int.Parse(array2[11]), Convert.ToBoolean(int.Parse(array2[12])), int.Parse(array2[13]), int.Parse(array2[14]), CommonUtils.GetIntListFromString(array2[15].Trim()), int.Parse(array2[16]), int.Parse(array2[17]));
				if (!_itemDict.ContainsKey(missionData.id))
				{
					_itemList.Add(missionData);
					_itemDict.Add(missionData.id, missionData);
				}
			}
		}

		public static MissionData GetMissionDataByKey(int id)
		{
			MissionData value;
			_itemDict.TryGetValue(id, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static MissionData TryGetMissionDataByKey(int id)
		{
			MissionData value;
			_itemDict.TryGetValue(id, out value);
			return value;
		}

		public static List<MissionData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (MissionData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
