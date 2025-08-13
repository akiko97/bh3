using System.Collections.Generic;

namespace MoleMole
{
	public class LevelTutorialMetaDataReader
	{
		private static List<LevelTutorialMetaData> _itemList;

		private static Dictionary<int, LevelTutorialMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/StageTutorialData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, LevelTutorialMetaData>();
			_itemList = new List<LevelTutorialMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				LevelTutorialMetaData levelTutorialMetaData = new LevelTutorialMetaData(int.Parse(array2[0]), int.Parse(array2[1]), CommonUtils.GetFloatListFromString(array2[2].Trim()), CommonUtils.GetStringListFromString(array2[3].Trim()), array2[4].Trim());
				if (!_itemDict.ContainsKey(levelTutorialMetaData.tutorialId))
				{
					_itemList.Add(levelTutorialMetaData);
					_itemDict.Add(levelTutorialMetaData.tutorialId, levelTutorialMetaData);
				}
			}
		}

		public static LevelTutorialMetaData GetLevelTutorialMetaDataByKey(int tutorialId)
		{
			LevelTutorialMetaData value;
			_itemDict.TryGetValue(tutorialId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static LevelTutorialMetaData TryGetLevelTutorialMetaDataByKey(int tutorialId)
		{
			LevelTutorialMetaData value;
			_itemDict.TryGetValue(tutorialId, out value);
			return value;
		}

		public static List<LevelTutorialMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (LevelTutorialMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
