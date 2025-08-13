using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class StoreDataItem
	{
		public int shopID;

		public bool isOpen;

		public UIShopType shopType = UIShopType.SHOP_GACHATICKET;

		public string shopNameTextID;

		public string shopButtonNameTextID;

		public List<Goods> goodsList;

		public uint nextAutoRefreshTime;

		public int manualRefreshTimes;

		public uint scheduleChangeTime;

		public int refreshItemID;

		public int nextRefreshCost;

		public int maxManualRefreshTimes;

		public List<int> currencyIDList;

		public StoreDataItem(Shop shopDataItem)
		{
			shopID = (int)shopDataItem.shop_id;
			isOpen = shopDataItem.is_open;
			if (shopDataItem.shop_typeSpecified)
			{
				shopType = (UIShopType)shopDataItem.shop_type;
			}
			shopNameTextID = shopDataItem.text_map_name;
			shopButtonNameTextID = shopDataItem.text_map_button_name;
			goodsList = shopDataItem.goods_list;
			nextAutoRefreshTime = shopDataItem.next_auto_refresh_time;
			scheduleChangeTime = shopDataItem.schedule_change_time;
			manualRefreshTimes = (int)shopDataItem.manual_refresh_times;
			refreshItemID = (int)shopDataItem.refresh_item;
			nextRefreshCost = (int)shopDataItem.next_refresh_cost;
			maxManualRefreshTimes = (int)shopDataItem.max_manual_refresh_times;
			currencyIDList = new List<int>();
			foreach (uint item in shopDataItem.currency_list)
			{
				currencyIDList.Add((int)item);
			}
		}

		public StoreDataItem(bool isOpen, string shopNameTextID, string shopButtonNameTextID, List<Goods> goodsList)
		{
			this.isOpen = isOpen;
			this.shopNameTextID = shopNameTextID;
			this.shopButtonNameTextID = shopButtonNameTextID;
			this.goodsList = goodsList;
		}

		public void UpdateFromShop(Shop shopDataItem)
		{
			shopID = (int)shopDataItem.shop_id;
			isOpen = shopDataItem.is_open;
			shopNameTextID = shopDataItem.text_map_name;
			shopButtonNameTextID = shopDataItem.text_map_button_name;
			goodsList = shopDataItem.goods_list;
			nextAutoRefreshTime = shopDataItem.next_auto_refresh_time;
			scheduleChangeTime = shopDataItem.schedule_change_time;
			manualRefreshTimes = (int)shopDataItem.manual_refresh_times;
			refreshItemID = (int)shopDataItem.refresh_item;
			nextRefreshCost = (int)shopDataItem.next_refresh_cost;
			maxManualRefreshTimes = (int)shopDataItem.max_manual_refresh_times;
			currencyIDList = new List<int>();
			foreach (uint item in shopDataItem.currency_list)
			{
				currencyIDList.Add((int)item);
			}
		}

		public override string ToString()
		{
			string text = string.Format("<StoreDataItem>\nID: {0}\nisOpen: {1}\nnameID: {2}\nnextAutoRefreshTime: {3}\nmanualRefreshTimes: {4}\nnextRefreshHCoinCost: {5}\nmaxManualRefreshTimes: {6}", shopID, isOpen, shopNameTextID, nextAutoRefreshTime, manualRefreshTimes, nextRefreshCost, maxManualRefreshTimes);
			text = text + "\ngoodsList: " + goodsList.Count + "\n";
			foreach (Goods goods in goodsList)
			{
				ShopGoodsMetaData shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int)goods.goods_id);
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(shopGoodsMetaDataByKey.ItemID, shopGoodsMetaDataByKey.ItemLevel);
				dummyStorageDataItem.number = shopGoodsMetaDataByKey.ItemNum;
				string text2 = text;
				text = text2 + "ID: " + goods.goods_id + " name: " + dummyStorageDataItem.GetDisplayTitle() + " level: " + shopGoodsMetaDataByKey.ItemLevel.ToString() + " number: " + shopGoodsMetaDataByKey.ItemNum.ToString() + " hcoinCost: " + shopGoodsMetaDataByKey.HCoinCost.ToString() + " scoinCost: " + shopGoodsMetaDataByKey.SCoinCost.ToString() + " maxBuyTimes: " + shopGoodsMetaDataByKey.MaxBuyTimes.ToString() + " buyTimes: " + goods.buy_times.ToString() + "\n";
			}
			return text;
		}
	}
}
