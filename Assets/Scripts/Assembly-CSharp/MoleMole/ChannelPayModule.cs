using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class ChannelPayModule : BaseModule
	{
		public enum PayMethod
		{
			ALIPAY = 0,
			WEIXIN_PAY = 1
		}

		private const float _showAllProductsMaxWaitTime = 10f;

		private List<RechargeDataItem> _rechargeItemListFromServer;

		private List<RechargeDataItem> _rechargeItemListFromMarket;

		private List<RechargeDataItem> _rechargeItemList;

		private Coroutine _showAllProductsCoroutine;

		private Coroutine _getProductListFromServerCoroutine;

		private Coroutine _getProductListFromMarketCoroutine;

		private LoadingWheelWidgetContext _loadingWheelDialogContext;

		private readonly string[] INVALID_TOKEN_LIST = new string[1] { "ewoJInNpZ25hdHVyZSIgPSAiQXBkeEpkdE53UFUyckE1L2NuM2tJTzFPVGsyNWZlREthMGFhZ3l5UnZlV2xjRmxnbHY2UkY2em5raUJTM3VtOVVjN3BWb2IrUHFaUjJUOHd5VnJITnBsb2YzRFgzSXFET2xXcSs5MGE3WWwrcXJSN0E3ald3dml3NzA4UFMrNjdQeUhSbmhPL0c3YlZxZ1JwRXI2RXVGeWJpVTFGWEFpWEpjNmxzMVlBc3NReEFBQURWekNDQTFNd2dnSTdvQU1DQVFJQ0NHVVVrVTNaV0FTMU1BMEdDU3FHU0liM0RRRUJCUVVBTUg4eEN6QUpCZ05WQkFZVEFsVlRNUk13RVFZRFZRUUtEQXBCY0hCc1pTQkpibU11TVNZd0pBWURWUVFMREIxQmNIQnNaU0JEWlhKMGFXWnBZMkYwYVc5dUlFRjFkR2h2Y21sMGVURXpNREVHQTFVRUF3d3FRWEJ3YkdVZ2FWUjFibVZ6SUZOMGIzSmxJRU5sY25ScFptbGpZWFJwYjI0Z1FYVjBhRzl5YVhSNU1CNFhEVEE1TURZeE5USXlNRFUxTmxvWERURTBNRFl4TkRJeU1EVTFObG93WkRFak1DRUdBMVVFQXd3YVVIVnlZMmhoYzJWU1pXTmxhWEIwUTJWeWRHbG1hV05oZEdVeEd6QVpCZ05WQkFzTUVrRndjR3hsSUdsVWRXNWxjeUJUZEc5eVpURVRNQkVHQTFVRUNnd0tRWEJ3YkdVZ1NXNWpMakVMTUFrR0ExVUVCaE1DVlZNd2daOHdEUVlKS29aSWh2Y05BUUVCQlFBRGdZMEFNSUdKQW9HQkFNclJqRjJjdDRJclNkaVRDaGFJMGc4cHd2L2NtSHM4cC9Sd1YvcnQvOTFYS1ZoTmw0WElCaW1LalFRTmZnSHNEczZ5anUrK0RyS0pFN3VLc3BoTWRkS1lmRkU1ckdYc0FkQkVqQndSSXhleFRldngzSExFRkdBdDFtb0t4NTA5ZGh4dGlJZERnSnYyWWFWczQ5QjB1SnZOZHk2U01xTk5MSHNETHpEUzlvWkhBZ01CQUFHamNqQndNQXdHQTFVZEV3RUIvd1FDTUFBd0h3WURWUjBqQkJnd0ZvQVVOaDNvNHAyQzBnRVl0VEpyRHRkREM1RllRem93RGdZRFZSMFBBUUgvQkFRREFnZUFNQjBHQTFVZERnUVdCQlNwZzRQeUdVakZQaEpYQ0JUTXphTittVjhrOVRBUUJnb3Foa2lHOTJOa0JnVUJCQUlGQURBTkJna3Foa2lHOXcwQkFRVUZBQU9DQVFFQUVhU2JQanRtTjRDL0lCM1FFcEszMlJ4YWNDRFhkVlhBZVZSZVM1RmFaeGMrdDg4cFFQOTNCaUF4dmRXLzNlVFNNR1k1RmJlQVlMM2V0cVA1Z204d3JGb2pYMGlreVZSU3RRKy9BUTBLRWp0cUIwN2tMczlRVWU4Y3pSOFVHZmRNMUV1bVYvVWd2RGQ0TndOWXhMUU1nNFdUUWZna1FRVnk4R1had1ZIZ2JFL1VDNlk3MDUzcEdYQms1MU5QTTN3b3hoZDNnU1JMdlhqK2xvSHNTdGNURXFlOXBCRHBtRzUrc2s0dHcrR0szR01lRU41LytlMVFUOW5wL0tsMW5qK2FCdzdDMHhzeTBiRm5hQWQxY1NTNnhkb3J5L0NVdk02Z3RLc21uT09kcVRlc2JwMGJzOHNuNldxczBDOWRnY3hSSHVPTVoydG04bnBMVW03YXJnT1N6UT09IjsKCSJwdXJjaGFzZS1pbmZvIiA9ICJld29KSW05eWFXZHBibUZzTFhCMWNtTm9ZWE5sTFdSaGRHVXRjSE4wSWlBOUlDSXlNREV5TFRBM0xURXlJREExT2pVME9qTTFJRUZ0WlhKcFkyRXZURzl6WDBGdVoyVnNaWE1pT3dvSkluQjFjbU5vWVhObExXUmhkR1V0YlhNaUlEMGdJakV6TkRJd09UYzJOelU0T0RJaU93b0pJbTl5YVdkcGJtRnNMWFJ5WVc1ellXTjBhVzl1TFdsa0lpQTlJQ0l4TnpBd01EQXdNamswTkRrME1qQWlPd29KSW1KMmNuTWlJRDBnSWpFdU5DSTdDZ2tpWVhCd0xXbDBaVzB0YVdRaUlEMGdJalExTURVME1qSXpNeUk3Q2draWRISmhibk5oWTNScGIyNHRhV1FpSUQwZ0lqRTNNREF3TURBeU9UUTBPVFF5TUNJN0Nna2ljWFZoYm5ScGRIa2lJRDBnSWpFaU93b0pJbTl5YVdkcGJtRnNMWEIxY21Ob1lYTmxMV1JoZEdVdGJYTWlJRDBnSWpFek5ESXdPVGMyTnpVNE9ESWlPd29KSW1sMFpXMHRhV1FpSUQwZ0lqVXpOREU0TlRBME1pSTdDZ2tpZG1WeWMybHZiaTFsZUhSbGNtNWhiQzFwWkdWdWRHbG1hV1Z5SWlBOUlDSTVNRFV4TWpNMklqc0tDU0p3Y205a2RXTjBMV2xrSWlBOUlDSmpiMjB1ZW1Wd2RHOXNZV0l1WTNSeVltOXVkWE11YzNWd1pYSndiM2RsY2pFaU93b0pJbkIxY21Ob1lYTmxMV1JoZEdVaUlEMGdJakl3TVRJdE1EY3RNVElnTVRJNk5UUTZNelVnUlhSakwwZE5WQ0k3Q2draWIzSnBaMmx1WVd3dGNIVnlZMmhoYzJVdFpHRjBaU0lnUFNBaU1qQXhNaTB3TnkweE1pQXhNam8xTkRvek5TQkZkR012UjAxVUlqc0tDU0ppYVdRaUlEMGdJbU52YlM1NlpYQjBiMnhoWWk1amRISmxlSEJsY21sdFpXNTBjeUk3Q2draWNIVnlZMmhoYzJVdFpHRjBaUzF3YzNRaUlEMGdJakl3TVRJdE1EY3RNVElnTURVNk5UUTZNelVnUVcxbGNtbGpZUzlNYjNOZlFXNW5aV3hsY3lJN0NuMD0iOwoJInBvZCIgPSAiMTciOwoJInNpZ25pbmctc3RhdHVzIiA9ICIwIjsKfQ==" };

		public ChannelPayModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_rechargeItemListFromServer = null;
			_rechargeItemListFromMarket = null;
			_rechargeItemList = new List<RechargeDataItem>();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 99:
				return OnGetProductsListRsp(pkt.getData<GetProductListRsp>());
			case 83:
				return OnRechargeSuccNotify(pkt.getData<RechargeFinishNotify>());
			case 208:
				return OnCreateWeiXinOrderRsp(pkt.getData<CreateWeiXinOrderRsp>());
			default:
				return false;
			}
		}

		public List<RechargeDataItem> GetRechargeItemList()
		{
			return _rechargeItemList;
		}

		public RechargeDataItem GetStoreItemByProductID(string productID)
		{
			if (_rechargeItemList != null)
			{
				for (int i = 0; i < _rechargeItemList.Count; i++)
				{
					if (_rechargeItemList[i] != null && _rechargeItemList[i].productID == productID)
					{
						return _rechargeItemList[i];
					}
				}
			}
			return null;
		}

		private IEnumerator ReqStoreItemListFromServer()
		{
			_rechargeItemListFromServer = null;
			Singleton<NetworkManager>.Instance.RequestProductList();
			while (_rechargeItemListFromServer == null)
			{
				yield return null;
			}
		}

		private IEnumerator ReqStoreItemListFromMarket()
		{
			_rechargeItemListFromMarket = null;
			if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch != ConfigAccount.PaymentBranch.APPSTORE_CN)
			{
				_rechargeItemListFromMarket = new List<RechargeDataItem>();
			}
			while (_rechargeItemListFromMarket == null)
			{
				yield return null;
			}
		}

		private void MergeStoreItemList()
		{
			_rechargeItemList.Clear();
			for (int i = 0; i < _rechargeItemListFromServer.Count; i++)
			{
				RechargeDataItem rechargeDataItem = new RechargeDataItem(_rechargeItemListFromServer[i]);
				if (_rechargeItemListFromMarket != null)
				{
					for (int j = 0; j < _rechargeItemListFromMarket.Count; j++)
					{
						if (rechargeDataItem.productID == _rechargeItemListFromMarket[j].productID)
						{
							rechargeDataItem.formattedPrice = _rechargeItemListFromMarket[j].formattedPrice;
							_rechargeItemList.Add(rechargeDataItem);
							break;
						}
					}
				}
				else
				{
					_rechargeItemList.Add(rechargeDataItem);
				}
			}
		}

		public IEnumerator ShowAllProductsAsync()
		{
			if (!string.IsNullOrEmpty(GetReceipt()))
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTransitionContinued")));
				CheckReceipt();
			}
			else if (_showAllProductsCoroutine == null && _getProductListFromServerCoroutine == null && _getProductListFromMarketCoroutine == null)
			{
				if (_loadingWheelDialogContext != null)
				{
					_loadingWheelDialogContext.Finish();
					_loadingWheelDialogContext = null;
				}
				_loadingWheelDialogContext = new LoadingWheelWidgetContext(0, RetryShowAllProducts);
				_loadingWheelDialogContext.SetMaxWaitTime(10f);
				Singleton<MainUIManager>.Instance.ShowWidget(_loadingWheelDialogContext);
				_getProductListFromServerCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(ReqStoreItemListFromServer());
				yield return _getProductListFromServerCoroutine;
				_getProductListFromServerCoroutine = null;
				if (CheckNeedGetProductListFromMarket())
				{
					_getProductListFromMarketCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(ReqStoreItemListFromMarket());
					yield return _getProductListFromMarketCoroutine;
					_getProductListFromMarketCoroutine = null;
				}
				MergeStoreItemList();
				_showAllProductsCoroutine = null;
				if (_loadingWheelDialogContext != null)
				{
					_loadingWheelDialogContext.Finish();
					_loadingWheelDialogContext = null;
				}
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RefreshRechargeTab));
			}
		}

		public bool PreparePurchaseProduct(string productID)
		{
			if (_loadingWheelDialogContext != null)
			{
				_loadingWheelDialogContext.Finish();
				_loadingWheelDialogContext = null;
			}
			if (!string.IsNullOrEmpty(GetReceipt()))
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTransitionContinued")));
				CheckReceipt();
				return false;
			}
			return true;
		}

		public void OnPurchaseCallback(PayResult payResult)
		{
			bool flag = false;
			if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.APPSTORE_CN)
			{
				MonoStoreKitEventListener.IAP_PURCHASE_CALLBACK = null;
				ApplePayResult applePayResult = (ApplePayResult)payResult;
				if (applePayResult.payRetCode == PayResult.PayRetcode.CANCELED)
				{
					flag = true;
				}
				else if (applePayResult.payRetCode == PayResult.PayRetcode.FAILED)
				{
					flag = true;
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTransitionFailed")));
				}
				else if (applePayResult.payRetCode == PayResult.PayRetcode.CONFIRMING)
				{
					flag = true;
				}
				else if (applePayResult.payRetCode != PayResult.PayRetcode.SUCCESS)
				{
					flag = true;
				}
			}
			else if (payResult.payRetCode != PayResult.PayRetcode.SUCCESS && payResult.payRetCode != PayResult.PayRetcode.CONFIRMING)
			{
				if (payResult.payRetCode == PayResult.PayRetcode.CANCELED)
				{
					flag = true;
				}
				else if (payResult.payRetCode == PayResult.PayRetcode.FAILED)
				{
					flag = true;
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(payResult.GetResultShowText()));
				}
				else
				{
					flag = true;
				}
			}
			if (flag && _loadingWheelDialogContext != null)
			{
				_loadingWheelDialogContext.Finish();
				_loadingWheelDialogContext = null;
			}
		}

		private bool CheckNeedGetProductListFromMarket()
		{
			if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch != ConfigAccount.PaymentBranch.APPSTORE_CN)
			{
				return false;
			}
			if (_rechargeItemListFromMarket == null)
			{
				return true;
			}
			for (int i = 0; i < _rechargeItemListFromServer.Count; i++)
			{
				RechargeDataItem rechargeDataItem = new RechargeDataItem(_rechargeItemListFromServer[i]);
				bool flag = false;
				for (int j = 0; j < _rechargeItemListFromMarket.Count; j++)
				{
					if (rechargeDataItem.productID == _rechargeItemListFromMarket[j].productID)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return true;
				}
			}
			return false;
		}

		public string GetReceipt()
		{
			return Singleton<MiHoYoGameData>.Instance.LocalData.Receipt;
		}

		private void AddReceipt(string receipt)
		{
			Singleton<MiHoYoGameData>.Instance.LocalData.Receipt = receipt;
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		private void RemoveReceipt()
		{
			Singleton<MiHoYoGameData>.Instance.LocalData.Receipt = string.Empty;
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		public void CheckReceipt()
		{
			string receipt = GetReceipt();
			if (!string.IsNullOrEmpty(receipt) && _loadingWheelDialogContext == null)
			{
				_loadingWheelDialogContext = new LoadingWheelWidgetContext
				{
					ignoreMaxWaitTime = true
				};
				Singleton<MainUIManager>.Instance.ShowWidget(_loadingWheelDialogContext);
			}
		}

		private bool VerifyReceiptLocal(string base64Receipt)
		{
			if (base64Receipt == null || (base64Receipt != null && base64Receipt.Length < 1024))
			{
				return false;
			}
			if (VerifyInvalidToken(base64Receipt))
			{
				return false;
			}
			string text = SecurityUtil.Base64Decoder(base64Receipt);
			if (text == string.Empty)
			{
				return false;
			}
			string base64StringByKey = GetBase64StringByKey(text, "purchase-info");
			if (base64StringByKey == string.Empty)
			{
				return false;
			}
			base64StringByKey = SecurityUtil.Base64Decoder(base64StringByKey);
			if (base64StringByKey == string.Empty)
			{
				return false;
			}
			string base64StringByKey2 = GetBase64StringByKey(base64StringByKey, "bid");
			if (base64StringByKey2 != Singleton<NetworkManager>.Instance.channelConfig.BundleIdentifier)
			{
				return false;
			}
			return true;
		}

		private string GetBase64StringByKey(string receipt, string key)
		{
			int num = receipt.IndexOf("\"" + key + "\" = \"") + key.Length + 6;
			int num2 = receipt.IndexOf("\";", num);
			if (num != -1 && num2 != -1 && num2 > num)
			{
				return receipt.Substring(num, num2 - num);
			}
			return string.Empty;
		}

		private bool VerifyInvalidToken(string token)
		{
			for (int i = 0; i < INVALID_TOKEN_LIST.Length; i++)
			{
				if (token == INVALID_TOKEN_LIST[i])
				{
					return true;
				}
			}
			return false;
		}

		private bool OnGetProductsListRsp(GetProductListRsp rsp)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			_rechargeItemListFromServer = new List<RechargeDataItem>();
			if ((int)rsp.retcode == 0)
			{
				List<Product> product_list = rsp.product_list;
				foreach (Product item2 in product_list)
				{
					RechargeDataItem item = new RechargeDataItem(item2);
					_rechargeItemListFromServer.Add(item);
				}
			}
			return false;
		}

		private bool OnRechargeSuccNotify(RechargeFinishNotify rsp)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Invalid comparison between Unknown and I4
			if (_loadingWheelDialogContext != null)
			{
				_loadingWheelDialogContext.Finish();
				_loadingWheelDialogContext = null;
			}
			if ((int)rsp.retcode == 0 || ((int)rsp.retcode == 2 && !string.IsNullOrEmpty(GetReceipt())))
			{
				RechargeDataItem storeItemByProductID = GetStoreItemByProductID(rsp.product_name);
				string text = ((storeItemByProductID == null) ? string.Empty : storeItemByProductID.productName);
				int num = (int)(rsp.pay_hcoin + rsp.free_hcoin);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralConfirmDialogContext
				{
					type = GeneralConfirmDialogContext.ButtonType.SingleButton,
					desc = LocalizationGeneralLogic.GetText("IAPTransitionSuccess", text, num),
					buttonCallBack = delegate
					{
						Singleton<ShopWelfareModule>.Instance.TryHintNewWelfare();
					}
				});
				RemoveReceipt();
				StopAllShowProductsCoroutines();
				_showAllProductsCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(ShowAllProductsAsync());
				Singleton<NetworkManager>.Instance.RequestGetVipRewardData();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTransitionFailed")));
				RemoveReceipt();
			}
			return false;
		}

		private bool OnCreateWeiXinOrderRsp(CreateWeiXinOrderRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY)
				{
					TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
					if (theOriginalAccountManager != null)
					{
						theOriginalAccountManager.PayByWeiXinOrder(rsp.partner_id, rsp.prepay_id, rsp.nonce_str, rsp.timestamp, rsp.sign);
					}
				}
			}
			else
			{
				OnPurchaseCallback(new WeixinPayResult("ErrFailCreateWeiXinOrder"));
			}
			return false;
		}

		public void SetPayMethodId(PayMethod payMethodId)
		{
			Singleton<MiHoYoGameData>.Instance.LocalData.PayMethod = (int)payMethodId;
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		public PayMethod GetPayMethodId()
		{
			return (PayMethod)Singleton<MiHoYoGameData>.Instance.LocalData.PayMethod;
		}

		private void RetryShowAllProducts()
		{
			StopAllShowProductsCoroutines();
			_showAllProductsCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(ShowAllProductsAsync());
		}

		private void StopAllShowProductsCoroutines()
		{
			if (_showAllProductsCoroutine != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_showAllProductsCoroutine);
				_showAllProductsCoroutine = null;
			}
			if (_getProductListFromServerCoroutine != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_getProductListFromServerCoroutine);
				_getProductListFromServerCoroutine = null;
			}
			if (_getProductListFromMarketCoroutine != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_getProductListFromMarketCoroutine);
				_getProductListFromMarketCoroutine = null;
			}
		}
	}
}
