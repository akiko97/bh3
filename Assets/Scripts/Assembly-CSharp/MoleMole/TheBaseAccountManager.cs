using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public abstract class TheBaseAccountManager
	{
		public const string ACCOUNT_CALLBACK_LISTENER = "AccountEventListener";

		protected TheBaseAccountDelegate _accountDelegate;

		protected string _accountArg1 = string.Empty;

		protected string _accountArg2 = string.Empty;

		public AccountType AccountType { get; protected set; }

		public string AccountUid { get; protected set; }

		public string AccountToken { get; protected set; }

		public string AccountExt { get; protected set; }

		public string ChannelRegion { get; set; }

		public virtual void Reset()
		{
			_accountArg1 = string.Empty;
			_accountArg2 = string.Empty;
			AccountUid = string.Empty;
			AccountToken = string.Empty;
			AccountExt = string.Empty;
		}

		public bool IsAccountBind()
		{
			return !string.IsNullOrEmpty(AccountUid);
		}

		public virtual void SaveAccountToLocal()
		{
		}

		public virtual void SetupByDispatchServerData()
		{
		}

		protected void LoginAccount()
		{
		}

		protected void BindAccount()
		{
		}

		public bool IsForceLoginType()
		{
			return false;
		}

		public virtual string GetAccountName()
		{
			return string.Empty;
		}

		public abstract void Init();

		public virtual void InitFinishedCallBack(string param)
		{
		}

		public abstract void LoginUI();

		public virtual void LoginUIFinishedCallBack(string arg1, string arg2)
		{
		}

		protected virtual void LoginTest()
		{
		}

		public virtual void LoginTestFinishedCallBack(string param)
		{
		}

		public virtual void SwitchAccountFinishedCallBack(string param)
		{
		}

		public virtual void RegisterUI()
		{
		}

		public virtual void RegisterUIFinishedCallBack(string arg1, string arg2, string arg3, string arg4)
		{
		}

		protected virtual void RegisterTest()
		{
		}

		public virtual void RegisterTestFinishedCallBack(string param)
		{
		}

		public virtual void BindUI()
		{
		}

		public virtual void BindUIFinishedCallBack(string arg1, string arg2)
		{
		}

		protected virtual void BindTest()
		{
		}

		public virtual void BindTestFinishedCallBack(string param)
		{
		}

		public void ShowAllProducts()
		{
			Singleton<ApplicationManager>.Instance.StartCoroutine(Singleton<ChannelPayModule>.Instance.ShowAllProductsAsync());
		}

		public List<RechargeDataItem> GetRechargeItemList()
		{
			return Singleton<ChannelPayModule>.Instance.GetRechargeItemList();
		}

		public RechargeDataItem GetStoreItemByProductID(string productID)
		{
			return Singleton<ChannelPayModule>.Instance.GetStoreItemByProductID(productID);
		}

		public virtual bool Pay(string productID, string productName, float productPrice)
		{
			RechargeDataItem storeItemByProductID = GetStoreItemByProductID(productID);
			if (!storeItemByProductID.CanPurchase())
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTPurchaseLimit")));
				return false;
			}
			if (!Singleton<ChannelPayModule>.Instance.PreparePurchaseProduct(productID))
			{
				return false;
			}
			return true;
		}

		public virtual void PayFinishedCallBack(string param)
		{
		}

		public virtual void ShowToolBar()
		{
		}

		public virtual void HideToolBar()
		{
		}

		public virtual void ShowPausePage()
		{
		}

		public virtual void ShowAccountPage()
		{
		}

		public virtual void ShowExitUI()
		{
		}

		public virtual void DoExit()
		{
		}

		public virtual void ShowStorePage(int scanior)
		{
		}

		public virtual bool DestroyCurrentBeforeShowStorePage()
		{
			return true;
		}

		public virtual void OnRegister()
		{
		}

		public virtual void OnLogin()
		{
		}

		public virtual void VerifyEmailApply()
		{
		}
	}
}
