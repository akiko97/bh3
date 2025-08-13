using System;
using System.Text;
using SimpleJSON;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class TheOriginalAccountManager : TheBaseAccountManager
	{
		private string ORIGINAL_ACCOUNT_SEVER;

		private string ORIGINAL_LOGIN_URL;

		private string ORIGINAL_TOKEN_LOGIN_URL;

		private string ORIGINAL_GET_USER_INFO_BY_TOKEN_URL;

		private string ORIGINAL_VERIFY_EMAIL_APPLY_URL;

		public string ORIGINAL_MOBILE_REGISTER_URL;

		public string ORIGINAL_EMAIL_REGISTER_URL;

		public string ORIGINAL_MOBILE_PASSWORD_URL;

		public string ORIGINAL_EMAIL_PASSWORD_URL;

		public string ORIGINAL_CHANGE_PASSWORD_URL;

		public string ORIGINAL_BIND_EMAIL_URL;

		public string ORIGINAL_BIND_MOBILE_URL;

		public string ORIGINAL_BIND_IDENTITY_URL;

		public string Name { get; private set; }

		public string Email { get; private set; }

		public string Mobile { get; private set; }

		public bool IsEmailVerify { get; private set; }

		public bool IsRealNameVerify { get; private set; }

		public TheOriginalAccountManager()
		{
			base.AccountType = (AccountType)1;
			_accountDelegate = new TheOriginalAccountDelegate();
		}

		public override void Init()
		{
		}

		public override void SetupByDispatchServerData()
		{
			ORIGINAL_ACCOUNT_SEVER = Singleton<NetworkManager>.Instance.DispatchSeverData.accountUrl;
			ORIGINAL_LOGIN_URL = ORIGINAL_ACCOUNT_SEVER + "/sdk/login";
			ORIGINAL_TOKEN_LOGIN_URL = ORIGINAL_ACCOUNT_SEVER + "/sdk/token_login";
			ORIGINAL_GET_USER_INFO_BY_TOKEN_URL = ORIGINAL_ACCOUNT_SEVER + "/sdk/get_user_info_by_token";
			ORIGINAL_VERIFY_EMAIL_APPLY_URL = ORIGINAL_ACCOUNT_SEVER + "/sdk/verify_email_apply";
			ORIGINAL_MOBILE_REGISTER_URL = ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/mobile_register";
			ORIGINAL_EMAIL_REGISTER_URL = ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/email_register";
			ORIGINAL_MOBILE_PASSWORD_URL = ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/change_password_by_mobile";
			ORIGINAL_EMAIL_PASSWORD_URL = ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/change_password_by_email_apply";
			ORIGINAL_CHANGE_PASSWORD_URL = ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/change_password";
			ORIGINAL_BIND_EMAIL_URL = ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/bind_email_apply";
			ORIGINAL_BIND_MOBILE_URL = ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/bind_mobile";
			ORIGINAL_BIND_IDENTITY_URL = ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/bind_realname";
		}

		public override void InitFinishedCallBack(string param)
		{
		}

		public override string GetAccountName()
		{
			if (!string.IsNullOrEmpty(Name))
			{
				return Name;
			}
			if (!string.IsNullOrEmpty(Email))
			{
				return Email;
			}
			if (!string.IsNullOrEmpty(Mobile))
			{
				return Mobile;
			}
			return string.Empty;
		}

		private string GetHiddenName()
		{
			if (string.IsNullOrEmpty(Name))
			{
				return string.Empty;
			}
			string name = Name;
			int num = 2;
			if (name.Length >= num)
			{
				return name.Substring(0, num) + "***";
			}
			return name + "***";
		}

		private string GetHiddenEmail()
		{
			if (string.IsNullOrEmpty(Email))
			{
				return string.Empty;
			}
			string email = Email;
			int num = 2;
			string[] array = email.Split("@"[0]);
			if (array == null || array.Length != 2)
			{
				return email;
			}
			if (array[0].Length >= num)
			{
				return array[0].Substring(0, num) + "***@" + array[1];
			}
			return array[0] + "***" + array[1];
		}

		private string GetHiddenMobile()
		{
			if (string.IsNullOrEmpty(Mobile))
			{
				return string.Empty;
			}
			string text = Mobile;
			if (text.Length > 4)
			{
				StringBuilder stringBuilder = new StringBuilder(Mobile);
				for (int i = 3; i < text.Length - 1; i++)
				{
					stringBuilder[i] = "*"[0];
				}
				text = stringBuilder.ToString();
			}
			return text;
		}

		public override void Reset()
		{
			Name = string.Empty;
			Email = string.Empty;
			Mobile = string.Empty;
			IsEmailVerify = false;
			IsRealNameVerify = false;
			base.Reset();
		}

		public override void SaveAccountToLocal()
		{
			GeneralLocalDataItem.AccountData accountData = new GeneralLocalDataItem.AccountData();
			accountData.uid = base.AccountUid;
			accountData.token = base.AccountToken;
			accountData.name = Name;
			accountData.email = Email;
			accountData.mobile = Mobile;
			accountData.isEmailVerify = IsEmailVerify;
			accountData.isRealNameVerify = IsRealNameVerify;
			GeneralLocalDataItem.AccountData accountData2 = accountData;
			Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account = accountData2;
			Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginAccountName = accountData2.GetAccountName();
			Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
		}

		public override void LoginUI()
		{
			if (!string.IsNullOrEmpty(Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.uid))
			{
				TokenLoginTest();
			}
			else
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowMihoyoLoginUI));
			}
		}

		private void LoginAlertCallBackDoubleAlert()
		{
		}

		private void LoginAlertCallBack()
		{
		}

		public void TokenLoginTest()
		{
			GeneralLocalDataItem generalLocalData = Singleton<MiHoYoGameData>.Instance.GeneralLocalData;
			string url = string.Format(ORIGINAL_TOKEN_LOGIN_URL + "?uid={0}&token={1}", generalLocalData.Account.uid, generalLocalData.Account.token);
			Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, TokenLoginTestFinishedCallBack, delegate
			{
				WWWTimeOut(TokenLoginTest);
			}));
		}

		public void TokenLoginTestFinishedCallBack(string param)
		{
			JSONNode jSONNode = JSON.Parse(param);
			GeneralLocalDataItem generalLocalData = Singleton<MiHoYoGameData>.Instance.GeneralLocalData;
			if (jSONNode["retcode"].AsInt >= 0)
			{
				base.AccountUid = generalLocalData.Account.uid;
				base.AccountToken = generalLocalData.Account.token;
				base.AccountExt = generalLocalData.Account.ext;
				Name = generalLocalData.Account.name;
				Email = generalLocalData.Account.email;
				Mobile = generalLocalData.Account.mobile;
				IsEmailVerify = generalLocalData.Account.isEmailVerify;
				IsRealNameVerify = generalLocalData.Account.isRealNameVerify;
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountLoginSuccess));
				if (IsAccountBind() && (!IsEmailVerify || !IsRealNameVerify))
				{
					GetUserInfoByToken();
				}
			}
			else
			{
				generalLocalData.ClearLastLoginUser();
				Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_MihoyoAccountTokenError")));
				LoginUI();
			}
		}

		public override void LoginUIFinishedCallBack(string arg1, string arg2)
		{
			_accountArg1 = arg1;
			_accountArg2 = arg2;
			LoginTest();
		}

		protected override void LoginTest()
		{
			string url = string.Format(ORIGINAL_LOGIN_URL + "?account={0}&password={1}", WWW.EscapeURL(_accountArg1), WWW.EscapeURL(_accountArg2));
			Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, LoginTestFinishedCallBack, delegate
			{
				WWWTimeOut(LoginTest);
			}));
		}

		public override void LoginTestFinishedCallBack(string param)
		{
			JSONNode jSONNode = JSON.Parse(param);
			if (jSONNode["retcode"].AsInt >= 0)
			{
				base.AccountUid = jSONNode["data"]["uid"];
				base.AccountToken = jSONNode["data"]["token"];
				Name = jSONNode["data"]["name"];
				Email = jSONNode["data"]["email"];
				Mobile = jSONNode["data"]["mobile"];
				IsEmailVerify = jSONNode["data"]["is_email_verify"].AsInt > 0;
				IsRealNameVerify = !string.IsNullOrEmpty(jSONNode["data"]["realname"].Value) && !string.IsNullOrEmpty(jSONNode["data"]["identity_card"].Value);
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountLoginSuccess));
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(jSONNode["msg"]));
			}
		}

		public override void RegisterUI()
		{
		}

		public override void RegisterUIFinishedCallBack(string arg1, string arg2, string arg3, string arg4)
		{
		}

		protected override void RegisterTest()
		{
		}

		public override void RegisterTestFinishedCallBack(string param)
		{
		}

		public override void BindUI()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoBindIndexDialogContext());
		}

		public override void BindUIFinishedCallBack(string arg1, string arg2)
		{
			_accountArg1 = arg1;
			_accountArg2 = arg2;
			BindTest();
		}

		protected override void BindTest()
		{
			string url = string.Format(ORIGINAL_LOGIN_URL + "?account={0}&password={1}", WWW.EscapeURL(_accountArg1), WWW.EscapeURL(_accountArg2));
			Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, BindTestFinishedCallBack, delegate
			{
				WWWTimeOut(BindTest);
			}));
		}

		public override void BindTestFinishedCallBack(string param)
		{
			JSONNode jSONNode = JSON.Parse(param);
			if (jSONNode["retcode"].AsInt >= 0)
			{
				base.AccountUid = jSONNode["data"]["uid"];
				base.AccountToken = jSONNode["data"]["token"];
				Name = jSONNode["data"]["name"];
				Email = jSONNode["data"]["email"];
				Mobile = jSONNode["data"]["mobile"];
				IsEmailVerify = jSONNode["data"]["is_email_verify"].AsInt > 0;
				IsRealNameVerify = !string.IsNullOrEmpty(jSONNode["data"]["realname"].Value) && !string.IsNullOrEmpty(jSONNode["data"]["identity_card"].Value);
				Singleton<NetworkManager>.Instance.RequestBindAccount();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(jSONNode["msg"]));
			}
		}

		public override bool Pay(string productID, string productName, float productPrice)
		{
			if (!base.Pay(productID, productName, productPrice))
			{
				return false;
			}
			switch (Singleton<AccountManager>.Instance.accountConfig.paymentBranch)
			{
			case ConfigAccount.PaymentBranch.APPSTORE_CN:
				_accountDelegate.pay(productID, productName, productPrice, Guid.NewGuid().ToString("N"), Singleton<PlayerModule>.Instance.playerData.userId.ToString(), string.Empty, string.Empty, string.Empty, null);
				break;
			case ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY:
				switch (Singleton<ChannelPayModule>.Instance.GetPayMethodId())
				{
				case ChannelPayModule.PayMethod.ALIPAY:
					_accountDelegate.pay(productID, productName, productPrice, Guid.NewGuid().ToString("N"), Singleton<PlayerModule>.Instance.playerData.userId.ToString(), Singleton<NetworkManager>.Instance.DispatchSeverData.oaServerUrl + "/callback/alipay", string.Empty, string.Empty, null);
					break;
				case ChannelPayModule.PayMethod.WEIXIN_PAY:
					(_accountDelegate as TheOriginalAccountDelegate).weixinPrepayOrderInfo = new TheOriginalAccountDelegate.WeixinPrepayOrderInfo
					{
						productID = productID,
						productName = productName,
						productPrice = productPrice
					};
					Singleton<NetworkManager>.Instance.RequestCreateWeiXinOrder(productID, productName, productPrice);
					break;
				}
				break;
			}
			return true;
		}

		public bool PayByWeiXinOrder(string partnerID, string prepayID, string nonceStr, string timestamp, string sign)
		{
			TheOriginalAccountDelegate.WeixinPrepayOrderInfo weixinPrepayOrderInfo = (_accountDelegate as TheOriginalAccountDelegate).weixinPrepayOrderInfo;
			weixinPrepayOrderInfo.partnerID = partnerID;
			weixinPrepayOrderInfo.prepayID = prepayID;
			weixinPrepayOrderInfo.partnerID = partnerID;
			weixinPrepayOrderInfo.nonceStr = nonceStr;
			weixinPrepayOrderInfo.timestamp = timestamp;
			weixinPrepayOrderInfo.sign = sign;
			_accountDelegate.pay(weixinPrepayOrderInfo.productID, weixinPrepayOrderInfo.productName, weixinPrepayOrderInfo.productPrice, string.Empty, Singleton<PlayerModule>.Instance.playerData.userId.ToString(), Singleton<NetworkManager>.Instance.DispatchSeverData.oaServerUrl + "/callback/weixin", string.Empty, string.Empty, null);
			return true;
		}

		public override void PayFinishedCallBack(string param)
		{
			if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY)
			{
				PayResult payResult = null;
				switch (Singleton<ChannelPayModule>.Instance.GetPayMethodId())
				{
				case ChannelPayModule.PayMethod.ALIPAY:
					payResult = new AliPayResult(param);
					break;
				case ChannelPayModule.PayMethod.WEIXIN_PAY:
					payResult = new WeixinPayResult(param);
					break;
				}
				if (payResult != null)
				{
					Singleton<ChannelPayModule>.Instance.OnPurchaseCallback(payResult);
				}
			}
		}

		public override void ShowAccountPage()
		{
		}

		public override void ShowExitUI()
		{
			if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
			{
				Singleton<MainUIManager>.Instance.SceneCanvas.ShowQuitGameDialog();
			}
		}

		public override void DoExit()
		{
			Application.Quit();
		}

		public void GetUserInfoByToken()
		{
			string url = string.Format(ORIGINAL_GET_USER_INFO_BY_TOKEN_URL + "?uid={0}&token={1}", base.AccountUid, base.AccountToken);
			Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, GetUserInfoByTokenFinishedCallBack, delegate
			{
				WWWTimeOut(GetUserInfoByToken);
			}));
		}

		private void GetUserInfoByTokenFinishedCallBack(string param)
		{
			JSONNode jSONNode = JSON.Parse(param);
			if (jSONNode["retcode"].AsInt >= 0)
			{
				Name = jSONNode["data"]["name"];
				Email = jSONNode["data"]["email"];
				Mobile = jSONNode["data"]["mobile"];
				IsEmailVerify = jSONNode["data"]["is_email_verify"].AsInt > 0;
				IsRealNameVerify = !string.IsNullOrEmpty(jSONNode["data"]["realname"].Value) && !string.IsNullOrEmpty(jSONNode["data"]["identity_card"].Value);
				SaveAccountToLocal();
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged));
			}
		}

		public override void VerifyEmailApply()
		{
			string url = string.Format(ORIGINAL_VERIFY_EMAIL_APPLY_URL + "?uid={0}&token={1}", base.AccountUid, base.AccountToken);
			Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, VerifyEmailApplyFinishedCallBack, delegate
			{
				WWWTimeOut(VerifyEmailApply);
			}));
		}

		private void VerifyEmailApplyFinishedCallBack(string param)
		{
			JSONNode jSONNode = JSON.Parse(param);
			if (jSONNode["retcode"].AsInt >= 0)
			{
				Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasSendVerifyEmailApply = true;
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(jSONNode["msg"]));
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged));
				GetUserInfoByToken();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(jSONNode["msg"]));
			}
		}

		private void WWWTimeOut(Action OkBtnCallback)
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(LocalizationGeneralLogic.GetText("Menu_MihoyoAccountTimeOut"), null, OkBtnCallback, LocalizationGeneralLogic.GetText("Menu_Tips")));
		}
	}
}
