namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class RewardData : IHashable
	{
		public readonly int RewardID;

		public readonly int RewardExp;

		public readonly int RewardSCoin;

		public readonly int RewardHCoin;

		public readonly int RewardStamina;

		public readonly int RewardSkillPoint;

		public readonly int RewardFriendPoint;

		public readonly int RewardItem1ID;

		public readonly int RewardItem1Level;

		public readonly int RewardItem1Num;

		public readonly int RewardItem2ID;

		public readonly int RewardItem2Level;

		public readonly int RewardItem2Num;

		public readonly int RewardItem3ID;

		public readonly int RewardItem3Level;

		public readonly int RewardItem3Num;

		public readonly int RewardItem4ID;

		public readonly int RewardItem4Level;

		public readonly int RewardItem4Num;

		public readonly int RewardItem5ID;

		public readonly int RewardItem5Level;

		public readonly int RewardItem5Num;

		public RewardData(int RewardID, int RewardExp, int RewardSCoin, int RewardHCoin, int RewardStamina, int RewardSkillPoint, int RewardFriendPoint, int RewardItem1ID, int RewardItem1Level, int RewardItem1Num, int RewardItem2ID, int RewardItem2Level, int RewardItem2Num, int RewardItem3ID, int RewardItem3Level, int RewardItem3Num, int RewardItem4ID, int RewardItem4Level, int RewardItem4Num, int RewardItem5ID, int RewardItem5Level, int RewardItem5Num)
		{
			this.RewardID = RewardID;
			this.RewardExp = RewardExp;
			this.RewardSCoin = RewardSCoin;
			this.RewardHCoin = RewardHCoin;
			this.RewardStamina = RewardStamina;
			this.RewardSkillPoint = RewardSkillPoint;
			this.RewardFriendPoint = RewardFriendPoint;
			this.RewardItem1ID = RewardItem1ID;
			this.RewardItem1Level = RewardItem1Level;
			this.RewardItem1Num = RewardItem1Num;
			this.RewardItem2ID = RewardItem2ID;
			this.RewardItem2Level = RewardItem2Level;
			this.RewardItem2Num = RewardItem2Num;
			this.RewardItem3ID = RewardItem3ID;
			this.RewardItem3Level = RewardItem3Level;
			this.RewardItem3Num = RewardItem3Num;
			this.RewardItem4ID = RewardItem4ID;
			this.RewardItem4Level = RewardItem4Level;
			this.RewardItem4Num = RewardItem4Num;
			this.RewardItem5ID = RewardItem5ID;
			this.RewardItem5Level = RewardItem5Level;
			this.RewardItem5Num = RewardItem5Num;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(RewardID, ref lastHash);
			HashUtils.ContentHashOnto(RewardExp, ref lastHash);
			HashUtils.ContentHashOnto(RewardSCoin, ref lastHash);
			HashUtils.ContentHashOnto(RewardHCoin, ref lastHash);
			HashUtils.ContentHashOnto(RewardStamina, ref lastHash);
			HashUtils.ContentHashOnto(RewardSkillPoint, ref lastHash);
			HashUtils.ContentHashOnto(RewardFriendPoint, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem1ID, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem1Level, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem1Num, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem2ID, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem2Level, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem2Num, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem3ID, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem3Level, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem3Num, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem4ID, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem4Level, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem4Num, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem5ID, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem5Level, ref lastHash);
			HashUtils.ContentHashOnto(RewardItem5Num, ref lastHash);
		}
	}
}
