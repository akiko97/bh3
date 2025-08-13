using System;
using System.Collections.Generic;
using System.Linq;
using MoleMole.Config;
using UniRx;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class MissionOverviewPageContext : BasePageContext
	{
		public enum E_MissionLinkType
		{
			None = 0,
			Stage = 1,
			Activity = 2,
			Serial = 3,
			Attack = 4,
			Valkyrja = 5,
			Equip = 6,
			Supply = 7,
			ActivityRoot = 8,
			MainStoryRoot = 9,
			Endless = 10,
			Island = 11
		}

		private List<MissionDataItem> _missionList = new List<MissionDataItem>();

		private MonoScrollerFadeManager _fadeMgr;

		private MonoMissionUtil _util;

		private bool _waitingForIslandServerData;

		private List<Tuple<StorageDataItemBase, bool>> _avatarGotList;

		private Dictionary<int, RectTransform> _dictBeforeFetch;

		private int _levelBeforeReward;

		public MissionOverviewPageContext()
		{
			config = new ContextPattern
			{
				contextName = "MissionOverviewPageContext",
				viewPrefabPath = "UI/Menus/Page/Mission/MissionOverviewPage"
			};
			_avatarGotList = new List<Tuple<StorageDataItemBase, bool>>();
		}

		protected override void BindViewCallbacks()
		{
		}

		protected override bool SetupView()
		{
			if (base.view == null || base.view.transform == null)
			{
				return false;
			}
			_util = base.view.GetComponent<MonoMissionUtil>();
			_util.Init();
			FilterMissions();
			_missionList.Sort(MissionDataItem.CompareToMission);
			MonoGridScroller component = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoGridScroller>();
			component.Init(OnScrollerChange, _missionList.Count);
			_fadeMgr = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_fadeMgr.Init(component.GetItemDict(), _dictBeforeFetch, IsMissionEqual);
			_fadeMgr.Play();
			_dictBeforeFetch = null;
			return false;
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		public override bool OnNotify(Notify ntf)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			if (ntf.type == NotifyTypes.MissionUpdated)
			{
				MissionDataItem missionDataItem = GetMissionDataItem((int)(uint)ntf.body);
				if (missionDataItem != null && !FilterMission(missionDataItem))
				{
					SetupView();
				}
			}
			else if (ntf.type == NotifyTypes.MissionRewardGot)
			{
				OnMissionRewardGot((GetMissionRewardRsp)ntf.body);
				_fadeMgr.Reset();
			}
			else if (ntf.type == NotifyTypes.MissionRewardAvatarGot)
			{
				ShowAvatarGot((AvatarCardDataItem)ntf.body);
			}
			else if (ntf.type == NotifyTypes.MissionDeleted)
			{
				SetupView();
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 157)
			{
				return OnGetIsLandRsp(pkt.getData<GetIslandRsp>());
			}
			return false;
		}

		private bool OnGetIsLandRsp(GetIslandRsp rsp)
		{
			if (!_waitingForIslandServerData)
			{
				return false;
			}
			_waitingForIslandServerData = false;
			Singleton<MainUIManager>.Instance.MoveToNextScene("Island", false, true);
			return false;
		}

		private void OnScrollerChange(Transform trans, int index)
		{
			MissionDataItem missionData = _missionList[index];
			MonoMissionInfo component = trans.GetComponent<MonoMissionInfo>();
			component.SetupView(missionData);
			component.RegisterCallBacks(OnFetchRewardBtnClick, OnGoMissionBtnClick);
		}

		protected override void OnSetActive(bool enabled)
		{
			if (!enabled)
			{
				_fadeMgr.Reset();
			}
		}

		private void OnFetchRewardBtnClick(MissionDataItem missionData)
		{
			SaveDataBeforeReward();
			_dictBeforeFetch = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoGridScroller>().GetItemDict()
				.ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			Singleton<NetworkManager>.Instance.RequestGetMissionReward((uint)missionData.id);
		}

		private void OnGoMissionBtnClick(MissionDataItem missionData)
		{
			E_MissionLinkType linkType = (E_MissionLinkType)missionData.metaData.LinkType;
			int linkParam = missionData.metaData.LinkParam;
			switch (linkType)
			{
			case E_MissionLinkType.Stage:
				GoToStageDetailDialog(linkParam);
				break;
			case E_MissionLinkType.Activity:
				GoToActivityPage(missionData.metaData.LinkParams);
				break;
			case E_MissionLinkType.Serial:
				GoToSerialPage(linkParam);
				break;
			case E_MissionLinkType.Attack:
				GoToChapterSelectPage();
				break;
			case E_MissionLinkType.Valkyrja:
				GoToAvatarPage();
				break;
			case E_MissionLinkType.Equip:
				GoToEquipPage();
				break;
			case E_MissionLinkType.Supply:
				GoToSupplyPage();
				break;
			case E_MissionLinkType.ActivityRoot:
				GoToActivityRootPage();
				break;
			case E_MissionLinkType.MainStoryRoot:
				GoToMainStoryRootPage();
				break;
			case E_MissionLinkType.Endless:
				GoToEndlessPage();
				break;
			case E_MissionLinkType.Island:
				GoToIsland();
				break;
			}
		}

		private void GoToIsland()
		{
			_waitingForIslandServerData = true;
			Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(157));
			Singleton<NetworkManager>.Instance.RequestGetIsland();
		}

		private void GoToEndlessPage()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext("Event"));
		}

		private void GoToMainStoryRootPage()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext("MainStory"));
		}

		private void GoToActivityRootPage()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext("Event"));
		}

		private void GoToSupplyPage()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new GachaMainPageContext());
		}

		private void GoToEquipPage()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext());
		}

		private void GoToAvatarPage()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new AvatarOverviewPageContext
			{
				type = AvatarOverviewPageContext.PageType.Show,
				selectedAvatarID = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.lastSelectedAvatarID
			});
		}

		private void GoToSerialPage(int serialID)
		{
			WeekDayActivityDataItem weekDayActivityDataItem = null;
			List<WeekDayActivityDataItem> allWeekDayActivityList = Singleton<LevelModule>.Instance.AllWeekDayActivityList;
			for (int i = 0; i < allWeekDayActivityList.Count; i++)
			{
				if (allWeekDayActivityList[i].GetSeriesID() == serialID && allWeekDayActivityList[i].GetStatus() == ActivityDataItemBase.Status.InProgress)
				{
					weekDayActivityDataItem = allWeekDayActivityList[i];
					break;
				}
			}
			if (weekDayActivityDataItem == null)
			{
				List<WeekDayActivityDataItem> activityListBySeriesID = Singleton<LevelModule>.Instance.GetActivityListBySeriesID(serialID);
				weekDayActivityDataItem = activityListBySeriesID[0];
			}
			Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(weekDayActivityDataItem));
		}

		private void GoToActivityPage(List<int> activityIDList)
		{
			int num = 0;
			WeekDayActivityDataItem weekDayActivityDataItem = null;
			foreach (int activityID in activityIDList)
			{
				weekDayActivityDataItem = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(activityID);
				if (weekDayActivityDataItem.GetStatus() == ActivityDataItemBase.Status.InProgress)
				{
					num = activityID;
					break;
				}
			}
			if (num <= 0)
			{
				num = activityIDList[0];
			}
			weekDayActivityDataItem = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(num);
			Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(weekDayActivityDataItem));
		}

		private void GoToStageDetailDialog(int stageID)
		{
			LevelDataItem levelDataItem = Singleton<LevelModule>.Instance.TryGetLevelById(stageID);
			if (levelDataItem == null)
			{
				levelDataItem = new LevelDataItem(stageID);
			}
			if (levelDataItem.UnlockPlayerLevel > Singleton<PlayerModule>.Instance.playerData.teamLevel)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ActivityLock", levelDataItem.UnlockPlayerLevel)));
				return;
			}
			ChapterDataItem chapterById = Singleton<LevelModule>.Instance.GetChapterById(levelDataItem.ChapterID);
			int totalFinishedChanllengeNum = chapterById.GetTotalFinishedChanllengeNum(levelDataItem.Diffculty);
			if (levelDataItem.UnlockChanllengeNum > totalFinishedChanllengeNum)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ChallengeLackLock", levelDataItem.UnlockChanllengeNum)));
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(levelDataItem));
			}
		}

		private void GoToChapterSelectPage()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext());
		}

		private void SaveDataBeforeReward()
		{
			_levelBeforeReward = Singleton<PlayerModule>.Instance.playerData.teamLevel;
		}

		public void OnRewardGotDialogClose(AvatarCardDataItem avatarData)
		{
			if (_levelBeforeReward < Singleton<PlayerModule>.Instance.playerData.teamLevel)
			{
				PlayerLevelUpDialogContext playerLevelUpDialogContext = new PlayerLevelUpDialogContext();
				playerLevelUpDialogContext.SetLevelBeforeNoScoreManager(_levelBeforeReward);
				if (avatarData != null)
				{
					playerLevelUpDialogContext.SetNotifyWhenDestroy(avatarData);
				}
				Singleton<MainUIManager>.Instance.ShowDialog(playerLevelUpDialogContext);
			}
			else if (avatarData != null)
			{
				ShowAvatarGot(avatarData);
			}
		}

		private void ShowAvatarGot(AvatarCardDataItem avatarData)
		{
			_avatarGotList.Clear();
			_avatarGotList.Add(new Tuple<StorageDataItemBase, bool>(avatarData, false));
			Singleton<MainUIManager>.Instance.ShowDialog(new DropNewItemDialogContextV2(_avatarGotList));
		}

		private void OnMissionUpdated(uint id)
		{
			SetupView();
		}

		private void OnMissionRewardGot(GetMissionRewardRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				MissionRewardGotDialogContext missionRewardGotDialogContext = new MissionRewardGotDialogContext(rsp.reward_list);
				missionRewardGotDialogContext.RegisterCallBack(OnRewardGotDialogClose);
				Singleton<MainUIManager>.Instance.ShowDialog(missionRewardGotDialogContext);
			}
		}

		private bool FilterMission(MissionDataItem mission)
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Invalid comparison between Unknown and I4
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Invalid comparison between Unknown and I4
			LinearMissionData linearMissionData = LinearMissionDataReader.TryGetLinearMissionDataByKey(mission.id);
			if (linearMissionData != null && linearMissionData.IsAchievement == 1)
			{
				return true;
			}
			if (linearMissionData != null && linearMissionData.PreMissionId > 0)
			{
				foreach (MissionDataItem value in Singleton<MissionModule>.Instance.GetMissionDict().Values)
				{
					if (value.id == linearMissionData.PreMissionId && ((int)value.status == 2 || (int)value.status == 3))
					{
						return true;
					}
				}
			}
			if (IsPreviewMission(mission))
			{
				DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp((uint)mission.beginTime);
				if ((dateTimeFromTimeStamp - TimeUtil.Now).TotalSeconds > (double)mission.metaData.PreviewTime)
				{
					return true;
				}
			}
			return false;
		}

		private void FilterMissions()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Invalid comparison between Unknown and I4
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Invalid comparison between Unknown and I4
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Invalid comparison between Unknown and I4
			List<MissionDataItem> list = Singleton<MissionModule>.Instance.GetMissionDict().Values.ToList();
			_missionList.Clear();
			foreach (MissionDataItem item in list)
			{
				bool flag = false;
				if ((int)item.status == 5)
				{
					flag = true;
				}
				else
				{
					LinearMissionData linearMissionData = LinearMissionDataReader.TryGetLinearMissionDataByKey(item.id);
					if (linearMissionData != null && linearMissionData.PreMissionId > 0)
					{
						foreach (MissionDataItem item2 in list)
						{
							if (item2.id == linearMissionData.PreMissionId && ((int)item2.status == 2 || (int)item2.status == 3))
							{
								flag = true;
								break;
							}
						}
					}
					if (IsPreviewMission(item))
					{
						DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp((uint)item.beginTime);
						if ((dateTimeFromTimeStamp - TimeUtil.Now).TotalSeconds > (double)item.metaData.PreviewTime)
						{
							flag = true;
							_util.AddPreviewMission(item);
						}
					}
					if (linearMissionData != null && linearMissionData.IsAchievement == 1)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					_missionList.Add(item);
				}
			}
		}

		private bool IsPreviewMission(MissionDataItem mission)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			return mission.metaData.type == 3 && (int)mission.status == 1 && mission.metaData.PreviewTime > 0;
		}

		private void HackAddMission()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Expected O, but got Unknown
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Expected O, but got Unknown
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Expected O, but got Unknown
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			Mission val = new Mission();
			val.mission_id = 10005u;
			Mission mission = val;
			MissionDataItem missionDataItem = new MissionDataItem(mission);
			missionDataItem.status = (MissionStatus)2;
			_missionList.Add(missionDataItem);
			val = new Mission();
			val.mission_id = 23001u;
			mission = val;
			missionDataItem = new MissionDataItem(mission);
			missionDataItem.status = (MissionStatus)2;
			_missionList.Add(missionDataItem);
			val = new Mission();
			val.mission_id = 22005u;
			mission = val;
			missionDataItem = new MissionDataItem(mission);
			missionDataItem.status = (MissionStatus)2;
			missionDataItem.progress = 20;
			_missionList.Add(missionDataItem);
			val = new Mission();
			val.mission_id = 22003u;
			mission = val;
			missionDataItem = new MissionDataItem(mission);
			missionDataItem.status = (MissionStatus)2;
			missionDataItem.progress = 20;
			_missionList.Add(missionDataItem);
			val = new Mission();
			val.mission_id = 31002u;
			mission = val;
			missionDataItem = new MissionDataItem(mission);
			missionDataItem.status = (MissionStatus)2;
			missionDataItem.progress = 20;
			_missionList.Add(missionDataItem);
			val = new Mission();
			val.mission_id = 31005u;
			mission = val;
			missionDataItem = new MissionDataItem(mission);
			missionDataItem.status = (MissionStatus)2;
			missionDataItem.progress = 20;
			_missionList.Add(missionDataItem);
			val = new Mission();
			val.mission_id = 31004u;
			mission = val;
			missionDataItem = new MissionDataItem(mission);
			missionDataItem.status = (MissionStatus)2;
			missionDataItem.progress = 20;
			_missionList.Add(missionDataItem);
		}

		private bool IsMissionEqual(RectTransform missionNew, RectTransform missionOld)
		{
			if (missionNew == null || missionOld == null)
			{
				return false;
			}
			MonoMissionInfo component = missionOld.GetComponent<MonoMissionInfo>();
			MonoMissionInfo component2 = missionNew.GetComponent<MonoMissionInfo>();
			return component2.GetMissionData().id == component.GetMissionData().id;
		}

		private MissionDataItem GetMissionDataItem(int id)
		{
			Dictionary<int, MissionDataItem> missionDict = Singleton<MissionModule>.Instance.GetMissionDict();
			if (missionDict.ContainsKey(id))
			{
				return missionDict[id];
			}
			return null;
		}
	}
}
