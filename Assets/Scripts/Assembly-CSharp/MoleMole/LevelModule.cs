using System.Collections.Generic;
using System.IO;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class LevelModule : BaseModule
	{
		private Dictionary<int, LevelDataItem> _allLevelDict;

		private Dictionary<int, ChapterDataItem> _allChapterDict;

		private Dictionary<int, WeekDayActivityDataItem> _allWeekDayActivityDict;

		private Dictionary<int, SeriesDataItem> _allSeriesDict;

		private Dictionary<int, List<int>> _levelDropItemsDict;

		private Dictionary<int, List<int>> _levelFirstDropItemsDict;

		private bool _isInitialized;

		private bool _isFirstStageDataRsp;

		public List<LevelDataItem> AllLevelList { get; private set; }

		public List<ChapterDataItem> AllChapterList { get; private set; }

		public List<WeekDayActivityDataItem> AllWeekDayActivityList { get; private set; }

		public LevelModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_isInitialized = false;
			_isFirstStageDataRsp = true;
		}

		private void Init()
		{
			_isInitialized = true;
			AllLevelList = new List<LevelDataItem>();
			_allLevelDict = new Dictionary<int, LevelDataItem>();
			AllChapterList = new List<ChapterDataItem>();
			_allChapterDict = new Dictionary<int, ChapterDataItem>();
			AllWeekDayActivityList = new List<WeekDayActivityDataItem>();
			_allWeekDayActivityDict = new Dictionary<int, WeekDayActivityDataItem>();
			_allSeriesDict = new Dictionary<int, SeriesDataItem>();
			_levelDropItemsDict = new Dictionary<int, List<int>>();
			_levelFirstDropItemsDict = new Dictionary<int, List<int>>();
			AddAllChaptersFromMetaData();
			AddAllActivityFromMetaData();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 42:
				return OnGetStageDataRsp(pkt.getData<GetStageDataRsp>());
			case 61:
				return OnGetStageDropDisplayRsp(pkt.getData<GetStageDropDisplayRsp>());
			case 126:
				return OnGetWeekDayActivityDataRsp(pkt.getData<GetWeekDayActivityDataRsp>());
			default:
				return false;
			}
		}

		private bool OnGetStageDataRsp(GetStageDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Invalid comparison between Unknown and I4
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Invalid comparison between Unknown and I4
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Invalid comparison between Unknown and I4
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode != 0)
			{
				return false;
			}
			if (!_isInitialized)
			{
				Init();
			}
			foreach (Stage item in rsp.stage_list)
			{
				int id = (int)item.id;
				if (LevelMetaDataReader.GetLevelMetaDataByKey(id) == null)
				{
					continue;
				}
				if (!_allLevelDict.ContainsKey(id))
				{
					LevelDataItem levelDataItem = new LevelDataItem(id);
					if ((int)levelDataItem.LevelType == 1)
					{
						AddLevelDataItem(levelDataItem);
					}
					else
					{
						_allLevelDict[id] = levelDataItem;
						AllLevelList.Add(levelDataItem);
					}
				}
				LevelDataItem levelDataItem2 = _allLevelDict[id];
				if ((int)levelDataItem2.LevelType == 1)
				{
					_allChapterDict[levelDataItem2.ChapterID].Unlocked = true;
				}
				if ((int)_allLevelDict[id].LevelType == 1 && !_allLevelDict[id].Initialized)
				{
					_allLevelDict[id].Init();
					if (!_isFirstStageDataRsp)
					{
						ActDataItem actDataItem = new ActDataItem(_allLevelDict[id].ActID);
						if (actDataItem != null && actDataItem.actType != ActDataItem.ActType.Extra)
						{
							Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Add(id);
						}
						if (IsLastLevelOfAct(_allLevelDict[id]))
						{
							Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime = TimeUtil.Now.AddHours(4.0);
							Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime = TimeUtil.Now.AddDays(-1.0);
						}
						else
						{
							Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime = TimeUtil.Now.AddDays(-1.0);
						}
						Singleton<MiHoYoGameData>.Instance.Save();
					}
				}
				LevelDataItem levelDataItem3 = _allLevelDict[id];
				UpdateField(item.progressSpecified, ref levelDataItem3.progress, (int)item.progress, _allLevelDict[id].OnProgressUpdate);
				UpdateField(item.enter_timesSpecified, ref levelDataItem3.enterTimes, (int)item.enter_times);
				UpdateField(item.reset_timesSpecified, ref levelDataItem3.resetTimes, (int)item.reset_times);
				UpdateField(item.bonus_enter_timesSpecified, ref levelDataItem3.dropActivityEnterTimes, (int)item.bonus_enter_times);
				UpdateField(item.bonus_total_timesSpecified, ref levelDataItem3.dropActivityMaxEnterTimes, (int)item.bonus_total_times);
				if (levelDataItem3.progress == 0 && (int)levelDataItem3.LevelType == 1 && Singleton<MainMenuBGM>.Instance != null)
				{
					Singleton<MainMenuBGM>.Instance.SetBGMSwitchByStage(IsLastLevelOfAct(levelDataItem3));
				}
				levelDataItem3.isDropActivityOpen = item.bonus_end_timeSpecified;
				levelDataItem3.dropActivityEndTime = Miscs.GetDateTimeFromTimeStamp(item.bonus_end_time);
				for (int i = 0; i < _allLevelDict[id].challengeList.Count; i++)
				{
					LevelChallengeDataItem levelChallengeDataItem = _allLevelDict[id].challengeList[i];
					if (item.challenge_index_list.Contains((uint)i))
					{
						levelChallengeDataItem.Finished = true;
					}
				}
			}
			_isFirstStageDataRsp = false;
			return false;
		}

		private bool OnGetStageDropDisplayRsp(GetStageDropDisplayRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode != 0)
			{
				return false;
			}
			foreach (StageDropDisplayInfo item in rsp.stage_drop_list)
			{
				int stage_id = (int)item.stage_id;
				if (LevelMetaDataReader.GetLevelMetaDataByKey(stage_id) == null)
				{
					continue;
				}
				if (!_allLevelDict.ContainsKey(stage_id))
				{
					LevelDataItem levelDataItem = new LevelDataItem(stage_id);
					if ((int)levelDataItem.LevelType == 1)
					{
						AddLevelDataItem(levelDataItem);
					}
					else
					{
						_allLevelDict[stage_id] = levelDataItem;
						AllLevelList.Add(levelDataItem);
					}
				}
				LevelDataItem levelDataItem2 = _allLevelDict[stage_id];
				levelDataItem2.dropDisplayInfoReceived = true;
				levelDataItem2.displayDropList = ConvertList(item.drop_item_id_list);
				levelDataItem2.displayFirstDropList = ConvertList(item.first_drop_item_id_list);
				levelDataItem2.displayBonusDropList = ConvertList(item.bonus_drop_item_id_list);
				levelDataItem2.isDoubleDrop = item.double_drop;
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.StageDropListUpdated));
			return false;
		}

		private bool OnGetWeekDayActivityDataRsp(GetWeekDayActivityDataRsp rsp)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			if (!_isInitialized)
			{
				Init();
			}
			if ((int)rsp.retcode == 0)
			{
				if (rsp.is_whole_dataSpecified && rsp.is_whole_data)
				{
					AllWeekDayActivityList.Clear();
				}
				List<uint> list = new List<uint>();
				foreach (WeekDayActivity item in rsp.activity_list)
				{
					WeekDayActivityDataItem value;
					_allWeekDayActivityDict.TryGetValue((int)item.activity_id, out value);
					if (value == null)
					{
						Debug.LogError("The activity with id: " + item.activity_id + " , is wrong!!!");
						continue;
					}
					if (AllWeekDayActivityList.Contains(value))
					{
						AllWeekDayActivityList.Remove(value);
					}
					value.enterTimes = (int)item.enter_times;
					value.beginTime = Miscs.GetDateTimeFromTimeStamp(item.begin_time);
					value.endTime = Miscs.GetDateTimeFromTimeStamp(item.end_time);
					value.InitStatusOnPacket();
					_allWeekDayActivityDict[value.GetActivityID()] = value;
					AllWeekDayActivityList.Add(value);
					foreach (uint item2 in item.stage_id_list)
					{
						LevelDataItem value2;
						_allLevelDict.TryGetValue((int)item2, out value2);
						if (value2 != null)
						{
							value2.status = (StageStatus)2;
						}
					}
					list.AddRange(item.stage_id_list);
				}
				Singleton<NetworkManager>.Instance.RequestLevelDropList(list);
				AllWeekDayActivityList.Sort((WeekDayActivityDataItem lobj, WeekDayActivityDataItem robj) => lobj.GetActivityID() - robj.GetActivityID());
			}
			return false;
		}

		private void AddAllChaptersFromMetaData()
		{
			AllChapterList.Clear();
			_allChapterDict.Clear();
			List<ChapterMetaData> itemList = ChapterMetaDataReader.GetItemList();
			foreach (ChapterMetaData item in itemList)
			{
				ChapterDataItem chapterDataItem = new ChapterDataItem(item.chapterId);
				AllChapterList.Add(chapterDataItem);
				_allChapterDict.Add(item.chapterId, chapterDataItem);
			}
		}

		private void AddAllActivityFromMetaData()
		{
			_allWeekDayActivityDict.Clear();
			_allSeriesDict.Clear();
			foreach (WeekDayActivityMetaData item in WeekDayActivityMetaDataReader.GetItemList())
			{
				WeekDayActivityDataItem weekDayActivityDataItem = new WeekDayActivityDataItem(item.weekDayActivityID);
				_allWeekDayActivityDict.Add(item.weekDayActivityID, weekDayActivityDataItem);
				int seriesID = weekDayActivityDataItem.GetSeriesID();
				if (!_allSeriesDict.ContainsKey(seriesID))
				{
					_allSeriesDict.Add(seriesID, new SeriesDataItem(seriesID));
				}
				_allSeriesDict[seriesID].weekActivityList.Add(weekDayActivityDataItem);
				foreach (int levelID in weekDayActivityDataItem.GetLevelIDList())
				{
					if (!_allLevelDict.ContainsKey(levelID) && LevelMetaDataReader.GetLevelMetaDataByKey(levelID) != null)
					{
						LevelDataItem levelDataItem = new LevelDataItem(levelID);
						levelDataItem.ActID = weekDayActivityDataItem.GetActivityID();
						levelDataItem.ChapterID = seriesID;
						_allLevelDict.Add(levelDataItem.levelId, levelDataItem);
					}
				}
			}
		}

		public LevelDataItem GetLevelById(int levelId)
		{
			return _allLevelDict[levelId];
		}

		public LevelDataItem TryGetLevelById(int levelId)
		{
			LevelDataItem value = null;
			if (_allLevelDict != null)
			{
				_allLevelDict.TryGetValue(levelId, out value);
			}
			return value;
		}

		public bool ContainLevelById(int levelId)
		{
			return _allLevelDict != null && _allLevelDict.ContainsKey(levelId);
		}

		public ChapterDataItem GetChapterById(int chapterId)
		{
			return _allChapterDict[chapterId];
		}

		public int GetOneUnlockChapterID()
		{
			int result = 1;
			foreach (KeyValuePair<int, ChapterDataItem> item in _allChapterDict)
			{
				if (item.Value.Unlocked)
				{
					return result;
				}
			}
			return result;
		}

		public bool HasChapter(int chapterID)
		{
			ChapterDataItem value;
			_allChapterDict.TryGetValue(chapterID, out value);
			return value != null && value.Unlocked;
		}

		private void AddLevelDataItem(LevelDataItem level)
		{
			List<LevelDataItem> list = _allChapterDict[level.ChapterID].AddLevel(level);
			AllLevelList.AddRange(list);
			foreach (LevelDataItem item in list)
			{
				_allLevelDict.Add(item.levelId, item);
			}
		}

		public bool HasPlot(int levelID)
		{
			List<PlotMetaData> itemList = PlotMetaDataReader.GetItemList();
			PlotMetaData plotMetaData = itemList.Find((PlotMetaData x) => x.levelID == levelID);
			if (plotMetaData != null)
			{
				return true;
			}
			return false;
		}

		public WeekDayActivityDataItem GetWeekDayActivityByID(int activityID)
		{
			WeekDayActivityDataItem value;
			_allWeekDayActivityDict.TryGetValue(activityID, out value);
			return value;
		}

		public SeriesDataItem GetWeekDaySeriesByActivityID(int activityID)
		{
			WeekDayActivityDataItem weekDayActivityByID = GetWeekDayActivityByID(activityID);
			if (weekDayActivityByID == null)
			{
				return null;
			}
			SeriesDataItem value;
			_allSeriesDict.TryGetValue(weekDayActivityByID.GetSeriesID(), out value);
			return value;
		}

		public List<LevelDataItem> GetWeekDayActivityLevelsByID(int activityID)
		{
			WeekDayActivityDataItem value;
			_allWeekDayActivityDict.TryGetValue(activityID, out value);
			if (value == null)
			{
				return null;
			}
			List<LevelDataItem> list = new List<LevelDataItem>();
			foreach (int levelID in value.GetLevelIDList())
			{
				if (_allLevelDict.ContainsKey(levelID))
				{
					list.Add(_allLevelDict[levelID]);
				}
			}
			return list;
		}

		public List<int> GetLevelDropItemIDList(int levelID)
		{
			List<int> value;
			_levelDropItemsDict.TryGetValue(levelID, out value);
			return value;
		}

		public List<int> GetLevelFirstDropItemIDList(int levelID)
		{
			List<int> value;
			_levelFirstDropItemsDict.TryGetValue(levelID, out value);
			return value;
		}

		public List<WeekDayActivityDataItem> GetActivityListBySeriesID(int seriesID)
		{
			if (_allSeriesDict.ContainsKey(seriesID))
			{
				return _allSeriesDict[seriesID].weekActivityList;
			}
			return null;
		}

		public bool IsLastLevelOfAct(LevelDataItem levelData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)levelData.LevelType != 1)
			{
				return false;
			}
			List<LevelDataItem> list = GetChapterById(levelData.ChapterID).GetLevelOfActs()[levelData.ActID];
			list.Sort((LevelDataItem x, LevelDataItem y) => y.SectionId - x.SectionId);
			return levelData.SectionId == list[0].SectionId;
		}

		public WeekDayActivityDataItem TryGetWeekDayActivityByLevelID(int levelID)
		{
			WeekDayActivityDataItem result = null;
			foreach (WeekDayActivityDataItem value in _allWeekDayActivityDict.Values)
			{
				if (value.GetLevelIDList().Contains(levelID))
				{
					result = value;
					break;
				}
			}
			return result;
		}

		public void SaveLevelEndReqInfo(StageEndReq stageEndReq)
		{
			if (MiscData.Config.BasicConfig.FeatureOnRetrySendLevelEndReq)
			{
				MemoryStream memoryStream = new MemoryStream();
				memoryStream.SetLength(0L);
				memoryStream.Position = 0L;
				Singleton<NetworkManager>.Instance.serializer.Serialize(memoryStream, stageEndReq);
				Singleton<MiHoYoGameData>.Instance.LocalData.ProcessingStageEndReq = memoryStream.ToArray();
				Singleton<MiHoYoGameData>.Instance.Save();
			}
		}

		public void ClearLevelEndReqInfo()
		{
			if (MiscData.Config.BasicConfig.FeatureOnRetrySendLevelEndReq)
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.ProcessingStageEndReq = null;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
		}

		public void RetrySendLevelEndReq()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			if (MiscData.Config.BasicConfig.FeatureOnRetrySendLevelEndReq && Singleton<MiHoYoGameData>.Instance.LocalData.ProcessingStageEndReq != null)
			{
				MemoryStream source = new MemoryStream(Singleton<MiHoYoGameData>.Instance.LocalData.ProcessingStageEndReq);
				StageEndReq val = (StageEndReq)Singleton<NetworkManager>.Instance.serializer.Deserialize(source, null, typeof(StageEndReq));
				if (val != null)
				{
					Singleton<NetworkManager>.Instance.SendPacket<StageEndReq>(val);
				}
				else
				{
					ClearLevelEndReqInfo();
				}
			}
		}

		public void HandleStageEndRspForRetry(StageEndRsp rsp)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (MiscData.Config.BasicConfig.FeatureOnRetrySendLevelEndReq)
			{
				ClearLevelEndReqInfo();
				if ((int)rsp.retcode == 0)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralConfirmDialogContext
					{
						type = GeneralConfirmDialogContext.ButtonType.SingleButton,
						desc = LocalizationGeneralLogic.GetText("Menu_Desc_SuccRetrySendLevelEndReq")
					});
				}
				else
				{
					string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralConfirmDialogContext
					{
						type = GeneralConfirmDialogContext.ButtonType.SingleButton,
						desc = LocalizationGeneralLogic.GetText("Menu_Desc_FailRetrySendLevelEndReq", networkErrCodeOutput)
					});
				}
			}
		}

		public bool IsLevelDone(int levelID)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			LevelDataItem levelDataItem = TryGetLevelById(levelID);
			return levelDataItem != null && (int)levelDataItem.status == 3;
		}
	}
}
