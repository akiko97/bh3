using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class SearchFriendDialogContext : BaseDialogContext
	{
		public SearchFriendDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "SearchFriendDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/SearchFriendDialog"
			};
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 73)
			{
				GetPlayerDetailDataRsp data = pkt.getData<GetPlayerDetailDataRsp>();
				OnSearchFriendDetailInfoRsp(data);
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/OKBtn").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Content/ErrHintText").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/SelfID/SelfIDNum").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
			return false;
		}

		public void OnOKButtonCallBack()
		{
			string text = base.view.transform.Find("Dialog/Content/SearchID/InputField").GetComponent<InputField>().text;
			int result;
			if (int.TryParse(text, out result))
			{
				if (result == Singleton<PlayerModule>.Instance.playerData.userId)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_SearchSelf")));
					return;
				}
				if (Singleton<FriendModule>.Instance.IsMyFriend(result))
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_IsFriend")));
					return;
				}
				FriendDetailDataItem friendDetailDataItem = Singleton<FriendModule>.Instance.TryGetFriendDetailData(result);
				if (friendDetailDataItem == null)
				{
					Singleton<NetworkManager>.Instance.RequestFriendDetailInfo(result);
					return;
				}
				Singleton<MainUIManager>.Instance.ShowDialog(new SearchedFriendDetailDialogContext(friendDetailDataItem));
				Close();
			}
			else
			{
				base.view.transform.Find("Dialog/Content/ErrHintText").gameObject.SetActive(true);
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

		private bool OnSearchFriendDetailInfoRsp(GetPlayerDetailDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected I4, but got Unknown
			Retcode retcode = rsp.retcode;
			switch ((int)retcode)
			{
			case 0:
				Singleton<MainUIManager>.Instance.ShowDialog(new SearchedFriendDetailDialogContext(new FriendDetailDataItem(rsp.detail)));
				Close();
				break;
			case 1:
			case 2:
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_TargetNotExist")));
				break;
			}
			return false;
		}
	}
}
