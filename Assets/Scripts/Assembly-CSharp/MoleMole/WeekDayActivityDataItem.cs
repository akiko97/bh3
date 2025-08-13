using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class WeekDayActivityDataItem : ActivityDataItemBase
	{
		private WeekDayActivityMetaData _metaData;

		public int enterTimes;

		public int maxEnterTimes
		{
			get
			{
				return _metaData.maxEnterTimes;
			}
		}

		public WeekDayActivityDataItem(int activityID)
		{
			_metaData = WeekDayActivityMetaDataReader.GetWeekDayActivityMetaDataByKey(activityID);
			_status = Status.Unavailable;
		}

		public override int GetActivityID()
		{
			return _metaData.weekDayActivityID;
		}

		public override ActivityType GetActivityType()
		{
			return (ActivityType)_metaData.activityType;
		}

		public int GetSeriesID()
		{
			return _metaData.seriesID;
		}

		public override string GetActitityTitle()
		{
			return LocalizationGeneralLogic.GetText(_metaData.title);
		}

		public override string GetActivityDescription()
		{
			return LocalizationGeneralLogic.GetText(_metaData.desc);
		}

		public string GetActivityLockDescription()
		{
			return LocalizationGeneralLogic.GetText(_metaData.descLock);
		}

		public override string GetActivityEnterImgPath()
		{
			return _metaData.enterImgPath;
		}

		public override List<int> GetLevelIDList()
		{
			return _metaData.levelIDList;
		}

		public override int GetMinPlayerLevelLimit()
		{
			return _metaData.minPlayerLevel;
		}

		public string GetSmallImgPath()
		{
			return _metaData.smallImgPath;
		}

		public string GetBgImgPath()
		{
			return _metaData.bgImgPath;
		}

		public string GetLevelPanelPath()
		{
			return _metaData.levelPanelPath;
		}

		public bool ShouldDisplayLeftTime()
		{
			return (_metaData.displayLeftTime != 0) ? true : false;
		}

		public override Status GetStatus()
		{
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			if (_status == Status.Unavailable)
			{
				return _status;
			}
			if (TimeUtil.Now < beginTime)
			{
				_status = Status.WaitToStart;
			}
			else if (TimeUtil.Now > endTime)
			{
				_status = Status.Over;
			}
			else
			{
				_status = Status.InProgress;
				flag = true;
			}
			if (_status != Status.Unavailable && Singleton<PlayerModule>.Instance.playerData.teamLevel < _metaData.minPlayerLevel)
			{
				_status = Status.Locked;
			}
			foreach (int levelID in _metaData.levelIDList)
			{
				LevelDataItem levelDataItem = Singleton<LevelModule>.Instance.TryGetLevelById(levelID);
				if (levelDataItem != null)
				{
					levelDataItem.status = (StageStatus)((!flag) ? 1 : 2);
				}
			}
			return _status;
		}

		public override void InitStatusOnPacket()
		{
			_status = Status.WaitToStart;
		}

		public bool ShowActivityShopEntry()
		{
			return _metaData.showActivityShopEntry != 0;
		}
	}
}
