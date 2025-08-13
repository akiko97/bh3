using System.Collections.Generic;
using System.Linq;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;
using Retcode = proto.StageEndRsp.Retcode;

namespace MoleMole
{
	public class FriendOverviewPageContext : BasePageContext
	{
		public enum FriendTab
		{
			FriendListTab = 0,
			AddFriendTab = 1,
			RequestListTab = 2,
			InviteCodeTab = 3
		}

		public static readonly string[] TAB_KEY = new string[4] { "FriendListTab", "AddFriendTab", "RequestListTab", "InviteCodeTab" };

		private TabManager _tabManager;

		private string _defaultTabKey;

		private InviteTab _currentInviteTab;

		private Dictionary<string, List<FriendBriefDataItem>> _tabItemList;

		private int _playerUidToShow;

		private bool _shouldMarkAllFriendsAsOld;

		private GetInviteeFriendRsp _inviteeInfo;

		private GetInviteFriendRsp _inviterInfo;

		private Dictionary<GameObject, MonoGridScroller> _scrollerDict;

		private Dictionary<GameObject, MonoScrollerFadeManager> _fadeManagerDict;

		private Dictionary<GameObject, Dictionary<int, RectTransform>> _itemBeforeDict;

		public FriendOverviewPageContext(string tabKey = "FriendListTab", InviteTab inviteTab = InviteTab.InviteeTab)
		{
			config = new ContextPattern
			{
				contextName = "FriendOverviewPageContext",
				viewPrefabPath = "UI/Menus/Page/Friend/FriendOverviewPage"
			};
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
			_defaultTabKey = tabKey;
			_currentInviteTab = inviteTab;
			_tabItemList = new Dictionary<string, List<FriendBriefDataItem>>();
			_playerUidToShow = -1;
			_shouldMarkAllFriendsAsOld = false;
			_currentInviteTab = InviteTab.InviteeTab;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.SetFriendSortType)
			{
				return OnSetSortType((FriendModule.FriendSortType)(int)ntf.body);
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 65:
			case 80:
				SetupFriendListTab();
				break;
			case 77:
				OnGetRecommandListRsp(pkt.getData<GetRecommendFriendListRsp>());
				PlayCurrentTabAnim();
				break;
			case 71:
				SetupRequestListTab();
				PlayCurrentTabAnim();
				break;
			case 73:
				OnPlayerDetailRsp(pkt.getData<GetPlayerDetailDataRsp>());
				break;
			case 67:
				OnAddFriendRsp(pkt.getData<AddFriendRsp>());
				break;
			case 69:
				OnDelFriendRsp(pkt.getData<DelFriendRsp>());
				break;
			case 226:
				OnGetInviteeFriendRsp(pkt.getData<GetInviteeFriendRsp>());
				break;
			case 224:
				OnGetInviteFriendRsp(pkt.getData<GetInviteFriendRsp>());
				break;
			case 228:
				OnGetAcceptFriendInviteRsp(pkt.getData<AcceptFriendInviteRsp>());
				break;
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), OnFriendListTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), OnAddFriendTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), OnRequestListTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_4").GetComponent<Button>(), OnInviteCodeBtnClick);
			BindViewCallback(base.view.transform.Find("InviteCodeTab/Tab/Tab_1").GetComponent<Button>(), OnInviteeTabBtnClick);
			BindViewCallback(base.view.transform.Find("InviteCodeTab/Tab/Tab_2").GetComponent<Button>(), OnInviterTabBtnClick);
			BindViewCallback(base.view.transform.Find("FriendListTab/SortBtn").GetComponent<Button>(), OnSortBtnClick);
			BindViewCallback(base.view.transform.Find("FriendListTab/SortPanel/BG").GetComponent<Button>(), OnSortBGClick);
			BindViewCallback(base.view.transform.Find("AddFriendTab/RefreshBtn").GetComponent<Button>(), OnAddRefreshBtnClick);
			BindViewCallback(base.view.transform.Find("AddFriendTab/SearchBtn").GetComponent<Button>(), OnSearchBtnClick);
			BindViewCallback(base.view.transform.Find("InviteCodeTab/MyInvitationCode/InviteCode/BtnCopy").GetComponent<Button>(), OnMyInviteCopyBtnClick);
			BindViewCallback(base.view.transform.Find("InviteCodeTab/InputInvitationCode/InviteCode/BtnOk").GetComponent<Button>(), OnAcceptInviteBtnClick);
		}

		protected override bool SetupView()
		{
			InitScroller();
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = ((!string.IsNullOrEmpty(showingTabKey)) ? showingTabKey : _defaultTabKey);
			SetupFriendListTab();
			SetupAddFriendTab();
			SetupRequestListTab();
			SetupInviteCodeTab();
			_tabManager.ShowTab(text);
			if (text == "FriendListTab" && !_shouldMarkAllFriendsAsOld)
			{
				_shouldMarkAllFriendsAsOld = true;
			}
			return false;
		}

		public override void BackPage()
		{
			if (_shouldMarkAllFriendsAsOld)
			{
				Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
			}
			base.BackPage();
		}

		public override void Destroy()
		{
			if (_shouldMarkAllFriendsAsOld)
			{
				Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
			}
			base.Destroy();
		}

		public override void OnLandedFromBackPage()
		{
			SetupView();
			base.OnLandedFromBackPage();
		}

		private void OnFriendListTabBtnClick()
		{
			if (!_shouldMarkAllFriendsAsOld)
			{
				_shouldMarkAllFriendsAsOld = true;
			}
			base.view.transform.Find("TabBtns/TabBtn_1/PopUp").gameObject.SetActive(false);
			_tabManager.ShowTab("FriendListTab");
		}

		private void OnAddFriendTabBtnClick()
		{
			if (_shouldMarkAllFriendsAsOld)
			{
				Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
			}
			_tabManager.ShowTab("AddFriendTab");
		}

		private void OnRequestListTabBtnClick()
		{
			if (_shouldMarkAllFriendsAsOld)
			{
				Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
			}
			Singleton<FriendModule>.Instance.MarkAllRequestsAsOld();
			base.view.transform.Find("TabBtns/TabBtn_3/PopUp").gameObject.SetActive(false);
			_tabManager.ShowTab("RequestListTab");
		}

		private void OnInviteCodeBtnClick()
		{
			if (_shouldMarkAllFriendsAsOld)
			{
				Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
			}
			_tabManager.ShowTab("InviteCodeTab");
		}

		private void OnInviteeTabBtnClick()
		{
			_currentInviteTab = InviteTab.InviteeTab;
			SetupInviteeTabUI();
			Singleton<NetworkManager>.Instance.RequestGetInviteeFriend();
		}

		private void OnInviterTabBtnClick()
		{
			_currentInviteTab = InviteTab.InviteeTab;
			SetupInviterTabUI();
			Singleton<NetworkManager>.Instance.RequestGetInviteFriend();
		}

		private void OnSortBtnClick()
		{
			SetupSortView(true);
		}

		private void OnSortBGClick()
		{
			SetupSortView(false);
		}

		private void OnAddRefreshBtnClick()
		{
			Singleton<NetworkManager>.Instance.RequestRecommandFriendList();
		}

		private void OnSearchBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new SearchFriendDialogContext());
		}

		private void OnMyInviteCopyBtnClick()
		{
			string text = base.view.transform.Find("InviteCodeTab/MyInvitationCode/InviteCode/InputField/Text").GetComponent<Text>().text;
			text = text.Trim();
			ClipboardManager.CopyToClipboard(text);
			ShowMyInviteCodeCopySuccessHint();
		}

		private void OnAcceptInviteBtnClick()
		{
			string text = base.view.transform.Find("InviteCodeTab/InputInvitationCode/InviteCode/InputField").GetComponent<InputField>().text.Trim();
			if (!string.IsNullOrEmpty(text))
			{
				text = text.ToUpper();
				Singleton<NetworkManager>.Instance.RequestGetAcceptFriendInvite(text);
			}
		}

		private bool OnSetSortType(FriendModule.FriendSortType sortType)
		{
			SetupSortView(true);
			SetupScrollView(_tabManager.GetShowingTabKey(), _tabManager.GetShowingTabContent());
			PlayCurrentTabAnim();
			return false;
		}

		private bool OnAddFriendRsp(AddFriendRsp rsp)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected I4, but got Unknown
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected I4, but got Unknown
			int target_uid = (int)rsp.target_uid;
			string desc = string.Empty;
			Retcode retcode = (Retcode)rsp.retcode;
			switch ((int)retcode)
			{
			case 0:
			{
				AddFriendAction action = rsp.action;
				switch ((int)action - 1)
				{
				case 1:
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_AgreeFriend", Singleton<FriendModule>.Instance.TryGetPlayerNickName(target_uid));
					break;
				case 2:
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_RejectFriend", Singleton<FriendModule>.Instance.TryGetPlayerNickName(target_uid));
					break;
				case 0:
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_RequestAddFriend", Singleton<FriendModule>.Instance.TryGetPlayerNickName(target_uid));
					break;
				}
				break;
			}
			case 3:
				desc = LocalizationGeneralLogic.GetText("Err_FriendFull");
				break;
			case 4:
				desc = LocalizationGeneralLogic.GetText("Err_TargetFriendFull");
				break;
			case 6:
				desc = LocalizationGeneralLogic.GetText("Err_IsFriend");
				break;
			case 5:
				desc = LocalizationGeneralLogic.GetText("Err_IsSelf");
				break;
			case 1:
				desc = LocalizationGeneralLogic.GetText("Err_FailToAddFriend");
				break;
			case 7:
				desc = LocalizationGeneralLogic.GetText("Err_AskTooOften");
				break;
			case 8:
				desc = LocalizationGeneralLogic.GetText("Err_TargetAskListFull");
				break;
			case 9:
				desc = LocalizationGeneralLogic.GetText("Err_TargetInAskList");
				break;
			default:
				desc = LocalizationGeneralLogic.GetText("Err_FailToAddFriend");
				break;
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(desc));
			return false;
		}

		private bool OnDelFriendRsp(DelFriendRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				string text = LocalizationGeneralLogic.GetText("Menu_Desc_DeleteFriend", Singleton<FriendModule>.Instance.TryGetPlayerNickName((int)rsp.target_uid));
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(text));
				SetupFriendListTab();
			}
			return false;
		}

		private bool OnGetInviteeFriendRsp(GetInviteeFriendRsp rsp)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (!MiscData.Config.BasicConfig.IsInviteFeatureEnable)
			{
				return false;
			}
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
			{
				return false;
			}
			if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
			{
				return false;
			}
			if ((int)rsp.retcode == 0)
			{
				_inviteeInfo = rsp;
				SetupInviteeTabUI();
				Transform transform = base.view.transform.Find("InviteCodeTab/InputInvitationCode");
				transform.gameObject.SetActive(true);
				base.view.transform.Find("InviteCodeTab/MyInvitationCode").gameObject.SetActive(false);
				base.view.transform.Find("InviteCodeTab/Tab/Tab_1").gameObject.SetActive(true);
				InputField component = transform.Find("InviteCode/InputField").GetComponent<InputField>();
				int maxLevelToAcceptInvite = Singleton<PlayerModule>.Instance.playerData.maxLevelToAcceptInvite;
				component.transform.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_InvitationCode_effective", maxLevelToAcceptInvite);
				if (rsp.invitee_codeSpecified)
				{
					component.text = rsp.invitee_code;
					component.interactable = false;
					transform.Find("InviteCode/BtnOk").gameObject.SetActive(false);
					transform.Find("InviteCode/Used").gameObject.SetActive(true);
				}
				else if (Singleton<PlayerModule>.Instance.playerData.teamLevel >= Singleton<PlayerModule>.Instance.playerData.maxLevelToAcceptInvite)
				{
					base.view.transform.Find("InviteCodeTab/Tab/Tab_1").gameObject.SetActive(false);
					_currentInviteTab = InviteTab.InviterTab;
					transform.gameObject.SetActive(false);
				}
				else
				{
					component.interactable = true;
					transform.Find("InviteCode/BtnOk").gameObject.SetActive(true);
					transform.Find("InviteCode/Used").gameObject.SetActive(false);
				}
				transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(OnInviteeRewardScrollChange, rsp.invitee_reward_list.Count, new Vector2(0f, 1f));
			}
			else
			{
				base.view.transform.Find("InviteCodeTab/Tab/Tab_1").gameObject.SetActive(false);
				_currentInviteTab = InviteTab.InviterTab;
				base.view.transform.Find("InviteCodeTab/InputInvitationCode").gameObject.SetActive(false);
			}
			return false;
		}

		private bool OnGetInviteFriendRsp(GetInviteFriendRsp rsp)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (!MiscData.Config.BasicConfig.IsInviteFeatureEnable)
			{
				return false;
			}
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
			{
				return false;
			}
			if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
			{
				return false;
			}
			if ((int)rsp.retcode == 0)
			{
				_inviterInfo = rsp;
				SetupInviterTabUI();
				base.view.transform.Find("InviteCodeTab/InputInvitationCode").gameObject.SetActive(false);
				base.view.transform.Find("InviteCodeTab/MyInvitationCode").gameObject.SetActive(true);
				base.view.transform.Find("InviteCodeTab/Tab/Tab_2").gameObject.SetActive(true);
				Transform transform = base.view.transform.Find("InviteCodeTab/MyInvitationCode");
				transform.Find("InviteCode/InputField/Text").GetComponent<Text>().text = rsp.my_invite_code.ToString();
				transform.Find("InviteCode/HaveInvited/Num").GetComponent<Text>().text = rsp.has_invite_num.ToString();
				transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(OnInviterRewardScrollChange, rsp.my_invite_reward_list.Count, new Vector2(0f, 1f));
			}
			else
			{
				base.view.transform.Find("InviteCodeTab/Tab/Tab_2").gameObject.SetActive(false);
				_currentInviteTab = InviteTab.InviteeTab;
				base.view.transform.Find("InviteCodeTab/MyInvitationCode").gameObject.SetActive(false);
			}
			return false;
		}

		private bool OnGetAcceptFriendInviteRsp(AcceptFriendInviteRsp rsp)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			if (!MiscData.Config.BasicConfig.IsInviteFeatureEnable)
			{
				return false;
			}
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
			{
				return false;
			}
			if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
			{
				return false;
			}
			if ((int)rsp.retcode == 0)
			{
				Singleton<NetworkManager>.Instance.RequestGetInviteeFriend();
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_InviteInputSuccess")));
			}
			else
			{
				GeneralDialogContext generalDialogContext = new GeneralDialogContext();
				generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
				generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
				generalDialogContext.desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				generalDialogContext.notDestroyAfterTouchBG = true;
				GeneralDialogContext dialogContext = generalDialogContext;
				Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
			}
			return false;
		}

		private bool OnGetRecommandListRsp(GetRecommendFriendListRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected I4, but got Unknown
			Retcode retcode = (Retcode)rsp.retcode;
			switch ((int)retcode)
			{
			case 0:
				SetupAddFriendTab();
				break;
			case 2:
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_RefreshToOften")));
				break;
			}
			return false;
		}

		private bool OnPlayerDetailRsp(GetPlayerDetailDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && _playerUidToShow == (int)rsp.detail.uid)
			{
				_playerUidToShow = -1;
				FriendDetailDataItem detailData = new FriendDetailDataItem(rsp.detail);
				return ShowFriendDetailInfo(detailData);
			}
			return false;
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.transform.Find("Image").GetComponent<Image>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			go.SetActive(active);
			if (active && _fadeManagerDict.ContainsKey(go))
			{
				_fadeManagerDict[go].Init(_scrollerDict[go].GetItemDict(), _itemBeforeDict[go], IsFriendDataEqual);
				_fadeManagerDict[go].Play();
				_itemBeforeDict[go] = null;
			}
		}

		private void OnScrollerChange(string key, List<FriendBriefDataItem> list, Transform trans, int index)
		{
			FriendBriefDataItem friendBriefData = list[index];
			trans.GetComponent<MonoFriendInfo>().SetupView(friendBriefData, key.ToEnum(FriendTab.FriendListTab), OnRequestBtnClick, OnAcceptBtnClick, OnRejectBtnClick, OnDetailBtnClick);
		}

		private void SetupTab(string key, Button tabBtn, GameObject tabGo, List<FriendBriefDataItem> list)
		{
			if (_tabItemList.ContainsKey(key))
			{
				_tabItemList[key] = list;
			}
			else
			{
				_tabItemList.Add(key, list);
			}
			SetupScrollView(key, tabGo);
			_tabManager.SetTab(key, tabBtn, tabGo);
		}

		private void SetupInviteCodeTab(string key, Button tabBtn, GameObject tabGo, string inviteCode)
		{
			_tabManager.SetTab(key, tabBtn, tabGo);
			bool active = Singleton<PlayerModule>.Instance.playerData.teamLevel < 10;
			base.view.transform.Find("InviteCodeTab/Tab/Tab_2").gameObject.SetActive(active);
		}

		private void SetupScrollView(string key, GameObject tabGo)
		{
			FriendModule.FriendSortType key2 = Singleton<FriendModule>.Instance.sortTypeMap[key];
			_tabItemList[key].Sort(Singleton<FriendModule>.Instance.sortComparisionMap[key2]);
			tabGo.transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(delegate(Transform trans, int index)
			{
				OnScrollerChange(key, _tabItemList[key], trans, index);
			}, _tabItemList[key].Count);
		}

		private void SetupFriendListTab()
		{
			GameObject gameObject = base.view.transform.Find("RequestListTab").gameObject;
			_itemBeforeDict[gameObject] = _scrollerDict[gameObject].GetItemDict().ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
			list.AddRange(Singleton<FriendModule>.Instance.friendsList);
			base.view.transform.Find("TabBtns/TabBtn_1/PopUp").gameObject.SetActive(Singleton<FriendModule>.Instance.HasNewFriend());
			SetupTab("FriendListTab", base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), base.view.transform.Find("FriendListTab").gameObject, list);
			SetupFriendNumView();
			SetupSortView(false);
		}

		private void SetupAddFriendTab()
		{
			GameObject gameObject = base.view.transform.Find("AddFriendTab").gameObject;
			_itemBeforeDict[gameObject] = _scrollerDict[gameObject].GetItemDict().ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
			list.AddRange(Singleton<FriendModule>.Instance.GetRecommandFriendList());
			SetupTab("AddFriendTab", base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), base.view.transform.Find("AddFriendTab").gameObject, list);
		}

		private void SetupRequestListTab()
		{
			GameObject gameObject = base.view.transform.Find("RequestListTab").gameObject;
			_itemBeforeDict[gameObject] = _scrollerDict[gameObject].GetItemDict().ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
			list.AddRange(Singleton<FriendModule>.Instance.askingList);
			base.view.transform.Find("TabBtns/TabBtn_3/PopUp").gameObject.SetActive(Singleton<FriendModule>.Instance.HasNewRequest());
			SetupTab("RequestListTab", base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), base.view.transform.Find("RequestListTab").gameObject, list);
		}

		private void SetupInviteCodeTab()
		{
			bool isInviteFeatureEnable = MiscData.Config.BasicConfig.IsInviteFeatureEnable;
			bool isReview = Singleton<NetworkManager>.Instance.DispatchSeverData.isReview;
			bool flag = Singleton<AccountManager>.Instance.manager.IsAccountBind();
			bool active = isInviteFeatureEnable && !isReview && flag;
			base.view.transform.Find("InviteCodeTab").gameObject.SetActive(active);
			base.view.transform.Find("TabBtns/TabBtn_4").gameObject.SetActive(active);
			if (!isReview && flag)
			{
				base.view.transform.Find("InviteCodeTab/Tab/Tab_1").gameObject.SetActive(false);
				base.view.transform.Find("InviteCodeTab/Tab/Tab_2").gameObject.SetActive(false);
				base.view.transform.Find("InviteCodeTab/InputInvitationCode").gameObject.SetActive(false);
				base.view.transform.Find("InviteCodeTab/MyInvitationCode").gameObject.SetActive(false);
				SetupInviteCodeTab("InviteCodeTab", base.view.transform.Find("TabBtns/TabBtn_4").GetComponent<Button>(), base.view.transform.Find("InviteCodeTab").gameObject, "some string as invite code");
				Singleton<NetworkManager>.Instance.RequestGetInviteeFriend();
				Singleton<NetworkManager>.Instance.RequestGetInviteFriend();
			}
		}

		private void SetupFriendNumView()
		{
			int count = Singleton<FriendModule>.Instance.friendsList.Count;
			int maxFriendFinal = Singleton<PlayerModule>.Instance.playerData.maxFriendFinal;
			base.view.transform.Find("FriendListTab/FriendNum/CurNum").GetComponent<Text>().text = count.ToString();
			Text component = base.view.transform.Find("FriendListTab/FriendNum/MaxNum").GetComponent<Text>();
			component.text = maxFriendFinal.ToString();
			component.color = ((count <= maxFriendFinal) ? Color.white : Color.red);
		}

		private void SetupSortView(bool sortActive)
		{
			base.view.transform.Find("FriendListTab/SortPanel").gameObject.SetActive(sortActive);
			base.view.transform.Find("FriendListTab/SortBtn").GetComponent<Button>().interactable = !sortActive;
			if (!sortActive)
			{
				return;
			}
			Transform transform = base.view.transform.Find("FriendListTab/SortPanel/Content");
			MonoFriendSortButton[] componentsInChildren = transform.GetComponentsInChildren<MonoFriendSortButton>();
			MonoFriendSortButton[] array = componentsInChildren;
			foreach (MonoFriendSortButton monoFriendSortButton in array)
			{
				if (monoFriendSortButton.gameObject.activeSelf)
				{
					monoFriendSortButton.SetupView(_tabManager.GetShowingTabKey());
				}
			}
		}

		private void OnRequestBtnClick(FriendBriefDataItem friendBriefData)
		{
			Singleton<FriendModule>.Instance.RecordRequestAddFriend(friendBriefData.uid);
			Singleton<FriendModule>.Instance.RemoveFriendInfo(FriendTab.AddFriendTab, friendBriefData.uid);
			SetupAddFriendTab();
			PlayCurrentTabAnim();
			Singleton<NetworkManager>.Instance.RequestAddFriend(friendBriefData.uid);
		}

		private void OnAcceptBtnClick(FriendBriefDataItem friendBriefData)
		{
			Singleton<FriendModule>.Instance.RecordRequestAddFriend(friendBriefData.uid);
			Singleton<FriendModule>.Instance.RemoveFriendInfo(FriendTab.RequestListTab, friendBriefData.uid);
			SetupRequestListTab();
			PlayCurrentTabAnim();
			Singleton<NetworkManager>.Instance.RequestAgreeFriend(friendBriefData.uid);
		}

		private void OnRejectBtnClick(FriendBriefDataItem friendBriefData)
		{
			Singleton<FriendModule>.Instance.RemoveFriendInfo(FriendTab.RequestListTab, friendBriefData.uid);
			SetupRequestListTab();
			PlayCurrentTabAnim();
			Singleton<NetworkManager>.Instance.RequestRejectFriend(friendBriefData.uid);
		}

		private void OnDetailBtnClick(FriendBriefDataItem friendBriefData)
		{
			Singleton<FriendModule>.Instance.MarkFriendAsOld(friendBriefData.uid);
			FriendDetailDataItem friendDetailDataItem = Singleton<FriendModule>.Instance.TryGetFriendDetailData(friendBriefData.uid);
			if (friendDetailDataItem == null)
			{
				_playerUidToShow = friendBriefData.uid;
				Singleton<NetworkManager>.Instance.RequestFriendDetailInfo(friendBriefData.uid);
			}
			else
			{
				ShowFriendDetailInfo(friendDetailDataItem);
			}
		}

		private bool ShowFriendDetailInfo(FriendDetailDataItem detailData)
		{
			RemoteAvatarDetailPageContext context = new RemoteAvatarDetailPageContext(detailData);
			Singleton<MainUIManager>.Instance.ShowPage(context);
			return false;
		}

		private void InitScroller()
		{
			_scrollerDict = new Dictionary<GameObject, MonoGridScroller>();
			_scrollerDict[base.view.transform.Find("FriendListTab").gameObject] = base.view.transform.Find("FriendListTab/ScrollView").GetComponent<MonoGridScroller>();
			_scrollerDict[base.view.transform.Find("AddFriendTab").gameObject] = base.view.transform.Find("AddFriendTab/ScrollView").GetComponent<MonoGridScroller>();
			_scrollerDict[base.view.transform.Find("RequestListTab").gameObject] = base.view.transform.Find("RequestListTab/ScrollView").GetComponent<MonoGridScroller>();
			_fadeManagerDict = new Dictionary<GameObject, MonoScrollerFadeManager>();
			_fadeManagerDict[base.view.transform.Find("FriendListTab").gameObject] = base.view.transform.Find("FriendListTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_fadeManagerDict[base.view.transform.Find("AddFriendTab").gameObject] = base.view.transform.Find("AddFriendTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_fadeManagerDict[base.view.transform.Find("RequestListTab").gameObject] = base.view.transform.Find("RequestListTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_itemBeforeDict = new Dictionary<GameObject, Dictionary<int, RectTransform>>();
			_itemBeforeDict[base.view.transform.Find("FriendListTab").gameObject] = null;
			_itemBeforeDict[base.view.transform.Find("AddFriendTab").gameObject] = null;
			_itemBeforeDict[base.view.transform.Find("RequestListTab").gameObject] = null;
		}

		private void PlayCurrentTabAnim()
		{
			GameObject showingTabContent = _tabManager.GetShowingTabContent();
			if (_scrollerDict.ContainsKey(showingTabContent))
			{
				_fadeManagerDict[showingTabContent].Init(_scrollerDict[showingTabContent].GetItemDict(), _itemBeforeDict[showingTabContent], IsFriendDataEqual);
				_fadeManagerDict[showingTabContent].Play();
				_itemBeforeDict[showingTabContent] = null;
			}
		}

		private bool IsFriendDataEqual(RectTransform friendNew, RectTransform friendOld)
		{
			if (friendNew == null || friendOld == null)
			{
				return false;
			}
			MonoFriendInfo component = friendOld.GetComponent<MonoFriendInfo>();
			MonoFriendInfo component2 = friendNew.GetComponent<MonoFriendInfo>();
			return component2.GetFriendUID() == component.GetFriendUID();
		}

		private void OnInviteeRewardScrollChange(Transform itemTrans, int index)
		{
			InviteeFriendRewardData rewardData = _inviteeInfo.invitee_reward_list[index];
			itemTrans.GetComponent<MonoInviteRewardRow>().SetupView(_inviteeInfo.invitee_codeSpecified, rewardData);
		}

		private void OnInviterRewardScrollChange(Transform itemTrans, int index)
		{
			InviteFriendRewardData rewardData = _inviterInfo.my_invite_reward_list[index];
			itemTrans.GetComponent<MonoInviteRewardRow>().SetupView(rewardData);
		}

		private void SetupInviteeTabUI()
		{
			GameObject gameObject = base.view.transform.Find("InviteCodeTab").gameObject;
			gameObject.transform.Find("Tab/Tab_1").GetComponent<Button>().interactable = false;
			gameObject.transform.Find("Tab/Tab_2").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("Tab/Tab_1/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
			gameObject.transform.Find("Tab/Tab_2/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("InputInvitationCode").gameObject.SetActive(false);
			gameObject.transform.Find("MyInvitationCode").gameObject.SetActive(false);
		}

		private void SetupInviterTabUI()
		{
			GameObject gameObject = base.view.transform.Find("InviteCodeTab").gameObject;
			gameObject.transform.Find("Tab/Tab_1").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("Tab/Tab_2").GetComponent<Button>().interactable = false;
			gameObject.transform.Find("Tab/Tab_1/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("Tab/Tab_2/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
			gameObject.transform.Find("InputInvitationCode").gameObject.SetActive(false);
			gameObject.transform.Find("MyInvitationCode").gameObject.SetActive(false);
		}

		private void ShowMyInviteCodeCopySuccessHint()
		{
			GeneralHintDialogContext dialogContext = new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_InvitationCode_CopySuccess"));
			Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
		}
	}
}
