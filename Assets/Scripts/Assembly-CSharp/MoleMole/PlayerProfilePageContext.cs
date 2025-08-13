using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class PlayerProfilePageContext : BasePageContext
	{
		public enum TabType
		{
			PlayerTab = 0,
			AccountTab = 1
		}

		private const string PLAYER_TAB_NAME = "Player";

		private const string ACCOUNT_TAB_NAME = "Account";

		private const int REDEEM_CODE_LENGTH = 10;

		private TabManager _tabManager;

		private MonoAvatarStar _avatarStar;

		private string _redeemCode;

		private string _currentTab;

		public PlayerProfilePageContext(TabType tabType = TabType.PlayerTab)
		{
			config = new ContextPattern
			{
				contextName = "PlayerProfilePageContext",
				viewPrefabPath = "UI/Menus/Page/PlayerProfile/PlayerProfilePage"
			};
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
			switch (tabType)
			{
			case TabType.PlayerTab:
				_currentTab = "Player";
				break;
			case TabType.AccountTab:
				_currentTab = "Account";
				break;
			}
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			switch (cmdId)
			{
			case 11:
				return SetupView();
			case 120:
				SetupAccount();
				break;
			}
			if (cmdId == 212)
			{
				OnGetRedeemCodeInfoRsp(pkt.getData<GetRedeemCodeInfoRsp>());
			}
			return false;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.MihoyoAccountInfoChanged)
			{
				SetupAccount();
			}
			if (ntf.type == NotifyTypes.MissionUpdated)
			{
				SetupCollection();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ProfilePanel/Buttons/ChangeDescBtn").GetComponent<Button>(), OnChangeDescBtnClick);
			BindViewCallback(base.view.transform.Find("ProfilePanel/Buttons/ChangeNickNameBtn").GetComponent<Button>(), OnChangeNickNameBtnClick);
			BindViewCallback(base.view.transform.Find("TabButtons/PlayerButton").GetComponent<Button>(), OnPlayerBtnClick);
			BindViewCallback(base.view.transform.Find("TabButtons/AccountButton").GetComponent<Button>(), OnAccountBtnClick);
			BindViewCallback(base.view.transform.Find("AccountPanel/LogoutBtn").GetComponent<Button>(), OnLogoutBtnClick);
			BindViewCallback(base.view.transform.Find("AccountPanel/BindingBtn").GetComponent<Button>(), OnAccountBindingBtnClick);
			BindViewCallback(base.view.transform.Find("AccountPanel/SecurityState/Mail/VerifyBtn").GetComponent<Button>(), OnMailVerifyBtnClick);
			BindViewCallback(base.view.transform.Find("AccountPanel/SecurityState/Mail/BindingBtn").GetComponent<Button>(), OnMailBindingBtnClick);
			BindViewCallback(base.view.transform.Find("AccountPanel/SecurityState/Mobile/BindingBtn").GetComponent<Button>(), OnMobileBindingBtnClick);
			BindViewCallback(base.view.transform.Find("AccountPanel/SecurityState/Identity/BindingBtn").GetComponent<Button>(), OnIdentityBindingBtnClick);
			BindViewCallback(base.view.transform.Find("EntryButtons/ItempediaButton").GetComponent<Button>(), OnItempediaBtnClick);
			BindViewCallback(base.view.transform.Find("EntryButtons/AchievementButton").GetComponent<Button>(), OnAchieveBtnClick);
			BindViewCallback(base.view.transform.Find("EntryButtons/CGReplayButton").GetComponent<Button>(), OnCGReplayBtnClick);
			BindViewCallback(base.view.transform.Find("AccountPanel/Award/GetAward").GetComponent<Button>(), OnGetRewardBtnClick);
		}

		protected override bool SetupView()
		{
			SetupPlayer();
			SetupAccount();
			SetupTabs();
			SetupCollection();
			SetPopupVisible(PopupNoticeAcitve());
			return false;
		}

		private void SetupPlayer()
		{
			PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
			int avatarID = 0;
			if (playerData.GetMemberList((StageType)1).Count > 0)
			{
				avatarID = playerData.GetMemberList((StageType)1)[0];
				AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID);
				base.view.transform.Find("ProfilePanel/Credit/Photo/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarByID.IconPath);
				base.view.transform.Find("ProfilePanel/Credit/Photo").GetComponent<Image>().sprite = avatarByID.GetBGSprite();
			}
			base.view.transform.Find("ProfilePanel/Credit/Description/NickName").GetComponent<Text>().text = playerData.NickNameText;
			base.view.transform.Find("ProfilePanel/Credit/Header/IDValue").GetComponent<Text>().text = playerData.userId.ToString();
			base.view.transform.Find("ProfilePanel/Credit/Header/LevelText").GetComponent<Text>().text = "Lv." + playerData.teamLevel;
			base.view.transform.Find("ProfilePanel/Exp/ExpText").GetComponent<Text>().text = playerData.teamExp + " / " + playerData.TeamMaxExp;
			base.view.transform.Find("ProfilePanel/Credit/Description/Intro").GetComponent<Text>().text = playerData.SelfDescText;
			base.view.transform.Find("ProfilePanel/Exp/ExpBar").GetComponent<MonoSliderGroup>().UpdateValue(playerData.teamExp, playerData.TeamMaxExp, 0f);
			AvatarDataItem avatarByID2 = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID);
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(avatarByID2.avatarID).avatarCardID);
			base.view.transform.Find("AvatarCard/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetImagePath());
			for (int i = 1; i <= 3; i++)
			{
				base.view.transform.Find("ProfilePanel/Credit/Bottom/Attr" + i).gameObject.SetActive(i == avatarByID2.Attribute);
			}
			_avatarStar = base.view.transform.Find("ProfilePanel/Credit/Bottom/Stars").GetComponent<MonoAvatarStar>();
			if (_avatarStar != null)
			{
				_avatarStar.SetupView(avatarByID2.star);
			}
		}

		private void SetupAccount()
		{
			GameObject gameObject = base.view.transform.Find("AccountPanel/SecurityState").gameObject;
			if (Singleton<AccountManager>.Instance.manager is TheOriginalAccountManager)
			{
				gameObject.SetActive(true);
				SetupSecurityState();
			}
			else
			{
				gameObject.SetActive(false);
			}
			GameObject gameObject2 = base.view.transform.Find("ProfilePanel/Credit/Description/Visitor").gameObject;
			GameObject gameObject3 = base.view.transform.Find("ProfilePanel/Credit/Description/User").gameObject;
			if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
			{
				gameObject2.SetActive(true);
				gameObject3.SetActive(false);
			}
			else
			{
				gameObject2.SetActive(false);
				gameObject3.SetActive(true);
			}
			GameObject gameObject4 = base.view.transform.Find("AccountPanel/LogoutBtn").gameObject;
			GameObject gameObject5 = base.view.transform.Find("AccountPanel/BindingBtn").gameObject;
			bool flag = !Singleton<AccountManager>.Instance.manager.IsAccountBind();
			gameObject4.SetActive(!flag);
			gameObject5.SetActive(flag);
			InputField component = base.view.transform.Find("AccountPanel/Award/InputField").GetComponent<InputField>();
			component.characterLimit = 10;
			base.view.transform.Find("AccountPanel/Award").gameObject.SetActive(!Singleton<NetworkManager>.Instance.DispatchSeverData.isReview);
		}

		private void SetupSecurityState()
		{
			GeneralLocalDataItem.AccountData account = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account;
			GameObject gameObject = base.view.transform.Find("AccountPanel/SecurityState/Mail/Content").gameObject;
			GameObject gameObject2 = base.view.transform.Find("AccountPanel/SecurityState/Mail/ContentWait").gameObject;
			GameObject gameObject3 = base.view.transform.Find("AccountPanel/SecurityState/Mail/ContentNo").gameObject;
			GameObject gameObject4 = base.view.transform.Find("AccountPanel/SecurityState/Mail/MarkYes").gameObject;
			GameObject gameObject5 = base.view.transform.Find("AccountPanel/SecurityState/Mail/MarkNo").gameObject;
			GameObject gameObject6 = base.view.transform.Find("AccountPanel/SecurityState/Mail/MarkCautious").gameObject;
			GameObject gameObject7 = base.view.transform.Find("AccountPanel/SecurityState/Mail/VerifyBtn").gameObject;
			GameObject gameObject8 = base.view.transform.Find("AccountPanel/SecurityState/Mail/BindingBtn").gameObject;
			bool active = Singleton<AccountManager>.Instance.manager.IsAccountBind() && string.IsNullOrEmpty(account.email);
			bool active2 = Singleton<AccountManager>.Instance.manager.IsAccountBind() && string.IsNullOrEmpty(account.email);
			gameObject7.SetActive(active);
			gameObject8.SetActive(active2);
			if (string.IsNullOrEmpty(account.email))
			{
				gameObject.SetActive(false);
				gameObject2.SetActive(false);
				gameObject3.SetActive(true);
				gameObject4.SetActive(false);
				gameObject5.SetActive(true);
				gameObject6.SetActive(false);
			}
			else if (account.isEmailVerify)
			{
				gameObject.SetActive(true);
				gameObject2.SetActive(false);
				gameObject3.SetActive(false);
				gameObject4.SetActive(true);
				gameObject5.SetActive(false);
				gameObject6.SetActive(false);
				gameObject.GetComponent<Text>().text = account.email;
			}
			else
			{
				gameObject.SetActive(false);
				gameObject2.SetActive(true);
				gameObject3.SetActive(false);
				gameObject4.SetActive(false);
				gameObject5.SetActive(false);
				gameObject6.SetActive(true);
				gameObject7.SetActive(true);
				gameObject8.SetActive(false);
			}
			if (gameObject7.activeSelf)
			{
				gameObject7.GetComponent<Button>().interactable = !Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasSendVerifyEmailApply;
			}
			gameObject = base.view.transform.Find("AccountPanel/SecurityState/Mobile/Content").gameObject;
			gameObject2 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/ContentWait").gameObject;
			gameObject3 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/ContentNo").gameObject;
			gameObject4 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/MarkYes").gameObject;
			gameObject5 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/MarkNo").gameObject;
			gameObject6 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/MarkCautious").gameObject;
			gameObject8 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/BindingBtn").gameObject;
			gameObject2.SetActive(false);
			active2 = Singleton<AccountManager>.Instance.manager.IsAccountBind() && string.IsNullOrEmpty(account.mobile);
			gameObject8.SetActive(active2);
			if (string.IsNullOrEmpty(account.mobile))
			{
				gameObject.SetActive(false);
				gameObject3.SetActive(true);
				gameObject4.SetActive(false);
				gameObject5.SetActive(true);
				gameObject6.SetActive(false);
			}
			else
			{
				gameObject.SetActive(true);
				gameObject3.SetActive(false);
				gameObject4.SetActive(true);
				gameObject5.SetActive(false);
				gameObject6.SetActive(false);
				gameObject.GetComponent<Text>().text = account.mobile;
			}
			gameObject = base.view.transform.Find("AccountPanel/SecurityState/Identity/Content").gameObject;
			gameObject2 = base.view.transform.Find("AccountPanel/SecurityState/Identity/ContentWait").gameObject;
			gameObject3 = base.view.transform.Find("AccountPanel/SecurityState/Identity/ContentNo").gameObject;
			gameObject4 = base.view.transform.Find("AccountPanel/SecurityState/Identity/MarkYes").gameObject;
			gameObject5 = base.view.transform.Find("AccountPanel/SecurityState/Identity/MarkNo").gameObject;
			gameObject6 = base.view.transform.Find("AccountPanel/SecurityState/Identity/MarkCautious").gameObject;
			gameObject8 = base.view.transform.Find("AccountPanel/SecurityState/Identity/BindingBtn").gameObject;
			gameObject2.SetActive(false);
			gameObject6.SetActive(false);
			active2 = Singleton<AccountManager>.Instance.manager.IsAccountBind() && !account.isRealNameVerify;
			gameObject8.SetActive(active2);
			if (!account.isRealNameVerify)
			{
				gameObject.SetActive(false);
				gameObject3.SetActive(true);
				gameObject4.SetActive(false);
				gameObject5.SetActive(true);
			}
			else
			{
				gameObject.SetActive(true);
				gameObject3.SetActive(false);
				gameObject4.SetActive(true);
				gameObject5.SetActive(false);
			}
		}

		private void SetupTabs()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string searchKey = ((!string.IsNullOrEmpty(showingTabKey)) ? showingTabKey : _currentTab);
			_tabManager.Clear();
			GameObject gameObject = null;
			Button button = null;
			gameObject = base.view.transform.Find("ProfilePanel").gameObject;
			button = base.view.transform.Find("TabButtons/PlayerButton").GetComponent<Button>();
			_tabManager.SetTab("Player", button, gameObject);
			gameObject = base.view.transform.Find("AccountPanel").gameObject;
			button = base.view.transform.Find("TabButtons/AccountButton").GetComponent<Button>();
			_tabManager.SetTab("Account", button, gameObject);
			_tabManager.ShowTab(searchKey);
		}

		private void SetupCollection()
		{
			Text component = base.view.transform.Find("EntryButtons/ItempediaButton/Progress/Current").GetComponent<Text>();
			Text component2 = base.view.transform.Find("EntryButtons/ItempediaButton/Progress/Max").GetComponent<Text>();
			Text component3 = base.view.transform.Find("EntryButtons/AchievementButton/Progress/Current").GetComponent<Text>();
			Text component4 = base.view.transform.Find("EntryButtons/AchievementButton/Progress/Max").GetComponent<Text>();
			Text component5 = base.view.transform.Find("EntryButtons/CGReplayButton/Progress/Current").GetComponent<Text>();
			Text component6 = base.view.transform.Find("EntryButtons/CGReplayButton/Progress/Max").GetComponent<Text>();
			component.text = Singleton<ItempediaModule>.Instance.GetUnlockCountTotal().ToString();
			component2.text = Singleton<ItempediaModule>.Instance.GetItempediaTotalCount().ToString();
			List<MissionDataItem> achievements = Singleton<MissionModule>.Instance.GetAchievements();
			List<MissionDataItem> list = achievements.FindAll((MissionDataItem x) => (int)x.status == 5 || (int)x.status == 3);
			component3.text = list.Count.ToString();
			List<int> finishedCGIDList = Singleton<CGModule>.Instance.GetFinishedCGIDList();
			List<int> allCGIDList = Singleton<CGModule>.Instance.GetAllCGIDList();
			component5.text = finishedCGIDList.Count.ToString();
			component6.text = allCGIDList.Count.ToString();
			List<LinearMissionData> list2 = LinearMissionDataReader.GetItemList().FindAll(delegate(LinearMissionData x)
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Invalid comparison between Unknown and I4
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Invalid comparison between Unknown and I4
				MissionData missionDataByKey = MissionDataReader.GetMissionDataByKey(x.MissionID);
				MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem(x.MissionID);
				bool flag = x.IsAchievement == 1;
				bool flag2 = missionDataByKey != null && missionDataByKey.NoDisplay == 0;
				bool flag3 = missionDataItem != null && ((int)missionDataItem.status == 3 || (int)missionDataItem.status == 5);
				return flag && (flag2 || flag3);
			});
			component4.text = list2.Count.ToString();
		}

		private void SetPopupVisible(bool visible)
		{
			Transform transform = base.view.transform.Find("EntryButtons/AchievementButton/PopUp");
			if (transform != null)
			{
				transform.gameObject.SetActive(visible);
			}
		}

		private bool PopupNoticeAcitve()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in Singleton<MissionModule>.Instance.GetMissionDict().Values)
			{
				LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey(value.id);
				if (linearMissionDataByKey != null && linearMissionDataByKey.IsAchievement == 1 && (int)value.status == 3)
				{
					return true;
				}
			}
			return false;
		}

		private bool OnGetRedeemCodeInfoRsp(GetRedeemCodeInfoRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new RedeemDialogContext(_redeemCode, RedeemDialogContext.RedeemStatus.ShowInfo, rsp));
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new RedeemDialogContext(_redeemCode, RedeemDialogContext.RedeemStatus.Error, rsp, LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)));
			}
			return false;
		}

		public void OnChangeDescBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new ChangeSelfDescDialogContext());
		}

		public void OnChangeNickNameBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new ChangeNicknameDialogContext());
		}

		public void Close()
		{
			Destroy();
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.interactable = !active;
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Label").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.transform.Find("Icon").GetComponent<Image>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			go.SetActive(active);
		}

		public void OnPlayerBtnClick()
		{
			_currentTab = "Player";
			_tabManager.ShowTab(_currentTab);
		}

		public void OnAccountBtnClick()
		{
			_currentTab = "Account";
			_tabManager.ShowTab(_currentTab);
		}

		public void OnMailVerifyBtnClick()
		{
			Singleton<AccountManager>.Instance.manager.VerifyEmailApply();
		}

		public void OnMailBindingBtnClick()
		{
			string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
			string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				string url = string.Format(theOriginalAccountManager.ORIGINAL_BIND_EMAIL_URL + "?uid={0}&token={1}", accountUid, accountToken);
				WebViewGeneralLogic.LoadUrl(url);
			}
		}

		public void OnMobileBindingBtnClick()
		{
			string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
			string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				string url = string.Format(theOriginalAccountManager.ORIGINAL_BIND_MOBILE_URL + "?uid={0}&token={1}", accountUid, accountToken);
				WebViewGeneralLogic.LoadUrl(url);
			}
		}

		public void OnIdentityBindingBtnClick()
		{
			string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
			string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				string url = string.Format(theOriginalAccountManager.ORIGINAL_BIND_IDENTITY_URL + "?uid={0}&token={1}", accountUid, accountToken);
				WebViewGeneralLogic.LoadUrl(url);
			}
		}

		public void OnLogoutBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Title_Logout"),
				desc = LocalizationGeneralLogic.GetText("Menu_Desc_Logout"),
				buttonCallBack = delegate(bool confirmed)
				{
					if (confirmed)
					{
						Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext());
						Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ClearLastLoginUser();
						Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
						GeneralLogicManager.RestartGame();
					}
				}
			});
		}

		public void OnAccountBindingBtnClick()
		{
			Singleton<AccountManager>.Instance.manager.BindUI();
		}

		public void OnItempediaBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ItempediaPageContext());
		}

		public void OnAchieveBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new AchieveOverviewPageContext());
		}

		public void OnCGReplayBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new CGReplayPageContext());
		}

		public void OnGetRewardBtnClick()
		{
			_redeemCode = base.view.transform.Find("AccountPanel/Award/InputField/Text").GetComponent<Text>().text;
			if (_redeemCode.Length != 10)
			{
				string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput((object)(Retcode)2);
				Singleton<MainUIManager>.Instance.ShowDialog(new RedeemDialogContext(_redeemCode, RedeemDialogContext.RedeemStatus.Error, null, networkErrCodeOutput));
			}
			else
			{
				Singleton<NetworkManager>.Instance.RequestGetRedeemCodeInfo(_redeemCode);
				Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(212));
			}
		}
	}
}
