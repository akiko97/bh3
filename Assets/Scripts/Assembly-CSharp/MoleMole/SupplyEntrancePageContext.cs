using MoleMole.Config;
using UnityEngine.UI;

namespace MoleMole
{
	public class SupplyEntrancePageContext : BasePageContext
	{
		public SupplyEntrancePageContext()
		{
			config = new ContextPattern
			{
				contextName = "SupplyEntrancePageContext",
				viewPrefabPath = "UI/Menus/Page/SupplyEntrancePage",
				cacheType = ViewCacheType.AlwaysCached
			};
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 198)
			{
				return SetupWelfareHint();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Btn/BtnSupply/Button").GetComponent<Button>(), OnGachaButtonClick);
			BindViewCallback(base.view.transform.Find("Btn/BtnShop/Button").GetComponent<Button>(), OnShopButtonClick);
			BindViewCallback(base.view.transform.Find("Btn/BtnRecharge/Button").GetComponent<Button>(), OnRechargeButtonClick);
		}

		protected override bool SetupView()
		{
			SetupGacha();
			SetupWelfareHint();
			return false;
		}

		private void OnGachaButtonClick()
		{
			Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
			Singleton<MainUIManager>.Instance.ShowPage(new GachaMainPageContext());
		}

		private void OnShopButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ShopPageContext());
		}

		private void OnRechargeButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext());
		}

		private void SetupGacha()
		{
			int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
			int gachaUnlockNeedPlayerLevel = MiscData.Config.BasicConfig.GachaUnlockNeedPlayerLevel;
			bool flag = teamLevel < gachaUnlockNeedPlayerLevel;
			base.view.transform.Find("Btn/BtnSupply").gameObject.SetActive(!flag);
			base.view.transform.Find("Btn/BtnSupply/Locked").gameObject.SetActive(false);
		}

		private bool SetupWelfareHint()
		{
			bool active = Singleton<ShopWelfareModule>.Instance.HasWelfareCanGet();
			base.view.transform.Find("Btn/BtnRecharge/PopUp").gameObject.SetActive(active);
			return false;
		}
	}
}
