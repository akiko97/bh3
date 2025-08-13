using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class ChangeChatRoomDialogContext : BaseDialogContext
	{
		public ChangeChatRoomDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "ChangeChatRoomDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/Chat/ChangeChatRoomDialog"
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
			if (cmdId == 89)
			{
				return OnEnterWorldChatroomRsp(pkt.getData<EnterWorldChatroomRsp>());
			}
			return false;
		}

		protected override bool SetupView()
		{
			InputField component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>();
			component.text = Singleton<ChatModule>.Instance.chatRoomId.ToString();
			Text component2 = base.view.transform.Find("Dialog/Content/RoomNumText").GetComponent<Text>();
			if (component2 != null)
			{
				int chatRoomMinNum = MiscData.Config.ChatConfig.ChatRoomMinNum;
				int chatRoomMaxNum = MiscData.Config.ChatConfig.ChatRoomMaxNum;
				component2.text = string.Format("[{0}-{1}]", chatRoomMinNum, chatRoomMaxNum);
			}
			return false;
		}

		public bool OnEnterWorldChatroomRsp(EnterWorldChatroomRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Close();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)));
			}
			return false;
		}

		public void OnOKButtonCallBack()
		{
			string text = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text;
			text = text.Trim();
			if (!string.IsNullOrEmpty(text))
			{
				int result = 0;
				if (!int.TryParse(text, out result))
				{
					ShowErrMsg(LocalizationGeneralLogic.GetText("Err_ChatRoomIdWrong"));
				}
				else if (result <= 0 || result > MiscData.Config.ChatConfig.ChatRoomMaxNum)
				{
					ShowErrMsg(LocalizationGeneralLogic.GetText("Err_ChatRoomIdWrong"));
				}
				else if (result == Singleton<ChatModule>.Instance.chatRoomId)
				{
					ShowErrMsg(LocalizationGeneralLogic.GetText("Err_ChatRoomIdTheSame"));
				}
				else
				{
					Singleton<NetworkManager>.Instance.RequestEnterWorldChatroom(result);
				}
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

		private void ShowErrMsg(string msg)
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(msg));
		}
	}
}
