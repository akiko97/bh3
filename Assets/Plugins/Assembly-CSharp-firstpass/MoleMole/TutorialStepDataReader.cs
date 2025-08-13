using System;
using System.Collections.Generic;

namespace MoleMole
{
	public class TutorialStepDataReader
	{
		private static List<TutorialStepData> _itemList;

		private static Dictionary<int, TutorialStepData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TutorialStepData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, TutorialStepData>();
			_itemList = new List<TutorialStepData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				TutorialStepData tutorialStepData = new TutorialStepData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), array2[3].Trim(), int.Parse(array2[4]), Convert.ToBoolean(int.Parse(array2[5])), array2[6].Trim(), array2[7].Trim(), int.Parse(array2[8]), int.Parse(array2[9]), float.Parse(array2[10]), CommonUtils.GetIntListFromString(array2[11].Trim()), CommonUtils.GetIntListFromString(array2[12].Trim()));
				if (!_itemDict.ContainsKey(tutorialStepData.id))
				{
					_itemList.Add(tutorialStepData);
					_itemDict.Add(tutorialStepData.id, tutorialStepData);
				}
			}
		}

		public static TutorialStepData GetTutorialStepDataByKey(int id)
		{
			TutorialStepData value;
			_itemDict.TryGetValue(id, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static TutorialStepData TryGetTutorialStepDataByKey(int id)
		{
			TutorialStepData value;
			_itemDict.TryGetValue(id, out value);
			return value;
		}

		public static List<TutorialStepData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (TutorialStepData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
