using System.Collections.Generic;

namespace MoleMole
{
	public class LinearMissionDataReader
	{
		private static List<LinearMissionData> _itemList;

		private static Dictionary<int, LinearMissionData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/LinearMissionData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, LinearMissionData>();
			_itemList = new List<LinearMissionData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				LinearMissionData linearMissionData = new LinearMissionData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]));
				if (!_itemDict.ContainsKey(linearMissionData.MissionID))
				{
					_itemList.Add(linearMissionData);
					_itemDict.Add(linearMissionData.MissionID, linearMissionData);
				}
			}
		}

		public static LinearMissionData GetLinearMissionDataByKey(int MissionID)
		{
			LinearMissionData value;
			_itemDict.TryGetValue(MissionID, out value);
			return value;
		}

		public static LinearMissionData TryGetLinearMissionDataByKey(int MissionID)
		{
			LinearMissionData value;
			_itemDict.TryGetValue(MissionID, out value);
			return value;
		}

		public static List<LinearMissionData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (LinearMissionData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
