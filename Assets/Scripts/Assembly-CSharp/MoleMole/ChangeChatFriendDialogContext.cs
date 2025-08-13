using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class ChangeChatFriendDialogContext : BaseDialogContext
	{
		public ChangeChatFriendDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "ChangeChatFriendDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/Chat/ChangeChatFriendDialog"
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
			return false;
		}

		protected override bool SetupView()
		{
			return false;
		}

		public void OnOKButtonCallBack()
		{
			string text = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text;
			text = text.Trim();
			if (!string.IsNullOrEmpty(text))
			{
				int result = 0;
				if (int.TryParse(text, out result) && result > 0)
				{
					Close();
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ChangeTalkingFriend, result));
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
