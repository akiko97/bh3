using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class RechargePageContext : BasePageContext
	{
		public const string RechargeTab = "RechargeTab";

		public const string WelfareTab = "RewardTab";

		private const string TAB_SELECT_BG_PATH = "SpriteOutput/GeneralUI/TabDialogSelected";

		private const string TAB_UNSELECT_BG_PATH = "SpriteOutput/GeneralUI/TabDialogUnselected";

		public readonly string defaultTab;

		private TabManager _tabManager;

		private ShopType _currentShopType;

		private int _playerLevelBefore;

		public RechargePageContext(string defaultTab = "RechargeTab")
		{
			config = new ContextPattern
			{
				contextName = "RechargePageContext",
				viewPrefabPath = "UI/Menus/Page/Shop/RechargePagePage",
				cacheType = ViewCacheType.AlwaysCached
			};
			showSpaceShip = false;
			this.defaultTab = defaultTab;
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 198:
				SetupWelfareHint();
				return OnGetVipRewardDataRsp(pkt.getData<GetVipRewardDataRsp>());
			case 200:
				SetupWelfareHint();
				return OnGetVipRewardRsp(pkt.getData<GetVipRewardRsp>());
			case 206:
				return OnManualRefreshShopRsp(pkt.getData<ManualRefreshShopRsp>());
			case 83:
				return OnRechargeSuccNotify(pkt.getData<RechargeFinishNotify>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.RefreshRechargeTab)
			{
				return OnRefreshRechargeTab();
			}
			if (ntf.type == NotifyTypes.SelectRechargeItem)
			{
				return OnSelectRechargeItem((string)ntf.body);
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_Recharge").GetComponent<Button>(), OnRechargeTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_Reward").GetComponent<Button>(), OnWelfareTabBtnClick);
			BindViewCallback(base.view.transform.Find("CartInfoPanel/BuyBtn").GetComponent<Button>(), OnBuyBtnClick);
			BindViewCallback(base.view.transform.Find("CartInfoPanel/Info/AndroidPayBtns/AilpayBtn").GetComponent<Button>(), OnAilpayBtnClick);
			BindViewCallback(base.view.transform.Find("CartInfoPanel/Info/AndroidPayBtns/WechatBtn").GetComponent<Button>(), OnWechatBtnClick);
		}

		protected override bool SetupView()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string searchKey = ((!string.IsNullOrEmpty(showingTabKey)) ? showingTabKey : defaultTab);
			_tabManager.Clear();
			SetupRechargeTab();
			SetupWelfareTab();
			_tabManager.ShowTab(searchKey);
			TrySetInfoPanel();
			base.view.transform.Find("ChargeHCoinPanel").gameObject.SetActive(false);
			if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
			{
				base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in");
			}
			return false;
		}

		public override void BackPage()
		{
			base.BackPage();
		}

		public override void BackToMainMenuPage()
		{
			base.BackToMainMenuPage();
		}

		public void OnRechargeTabBtnClick()
		{
			_tabManager.ShowTab("RechargeTab");
			base.view.transform.Find("RechargeTab").GetComponent<MonoShopRechargeTab>().SetupView();
			OnEnterRechargeTab();
			base.view.transform.Find("CartInfoPanel").gameObject.SetActive(true);
			base.view.transform.Find("ChargeHCoinPanel").gameObject.SetActive(false);
			TrySetInfoPanel();
		}

		public void OnWelfareTabBtnClick()
		{
			_tabManager.ShowTab("RewardTab");
			base.view.transform.Find("RewardTab").GetComponent<MonoShopWelfareTab>().SetupView(RecordPlayerLevel);
			base.view.transform.Find("CartInfoPanel").gameObject.SetActive(false);
			base.view.transform.Find("ChargeHCoinPanel").gameObject.SetActive(true);
			TrySetInfoPanel();
		}

		public void OnBuyBtnClick()
		{
			if (Singleton<AccountManager>.Instance.manager.IsAccountBind() || Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
			{
				DoBuyHcoin();
				return;
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
				desc = LocalizationGeneralLogic.GetText("Menu_Action_TouristeCharge"),
				okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_DoBindAccount"),
				cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Action_ContinueRecharge"),
				notDestroyAfterTouchBG = true,
				buttonCallBack = delegate(bool confirmed)
				{
					if (confirmed)
					{
						Singleton<MainUIManager>.Instance.ShowPage(new PlayerProfilePageContext(PlayerProfilePageContext.TabType.AccountTab));
					}
					else
					{
						DoBuyHcoin();
					}
				}
			});
		}

		public void OnAilpayBtnClick()
		{
			base.view.transform.Find("RechargeTab").GetComponent<MonoShopRechargeTab>().SetPayMethodId(ChannelPayModule.PayMethod.ALIPAY);
		}

		public void OnWechatBtnClick()
		{
			base.view.transform.Find("RechargeTab").GetComponent<MonoShopRechargeTab>().SetPayMethodId(ChannelPayModule.PayMethod.WEIXIN_PAY);
		}

		private void DoBuyHcoin()
		{
			RecordPlayerLevel();
			string showingTabKey = _tabManager.GetShowingTabKey();
			if (!(showingTabKey == "RechargeTab"))
			{
				return;
			}
			Transform transform = base.view.transform.Find("RechargeTab");
			if (transform != null)
			{
				MonoShopRechargeTab component = transform.GetComponent<MonoShopRechargeTab>();
				if (component != null)
				{
					component.OnPucharseProduct();
				}
			}
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.transform.Find("Image").GetComponent<Image>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			go.SetActive(active);
		}

		private void SetupRechargeTab()
		{
			GameObject gameObject = base.view.transform.Find("RechargeTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/TabBtn_Recharge").GetComponent<Button>();
			_tabManager.SetTab("RechargeTab", component, gameObject);
			if (defaultTab == "RechargeTab")
			{
				gameObject.GetComponent<MonoShopRechargeTab>().SetupView();
				OnEnterRechargeTab();
			}
		}

		private void SetupWelfareTab()
		{
			GameObject gameObject = base.view.transform.Find("RewardTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/TabBtn_Reward").GetComponent<Button>();
			_tabManager.SetTab("RewardTab", component, gameObject);
			if (defaultTab == "RewardTab")
			{
				gameObject.GetComponent<MonoShopWelfareTab>().SetupView(RecordPlayerLevel);
			}
			SetupWelfareHint();
		}

		private bool OnRefreshRechargeTab()
		{
			GameObject gameObject = base.view.transform.Find("RechargeTab").gameObject;
			gameObject.GetComponent<MonoShopRechargeTab>().OnRefreshRechargeTab();
			return false;
		}

		private bool OnSelectRechargeItem(string productID)
		{
			Transform transform = base.view.transform.Find("RechargeTab");
			if (transform != null)
			{
				MonoShopRechargeTab component = transform.GetComponent<MonoShopRechargeTab>();
				if (component != null)
				{
					component.OnSelectProduct(productID);
				}
			}
			return false;
		}

		private void TrySetInfoPanel()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			bool active = showingTabKey == "RechargeTab";
			base.view.transform.Find("CartInfoPanel").gameObject.SetActive(active);
			base.view.transform.Find("CartInfoPanel/Info").gameObject.SetActive(false);
			base.view.transform.Find("ChargeHCoinPanel/HCoin/Num").GetComponent<Text>().text = Singleton<ShopWelfareModule>.Instance.totalPayHCoin.ToString();
		}

		private void OnEnterRechargeTab()
		{
		}

		private bool OnGetVipRewardDataRsp(GetVipRewardDataRsp rsp)
		{
			GameObject gameObject = base.view.transform.Find("RewardTab").gameObject;
			gameObject.GetComponent<MonoShopWelfareTab>().SetupView(RecordPlayerLevel);
			return false;
		}

		private bool OnGetVipRewardRsp(GetVipRewardRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && rsp.reward_list.Count > 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new RewardGotDialogContext(rsp.reward_list[0], _playerLevelBefore, null, "Menu_ShopWelfareRewardDlgTitle", "SpriteOutput/ShopIcons/WelfareIcon"));
			}
			return false;
		}

		private bool OnManualRefreshShopRsp(ManualRefreshShopRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopManualRefreshSuccess")));
			}
			else
			{
				GeneralDialogContext generalDialogContext = new GeneralDialogContext();
				generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
				generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
				GeneralDialogContext generalDialogContext2 = generalDialogContext;
				generalDialogContext2.desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				if (!string.IsNullOrEmpty(generalDialogContext2.desc))
				{
					Singleton<MainUIManager>.Instance.ShowDialog(generalDialogContext2);
				}
			}
			return false;
		}

		private bool OnRechargeSuccNotify(RechargeFinishNotify rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
			}
			return false;
		}

		private void RecordPlayerLevel()
		{
			_playerLevelBefore = Singleton<PlayerModule>.Instance.playerData.teamLevel;
		}

		private bool SetupWelfareHint()
		{
			bool active = Singleton<ShopWelfareModule>.Instance.HasWelfareCanGet();
			base.view.transform.Find("TabBtns/TabBtn_Reward/PopUp").gameObject.SetActive(active);
			return false;
		}
	}
}
