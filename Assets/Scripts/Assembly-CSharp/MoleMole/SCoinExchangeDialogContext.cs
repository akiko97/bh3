using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class SCoinExchangeDialogContext : BaseDialogContext
	{
		public SCoinExchangeDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "SCoinExchangeDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/ScoinExchangeDialog"
			};
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 13)
			{
				return SetupView();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			PlayerScoinExchangeInfo value = Singleton<PlayerModule>.Instance.playerData.scoinExchangeCache.Value;
			if (value == null)
			{
				return false;
			}
			base.view.transform.Find("Dialog/Content/Times/NumText").GetComponent<Text>().text = string.Format("({0}/{1})", value.usableTimes, value.totalTimes);
			base.view.transform.Find("Dialog/Content/Exchange/HCoinNumText").GetComponent<Text>().text = value.hcoinCost.ToString();
			base.view.transform.Find("Dialog/Content/Exchange/SCoinNumText").GetComponent<Text>().text = value.scoinGet.ToString();
			return false;
		}

		public void OnOKButtonCallBack()
		{
			Singleton<NetworkManager>.Instance.RequestScoinExchange();
			Destroy();
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public void Close()
		{
			Destroy();
		}
	}
}
