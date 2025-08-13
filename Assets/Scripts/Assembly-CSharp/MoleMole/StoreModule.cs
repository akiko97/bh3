using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class StoreModule : BaseModule
	{
		private List<StoreDataItem> _storeDataItemList;

		private Dictionary<UIShopType, StoreDataItem> _shopDict;

		public StoreModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_storeDataItemList = new List<StoreDataItem>();
			_shopDict = new Dictionary<UIShopType, StoreDataItem>();
		}

		public List<StoreDataItem> GetStoreDataItemList()
		{
			return _storeDataItemList;
		}

		public StoreDataItem GetStoreDateItemByID(int shopID)
		{
			foreach (StoreDataItem storeDataItem in _storeDataItemList)
			{
				if (storeDataItem.shopID == shopID)
				{
					return storeDataItem;
				}
			}
			return null;
		}

		public StoreDataItem GetStoreDataByType(UIShopType shopType)
		{
			if (_shopDict.ContainsKey(shopType))
			{
				return _shopDict[shopType];
			}
			return null;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 202:
				return OnGetShopListRsp(pkt.getData<GetShopListRsp>());
			case 204:
				return OnBuyGoodsRsp(pkt.getData<BuyGoodsRsp>());
			default:
				return false;
			}
		}

		private bool OnGetShopListRsp(GetShopListRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				MergeStoreDataItemList(rsp.shop_list);
			}
			return false;
		}

		private void MergeStoreDataItemList(List<Shop> shopList)
		{
			foreach (Shop shop in shopList)
			{
				StoreDataItem storeDateItemByID = GetStoreDateItemByID((int)shop.shop_id);
				if (storeDateItemByID == null)
				{
					StoreDataItem storeDataItem = new StoreDataItem(shop);
					_storeDataItemList.Add(storeDataItem);
					_shopDict[(UIShopType)shop.shop_type] = storeDataItem;
					if (storeDataItem.shopType == UIShopType.SHOP_ACTIVITY && storeDataItem.isOpen)
					{
						Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ActivtyShopScheduleChange, true));
					}
					continue;
				}
				bool flag = false;
				if (storeDateItemByID.shopType == UIShopType.SHOP_ACTIVITY && storeDateItemByID.isOpen != shop.is_open)
				{
					flag = true;
				}
				storeDateItemByID.UpdateFromShop(shop);
				if (flag)
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ActivtyShopScheduleChange, shop.is_open));
				}
			}
		}

		private bool OnBuyGoodsRsp(BuyGoodsRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Goods goods = GetGoods((int)rsp.shop_id, (int)rsp.goods_id);
				if (goods != null)
				{
					goods.buy_times = rsp.goods_buy_times;
				}
				Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
			}
			return false;
		}

		private Goods GetGoods(int shopID, int goodsID)
		{
			foreach (StoreDataItem storeDataItem in _storeDataItemList)
			{
				if (storeDataItem.shopID != shopID)
				{
					continue;
				}
				foreach (Goods goods in storeDataItem.goodsList)
				{
					if (goods.goods_id == (uint)goodsID)
					{
						return goods;
					}
				}
			}
			return null;
		}
	}
}
