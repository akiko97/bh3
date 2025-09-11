using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class TutorialModule : BaseModule
	{
		private enum WaitingListStatus
		{
			Invalid = 0,
			SendToServer = 1,
			WaitingFinish = 2,
			Finished = 3
		}

		private enum TutorialWaitingStatus
		{
			NoWaiting = 0,
			WaitingFinishOnStart = 1,
			WaitingFinishOnEndPre = 2,
			WaitingFinishOnEnd = 3,
			WaitingSkip = 4
		}

		private List<int> _finishTutorialList;

		private Dictionary<string, List<int>> _contextTutorialDict;

		private Dictionary<int, List<int>> _missionTutorialDict;

		private bool _isInTutorial;

		private List<int> _waitingList;

		private WaitingListStatus _waitingListStatus;

		private LoadingWheelWidgetContext _wheelContext;

		private TutorialWaitingStatus _tutorialWaitingStatus;

		private Coroutine _waitingFinishOnStartCoroutine;

		private Coroutine _waitingFinishOnEndPreCoroutine;

		private Coroutine _waitingFinishOnEndCoroutine;

		private List<int> _skipList;

		private Coroutine _waitingSkip;

		public bool IsInTutorial
		{
			get
			{
				return _isInTutorial;
			}
		}

		public TutorialModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_finishTutorialList = new List<int>();
			_contextTutorialDict = new Dictionary<string, List<int>>();
			_missionTutorialDict = new Dictionary<int, List<int>>();
			_skipList = new List<int>();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 128:
				return OnGetFinishGuideDataRsp(pkt.getData<GetFinishGuideDataRsp>());
			case 130:
				return OnFinishGuideReportRsp(pkt.getData<FinishGuideReportRsp>());
			default:
				return false;
			}
		}

		public void TryToDoTutoialWhenShowContext(BaseContext context)
		{
			if (!_isInTutorial)
			{
				bool flag = false;
				if (_contextTutorialDict.ContainsKey(context.config.contextName))
				{
					flag = DoTutorialWhenShowContext(context);
				}
				if (!flag)
				{
					TryToDoTutorialByCheckPlayerStatusWidget();
				}
			}
		}

		public void TryToDoTutoialWhenUpdateMissionStatus(Mission mission)
		{
			if (_isInTutorial || !_missionTutorialDict.ContainsKey((int)mission.mission_id))
			{
				return;
			}
			List<int> list = _missionTutorialDict[(int)mission.mission_id];
			foreach (int item in list)
			{
				TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(item);
				BaseContext baseContext = CheckContextEnable(tutorialDataByKey.triggerUIContextName);
				if (baseContext == null || !CheckCanDoStep(tutorialDataByKey, tutorialDataByKey.startStepID))
				{
					continue;
				}
				DoTutorial(tutorialDataByKey, tutorialDataByKey.startStepID);
				break;
			}
		}

		public void SetTutorialFlag(bool isInTutorial)
		{
			_isInTutorial = isInTutorial;
		}

		public bool IsTutorialIDFinish(int tutorialID)
		{
			return _finishTutorialList.Contains(tutorialID);
		}

		public void Destroy()
		{
			StopAllCoroutines();
		}

		public void TryToSkipTutorial(int tutorialID, Action skipUICallback)
		{
			int skipGroup = TutorialDataReader.GetTutorialDataByKey(tutorialID).SkipGroup;
			if (skipGroup == 0)
			{
				Debug.LogError("skip group is zero");
				return;
			}
			_skipList.Clear();
			foreach (TutorialData item in TutorialDataReader.GetItemList())
			{
				if (item.SkipGroup == skipGroup)
				{
					_skipList.Add(item.id);
				}
			}
			if (MarkTutorialIDFinishToServer(_skipList, true))
			{
				if (_wheelContext != null)
				{
					_wheelContext.Finish();
				}
				_wheelContext = new LoadingWheelWidgetContext(130, delegate
				{
					RetryRequestFinishGuideReport(_skipList, true);
				});
				Singleton<MainUIManager>.Instance.ShowWidget(_wheelContext);
				if (Singleton<ApplicationManager>.Instance != null)
				{
					_waitingSkip = Singleton<ApplicationManager>.Instance.StartCoroutine(SkipWait(_skipList, skipUICallback));
				}
			}
			else
			{
				skipUICallback();
			}
		}

		private IEnumerator SkipWait(List<int> skipList, Action skipUICallback)
		{
			_tutorialWaitingStatus = TutorialWaitingStatus.WaitingSkip;
			while (_waitingList == skipList && _waitingListStatus == WaitingListStatus.SendToServer)
			{
				yield return null;
			}
			_waitingList = null;
			_waitingListStatus = WaitingListStatus.Invalid;
			_waitingSkip = null;
			_wheelContext = null;
			_tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
			skipUICallback();
		}

		private void TryToDoTutorialByCheckPlayerStatusWidget()
		{
			string key = "PlayerStatusWidgetContext";
			if (_contextTutorialDict.ContainsKey(key))
			{
				BaseContext baseContext = CheckPlayerStatusWidgetEnable();
				if (baseContext != null)
				{
					DoTutorialWhenShowContext(baseContext);
				}
			}
		}

		private bool DoTutorialWhenShowContext(BaseContext context)
		{
			List<int> list = _contextTutorialDict[context.config.contextName];
			foreach (int item in list)
			{
				TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(item);
				if (!CheckCanDoStep(tutorialDataByKey, tutorialDataByKey.startStepID))
				{
					continue;
				}
				DoTutorial(tutorialDataByKey, tutorialDataByKey.startStepID);
				return true;
			}
			return false;
		}

		private void DoTutorial(TutorialData tutorialData, int tutorialStepID)
		{
			_tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
			SetTutorialFlag(true);
			TutorialStepData tutorialStepDataByKey = TutorialStepDataReader.GetTutorialStepDataByKey(tutorialStepID);
			CheckAndSetScrollViewBegin(tutorialStepID);
			List<int> finishOnStartList = tutorialStepDataByKey.FinishOnStart;
			if (finishOnStartList.Count > 0)
			{
				if (MarkTutorialIDFinishToServer(finishOnStartList, true))
				{
					if (_wheelContext != null)
					{
						_wheelContext.Finish();
					}
					_wheelContext = new LoadingWheelWidgetContext(130, delegate
					{
						RetryRequestFinishGuideReport(finishOnStartList, true);
					});
					Singleton<MainUIManager>.Instance.ShowWidget(_wheelContext);
					if (Singleton<ApplicationManager>.Instance != null)
					{
						_waitingFinishOnStartCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(FinishOnStartWait(finishOnStartList, tutorialData, tutorialStepDataByKey));
					}
				}
				else
				{
					DoAfterFinishOnStart(tutorialData, tutorialStepDataByKey);
				}
			}
			else
			{
				DoAfterFinishOnStart(tutorialData, tutorialStepDataByKey);
			}
		}

		private bool CheckCanSkip(TutorialData tutorialData, int tutorialStepID)
		{
			if (tutorialData.SkipGroup == 0)
			{
				return false;
			}
			if (tutorialStepID == tutorialData.startStepID)
			{
				int num = Singleton<MiHoYoGameData>.Instance.LocalData.IsVisited_Tutorial(tutorialStepID);
				if (num >= 0)
				{
					Singleton<MiHoYoGameData>.Instance.LocalData.SetVisited_Tutorial(tutorialStepID);
					return true;
				}
				Singleton<MiHoYoGameData>.Instance.LocalData.SetVisited_Tutorial(tutorialStepID);
				return false;
			}
			int num2 = Singleton<MiHoYoGameData>.Instance.LocalData.IsVisited_Tutorial(tutorialData.startStepID);
			return num2 > 0;
		}

		private void CheckAndSetScrollViewBegin(int tutorialStepID)
		{
			TutorialStepData tutorialStepDataByKey = TutorialStepDataReader.GetTutorialStepDataByKey(tutorialStepID);
			string scrollUIPath = tutorialStepDataByKey.scrollUIPath;
			if (!(scrollUIPath != string.Empty))
			{
				return;
			}
			BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
			if (!(sceneCanvas == null))
			{
				Transform transform = sceneCanvas.transform.Find(scrollUIPath);
				if (!(transform == null) && transform.gameObject.activeInHierarchy && !(transform.GetComponent<ScrollRect>() == null))
				{
					MonoGridScroller component = transform.GetComponent<MonoGridScroller>();
					component.ScrollToBegin();
				}
			}
		}

		private void DoNextStep(TutorialData tutorialData, TutorialStepData currentTutorialStepData)
		{
			int nextStepID = currentTutorialStepData.nextStepID;
			if (!CheckCanDoStepByHighlight(nextStepID))
			{
				_tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
				SetTutorialFlag(false);
			}
			else
			{
				DoTutorial(tutorialData, nextStepID);
			}
		}

		private void DoTutorialOver(TutorialData tutorialData, TutorialStepData currentTutorialStepData)
		{
			_tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
			SetTutorialFlag(false);
		}

		private bool CheckCanDoStep(TutorialData tutorialData, int tutorialStepID)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Invalid comparison between Unknown and I4
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Invalid comparison between Unknown and I4
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Invalid comparison between Unknown and I4
			if (!CheckCanDoStepSpecialCondition(tutorialData))
			{
				return false;
			}
			MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem(tutorialData.triggerMissionID);
			if (missionDataItem == null)
			{
				return false;
			}
			if ((!tutorialData.triggerOnDoing || (int)missionDataItem.status != 2) && (!tutorialData.triggerOnFinish || (int)missionDataItem.status != 3) && (!tutorialData.triggerOnClose || (int)missionDataItem.status != 5))
			{
				return false;
			}
			if (!CheckCanDoStepByHighlight(tutorialStepID))
			{
				return false;
			}
			return true;
		}

		private bool CheckCanDoStepSpecialCondition(TutorialData tutorialData)
		{
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Invalid comparison between Unknown and I4
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Invalid comparison between Unknown and I4
			if (tutorialData.triggerSpecial == 99)
			{
				return false;
			}
			if (tutorialData.triggerSpecial == 1 && !Singleton<AvatarModule>.Instance.anyAvatarCanUnlock)
			{
				return false;
			}
			if (tutorialData.triggerSpecial == 2)
			{
				BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
				if (currentPageContext != null && currentPageContext is StorageItemDetailPageContext)
				{
					StorageDataItemBase storageItem = ((StorageItemDetailPageContext)currentPageContext).storageItem;
					if (storageItem is StigmataDataItem && !((StigmataDataItem)storageItem).IsAffixIdentify)
					{
						return true;
					}
				}
				return false;
			}
			if (tutorialData.triggerSpecial == 3)
			{
				BasePageContext currentPageContext2 = Singleton<MainUIManager>.Instance.CurrentPageContext;
				if (currentPageContext2 != null && currentPageContext2 is StorageItemDetailPageContext)
				{
					Transform transform = ((StorageItemDetailPageContext)currentPageContext2).view.transform.Find("ActionBtns/NewAffixBtn");
					if (transform != null && transform.gameObject.activeInHierarchy && transform.GetComponent<Button>().interactable)
					{
						return true;
					}
				}
				return false;
			}
			if (tutorialData.triggerSpecial == 4)
			{
				BasePageContext currentPageContext3 = Singleton<MainUIManager>.Instance.CurrentPageContext;
				if (currentPageContext3 is MainPageContext)
				{
					Transform transform2 = ((MainPageContext)currentPageContext3).view.transform.Find("MainBtns/IslandBtn");
					if (transform2 != null && transform2.gameObject.activeInHierarchy && transform2.GetComponent<Button>().interactable)
					{
						return true;
					}
				}
				return false;
			}
			if (tutorialData.triggerSpecial == 5)
			{
				BasePageContext currentPageContext4 = Singleton<MainUIManager>.Instance.CurrentPageContext;
				if (currentPageContext4 != null && currentPageContext4 is MainPageContext)
				{
					foreach (MissionDataItem value in Singleton<MissionModule>.Instance.GetMissionDict().Values)
					{
						LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey(value.id);
						if (linearMissionDataByKey != null && linearMissionDataByKey.IsAchievement == 1 && (int)value.status == 3)
						{
							return true;
						}
					}
				}
				return false;
			}
			if (tutorialData.triggerSpecial == 6)
			{
				List<BaseDialogContext> dialogContextList = Singleton<MainUIManager>.Instance.CurrentPageContext.dialogContextList;
				if (dialogContextList != null)
				{
					foreach (BaseDialogContext item in dialogContextList)
					{
						if (item is LevelDetailDialogContextV2)
						{
							LevelDataItem levelData = ((LevelDetailDialogContextV2)item).levelData;
							if ((int)levelData.LevelType == 5)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			if (tutorialData.triggerSpecial == 7)
			{
				List<BaseDialogContext> dialogContextList2 = Singleton<MainUIManager>.Instance.CurrentPageContext.dialogContextList;
				if (dialogContextList2 != null)
				{
					foreach (BaseDialogContext item2 in dialogContextList2)
					{
						if (item2 is LevelDetailDialogContextV2)
						{
							LevelDataItem levelData2 = ((LevelDetailDialogContextV2)item2).levelData;
							if (ActMetaDataReader.GetActMetaDataByKey(levelData2.ActID) == null)
							{
								return false;
							}
							ActDataItem actDataItem = new ActDataItem(levelData2.ActID);
							if (actDataItem.actType == ActDataItem.ActType.Extra)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			return true;
		}

		private bool CheckCanDoStepByHighlight(int tutorialStepID)
		{
			TutorialStepData tutorialStepDataByKey = TutorialStepDataReader.GetTutorialStepDataByKey(tutorialStepID);
			string targetUIPath = tutorialStepDataByKey.targetUIPath;
			if (targetUIPath != string.Empty)
			{
				BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
				if (sceneCanvas == null)
				{
					return false;
				}
				Transform transform = sceneCanvas.transform.Find(targetUIPath);
				if (transform == null)
				{
					return false;
				}
			}
			return true;
		}

		private BaseContext CheckContextEnable(string contextName)
		{
			try
			{
				BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
				if (currentPageContext == null)
				{
					return null;
				}
				if (currentPageContext.config.contextName == contextName)
				{
					return currentPageContext;
				}
				List<BaseDialogContext> dialogContextList = currentPageContext.dialogContextList;
				foreach (BaseDialogContext item in dialogContextList)
				{
					if (item.IsActive && item.config.contextName == contextName)
					{
						return item;
					}
				}
				if (contextName == "PlayerStatusWidgetContext")
				{
					return CheckPlayerStatusWidgetEnable();
				}
			}
			catch (InvalidOperationException)
			{
				return null;
			}
			return null;
		}

		private BaseContext CheckPlayerStatusWidgetEnable()
		{
			MonoMainCanvas monoMainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas() as MonoMainCanvas;
			if (monoMainCanvas != null && monoMainCanvas.playerBar.IsActive)
			{
				return monoMainCanvas.playerBar;
			}
			return null;
		}

		private bool OnGetFinishGuideDataRsp(GetFinishGuideDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				foreach (uint item in rsp.guide_id_list)
				{
					UpdateFinishTutorialID((int)item);
				}
			}
			FilterUnfinishedTutorial();
			TryToDoTutorialWhenLogin();
			return false;
		}

		private bool OnFinishGuideReportRsp(FinishGuideReportRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				List<uint> guide_id_list = rsp.guide_id_list;
				if (_tutorialWaitingStatus == TutorialWaitingStatus.NoWaiting)
				{
					foreach (uint item in guide_id_list)
					{
						UpdateFinishTutorialID((int)item);
					}
				}
				else
				{
					if (!CheckTutorialListEqual(guide_id_list, _waitingList))
					{
						return false;
					}
					if (_tutorialWaitingStatus == TutorialWaitingStatus.WaitingFinishOnStart)
					{
						_waitingListStatus = WaitingListStatus.Finished;
						foreach (uint item2 in guide_id_list)
						{
							UpdateFinishTutorialID((int)item2);
						}
					}
					else if (_tutorialWaitingStatus == TutorialWaitingStatus.WaitingFinishOnEndPre)
					{
						_waitingListStatus = ((!rsp.is_finish) ? WaitingListStatus.WaitingFinish : WaitingListStatus.Finished);
						if (_waitingListStatus == WaitingListStatus.Finished)
						{
							foreach (uint item3 in guide_id_list)
							{
								UpdateFinishTutorialID((int)item3);
							}
						}
					}
					else if (_tutorialWaitingStatus == TutorialWaitingStatus.WaitingFinishOnEnd)
					{
						_waitingListStatus = ((!rsp.is_finish) ? WaitingListStatus.WaitingFinish : WaitingListStatus.Finished);
						if (_waitingListStatus == WaitingListStatus.WaitingFinish)
						{
							GeneralLogicManager.RestartGame();
							return false;
						}
						foreach (uint item4 in guide_id_list)
						{
							UpdateFinishTutorialID((int)item4);
						}
					}
					else if (_tutorialWaitingStatus == TutorialWaitingStatus.WaitingSkip)
					{
						_waitingListStatus = WaitingListStatus.Finished;
						foreach (uint item5 in guide_id_list)
						{
							UpdateFinishTutorialID((int)item5);
						}
					}
				}
			}
			return false;
		}

		private bool MarkTutorialIDFinishToServer(List<int> tutorialIDList, bool isForceFinish)
		{
			bool flag = false;
			foreach (int tutorialID in tutorialIDList)
			{
				if (!_finishTutorialList.Contains(tutorialID))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			if (_waitingList != null && _waitingList.Count > 0)
			{
				return false;
			}
			_waitingList = tutorialIDList;
			_waitingListStatus = WaitingListStatus.SendToServer;
			Singleton<NetworkManager>.Instance.RequestFinishGuideReport(tutorialIDList, isForceFinish);
			return true;
		}

		private void UpdateFinishTutorialID(int tutorialID)
		{
			if (!_finishTutorialList.Contains(tutorialID))
			{
				_finishTutorialList.Add(tutorialID);
				FilterFinishedTutorial(tutorialID);
			}
		}

		private void FilterUnfinishedTutorial()
		{
			List<TutorialData> itemList = TutorialDataReader.GetItemList();
			foreach (TutorialData item in itemList)
			{
				FilterUnfinishedTutorial(item.id);
			}
		}

		private void FilterUnfinishedTutorial(int tutorialID)
		{
			if (_finishTutorialList.Contains(tutorialID))
			{
				FilterFinishedTutorial(tutorialID);
				return;
			}
			TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(tutorialID);
			if (tutorialDataByKey == null)
			{
				return;
			}
			string triggerUIContextName = tutorialDataByKey.triggerUIContextName;
			if (_contextTutorialDict.ContainsKey(triggerUIContextName))
			{
				List<int> list = _contextTutorialDict[triggerUIContextName];
				if (list != null && !list.Contains(tutorialID))
				{
					list.Add(tutorialID);
				}
			}
			else
			{
				List<int> list2 = new List<int>();
				list2.Add(tutorialID);
				_contextTutorialDict.Add(triggerUIContextName, list2);
			}
			int triggerMissionID = tutorialDataByKey.triggerMissionID;
			if (_missionTutorialDict.ContainsKey(triggerMissionID))
			{
				List<int> list3 = _missionTutorialDict[triggerMissionID];
				if (list3 != null && !list3.Contains(tutorialID))
				{
					list3.Add(tutorialID);
				}
			}
			else
			{
				List<int> list4 = new List<int>();
				list4.Add(tutorialID);
				_missionTutorialDict.Add(triggerMissionID, list4);
			}
		}

		private void FilterFinishedTutorial(int tutorialID)
		{
			if (tutorialID > LevelTutorialModule.BASE_LEVEL_TUTORIAL_ID || !_finishTutorialList.Contains(tutorialID))
			{
				return;
			}
			TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(tutorialID);
			if (tutorialDataByKey == null)
			{
				return;
			}
			string triggerUIContextName = tutorialDataByKey.triggerUIContextName;
			if (_contextTutorialDict.ContainsKey(triggerUIContextName))
			{
				List<int> list = _contextTutorialDict[triggerUIContextName];
				if (list != null && list.Contains(tutorialID))
				{
					list.Remove(tutorialID);
				}
			}
			int triggerMissionID = tutorialDataByKey.triggerMissionID;
			if (_missionTutorialDict.ContainsKey(triggerMissionID))
			{
				List<int> list2 = _missionTutorialDict[triggerMissionID];
				if (list2 != null && list2.Contains(tutorialID))
				{
					list2.Remove(tutorialID);
				}
			}
		}

		private void TryToDoTutorialWhenLogin()
		{
			if (_isInTutorial)
			{
				return;
			}
			foreach (string key in _contextTutorialDict.Keys)
			{
				BaseContext baseContext = CheckContextEnable(key);
				if (baseContext == null)
				{
					continue;
				}
				List<int> list = _contextTutorialDict[key];
				foreach (int item in list)
				{
					TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(item);
					if (tutorialDataByKey == null || !CheckCanDoStep(tutorialDataByKey, tutorialDataByKey.startStepID))
					{
						continue;
					}
					DoTutorial(tutorialDataByKey, tutorialDataByKey.startStepID);
					return;
				}
			}
		}

		private IEnumerator FinishOnStartWait(List<int> finishOnStartList, TutorialData tutorialData, TutorialStepData tutorialStepData)
		{
			_tutorialWaitingStatus = TutorialWaitingStatus.WaitingFinishOnStart;
			while (_waitingList == finishOnStartList && _waitingListStatus == WaitingListStatus.SendToServer)
			{
				yield return null;
			}
			_waitingList = null;
			_waitingListStatus = WaitingListStatus.Invalid;
			_waitingFinishOnStartCoroutine = null;
			_wheelContext = null;
			_tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
			DoAfterFinishOnStart(tutorialData, tutorialStepData);
		}

		private void DoAfterFinishOnStart(TutorialData tutorialData, TutorialStepData tutorialStepData)
		{
			List<int> finishOnEndList = tutorialStepData.FinishOnEnd;
			if (finishOnEndList.Count > 0)
			{
				if (MarkTutorialIDFinishToServer(finishOnEndList, false))
				{
					_wheelContext = new LoadingWheelWidgetContext(130, delegate
					{
						RetryRequestFinishGuideReport(finishOnEndList, false);
					});
					Singleton<MainUIManager>.Instance.ShowWidget(_wheelContext);
					if (Singleton<ApplicationManager>.Instance != null)
					{
						_waitingFinishOnEndPreCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(FinishOnEndPreWait(finishOnEndList, tutorialData, tutorialStepData));
					}
				}
				else
				{
					DoAfterFinishOnEndPre(tutorialData, tutorialStepData);
				}
			}
			else
			{
				DoAfterFinishOnEndPre(tutorialData, tutorialStepData);
			}
		}

		private void RetryRequestFinishGuideReport(List<int> tutorialIDList, bool isForceFinish)
		{
			Singleton<NetworkManager>.Instance.RequestFinishGuideReport(tutorialIDList, isForceFinish);
			_wheelContext = new LoadingWheelWidgetContext(130, delegate
			{
				RetryRequestFinishGuideReport(tutorialIDList, isForceFinish);
			});
			Singleton<MainUIManager>.Instance.ShowWidget(_wheelContext);
		}

		private IEnumerator FinishOnEndPreWait(List<int> finishOnEndList, TutorialData tutorialData, TutorialStepData tutorialStepData)
		{
			_tutorialWaitingStatus = TutorialWaitingStatus.WaitingFinishOnEndPre;
			while (_waitingList == finishOnEndList && _waitingListStatus == WaitingListStatus.SendToServer)
			{
				yield return null;
			}
			if (_waitingListStatus == WaitingListStatus.Finished)
			{
				_waitingList = null;
				_waitingListStatus = WaitingListStatus.Invalid;
			}
			_waitingFinishOnEndPreCoroutine = null;
			_wheelContext = null;
			_tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
			DoAfterFinishOnEndPre(tutorialData, tutorialStepData);
		}

		private void DoAfterFinishOnEndPre(TutorialData tutorialData, TutorialStepData tutorialStepData)
		{
			bool bShowSkip = CheckCanSkip(tutorialData, tutorialStepData.id);
			string targetUIPath = tutorialStepData.targetUIPath;
			Transform highlightTrans = null;
			if (targetUIPath != string.Empty)
			{
				BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
				highlightTrans = sceneCanvas.transform.Find(targetUIPath);
			}
			bool disableMask = false;
			if (tutorialStepData.stepType == 2)
			{
				disableMask = true;
			}
			Action preCallback = null;
			List<int> finishOnEndList = tutorialStepData.FinishOnEnd;
			Func<bool> pointerUpCallback = delegate
			{
				NewbieDialogCallback(tutorialData, tutorialStepData, finishOnEndList.Count > 0);
				return false;
			};
			if (tutorialStepData.nextStepID == 0)
			{
				preCallback = delegate
				{
					DoTutorialOver(tutorialData, tutorialStepData);
				};
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
			{
				disableMask = disableMask,
				highlightTrans = highlightTrans,
				highlightPath = targetUIPath,
				bubblePosType = (NewbieDialogContext.BubblePosType)tutorialStepData.bubblePosType,
				handIconPosType = (NewbieDialogContext.HandIconPosType)tutorialStepData.handIconPosType,
				disableHighlightEffect = !tutorialStepData.playEffect,
				guideDesc = LocalizationGeneralLogic.GetText(tutorialStepData.guideDesc),
				preCallback = preCallback,
				pointerUpCallback = pointerUpCallback,
				destroyButNoClickedCallback = delegate
				{
					DoTutorialOver(tutorialData, tutorialStepData);
				},
				delayShowTime = tutorialStepData.delayTime,
				bShowReward = (tutorialStepData.stepType == 3),
				rewardID = tutorialData.Reward,
				guideID = tutorialData.id,
				bShowSkip = bShowSkip
			});
		}

		private void NewbieDialogCallback(TutorialData tutorialData, TutorialStepData tutorialStepData, bool waitServer)
		{
			if (waitServer)
			{
				_wheelContext = new LoadingWheelWidgetContext(130, delegate
				{
					RetryRequestFinishGuideReport(tutorialStepData.FinishOnEnd, false);
				});
				Singleton<MainUIManager>.Instance.ShowWidget(_wheelContext);
				if (Singleton<ApplicationManager>.Instance != null)
				{
					_waitingFinishOnEndCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(FinishOnEndWait(tutorialData, tutorialStepData));
				}
			}
			else
			{
				DoAfterFinishOnEnd(tutorialData, tutorialStepData);
			}
		}

		private IEnumerator FinishOnEndWait(TutorialData tutorialData, TutorialStepData tutorialStepData)
		{
			_tutorialWaitingStatus = TutorialWaitingStatus.WaitingFinishOnEnd;
			while (_waitingList == tutorialStepData.FinishOnEnd && _waitingListStatus != WaitingListStatus.Finished)
			{
				yield return null;
			}
			_waitingList = null;
			_waitingListStatus = WaitingListStatus.Invalid;
			_waitingFinishOnEndCoroutine = null;
			_wheelContext = null;
			_tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
			DoAfterFinishOnEnd(tutorialData, tutorialStepData);
		}

		private void DoAfterFinishOnEnd(TutorialData tutorialData, TutorialStepData tutorialStepData)
		{
			if (tutorialStepData.nextStepID != 0)
			{
				DoNextStep(tutorialData, tutorialStepData);
			}
		}

		private bool CheckTutorialListEqual(List<uint> tutorialList1, List<int> tutorialList2)
		{
			if (tutorialList1 == null || tutorialList2 == null)
			{
				return false;
			}
			if (tutorialList1.Count != tutorialList2.Count)
			{
				return false;
			}
			int count = tutorialList1.Count;
			for (int i = 0; i < count; i++)
			{
				if (tutorialList1[i] != (uint)tutorialList2[i])
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckTutorialListEqual(List<int> tutorialList1, List<int> tutorialList2)
		{
			if (tutorialList1.Count != tutorialList2.Count)
			{
				return false;
			}
			int count = tutorialList1.Count;
			for (int i = 0; i < count; i++)
			{
				if (tutorialList1[i] != tutorialList2[i])
				{
					return false;
				}
			}
			return true;
		}

		private void StopAllCoroutines()
		{
			if (_waitingFinishOnStartCoroutine != null)
			{
				if (Singleton<ApplicationManager>.Instance != null)
				{
					Singleton<ApplicationManager>.Instance.StopCoroutine(_waitingFinishOnStartCoroutine);
				}
				_waitingFinishOnStartCoroutine = null;
			}
			if (_waitingFinishOnEndPreCoroutine != null)
			{
				if (Singleton<ApplicationManager>.Instance != null)
				{
					Singleton<ApplicationManager>.Instance.StopCoroutine(_waitingFinishOnEndPreCoroutine);
				}
				_waitingFinishOnEndPreCoroutine = null;
			}
			if (_waitingFinishOnEndCoroutine != null)
			{
				if (Singleton<ApplicationManager>.Instance != null)
				{
					Singleton<ApplicationManager>.Instance.StopCoroutine(_waitingFinishOnEndCoroutine);
				}
				_waitingFinishOnEndCoroutine = null;
			}
			if (_waitingSkip != null)
			{
				if (Singleton<ApplicationManager>.Instance != null)
				{
					Singleton<ApplicationManager>.Instance.StopCoroutine(_waitingSkip);
				}
				_waitingSkip = null;
			}
		}
	}
}
