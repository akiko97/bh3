using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class GachaTicketLackDialogContext : BaseSequenceDialogContext
	{
		private int _ticketID;

		private int _wantedNum;

		private int _currentTicketNum;

		private int _ticketPrice;

		private int _lackTicketNum;

		private bool _hcoinEnough;

		public GachaTicketLackDialogContext(int ticketID, int totalNum)
		{
			config = new ContextPattern
			{
				contextName = "GachaTicketLackDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/GachaTicketLackDialog"
			};
			_ticketID = ticketID;
			_wantedNum = totalNum;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), OnCancelButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Destroy);
		}

		protected override bool SetupView()
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_ticketID);
			StorageDataItemBase storageDataItemBase = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(_ticketID);
			_currentTicketNum = ((storageDataItemBase != null) ? storageDataItemBase.number : 0);
			_ticketPrice = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[_ticketID];
			_lackTicketNum = _wantedNum - _currentTicketNum;
			_hcoinEnough = Singleton<PlayerModule>.Instance.playerData.hcoin >= _lackTicketNum * _ticketPrice;
			string text = ((!_hcoinEnough) ? LocalizationGeneralLogic.GetText("Menu_GoToRecharge") : LocalizationGeneralLogic.GetText("Menu_Buy"));
			string text2 = ((!_hcoinEnough) ? LocalizationGeneralLogic.GetText("Menu_GoToRechargeDesc") : LocalizationGeneralLogic.GetText("Menu_Desc_GachaTicketLack", dummyStorageDataItem.GetDisplayTitle(), _lackTicketNum * _ticketPrice, _lackTicketNum, dummyStorageDataItem.GetDisplayTitle()));
			base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn/Text").GetComponent<Text>().text = text;
			base.view.transform.Find("Dialog/Content/Desc/DescText").GetComponent<Text>().text = text2;
			string iconPath = dummyStorageDataItem.GetIconPath();
			base.view.transform.Find("Dialog/Content/TicketIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(iconPath);
			base.view.transform.Find("Dialog/Content/CurretnTickets/Content/TicketLabel").GetComponent<Text>().text = dummyStorageDataItem.GetDisplayTitle();
			base.view.transform.Find("Dialog/Content/CurretnTickets/Content/Num").GetComponent<Text>().text = _currentTicketNum.ToString();
			base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_GachaTitcketLack", dummyStorageDataItem.GetDisplayTitle());
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 216)
			{
				OnBuyGachaTicketRsp(pkt.getData<BuyGachaTicketRsp>());
			}
			return false;
		}

		public void OnOKButtonCallBack()
		{
			if (_hcoinEnough)
			{
				Singleton<NetworkManager>.Instance.RequestBuyGachaTicket(_ticketID, _lackTicketNum);
				Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(216));
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext());
				Destroy();
			}
		}

		public void OnCancelButtonCallBack()
		{
			Destroy();
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public bool OnBuyGachaTicketRsp(BuyGachaTicketRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_ticketID);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_BuyGachaTicketSuccess", _lackTicketNum, dummyStorageDataItem.GetDisplayTitle())));
			}
			Destroy();
			return false;
		}
	}
}
