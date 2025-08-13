using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class ChangeSelfDescDialogContext : BaseDialogContext
	{
		private const int MAX_LENGTH = 40;

		public ChangeSelfDescDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "ChangeSelfDescDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/ChangeSelfDescDialog"
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), Close);
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 79)
			{
				return OnSetSelfDescRsp(pkt.getData<SetSelfDescRsp>());
			}
			return false;
		}

		protected override bool SetupView()
		{
			InputField component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>();
			component.characterLimit = 0;
			component.GetComponent<InputFieldHelper>().mCharacterlimit = 40;
			component.text = Singleton<PlayerModule>.Instance.playerData.SelfDescText;
			return false;
		}

		public void OnOKButtonCallBack()
		{
			string text = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text.Trim();
			int length = Mathf.Min(40, text.Length);
			text = text.Substring(0, length);
			if (!string.IsNullOrEmpty(text))
			{
				Singleton<NetworkManager>.Instance.RequestSelfDescChange(text);
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

		public bool OnSetSelfDescRsp(SetSelfDescRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Close();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			return false;
		}
	}
}
