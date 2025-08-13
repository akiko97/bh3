using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class ShopWelfareModule : BaseModule
	{
		private bool _readyToHint;

		private List<WelfareDataItem> _welfareDataItemList;

		public int totalPayHCoin { get; private set; }

		public ShopWelfareModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_welfareDataItemList = new List<WelfareDataItem>();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 198:
				return OnGetVipRewardDataRsp(pkt.getData<GetVipRewardDataRsp>());
			case 200:
				return OnGetVipRewardRsp(pkt.getData<GetVipRewardRsp>());
			default:
				return false;
			}
		}

		public List<WelfareDataItem> GetWelfareDataItemList()
		{
			return _welfareDataItemList;
		}

		private bool OnGetVipRewardDataRsp(GetVipRewardDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode == 0)
			{
				_welfareDataItemList.Clear();
				totalPayHCoin = (int)rsp.total_pay_hcoin;
				List<VipReward> vip_reward_list = rsp.vip_reward_list;
				foreach (VipReward item in vip_reward_list)
				{
					_welfareDataItemList.Add(new WelfareDataItem(item));
				}
				if (!_readyToHint)
				{
					_readyToHint = true;
					int i = 0;
					for (int count = _welfareDataItemList.Count; i < count; i++)
					{
						_readyToHint &= (int)_welfareDataItemList[i].rewardStatus == 1;
					}
				}
				SortVipRewardList();
			}
			foreach (WelfareDataItem welfareDataItem in _welfareDataItemList)
			{
			}
			return false;
		}

		private bool OnGetVipRewardRsp(GetVipRewardRsp rsp)
		{
			Singleton<NetworkManager>.Instance.RequestGetVipRewardData();
			return false;
		}

		private void SortVipRewardList()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Invalid comparison between Unknown and I4
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Invalid comparison between Unknown and I4
			List<WelfareDataItem> list = new List<WelfareDataItem>();
			List<WelfareDataItem> list2 = new List<WelfareDataItem>();
			List<WelfareDataItem> list3 = new List<WelfareDataItem>();
			foreach (WelfareDataItem welfareDataItem in _welfareDataItemList)
			{
				if ((int)welfareDataItem.rewardStatus == 2)
				{
					list.Add(welfareDataItem);
				}
				else if ((int)welfareDataItem.rewardStatus == 1)
				{
					list2.Add(welfareDataItem);
				}
				else if ((int)welfareDataItem.rewardStatus == 3)
				{
					list3.Add(welfareDataItem);
				}
			}
			list.Sort((WelfareDataItem lo, WelfareDataItem ro) => lo.payHCoin - ro.payHCoin);
			list2.Sort((WelfareDataItem lo, WelfareDataItem ro) => lo.payHCoin - ro.payHCoin);
			list3.Sort((WelfareDataItem lo, WelfareDataItem ro) => lo.payHCoin - ro.payHCoin);
			int num = 0;
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				_welfareDataItemList[num] = list[num2];
				num++;
			}
			for (int num3 = 0; num3 < list2.Count; num3++)
			{
				_welfareDataItemList[num] = list2[num3];
				num++;
			}
			for (int num4 = 0; num4 < list3.Count; num4++)
			{
				_welfareDataItemList[num] = list3[num4];
				num++;
			}
		}

		public bool HasWelfareCanGet()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Invalid comparison between Unknown and I4
			foreach (WelfareDataItem welfareDataItem in _welfareDataItemList)
			{
				if ((int)welfareDataItem.rewardStatus == 2)
				{
					return true;
				}
			}
			return false;
		}

		public void TryHintNewWelfare()
		{
			if (_readyToHint && HasWelfareCanGet())
			{
				_readyToHint = false;
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralConfirmDialogContext
				{
					type = GeneralConfirmDialogContext.ButtonType.SingleButton,
					desc = LocalizationGeneralLogic.GetText("Menu_WelfareGuideTitle")
				});
			}
		}
	}
}
