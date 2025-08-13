using System.Collections.Generic;

namespace MoleMole
{
	public class EvenSignInRewardMetaDataReader
	{
		private static List<EvenSignInRewardMetaData> _itemList;

		private static Dictionary<int, EvenSignInRewardMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/EvenSignInRewardData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, EvenSignInRewardMetaData>();
			_itemList = new List<EvenSignInRewardMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				EvenSignInRewardMetaData evenSignInRewardMetaData = new EvenSignInRewardMetaData(int.Parse(array2[0]), int.Parse(array2[1]));
				if (!_itemDict.ContainsKey(evenSignInRewardMetaData.day))
				{
					_itemList.Add(evenSignInRewardMetaData);
					_itemDict.Add(evenSignInRewardMetaData.day, evenSignInRewardMetaData);
				}
			}
		}

		public static EvenSignInRewardMetaData GetEvenSignInRewardMetaDataByKey(int day)
		{
			EvenSignInRewardMetaData value;
			_itemDict.TryGetValue(day, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static EvenSignInRewardMetaData TryGetEvenSignInRewardMetaDataByKey(int day)
		{
			EvenSignInRewardMetaData value;
			_itemDict.TryGetValue(day, out value);
			return value;
		}

		public static List<EvenSignInRewardMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (EvenSignInRewardMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
