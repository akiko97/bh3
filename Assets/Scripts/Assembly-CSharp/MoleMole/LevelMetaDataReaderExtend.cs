using System.Collections.Generic;

namespace MoleMole
{
	public class LevelMetaDataReaderExtend
	{
		private static Dictionary<int, List<int>> _chapterMap;

		public static void LoadFromFileAndBuildMap()
		{
			LevelMetaDataReader.LoadFromFile();
			List<LevelMetaData> itemList = LevelMetaDataReader.GetItemList();
			_chapterMap = new Dictionary<int, List<int>>();
			foreach (LevelMetaData item in itemList)
			{
				if (!_chapterMap.ContainsKey(item.chapterId))
				{
					_chapterMap.Add(item.chapterId, new List<int>());
				}
				_chapterMap[item.chapterId].Add(item.levelId);
			}
		}

		public static List<int> GetChapterLevelIdList(int chapterId)
		{
			if (!_chapterMap.ContainsKey(chapterId))
			{
				return new List<int>();
			}
			return _chapterMap[chapterId];
		}
	}
}
