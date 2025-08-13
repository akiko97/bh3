using System;
using System.Collections.Generic;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class LevelDataItem
	{
		public int levelId;

		public int progress;

		public int enterTimes;

		public int resetTimes;

		public List<LevelChallengeDataItem> challengeList;

		public List<int> displayDropList;

		public List<int> displayFirstDropList;

		public bool isDropActivityOpen;

		public DateTime dropActivityEndTime;

		public int dropActivityMaxEnterTimes;

		public int dropActivityEnterTimes;

		public List<int> displayBonusDropList;

		public bool isDoubleDrop;

		private LevelMetaData _metaData;

		public bool Initialized;

		public StageStatus status;

		public bool dropDisplayInfoReceived;

		private int _chapterID;

		private int _actID;

		public bool isNewLevel;

		public int ChapterID
		{
			get
			{
				return _chapterID;
			}
			set
			{
				_chapterID = value;
			}
		}

		public int ActID
		{
			get
			{
				return _actID;
			}
			set
			{
				_actID = value;
			}
		}

		public StageType LevelType
		{
			get
			{
				return (StageType)_metaData.type;
			}
		}

		public LevelDiffculty Diffculty
		{
			get
			{
				return (LevelDiffculty)_metaData.difficulty;
			}
		}

		public string StageName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.name);
			}
		}

		public string Title
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.displayTitle);
			}
		}

		public string Desc
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.displayDetail);
			}
		}

		public string BtnPointName
		{
			get
			{
				return "BtnPoint_" + _metaData.levelId % 100;
			}
		}

		public bool IsMultMode
		{
			get
			{
				return _metaData.battleType == 2 || _metaData.battleType == 3;
			}
		}

		public string LevelTypeName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.LevelTypeName[_metaData.type]);
			}
		}

		public int RecommandLv
		{
			get
			{
				return _metaData.recommendPlayerLevel;
			}
		}

		public int StaminaCost
		{
			get
			{
				return _metaData.staminaCost;
			}
		}

		public int NPCHardLevel
		{
			get
			{
				return _metaData.hardLevel;
			}
		}

		public int UnlockPlayerLevel
		{
			get
			{
				return _metaData.unlockPlayerLevel;
			}
		}

		public int UnlockChanllengeNum
		{
			get
			{
				return _metaData.unlockStarNum;
			}
		}

		public float AvatarExpFixed
		{
			get
			{
				return (float)_metaData.avatarExpReward * (1f - _metaData.avatarExpInside);
			}
		}

		public float AvatarExpInside
		{
			get
			{
				return (float)_metaData.avatarExpReward * _metaData.avatarExpInside;
			}
		}

		public float ScoinFixed
		{
			get
			{
				return (float)_metaData.scoinReward * (1f - _metaData.scoinInside);
			}
		}

		public float ScoinInside
		{
			get
			{
				return (float)_metaData.scoinReward * _metaData.scoinInside;
			}
		}

		public List<string> LoseDescList
		{
			get
			{
				return _metaData.loseDescList;
			}
		}

		public string LuaFile
		{
			get
			{
				return _metaData.luaFile;
			}
		}

		public int SectionId
		{
			get
			{
				return _metaData.sectionId;
			}
		}

		public int maxReviveTimes
		{
			get
			{
				return _metaData.reviveTimes;
			}
		}

		public int MaxEnterTimes
		{
			get
			{
				return _metaData.enterTimes;
			}
		}

		public int MaxResetTimes
		{
			get
			{
				return _metaData.resetTimes;
			}
		}

		public int ResetCostType
		{
			get
			{
				return _metaData.resetCostType;
			}
		}

		public string BattleTypePath
		{
			get
			{
				return BattleTypeMetaDataReader.GetBattleTypeMetaDataByKey(_metaData.battleType).iconPath;
			}
		}

		public LevelActor.Mode BattleType
		{
			get
			{
				return (_metaData.battleType <= 100) ? ((LevelActor.Mode)(_metaData.battleType - 1)) : LevelActor.Mode.Single;
			}
		}

		public int MinEnterAvatarNum
		{
			get
			{
				return _metaData.MinEnterNum;
			}
		}

		public bool IsNormalBattleType
		{
			get
			{
				return _metaData.battleType == 1;
			}
		}

		public LevelDataItem(int levelID)
			: this(levelID, LevelMetaDataReader.GetLevelMetaDataByKey(levelID))
		{
		}

		public LevelDataItem(int levelId, LevelMetaData levelMetaData)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			this.levelId = levelId;
			_metaData = levelMetaData;
			Initialized = false;
			status = (StageStatus)1;
			challengeList = new List<LevelChallengeDataItem>();
			foreach (LevelMetaData.LevelChallengeMetaNode challenge in _metaData.challengeList)
			{
				challengeList.Add(new LevelChallengeDataItem(challenge.challengeId, _metaData, challenge.rewardId));
			}
			isNewLevel = true;
			_actID = _metaData.actId;
			_chapterID = _metaData.chapterId;
			isDropActivityOpen = false;
			isDoubleDrop = false;
			displayDropList = new List<int>();
			displayFirstDropList = new List<int>();
			displayBonusDropList = new List<int>();
			dropDisplayInfoReceived = false;
		}

		public Sprite GetBattleTypeSprite()
		{
			return Miscs.GetSpriteByPrefab(BattleTypeMetaDataReader.GetBattleTypeMetaDataByKey(_metaData.battleType).iconPath);
		}

		public Color GetBattleTypeColor()
		{
			return Miscs.ParseColor(BattleTypeMetaDataReader.GetBattleTypeMetaDataByKey(_metaData.battleType).colorCode);
		}

		public Sprite GetBriefPicSprite()
		{
			return Miscs.GetSpriteByPrefab(_metaData.briefPicPath);
		}

		public Sprite GetDetailPicSprite()
		{
			return Miscs.GetSpriteByPrefab(_metaData.detailPicPath);
		}

		public void Init()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if ((int)LevelType == 1)
			{
				status = (StageStatus)2;
			}
			else
			{
				status = (StageStatus)1;
			}
			Initialized = true;
		}

		public void OnProgressUpdate(int preValue, int newValue)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (preValue != newValue && (int)LevelType == 1 && newValue == _metaData.maxProgress)
			{
				status = (StageStatus)3;
			}
		}

		public List<int> GetTrackChallengeIdList()
		{
			List<LevelChallengeDataItem> list = challengeList.FindAll((LevelChallengeDataItem x) => !x.Finished || x.IsSpecialChallenge());
			List<int> list2 = new List<int>();
			foreach (LevelChallengeDataItem item in list)
			{
				list2.Add(item.challengeId);
			}
			return list2;
		}

		public List<int> GetAllChallengeIdList()
		{
			List<int> list = new List<int>();
			foreach (LevelChallengeDataItem challenge in challengeList)
			{
				list.Add(challenge.challengeId);
			}
			return list;
		}

		public int GetHCoinSpentToResetLevel(int resetTimes)
		{
			LevelResetCostMetaData levelResetCostMetaDataByKey = LevelResetCostMetaDataReader.GetLevelResetCostMetaDataByKey(resetTimes);
			return levelResetCostMetaDataByKey.costList[_metaData.resetCostType];
		}

		public int GetReviveCost(int reviveTime)
		{
			if (ReviveCostTypeMetaDataReader.GetItemList().Count < reviveTime)
			{
				reviveTime = ReviveCostTypeMetaDataReader.GetItemList().Count;
			}
			ReviveCostTypeMetaData reviveCostTypeMetaDataByKey = ReviveCostTypeMetaDataReader.GetReviveCostTypeMetaDataByKey(reviveTime);
			return reviveCostTypeMetaDataByKey.costList[_metaData.reviveCostType];
		}
	}
}
