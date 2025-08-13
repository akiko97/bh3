using System.Collections;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class PlayerStatusWidgetContext : BaseWidgetContext
	{
		private CanvasTimer _timer;

		private bool teamProfileActive = true;

		private SequenceDialogManager _achieveDialogManager;

		public PlayerStatusWidgetContext()
		{
			config = new ContextPattern
			{
				contextName = "PlayerStatusWidgetContext",
				viewPrefabPath = "UI/Menus/Widget/PlayerStatusPanel",
				cacheType = ViewCacheType.AlwaysCached
			};
			findViewSavedInScene = true;
			uiType = UIType.SuspendBar;
			_achieveDialogManager = new SequenceDialogManager(delegate
			{
				_achieveDialogManager.ClearDialogs();
			});
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 11:
				return SetupView();
			case 15:
				return OnScoinExchangeRsp(pkt.getData<ScoinExchangeRsp>());
			case 19:
				return OnStaminaExchangeRsp(pkt.getData<StaminaExchangeRsp>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.ShowScoinExchangeInfo)
			{
				return ShowSCoinExchangeDialog();
			}
			if (ntf.type == NotifyTypes.ShowStaminaExchangeInfo)
			{
				return ShowStaminaExchangeDialog();
			}
			if (ntf.type == NotifyTypes.SetBackButtonActive)
			{
				return OnSetBackButtonActive((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetPlayerStatusWidgetDisplay)
			{
				return OnSetPlayerStatusWidgetDisplay((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.MissionUpdated)
			{
				return OnMissionUpdated((uint)ntf.body);
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("TeamBriefPanel/Button").GetComponent<Button>(), OnTeamBriefPanelClick);
			BindViewCallback(base.view.transform.Find("RightPanel/SCoin").GetComponent<Button>(), OnSCoinPanelClick);
			BindViewCallback(base.view.transform.Find("RightPanel/HCoin").GetComponent<Button>(), OnHCoinPanelClick);
			BindViewCallback(base.view.transform.Find("RightPanel/Stamina").GetComponent<Button>(), OnStaminaPanelPanelClick);
			BindViewCallback(base.view.transform.Find("ActionBtns/BackBtn").GetComponent<Button>(), OnBackBtnClick);
			BindViewCallback(base.view.transform.Find("ActionBtns/MainMenuBtn").GetComponent<Button>(), OnMainMenuBtnClick);
			if (GlobalVars.LEVEL_MODE_DEBUG)
			{
				BindViewCallback(base.view.transform.Find("GMTalkButton").GetComponent<Button>(), OnGMTalkBtnClick);
			}
		}

		protected override bool SetupView()
		{
			PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
			base.view.transform.Find("TeamBriefPanel").gameObject.SetActive(true);
			base.view.transform.Find("RightPanel").gameObject.SetActive(true);
			base.view.transform.Find("ActionBtns/MainMenuBtn").gameObject.SetActive(true);
			base.view.transform.Find("TeamBriefPanel").gameObject.SetActive(teamProfileActive);
			base.view.transform.Find("TeamBriefPanel/NickName/NicknameText").GetComponent<Text>().text = playerData.NickNameText;
			base.view.transform.Find("TeamBriefPanel/LevelText").GetComponent<Text>().text = "LV." + playerData.teamLevel;
			base.view.transform.Find("TeamBriefPanel/NickName/PopUp/Notice").gameObject.SetActive(PopupNoticeAcitve());
			MonoSliderGroup component = base.view.transform.Find("TeamBriefPanel/TeamExpSlider").GetComponent<MonoSliderGroup>();
			component.UpdateValue(playerData.teamExp, playerData.TeamMaxExp, 0f);
			base.view.transform.Find("RightPanel/SCoin/NumText").GetComponent<Text>().text = string.Empty + playerData.scoin;
			base.view.transform.Find("RightPanel/HCoin/NumText").GetComponent<Text>().text = string.Empty + playerData.hcoin;
			base.view.transform.Find("RightPanel/Stamina/NumText").GetComponent<Text>().text = string.Empty + playerData.stamina;
			base.view.transform.Find("RightPanel/Stamina/MaxNumText").GetComponent<Text>().text = string.Empty + playerData.MaxStamina;
			base.view.transform.Find("GMTalkButton").gameObject.SetActive(GlobalVars.LEVEL_MODE_DEBUG);
			base.view.transform.Find("GMTalkButton").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
			return false;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent = null)
		{
			base.StartUp(canvasTrans, viewParent);
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateInfiniteTimer(1f);
			_timer.timeTriggerCallback = CheckPlayerData;
		}

		public override void Destroy()
		{
			if (_timer != null)
			{
				_timer.Destroy();
			}
			base.Destroy();
		}

		public void OnTeamBriefPanelClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new PlayerProfilePageContext());
		}

		public void OnSCoinPanelClick()
		{
			if (Singleton<PlayerModule>.Instance.playerData.scoinExchangeCache.Value != null)
			{
				ShowSCoinExchangeDialog();
			}
			else
			{
				Singleton<NetworkManager>.Instance.RequestGetScoinExchangeInfo();
			}
		}

		public void OnHCoinPanelClick()
		{
			if (!(Singleton<MainUIManager>.Instance.CurrentPageContext is RechargePageContext))
			{
				Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext());
			}
		}

		public void OnStaminaPanelPanelClick()
		{
			Singleton<PlayerModule>.Instance.playerData._cacheDataUtil.CheckCacheValidAndGo<PlayerStaminaExchangeInfo>(ECacheData.Stamina, NotifyTypes.ShowStaminaExchangeInfo);
		}

		public void OnGMTalkBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GMTalkDialogContext());
		}

		public void OnBackBtnClick()
		{
			Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
		}

		public void OnMainMenuBtnClick()
		{
			Singleton<MainUIManager>.Instance.CurrentPageContext.BackToMainMenuPage();
		}

		public bool OnScoinExchangeRsp(ScoinExchangeRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Invalid comparison between Unknown and I4
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				string text = MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Hcoin")) + " - " + rsp.hcoin_cost + " ， " + MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Scoin")) + " + " + rsp.scoin_get;
				if (rsp.boost_rateSpecified && rsp.boost_rate > 100)
				{
					string text2 = text;
					text = text2 + " ， " + MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Desc_Critical")) + " × " + string.Format("{0:0%}", (float)rsp.boost_rate / 100f);
				}
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeSucc"),
					desc = text
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

		private bool OnMissionUpdated(uint id)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Invalid comparison between Unknown and I4
			base.view.transform.Find("TeamBriefPanel/NickName/PopUp/Notice").gameObject.SetActive(PopupNoticeAcitve());
			MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem((int)id);
			if (missionDataItem == null)
			{
				return false;
			}
			LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey((int)id);
			if (linearMissionDataByKey == null || linearMissionDataByKey.IsAchievement == 0)
			{
				return false;
			}
			if ((int)missionDataItem.status == 3)
			{
				_achieveDialogManager.AddDialog(new AchieveUnlockContext((int)id));
				if (!_achieveDialogManager.IsPlaying())
				{
					Singleton<ApplicationManager>.Instance.StartCoroutine(DelayShowAchieveUnlockCoroutine());
				}
			}
			return false;
		}

		public bool ShowSCoinExchangeDialog()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new SCoinExchangeDialogContext());
			return false;
		}

		public bool ShowStaminaExchangeDialog()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new StaminaExchangeDialogContext("Menu_Desc_StaminaExchange"));
			return false;
		}

		public bool OnSetBackButtonActive(bool active)
		{
			teamProfileActive = !active;
			base.view.transform.Find("TeamBriefPanel").gameObject.SetActive(teamProfileActive);
			base.view.transform.Find("ActionBtns").gameObject.SetActive(!teamProfileActive);
			return false;
		}

		public bool OnSetPlayerStatusWidgetDisplay(bool show)
		{
			base.view.gameObject.SetActive(show);
			return false;
		}

		public bool OnStaminaExchangeRsp(StaminaExchangeRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Invalid comparison between Unknown and I4
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				string desc = MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Hcoin")) + "-" + rsp.hcoin_cost + " ， " + MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Stamina")) + "+" + rsp.stamina_get;
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeSucc"),
					desc = desc
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

		private IEnumerator DelayShowAchieveUnlockCoroutine()
		{
			yield return new WaitForSeconds(0.8f);
			while (true)
			{
				BasePageContext pageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
				if (pageContext == null || pageContext is GachaMainPageContext || pageContext is GachaResultPageContext || pageContext.dialogContextList.Count > 0)
				{
					yield return null;
					continue;
				}
				break;
			}
			_achieveDialogManager.StartShow();
		}

		private void CheckPlayerData()
		{
			if (!Singleton<PlayerModule>.Instance.playerData.initByGetMainDataRsp)
			{
				Singleton<NetworkManager>.Instance.RequestGetAllMainData();
			}
			else if (!Singleton<PlayerModule>.Instance.playerData.IsStaminaFull() && Singleton<PlayerModule>.Instance.playerData.nextStaminaRecoverDatetime < TimeUtil.Now)
			{
				Singleton<NetworkManager>.Instance.RequestGetStaminaRecoverLeftTime();
			}
			else if (!Singleton<PlayerModule>.Instance.playerData.IsSkillPointFull() && Singleton<PlayerModule>.Instance.playerData.nextSkillPtRecoverDatetime < TimeUtil.Now)
			{
				Singleton<NetworkManager>.Instance.RequestGetSkillPointRecoverLeftTime();
			}
		}

		public void HideAllButBackBtn()
		{
			base.view.transform.Find("TeamBriefPanel").gameObject.SetActive(false);
			base.view.transform.Find("RightPanel").gameObject.SetActive(false);
			base.view.transform.Find("ActionBtns/MainMenuBtn").gameObject.SetActive(false);
			base.view.transform.Find("GMTalkButton").gameObject.SetActive(GlobalVars.LEVEL_MODE_DEBUG);
			base.view.transform.Find("ActionBtns/BackBtn").gameObject.SetActive(true);
		}

		public void ResetView()
		{
			SetupView();
		}

		public void SetPopupVisible(bool visible)
		{
			Transform transform = base.view.transform.Find("TeamBriefPanel/NickName/PopUp/Notice");
			if (transform != null)
			{
				transform.gameObject.SetActive(visible);
			}
			base.view.transform.Find("TeamBriefPanel/NickName/PopUp/Normal").gameObject.SetActive(false);
		}
	}
}
