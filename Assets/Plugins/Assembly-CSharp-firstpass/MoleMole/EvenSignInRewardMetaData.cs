namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class EvenSignInRewardMetaData : IHashable
	{
		public readonly int day;

		public readonly int rewardItemID;

		public EvenSignInRewardMetaData(int day, int rewardItemID)
		{
			this.day = day;
			this.rewardItemID = rewardItemID;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(day, ref lastHash);
			HashUtils.ContentHashOnto(rewardItemID, ref lastHash);
		}
	}
}
