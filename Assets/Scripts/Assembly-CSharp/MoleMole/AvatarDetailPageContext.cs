using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;
using Avatar = proto.Avatar;

namespace MoleMole
{
	public class AvatarDetailPageContext : BasePageContext
	{
		public const string LV_UP_TAB = "LvUpTab";

		public const string WEAPON_TAB = "WeaponTab";

		public const string STIGMATA_TAB = "StigmataTab";

		public const string SKILL_TAB = "SkillTab";

		public readonly AvatarDataItem avatarData;

		public readonly string defaultTab;

		private TabManager _tabManager;

		private int _showingSkillId;

		private MonoAvatarRotatePanel _avatarRotatePanel;

		private Transform _avatarModel;

		private bool _skillPopUpVisible;

		private bool _shouldShowSkillPointExchangeDialog;

		private AvatarDataItem _avatarBeforeLevelUp;

		public AvatarDetailPageContext(AvatarDataItem avatarData, string defaultTab = "LvUpTab")
		{
			config = new ContextPattern
			{
				contextName = "AvatarDetailPageContext",
				viewPrefabPath = "UI/Menus/Page/AvatarDetailPage"
			};
			showSpaceShip = true;
			this.avatarData = avatarData;
			this.defaultTab = defaultTab;
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.SelectAvtarSkillIconChange)
			{
				return OnSelectedSkillChanged((int)ntf.body);
			}
			if (ntf.type == NotifyTypes.SubSkillStatusCacheUpdate)
			{
				return SetupSkillTab();
			}
			if (ntf.type == NotifyTypes.SubSkillStatusCacheUpdate)
			{
				return RecordShouldShowSkillPointExchangeDialog();
			}
			if (ntf.type == NotifyTypes.BeforeAvatarLevelUp)
			{
				return OnBeforeAvatarLevelUp((AvatarDataItem)ntf.body);
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 25:
				return OnGetAvatarDataRsp(pkt.getData<GetAvatarDataRsp>());
			case 30:
				return OnAvatarStarUpRsp(pkt.getData<AvatarStarUpRsp>());
			case 36:
				return OnAddAvatarExpByMaterialRsp(pkt.getData<AddAvatarExpByMaterialRsp>());
			case 11:
				return OnGetMainDataRsp(pkt.getData<GetMainDataRsp>());
			case 55:
				return OnSkillPointExchangeRsp(pkt.getData<SkillPointExchangeRsp>());
			case 53:
				return OnGetSkillPointExchangeInfoRsp(pkt.getData<GetSkillPointExchangeInfoRsp>());
			case 51:
				return OnAvatarSubSkillLevelUpRsp(pkt.getData<AvatarSubSkillLevelUpRsp>());
			case 32:
				return OnEquipmentPowerupRsp(pkt.getData<EquipmentPowerUpRsp>());
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
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = ((!string.IsNullOrEmpty(showingTabKey)) ? showingTabKey : defaultTab);
			base.view.transform.Find("AvatarDetailProfile").GetComponent<MonoAvatarDetailProfile>().SetupView(avatarData);
			UIUtil.Create3DAvatarByPage(avatarData, MiscData.PageInfoKey.AvatarDetailPage, text);
			_tabManager.Clear();
			SetupLvUpTab();
			SetupWeaponTab();
			SetupStigmataTab();
			SetupSkillTab();
			SetupAvatarRotatePanel(text);
			SetupMeiHairFade(text);
			base.view.transform.Find("RemotePlayerInfo").gameObject.SetActive(false);
			_tabManager.ShowTab(text);
			CheckLockByMissionTutorial();
			return false;
		}

		public override void BackPage()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			if (showingTabKey == "SkillTab")
			{
				base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().OnBackPage();
				return;
			}
			UIUtil.SetAvatarTattooVisible(false, avatarData);
			base.BackPage();
		}

		public override void BackToMainMenuPage()
		{
			UIUtil.SetAvatarTattooVisible(false, avatarData);
			base.BackToMainMenuPage();
		}

