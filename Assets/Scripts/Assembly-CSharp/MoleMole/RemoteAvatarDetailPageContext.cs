using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class RemoteAvatarDetailPageContext : BasePageContext
	{
		public const string LV_UP_TAB = "LvUpTab";

		public const string WEAPON_TAB = "WeaponTab";

		public const string STIGMATA_TAB = "StigmataTab";

		public const string SKILL_TAB = "SkillTab";

		public readonly FriendDetailDataItem userData;

		private TabManager _tabManager;

		private int _showingSkillId;

		private bool _fromDialog;

		private Transform _dialogTrans;

		private MonoAvatarRotatePanel _avatarRotatePanel;

		private Transform _avatarModel;

		public RemoteAvatarDetailPageContext(FriendDetailDataItem userData, bool fromDialog = false, Transform dialogTrans = null)
		{
			config = new ContextPattern
			{
				contextName = "RemoteAvatarDetailPageContext",
				viewPrefabPath = "UI/Menus/Page/AvatarDetailPage"
			};
			showSpaceShip = true;
			this.userData = userData;
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
			_fromDialog = fromDialog;
			_dialogTrans = dialogTrans;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.SelectAvtarSkillIconChange)
			{
				return OnSelectedSkillChanged((int)ntf.body);
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 67:
				return OnAddFriendRsp(pkt.getData<AddFriendRsp>());
			case 69:
				return OnDelFriendRsp(pkt.getData<DelFriendRsp>());
			default:
				return false;
			}
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("TabBtns/LvUpTabBtn").GetComponent<Button>(), OnLvUpTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/WeaponTabBtn").GetComponent<Button>(), OnWeaponTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/StigmataTabBtn").GetComponent<Button>(), OnStigmataTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>(), OnSkillTabBtnClick);
		}

		protected override bool SetupView()
		{
			if (_fromDialog && _dialogTrans != null)
			{
				_dialogTrans.gameObject.SetActive(false);
			}
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = ((!string.IsNullOrEmpty(showingTabKey)) ? showingTabKey : "LvUpTab");
			base.view.transform.Find("AvatarDetailProfile").GetComponent<MonoAvatarDetailProfile>().SetupView(userData.leaderAvatar);
			UIUtil.Create3DAvatarByPage(userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, text);
			SetupLvUpTab();
			SetupWeaponTab();
			SetupStigmataTab();
			SetupSkillTab();
			SetupAvatarRotatePanel();
			SetupMeiHairFade(text);
			base.view.transform.Find("RemotePlayerInfo").gameObject.SetActive(true);
			SetupRemotePlayerView();
			_tabManager.ShowTab(text);
			if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
			{
				base.view.GetComponent<MonoFadeInAnimManager>().Play("FriendLvUpTabFadeIn");
			}
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			string showingTabKey = _tabManager.GetShowingTabKey();
			UIUtil.Create3DAvatarByPage(userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, showingTabKey);
			if (showingTabKey == "StigmataTab")
			{
				_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToBack);
			}
		}

		public override void BackPage()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			if (showingTabKey == "SkillTab")
			{
				base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().OnBackPage();
			}
			else
			{
				base.BackPage();
			}
			if (_fromDialog && _dialogTrans != null)
			{
				_dialogTrans.gameObject.SetActive(true);
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			SetMeiHairFade(string.Empty);
		}

		public void OnLvUpTabBtnClick()
		{
			_tabManager.ShowTab("LvUpTab");
			UIUtil.SetCameraLookAt(userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, "LvUpTab");
			_avatarRotatePanel.enableManualRotate = true;
			_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "LvUpTab");
			SetMeiHairFade("LvUpTab");
		}

		public void OnWeaponTabBtnClick()
		{
			_tabManager.ShowTab("WeaponTab");
			if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
			{
				base.view.GetComponent<MonoFadeInAnimManager>().Play("WeaponTabFadeIn");
			}
			UIUtil.SetCameraLookAt(userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, "WeaponTab");
			_avatarRotatePanel.enableManualRotate = false;
			_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "WeaponTab");
			SetMeiHairFade("WeaponTab");
		}

		public void OnStigmataTabBtnClick()
		{
			_tabManager.ShowTab("StigmataTab");
			UIUtil.SetCameraLookAt(userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
			_avatarRotatePanel.enableManualRotate = false;
			_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
			SetMeiHairFade("StigmataTab");
		}

		public void OnSkillTabBtnClick()
		{
			_tabManager.ShowTab("SkillTab");
			base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(userData);
			UIUtil.SetCameraLookAt(userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, "SkillTab");
			_avatarRotatePanel.enableManualRotate = true;
			_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "SkillTab");
			SetMeiHairFade("SkillTab");
		}

		private bool OnAddFriendRsp(AddFriendRsp rsp)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected I4, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected I4, but got Unknown
			int target_uid = (int)rsp.target_uid;
			string desc = string.Empty;
			Retcode retcode = rsp.retcode;
			switch ((int)retcode)
			{
			case 0:
			{
				string text = Singleton<FriendModule>.Instance.TryGetPlayerNickName(target_uid);
				AddFriendAction action = rsp.action;
				switch (action - 1)
				{
				case 1:
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_AgreeFriend", text);
					break;
				case 2:
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_RejectFriend", text);
					break;
				case 0:
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_RequestAddFriend", text);
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
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(desc));
			return false;
		}

		private bool OnDelFriendRsp(DelFriendRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
				string text = Singleton<FriendModule>.Instance.TryGetPlayerNickName((int)rsp.target_uid);
				string text2 = LocalizationGeneralLogic.GetText("Menu_Desc_DeleteFriend", text);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(text2));
			}
			else
			{
				string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput));
			}
			return false;
		}

		private bool OnSelectedSkillChanged(int newSkillId)
		{
			_showingSkillId = newSkillId;
			AvatarSkillDataItem selectedSkillData = ((_showingSkillId != 0) ? userData.leaderAvatar.GetAvatarSkillBySkillID(_showingSkillId) : null);
			base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(userData, selectedSkillData);
			return false;
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("TabGreen") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			go.SetActive(active);
		}

		private void SetupLvUpTab()
		{
			GameObject gameObject = base.view.transform.Find("LvUpTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/LvUpTabBtn").GetComponent<Button>();
			component.transform.Find("Text").GetComponent<LocalizedText>().TextID = "Menu_Detail";
			component.transform.Find("PopUp").gameObject.SetActive(false);
			_tabManager.SetTab("LvUpTab", component, gameObject);
			gameObject.GetComponent<MonoAvatarDetailLvUpTab>().SetupView(userData);
			FriendBriefDataItem friendBriefDataItem = Singleton<FriendModule>.Instance.TryGetFriendBriefData(userData.uid);
			if (friendBriefDataItem != null)
			{
				base.view.transform.Find("AvatarDetailProfile/Info/CombatNumText").GetComponent<Text>().text = friendBriefDataItem.avatarCombat.ToString();
			}
		}

		private void SetupWeaponTab()
		{
			GameObject gameObject = base.view.transform.Find("WeaponTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/WeaponTabBtn").GetComponent<Button>();
			_tabManager.SetTab("WeaponTab", component, gameObject);
			gameObject.GetComponent<MonoAvatarDetailWeaponTab>().SetupView(userData);
		}

		private void SetupStigmataTab()
		{
			GameObject gameObject = base.view.transform.Find("StigmataTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/StigmataTabBtn").GetComponent<Button>();
			_tabManager.SetTab("StigmataTab", component, gameObject);
			gameObject.GetComponent<MonoAvatarDetailStigmataTab>().SetupView(userData);
			RectTransform rectTransform = gameObject.transform.Find("Effect") as RectTransform;
			Vector3 vector = rectTransform.anchoredPosition;
			vector.y = 260f;
			rectTransform.anchoredPosition = vector;
		}

		private void SetupSkillTab()
		{
			GameObject gameObject = base.view.transform.Find("SkillTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>();
			_tabManager.SetTab("SkillTab", component, gameObject);
			component.transform.Find("PopUp").gameObject.SetActive(false);
			AvatarSkillDataItem selectedSkillData = ((_showingSkillId != 0) ? userData.leaderAvatar.GetAvatarSkillBySkillID(_showingSkillId) : null);
			gameObject.GetComponent<MonoAvatarDetailSkillTab>().SetupView(userData, selectedSkillData);
		}

		private void SetupAvatarRotatePanel()
		{
			Transform transform = base.view.transform.Find("AvatarRotatePanel");
			_avatarRotatePanel = transform.GetComponent<MonoAvatarRotatePanel>();
			_avatarRotatePanel.SetupView(userData.leaderAvatar);
		}

		private void SetupRemotePlayerView()
		{
			base.view.transform.Find("RemotePlayerInfo/Info/NameText").GetComponent<Text>().text = userData.nickName;
			base.view.transform.Find("RemotePlayerInfo/Info/IDText").GetComponent<Text>().text = "ID." + userData.uid;
		}

		private void SetupMeiHairFade(string tabName)
		{
			BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
			if (sceneCanvas is MonoMainCanvas)
			{
				_avatarModel = ((MonoMainCanvas)sceneCanvas).avatar3dModelContext.GetAvatarById(userData.leaderAvatar.avatarID);
			}
			else
			{
				_avatarModel = ((MonoTestUI)sceneCanvas).avatar3dModelContext.GetAvatarById(userData.leaderAvatar.avatarID);
			}
			SetMeiHairFade(tabName);
		}

		private void SetMeiHairFade(string tabName = "")
		{
			MonoMeiHairFadeAnimation[] componentsInChildren = _avatarModel.GetComponentsInChildren<MonoMeiHairFadeAnimation>();
			foreach (MonoMeiHairFadeAnimation monoMeiHairFadeAnimation in componentsInChildren)
			{
				if (tabName == "WeaponTab")
				{
					monoMeiHairFadeAnimation.FadeForWeaponTab();
				}
				else if (tabName == "StigmataTab")
				{
					monoMeiHairFadeAnimation.FadeForStigmataTab();
				}
				else
				{
					monoMeiHairFadeAnimation.CancelFade();
				}
			}
		}
	}
}
