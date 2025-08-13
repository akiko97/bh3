namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class LinearMissionData : IHashable
	{
		public readonly int MissionID;

		public readonly string comment;

		public readonly int PreMissionId;

		public readonly int MinLevel;

		public readonly int IsAchievement;

		public LinearMissionData(int MissionID, string comment, int PreMissionId, int MinLevel, int IsAchievement)
		{
			this.MissionID = MissionID;
			this.comment = comment;
			this.PreMissionId = PreMissionId;
			this.MinLevel = MinLevel;
			this.IsAchievement = IsAchievement;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(MissionID, ref lastHash);
			HashUtils.ContentHashOnto(comment, ref lastHash);
			HashUtils.ContentHashOnto(PreMissionId, ref lastHash);
			HashUtils.ContentHashOnto(MinLevel, ref lastHash);
			HashUtils.ContentHashOnto(IsAchievement, ref lastHash);
		}
	}
}
