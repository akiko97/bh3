using System.Collections.Generic;

namespace MoleMole
{
	public class ChapterMetaDataReader
	{
		private static List<ChapterMetaData> _itemList;

		private static Dictionary<int, ChapterMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/ChapterData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, ChapterMetaData>();
			_itemList = new List<ChapterMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				ChapterMetaData chapterMetaData = new ChapterMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), CommonUtils.GetIntListFromString(array2[3].Trim()), int.Parse(array2[4]), array2[5].Trim());
				if (!_itemDict.ContainsKey(chapterMetaData.chapterId))
				{
					_itemList.Add(chapterMetaData);
					_itemDict.Add(chapterMetaData.chapterId, chapterMetaData);
				}
			}
		}

		public static ChapterMetaData GetChapterMetaDataByKey(int chapterId)
		{
			ChapterMetaData value;
			_itemDict.TryGetValue(chapterId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static ChapterMetaData TryGetChapterMetaDataByKey(int chapterId)
		{
			ChapterMetaData value;
			_itemDict.TryGetValue(chapterId, out value);
			return value;
		}

		public static List<ChapterMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (ChapterMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
