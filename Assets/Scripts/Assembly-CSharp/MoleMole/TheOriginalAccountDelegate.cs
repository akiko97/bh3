using Com.Alipay;
using UnityEngine;

namespace MoleMole
{
	public class TheOriginalAccountDelegate : TheBaseAccountDelegate
	{
		public class WeixinPrepayOrderInfo
		{
			public string productID;

			public string productName;

			public float productPrice;

			public string partnerID;

			public string prepayID;

			public string nonceStr;

			public string timestamp;

			public string sign;
		}

		private static string ALIPAY_PARTNER = "2088021131947549";

		private static string ALIPAY_SELLER = "2088021131947549";

		private static string ALIPAY_RSAPRIVATE = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBALLr6IbybR4f7HkAHbtVUI3+fNcN8BGtfDQyP8TzFj80E2xTC8wrdrUj+2oRTVqcK1l7QA08pZr4SgGx/8WtBr5wUEPiKcO5IG1/A9D4s06t3/Gw2uxoxB2fF5I0ci7QwYVI0H8mS/j9GX2z1lqvPXCcGhJq5FMyIDCzHEMibwAXAgMBAAECgYA5fBH5SWpFg3w2ZBMpXP/Enz782T2IcHS3UG2smW1MYS7cXtIrhstc53KfYW+47PQAi9jIZ/PNFniwkr/agvznKKqD5K95g1z0DGtkKIs5rMGrjS04FtRRUw7SCXJODhbH2VLZ+QJj9fLnbPQe23lPrdcns7oVges1bLEyWjn6AQJBAOjpz9kYcZFMwj8lddd2f3qu0B6rSGdaRdrWt9GHRwipqZQFJB+DW9nA3r5xq9+TNg9p38r17A00+tGALaSqboUCQQDEqA0jrVrf95ERk9gp/7NJCSbWW+3glO3abYhjpJbaUfM+MjilLKN0xTMw+SjJJWymrAuWLt/M+6f5QwBX4RzrAkA+Nh2XTikfd1I3DalxOKyKN2FNn9CCEqGv90Q4ChsWHEM4Tzs705lYC2UzlyciW67H5S6qho9bY7hO9x656fAFAkBH7fvYV9kMYH30QvJm8jr+dNV6xGcupOqW4UdowtPWiPECh9YGPFyRImwF9qx/XivujrEyPnTnggi/eE1Q12r/AkEAnsIov6ihXXMEN1/xEerJV3bze3MSga2APRNDwEEMXn3BSUaaQP4iU67QWTmLrTUv9l+uPHqMqqpxCm0Kl5ASgg==";

		protected AndroidJavaObject _delegate_weixin;

		private static string WEIXIN_PAY_APP_ID = "wxc090bb4ff5c352e4";

		private static string WEIXIN_PAY_PACKAGE_VALUE = "Sign=WXPay";

		public WeixinPrepayOrderInfo weixinPrepayOrderInfo;

		public TheOriginalAccountDelegate()
		{
			/*if (_delegate == null)
			{
				_delegate = new AndroidJavaObject("com.miHoYo.originpaydelegate.AlipayDelegate", _activity);
			}
			if (_delegate_weixin == null)
			{
				_delegate_weixin = new AndroidJavaObject("com.miHoYo.originpaydelegate.WeixinPayDelegate", _activity, _handler, WEIXIN_PAY_APP_ID);
			}*/
		}

		public override void pay(string productID, string productName, float productPrice, string tradeNo, string userID, string notifyUrl, string callbackClass, string callbackMethod, Function callback)
		{
			if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch != ConfigAccount.PaymentBranch.APPSTORE_CN && Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY)
			{
				if (Singleton<ChannelPayModule>.Instance.GetPayMethodId() == ChannelPayModule.PayMethod.ALIPAY)
				{
					string text = SecurityUtil.Md5("subject=" + productID + "&out_trade_no=" + tradeNo + "&uid=" + userID) + userID;
					string empty = string.Empty;
					empty = empty + "partner=\"" + ALIPAY_PARTNER + "\"";
					empty = empty + "&out_trade_no=\"" + tradeNo + "\"";
					empty = empty + "&subject=\"" + productID + "\"";
					empty = empty + "&body=\"" + text + "\"";
					string text2 = empty;
					empty = text2 + "&total_fee=\"" + productPrice + "\"";
					empty = empty + "&notify_url=\"" + WWW.EscapeURL(notifyUrl) + "\"";
					empty += "&service=\"mobile.securitypay.pay\"";
					empty += "&_input_charset=\"utf-8\"";
					empty = empty + "&return_url=\"" + WWW.EscapeURL("http://m.alipay.com") + "\"";
					empty += "&payment_type=\"1\"";
					empty = empty + "&seller_id=\"" + ALIPAY_SELLER + "\"";
					empty += "&it_b_pay=\"1m\"";
					string s = RSAFromPkcs8.sign(empty, ALIPAY_RSAPRIVATE, "utf-8");
					empty = empty + "&sign=\"" + WWW.EscapeURL(s) + "\"&sign_type=\"RSA\"";
					_delegate.Call("pay", empty);
				}
				else if (Singleton<ChannelPayModule>.Instance.GetPayMethodId() == ChannelPayModule.PayMethod.WEIXIN_PAY)
				{
					//_delegate_weixin.Call("pay", WEIXIN_PAY_APP_ID, weixinPrepayOrderInfo.partnerID, weixinPrepayOrderInfo.prepayID, WEIXIN_PAY_PACKAGE_VALUE, weixinPrepayOrderInfo.nonceStr, weixinPrepayOrderInfo.timestamp, weixinPrepayOrderInfo.sign);
				}
			}
		}
	}
}
