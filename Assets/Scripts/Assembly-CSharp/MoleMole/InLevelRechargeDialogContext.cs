using MoleMole.Config;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class InLevelRechargeDialogContext : BaseDialogContext
	{
		private InLevelReviveDialogContext _reviveContext;

		private MonoShopRechargeTab _rechargeTab;

		public InLevelRechargeDialogContext(InLevelReviveDialogContext reviveContext)
		{
			config = new ContextPattern
			{
				contextName = "InLevelRechargeDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/InLevelShopDialog"
			};
			_reviveContext = reviveContext;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 83)
			{
				return OnRechargeSuccNotify(pkt.getData<RechargeFinishNotify>());
			}
			return false;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.RefreshRechargeTab)
			{
				return OnRefreshRechargeTab();
			}
			if (ntf.type == NotifyTypes.SelectRechargeItem)
			{
				return OnSelectProduct((string)ntf.body);
			}
			return false;
		}

		protected override bool SetupView()
		{
			_reviveContext.SetActive(false);
			_rechargeTab = base.view.transform.Find("Dialog/RechargeTab").GetComponent<MonoShopRechargeTab>();
			_rechargeTab.SetupView();
			base.view.transform.Find("Dialog/CartInfoPanel/Info").gameObject.SetActive(false);
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnCancelBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/ActionBtns/CancelBtn").GetComponent<Button>(), OnCancelBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CartInfoPanel/BuyBtn").GetComponent<Button>(), OnBuyBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CartInfoPanel/Info/AndroidPayBtns/AilpayBtn").GetComponent<Button>(), OnAilpayBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CartInfoPanel/Info/AndroidPayBtns/WechatBtn").GetComponent<Button>(), OnWechatBtnClick);
		}

		public bool OnRefreshRechargeTab()
		{
			_rechargeTab.OnRefreshRechargeTab();
			return false;
		}

		public bool OnSelectProduct(string productID)
		{
			_rechargeTab.OnSelectProduct(productID);
			return false;
		}

		private void OnCancelBtnClick()
		{
			_reviveContext.SetActive(true);
			Destroy();
		}

		private void OnBuyBtnClick()
		{
			_rechargeTab.OnPucharseProduct();
		}

		private bool OnRechargeSuccNotify(RechargeFinishNotify rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode == 0 || (int)rsp.retcode == 2)
			{
				_reviveContext.SetActive(true);
				_reviveContext.RefreshView();
				Destroy();
			}
			return false;
		}

		public void OnAilpayBtnClick()
		{
			_rechargeTab.SetPayMethodId(ChannelPayModule.PayMethod.ALIPAY);
		}

		public void OnWechatBtnClick()
		{
			_rechargeTab.SetPayMethodId(ChannelPayModule.PayMethod.WEIXIN_PAY);
		}
	}
}
