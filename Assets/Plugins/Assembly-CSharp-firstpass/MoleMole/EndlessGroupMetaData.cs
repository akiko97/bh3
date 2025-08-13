namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class EndlessGroupMetaData : IHashable
	{
		public readonly int groupLevel;

		public readonly string comment;

		public readonly string groupName;

		public readonly int isGlobal;

		public readonly int minCapacity;

		public readonly int maxCapacity;

		public readonly int unlockCapacity;

		public readonly int promoteRank;

		public readonly int demoteRank;

		public readonly int prototeRewardID;

		public readonly int normalRewardID;

		public readonly int demoteRewardID;

		public readonly float baseHardLevel;

		public readonly float deltaHardLevel;

		public EndlessGroupMetaData(int groupLevel, string comment, string groupName, int isGlobal, int minCapacity, int maxCapacity, int unlockCapacity, int promoteRank, int demoteRank, int prototeRewardID, int normalRewardID, int demoteRewardID, float baseHardLevel, float deltaHardLevel)
		{
			this.groupLevel = groupLevel;
			this.comment = comment;
			this.groupName = groupName;
			this.isGlobal = isGlobal;
			this.minCapacity = minCapacity;
			this.maxCapacity = maxCapacity;
			this.unlockCapacity = unlockCapacity;
			this.promoteRank = promoteRank;
			this.demoteRank = demoteRank;
			this.prototeRewardID = prototeRewardID;
			this.normalRewardID = normalRewardID;
			this.demoteRewardID = demoteRewardID;
			this.baseHardLevel = baseHardLevel;
			this.deltaHardLevel = deltaHardLevel;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(groupLevel, ref lastHash);
			HashUtils.ContentHashOnto(comment, ref lastHash);
			HashUtils.ContentHashOnto(groupName, ref lastHash);
			HashUtils.ContentHashOnto(isGlobal, ref lastHash);
			HashUtils.ContentHashOnto(minCapacity, ref lastHash);
			HashUtils.ContentHashOnto(maxCapacity, ref lastHash);
			HashUtils.ContentHashOnto(unlockCapacity, ref lastHash);
			HashUtils.ContentHashOnto(promoteRank, ref lastHash);
			HashUtils.ContentHashOnto(demoteRank, ref lastHash);
			HashUtils.ContentHashOnto(prototeRewardID, ref lastHash);
			HashUtils.ContentHashOnto(normalRewardID, ref lastHash);
			HashUtils.ContentHashOnto(demoteRewardID, ref lastHash);
			HashUtils.ContentHashOnto(baseHardLevel, ref lastHash);
			HashUtils.ContentHashOnto(deltaHardLevel, ref lastHash);
		}
	}
}
