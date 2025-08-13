using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoRechargeItem : MonoBehaviour
	{
		private RechargeDataItem _storeDataItem;

		private string _iconPathPrex = "SpriteOutput/ShopIcons/";

		public void SetupView(RechargeDataItem rechargeDataItem, bool isSelected)
		{
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Invalid comparison between Unknown and I4
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Invalid comparison between Unknown and I4
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Invalid comparison between Unknown and I4
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Invalid comparison between Unknown and I4
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Invalid comparison between Unknown and I4
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Invalid comparison between Unknown and I4
			_storeDataItem = rechargeDataItem;
			base.transform.Find("InnerPanel/BG/Selected").gameObject.SetActive(isSelected);
			base.transform.Find("InnerPanel/BG/Unselected").gameObject.SetActive(!isSelected);
			base.transform.Find("InnerPanel/BG/Unselected/NowPrize/Num").GetComponent<Text>().text = rechargeDataItem.formattedPrice;
			base.transform.Find("InnerPanel/BG/Selected/NowPrize/Num").GetComponent<Text>().text = rechargeDataItem.formattedPrice;
			base.transform.Find("InnerPanel/NumPanel/Num/Num").GetComponent<Text>().text = rechargeDataItem.payHardCoin.ToString();
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview && (int)rechargeDataItem.productType == 3)
			{
				rechargeDataItem.productType = (ProductType)1;
			}
			if ((int)rechargeDataItem.productType == 3)
			{
				if (rechargeDataItem.cardLeftDays > 0)
				{
					base.transform.Find("InnerPanel/MonthCardLeftDaysPanel").gameObject.SetActive(true);
					base.transform.Find("InnerPanel/MonthCardDescPanel").gameObject.SetActive(false);
					base.transform.Find("InnerPanel/MonthCardLeftDaysPanel/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_GiftHardCoinDaysLeft", rechargeDataItem.cardLeftDays);
				}
				else
				{
					base.transform.Find("InnerPanel/MonthCardLeftDaysPanel").gameObject.SetActive(false);
					base.transform.Find("InnerPanel/MonthCardDescPanel").gameObject.SetActive(true);
					base.transform.Find("InnerPanel/MonthCardDescPanel/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_GifthardCoinDesc", rechargeDataItem.cardDailyHardCoin);
				}
				base.transform.Find("InnerPanel/LimitBuyPanel").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/MonthCardLimitBuyPanel").gameObject.SetActive(rechargeDataItem.cardLeftDays < 180);
			}
			else
			{
				base.transform.Find("InnerPanel/MonthCardLeftDaysPanel").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/MonthCardDescPanel").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/MonthCardLimitBuyPanel").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/LimitBuyPanel").gameObject.SetActive(true);
				if ((int)rechargeDataItem.productType == 2)
				{
					base.transform.Find("InnerPanel/LimitBuyPanel/addition/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_FirstRechargeBonus");
					base.transform.Find("InnerPanel/LimitBuyPanel/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_FirstHardCoinDesc", rechargeDataItem.leftBuyTimes);
				}
				else
				{
					base.transform.Find("InnerPanel/LimitBuyPanel/addition/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("MenuRechargeBonus");
					base.transform.Find("InnerPanel/LimitBuyPanel/Desc").gameObject.SetActive(false);
				}
			}
			if ((int)rechargeDataItem.productType == 3)
			{
				base.transform.Find("InnerPanel/SaleLabel/SalePatten5").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/SaleLabel/SalePatten4").gameObject.SetActive(false);
			}
			else if ((int)rechargeDataItem.productType == 2)
			{
				base.transform.Find("InnerPanel/SaleLabel/SalePatten4").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/SaleLabel/SalePatten5").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/LimitBuyPanel/addition/Num").GetComponent<Text>().text = rechargeDataItem.freeHardCoin.ToString();
			}
			else
			{
				if (rechargeDataItem.freeHardCoin > 0)
				{
					base.transform.Find("InnerPanel/LimitBuyPanel/addition/Num").GetComponent<Text>().text = rechargeDataItem.freeHardCoin.ToString();
				}
				else
				{
					base.transform.Find("InnerPanel/LimitBuyPanel/").gameObject.SetActive(false);
				}
				base.transform.Find("InnerPanel/SaleLabel/SalePatten4").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/SaleLabel/SalePatten5").gameObject.SetActive(false);
			}
			string empty = string.Empty;
			empty = ((!rechargeDataItem.productID.StartsWith("Bh3First")) ? (empty + rechargeDataItem.productID.Substring(3)) : (empty + rechargeDataItem.productID.Substring(8)));
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview && empty == "GiftHardCoinTier5")
			{
				empty = "HardCoinTier5";
			}
			base.transform.Find("InnerPanel/ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_iconPathPrex + empty);
			if ((int)rechargeDataItem.productType == 3)
			{
				RectTransform component = base.transform.Find("InnerPanel/ItemIcon/Icon").GetComponent<RectTransform>();
				component.anchoredPosition = new Vector2(component.anchoredPosition.x, 35f);
			}
		}

		public void OnClick()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectRechargeItem, _storeDataItem.productID));
			base.transform.Find("InnerPanel/BG/Selected").gameObject.SetActive(true);
			base.transform.Find("InnerPanel/BG/Unselected").gameObject.SetActive(false);
		}
	}
}
