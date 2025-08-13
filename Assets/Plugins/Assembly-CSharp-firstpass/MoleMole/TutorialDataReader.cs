using System;
using System.Collections.Generic;

namespace MoleMole
{
	public class TutorialDataReader
	{
		private static List<TutorialData> _itemList;

		private static Dictionary<int, TutorialData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TutorialData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, TutorialData>();
			_itemList = new List<TutorialData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				TutorialData tutorialData = new TutorialData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), Convert.ToBoolean(int.Parse(array2[3])), Convert.ToBoolean(int.Parse(array2[4])), Convert.ToBoolean(int.Parse(array2[5])), array2[6].Trim(), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]));
				if (!_itemDict.ContainsKey(tutorialData.id))
				{
					_itemList.Add(tutorialData);
					_itemDict.Add(tutorialData.id, tutorialData);
				}
			}
		}

		public static TutorialData GetTutorialDataByKey(int id)
		{
			TutorialData value;
			_itemDict.TryGetValue(id, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static TutorialData TryGetTutorialDataByKey(int id)
		{
			TutorialData value;
			_itemDict.TryGetValue(id, out value);
			return value;
		}

		public static List<TutorialData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (TutorialData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
