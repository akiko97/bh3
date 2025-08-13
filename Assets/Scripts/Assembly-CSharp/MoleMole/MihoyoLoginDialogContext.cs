using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class MihoyoLoginDialogContext : BaseDialogContext
	{
		private const int MAX_LENGTH = 50;

		public MihoyoLoginDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "MihoyoLoginDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/MiHoYoAccount/MiHoYoLoginDialog"
			};
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.MihoyoAccountLoginSuccess)
			{
				Close();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ForgetPswLink").GetComponent<Button>(), OnForgetPswLinkCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ModifyPswLink").GetComponent<Button>(), OnModifyPswLinkCallBack);
		}

		protected override bool SetupView()
		{
			InputField component = base.view.transform.Find("Dialog/Content/AccountInputField").GetComponent<InputField>();
			InputField component2 = base.view.transform.Find("Dialog/Content/PswInputField").GetComponent<InputField>();
			component.characterLimit = 50;
			component2.characterLimit = 50;
			string lastLoginAccountName = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginAccountName;
			if (!string.IsNullOrEmpty(lastLoginAccountName))
			{
				component.text = lastLoginAccountName;
			}
			return false;
		}

		public void OnOKButtonCallBack()
		{
			string text = base.view.transform.Find("Dialog/Content/AccountInputField").GetComponent<InputField>().text;
			string text2 = base.view.transform.Find("Dialog/Content/PswInputField").GetComponent<InputField>().text;
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				Singleton<AccountManager>.Instance.manager.LoginUIFinishedCallBack(text, text2);
			}
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		public void Close()
		{
			Destroy();
		}

		public void OnForgetPswLinkCallBack()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoForgetPwdDialogContext());
		}

		public void OnModifyPswLinkCallBack()
		{
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				WebViewGeneralLogic.LoadUrl(theOriginalAccountManager.ORIGINAL_CHANGE_PASSWORD_URL);
			}
		}
	}
}
