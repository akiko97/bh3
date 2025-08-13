using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class MihoyoForgetPwdDialogContext : BaseDialogContext
	{
		public MihoyoForgetPwdDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "MihoyoForgetPwdDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/MiHoYoAccount/MiHoYoForgetPswDialog"
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/UsePhoneBtn").GetComponent<Button>(), OnUsePhoneBtnCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/UseEmailBtn").GetComponent<Button>(), OnUseEmailBtnCallBack);
		}

		protected override bool SetupView()
		{
			return false;
		}

		public void OnUsePhoneBtnCallBack()
		{
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				WebViewGeneralLogic.LoadUrl(theOriginalAccountManager.ORIGINAL_MOBILE_PASSWORD_URL);
			}
		}

		public void OnUseEmailBtnCallBack()
		{
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				WebViewGeneralLogic.LoadUrl(theOriginalAccountManager.ORIGINAL_EMAIL_PASSWORD_URL);
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
	}
}
