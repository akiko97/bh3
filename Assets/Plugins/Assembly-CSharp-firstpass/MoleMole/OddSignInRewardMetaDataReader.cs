using System.Collections.Generic;

namespace MoleMole
{
	public class OddSignInRewardMetaDataReader
	{
		private static List<OddSignInRewardMetaData> _itemList;

		private static Dictionary<int, OddSignInRewardMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/OddSignInRewardData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, OddSignInRewardMetaData>();
			_itemList = new List<OddSignInRewardMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				OddSignInRewardMetaData oddSignInRewardMetaData = new OddSignInRewardMetaData(int.Parse(array2[0]), int.Parse(array2[1]));
				if (!_itemDict.ContainsKey(oddSignInRewardMetaData.day))
				{
					_itemList.Add(oddSignInRewardMetaData);
					_itemDict.Add(oddSignInRewardMetaData.day, oddSignInRewardMetaData);
				}
			}
		}

		public static OddSignInRewardMetaData GetOddSignInRewardMetaDataByKey(int day)
		{
			OddSignInRewardMetaData value;
			_itemDict.TryGetValue(day, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static OddSignInRewardMetaData TryGetOddSignInRewardMetaDataByKey(int day)
		{
			OddSignInRewardMetaData value;
			_itemDict.TryGetValue(day, out value);
			return value;
		}

		public static List<OddSignInRewardMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (OddSignInRewardMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
