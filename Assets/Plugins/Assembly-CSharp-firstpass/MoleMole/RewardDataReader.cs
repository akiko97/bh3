using System.Collections.Generic;

namespace MoleMole
{
	public class RewardDataReader
	{
		private static List<RewardData> _itemList;

		private static Dictionary<int, RewardData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/RewardData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, RewardData>();
			_itemList = new List<RewardData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				RewardData rewardData = new RewardData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]), int.Parse(array2[13]), int.Parse(array2[14]), int.Parse(array2[15]), int.Parse(array2[16]), int.Parse(array2[17]), int.Parse(array2[18]), int.Parse(array2[19]), int.Parse(array2[20]), int.Parse(array2[21]));
				if (!_itemDict.ContainsKey(rewardData.RewardID))
				{
					_itemList.Add(rewardData);
					_itemDict.Add(rewardData.RewardID, rewardData);
				}
			}
		}

		public static RewardData GetRewardDataByKey(int RewardID)
		{
			RewardData value;
			_itemDict.TryGetValue(RewardID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static RewardData TryGetRewardDataByKey(int RewardID)
		{
			RewardData value;
			_itemDict.TryGetValue(RewardID, out value);
			return value;
		}

		public static List<RewardData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (RewardData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
