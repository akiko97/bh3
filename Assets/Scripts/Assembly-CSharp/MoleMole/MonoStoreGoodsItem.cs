using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoStoreGoodsItem : MonoBehaviour
	{
		private const string _salePanelDiscountSpritePath = "SpriteOutput/ShopIcons/SalePatternDiscount";

		private const string _salePanelSuperWorthSpritePath = "SpriteOutput/ShopIcons/SalePatternSuperWorth";

		private const string _salePanelNewSpritePath = "SpriteOutput/ShopIcons/SalePatternNew";

		private const string _salePanelGreySpritePath = "SpriteOutput/ShopIcons/SalePatternGrey";

		private Goods _goods;

		private int _ticketID;

		private bool _isMultiCurrency;

		public void SetupView(Goods goods, bool isSelected, int ticketID, bool isMultiCurrency = false)
		{
			_goods = goods;
			_ticketID = ticketID;
			_isMultiCurrency = isMultiCurrency;
			SetItemDefaultColor();
			base.transform.Find("BG/Selected").gameObject.SetActive(isSelected);
			base.transform.Find("BG/Unselected").gameObject.SetActive(!isSelected);
			ShopGoodsMetaData shopGoodsMetaData;
			if (_ticketID > 0)
			{
				int hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[_ticketID];
				shopGoodsMetaData = new ShopGoodsMetaData(_ticketID, _ticketID, 1, 1, hCoinCost, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, int.MaxValue, 1, 10000, false);
				if (Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(_ticketID / 10))
				{
					hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[_ticketID / 10];
					shopGoodsMetaData = new ShopGoodsMetaData(_ticketID, _ticketID / 10, 1, 10, hCoinCost * 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, int.MaxValue, 1, 10000, false);
				}
			}
			else
			{
				shopGoodsMetaData = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int)goods.goods_id);
			}
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(shopGoodsMetaData.ItemID, shopGoodsMetaData.ItemLevel);
			dummyStorageDataItem.number = shopGoodsMetaData.ItemNum;
			SetupSaleLabel(shopGoodsMetaData);
			base.transform.Find("FragmentIcon").gameObject.SetActive(dummyStorageDataItem is AvatarFragmentDataItem);
			Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
			base.transform.Find("ItemIcon/Icon").GetComponent<Image>().sprite = spriteByPrefab;
			base.transform.Find("ItemIcon/FrameBg").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[dummyStorageDataItem.rarity]);
			base.transform.Find("ItemIcon/FrameLight").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityLightImgPath[dummyStorageDataItem.rarity]);
			base.transform.Find("ItemIcon/FrameBg").gameObject.SetActive(true);
			base.transform.Find("ItemIcon/FrameLight").gameObject.SetActive(true);
			SetupDesc(dummyStorageDataItem);
			SetupRarityView(dummyStorageDataItem);
			SetupStigmataTypeIcon(dummyStorageDataItem);
			SetupPrice(shopGoodsMetaData);
			if (goods.buy_times >= shopGoodsMetaData.MaxBuyTimes)
			{
				SetItemGrey();
				return;
			}
			base.transform.Find("BG/Unselected/NowPrize").gameObject.SetActive(true);
			base.transform.Find("BG/Unselected/Empty").gameObject.SetActive(false);
			Color color = base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color;
			color.a = 1f;
			base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color = color;
			base.transform.Find("BG/Selected/NowPrize").gameObject.SetActive(true);
			base.transform.Find("BG/Selected/Empty").gameObject.SetActive(false);
			color = base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color;
			color.a = 1f;
			base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color = color;
			base.transform.Find("ItemIcon/SellOut").gameObject.SetActive(false);
		}

		public void OnClick()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectStoreGoodsItem, _goods));
			base.transform.Find("BG/Selected").gameObject.SetActive(true);
			base.transform.Find("BG/Unselected").gameObject.SetActive(false);
		}

		public void OnIconClick()
		{
			ShopGoodsMetaData shopGoodsMetaData;
			if (_ticketID > 0)
			{
				int hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[_ticketID];
				shopGoodsMetaData = new ShopGoodsMetaData(_ticketID, _ticketID, 1, 1, hCoinCost, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, int.MaxValue, 1, 10000, false);
				if (Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(_ticketID / 10))
				{
					hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[_ticketID / 10];
					shopGoodsMetaData = new ShopGoodsMetaData(_ticketID, _ticketID / 10, 1, 10, hCoinCost * 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, int.MaxValue, 1, 10000, false);
				}
			}
			else
			{
				shopGoodsMetaData = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int)_goods.goods_id);
			}
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(shopGoodsMetaData.ItemID, shopGoodsMetaData.ItemLevel);
			dummyStorageDataItem.number = shopGoodsMetaData.ItemNum;
			UIUtil.ShowItemDetail(dummyStorageDataItem, true);
			OnClick();
		}

		private void SetupDesc(StorageDataItemBase item)
		{
			if (item is WeaponDataItem || item is StigmataDataItem)
			{
				base.transform.Find("NumPanel").gameObject.SetActive(false);
				base.transform.Find("LevelPanel").gameObject.SetActive(true);
				base.transform.Find("LevelPanel/Num/Num").GetComponent<Text>().text = item.level.ToString();
			}
			else if (item is AvatarCardDataItem)
			{
				base.transform.Find("NumPanel").gameObject.SetActive(false);
				base.transform.Find("LevelPanel").gameObject.SetActive(true);
				AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.ID).avatarID);
				base.transform.Find("LevelPanel/Num/Num").GetComponent<Text>().text = dummyAvatarDataItem.level.ToString();
			}
			else
			{
				base.transform.Find("NumPanel").gameObject.SetActive(true);
				base.transform.Find("LevelPanel").gameObject.SetActive(false);
				base.transform.Find("NumPanel/Num/Num").GetComponent<Text>().text = item.number.ToString();
			}
		}

		private void SetupStigmataTypeIcon(StorageDataItemBase item)
		{
			base.transform.Find("StigmataType").gameObject.SetActive(item is StigmataDataItem);
			if (item is StigmataDataItem)
			{
				base.transform.Find("StigmataType/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.StigmataTypeIconPath[item.GetBaseType()]);
			}
		}

		private void SetupRarityView(StorageDataItemBase item)
		{
			base.transform.Find("AvatarStar").gameObject.SetActive(false);
			base.transform.Find("Star").gameObject.SetActive(false);
			string hexString = MiscData.Config.ItemRarityColorList[item.rarity];
			base.transform.Find("ItemIcon").GetComponent<Image>().color = Miscs.ParseColor(hexString);
			if (item is AvatarCardDataItem)
			{
				AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.ID).avatarID);
				SetupAvatarStar(dummyAvatarDataItem.star);
				base.transform.Find("AvatarStar").gameObject.SetActive(true);
			}
			else if (!(item is AvatarFragmentDataItem))
			{
				base.transform.Find("Star").gameObject.SetActive(true);
				int maxStar = item.rarity;
				if (item is WeaponDataItem)
				{
					maxStar = (item as WeaponDataItem).GetMaxRarity();
				}
				else if (item is StigmataDataItem)
				{
					maxStar = (item as StigmataDataItem).GetMaxRarity();
				}
				base.transform.Find("Star").GetComponent<MonoItemIconStar>().SetupView(item.rarity, maxStar);
			}
		}

		private void SetupPrice(ShopGoodsMetaData goodsItem)
		{
			List<int> list2;
			if (_ticketID > 0)
			{
				List<int> list = new List<int>();
				list.Add(goodsItem.HCoinCost);
				list2 = list;
			}
			else
			{
				list2 = UIUtil.GetGoodsRealPrice(_goods);
			}
			List<int> list3 = list2;
			if (_isMultiCurrency)
			{
				if (list3.Count > 3)
				{
					base.transform.GetComponent<LayoutElement>().preferredWidth = 320f;
				}
				else
				{
					base.transform.GetComponent<LayoutElement>().preferredWidth = 220f;
				}
			}
			Transform transform = base.transform.Find("BG/Unselected/NowPrize");
			Transform transform2 = base.transform.Find("BG/Selected/NowPrize");
			int num = 1;
			if (goodsItem.HCoinCost > 0)
			{
				if (_isMultiCurrency)
				{
					transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Hcoin");
					transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Hcoin");
					transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
				}
				else
				{
					transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Hcoin");
					transform2.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Hcoin");
					transform.Find("Num").GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find("Num").GetComponent<Text>().text = list3[num - 1].ToString();
				}
				SetupCurrencyColor(list3[num - 1] <= Singleton<PlayerModule>.Instance.playerData.hcoin, num);
				num++;
			}
			if (goodsItem.SCoinCost > 0)
			{
				if (_isMultiCurrency)
				{
					transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Scoin");
					transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Scoin");
					transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
				}
				else
				{
					transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Scoin");
					transform2.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Scoin");
					transform.Find("Num").GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find("Num").GetComponent<Text>().text = list3[num - 1].ToString();
				}
				SetupCurrencyColor(list3[num - 1] <= Singleton<PlayerModule>.Instance.playerData.scoin, num);
				num++;
			}
			if (goodsItem.CostItemId > 0)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId);
				string text = MiscData.GetCurrencyIconPath(goodsItem.CostItemId);
				if (string.IsNullOrEmpty(text))
				{
					text = dummyStorageDataItem.GetIconPath();
				}
				if (_isMultiCurrency)
				{
					transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text);
					transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text);
					transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
				}
				else
				{
					transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text);
					transform2.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text);
					transform.Find("Num").GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find("Num").GetComponent<Text>().text = list3[num - 1].ToString();
				}
				int num2 = 0;
				StorageDataItemBase storageDataItemBase = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId);
				if (storageDataItemBase != null)
				{
					num2 = storageDataItemBase.number;
				}
				SetupCurrencyColor(list3[num - 1] <= num2, num);
				num++;
			}
			if (goodsItem.CostItemId2 > 0)
			{
				StorageDataItemBase dummyStorageDataItem2 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId2);
				string text2 = MiscData.GetCurrencyIconPath(goodsItem.CostItemId2);
				if (string.IsNullOrEmpty(text2))
				{
					text2 = dummyStorageDataItem2.GetIconPath();
				}
				if (_isMultiCurrency)
				{
					transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text2);
					transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text2);
					transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
				}
				int num3 = 0;
				StorageDataItemBase storageDataItemBase2 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId2);
				if (storageDataItemBase2 != null)
				{
					num3 = storageDataItemBase2.number;
				}
				SetupCurrencyColor(list3[num - 1] <= num3, num);
				num++;
			}
			if (goodsItem.CostItemId3 > 0)
			{
				StorageDataItemBase dummyStorageDataItem3 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId3);
				string text3 = MiscData.GetCurrencyIconPath(goodsItem.CostItemId3);
				if (string.IsNullOrEmpty(text3))
				{
					text3 = dummyStorageDataItem3.GetIconPath();
				}
				if (_isMultiCurrency)
				{
					transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text3);
					transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text3);
					transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
				}
				int num4 = 0;
				StorageDataItemBase storageDataItemBase3 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId3);
				if (storageDataItemBase3 != null)
				{
					num4 = storageDataItemBase3.number;
				}
				SetupCurrencyColor(list3[num - 1] <= num4, num);
				num++;
			}
			if (goodsItem.CostItemId4 > 0)
			{
				StorageDataItemBase dummyStorageDataItem4 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId4);
				string text4 = MiscData.GetCurrencyIconPath(goodsItem.CostItemId4);
				if (string.IsNullOrEmpty(text4))
				{
					text4 = dummyStorageDataItem4.GetIconPath();
				}
				if (_isMultiCurrency)
				{
					transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text4);
					transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text4);
					transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
				}
				int num5 = 0;
				StorageDataItemBase storageDataItemBase4 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId4);
				if (storageDataItemBase4 != null)
				{
					num5 = storageDataItemBase4.number;
				}
				SetupCurrencyColor(list3[num - 1] <= num5, num);
				num++;
			}
			if (goodsItem.CostItemId5 > 0)
			{
				StorageDataItemBase dummyStorageDataItem5 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId5);
				string text5 = MiscData.GetCurrencyIconPath(goodsItem.CostItemId5);
				if (string.IsNullOrEmpty(text5))
				{
					text5 = dummyStorageDataItem5.GetIconPath();
				}
				if (_isMultiCurrency)
				{
					transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text5);
					transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text5);
					transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
					transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list3[num - 1].ToString();
				}
				int num6 = 0;
				StorageDataItemBase storageDataItemBase5 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId5);
				if (storageDataItemBase5 != null)
				{
					num6 = storageDataItemBase5.number;
				}
				SetupCurrencyColor(list3[num - 1] <= num6, num);
				num++;
			}
			if (list3.Count < transform.childCount)
			{
				for (int i = list3.Count; i < transform.childCount; i++)
				{
					transform.GetChild(i).gameObject.SetActive(false);
					transform2.GetChild(i).gameObject.SetActive(false);
				}
			}
			if (goodsItem.Discount < 10000)
			{
				int num7 = UIUtil.GetGoodsOriginPrice(_goods)[0];
				base.transform.Find("BG/Unselected/FakePrize").gameObject.SetActive(true);
				base.transform.Find("BG/Selected/FakePrize").gameObject.SetActive(true);
				base.transform.Find("BG/Unselected/FakePrize/Num").GetComponent<Text>().text = num7.ToString();
				base.transform.Find("BG/Selected/FakePrize/Num").GetComponent<Text>().text = num7.ToString();
			}
			else
			{
				base.transform.Find("BG/Unselected/FakePrize").gameObject.SetActive(false);
				base.transform.Find("BG/Selected/FakePrize").gameObject.SetActive(false);
			}
			if (!_isMultiCurrency)
			{
				base.transform.Find("BG/Unselected/NowPrize/Image").gameObject.SetActive(true);
				base.transform.Find("BG/Unselected/NowPrize/x").gameObject.SetActive(true);
				base.transform.Find("BG/Unselected/NowPrize/Num").gameObject.SetActive(true);
				base.transform.Find("BG/Selected/NowPrize/Image").gameObject.SetActive(true);
				base.transform.Find("BG/Selected/NowPrize/x").gameObject.SetActive(true);
				base.transform.Find("BG/Selected/NowPrize/Num").gameObject.SetActive(true);
			}
		}

		private void SetupAvatarStar(int starNum)
		{
			for (int i = 1; i < 7; i++)
			{
				string text = string.Format("AvatarStar/{0}", i);
				base.transform.Find(text).gameObject.SetActive(i == starNum);
			}
		}

		private void SetItemGrey()
		{
			base.transform.Find("BG/Unselected/FrameTop").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyTop");
			base.transform.Find("BG/Unselected/FrameBottom").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyBG");
			base.transform.Find("BG/Unselected/NowPrize").gameObject.SetActive(false);
			base.transform.Find("BG/Unselected/FakePrize").gameObject.SetActive(false);
			base.transform.Find("BG/Unselected/Empty").gameObject.SetActive(true);
			Color color = base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color;
			color.a = 0.5f;
			base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color = color;
			base.transform.Find("BG/Selected/FrameTop").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyTop");
			base.transform.Find("BG/Selected/NowPrize").gameObject.SetActive(false);
			base.transform.Find("BG/Selected/FakePrize").gameObject.SetActive(false);
			base.transform.Find("BG/Selected/Empty").gameObject.SetActive(true);
			color = base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color;
			color.a = 0.5f;
			base.transform.Find("BG/Selected/Image").GetComponent<Image>().color = color;
			base.transform.Find("ItemIcon").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyIcon");
			base.transform.Find("ItemIcon/SellOut").gameObject.SetActive(true);
			base.transform.Find("StigmataType/Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyIcon");
			base.transform.Find("FragmentIcon").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyIcon");
			base.transform.Find("SaleLabel/Bg").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ShopIcons/SalePatternGrey");
		}

		private void SetItemDefaultColor()
		{
			base.transform.Find("BG/Unselected/FrameTop").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedTop");
			base.transform.Find("BG/Unselected/FrameBottom").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedBottom");
			Transform transform = base.transform.Find("BG/Unselected/NowPrize");
			if (_isMultiCurrency)
			{
				foreach (Transform item in transform)
				{
					item.Find("Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
					item.Find("Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedPrice");
					item.Find("x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedPriceX");
				}
			}
			else
			{
				transform.Find("Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
				transform.Find("Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedPrice");
				transform.Find("x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedPriceX");
			}
			base.transform.Find("BG/Unselected/FakePrize/Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultDiscountNum");
			base.transform.Find("BG/Unselected/FakePrize/Num/Line").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultDiscountLine");
			base.transform.Find("BG/Selected/FrameTop").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultSelectedTop");
			Transform transform3 = base.transform.Find("BG/Selected/NowPrize");
			if (_isMultiCurrency)
			{
				foreach (Transform item2 in transform3)
				{
					item2.Find("Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
					item2.Find("Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultSelectedPrice");
					item2.Find("x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultSelectedPriceX");
				}
			}
			else
			{
				transform3.Find("Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
				transform3.Find("Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultSelectedPrice");
				transform3.Find("x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultSelectedPriceX");
			}
			base.transform.Find("BG/Selected/FakePrize/Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultDiscountNum");
			base.transform.Find("BG/Selected/FakePrize/Num/Line").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultDiscountLine");
			base.transform.Find("ItemIcon").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
			for (int i = 1; i < 6; i++)
			{
				Transform transform5 = base.transform.Find("Star/" + i);
				if (!(transform5 == null))
				{
					transform5.GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
				}
			}
			for (int j = 1; j < 6; j++)
			{
				Transform transform6 = base.transform.Find("AvatarStar/" + j);
				if (!(transform6 == null))
				{
					transform6.GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
				}
			}
			base.transform.Find("StigmataType/Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
			base.transform.Find("FragmentIcon").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
			base.transform.Find("NumPanel/Num/Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultNum");
			base.transform.Find("NumPanel/Num/x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultNumX");
			base.transform.Find("LevelPanel/Num/Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultNum");
			base.transform.Find("LevelPanel/Num/Lv").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultLevel");
		}

		private void SetupSaleLabel(ShopGoodsMetaData goodsItem)
		{
			if (goodsItem.IsSuperWorth)
			{
				base.transform.Find("SaleLabel").gameObject.SetActive(true);
				base.transform.Find("SaleLabel/Bg").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ShopIcons/SalePatternSuperWorth");
				base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountSuperWorth");
			}
			else if (goodsItem.Discount < 10000)
			{
				base.transform.Find("SaleLabel").gameObject.SetActive(true);
				base.transform.Find("SaleLabel/Bg").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ShopIcons/SalePatternDiscount");
				switch (Mathf.RoundToInt(goodsItem.Discount / 1000))
				{
				case 1:
					base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountOne");
					break;
				case 2:
					base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountTwo");
					break;
				case 3:
					base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountThree");
					break;
				case 4:
					base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountFour");
					break;
				case 5:
					base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountFive");
					break;
				case 6:
					base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountSix");
					break;
				case 7:
					base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountSeven");
					break;
				case 8:
					base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountEight");
					break;
				case 9:
					base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountNine");
					break;
				}
			}
			else
			{
				base.transform.Find("SaleLabel").gameObject.SetActive(false);
			}
		}

		private void SetupCurrencyColor(bool isEnough, int index)
		{
			if (!isEnough)
			{
				if (_isMultiCurrency)
				{
					base.transform.Find(string.Format("BG/Unselected/NowPrize/{0}/Num", index)).GetComponent<Text>().color = MiscData.GetColor("WarningRed");
					base.transform.Find(string.Format("BG/Unselected/NowPrize/{0}/x", index)).GetComponent<Text>().color = MiscData.GetColor("WarningRed");
					base.transform.Find(string.Format("BG/Selected/NowPrize/{0}/Num", index)).GetComponent<Text>().color = MiscData.GetColor("WarningRed");
					base.transform.Find(string.Format("BG/Selected/NowPrize/{0}/x", index)).GetComponent<Text>().color = MiscData.GetColor("WarningRed");
				}
				else
				{
					base.transform.Find("BG/Unselected/NowPrize/Num").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
					base.transform.Find("BG/Unselected/NowPrize/x").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
					base.transform.Find("BG/Selected/NowPrize/Num").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
					base.transform.Find("BG/Selected/NowPrize/x").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
				}
			}
			else if (_isMultiCurrency)
			{
				base.transform.Find(string.Format("BG/Unselected/NowPrize/{0}/Num", index)).GetComponent<Text>().color = MiscData.GetColor("Black");
				base.transform.Find(string.Format("BG/Unselected/NowPrize/{0}/x", index)).GetComponent<Text>().color = MiscData.GetColor("Black");
				base.transform.Find(string.Format("BG/Selected/NowPrize/{0}/Num", index)).GetComponent<Text>().color = MiscData.GetColor("Black");
				base.transform.Find(string.Format("BG/Selected/NowPrize/{0}/x", index)).GetComponent<Text>().color = MiscData.GetColor("Black");
			}
			else
			{
				base.transform.Find("BG/Unselected/NowPrize/Num").GetComponent<Text>().color = MiscData.GetColor("Black");
				base.transform.Find("BG/Unselected/NowPrize/x").GetComponent<Text>().color = MiscData.GetColor("Black");
				base.transform.Find("BG/Selected/NowPrize/Num").GetComponent<Text>().color = MiscData.GetColor("Black");
				base.transform.Find("BG/Selected/NowPrize/x").GetComponent<Text>().color = MiscData.GetColor("Black");
			}
		}
	}
}
