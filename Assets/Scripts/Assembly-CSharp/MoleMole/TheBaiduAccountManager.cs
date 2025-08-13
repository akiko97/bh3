using System;
using proto;

namespace MoleMole
{
	public class TheBaiduAccountManager : TheBaseAccountManager
	{
		public bool DebugBuild;

		public TheBaiduAccountManager()
		{
			_accountDelegate = new TheBaiduAccountDelegate();
		}

		public override void Init()
		{
			base.AccountType = (AccountType)14;
			_accountDelegate.init(DebugBuild, "AccountEventListener", "InitFinishedCallBack", null);
			InitFinishedCallBack(string.Empty);
		}

		public override void InitFinishedCallBack(string param)
		{
			if (_accountDelegate is TheBaiduAccountDelegate)
			{
				(_accountDelegate as TheBaiduAccountDelegate).setSwitchAccountListener("AccountEventListener", "SwitchAccountFinishedCallBack");
			}
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
			int result = 0;
			if (int.TryParse(param, out result) && result == 1)
			{
				if (!string.IsNullOrEmpty(base.AccountUid))
				{
					SwitchAccountFinishedCallBack(param);
					return;
				}
				base.AccountUid = _accountDelegate.getUid();
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SDKAccountLoginSuccess));
			}
			else
			{
				LoginUI();
			}
		}

		public override void SwitchAccountFinishedCallBack(string param)
		{
			GeneralLogicManager.RestartGame();
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
			_accountDelegate.pay(productID, productName, productPrice, Guid.NewGuid().ToString("N"), userId.ToString(), base.ChannelRegion, "AccountEventListener", "PayFinishedCallBack", null);
			return false;
		}

		public override void PayFinishedCallBack(string param)
		{
			SDKPayResult sDKPayResult = new SDKPayResult();
			int result = 0;
			if (!int.TryParse(param, out result) || result == 2)
			{
				sDKPayResult.payRetCode = PayResult.PayRetcode.CANCELED;
			}
			else if (!int.TryParse(param, out result) || result == 0)
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
			_accountDelegate.exit();
		}

		public override void DoExit()
		{
		}
	}
}
