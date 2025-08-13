using MoleMole.Config;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MihoyoRegisterDialogContext : BaseDialogContext
	{
		public MihoyoRegisterDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "MihoyoRegisterDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/MiHoYoAccount/MihoyoRegisterDialog"
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

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.MihoyoAccountRegisterSuccess)
			{
				return OnMihoyoAccountRegisterSuccess((Tuple<string, string>)ntf.body);
			}
			if (ntf.type == NotifyTypes.MihoyoAccountLoginSuccess)
			{
				Close();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/EmailBtn").GetComponent<Button>(), OnEmailBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/PhoneBtn").GetComponent<Button>(), OnPhoneBtnClick);
		}

		protected override bool SetupView()
		{
			return false;
		}

		private bool OnMihoyoAccountRegisterSuccess(Tuple<string, string> registerData)
		{
			if (Singleton<PlayerModule>.Instance.playerData.userId > 0)
			{
				Singleton<AccountManager>.Instance.manager.BindUIFinishedCallBack(registerData.Item1, registerData.Item2);
			}
			else
			{
				Singleton<AccountManager>.Instance.manager.LoginUIFinishedCallBack(registerData.Item1, registerData.Item2);
			}
			return false;
		}

		public bool OnBindAccountRsp(BindAccountRsp rsp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			Close();
			if ((int)rsp.retcode == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_BindSuccess")));
			}
			else
			{
				string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput));
			}
			return false;
		}

		public void OnEmailBtnClick()
		{
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				WebViewGeneralLogic.LoadUrl(theOriginalAccountManager.ORIGINAL_EMAIL_REGISTER_URL);
			}
		}

		public void OnPhoneBtnClick()
		{
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				WebViewGeneralLogic.LoadUrl(theOriginalAccountManager.ORIGINAL_MOBILE_REGISTER_URL);
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
