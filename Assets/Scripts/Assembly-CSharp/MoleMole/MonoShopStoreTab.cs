using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoShopStoreTab : MonoBehaviour
	{
		private const string ACTIVITY_SHOP_ITEM_BUTTON_PREFAB_PATH = "UI/Menus/Widget/Shop/ActivityItemButton";

		private Transform _scrollViewTrans;

		private int _currentSelectedGoodsID;

		private StoreDataItem _storeDataItem;

		private bool _hasPlayAnim;

		private UIShopType _shopType;

		public void SetupView(UIShopType shopType, StoreDataItem storeDataItem, Text tabText, bool playAnim = true, bool clearCurrentSelectGoods = true)
		{
			_shopType = shopType;
			if (clearCurrentSelectGoods)
			{
				_currentSelectedGoodsID = 0;
				base.transform.parent.Find("CartInfoPanel/Info").gameObject.SetActive(false);
			}
			_scrollViewTrans = base.transform.Find("ScrollView");
			_storeDataItem = storeDataItem;
			SetShopIsOpen();
			tabText.text = LocalizationGeneralLogic.GetText(_storeDataItem.shopNameTextID);
			SetupMetalNum();
			SetupAutoRefreshInfo();
			SetupManualRefreshInfo();
			SetupGoodsItem(playAnim);
			base.transform.Find("SpecialDesc").gameObject.SetActive(false);
		}

		public void OnManualRefresh()
		{
			if (_storeDataItem.manualRefreshTimes >= _storeDataItem.maxManualRefreshTimes)
			{
				return;
			}
			string text = LocalizationGeneralLogic.GetText("Menu_Hcoin");
			if (_storeDataItem.refreshItemID == 0)
			{
				if (_storeDataItem.nextRefreshCost > Singleton<PlayerModule>.Instance.playerData.hcoin)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopManualRefreshLackHCoin", text)));
					return;
				}
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.DoubleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_RefreshShopHint", _storeDataItem.nextRefreshCost, text),
					buttonCallBack = DoRequestToRefresh
				});
				return;
			}
			int num = 0;
			StorageDataItemBase storageDataItemBase = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(_storeDataItem.refreshItemID);
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_storeDataItem.refreshItemID);
			if (storageDataItemBase != null)
			{
				num = storageDataItemBase.number;
			}
			if (_storeDataItem.nextRefreshCost > num)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopManualRefreshLackHCoin", dummyStorageDataItem.GetDisplayTitle())));
				return;
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
				desc = LocalizationGeneralLogic.GetText("Menu_Desc_RefreshShopHint", _storeDataItem.nextRefreshCost, dummyStorageDataItem.GetDisplayTitle()),
				buttonCallBack = DoRequestToRefresh
			});
		}

		public void OnBuyGoods()
		{
			if (!CanBuyGoods())
			{
				return;
			}
			if (_shopType == UIShopType.SHOP_GACHATICKET)
			{
				int ticketID = _currentSelectedGoodsID;
				int num = 1;
				if (Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(_currentSelectedGoodsID / 10))
				{
					ticketID = _currentSelectedGoodsID / 10;
					num = 10;
				}
				Singleton<NetworkManager>.Instance.RequestBuyGachaTicket(ticketID, num);
			}
			else
			{
				Singleton<NetworkManager>.Instance.RequestBuyGoods(_storeDataItem.shopID, _currentSelectedGoodsID);
			}
		}

		public void OnSelectGoods(Goods goods)
		{
			ShopGoodsMetaData shopGoodsMetaData;
			if (_shopType == UIShopType.SHOP_GACHATICKET)
			{
				int goods_id = (int)goods.goods_id;
				int hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[(int)goods.goods_id];
				shopGoodsMetaData = new ShopGoodsMetaData((int)goods.goods_id, (int)goods.goods_id, 1, 1, hCoinCost, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, int.MaxValue, 1, 10000, false);
				if (Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(goods_id / 10))
				{
					hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[(int)goods.goods_id / 10];
					shopGoodsMetaData = new ShopGoodsMetaData(goods_id, goods_id / 10, 1, 10, hCoinCost * 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, int.MaxValue, 1, 10000, false);
				}
			}
			else
			{
				shopGoodsMetaData = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int)goods.goods_id);
			}
			if (shopGoodsMetaData == null)
			{
				return;
			}
			_currentSelectedGoodsID = (int)goods.goods_id;
			if (_shopType == UIShopType.SHOP_ACTIVITY)
			{
				RefreshActivityScroller();
			}
			else
			{
				_scrollViewTrans.GetComponent<MonoGridScroller>().RefreshCurrent();
			}
			base.transform.parent.Find("CartInfoPanel/Info").gameObject.SetActive(true);
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(shopGoodsMetaData.ItemID, shopGoodsMetaData.ItemLevel);
			string empty = string.Empty;
			if (goods.buy_times >= shopGoodsMetaData.MaxBuyTimes)
			{
				empty = LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostLackMoney");
				base.transform.parent.Find("CartInfoPanel/BuyBtn").GetComponent<Button>().interactable = false;
			}
			else
			{
				base.transform.parent.Find("CartInfoPanel/BuyBtn").GetComponent<Button>().interactable = true;
				empty = ((shopGoodsMetaData.ItemNum <= 1) ? LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostDescOne", dummyStorageDataItem.GetDisplayTitle()) : LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostDescMulti", dummyStorageDataItem.GetDisplayTitle(), shopGoodsMetaData.ItemNum));
				if (_shopType != UIShopType.SHOP_GACHATICKET)
				{
					int num = shopGoodsMetaData.MaxBuyTimes - (int)goods.buy_times;
					empty = ((!goods.can_be_refreshSpecified || !goods.can_be_refresh) ? (empty + LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostRemainTotal", num)) : (empty + LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostRemainToday", num)));
				}
			}
			base.transform.parent.Find("CartInfoPanel/Info/Desc").GetComponent<Text>().text = empty;
		}

		public void OnBuyGoodsRsp(BuyGoodsRsp rsp)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			_storeDataItem = Singleton<StoreModule>.Instance.GetStoreDateItemByID(_storeDataItem.shopID);
			if ((int)rsp.retcode != 0)
			{
				return;
			}
			if (_shopType == UIShopType.SHOP_ACTIVITY)
			{
				RefreshActivityScroller();
			}
			else
			{
				_scrollViewTrans.GetComponent<MonoGridScroller>().RefreshCurrent();
			}
			if (_currentSelectedGoodsID != 0)
			{
				Goods goodsByID = GetGoodsByID(_currentSelectedGoodsID);
				if (goodsByID != null)
				{
					OnSelectGoods(goodsByID);
				}
			}
		}

		private void OnScrollChange(Transform trans, int index)
		{
			Goods val = _storeDataItem.goodsList[index];
			trans.GetComponent<MonoStoreGoodsItem>().SetupView(val, val.goods_id == _currentSelectedGoodsID, (int)((_shopType == UIShopType.SHOP_GACHATICKET) ? val.goods_id : 0));
		}

		private bool CanBuyGoods()
		{
			if (_currentSelectedGoodsID == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopNoSelect")));
				return false;
			}
			Goods goodsByID = GetGoodsByID(_currentSelectedGoodsID);
			if (goodsByID == null)
			{
				return false;
			}
			ShopGoodsMetaData shopGoodsMetaData;
			if (_shopType == UIShopType.SHOP_GACHATICKET)
			{
				int hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[(int)goodsByID.goods_id];
				shopGoodsMetaData = new ShopGoodsMetaData((int)goodsByID.goods_id, (int)goodsByID.goods_id, 1, 1, hCoinCost, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, int.MaxValue, 1, 10000, false);
			}
			else
			{
				shopGoodsMetaData = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int)goodsByID.goods_id);
			}
			if (goodsByID.buy_times >= shopGoodsMetaData.MaxBuyTimes)
			{
				return false;
			}
			List<int> list2;
			if (_shopType == UIShopType.SHOP_GACHATICKET)
			{
				List<int> list = new List<int>();
				list.Add(Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[(int)goodsByID.goods_id]);
				list2 = list;
			}
			else
			{
				list2 = UIUtil.GetGoodsRealPrice(goodsByID);
			}
			List<int> list3 = list2;
			int num = 0;
			if (shopGoodsMetaData.HCoinCost > 0)
			{
				if (list3[num] > Singleton<PlayerModule>.Instance.playerData.hcoin)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackHCoin")));
					return false;
				}
				num++;
			}
			if (shopGoodsMetaData.SCoinCost > 0)
			{
				if (list3[num] > Singleton<PlayerModule>.Instance.playerData.scoin)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin")));
					return false;
				}
				num++;
			}
			if (shopGoodsMetaData.CostItemNum > 0)
			{
				StorageDataItemBase storageItemByTypeAndID = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaData.CostItemId);
				if (storageItemByTypeAndID == null || list3[num] > storageItemByTypeAndID.number)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin")));
					return false;
				}
				num++;
			}
			if (shopGoodsMetaData.CostItemNum2 > 0)
			{
				StorageDataItemBase storageItemByTypeAndID2 = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaData.CostItemId2);
				if (storageItemByTypeAndID2 == null || list3[num] > storageItemByTypeAndID2.number)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin")));
					return false;
				}
				num++;
			}
			if (shopGoodsMetaData.CostItemNum3 > 0)
			{
				StorageDataItemBase storageItemByTypeAndID3 = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaData.CostItemId3);
				if (storageItemByTypeAndID3 == null || list3[num] > storageItemByTypeAndID3.number)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin")));
					return false;
				}
				num++;
			}
			if (shopGoodsMetaData.CostItemNum4 > 0)
			{
				StorageDataItemBase storageItemByTypeAndID4 = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaData.CostItemId4);
				if (storageItemByTypeAndID4 == null || list3[num] > storageItemByTypeAndID4.number)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin")));
					return false;
				}
				num++;
			}
			if (shopGoodsMetaData.CostItemNum5 > 0)
			{
				StorageDataItemBase storageItemByTypeAndID5 = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaData.CostItemId5);
				if (storageItemByTypeAndID5 == null || list3[num] > storageItemByTypeAndID5.number)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin")));
					return false;
				}
				num++;
			}
			if (Singleton<StorageModule>.Instance.IsFull())
			{
				GeneralDialogContext generalDialogContext = new GeneralDialogContext();
				generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
				generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
				generalDialogContext.desc = LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsBagFull");
				GeneralDialogContext dialogContext = generalDialogContext;
				Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
				return false;
			}
			return true;
		}

		private Goods GetGoodsByID(int goodsID)
		{
			foreach (Goods goods in _storeDataItem.goodsList)
			{
				if (goods.goods_id == (uint)goodsID)
				{
					return goods;
				}
			}
			return null;
		}

		private void SetupMetalNum()
		{
			base.transform.Find("SpecialDesc").gameObject.SetActive(true);
			Transform transform = base.transform.Find("SystemInfoPanel/Currency");
			switch (_shopType)
			{
			case UIShopType.SHOP_GACHATICKET:
				base.transform.Find("SystemInfoPanel/Currency").gameObject.SetActive(false);
				base.transform.Find("SystemInfoPanel/ActivityCurrency").gameObject.SetActive(false);
				return;
			case UIShopType.SHOP_ACTIVITY:
				base.transform.Find("SpecialDesc").gameObject.SetActive(false);
				base.transform.Find("SystemInfoPanel/Currency").gameObject.SetActive(false);
				transform = base.transform.Find("SystemInfoPanel/ActivityCurrency");
				transform.gameObject.SetActive(true);
				break;
			default:
				base.transform.Find("SystemInfoPanel/ActivityCurrency").gameObject.SetActive(false);
				transform.gameObject.SetActive(true);
				break;
			}
			transform.gameObject.SetActive(_storeDataItem.currencyIDList.Count > 0);
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (i >= _storeDataItem.currencyIDList.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				child.gameObject.SetActive(true);
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_storeDataItem.currencyIDList[i]);
				string text = MiscData.GetCurrencyIconPath(dummyStorageDataItem.ID);
				if (string.IsNullOrEmpty(text))
				{
					text = dummyStorageDataItem.GetIconPath();
				}
				child.Find("ImgMetal").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text);
				List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.TryGetStorageDataItemByMetaId(_storeDataItem.currencyIDList[i]);
				int num = 0;
				if (list.Count > 0)
				{
					num = ((!(dummyStorageDataItem is MaterialDataItem)) ? list.Count : list[0].number);
				}
				child.Find("Num").GetComponent<Text>().text = num.ToString();
			}
		}

		private void SetupAutoRefreshInfo()
		{
			base.transform.Find("SystemInfoPanel/SystemRefresh").gameObject.SetActive(_shopType != UIShopType.SHOP_GACHATICKET);
			if (_shopType == UIShopType.SHOP_GACHATICKET)
			{
				return;
			}
			bool flag = true;
			DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(_storeDataItem.nextAutoRefreshTime);
			if (_shopType == UIShopType.SHOP_ACTIVITY)
			{
				if (_storeDataItem.scheduleChangeTime != 0)
				{
					dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(_storeDataItem.scheduleChangeTime);
				}
				else
				{
					flag = false;
				}
			}
			base.transform.Find("SystemInfoPanel/SystemRefresh").gameObject.SetActive(flag);
			if (flag)
			{
				base.transform.Find("SystemInfoPanel/SystemRefresh/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(dateTimeFromTimeStamp, null, OnAutoRefreshTimeOutCallback);
			}
		}

		private void SetupManualRefreshInfo()
		{
			base.transform.Find("RefreshTab").gameObject.SetActive(_shopType == UIShopType.SHOP_NORMAL || _shopType == UIShopType.SHOP_ACTIVITY);
			if (_shopType == UIShopType.SHOP_GACHATICKET || _shopType == UIShopType.SHOP_ENDLESS)
			{
				return;
			}
			if (_shopType == UIShopType.SHOP_ACTIVITY)
			{
				base.transform.Find("RefreshTab").gameObject.SetActive(_storeDataItem.isOpen);
				if (!_storeDataItem.isOpen)
				{
					return;
				}
			}
			base.transform.Find("RefreshTab/RefreshTime/Num").GetComponent<Text>().text = (_storeDataItem.maxManualRefreshTimes - _storeDataItem.manualRefreshTimes).ToString();
			base.transform.Find("RefreshTab/MetalNum/Num").GetComponent<Text>().text = _storeDataItem.nextRefreshCost.ToString();
			base.transform.Find("RefreshTab/MetalNum/Num").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
			base.transform.Find("RefreshTab/MetalNum/x").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
			if (_storeDataItem.refreshItemID == 0)
			{
				base.transform.Find("RefreshTab/MetalNum/ImgMetal").GetComponent<Image>().sprite = UIUtil.GetResourceSprite(ResourceType.Hcoin);
				if (Singleton<PlayerModule>.Instance.playerData.hcoin < _storeDataItem.nextRefreshCost)
				{
					base.transform.Find("RefreshTab/MetalNum/Num").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
					base.transform.Find("RefreshTab/MetalNum/x").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
				}
			}
			else
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_storeDataItem.refreshItemID);
				string prefabPath = dummyStorageDataItem.GetIconPath();
				if (MiscData.GetCurrencyIconPath(_storeDataItem.refreshItemID) != null)
				{
					prefabPath = MiscData.GetCurrencyIconPath(_storeDataItem.refreshItemID);
				}
				base.transform.Find("RefreshTab/MetalNum/ImgMetal").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
				int num = 0;
				StorageDataItemBase storageDataItemBase = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(_storeDataItem.refreshItemID);
				if (storageDataItemBase != null)
				{
					num = storageDataItemBase.number;
				}
				if (num < _storeDataItem.nextRefreshCost)
				{
					base.transform.Find("RefreshTab/MetalNum/Num").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
					base.transform.Find("RefreshTab/MetalNum/x").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
				}
			}
			if (_storeDataItem.manualRefreshTimes >= _storeDataItem.maxManualRefreshTimes)
			{
				base.transform.Find("RefreshTab/RefreshBtn").GetComponent<Button>().interactable = false;
			}
			else
			{
				base.transform.Find("RefreshTab/RefreshBtn").GetComponent<Button>().interactable = true;
			}
		}

		private void SetupGoodsItem(bool playAnim)
		{
			if (_shopType == UIShopType.SHOP_ACTIVITY)
			{
				Transform transform = base.transform.Find("ScrollViewActivity");
				Transform transform2 = base.transform.Find("ScrollViewActivity/Content");
				transform.gameObject.SetActive(true);
				base.transform.Find("ScrollView").gameObject.SetActive(false);
				if (!_storeDataItem.isOpen)
				{
					transform.gameObject.SetActive(false);
					return;
				}
				int num = _storeDataItem.goodsList.Count - transform2.childCount;
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						Transform transform3 = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Shop/ActivityItemButton")).transform;
						transform3.SetParent(transform2, false);
					}
				}
				else if (num < 0)
				{
					num = -num;
					for (int j = 0; j < num; j++)
					{
						UnityEngine.Object.Destroy(transform2.GetChild(j));
					}
				}
				RefreshActivityScroller();
			}
			else
			{
				base.transform.Find("ScrollViewActivity").gameObject.SetActive(false);
				base.transform.Find("ScrollView").gameObject.SetActive(true);
				MonoGridScroller component = _scrollViewTrans.GetComponent<MonoGridScroller>();
				component.Init(OnScrollChange, _storeDataItem.goodsList.Count, new Vector2(0f, 0f));
				_scrollViewTrans.gameObject.SetActive(true);
				if (playAnim && !_hasPlayAnim)
				{
					base.transform.Find("ScrollView/Content").GetComponent<Animation>().Play("GoodsItemsFadeIn");
					_hasPlayAnim = true;
				}
			}
		}

		private void OnAutoRefreshTimeOutCallback()
		{
			if (Singleton<NetworkManager>.Instance != null)
			{
				Singleton<NetworkManager>.Instance.RequestGetShopList();
			}
		}

		private void SetShopIsOpen()
		{
			bool isOpen = _storeDataItem.isOpen;
			base.transform.Find("ShopUnOpen").gameObject.SetActive(!isOpen);
			base.transform.Find("ArrowLeft").gameObject.SetActive(isOpen);
			base.transform.Find("ArrowRight").gameObject.SetActive(isOpen);
			base.transform.Find("ScrollView").gameObject.SetActive(isOpen);
			base.transform.Find("ScrollViewActivity").gameObject.SetActive(isOpen);
			base.transform.Find("RefreshTab").gameObject.SetActive(isOpen);
			base.transform.Find("SpecialDesc").gameObject.SetActive(isOpen);
		}

		private void OnDestroy()
		{
			_currentSelectedGoodsID = 0;
		}

		private void OnDisable()
		{
			_currentSelectedGoodsID = 0;
		}

		private void DoRequestToRefresh(bool confirmed)
		{
			if (confirmed)
			{
				Singleton<NetworkManager>.Instance.RequestManualRefreshShop(_storeDataItem.shopID);
			}
		}

		private void RefreshActivityScroller()
		{
			Transform transform = base.transform.Find("ScrollViewActivity/Content");
			for (int i = 0; i < _storeDataItem.goodsList.Count; i++)
			{
				Transform child = transform.GetChild(i);
				Goods val = _storeDataItem.goodsList[i];
				child.GetComponent<MonoStoreGoodsItem>().SetupView(val, val.goods_id == _currentSelectedGoodsID, 0, true);
			}
		}
	}
}
