using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class InLevelPauseDialogContext : BaseDialogContext
	{
		public delegate void OnClosedHandler();

		public const string StatusTab = "StatusTab";

		public const string SettingTab = "SettingTab";

		private const float DROP_ITEM_SCALE = 0.85f;

		private readonly string defaultTab;

		private TabManager _tabManager;

		private MonoSettingAudioTab _audioSetting;

		private MonoSettingGraphicsTab _graphicSetting;

		private LevelScoreManager _levelScoreManager;

		private List<DropItem> _dropItemList;

		private LevelDataItem _levelData;

		private MonoGridScroller _dropGridScroller;

		public event OnClosedHandler OnClosed;

		public InLevelPauseDialogContext(string defaultTab = "StatusTab")
		{
			config = new ContextPattern
			{
				contextName = "InLevelPauseDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/InLevelPauseDialog"
			};
			this.defaultTab = defaultTab;
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
		}

		protected override bool SetupView()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string searchKey = ((!string.IsNullOrEmpty(showingTabKey)) ? showingTabKey : defaultTab);
			_tabManager.Clear();
			SetupStatusTab();
			SetupSettingTab();
			_tabManager.ShowTab(searchKey);
			Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_On");
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/TabBtns/TabBtn_1").GetComponent<Button>(), OnStatusTabBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/TabBtns/TabBtn_2").GetComponent<Button>(), OnSettingTabBtnClick);
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/StatusTab/Content/ActionBtns/SurrenderBtn").GetComponent<Button>(), OnSurrenderBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/StatusTab/Content/ActionBtns/ContinueBtn").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/SettingTab/Content/Graphics/Content/NoSaveBtn").GetComponent<Button>(), OnNoSaveBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/SettingTab/Content/Graphics/Content/SaveBtn").GetComponent<Button>(), OnSaveBtnClick);
		}

		public override void Destroy()
		{
			Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_Off");
			base.Destroy();
		}

		public void OnStatusTabBtnClick()
		{
			if (_tabManager.GetShowingTabKey() != "StatusTab")
			{
				CheckSave();
			}
			_tabManager.ShowTab("StatusTab");
		}

		public void OnSettingTabBtnClick()
		{
			_tabManager.ShowTab("SettingTab");
			_audioSetting.SetupView();
			_graphicSetting.SetupView(true);
		}

		private void OnBGBtnClick()
		{
			CheckSave();
			Singleton<LevelManager>.Instance.SetPause(false);
			if (this.OnClosed != null)
			{
				this.OnClosed();
				this.OnClosed = null;
			}
			Destroy();
		}

		private void OnSurrenderBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new InLevelGiveUpConfirmDialogContext(this, OnSurrenderConfirm));
		}

		private void OnDropItemLeftArrowClick()
		{
			Transform transform = base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems/Items");
			transform.Find("ScrollView").GetComponent<MonoGridScroller>().ScrollToPreItem();
		}

		private void OnDropItemRightArrowClick()
		{
			Transform transform = base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems/Items");
			transform.Find("ScrollView").GetComponent<MonoGridScroller>().ScrollToNextItem();
		}

		private void OnSaveBtnClick()
		{
			_audioSetting.OnSaveBtnClick();
			_graphicSetting.OnSaveBtnClick();
		}

		private void OnNoSaveBtnClick()
		{
			_audioSetting.OnNoSaveBtnClick();
			_graphicSetting.OnNoSaveBtnClick();
		}

		private void SetupStatusTab()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Invalid comparison between Unknown and I4
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Invalid comparison between Unknown and I4
			GameObject gameObject = base.view.transform.Find("Dialog/StatusTab").gameObject;
			Button component = base.view.transform.Find("Dialog/TabBtns/TabBtn_1").GetComponent<Button>();
			_tabManager.SetTab("StatusTab", component, gameObject);
			_levelScoreManager = Singleton<LevelScoreManager>.Instance;
			if (_levelScoreManager.isTryLevel || _levelScoreManager.isDebugLevel || (int)_levelScoreManager.LevelType == 4)
			{
				SetupViewForTryOrDebugLevel();
				return;
			}
			_levelData = Singleton<LevelModule>.Instance.GetLevelById(_levelScoreManager.LevelId);
			string text = (((int)_levelData.LevelType != 1) ? (Singleton<LevelModule>.Instance.GetWeekDayActivityByID(_levelData.ActID).GetActitityTitle() + " " + _levelData.Title) : (_levelScoreManager.chapterTitle + " " + _levelScoreManager.actTitle + " " + _levelScoreManager.stageName + " " + _levelScoreManager.LevelTitle));
			base.view.transform.Find("Dialog/StatusTab/Content/Title/Text").GetComponent<Text>().text = text;
			base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems/Scoin/Num").GetComponent<Text>().text = Mathf.FloorToInt(_levelScoreManager.scoinInside).ToString();
			Transform transform = base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems/Items");
			_dropItemList = _levelScoreManager.GetDropListToShow();
			transform.gameObject.SetActive(_dropItemList.Count > 0);
			_dropGridScroller = transform.Find("ScrollView").GetComponent<MonoGridScroller>();
			_dropGridScroller.Init(OnScrollerChange, _dropItemList.Count);
			bool active = _dropItemList.Count > _dropGridScroller.GetMaxItemCountWithouScroll();
			transform.Find("PrevBtn").gameObject.SetActive(active);
			transform.Find("NextBtn").gameObject.SetActive(active);
			Transform transform2 = base.view.transform.Find("Dialog/StatusTab/Content/ChallengePanel");
			List<LevelChallengeDataItem> list = new List<LevelChallengeDataItem>();
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			LevelMetaData levelMetaDataByKey = LevelMetaDataReader.GetLevelMetaDataByKey(_levelData.levelId);
			foreach (int configChallengeId in instance.configChallengeIds)
			{
				LevelChallengeDataItem item = new LevelChallengeDataItem(configChallengeId, levelMetaDataByKey);
				list.Add(item);
			}
			Dictionary<int, BaseLevelChallenge> dictionary = new Dictionary<int, BaseLevelChallenge>();
			foreach (BaseLevelChallenge challenge in Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelChallengeHelperPlugin>().challengeList)
			{
				dictionary[challenge.challengeId] = challenge;
			}
			for (int i = 0; i < list.Count; i++)
			{
				LevelChallengeDataItem levelChallengeDataItem = list[i];
				Transform child = transform2.GetChild(i);
				child.Find("Content").GetComponent<Text>().text = levelChallengeDataItem.DisplayTarget;
				bool flag = !dictionary.ContainsKey(levelChallengeDataItem.challengeId) || dictionary[levelChallengeDataItem.challengeId].IsFinished();
				bool flag2 = dictionary.ContainsKey(levelChallengeDataItem.challengeId);
				child.Find("Achieve").gameObject.SetActive(flag);
				child.Find("Unachieve").gameObject.SetActive(!flag);
				child.Find("Achieve/CompleteMark").gameObject.SetActive(!flag2);
				child.Find("Achieve/Progress").gameObject.SetActive(flag2);
				child.Find("Unachieve/Progress").gameObject.SetActive(flag2);
				if (flag2)
				{
					string localizedText = GetLocalizedText(dictionary[levelChallengeDataItem.challengeId].GetProcessMsg());
					child.Find("Achieve/Progress").GetComponent<Text>().text = localizedText;
					child.Find("Unachieve/Progress").GetComponent<Text>().text = localizedText;
				}
			}
		}

		private void SetupSettingTab()
		{
			GameObject gameObject = base.view.transform.Find("Dialog/SettingTab").gameObject;
			Button component = base.view.transform.Find("Dialog/TabBtns/TabBtn_2").GetComponent<Button>();
			_tabManager.SetTab("SettingTab", component, gameObject);
			_audioSetting = gameObject.transform.Find("Content/Audio").GetComponent<MonoSettingAudioTab>();
			_audioSetting.SetupView();
			_graphicSetting = gameObject.transform.Find("Content/Graphics").GetComponent<MonoSettingGraphicsTab>();
			_graphicSetting.SetupView(true);
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			go.SetActive(active);
		}

		private void OnScrollerChange(Transform trans, int index)
		{
			Vector2 cellSize = _dropGridScroller.grid.GetComponent<GridLayoutGroup>().cellSize;
			trans.SetLocalScaleX(0.85f);
			trans.SetLocalScaleY(0.85f);
			DropItem val = _dropItemList[index];
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)val.item_id);
			dummyStorageDataItem.level = (int)val.level;
			dummyStorageDataItem.number = (int)val.num;
			trans.GetComponent<CanvasGroup>().alpha = 1f;
			trans.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true);
		}

		private void OnSurrenderConfirm()
		{
			Singleton<LevelManager>.Instance.SetPause(false);
			Destroy();
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EndLose, EvtLevelState.LevelEndReason.EndLoseQuit));
		}

		private void SetupViewForTryOrDebugLevel()
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Invalid comparison between Unknown and I4
			base.view.transform.Find("Dialog/StatusTab/Content/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_TrySkill");
			base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/StatusTab/Content/ChallengePanel").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/StatusTab/Content/Pause").gameObject.SetActive(true);
			if ((int)Singleton<LevelScoreManager>.Instance.LevelType == 4)
			{
				EndlessGroupMetaData endlessGroupMetaData = EndlessGroupMetaDataReader.TryGetEndlessGroupMetaDataByKey(Singleton<EndlessModule>.Instance.currentGroupLevel);
				base.view.transform.Find("Dialog/StatusTab/Content/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(endlessGroupMetaData.groupName);
			}
		}

		private void CheckSave()
		{
			if (!_audioSetting.CheckNeedSave() && !_graphicSetting.CheckNeedSave())
			{
				return;
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgTitle"),
				desc = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgDesc"),
				okBtnText = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgSave"),
				cancelBtnText = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgNoSave"),
				buttonCallBack = delegate(bool confirmed)
				{
					if (confirmed)
					{
						OnSaveBtnClick();
					}
				}
			});
		}

		private string GetLocalizedText(string progressStr)
		{
			switch (progressStr)
			{
			case "Succ":
				return LocalizationGeneralLogic.GetText("Menu_InLevelPauseDialog_Challenge_Succ");
			case "Fail":
				return LocalizationGeneralLogic.GetText("Menu_InLevelPauseDialog_Challenge_Fail");
			case "Doing":
				return LocalizationGeneralLogic.GetText("Menu_InLevelPauseDialog_Challenge_Doing");
			default:
				return progressStr;
			}
		}
	}
}
