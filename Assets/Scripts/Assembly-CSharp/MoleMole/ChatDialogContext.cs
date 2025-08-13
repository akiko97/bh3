using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class ChatDialogContext : BaseDialogContext
	{
		public enum Mode
		{
			World = 0,
			Guild = 1,
			Friend = 2
		}

		private const int MAX_LENGTH = 40;

		private const float LONG_INPUT_LENGTH = 946f;

		private const float SHORT_INPUT_LENGTH = 710f;

		private const float FRIEND_LIST_MIN_HEIGHT = 215.3f;

		private const float FRIEND_LIST_MAX_HEIGHT = 351.3f;

		private const float FRIEND_LIST_ROW_HEIGHT = 60f;

		private const int LINE_NUMBER_BETWEEN_HISTORY_MSG = 4;

		private Mode _mode;

		private int _talkingFriendUid;

		private TabManager _tabManager;

		private bool _isFriendListShow;

		private TabManager _friendTabManager;

		public ChatDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "ChatDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/Chat/ChatDialog"
			};
			_tabManager = new TabManager();
			_friendTabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
			_friendTabManager.onSetActive += OnFriendTabSetActive;
			if (false)
			{
				_mode = Mode.Guild;
			}
			else
			{
				_mode = Mode.World;
			}
			_talkingFriendUid = 0;
		}

		public ChatDialogContext(int talkingUid)
		{
			config = new ContextPattern
			{
				contextName = "ChatDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/Chat/ChatDialog"
			};
			_tabManager = new TabManager();
			_friendTabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
			_friendTabManager.onSetActive += OnFriendTabSetActive;
			_mode = Mode.Friend;
			_talkingFriendUid = (Singleton<FriendModule>.Instance.IsMyFriend(talkingUid) ? talkingUid : 0);
			if (_talkingFriendUid != 0)
			{
				Singleton<ChatModule>.Instance.SetFriendMsgRead(_talkingFriendUid);
			}
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/Room/ChangeBtn").GetComponent<Button>(), OnChangeRoomBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/Friend/ChangeBtn").GetComponent<Button>(), OnChangeFriendBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/TabBtns/WorldBtn").GetComponent<Button>(), OnWorldTabBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/TabBtns/GuildBtn").GetComponent<Button>(), OnGuildTabBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/TabBtns/FriendBtn").GetComponent<Button>(), OnFriendTabBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/SendMsgBtn").GetComponent<Button>(), OnSendMsgBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 89:
				return OnEnterWorldChatroomRsp(pkt.getData<EnterWorldChatroomRsp>());
			case 91:
				return OnRecvWorldChatMsgNotify(pkt.getData<RecvWorldChatMsgNotify>());
			case 93:
				return OnRecvFriendChatMsgNotify(pkt.getData<RecvFriendChatMsgNotify>());
			case 65:
				return OnAddFriendRsp(pkt.getData<GetFriendListRsp>());
			case 80:
				return OnDelFriendNotify(pkt.getData<DelFriendNotify>());
			case 21:
				return OnNicknameModifyRsp(pkt.getData<NicknameModifyRsp>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.ChangeTalkingFriend)
			{
				return OnChangeTalkingFriend((int)ntf.body);
			}
			return false;
		}

		protected override bool SetupView()
		{
			InputField component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>();
			component.characterLimit = 0;
			component.GetComponent<InputFieldHelper>().mCharacterlimit = 40;
			base.view.transform.Find("Dialog/Content/Room").gameObject.SetActive(_mode == Mode.World);
			base.view.transform.Find("Dialog/Content/WorldModeBtn").gameObject.SetActive(_mode == Mode.World);
			base.view.transform.Find("Dialog/Content/GuildModeBtn").gameObject.SetActive(_mode == Mode.Guild);
			base.view.transform.Find("Dialog/Content/FriendModeBtn").gameObject.SetActive(_mode == Mode.Friend);
			base.view.transform.Find("Dialog/Content/Friend").gameObject.SetActive(_mode == Mode.Friend);
			base.view.transform.Find("Dialog/TabBtns/GuildBtn").GetComponent<Button>().enabled = false;
			UpdateFriendListOpenState();
			UpdateNewMsgBtnTip();
			SetupChannelView();
			SetupChatList();
			SetupFriendList();
			SetupTabView();
			return false;
		}

		private bool OnEnterWorldChatroomRsp(EnterWorldChatroomRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				SetupView();
			}
			else
			{
				string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput));
			}
			return false;
		}

		private bool OnRecvWorldChatMsgNotify(RecvWorldChatMsgNotify rsp)
		{
			if (_mode == Mode.World && Singleton<ChatModule>.Instance.chatRoomId > 0)
			{
				UpdateChatList(1);
			}
			return false;
		}

		private bool OnRecvFriendChatMsgNotify(RecvFriendChatMsgNotify rsp)
		{
			if (_talkingFriendUid > 0 && rsp.chat_msg.uid == _talkingFriendUid)
			{
				if (_mode == Mode.Friend)
				{
					UpdateChatList(1);
				}
				Singleton<ChatModule>.Instance.SetFriendMsgRead(_talkingFriendUid);
			}
			else
			{
				SetupFriendList();
				UpdateNewMsgBtnTip();
			}
			return false;
		}

		private bool OnAddFriendRsp(GetFriendListRsp rsp)
		{
			SetupView();
			return false;
		}

		private bool OnDelFriendNotify(DelFriendNotify ntf)
		{
			return false;
		}

		private bool OnNicknameModifyRsp(NicknameModifyRsp rsp)
		{
			SetupView();
			return false;
		}

		private bool OnChangeTalkingFriend(int uid)
		{
			_talkingFriendUid = uid;
			SetupView();
			return false;
		}

		private void RefreshMode(Mode mode)
		{
			_mode = mode;
			SetupView();
		}

		private void OnWorldTabBtnClick()
		{
			_tabManager.ShowTab("WorldTab");
			RefreshMode(Mode.World);
		}

		private void OnGuildTabBtnClick()
		{
			_tabManager.ShowTab("GuildTab");
			RefreshMode(Mode.Guild);
		}

		private void OnFriendTabBtnClick()
		{
			_tabManager.ShowTab("FriendTab");
			RefreshMode(Mode.Friend);
		}

		private void OnChangeRoomBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new ChangeChatRoomDialogContext());
		}

		private void OnChangeFriendBtnClick()
		{
			_isFriendListShow = !_isFriendListShow;
			UpdateFriendListOpenState();
			if (_mode == Mode.Friend)
			{
				if (_isFriendListShow)
				{
					base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<MonoFriendChatList>().OpenFriendChatList();
				}
				else
				{
					base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<MonoFriendChatList>().CloseFriendChatList();
				}
			}
		}

		private void OnSendMsgBtnClick()
		{
			string text = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text;
			text = text.Trim();
			int length = Mathf.Min(40, text.Length);
			text = text.Substring(0, length);
			if (string.IsNullOrEmpty(text) || (_mode == Mode.Friend && _talkingFriendUid <= 0))
			{
				return;
			}
			ChatMsgDataItem chatMsgDataItem = GenerateMsgSendByMe(text);
			if (_mode == Mode.World)
			{
				if (Singleton<ChatModule>.Instance.IsWorldChatAllowed(chatMsgDataItem))
				{
					Singleton<ChatModule>.Instance.worldChatMsgList.Add(chatMsgDataItem);
					Singleton<ChatModule>.Instance.lastWorldChatTime = chatMsgDataItem.time;
					Singleton<NetworkManager>.Instance.NotifySendWorldChatMsg(chatMsgDataItem.msg);
				}
				else
				{
					Singleton<ChatModule>.Instance.worldChatMsgList.Add(ChatMsgDataItem.TALK_TOO_FAST_MSG);
				}
			}
			else if (_mode == Mode.Friend)
			{
				if (Singleton<FriendModule>.Instance.IsMyFriend(_talkingFriendUid))
				{
					Singleton<ChatModule>.Instance.AddFriendChatMsgByMySelf(chatMsgDataItem, _talkingFriendUid);
					Singleton<NetworkManager>.Instance.NotifySendFriendChatMsg(_talkingFriendUid, chatMsgDataItem.msg);
					SetupFriendList();
				}
				else
				{
					chatMsgDataItem.msg = LocalizationGeneralLogic.GetText("Chat_Content_NotFriend");
					Singleton<ChatModule>.Instance.AddFriendChatMsgByMySelf(chatMsgDataItem, _talkingFriendUid, false);
				}
			}
			else if (_mode == Mode.Guild)
			{
				Singleton<ChatModule>.Instance.guildChatMsgList.Add(chatMsgDataItem);
				Singleton<NetworkManager>.Instance.NotifySendGuildChatMsg(chatMsgDataItem.msg);
			}
			UpdateChatList(1);
			ClearInputFieldText();
		}

		private void ClearInputFieldText()
		{
			InputField component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>();
			component.textComponent.text = string.Empty;
			component.text = string.Empty;
		}

		private void Close()
		{
			Destroy();
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.transform.Find("Image").GetComponent<Image>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			if (!btn.enabled)
			{
				btn.GetComponent<Image>().color = Color.grey;
			}
			ClearInputFieldText();
		}

		private void SetupChannelView()
		{
			if (_mode == Mode.World)
			{
				SetupWorldView();
			}
			if (_mode == Mode.Guild)
			{
				SetupGuildView();
			}
			else if (_mode == Mode.Friend)
			{
				SetupFriendView();
			}
		}

		private void OnFriendTabSetActive(bool active, GameObject go, Button btn)
		{
			MonoFriendChatInfo component = btn.transform.parent.GetComponent<MonoFriendChatInfo>();
			if (active)
			{
				FriendBriefDataItem friendData = component.GetFriendData();
				if (friendData != null)
				{
					Singleton<ChatModule>.Instance.SetFriendMsgRead(friendData.uid);
					component.SetNewMessageTipShow(false);
				}
			}
			component.RefreshNickName();
		}

		private void SetInputLength(float length)
		{
			RectTransform component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(length, component.sizeDelta.y);
		}

		private void SetupWorldView()
		{
			base.view.transform.Find("Dialog/Content/Room/ChangeBtn/Num").GetComponent<Text>().text = Singleton<ChatModule>.Instance.chatRoomId.ToString();
			SetInputLength(946f);
		}

		private void SetupGuildView()
		{
			SetInputLength(946f);
		}

		private void SetupFriendView()
		{
			string text = LocalizationGeneralLogic.GetText("Chat_Content_FriendUnselected");
			base.view.transform.Find("Dialog/Content/Friend/ChangeBtn/Name").GetComponent<Text>().text = ((_talkingFriendUid != 0) ? Singleton<FriendModule>.Instance.TryGetPlayerNickName(_talkingFriendUid) : text);
			SetInputLength(710f);
		}

		private void SetupTabView()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string searchKey = showingTabKey;
			if (string.IsNullOrEmpty(showingTabKey))
			{
				if (_mode == Mode.World)
				{
					searchKey = "WorldTab";
				}
				else if (_mode == Mode.Guild)
				{
					searchKey = "GuildTab";
				}
				else if (_mode == Mode.Friend)
				{
					searchKey = "FriendTab";
				}
			}
			MonoGridScroller component = base.view.transform.Find("Dialog/Content/ChatList/ScrollView").GetComponent<MonoGridScroller>();
			_tabManager.SetTab("WorldTab", base.view.transform.Find("Dialog/TabBtns/WorldBtn").GetComponent<Button>(), component.gameObject);
			_tabManager.SetTab("FriendTab", base.view.transform.Find("Dialog/TabBtns/FriendBtn").GetComponent<Button>(), component.gameObject);
			_tabManager.ShowTab(searchKey);
		}

		private void SetupChatList()
		{
			MonoGridScroller component = base.view.transform.Find("Dialog/Content/ChatList/ScrollView").GetComponent<MonoGridScroller>();
			component.Init(OnScrollChange, GetScrollerCount());
			component.ScrollToEnd();
		}

		private int GetScrollerCount()
		{
			if (GetMsgHistoryIndex() > 0 && GetShowMsgDataList().Count > 0)
			{
				return GetShowMsgDataList().Count + 4;
			}
			return GetShowMsgDataList().Count;
		}

		private void SetupFriendList()
		{
			MonoGridScroller component = base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList/ScrollView").GetComponent<MonoGridScroller>();
			RectTransform component2 = base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<RectTransform>();
			int friendChatCount = Singleton<ChatModule>.Instance.GetFriendChatCount();
			float size = Mathf.Clamp((float)friendChatCount * 60f + 30f, 215.3f, 351.3f);
			component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			_friendTabManager.Clear();
			component.Init(OnFriendScrollChange, friendChatCount);
			component.ScrollToEnd();
		}

		private void OnFriendScrollChange(Transform trans, int index)
		{
			if (_mode == Mode.Friend)
			{
				int targetUid = Singleton<ChatModule>.Instance.GetSortedChatFriendList()[index];
				FriendBriefDataItem friendBriefDataItem = Singleton<FriendModule>.Instance.TryGetFriendBriefData(targetUid);
				trans.Find("FriendBtn/Name").GetComponent<Text>().text = friendBriefDataItem.nickName;
				MonoFriendChatInfo component = trans.GetComponent<MonoFriendChatInfo>();
				bool friendMsgNewState = Singleton<ChatModule>.Instance.GetFriendMsgNewState(friendBriefDataItem.uid);
				bool flag = friendBriefDataItem.uid == _talkingFriendUid;
				bool hasNewMessage = friendMsgNewState && !flag;
				component.SetupView(friendBriefDataItem, hasNewMessage, OnChangeTalkingFriendClick);
				MonoGridScroller component2 = base.view.transform.Find("Dialog/Content/ChatList/ScrollView").GetComponent<MonoGridScroller>();
				Button component3 = trans.Find("FriendBtn").GetComponent<Button>();
				_friendTabManager.SetTab(friendBriefDataItem.uid.ToString(), component3, component2.gameObject);
			}
		}

		private void OnChangeTalkingFriendClick(FriendBriefDataItem data)
		{
			_isFriendListShow = false;
			base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<MonoFriendChatList>().CloseFriendChatList();
			_talkingFriendUid = data.uid;
			_friendTabManager.ShowTab(data.uid.ToString());
			RefreshMode(Mode.Friend);
		}

		private ChatMsgDataItem GetShowChatMsgDataItem(int index, int msgHistoryIndex)
		{
			List<ChatMsgDataItem> showMsgDataList = GetShowMsgDataList();
			int count = showMsgDataList.Count;
			ChatMsgDataItem result = null;
			if (count > 0)
			{
				if (msgHistoryIndex == 0)
				{
					result = GetShowMsgDataList()[index];
				}
				else if (index < msgHistoryIndex)
				{
					result = showMsgDataList[index];
				}
				else if (index >= msgHistoryIndex && index < msgHistoryIndex + 4)
				{
					result = ((index != msgHistoryIndex) ? ChatMsgDataItem.EMPTY_MSG : ChatMsgDataItem.HISTORY_LINE_MSG);
				}
				else if (index >= msgHistoryIndex + 4)
				{
					int num = index - 4;
					if (num < count)
					{
						result = showMsgDataList[num];
					}
				}
			}
			return result;
		}

		private void OnScrollChange(Transform trans, int index)
		{
			Text component = trans.Find("Nickname").GetComponent<Text>();
			Text component2 = trans.Find("Msg").GetComponent<Text>();
			int msgHistoryIndex = GetMsgHistoryIndex();
			ChatMsgDataItem showChatMsgDataItem = GetShowChatMsgDataItem(index, msgHistoryIndex);
			if (showChatMsgDataItem == null)
			{
				component.gameObject.SetActive(false);
				component2.gameObject.SetActive(false);
				return;
			}
			bool flag = Singleton<PlayerModule>.Instance.playerData.userId == showChatMsgDataItem.uid;
			bool flag2 = showChatMsgDataItem.isMsgDataItemBelongToType(ChatMsgDataItem.Type.HISTORY_LINE);
			component.gameObject.SetActive(!flag2);
			component2.gameObject.SetActive(!flag2);
			trans.Find("HistoryLine").gameObject.SetActive(flag2);
			if (showChatMsgDataItem.isMsgDataItemBelongToType(ChatMsgDataItem.Type.EMPTY))
			{
				component.text = string.Empty;
				component2.text = string.Empty;
				return;
			}
			Color color = new Color(1f, 0.6f, 0f);
			Color color2 = new Color(0f, 0.76f, 1f);
			Color color3 = new Color(0.99f, 0.87f, 0.3f);
			Color white = Color.white;
			Color grey = Color.grey;
			Color color4 = new Color(1f, 0.36f, 0.25f);
			if (_mode == Mode.World)
			{
				component.color = ((!flag) ? color2 : color);
				component.text = string.Format("[{0}]", showChatMsgDataItem.nickname);
				component2.color = ((!flag) ? white : color3);
				component2.text = showChatMsgDataItem.msg;
				if (!flag)
				{
				}
			}
			else if (_mode == Mode.Guild)
			{
				component.text = WrapGuildTalkMsgRichText(showChatMsgDataItem);
				component2.color = ((!flag) ? white : color3);
				component2.text = showChatMsgDataItem.msg;
			}
			else if (_mode == Mode.Friend)
			{
				component.text = WrapFriendTalkMsgRichText(showChatMsgDataItem);
				component2.text = showChatMsgDataItem.msg;
				if (Singleton<FriendModule>.Instance.IsMyFriend(_talkingFriendUid))
				{
					component2.color = ((!flag) ? white : color3);
				}
				else
				{
					component2.color = grey;
				}
			}
			if (showChatMsgDataItem.isMsgDataItemBelongToType(ChatMsgDataItem.Type.SYSTEM))
			{
				component.color = color4;
				component2.color = color4;
				component.text = string.Format("[{0}]", LocalizationGeneralLogic.GetText("Chat_Content_System"));
			}
			bool supportRichText = showChatMsgDataItem.isMsgDataItemBelongToType(ChatMsgDataItem.Type.LUCK_GECHA);
			component2.supportRichText = supportRichText;
			if (index < msgHistoryIndex)
			{
				component2.color = grey;
			}
		}

		private string WrapFriendTalkMsgRichText(ChatMsgDataItem msgData)
		{
			string text = LocalizationGeneralLogic.GetText("Chat_Content_I");
			string text2 = LocalizationGeneralLogic.GetText("Chat_Content_To");
			string text3 = LocalizationGeneralLogic.GetText("Chat_Content_Speak");
			if (Singleton<PlayerModule>.Instance.playerData.userId == msgData.uid)
			{
				return string.Format("<color=#ff9900>{0}{1}</color><color=#00c1ff>[{2}]</color><color=#ff9900>{3}</color>:", text, text2, Singleton<FriendModule>.Instance.TryGetPlayerNickName(_talkingFriendUid), text3);
			}
			return string.Format("{0}[{1}]{2}{3}{4}{5}{6}:{7}", "<color=#00c1ff>", Singleton<FriendModule>.Instance.TryGetPlayerNickName(_talkingFriendUid), "</color>", "<color=white>", text2, text, text3, "</color>");
		}

		private string WrapGuildTalkMsgRichText(ChatMsgDataItem msgData)
		{
			if (Singleton<PlayerModule>.Instance.playerData.userId == msgData.uid)
			{
				return string.Format("{0}[{1}]{2}{3}[{4}]:{5}", "<color=#ff9900>", msgData.nickname, "</color>", "<color=#00c1ff>", msgData.guildTitle, "</color>");
			}
			return string.Format("{0}[{1}]{2}{3}[{4}]:{5}", "<color=#00c1ff>", msgData.nickname, "</color>", "<color=#00c1ff>", msgData.guildTitle, "</color>");
		}

		private void UpdateChatList(int addMsgCount)
		{
			MonoGridScroller component = base.view.transform.Find("Dialog/Content/ChatList/ScrollView").GetComponent<MonoGridScroller>();
			component.AddChildren(addMsgCount);
			component.ScrollToNextItem();
		}

		private void UpdateFriendList(int addFriendCount)
		{
			MonoGridScroller component = base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList/ScrollView").GetComponent<MonoGridScroller>();
			component.AddChildren(addFriendCount);
			component.ScrollToNextItem();
		}

		private void UpdateFriendListOpenState()
		{
			base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").gameObject.SetActive(_mode == Mode.Friend);
			bool flag = base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<MonoFriendChatList>().status == MonoFriendChatList.Status.Open;
			base.view.transform.Find("Dialog/Content/FriendListPanel").GetComponent<CanvasGroup>().blocksRaycasts = _mode == Mode.Friend && flag;
			base.view.transform.Find("Dialog/Content/Friend/ChangeBtn/Image").GetComponent<RectTransform>().SetLocalEulerAnglesZ((!_isFriendListShow) ? 180 : 0);
		}

		private void UpdateNewMsgBtnTip()
		{
			bool active = Singleton<ChatModule>.Instance.IsFriendChatListHasNewMsg();
			base.view.transform.Find("Dialog/Content/Friend/ChangeBtn/Tip").gameObject.SetActive(active);
		}

		private ChatMsgDataItem GenerateMsgSendByMe(string msg)
		{
			PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
			return new ChatMsgDataItem(playerData.userId, playerData.NickNameText, TimeUtil.Now, msg);
		}

		private List<ChatMsgDataItem> GetShowMsgDataList()
		{
			if (_mode == Mode.World)
			{
				return Singleton<ChatModule>.Instance.worldChatMsgList;
			}
			if (_mode == Mode.Guild)
			{
				return Singleton<ChatModule>.Instance.guildChatMsgList;
			}
			if (_mode == Mode.Friend)
			{
				return Singleton<ChatModule>.Instance.GetFriendChatMsgList(_talkingFriendUid);
			}
			throw new Exception("Invalid Type or State!");
		}

		private int GetMsgHistoryIndex()
		{
			if (_mode == Mode.World)
			{
				return Singleton<ChatModule>.Instance.worldHistoryMsgIndex;
			}
			if (_mode == Mode.Guild)
			{
				return Singleton<ChatModule>.Instance.guildHistoryMsgIndex;
			}
			if (_mode == Mode.Friend)
			{
				if (Singleton<ChatModule>.Instance.friendHistoryMsgIndexDic.ContainsKey(_talkingFriendUid))
				{
					return Singleton<ChatModule>.Instance.friendHistoryMsgIndexDic[_talkingFriendUid];
				}
				return 0;
			}
			throw new Exception("Invalid Type or State!");
		}
	}
}
