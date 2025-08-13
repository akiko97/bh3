using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class LevelEndPageContext : BasePageContext
	{
		private const float WAIT_TIME = 10f;

		private CanvasTimer _timer;

		private int ANIMATOR_LEVEL_WIN_BOOL_ID = Animator.StringToHash("LevelWin");

		private int ANIMATOR_TRIGGER_PLAY_ID = Animator.StringToHash("TriggerPlay");

		public readonly bool isSuccess;

		public readonly EvtLevelState.LevelEndReason endReason;

		private int _cgId;

		private bool _forceEnableWhenSetup;

		public LevelEndPageContext(bool isSuccess)
		{
			config = new ContextPattern
			{
				contextName = "LevelEndPageContext",
				viewPrefabPath = "UI/Menus/Page/InLevel/LevelEndPage"
			};
			this.isSuccess = isSuccess;
			_cgId = 0;
			if (Singleton<LevelScoreManager>.Instance != null)
			{
				Singleton<LevelScoreManager>.Instance.isLevelSuccess = isSuccess;
			}
		}

		public LevelEndPageContext(EvtLevelState.LevelEndReason reason, bool forceEnableWhenSetup = false, int cgId = 0)
		{
			config = new ContextPattern
			{
				contextName = "LevelEndPageContext",
				viewPrefabPath = "UI/Menus/Page/InLevel/LevelEndPage"
			};
			endReason = reason;
			isSuccess = reason == EvtLevelState.LevelEndReason.EndWin;
			if (Singleton<LevelScoreManager>.Instance != null)
			{
				Singleton<LevelScoreManager>.Instance.isLevelSuccess = isSuccess;
			}
			_forceEnableWhenSetup = forceEnableWhenSetup;
			_cgId = cgId;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Button").GetComponent<Button>(), OnFinishButtonClicked);
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 46)
			{
				return OnStageEndRsp(pkt.getData<StageEndRsp>());
			}
			return false;
		}

		protected override bool SetupView()
		{
			Singleton<LevelScoreManager>.Instance.HandleLevelEnd(endReason);
			base.view.transform.Find("WinPanel").gameObject.SetActive(isSuccess);
			base.view.transform.Find("LosePanel").gameObject.SetActive(!isSuccess);
			base.view.transform.Find("Button").gameObject.SetActive(false);
			SetupTitle();
			if (Singleton<LevelScoreManager>.Instance.isTryLevel || Singleton<LevelScoreManager>.Instance.isDebugLevel)
			{
				Singleton<LevelScoreManager>.Destroy();
				base.view.transform.Find("WinPanel/ChallengePanel").gameObject.SetActive(false);
				Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_LevelTryEnd"), null, Finish, LocalizationGeneralLogic.GetText("Menu_Tips")));
			}
			else
			{
				if (isSuccess)
				{
					base.view.transform.Find("WinPanel/ChallengePanel").gameObject.SetActive(false);
				}
				if (!_forceEnableWhenSetup && (endReason == EvtLevelState.LevelEndReason.EndWin || endReason == EvtLevelState.LevelEndReason.EndLoseNotMeetCondition))
				{
					base.view.transform.Find("WinPanel").gameObject.SetActive(false);
					base.view.transform.Find("LosePanel").gameObject.SetActive(false);
					SetActive(false);
				}
				LoadingWheelWidgetContext loadingWheelWidgetContext = new LoadingWheelWidgetContext(46);
				loadingWheelWidgetContext.ignoreMaxWaitTime = true;
				Singleton<MainUIManager>.Instance.ShowWidget(loadingWheelWidgetContext);
				if (!Singleton<LevelScoreManager>.Instance.RequestLevelEnd())
				{
					loadingWheelWidgetContext.Finish();
				}
			}
			return false;
		}

		private bool OnStageEndRsp(StageEndRsp rsp)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			Singleton<LevelScoreManager>.Instance.stageEndRsp = rsp;
			Singleton<LevelModule>.Instance.ClearLevelEndReqInfo();
			if ((int)rsp.retcode == 0)
			{
				if (isSuccess)
				{
					SetupWinPanel(rsp);
				}
				else
				{
					SetupHintListPanel();
				}
				SetupDelayTimer();
			}
			else
			{
				string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput));
				SetupDelayTimer();
			}
			return false;
		}

		private void OnFinishButtonClicked()
		{
			if (_cgId != 0)
			{
				CgDataItem cgDataItem = Singleton<CGModule>.Instance.GetCgDataItem(_cgId);
				bool allowPlayCgWithSkipBtn = false;
				if (cgDataItem != null)
				{
					allowPlayCgWithSkipBtn = Singleton<LevelDesignManager>.Instance.AllowSkipVideo(_cgId);
					Singleton<CGModule>.Instance.MarkCGIDFinish(cgDataItem.cgID);
				}
				MonoInLevelUICanvas canvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
				if (canvas != null && cgDataItem != null)
				{
					MonoInLevelUICanvas monoInLevelUICanvas = canvas;
					Action fadeStartCallback = delegate
					{
						if (_timer != null)
						{
							_timer.Destroy();
						}
						Singleton<WwiseAudioManager>.Instance.Post("UI_CG_Enter_Long");
					};
					monoInLevelUICanvas.FadeOutStageTransitPanel(1f, false, fadeStartCallback, delegate
					{
						canvas.VideoPlayer.LoadOrPlayVideo(OnVideoEndCallback: OnCgFinishedCallback, withSkipBtn: allowPlayCgWithSkipBtn, cgDataItem: cgDataItem, OverrideSkipCallback: null, OnVideoBeginCallback: null, controlType: MonoVideoPlayer.VideoControlType.Play, endDestroyDisplay: false);
					});
				}
				else
				{
					Finish();
				}
			}
			else
			{
				Finish();
			}
		}

		private void OnCgFinishedCallback(CgDataItem cgDataItem)
		{
			Singleton<CGModule>.Instance.MarkCGIDFinish(cgDataItem.cgID);
			Singleton<EventManager>.Instance.FireEvent(new EvtVideoState((uint)cgDataItem.cgID, EvtVideoState.State.Finish));
			Finish();
		}

		private void Finish()
		{
			if (_timer != null)
			{
				_timer.Destroy();
			}
			if (BehaviorManager.instance != null && BehaviorManager.instance.gameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(BehaviorManager.instance.gameObject);
			}
			Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceshipWithUI");
		}

		private void SetupWinPanel(StageEndRsp rsp)
		{
			Singleton<MainUIManager>.Instance.LockUI(true, 2f);
			base.view.transform.Find("WinPanel/ChallengePanel").gameObject.SetActive(true);
			base.view.transform.Find("WinPanel/DataPanel/0/Num").GetComponent<Text>().text = "0";
			base.view.transform.Find("WinPanel/DataPanel/0").GetComponent<MonoNumberJump>().SetTargetValue(Singleton<LevelScoreManager>.Instance.maxComboNum);
			base.view.transform.Find("WinPanel/DataPanel/1/Num").GetComponent<Text>().text = "00:00";
			base.view.transform.Find("WinPanel/DataPanel/1").GetComponent<MonoNumberJump>().SetTargetValue(GetStageTime(), true);
			List<LevelChallengeDataItem> list = new List<LevelChallengeDataItem>();
			Dictionary<int, LevelChallengeDataItem> dictionary = new Dictionary<int, LevelChallengeDataItem>();
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			LevelMetaData levelMeta = LevelMetaDataReader.TryGetLevelMetaDataByKey((int)rsp.stage_id);
			foreach (int configChallengeId in instance.configChallengeIds)
			{
				LevelChallengeDataItem levelChallengeDataItem = new LevelChallengeDataItem(configChallengeId, levelMeta);
				levelChallengeDataItem.Finished = true;
				list.Add(levelChallengeDataItem);
				dictionary[configChallengeId] = levelChallengeDataItem;
			}
			foreach (int trackChallengeId in instance.trackChallengeIds)
			{
				dictionary[trackChallengeId].Finished = false;
			}
			Dictionary<int, StageChallengeData> dictionary2 = new Dictionary<int, StageChallengeData>();
			foreach (StageChallengeData item in rsp.challenge_list)
			{
				int challenge_index = (int)item.challenge_index;
				if (challenge_index < list.Count)
				{
					int challengeId = list[challenge_index].challengeId;
					dictionary[challengeId].Finished = true;
					dictionary2[challengeId] = item;
				}
			}
			Dictionary<int, StageSpecialChallengeData> dictionary3 = new Dictionary<int, StageSpecialChallengeData>();
			foreach (StageSpecialChallengeData item2 in rsp.special_challenge_list)
			{
				int challenge_index2 = (int)item2.challenge_index;
				if (challenge_index2 < list.Count)
				{
					int challengeId2 = list[challenge_index2].challengeId;
					dictionary[challengeId2].Finished = true;
					dictionary3[challengeId2] = item2;
				}
			}
			Transform transform = base.view.transform.Find("WinPanel/ChallengePanel");
			for (int i = 0; i < list.Count; i++)
			{
				LevelChallengeDataItem levelChallengeDataItem2 = list[i];
				Transform child = transform.GetChild(i);
				child.Find("Content").GetComponent<Text>().text = levelChallengeDataItem2.DisplayTarget;
				child.Find("Achieve").gameObject.SetActive(levelChallengeDataItem2.Finished);
				child.Find("Unachieve").gameObject.SetActive(!levelChallengeDataItem2.Finished);
				if (dictionary2.ContainsKey(levelChallengeDataItem2.challengeId))
				{
					StageChallengeData challengeData = dictionary2[levelChallengeDataItem2.challengeId];
					child.Find("Achieve/CompleteMark").gameObject.SetActive(false);
					child.Find("Achieve/Reward").gameObject.SetActive(true);
					child.Find("Achieve/Box").gameObject.SetActive(false);
					int targetValue = SetupReward(child.Find("Achieve/Reward"), challengeData);
					child.GetComponent<MonoNumberJump>().SetTargetValue(targetValue);
				}
				else if (dictionary3.ContainsKey(levelChallengeDataItem2.challengeId))
				{
					child.Find("Achieve/CompleteMark").gameObject.SetActive(false);
					child.Find("Achieve/Reward").gameObject.SetActive(false);
					child.Find("Achieve/Box").gameObject.SetActive(true);
				}
				else
				{
					child.Find("Achieve/CompleteMark").gameObject.SetActive(true);
					child.Find("Achieve/Reward").gameObject.SetActive(false);
				}
			}
			PlayLevelEndAnim(true);
		}

		private int SetupReward(Transform trans, StageChallengeData challengeData)
		{
			int result = 0;
			Image component = trans.Find("Icon").GetComponent<Image>();
			Text component2 = trans.Find("Num").GetComponent<Text>();
			if (challengeData.reward.hcoin != 0)
			{
				component.sprite = UIUtil.GetResourceSprite(ResourceType.Hcoin);
				result = (int)challengeData.reward.hcoin;
			}
			if (challengeData.reward.scoin != 0)
			{
				component.sprite = UIUtil.GetResourceSprite(ResourceType.Scoin);
				result = (int)challengeData.reward.scoin;
			}
			if (challengeData.reward.stamina != 0)
			{
				component.sprite = UIUtil.GetResourceSprite(ResourceType.Stamina);
				result = (int)challengeData.reward.stamina;
			}
			if (challengeData.reward.skill_point != 0)
			{
				component.sprite = UIUtil.GetResourceSprite(ResourceType.SkillPoint);
				result = (int)challengeData.reward.skill_point;
			}
			if (challengeData.reward.exp != 0)
			{
				component.sprite = UIUtil.GetResourceSprite(ResourceType.PlayerExp);
				result = (int)challengeData.reward.exp;
			}
			if (challengeData.reward.item_list.Count > 0)
			{
				RewardItemData val = challengeData.reward.item_list[0];
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)val.id);
				dummyStorageDataItem.level = (int)val.level;
				dummyStorageDataItem.number = (int)val.num;
				component.sprite = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
				result = dummyStorageDataItem.number;
			}
			component2.text = result.ToString();
			return result;
		}

		private void SetupHintListPanel()
		{
			SetupLoseDescription();
			PlayLevelEndAnim(false);
		}

		private void SetupLoseDescription()
		{
			LevelDataItem levelById = Singleton<LevelModule>.Instance.GetLevelById(Singleton<LevelScoreManager>.Instance.LevelId);
			List<string> loseDescList = levelById.LoseDescList;
			List<string> list = new List<string>();
			list.AddRange(loseDescList);
			if (loseDescList.Count > 3)
			{
				list.Shuffle();
				list = list.GetRange(0, 3);
			}
			Transform transform = base.view.transform.Find("LosePanel/HintListPanel");
			for (int i = 0; i < list.Count; i++)
			{
				Transform transform2 = transform.FindChild(i + "/Content");
				transform2.GetComponent<Text>().text = LocalizationGeneralLogic.GetText(list[i]);
			}
		}

		private void SetupDelayTimer()
		{
			base.view.transform.Find("Button").gameObject.SetActive(true);
			if (_timer != null)
			{
				_timer.Destroy();
			}
			float num = 10f;
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer((!isSuccess) ? (num * 2f) : num, 0f);
			_timer.timeUpCallback = OnFinishButtonClicked;
		}

		private void SetupTitle()
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected I4, but got Unknown
			LevelDataItem levelDataItem = Singleton<LevelModule>.Instance.TryGetLevelById(Singleton<LevelScoreManager>.Instance.LevelId);
			if (levelDataItem != null)
			{
				string text = string.Empty;
				string empty = string.Empty;
				StageType levelType = levelDataItem.LevelType;
				switch ((int)levelType - 1)
				{
				case 0:
					text = levelDataItem.StageName;
					break;
				case 1:
				case 2:
				{
					WeekDayActivityDataItem weekDayActivityByID = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(levelDataItem.ActID);
					text = weekDayActivityByID.GetActitityTitle();
					break;
				}
				}
				empty = levelDataItem.Title;
				base.view.transform.Find("WinPanel/Title/LevelInfo/ActName").GetComponent<Text>().text = text;
				base.view.transform.Find("LosePanel/Title/LevelInfo/ActName").GetComponent<Text>().text = text;
				base.view.transform.Find("WinPanel/Title/LevelInfo/LevelName").GetComponent<Text>().text = empty;
				base.view.transform.Find("LosePanel/Title/LevelInfo/LevelName").GetComponent<Text>().text = empty;
			}
		}

		public override void Destroy()
		{
			if (_timer != null)
			{
				_timer.Destroy();
			}
			base.Destroy();
		}

		private int GetStageTime()
		{
			if (Singleton<LevelManager>.Instance.levelActor != null && Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorTimerPlugin>() != null)
			{
				return Mathf.FloorToInt(Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorTimerPlugin>().Timer + 1f);
			}
			return 0;
		}

		private void PlayLevelEndAnim(bool isWin)
		{
			Animator component = base.view.GetComponent<Animator>();
			component.SetTrigger(ANIMATOR_TRIGGER_PLAY_ID);
			component.SetBool(ANIMATOR_LEVEL_WIN_BOOL_ID, isWin);
		}
	}
}
