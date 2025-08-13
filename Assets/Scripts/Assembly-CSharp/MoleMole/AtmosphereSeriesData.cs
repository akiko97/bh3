using System.Collections.Generic;
using MoleMole.MainMenu;

namespace MoleMole
{
	public class AtmosphereSeriesData
	{
		private static ConfigAtmosphereSeriesEntry[] _items;

		private static ConfigAtmosphereSeriesEntry[] _allLevelClearedItems;

		private static bool _isAllLevelsCleared;

		private static bool _hasCheckedAllLevelsCleared;

		public static ConfigAtmosphereSeriesEntry[] Items
		{
			get
			{
				return (!IsAllLevelsCleared()) ? _items : _allLevelClearedItems;
			}
		}

		public static void ReloadFromFile()
		{
			string atmosphereRegistryPath = GlobalDataManager.metaConfig.atmosphereRegistryPath;
			ConfigAtmosphereSeriesRegistry configAtmosphereSeriesRegistry = ConfigUtil.LoadConfig<ConfigAtmosphereSeriesRegistry>(atmosphereRegistryPath);
			_items = configAtmosphereSeriesRegistry.Items;
			atmosphereRegistryPath = GlobalDataManager.metaConfig.allLevelsClearedAtmosphereRegistryPath;
			configAtmosphereSeriesRegistry = ConfigUtil.LoadConfig<ConfigAtmosphereSeriesRegistry>(atmosphereRegistryPath);
			_allLevelClearedItems = configAtmosphereSeriesRegistry.Items;
		}

		private static ChapterDataItem GetTheLastStoryChapter(List<ChapterDataItem> ls)
		{
			int num = -1;
			ChapterDataItem result = null;
			foreach (ChapterDataItem l in ls)
			{
				if (l.ChapterType == ChapterDataItem.ChpaterType.MainStory && l.chapterId > num)
				{
					num = l.chapterId;
					result = l;
				}
			}
			return result;
		}

		public static bool IsAllLevelsCleared()
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Invalid comparison between Unknown and I4
			if (_hasCheckedAllLevelsCleared)
			{
				return _isAllLevelsCleared;
			}
			_isAllLevelsCleared = false;
			try
			{
				LevelModule instance = Singleton<LevelModule>.Instance;
				ChapterDataItem theLastStoryChapter = GetTheLastStoryChapter(instance.AllChapterList);
				List<int> chapterLevelIdList = LevelMetaDataReaderExtend.GetChapterLevelIdList(theLastStoryChapter.chapterId);
				chapterLevelIdList.Sort();
				List<LevelDataItem> allLevelList = theLastStoryChapter.GetAllLevelList();
				foreach (LevelDataItem item in allLevelList)
				{
					if (item.levelId == chapterLevelIdList[chapterLevelIdList.Count - 1])
					{
						_isAllLevelsCleared = (int)item.status != 1;
						break;
					}
				}
				_hasCheckedAllLevelsCleared = true;
			}
			catch
			{
				return false;
			}
			return _isAllLevelsCleared;
		}

		public static string GetPath(int id)
		{
			return Items[id].Path;
		}

		public static int GetId(string path)
		{
			for (int i = 0; i < Items.Length; i++)
			{
				if (Items[i].Path == path)
				{
					return i;
				}
			}
			return -1;
		}

		public static int GetIdRandomly()
		{
			int[] array = new int[Items.Length];
			for (int i = 0; i < Items.Length; i++)
			{
				array[i] = Items[i].ChooseRate;
			}
			return ConfigAtmosphereUtil.ChooseRandomly(array);
		}

		public static string GetPathRandomly()
		{
			int idRandomly = GetIdRandomly();
			return GetPath(idRandomly);
		}

		public static int GetNextId(int curId)
		{
			int num = curId + 1;
			if (num >= Items.Length)
			{
				num = 0;
			}
			return num;
		}
	}
}
