using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class AddFriendDialogContext : BaseSequenceDialogContext
	{
		private const float TIMER_SPAN = 1f;

		private FriendDetailDataItem _friendDetailData;

		private CanvasTimer _timer;

		public AddFriendDialogContext(FriendDetailDataItem friendDetailData)
		{
			config = new ContextPattern
			{
				contextName = "AddFriendDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AddFriendDialog"
			};
			_friendDetailData = friendDetailData;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnCloseButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/AddBtn").GetComponent<Button>(), OnAddButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/Portrait/Button").GetComponent<Button>(), OnPortraitButtonCallBack);
		}

		protected override bool SetupView()
		{
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(1f, 0f);
			_timer.timeUpCallback = Close;
			_timer.StopRun();
			Transform transform = base.view.transform.Find("Dialog/Content/");
			AvatarDataItem leaderAvatar = _friendDetailData.leaderAvatar;
			transform.Find("Portrait/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(leaderAvatar.IconPath);
			Color color = UIUtil.SetupColor(MiscData.Config.AvatarAttributeColorList[leaderAvatar.Attribute]);
			transform.Find("Portrait/BG").GetComponent<Image>().color = color;
			transform.Find("Stars/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(leaderAvatar.star);
			transform.Find("NickName/Text").GetComponent<Text>().text = _friendDetailData.nickName;
			transform.Find("Level/Text").GetComponent<Text>().text = string.Format("LV.{0}", _friendDetailData.level);
			transform.Find("CombatValue/Text").GetComponent<Text>().text = Mathf.FloorToInt(leaderAvatar.CombatNum).ToString();
			transform.Find("SelfDesc/Text").GetComponent<Text>().text = _friendDetailData.Desc;
			return false;
		}

		private void OnCloseButtonCallBack()
		{
			Close();
		}

		private void OnAddButtonCallBack()
		{
			Singleton<NetworkManager>.Instance.RequestAddFriend(_friendDetailData.uid);
			base.view.transform.Find("Dialog/Content/AddBtn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_RequestSend");
			base.view.transform.Find("Dialog/Content/AddBtn").GetComponent<Button>().interactable = false;
			_timer.StartRun();
		}

		private void OnPortraitButtonCallBack()
		{
			FriendDetailDataItem friendDetailDataItem = Singleton<FriendModule>.Instance.TryGetFriendDetailData(_friendDetailData.uid);
			if (friendDetailDataItem != null)
			{
				RemoteAvatarDetailPageContext context = new RemoteAvatarDetailPageContext(friendDetailDataItem);
				Singleton<MainUIManager>.Instance.ShowPage(context);
			}
		}

		private void Close()
		{
			Destroy();
		}

		public override void Destroy()
		{
			_timer.Destroy();
			base.Destroy();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 67)
			{
				return OnAddFriendRsp(pkt.getData<AddFriendRsp>());
			}
			return false;
		}

		private bool OnAddFriendRsp(AddFriendRsp rsp)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected I4, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected I4, but got Unknown
			int target_uid = (int)rsp.target_uid;
			string desc = string.Empty;
            proto.AddFriendRsp.Retcode retcode = rsp.retcode;
			switch ((int)retcode)
			{
			case 0:
			{
				string text = Singleton<FriendModule>.Instance.TryGetPlayerNickName(target_uid);
				AddFriendAction action = rsp.action;
				switch ((int)action - 1)
				{
				case 1:
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_AgreeFriend", text);
					break;
				case 2:
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_RejectFriend", text);
					break;
				case 0:
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_RequestAddFriend", text);
					break;
				}
				break;
			}
			case 3:
				desc = LocalizationGeneralLogic.GetText("Err_FriendFull");
				break;
			case 4:
				desc = LocalizationGeneralLogic.GetText("Err_TargetFriendFull");
				break;
			case 6:
				desc = LocalizationGeneralLogic.GetText("Err_IsFriend");
				break;
			case 5:
				desc = LocalizationGeneralLogic.GetText("Err_IsSelf");
				break;
			case 1:
				desc = LocalizationGeneralLogic.GetText("Err_FailToAddFriend");
				break;
			case 7:
				desc = LocalizationGeneralLogic.GetText("Err_AskTooOften");
				break;
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(desc));
			return false;
		}
	}
}