		public override void Destroy()
		{
			base.Destroy();
			SetMeiHairFade(string.Empty);
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			LetterSpacing[] componentsInChildren = base.view.transform.GetComponentsInChildren<LetterSpacing>();
			foreach (LetterSpacing letterSpacing in componentsInChildren)
			{
				if (letterSpacing.autoFixLine)
				{
					letterSpacing.AccommodateText();
				}
			}
			if (_tabManager.GetShowingTabKey() == "StigmataTab")
			{
				UIUtil.SetCameraLookAt(avatarData, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
				UIUtil.SetAvatarTattooVisible(true, avatarData);
			}
		}

		public void OnLvUpTabBtnClick()
		{
			_tabManager.ShowTab("LvUpTab");
			UIUtil.SetCameraLookAt(avatarData, MiscData.PageInfoKey.AvatarDetailPage, "LvUpTab");
			UIUtil.SetAvatarTattooVisible(false, avatarData);
			_avatarRotatePanel.enableManualRotate = true;
			_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "LvUpTab");
			SetMeiHairFade("LvUpTab");
			CheckLockByMissionTutorial();
		}

		public void OnWeaponTabBtnClick()
		{
			_tabManager.ShowTab("WeaponTab");
			UIUtil.SetCameraLookAt(avatarData, MiscData.PageInfoKey.AvatarDetailPage, "WeaponTab");
			UIUtil.SetAvatarTattooVisible(false, avatarData);
			_avatarRotatePanel.enableManualRotate = false;
			_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "WeaponTab");
			SetMeiHairFade("WeaponTab");
			CheckLockByMissionTutorial();
		}

		public void OnStigmataTabBtnClick()
		{
			_tabManager.ShowTab("StigmataTab");
			UIUtil.SetCameraLookAt(avatarData, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
			UIUtil.SetAvatarTattooVisible(true, avatarData);
			_avatarRotatePanel.enableManualRotate = false;
			_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
			SetMeiHairFade("StigmataTab");
			CheckLockByMissionTutorial();
		}

		public void OnSkillTabBtnClick()
		{
			_tabManager.ShowTab("SkillTab");
			base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(avatarData);
			UIUtil.SetCameraLookAt(avatarData, MiscData.PageInfoKey.AvatarDetailPage, "SkillTab");
			UIUtil.SetAvatarTattooVisible(false, avatarData);
			_avatarRotatePanel.enableManualRotate = true;
			_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "SkillTab");
			SetMeiHairFade("SkillTab");
		}

		private bool OnGetAvatarDataRsp(GetAvatarDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				SetupView();
				if (rsp.avatar_list.Count > 0 && _avatarBeforeLevelUp != null)
				{
					Avatar val = rsp.avatar_list[0];
					if (_avatarBeforeLevelUp.avatarID == val.avatar_id && val.level > _avatarBeforeLevelUp.level)
					{
						Singleton<ApplicationManager>.Instance.StartCoroutine(PostLevelUpAudioCoroutine((int)val.avatar_id));
						Singleton<MainUIManager>.Instance.ShowDialog(new AvatarLevelUpDialogContext(val.level, (uint)_avatarBeforeLevelUp.level));
						UIUtil.UpdateAvatarSkillStatusInLocalData(avatarData);
					}
					_avatarBeforeLevelUp = null;
				}
			}
			return false;
		}

		private bool OnAvatarStarUpRsp(AvatarStarUpRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode != 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			else
			{
				Singleton<ApplicationManager>.Instance.StartCoroutine(PostLevelUpAudioCoroutine(avatarData.avatarID));
				Singleton<MainUIManager>.Instance.ShowDialog(new AvatarPromotionDialogContext(avatarData));
				UIUtil.UpdateAvatarSkillStatusInLocalData(avatarData);
				SetupSkillTab();
			}
			return false;
		}

		private bool OnAddAvatarExpByMaterialRsp(AddAvatarExpByMaterialRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				UIUtil.UpdateAvatarSkillStatusInLocalData(avatarData);
				SetupSkillTab();
			}
			return false;
		}

		private bool OnGetSkillPointExchangeInfoRsp(GetSkillPointExchangeInfoRsp rsp)
		{
			if (_shouldShowSkillPointExchangeDialog)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new SkillPointExchangeDialogContext());
				_shouldShowSkillPointExchangeDialog = false;
			}
			return false;
		}

		private bool OnSkillPointExchangeRsp(SkillPointExchangeRsp rsp)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Invalid comparison between Unknown and I4
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupSkillPoint();
			if ((int)rsp.retcode == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeSucc"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_SkillPtExchangeRes", rsp.hcoin_cost, rsp.skill_point_get)
				});
			}
			else if ((int)rsp.retcode == 2)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.DoubleButton,
					title = LocalizationGeneralLogic.GetText("Menu_GoToRecharge"),
					desc = LocalizationGeneralLogic.GetText("Menu_GoToRechargeDesc"),
					okBtnText = LocalizationGeneralLogic.GetText("Menu_GoToRecharge"),
					cancelBtnText = LocalizationGeneralLogic.GetText("Menu_GiveUpRecharge"),
					buttonCallBack = delegate(bool confirmed)
					{
						if (confirmed)
						{
							Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext());
						}
					}
				});
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			return false;
		}

		private bool OnGetMainDataRsp(GetMainDataRsp rsp)
		{
			if (rsp.skill_pointSpecified || rsp.skill_point_limitSpecified)
			{
				base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupSkillPoint();
			}
			return false;
		}

		private bool OnAvatarSubSkillLevelUpRsp(AvatarSubSkillLevelUpRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				AvatarSkillDataItem selectedSkillData = ((_showingSkillId != 0) ? avatarData.GetAvatarSkillBySkillID(_showingSkillId) : null);
				base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(avatarData, selectedSkillData);
			}
			return false;
		}

		private bool OnEquipmentPowerupRsp(EquipmentPowerUpRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				SetupLvUpTab();
				SetupWeaponTab();
				SetupStigmataTab();
			}
			return false;
		}

		private bool OnSelectedSkillChanged(int newSkillId)
		{
			_showingSkillId = newSkillId;
			AvatarSkillDataItem selectedSkillData = ((_showingSkillId != 0) ? avatarData.GetAvatarSkillBySkillID(_showingSkillId) : null);
			base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(avatarData, selectedSkillData);
			SetupAvatarSkillPopUp();
			return false;
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			go.SetActive(active);
		}

		private void SetupLvUpTab()
		{
			GameObject gameObject = base.view.transform.Find("LvUpTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/LvUpTabBtn").GetComponent<Button>();
			component.transform.Find("PopUp").gameObject.SetActive(avatarData.CanStarUp);
			_tabManager.SetTab("LvUpTab", component, gameObject);
			gameObject.GetComponent<MonoAvatarDetailLvUpTab>().SetupView(avatarData);
		}

		private void SetupWeaponTab()
		{
			GameObject gameObject = base.view.transform.Find("WeaponTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/WeaponTabBtn").GetComponent<Button>();
			_tabManager.SetTab("WeaponTab", component, gameObject);
			gameObject.GetComponent<MonoAvatarDetailWeaponTab>().SetupView(avatarData);
		}

		private void SetupStigmataTab()
		{
			GameObject gameObject = base.view.transform.Find("StigmataTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/StigmataTabBtn").GetComponent<Button>();
			_tabManager.SetTab("StigmataTab", component, gameObject);
			gameObject.GetComponent<MonoAvatarDetailStigmataTab>().SetupView(avatarData);
			RectTransform rectTransform = gameObject.transform.Find("Effect") as RectTransform;
			Vector3 vector = rectTransform.anchoredPosition;
			vector.y = 140f;
			rectTransform.anchoredPosition = vector;
		}

		private bool SetupSkillTab()
		{
			GameObject gameObject = base.view.transform.Find("SkillTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>();
			_tabManager.SetTab("SkillTab", component, gameObject);
			AvatarSkillDataItem selectedSkillData = ((_showingSkillId != 0) ? avatarData.GetAvatarSkillBySkillID(_showingSkillId) : null);
			gameObject.GetComponent<MonoAvatarDetailSkillTab>().SetupView(avatarData, selectedSkillData);
			SetupAvatarSkillPopUp();
			return false;
		}

		private bool RecordShouldShowSkillPointExchangeDialog()
		{
			_shouldShowSkillPointExchangeDialog = true;
			return false;
		}

		private bool OnBeforeAvatarLevelUp(AvatarDataItem avatarData)
		{
			_avatarBeforeLevelUp = new AvatarDataItem(avatarData.avatarID, avatarData.level, avatarData.star);
			return false;
		}

		private void SetupAvatarRotatePanel(string defaultTab)
		{
			Transform transform = base.view.transform.Find("AvatarRotatePanel");
			_avatarRotatePanel = transform.GetComponent<MonoAvatarRotatePanel>();
			_avatarRotatePanel.SetupView(avatarData);
			UIUtil.SetCameraLookAt(avatarData, MiscData.PageInfoKey.AvatarDetailPage, defaultTab);
			if (defaultTab == "LvUpTab" || defaultTab == "SkillTab")
			{
				_avatarRotatePanel.enableManualRotate = true;
			}
			else
			{
				_avatarRotatePanel.enableManualRotate = false;
			}
			_avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, defaultTab);
		}

		private bool SetupAvatarSkillPopUp()
		{
			_skillPopUpVisible = false;
			Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
			foreach (AvatarSkillDataItem skillData in avatarData.skillDataList)
			{
				if (!skillData.UnLocked)
				{
					continue;
				}
				foreach (AvatarSubSkillDataItem avatarSubSkill in skillData.avatarSubSkillList)
				{
					if (subSkillStatusDict.ContainsKey(avatarSubSkill.subSkillID))
					{
						_skillPopUpVisible = true;
						break;
					}
				}
			}
			base.view.transform.Find("TabBtns/SkillTabBtn/PopUp").gameObject.SetActive(_skillPopUpVisible && base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>().interactable);
			return false;
		}

		private void CheckLockByMissionTutorial()
		{
			bool flag = UnlockUIDataReaderExtend.UnLockByMission(2) && UnlockUIDataReaderExtend.UnlockByTutorial(2);
			base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>().interactable = flag;
			base.view.transform.Find("TabBtns/SkillTabBtn/PopUp").gameObject.SetActive(_skillPopUpVisible && flag);
			base.view.transform.Find("TabBtns/SkillTabBtn/Lock").gameObject.SetActive(!flag);
			MonoButtonWwiseEvent monoButtonWwiseEvent = base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<MonoButtonWwiseEvent>();
			if (monoButtonWwiseEvent == null)
			{
				monoButtonWwiseEvent = base.view.transform.Find("TabBtns/SkillTabBtn").gameObject.AddComponent<MonoButtonWwiseEvent>();
			}
			monoButtonWwiseEvent.eventName = ((!flag) ? "UI_Gen_Select_Negative" : "UI_Click");
		}

		private void SetupMeiHairFade(string tabName)
		{
			BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
			if (sceneCanvas is MonoMainCanvas)
			{
				_avatarModel = ((MonoMainCanvas)sceneCanvas).avatar3dModelContext.GetAvatarById(avatarData.avatarID);
			}
			else
			{
				_avatarModel = ((MonoTestUI)sceneCanvas).avatar3dModelContext.GetAvatarById(avatarData.avatarID);
			}
			SetMeiHairFade(tabName);
		}

		private void SetMeiHairFade(string tabName = "")
		{
			if (_avatarModel == null)
			{
				return;
			}
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

		private IEnumerator PostLevelUpAudioCoroutine(int id)
		{
			yield return new WaitForSeconds(1f);
			PostLevelUpAudioEvent(id);
		}

		private void PostLevelUpAudioEvent(int id)
		{
			int num = id / 100;
			string text = null;
			switch (num)
			{
			case 1:
				text = "VO_M_Kia_05_LevelUp";
				break;
			case 2:
				text = "VO_M_Mei_05_LevelUp";
				break;
			case 3:
				text = "VO_M_Bro_05_LevelUp";
				break;
			}
			if (text != null)
			{
				Singleton<WwiseAudioManager>.Instance.Post(text);
			}
		}
	}
}
