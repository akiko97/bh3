using proto;

namespace MoleMole
{
	public class WelfareDataItem
	{
		public int vipLevel;

		public int payHCoin;

		public int rewardID;

		public VipRewardStatus rewardStatus;

		public WelfareDataItem(WelfareDataItem welfareItem)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			vipLevel = welfareItem.vipLevel;
			payHCoin = welfareItem.payHCoin;
			rewardID = welfareItem.rewardID;
			rewardStatus = welfareItem.rewardStatus;
		}

		public WelfareDataItem(VipReward welfareItem)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			vipLevel = (int)welfareItem.vip_level;
			payHCoin = (int)welfareItem.pay_hcoin;
			rewardID = (int)welfareItem.reward_id;
			rewardStatus = welfareItem.status;
		}

		public override string ToString()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			return string.Format("<WelfareDataItem>\nID: {0}\npayHCoin: {1}\nrewardID: {2}\nrewardStatus: {3}", vipLevel, payHCoin, rewardID, rewardStatus);
		}
	}
}
