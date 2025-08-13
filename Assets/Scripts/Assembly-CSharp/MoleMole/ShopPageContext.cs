using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class ShopPageContext : BasePageContext
	{
		private const string TAB_SELECT_BG_PATH = "SpriteOutput/GeneralUI/TabDialogSelected";

		private const string TAB_UNSELECT_BG_PATH = "SpriteOutput/GeneralUI/TabDialogUnselected";

		private const string StoreTab = "ShopTab";

		private UIShopType _currentShopType;

		public ShopPageContext(UIShopType shopType = UIShopType.SHOP_NORMAL)
		{
			config = new ContextPattern
			{
				contextName = "ShopPageContext",
				viewPrefabPath = "UI/Menus/Page/Shop/ShopOverviewPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			showSpaceShip = false;
			_currentShopType = shopType;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 202:
				return OnGetShopListRsp(pkt.getData<GetShopListRsp>());
			case 204:
				return OnBuyGoodsRsp(pkt.getData<BuyGoodsRsp>());
			case 206:
				return OnManualRefreshShopRsp(pkt.getData<ManualRefreshShopRsp>());
			case 11:
				return OnGetMainDataRsp(pkt.getData<GetMainDataRsp>());
			case 216:
				return OnBuyGachaTicket(pkt.getData<BuyGachaTicketRsp>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			if (ntf.type == NotifyTypes.SelectStoreGoodsItem)
			{
				return OnSelectStoreGoodsItem((Goods)ntf.body);
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("CartInfoPanel/BuyBtn").GetComponent<Button>(), OnBuyBtnClick);
			BindViewCallback(base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>(), SetupNormalStoreTab);
			BindViewCallback(base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>(), SetupEndlessStoreTab);
			BindViewCallback(base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>(), SetupGachaTicketStoreTab);
			BindViewCallback(base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>(), SetupActivityStoreTab);
		}

		protected override bool SetupView()
		{
			bool flag = Singleton<PlayerModule>.Instance.playerData.teamLevel >= MiscData.Config.BasicConfig.GachaUnlockNeedPlayerLevel;
			if (!flag && _currentShopType == UIShopType.SHOP_GACHATICKET)
			{
				_currentShopType = UIShopType.SHOP_NORMAL;
			}
			switch (_currentShopType)
			{
			case UIShopType.SHOP_NORMAL:
				SetupNormalStoreTab();
				break;
			case UIShopType.SHOP_ENDLESS:
				SetupEndlessStoreTab();
				break;
			case UIShopType.SHOP_ACTIVITY:
				SetupActivityStoreTab();
				break;
			case UIShopType.SHOP_GACHATICKET:
				SetupGachaTicketStoreTab();
				break;
			}
			base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").gameObject.SetActive(flag);
			TrySetCartInfoPanel();
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

		public void OnBuyBtnClick()
		{
			GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
			gameObject.GetComponent<MonoShopStoreTab>().OnBuyGoods();
		}

		private void SetupNormalStoreTab()
		{
			_currentShopType = UIShopType.SHOP_NORMAL;
			GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
			StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_NORMAL);
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>().interactable = false;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>().color = Color.white;
			if (storeDataByType != null)
			{
				base.view.transform.Find("ShopTab").GetComponent<MonoShopStoreTab>().SetupView(UIShopType.SHOP_NORMAL, storeDataByType, gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>());
			}
		}

		private void SetupEndlessStoreTab()
		{
			_currentShopType = UIShopType.SHOP_ENDLESS;
			GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
			StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_ENDLESS);
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>().interactable = false;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>().color = Color.white;
			if (storeDataByType != null)
			{
				base.view.transform.Find("ShopTab").GetComponent<MonoShopStoreTab>().SetupView(UIShopType.SHOP_ENDLESS, storeDataByType, gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>());
			}
		}

		private void SetupGachaTicketStoreTab()
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			_currentShopType = UIShopType.SHOP_GACHATICKET;
			GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
			List<Goods> list = new List<Goods>();
			Dictionary<int, int> gachaTicketPriceDict = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict;
			foreach (int key in gachaTicketPriceDict.Keys)
			{
				Goods val = new Goods();
				val.goods_id = (uint)key;
				list.Add(val);
			}
			StoreDataItem storeDataItem = new StoreDataItem(true, "Menu_Label_GachaTicket", "Menu_Label_GachaTicket", list);
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>().interactable = false;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>().color = Color.white;
			if (storeDataItem != null)
			{
				base.view.transform.Find("ShopTab").GetComponent<MonoShopStoreTab>().SetupView(UIShopType.SHOP_GACHATICKET, storeDataItem, gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>());
			}
		}

		private void SetupActivityStoreTab()
		{
			_currentShopType = UIShopType.SHOP_ACTIVITY;
			GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
			StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_ACTIVITY);
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>().interactable = true;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>().interactable = false;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>().color = Color.white;
			gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>().color = Color.white;
			if (storeDataByType != null)
			{
				base.view.transform.Find("ShopTab").GetComponent<MonoShopStoreTab>().SetupView(UIShopType.SHOP_ACTIVITY, storeDataByType, gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>());
			}
		}

		private bool OnSelectStoreGoodsItem(Goods goods)
		{
			Transform transform = base.view.transform.Find("ShopTab");
			if (transform != null)
			{
				MonoShopStoreTab component = transform.GetComponent<MonoShopStoreTab>();
				if (component != null)
				{
					component.OnSelectGoods(goods);
				}
			}
			return false;
		}

		private void TrySetCartInfoPanel()
		{
			base.view.transform.Find("CartInfoPanel").gameObject.SetActive(true);
			base.view.transform.Find("CartInfoPanel/Info").gameObject.SetActive(false);
		}

		private void OnEnterRechargeTab()
		{
		}

		private bool OnGetShopListRsp(GetShopListRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				SetupView();
			}
			return false;
		}

		private bool OnBuyGoodsRsp(BuyGoodsRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Invalid comparison between Unknown and I4
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode == 0)
			{
				SetupView();
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsSuccess")));
				GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
				gameObject.GetComponent<MonoShopStoreTab>().OnBuyGoodsRsp(rsp);
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
				if ((int)rsp.retcode == 6 || (int)rsp.retcode == 7)
				{
					Singleton<NetworkManager>.Instance.RequestGetShopList();
				}
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

		private bool OnGetMainDataRsp(GetMainDataRsp rsp)
		{
			if (rsp.hcoinSpecified || rsp.scoinSpecified)
			{
				SetupView();
			}
			return false;
		}

		private bool OnBuyGachaTicket(BuyGachaTicketRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)rsp.material_id);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_BuyGachaTicketSuccess", dummyStorageDataItem.GetDisplayTitle(), (int)rsp.num)));
				Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
				return SetupView();
			}
			return false;
		}
	}
}
