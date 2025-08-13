using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MihoyoBindAccountDialogContext : BaseDialogContext
	{
		private const int MAX_LENGTH = 50;

		public MihoyoBindAccountDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "MihoyoBindAccountDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/MiHoYoAccount/MiHoYoBindAccountDialog"
			};
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 120)
			{
				return OnBindAccountRsp(pkt.getData<BindAccountRsp>());
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
			return false;
		}

		public bool OnBindAccountRsp(BindAccountRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_BindSuccess")));
				Close();
			}
			else
			{
				string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput));
			}
			return false;
		}

		public void OnOKButtonCallBack()
		{
			string text = base.view.transform.Find("Dialog/Content/AccountInputField").GetComponent<InputField>().text;
			string text2 = base.view.transform.Find("Dialog/Content/PswInputField").GetComponent<InputField>().text;
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				Singleton<AccountManager>.Instance.manager.BindUIFinishedCallBack(text, text2);
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
