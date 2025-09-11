using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MainPageContext : BasePageContext
	{
		public enum MessageSource
		{
			None = 0,
			System = 1,
			World = 2,
			Guild = 3,
			Friend = 4
		}

		public class PopShowMessage
		{
			public string message;

			public MessageSource source;

			public uint talkingUid;

			public PopShowMessage(string msgContent, MessageSource msgSource)
			{
				message = msgContent;
				source = msgSource;
			}

			public PopShowMessage(string msgContent, uint uid)
			{
				message = msgContent;
				talkingUid = uid;
				source = MessageSource.Friend;
			}
		}

		private const string PAGE_FADE_IN_STR = "PageFadeIn";

		private const string BTNS_FADE_IN_STR = "MainMenuBtnFadeIn";

		private const string BTNS_FADE_OUT_STR = "MainMenuBtnFadeOut";

		private int _maxQuenedMessageCount = 10;

		private bool _optionalBtnsShowed;

		private Queue<PopShowMessage> _showMessageQueue;

		private bool _isMessagePlaying;

		private SequenceDialogManager _firstLoginDialogManager;

		private bool _waitingForIslandServerData;

		private MonoFadeImage _fadeImage;

		public MainPageContext()
		{
			config = new ContextPattern
			{
				contextName = "MainPageContext",
				viewPrefabPath = "UI/Menus/Page/MainPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			showSpaceShip = true;
			_showMessageQueue = new Queue<PopShowMessage>();
			_maxQuenedMessageCount = MiscData.Config.ChatConfig.MaxQuenedPopMessageAmount;
			_isMessagePlaying = false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			switch (cmdId)
			{
			case 25:
			case 48:
				return OnGetAvatarReletedInfo();
			case 97:
				return OnRecvSystemChatMsgNotify(pkt.getData<RecvSystemChatMsgNotify>());
			case 81:
				return OnGetOfflineFriendsPointNotify(pkt.getData<GetOfflineFriendsPointNotify>());
			case 91:
				return OnRecvWorldChatMsgNotify(pkt.getData<RecvWorldChatMsgNotify>());
			case 93:
				return OnRecvFriendChatMsgNotify(pkt.getData<RecvFriendChatMsgNotify>());
			case 94:
				return OnRecvFriendOfflineChatMsgNotify(pkt.getData<RecvFriendOfflineChatMsgNotify>());
			case 65:
			case 71:
				return OnRecvFriendRelatedNotify();
			case 85:
			case 87:
				return OnRecvMailRelatedNotify();
			case 113:
			case 116:
				return OnRecvMissionNotify();
			case 122:
				return OnGetSignInRewardStatusRsp(pkt.getData<GetSignInRewardStatusRsp>());
			case 138:
				return OnRecvBulletinRelatedNotify();
			case 157:
				return OnGetIsLandRsp(pkt.getData<GetIslandRsp>());
			case 198:
				return SetupWelfareHint();
			default:
				if (cmdId == 157 || cmdId == 169)
				{
					return SetupIslandHint();
				}
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.BulletinPopUpUpdate)
			{
				return OnRecvBulletinRelatedNotify();
			}
			return false;
		}

		public override void SetActive(bool enabled)
		{
			if (!enabled)
			{
				MonoGalTouchView monoGalTouchView = UnityEngine.Object.FindObjectOfType<MonoGalTouchView>();
				if (monoGalTouchView != null)
				{
					monoGalTouchView.gameObject.SetActive(false);
				}
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ExitMainPage));
			}
			else
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EnterMainPage));
			}
			base.SetActive(enabled);
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("MainBtns/AvatarBtn").GetComponent<Button>(), OnAvatarBtnClick);
			BindViewCallback(base.view.transform.Find("MainBtns/StorageBtn").GetComponent<Button>(), OnStorageBtnClick);
			BindViewCallback(base.view.transform.Find("MainBtns/MapBtn").GetComponent<Button>(), OnMapBtnClick);
			BindViewCallback(base.view.transform.Find("MainBtns/GachaBtn").GetComponent<Button>(), OnGachaButtonClick);
			BindViewCallback(base.view.transform.Find("MainBtns/IslandBtn").GetComponent<Button>(), OnIslandButtonClick);
			BindViewCallback(base.view.transform.Find("OptionMainBtn").GetComponent<Button>(), OnOptionBtnClick);
			BindViewCallback(base.view.transform.Find("OptionBtns/Btns/FriendBtn").GetComponent<Button>(), OnFriendButtonClick);
			BindViewCallback(base.view.transform.Find("OptionBtns/Btns/MailBtn").GetComponent<Button>(), OnMailButtonClick);
			BindViewCallback(base.view.transform.Find("OptionBtns/Btns/ConfigBtn").GetComponent<Button>(), OnConfigButtonClick);
			BindViewCallback(base.view.transform.Find("OptionBtns/Btns/MissionBtn").GetComponent<Button>(), OnMissionButtonClick);
			BindViewCallback(base.view.transform.Find("OptionBtns/Btns/BulletinBoardBtn").GetComponent<Button>(), OnBulletinBoardButtonClick);
			BindViewCallback(base.view.transform.Find("OptionBtns/Btns/FeedbackBtn").GetComponent<Button>(), OnFeedbackButtonClick);
			BindViewCallback(base.view.transform.Find("Talk").GetComponent<Button>(), OnTalkBtnClick);
		}

		protected override bool SetupView()
		{
			SetupAvatar3dModel();
			OnRecvFriendRelatedNotify();
			OnRecvMissionNotify();
			OnRecvMailRelatedNotify();
			OnRecvBulletinRelatedNotify();
			PressWithCallBack[] componentsInChildren = base.view.transform.Find("MainBtns").GetComponentsInChildren<PressWithCallBack>(true);
			PressWithCallBack[] array = componentsInChildren;
			foreach (PressWithCallBack pressWithCallBack in array)
			{
				pressWithCallBack.onPress = OnMainBtnPress;
			}
			PlayPageFadeInAniamtion();
			if (!Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasLandedMainPage)
			{
				Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasLandedMainPage = true;
				OnFirstTimeLandedOnMainPage();
			}
			else
			{
				CheckDailyUpdateTime();
				Singleton<AssetBundleManager>.GetInstance().CheckSVNVersion();
				if (CanShowStartUpDialogs())
				{
					ShowStartUpDialogs();
				}
				else
				{
					CheckBulletin();
				}
			}
			base.view.transform.Find("CloundDebugBtn").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
			InitFade();
			UpdateAvatarPopUp();
			UIUtil.SpaceshipCheckWeather();
			SetIslandEntry();
			SetupWelfareHint();
			SetupIslandHint();
			return false;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent)
		{
			base.StartUp(canvasTrans, viewParent);
			SetGalTouchActive(true);
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			SetupAvatar3dModel();
			OnRecvFriendRelatedNotify();
			PlayPageFadeInAniamtion();
			SetRedPoints();
			CheckDailyUpdateTime();
			UIUtil.SpaceshipCheckWeather();
			SetGalTouchActive(true);
			Singleton<AssetBundleManager>.Instance.CheckSVNVersion();
			CheckBulletin();
			SetIslandEntry();
			CheckInviteHint();
		}

		private void OnAvatarBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new AvatarOverviewPageContext
			{
				type = AvatarOverviewPageContext.PageType.Show,
				selectedAvatarID = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.lastSelectedAvatarID
			});
		}

		private void OnStorageBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext());
		}

		private void OnMapBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext());
		}

		private void OnGachaButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new SupplyEntrancePageContext());
		}

		private void OnIslandButtonClick()
		{
			_waitingForIslandServerData = true;
			Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(157));
			Singleton<NetworkManager>.Instance.RequestGetIsland();
		}

		private void OnOptionBtnClick()
		{
			_optionalBtnsShowed = !_optionalBtnsShowed;
			base.view.transform.Find("OptionMainBtn/BG/OnImg").gameObject.SetActive(_optionalBtnsShowed);
			base.view.transform.Find("OptionMainBtn/BG/OffImg").gameObject.SetActive(!_optionalBtnsShowed);
			if (_optionalBtnsShowed)
			{
				base.view.transform.GetComponent<Animation>().Play("MainMenuBtnFadeIn");
				MonoMainCanvas monoMainCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoMainCanvas;
				SuperDebug.VeryImportantAssert(monoMainCanvas != null && Singleton<MainUIManager>.Instance.SceneCanvas != null, "canvas is not maincanvas1 " + Singleton<MainUIManager>.Instance.SceneCanvas.ToString());
				if (monoMainCanvas != null)
				{
					Transform transform = monoMainCanvas.playerBar.view.transform;
					transform.Find("TeamBriefPanel").GetComponent<Animation>().Play("TeamBriefFadeIn");
					transform.Find("RightPanel").GetComponent<Animation>().Play("RightPanelFadeIn");
				}
			}
			else
			{
				base.view.transform.GetComponent<Animation>().Play("MainMenuBtnFadeOut");
				MonoMainCanvas monoMainCanvas2 = Singleton<MainUIManager>.Instance.SceneCanvas as MonoMainCanvas;
				SuperDebug.VeryImportantAssert(monoMainCanvas2 != null && Singleton<MainUIManager>.Instance.SceneCanvas != null, "canvas is not maincanvas2 " + Singleton<MainUIManager>.Instance.SceneCanvas.ToString());
				if (monoMainCanvas2 != null)
				{
					Transform transform2 = monoMainCanvas2.playerBar.view.transform;
					transform2.Find("TeamBriefPanel").GetComponent<Animation>().Play("TeamBriefFadeOut");
					transform2.Find("RightPanel").GetComponent<Animation>().Play("RightPanelFadeOut");
				}
			}
			SetOptionalBtnsInteractable(_optionalBtnsShowed);
		}

		public void OnFriendButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new FriendOverviewPageContext());
		}

		public void OnMailButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new MailOverviewPageContext());
		}

		public void OnTalkBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext());
		}

		private void OnConfigButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new SettingPageContext());
		}

		private void OnMissionButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new MissionOverviewPageContext());
		}

		private void OnBulletinBoardButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new BulletinBoardDialogContext());
		}

		private void OnFeedbackButtonClick()
		{
			int userId = Singleton<PlayerModule>.Instance.playerData.userId;
			WebViewGeneralLogic.LoadUrl(string.Format("http://webview.bh3.com/bug_feedback/index.php?uid={0}", userId), true);
		}

		private bool OnGetOfflineFriendsPointNotify(GetOfflineFriendsPointNotify rsp)
		{
			TryShowOfflineFriendsPointView();
			return false;
		}

		private bool OnRecvSystemChatMsgNotify(RecvSystemChatMsgNotify rsp)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			SystemChatMsgType type = rsp.chat_msg.type;
			if ((int)type == 1)
			{
				ChatMsgDataItem chatMsgDataItem = new ChatMsgDataItem(rsp.chat_msg);
				string msgContent = string.Format("{0} {1}", chatMsgDataItem.nickname, chatMsgDataItem.msg);
				EnquenePopShowMessage(new PopShowMessage(msgContent, MessageSource.System));
			}
			CheckMessage();
			return false;
		}

		private bool OnRecvWorldChatMsgNotify(RecvWorldChatMsgNotify rsp)
		{
			EnquenePopShowMessage(new PopShowMessage(rsp.chat_msg.msg, MessageSource.World));
			CheckMessage();
			return false;
		}

		private bool OnRecvFriendChatMsgNotify(RecvFriendChatMsgNotify rsp)
		{
			EnquenePopShowMessage(new PopShowMessage(rsp.chat_msg.msg, rsp.chat_msg.uid));
			CheckMessage();
			return false;
		}

		private bool OnRecvFriendOfflineChatMsgNotify(RecvFriendOfflineChatMsgNotify rsp)
		{
			foreach (ChatMsg item in rsp.chat_msg_list)
			{
				EnquenePopShowMessage(new PopShowMessage(item.msg, item.uid));
			}
			CheckMessage();
			return false;
		}

		private bool OnRecvFriendRelatedNotify()
		{
			base.view.transform.Find("OptionBtns/Btns/FriendBtn/PopUp").gameObject.SetActive(Singleton<FriendModule>.Instance.HasNewFriend() || Singleton<FriendModule>.Instance.HasNewRequest());
			return false;
		}

		private bool OnRecvMailRelatedNotify()
		{
			base.view.transform.Find("OptionBtns/Btns/MailBtn/PopUp").gameObject.SetActive(Singleton<MailModule>.Instance.HasNewMail());
			return false;
		}

		private bool OnRecvBulletinRelatedNotify()
		{
			base.view.transform.Find("OptionBtns/Btns/BulletinBoardBtn/PopUp").gameObject.SetActive(Singleton<BulletinModule>.Instance.HasNewBulletins());
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

		private bool OnRecvMissionNotify()
		{
			base.view.transform.Find("OptionBtns/Btns/MissionBtn/PopUp").gameObject.SetActive(Singleton<MissionModule>.Instance.NeedNotify());
			return false;
		}

		private bool OnGetAvatarReletedInfo()
		{
			UpdateAvatarPopUp();
			SetupAvatar3dModel();
			return false;
		}

		private bool OnGetSignInRewardStatusRsp(GetSignInRewardStatusRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && rsp.is_need_sign_in)
			{
				SignInDialogContext dialogContext = new SignInDialogContext(rsp);
				if (_firstLoginDialogManager != null && _firstLoginDialogManager.IsPlaying())
				{
					_firstLoginDialogManager.AddDialog(dialogContext);
				}
				else
				{
					Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
				}
			}
			return false;
		}

		private bool SetupAvatar3dModel()
		{
			int num = Singleton<GalTouchModule>.Instance.GetCurrentTouchAvatarID();
			if (num == 0)
			{
				num = 101;
			}
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(num);
			UIUtil.Create3DAvatarByPage(avatarByID, MiscData.PageInfoKey.MainPage);
			return false;
		}

		private void OnOptionBtnPress(Transform trans, bool isPress)
		{
			trans.Find("Text").GetComponent<Outline>().effectColor = ((!isPress) ? MiscData.GetColor("Blue") : MiscData.GetColor("Orange"));
		}

		private void OnMainBtnPress(Transform trans, bool isPress)
		{
			trans.Find("Unselect").gameObject.SetActive(!isPress);
			trans.Find("Select").gameObject.SetActive(isPress);
		}

		private void TryShowOfflineFriendsPointView()
		{
			int offlineFriendsPoint = Singleton<PlayerModule>.Instance.playerData.offlineFriendsPoint;
			if (offlineFriendsPoint > 0)
			{
				GeneralDialogContext generalDialogContext = new GeneralDialogContext();
				generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
				generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
				generalDialogContext.desc = LocalizationGeneralLogic.GetText("Menu_Desc_OfflineFriendPointGet", offlineFriendsPoint);
				GeneralDialogContext dialogContext = generalDialogContext;
				if (_firstLoginDialogManager != null && _firstLoginDialogManager.IsPlaying())
				{
					_firstLoginDialogManager.AddDialog(dialogContext);
				}
				else
				{
					Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
				}
				Singleton<PlayerModule>.Instance.playerData.offlineFriendsPoint = 0;
			}
		}

		private void PlayPageFadeInAniamtion()
		{
			base.view.transform.GetComponent<Animation>().Play("MainMenuBtnFadeIn");
			MonoMainCanvas monoMainCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoMainCanvas;
			if (monoMainCanvas != null)
			{
				Transform transform = monoMainCanvas.playerBar.view.transform;
				transform.Find("TeamBriefPanel").GetComponent<Animation>().Play("TeamBriefFadeIn");
				transform.Find("RightPanel").GetComponent<Animation>().Play("RightPanelFadeIn");
			}
			_optionalBtnsShowed = true;
			SetOptionalBtnsInteractable(_optionalBtnsShowed);
		}

		private bool AllowEnqueneMessage()
		{
			if (_showMessageQueue.Count < _maxQuenedMessageCount)
			{
				return true;
			}
			return false;
		}

		private void EnquenePopShowMessage(PopShowMessage message)
		{
			if (message != null && AllowEnqueneMessage())
			{
				_showMessageQueue.Enqueue(message);
			}
		}

		private void ShowNextMessage()
		{
			_showMessageQueue.Dequeue();
			if (_showMessageQueue.Count <= 0)
			{
				_isMessagePlaying = false;
			}
			else
			{
				base.view.transform.Find("NewMessages").GetComponent<MonoMainMessages>().ShowMessage(_showMessageQueue.Peek(), ShowNextMessage);
			}
		}

		private void CheckMessage()
		{
			if (_showMessageQueue.Count > 0 && !_isMessagePlaying)
			{
				_isMessagePlaying = true;
				base.view.transform.Find("NewMessages").GetComponent<MonoMainMessages>().ShowMessage(_showMessageQueue.Peek(), ShowNextMessage);
			}
		}

		private void OnFirstTimeLandedOnMainPage()
		{
			_firstLoginDialogManager = new SequenceDialogManager();
			int offlineFriendsPoint = Singleton<PlayerModule>.Instance.playerData.offlineFriendsPoint;
			if (offlineFriendsPoint > 0)
			{
				_firstLoginDialogManager.AddDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_OfflineFriendPointGet", offlineFriendsPoint)
				});
				Singleton<PlayerModule>.Instance.playerData.offlineFriendsPoint = 0;
			}
			if (CanShowStartUpDialogs())
			{
				if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
				{
					if (TimeUtil.Now.Subtract(Singleton<MiHoYoGameData>.Instance.LocalData.LastShowBindAccountWarningTime).TotalSeconds >= (double)MiscData.Config.BasicConfig.BindAccountWarningIntervalSecond)
					{
						GeneralDialogContext generalDialogContext = new GeneralDialogContext();
						generalDialogContext.type = GeneralDialogContext.ButtonType.DoubleButton;
						generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_BindAccount");
						generalDialogContext.desc = LocalizationGeneralLogic.GetText("Menu_Desc_BindAccount");
						generalDialogContext.okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_DoBindAccount");
						generalDialogContext.cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Action_CancelBindAccount");
						generalDialogContext.buttonCallBack = delegate(bool confirmed)
						{
							if (confirmed)
							{
								Singleton<AccountManager>.Instance.manager.BindUI();
							}
						};
						GeneralDialogContext dialogContext = generalDialogContext;
						_firstLoginDialogManager.AddDialog(dialogContext);
						Singleton<MiHoYoGameData>.Instance.LocalData.LastShowBindAccountWarningTime = TimeUtil.Now;
						Singleton<MiHoYoGameData>.Instance.Save();
					}
				}
				else if (Singleton<AccountManager>.Instance.manager is TheOriginalAccountManager && !(Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager).IsRealNameVerify && TimeUtil.Now.Subtract(Singleton<MiHoYoGameData>.Instance.LocalData.LastShowBindIdentityWarningTime).TotalSeconds >= (double)MiscData.Config.BasicConfig.BindIdentityWarningIntervalSecond)
				{
					GeneralDialogContext generalDialogContext = new GeneralDialogContext();
					generalDialogContext.type = GeneralDialogContext.ButtonType.DoubleButton;
					generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_BindIdentity");
					generalDialogContext.desc = LocalizationGeneralLogic.GetText("Menu_Desc_BindIdentity");
					generalDialogContext.okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_DoBindIdentity");
					generalDialogContext.cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Action_CancelBindIdentity");
					generalDialogContext.buttonCallBack = delegate(bool confirmed)
					{
						if (confirmed)
						{
							string url = string.Format((Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager).ORIGINAL_BIND_IDENTITY_URL + "?uid={0}&token={1}", Singleton<AccountManager>.Instance.manager.AccountUid, Singleton<AccountManager>.Instance.manager.AccountToken);
							WebViewGeneralLogic.LoadUrl(url);
						}
					};
					GeneralDialogContext dialogContext2 = generalDialogContext;
					_firstLoginDialogManager.AddDialog(dialogContext2);
					Singleton<MiHoYoGameData>.Instance.LocalData.LastShowBindIdentityWarningTime = TimeUtil.Now;
					Singleton<MiHoYoGameData>.Instance.Save();
				}
				if (CanShowInviteHintDialog())
				{
					GeneralDialogContext generalDialogContext = new GeneralDialogContext();
					generalDialogContext.type = GeneralDialogContext.ButtonType.DoubleButton;
					generalDialogContext.title = LocalizationGeneralLogic.GetText("Title_InvitationCode_Input");
					generalDialogContext.desc = LocalizationGeneralLogic.GetText("Invitation_Tutorial");
					generalDialogContext.okBtnText = LocalizationGeneralLogic.GetText("Menu_OK");
					generalDialogContext.cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Cancel");
					generalDialogContext.destroyCallBack = delegate
					{
						Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog = true;
						Singleton<MiHoYoGameData>.Instance.Save();
					};
					generalDialogContext.buttonCallBack = delegate(bool confirmed)
					{
						Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog = true;
						Singleton<MiHoYoGameData>.Instance.Save();
						if (confirmed)
						{
							Singleton<MainUIManager>.Instance.ShowPage(new FriendOverviewPageContext(FriendOverviewPageContext.TAB_KEY[3]));
						}
					};
					GeneralDialogContext dialogContext3 = generalDialogContext;
					_firstLoginDialogManager.AddDialog(dialogContext3);
				}
			}
			if (CanShowStartUpDialogs())
			{
				ShowBulletin();
			}
			_firstLoginDialogManager.StartShow();
			if (CanShowStartUpDialogs())
			{
				Singleton<NetworkManager>.GetInstance().RequestGetSignInRewardStatus();
				Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasShowedStartUpDialogs = true;
			}
			//AntiCheatPlugin.Init(MiscData.Config.AntiCheat.Enable, MiscData.Config.AntiCheat.LibList, MiscData.Config.AntiCheat.ProcList);
			Singleton<ApplicationManager>.Instance.DetectCheat();
			AntiEmulatorPlugin.Init(MiscData.Config.AntiEmulator.Enable, MiscData.Config.AntiEmulator.DeviceModelList);
			Singleton<ApplicationManager>.Instance.DetectEmulator();
		}

		private bool CanShowStartUpDialogs()
		{
			if (Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasShowedStartUpDialogs)
			{
				return false;
			}
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
			{
				return false;
			}
			return Singleton<TutorialModule>.GetInstance().IsTutorialIDFinish(1081);
		}

		private bool CanShowInviteHintDialog()
		{
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
			PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
			if (playerData.teamLevel >= playerData.maxLevelToAcceptInvite)
			{
				return false;
			}
			if (Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog)
			{
				return false;
			}
			return Singleton<TutorialModule>.Instance.IsTutorialIDFinish(1081);
		}

		private void ShowStartUpDialogs()
		{
			ShowBulletin(true);
			if (_firstLoginDialogManager != null)
			{
				_firstLoginDialogManager.StartShow();
			}
			Singleton<NetworkManager>.GetInstance().RequestGetSignInRewardStatus();
			Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasShowedStartUpDialogs = true;
		}

		private void ShowBulletin(bool force = false)
		{
			TimeSpan timeSpan = TimeUtil.Now.Subtract(Singleton<MiHoYoGameData>.GetInstance().LocalData.LastShowBulletinTime);
			if (force || timeSpan.TotalSeconds >= (double)MiscData.Config.BasicConfig.ShowBulletinIntervalSecond)
			{
				if (_firstLoginDialogManager != null)
				{
					_firstLoginDialogManager.AddDialog(new BulletinBoardDialogContext());
				}
				Singleton<MiHoYoGameData>.GetInstance().LocalData.LastShowBulletinTime = TimeUtil.Now;
				Singleton<MiHoYoGameData>.GetInstance().Save();
			}
		}

		private void SetRedPoints()
		{
			OnRecvFriendRelatedNotify();
			OnRecvMailRelatedNotify();
			OnRecvBulletinRelatedNotify();
			OnRecvMissionNotify();
			UpdateAvatarPopUp();
		}

		private void SetIslandEntry()
		{
			int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
			Transform transform = base.view.transform.Find("MainBtns/IslandBtn");
			if (!GlobalVars.ENABLE_ISLAND_ENTRY)
			{
				transform.gameObject.SetActive(false);
				return;
			}
			int playerLevelNeedShowIslandButton = MiscData.Config.BasicConfig.PlayerLevelNeedShowIslandButton;
			int playerLevelNeedEnterIsland = MiscData.Config.BasicConfig.PlayerLevelNeedEnterIsland;
			if (teamLevel < playerLevelNeedShowIslandButton)
			{
				transform.gameObject.SetActive(false);
			}
			else if (teamLevel < playerLevelNeedEnterIsland)
			{
				transform.gameObject.SetActive(true);
				transform.Find("LockPanel").gameObject.SetActive(true);
				transform.GetComponent<PressWithCallBack>().onPress = null;
				transform.GetComponent<Button>().interactable = false;
			}
			else
			{
				transform.gameObject.SetActive(true);
				transform.Find("LockPanel").gameObject.SetActive(false);
				transform.GetComponent<PressWithCallBack>().onPress = OnMainBtnPress;
				transform.GetComponent<Button>().interactable = true;
			}
		}

		private void SetOptionalBtnsInteractable(bool interactable)
		{
			foreach (Transform item in base.view.transform.Find("OptionBtns/Btns"))
			{
				item.GetComponent<Button>().enabled = interactable;
				item.GetComponent<MonoButtonWwiseEvent>().enabled = interactable;
			}
		}

		private void UpdateAvatarPopUp()
		{
			List<AvatarDataItem> userAvatarList = Singleton<AvatarModule>.Instance.UserAvatarList;
			bool active = false;
			foreach (AvatarDataItem item in userAvatarList)
			{
				if (item.CanStarUp)
				{
					active = true;
					break;
				}
			}
			base.view.transform.Find("MainBtns/AvatarBtn/PopUp").gameObject.SetActive(active);
		}

		private void CheckDailyUpdateTime()
		{
			if (TimeUtil.AcrossDailyUpdateTime())
			{
				Singleton<NetworkManager>.Instance.SendPacketsOnLoginSuccess(true, 0u);
			}
		}

		private void CheckWeather()
		{
			if (TimeUtil.Now < Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowThunderWeather));
			}
			else if (TimeUtil.Now > Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowRandomWeather));
				Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime = TimeUtil.Now.AddHours(2.0);
			}
		}

		private void CheckBulletin()
		{
			if (!(TimeUtil.Now < Singleton<BulletinModule>.Instance.LastCheckBulletinTime.AddSeconds(MiscData.Config.BasicConfig.CheckBulletinIntervalSecond)))
			{
				Singleton<NetworkManager>.Instance.RequestGetBulletin();
			}
		}

		private void CheckInviteHint()
		{
			if (!CanShowInviteHintDialog())
			{
				return;
			}
			GeneralDialogContext generalDialogContext = new GeneralDialogContext();
			generalDialogContext.type = GeneralDialogContext.ButtonType.DoubleButton;
			generalDialogContext.title = LocalizationGeneralLogic.GetText("Title_InvitationCode_Input");
			generalDialogContext.desc = LocalizationGeneralLogic.GetText("Invitation_Tutorial");
			generalDialogContext.okBtnText = LocalizationGeneralLogic.GetText("Menu_OK");
			generalDialogContext.cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Cancel");
			generalDialogContext.destroyCallBack = delegate
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog = true;
				Singleton<MiHoYoGameData>.Instance.Save();
			};
			generalDialogContext.buttonCallBack = delegate(bool confirmed)
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog = true;
				Singleton<MiHoYoGameData>.Instance.Save();
				if (confirmed)
				{
					Singleton<MainUIManager>.Instance.ShowPage(new FriendOverviewPageContext(FriendOverviewPageContext.TAB_KEY[3]));
				}
			};
			GeneralDialogContext dialogContext = generalDialogContext;
			if (_firstLoginDialogManager != null && _firstLoginDialogManager.IsPlaying())
			{
				_firstLoginDialogManager.AddDialog(dialogContext);
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
			}
		}

		private bool SetupWelfareHint()
		{
			bool active = Singleton<ShopWelfareModule>.Instance.HasWelfareCanGet();
			base.view.transform.Find("MainBtns/GachaBtn/PopUp").gameObject.SetActive(active);
			return false;
		}

		private bool SetupIslandHint()
		{
			bool active = Singleton<IslandModule>.Instance.HasCabinNeedToShowLevelUp() || Singleton<IslandModule>.Instance.GetVentureDoneNum() > 0;
			base.view.transform.Find("MainBtns/IslandBtn/PopUp").gameObject.SetActive(active);
			return false;
		}

		protected override void OnSetActive(bool enabled)
		{
			if (enabled)
			{
				if (CanShowStartUpDialogs())
				{
					ShowStartUpDialogs();
				}
			}
			else
			{
				SetGalTouchActive(false);
			}
		}

		private void SetGalTouchActive(bool active)
		{
			MonoMainCanvas monoMainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas() as MonoMainCanvas;
			if (!(monoMainCanvas != null))
			{
				return;
			}
			Avatar3dModelContext avatar3dModelContext = monoMainCanvas.avatar3dModelContext;
			if (avatar3dModelContext != null)
			{
				if (active)
				{
					avatar3dModelContext.TriggerStartGalTouch();
				}
				else
				{
					avatar3dModelContext.TriggerStopGalTouch();
				}
			}
		}

		private void InitFade()
		{
			_fadeImage = base.view.GetComponentInChildren<MonoFadeImage>();
		}

		public void FadeOut(float speed = 1f)
		{
			if (_fadeImage != null)
			{
				_fadeImage.FadeOut(speed);
			}
		}

		public void FadeIn(float speed = 1f)
		{
			if (_fadeImage != null)
			{
				_fadeImage.FadeIn(speed);
			}
		}
	}
}
