using System;
using proto;

namespace MoleMole
{
	public class TheUCAccountManager : TheBaseAccountManager
	{
		private const string RESPONSE_SUCCESS_CODE = "1";

		private const string RESPONSE_FAIL_CODE = "2";

		private const string RESPONSE_LOGIN_OUT = "3";

		private const string RESPONSE_PAY_QUIT = "4";

		private const string RESPONSE_UNINIT = "5";

		private const string RESPONSE_PAY_CANCEL = "6";

		private const bool DebugBuild = false;

		public TheUCAccountManager()
		{
			_accountDelegate = new TheUCAccountDelegate();
		}

		public override void Init()
		{
			base.AccountType = (AccountType)3;
			_accountDelegate.init(false, "AccountEventListener", "InitFinishedCallBack", null);
		}

		public override void InitFinishedCallBack(string param)
		{
		}

		public override void LoginUI()
		{
			_accountDelegate.login("AccountEventListener", "LoginTestFinishedCallBack", string.Empty, string.Empty, null);
		}

		public override void LoginUIFinishedCallBack(string arg1, string arg2)
		{
		}

		protected override void LoginTest()
		{
		}

		public override void LoginTestFinishedCallBack(string param)
		{
			if (param == "1")
			{
				base.AccountUid = _accountDelegate.getUid();
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SDKAccountLoginSuccess));
			}
			else
			{
				LoginUI();
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
		}

		public override void BindUIFinishedCallBack(string arg1, string arg2)
		{
		}

		protected override void BindTest()
		{
		}

		public override void BindTestFinishedCallBack(string param)
		{
		}

		public override bool Pay(string productID, string productName, float productPrice)
		{
			if (!base.Pay(productID, productName, productPrice))
			{
				return false;
			}
			int userId = Singleton<PlayerModule>.Instance.playerData.userId;
			string oaServerUrl = Singleton<NetworkManager>.Instance.DispatchSeverData.oaServerUrl;
			_accountDelegate.pay(productID, productName, productPrice, Guid.NewGuid().ToString("N"), userId.ToString(), oaServerUrl + "/callback/uc", "AccountEventListener", "PayFinishedCallBack", null);
			return false;
		}

		public override void PayFinishedCallBack(string param)
		{
			SDKPayResult sDKPayResult = new SDKPayResult();
			if (param == "4")
			{
				sDKPayResult.payRetCode = PayResult.PayRetcode.CANCELED;
			}
			else if (param == "2")
			{
				sDKPayResult.payRetCode = PayResult.PayRetcode.FAILED;
			}
			else
			{
				sDKPayResult.payRetCode = PayResult.PayRetcode.SUCCESS;
			}
			Singleton<ChannelPayModule>.Instance.OnPurchaseCallback(sDKPayResult);
		}

		public override void ShowToolBar()
		{
			_accountDelegate.showToolBar();
		}

		public override void HideToolBar()
		{
			_accountDelegate.hideToolBar();
		}

		public override void ShowPausePage()
		{
			_accountDelegate.showPausePage();
		}

		public override void ShowAccountPage()
		{
		}

		public override void ShowExitUI()
		{
			DoExit();
		}

		public override void DoExit()
		{
			_accountDelegate.exit();
		}
	}
}
