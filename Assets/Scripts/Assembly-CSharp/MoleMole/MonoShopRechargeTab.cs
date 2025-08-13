using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoShopRechargeTab : MonoBehaviour
	{
		private Transform _scrollViewTrans;

		private string _currentSelectedProductID = string.Empty;

		private List<RechargeDataItem> _rechargeDataItemList;

		private bool _waitRefreshAfterSetupView;

		private bool _hasPlayAnim;

		public void SetupView()
		{
			_currentSelectedProductID = string.Empty;
			_scrollViewTrans = base.transform.Find("ScrollView");
			_scrollViewTrans.gameObject.SetActive(false);
			Singleton<AccountManager>.Instance.manager.ShowAllProducts();
			_waitRefreshAfterSetupView = true;
		}

		public void OnRefreshRechargeTab()
		{
			_rechargeDataItemList = Singleton<AccountManager>.Instance.manager.GetRechargeItemList();
			foreach (RechargeDataItem rechargeDataItem in _rechargeDataItemList)
			{
			}
			_currentSelectedProductID = string.Empty;
			_scrollViewTrans.GetComponent<MonoGridScroller>().Init(OnScrollChange, _rechargeDataItemList.Count, new Vector2(0f, 1f));
			_scrollViewTrans.gameObject.SetActive(true);
			if (_waitRefreshAfterSetupView && !_hasPlayAnim)
			{
				base.transform.Find("ScrollView/Content").GetComponent<Animation>().Play("RechargeItemsFadeIn");
				_waitRefreshAfterSetupView = false;
				_hasPlayAnim = true;
			}
		}

		public void OnPucharseProduct()
		{
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.forbidRecharge)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ForbidRecharge")));
				return;
			}
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.rechargeMaxLimit > 0 && Singleton<ShopWelfareModule>.Instance.totalPayHCoin >= Singleton<NetworkManager>.Instance.DispatchSeverData.rechargeMaxLimit)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopRechargeHCoinLimit")));
				return;
			}
			if (string.IsNullOrEmpty(_currentSelectedProductID))
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopNoSelect")));
				return;
			}
			RechargeDataItem storeItemByProductID = Singleton<AccountManager>.Instance.manager.GetStoreItemByProductID(_currentSelectedProductID);
			if (storeItemByProductID != null)
			{
				if (!storeItemByProductID.CanPurchase())
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTPurchaseLimit")));
				}
				else
				{
					Singleton<AccountManager>.Instance.manager.Pay(_currentSelectedProductID, storeItemByProductID.productName, (float)storeItemByProductID.serverPrice / 100f);
				}
			}
			else
			{
				_currentSelectedProductID = string.Empty;
			}
		}

		public void OnSelectProduct(string productID)
		{
			_currentSelectedProductID = productID;
			_scrollViewTrans.GetComponent<MonoGridScroller>().RefreshCurrent();
			RechargeDataItem storeItemByProductID = Singleton<AccountManager>.Instance.manager.GetStoreItemByProductID(productID);
			if (storeItemByProductID != null)
			{
				base.transform.parent.Find("CartInfoPanel/BuyBtn").GetComponent<Button>().interactable = true;
				base.transform.parent.Find("CartInfoPanel/Info").gameObject.SetActive(true);
				base.transform.parent.Find("CartInfoPanel/Info/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopRechargeBuyDesc", storeItemByProductID.formattedPrice, storeItemByProductID.productName);
				SetupPayMethodView();
			}
		}

		private void SetupPayMethodView()
		{
			bool flag = Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY;
			base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns").gameObject.SetActive(flag);
			if (flag)
			{
				bool flag2 = Singleton<ChannelPayModule>.Instance.GetPayMethodId() == ChannelPayModule.PayMethod.ALIPAY;
				base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns/AilpayBtn/ActiveImage").gameObject.SetActive(flag2);
				base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns/AilpayBtn/UnactiveImage").gameObject.SetActive(!flag2);
				base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns/WechatBtn/ActiveImage").gameObject.SetActive(!flag2);
				base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns/WechatBtn/UnactiveImage").gameObject.SetActive(flag2);
			}
		}

		public void SetPayMethodId(ChannelPayModule.PayMethod payMethodId)
		{
			Singleton<ChannelPayModule>.Instance.SetPayMethodId(payMethodId);
			SetupPayMethodView();
		}

		private void OnScrollChange(Transform trans, int index)
		{
			RechargeDataItem rechargeDataItem = _rechargeDataItemList[index];
			trans.GetComponent<MonoRechargeItem>().SetupView(rechargeDataItem, rechargeDataItem.productID == _currentSelectedProductID);
		}

		private void OnDisable()
		{
		}

		private void OnDestroy()
		{
			_currentSelectedProductID = string.Empty;
		}
	}
}
