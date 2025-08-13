using System.Collections.Generic;

namespace MoleMole
{
	public class ChapterDataItem
	{
		public enum ChpaterType
		{
			MainStory = 1,
			Event = 2,
			SpecialStory = 3
		}

		public int chapterId;

		private ChapterMetaData _metaData;

		public bool Unlocked;

		private Dictionary<LevelDiffculty, List<LevelDataItem>> _chapterLevelDict;

		public string ChapterPrefabPath
		{
			get
			{
				return "UI/Menus/Widget/Map/Chapter/ChapterMap_" + chapterId;
			}
		}

		public string Title
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.title);
			}
		}

		public string BtnPointName
		{
			get
			{
				return "BtnPoint_" + chapterId;
			}
		}

		public List<int> MonsterIDList
		{
			get
			{
				return _metaData.monsterList;
			}
		}

		public ChpaterType ChapterType
		{
			get
			{
				return (ChpaterType)_metaData.chapterType;
			}
		}

		public string CoverPic
		{
			get
			{
				return _metaData.coverPicture;
			}
		}

		public ChapterDataItem(int chapterId)
		{
			this.chapterId = chapterId;
			_metaData = ChapterMetaDataReader.GetChapterMetaDataByKey(chapterId);
			Unlocked = false;
			_chapterLevelDict = new Dictionary<LevelDiffculty, List<LevelDataItem>>();
		}

		public List<LevelDiffculty> GetLevelDifficultyListInChapter()
		{
			List<LevelDiffculty> list = new List<LevelDiffculty>(_chapterLevelDict.Keys);
			list.Sort();
			return list;
		}

		public bool HasLevelsOfDifficulty(LevelDiffculty difficulty)
		{
			return _chapterLevelDict.ContainsKey(difficulty);
		}

		public bool HasLevelsOfAct(LevelDiffculty difficulty, int act)
		{
			return GetLevelOfActs(difficulty).ContainsKey(act);
		}

		public List<LevelDataItem> GetLevelList(LevelDiffculty diffcult = LevelDiffculty.Normal)
		{
			if (_chapterLevelDict.ContainsKey(diffcult))
			{
				return _chapterLevelDict[diffcult];
			}
			return new List<LevelDataItem>();
		}

		public Dictionary<int, List<LevelDataItem>> GetLevelOfActs(LevelDiffculty difficulty = LevelDiffculty.Normal)
		{
			Dictionary<int, List<LevelDataItem>> dictionary = new Dictionary<int, List<LevelDataItem>>();
			foreach (LevelDataItem level in GetLevelList(difficulty))
			{
				if (!dictionary.ContainsKey(level.ActID))
				{
					dictionary.Add(level.ActID, new List<LevelDataItem>());
				}
				dictionary[level.ActID].Add(level);
			}
			return dictionary;
		}

		public List<LevelDataItem> GetAllLevelList()
		{
			List<LevelDataItem> list = new List<LevelDataItem>();
			foreach (LevelDiffculty key in _chapterLevelDict.Keys)
			{
				list.AddRange(GetLevelList(key));
			}
			return list;
		}

		public List<LevelDataItem> AddLevel(LevelDataItem m_level)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)m_level.LevelType == 1)
			{
				return AddStoryLevel(m_level);
			}
			List<LevelDataItem> list = new List<LevelDataItem>();
			list.Add(m_level);
			return list;
		}

		private List<LevelDataItem> AddStoryLevel(LevelDataItem m_level)
		{
			List<LevelDataItem> list = new List<LevelDataItem>();
			List<int> chapterLevelIdList = LevelMetaDataReaderExtend.GetChapterLevelIdList(m_level.ChapterID);
			foreach (int item in chapterLevelIdList)
			{
				LevelDataItem levelDataItem = ((m_level.levelId != item) ? new LevelDataItem(item) : m_level);
				if (!_chapterLevelDict.ContainsKey(levelDataItem.Diffculty))
				{
					_chapterLevelDict.Add(levelDataItem.Diffculty, new List<LevelDataItem>());
				}
				_chapterLevelDict[levelDataItem.Diffculty].Add(levelDataItem);
				list.Add(levelDataItem);
			}
			return list;
		}

		public int GetTotalFinishedChanllengeNum(LevelDiffculty difficulty)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Invalid comparison between Unknown and I4
			int num = 0;
			List<LevelDataItem> levelList = GetLevelList(difficulty);
			int i = 0;
			for (int count = levelList.Count; i < count; i++)
			{
				if ((int)levelList[i].status != 1)
				{
					num += levelList[i].challengeList.FindAll((LevelChallengeDataItem x) => x.Finished).Count;
				}
			}
			return num;
		}
	}
}
