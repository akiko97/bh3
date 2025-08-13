using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class ChangeNicknameDialogContext : BaseDialogContext
	{
		private const int MAX_LENGTH = 8;

		public ChangeNicknameDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "ChangeNicknameDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/ChangeNicknameDialog"
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
			if (cmdId == 21)
			{
				return OnNicknameModifyRsp(pkt.getData<NicknameModifyRsp>());
			}
			return false;
		}

		protected override bool SetupView()
		{
			InputField component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>();
			component.characterLimit = 0;
			component.GetComponent<InputFieldHelper>().mCharacterlimit = 8;
			component.text = Singleton<PlayerModule>.Instance.playerData.nickname;
			return false;
		}

		public bool OnNicknameModifyRsp(NicknameModifyRsp rsp)
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

		public void OnOKButtonCallBack()
		{
			string text = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text.Trim();
			int length = Mathf.Min(8, text.Length);
			text = text.Substring(0, length);
			if (!string.IsNullOrEmpty(text))
			{
				Singleton<NetworkManager>.Instance.RequestNicknameChange(text);
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
