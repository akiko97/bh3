using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class LevelDetailDialogContextV2 : BaseDialogContext
	{
		public readonly LevelDataItem levelData;

		private LevelDiffculty difficulty;

		public LevelDetailDialogContextV2(LevelDataItem levelData, LevelDiffculty difficulty)
		{
			config = new ContextPattern
			{
				contextName = "LevelDetailDialogContext1",
				viewPrefabPath = "UI/Menus/Dialog/LevelDetailDialogV2",
				ignoreNotify = false
			};
			this.levelData = levelData;
			this.difficulty = difficulty;
			uiType = UIType.SpecialDialog;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Missions/Prepare/Btn").GetComponent<Button>(), OnOkButtonCallBack);
			BindViewCallback(base.view.transform.Find("Missions/Challenge/ResetBtn").GetComponent<Button>(), OnResetButtonCallBack);
		}

		protected override bool SetupView()
		{
			SetupProfile();
			SetupMissionPanel();
			SetupCost();
			return false;
		}

		public void OnOkButtonCallBack()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if ((int)levelData.LevelType == 1 && levelData.UnlockChanllengeNum > Singleton<LevelModule>.Instance.GetChapterById(levelData.ChapterID).GetTotalFinishedChanllengeNum(levelData.Diffculty))
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ChallengeLackLock", levelData.UnlockChanllengeNum)));
			}
			else if (levelData.isDropActivityOpen && levelData.dropActivityEndTime <= TimeUtil.Now)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.DoubleButton,
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_LevelDropActivityEndHint"),
					buttonCallBack = delegate(bool confirmed)
					{
						if (confirmed)
						{
							EnterPreparePage();
						}
					}
				});
				SetupView();
			}
			else
			{
				EnterPreparePage();
			}
		}

		public void OnResetButtonCallBack()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new ResetLevelDialogContext(levelData, this));
		}

		private void SetupProfile()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected I4, but got Unknown
			Transform transform = base.view.transform.Find("Profile");
			StageType levelType = levelData.LevelType;
			switch ((int)levelType - 1)
			{
			case 0:
			{
				ActDataItem actDataItem = new ActDataItem(levelData.ActID);
				transform.Find("Title/Desc").GetComponent<Text>().text = actDataItem.actTitle + " " + actDataItem.actName;
				break;
			}
			case 1:
			case 2:
			{
				WeekDayActivityDataItem weekDayActivityByID = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(levelData.ActID);
				transform.Find("Title/Desc").GetComponent<Text>().text = weekDayActivityByID.GetActitityTitle();
				break;
			}
			}
			transform.Find("Title/Desc").GetComponent<TypewriterEffect>().RestartRead();
			transform.Find("Pic/LevelName").gameObject.SetActive(false);
			transform.Find("Pic/LevelName").GetComponent<Text>().text = levelData.StageName;
			transform.Find("Pic/Icon").GetComponent<Image>().sprite = levelData.GetDetailPicSprite();
			transform.Find("Info/Text").GetComponent<Text>().text = levelData.Desc;
			transform.Find("Info/Text").GetComponent<TypewriterEffect>().RestartRead();
		}

		private void SetupMissionPanel()
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Invalid comparison between Unknown and I4
			Transform transform = base.view.transform.Find("Missions");
			transform.Find("Title/Name/Text").GetComponent<Text>().text = levelData.Title;
			transform.Find("Title/HorizontialLayOut/Recommand/LvNum").GetComponent<Text>().text = levelData.RecommandLv.ToString();
			if ((int)levelData.LevelType != 1)
			{
				transform.Find("Title/HorizontialLayOut/Difficulty").gameObject.SetActive(false);
			}
			else
			{
				Transform transform2 = transform.Find("Title/HorizontialLayOut/Difficulty/Difficulty");
				Color difficultyColor = Miscs.GetDifficultyColor(difficulty);
				string text = Miscs.GetDifficultyDesc(difficulty).Substring(0, 2);
				string difficultyMark = UIUtil.GetDifficultyMark(difficulty);
				transform2.Find("Color").GetComponent<Image>().color = difficultyColor;
				transform2.Find("Desc").GetComponent<Text>().text = text;
				transform2.Find("Icon/Image").GetComponent<Image>().color = difficultyColor;
				transform2.Find("Icon/Image/Text").GetComponent<Text>().text = difficultyMark;
				base.view.transform.Find("BG/GradualRight").GetComponent<Image>().color = difficultyColor;
				transform.Find("BG/GradualLeft").GetComponent<Image>().color = difficultyColor;
			}
			Transform transform3 = transform.Find("MissionList/MissionPanel");
			for (int i = 0; i < transform3.childCount; i++)
			{
				Transform child = transform3.GetChild(i);
				if (i >= levelData.challengeList.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				LevelChallengeDataItem levelChallengeDataItem = levelData.challengeList[i];
				string displayTarget = levelChallengeDataItem.DisplayTarget;
				child.Find("Achieve/Text").GetComponent<Text>().text = displayTarget;
				child.Find("Unachieve/Text").GetComponent<Text>().text = displayTarget;
				child.Find("Achieve").gameObject.SetActive(levelChallengeDataItem.Finished);
				child.Find("Unachieve").gameObject.SetActive(!levelChallengeDataItem.Finished);
				child.Find("Loop").gameObject.SetActive(levelChallengeDataItem.IsSpecialChallenge());
			}
			RefreshDropList();
			RefreshChallengeNumber(transform);
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.StageDropListUpdated)
			{
				RefreshDropList();
				RefreshChallengeNumber();
			}
			return false;
		}

		private void RefreshDropList()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Invalid comparison between Unknown and I4
			string empty = string.Empty;
			List<int> list;
			if ((int)levelData.LevelType == 1 && levelData.displayFirstDropList.Count > 0 && (int)levelData.status != 3)
			{
				list = levelData.displayFirstDropList;
				empty = LocalizationGeneralLogic.GetText("Menu_FirstDropList");
			}
			else
			{
				list = levelData.displayDropList;
				empty = LocalizationGeneralLogic.GetText("Menu_DisplayDropList");
			}
			List<int> list2 = levelData.displayBonusDropList;
			if (!levelData.isDropActivityOpen)
			{
				list2 = new List<int>();
			}
			base.view.transform.Find("Missions/Drop/Title/Text").GetComponent<Text>().text = empty;
			Transform transform = base.view.transform.Find("Missions/Drop/Drops/ScollerContent");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				bool flag = i < list.Count + list2.Count;
				bool flag2 = i >= list.Count;
				child.gameObject.SetActive(flag);
				if (flag)
				{
					StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((!flag2) ? list[i] : list2[i - list.Count]);
					child.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, OnDropItemBtnClick, true);
					child.Find("BG/Desc").gameObject.SetActive(false);
					child.Find("AddDropIcon").gameObject.SetActive(flag2);
				}
			}
			base.view.transform.Find("Missions/Drop/Title/IconDropUp").gameObject.SetActive(levelData.isDropActivityOpen && levelData.isDoubleDrop);
		}

		public void RefreshChallengeNumber(Transform missionTrans = null)
		{
			if (missionTrans == null)
			{
				missionTrans = base.view.transform.Find("Missions");
			}
			bool flag = levelData.isDropActivityOpen && levelData.dropActivityMaxEnterTimes > 0;
			int num = levelData.MaxEnterTimes - levelData.enterTimes;
			int num2 = levelData.MaxEnterTimes;
			if (flag)
			{
				num += levelData.dropActivityMaxEnterTimes - levelData.dropActivityEnterTimes;
				num2 += levelData.dropActivityMaxEnterTimes;
			}
			if (num2 < 65535)
			{
				missionTrans.Find("Challenge").gameObject.SetActive(true);
				missionTrans.Find("Challenge/Frame").gameObject.SetActive(!flag);
				missionTrans.Find("Challenge/FrameFever").gameObject.SetActive(flag);
				string arg = ((num <= 0) ? "red" : "white");
				string text = string.Format(" <color={0}>{1}</color>/{2}", arg, num, num2);
				missionTrans.Find("Challenge/EnterNumber").GetComponent<Text>().text = text;
				missionTrans.Find("Challenge/ResetBtn").gameObject.SetActive(num <= 0);
			}
			else
			{
				missionTrans.Find("Challenge").gameObject.SetActive(false);
			}
		}

		private void SetupCost()
		{
			base.view.transform.Find("Missions/Prepare/Cost/Stamina").GetComponent<Text>().text = levelData.StaminaCost.ToString();
		}

		private void OnDropItemBtnClick(StorageDataItemBase itemData)
		{
			UIUtil.ShowItemDetail(itemData, true);
		}

		private void EnterPreparePage()
		{
			LevelPreparePageContext context = new LevelPreparePageContext(levelData);
			Singleton<MainUIManager>.Instance.ShowPage(context);
			Singleton<AssetBundleManager>.Instance.CheckSVNVersion();
		}
	}
}
