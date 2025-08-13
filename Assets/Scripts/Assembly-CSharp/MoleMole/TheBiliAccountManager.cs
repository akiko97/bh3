using System;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class TheBiliAccountManager : TheBaseAccountManager
	{
		private const int SUCCESS_RET_CODE = 10010;

		private const int CANCEL_RET_CODE = 6001;

		private const int LOGIN_ALERT_TITLE_TEXT_ID = 10217;

		private const int LOGIN_ALERT_DESP_TEXT_ID = 10218;

		private const int LOGIN_ALERT_CONFIRM_BUTTON_TEXT_ID = 10215;

		private const int LOGIN_ALERT_CANCEL_BUTTON_TEXT_ID = 10040;

		private const int LOGIN_ALERT_2_TITLE_TEXT_ID = 10234;

		private const int LOGIN_ALERT_2_DESP_TEXT_ID = 10235;

		private const int LOGIN_ALERT_2_CONFIRM_BUTTON_TEXT_ID = 10236;

		private const int LOGIN_ALERT_2_CANCEL_BUTTON_TEXT_ID = 10040;

		private const int LOGIN_ALERT_3_TITLE_TEXT_ID = 10237;

		private const int LOGIN_ALERT_3_DESP_TEXT_ID = 10238;

		private const int LOGIN_ALERT_3_CONFIRM_BUTTON_TEXT_ID = 10215;

		private const int LOGIN_ALERT_3_CANCEL_BUTTON_TEXT_ID = 10040;

		private const bool DebugBuild = false;

		public TheBiliAccountManager()
		{
			_accountDelegate = new TheBiliAccountDelegate();
		}

		public override void Init()
		{
			base.AccountType = (AccountType)2;
			_accountDelegate.init(false, string.Empty, string.Empty, null);
		}

		public override void InitFinishedCallBack(string param)
		{
		}

		public override void LoginUI()
		{
			_accountDelegate.login("AccountEventListener", "LoginTestFinishedCallBack", string.Empty, string.Empty, null);
		}

		public override void LoginTestFinishedCallBack(string param)
		{
			int result = 0;
			if (int.TryParse(param, out result))
			{
				if (result == 10010)
				{
					base.AccountUid = _accountDelegate.getUid();
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SDKAccountLoginSuccess));
				}
				else
				{
					LoginUI();
				}
			}
		}

		public override void RegisterUI()
		{
			_accountDelegate.register("AccountEventListener", "RegisterUIFinishedCallBack", string.Empty, string.Empty, string.Empty, string.Empty, null);
		}

		public override void RegisterUIFinishedCallBack(string arg1, string arg2, string arg3, string arg4)
		{
			int result = 0;
			if (int.TryParse(arg1, out result) && result == 10010)
			{
				LoginUI();
			}
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
			if (!int.TryParse(param, out result) || result == 1000 || result == 6001)
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
		}

		public override void HideToolBar()
		{
		}

		public override void ShowPausePage()
		{
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
	}
}
