namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class TutorialData : IHashable
	{
		public readonly int id;

		public readonly string comment;

		public readonly int triggerMissionID;

		public readonly bool triggerOnDoing;

		public readonly bool triggerOnFinish;

		public readonly bool triggerOnClose;

		public readonly string triggerUIContextName;

		public readonly int triggerSpecial;

		public readonly int startStepID;

		public readonly int Reward;

		public readonly int FinishRsp;

		public readonly int SkipGroup;

		public readonly int SkipReward;

		public TutorialData(int id, string comment, int triggerMissionID, bool triggerOnDoing, bool triggerOnFinish, bool triggerOnClose, string triggerUIContextName, int triggerSpecial, int startStepID, int Reward, int FinishRsp, int SkipGroup, int SkipReward)
		{
			this.id = id;
			this.comment = comment;
			this.triggerMissionID = triggerMissionID;
			this.triggerOnDoing = triggerOnDoing;
			this.triggerOnFinish = triggerOnFinish;
			this.triggerOnClose = triggerOnClose;
			this.triggerUIContextName = triggerUIContextName;
			this.triggerSpecial = triggerSpecial;
			this.startStepID = startStepID;
			this.Reward = Reward;
			this.FinishRsp = FinishRsp;
			this.SkipGroup = SkipGroup;
			this.SkipReward = SkipReward;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(id, ref lastHash);
			HashUtils.ContentHashOnto(comment, ref lastHash);
			HashUtils.ContentHashOnto(triggerMissionID, ref lastHash);
			HashUtils.ContentHashOnto(triggerOnDoing, ref lastHash);
			HashUtils.ContentHashOnto(triggerOnFinish, ref lastHash);
			HashUtils.ContentHashOnto(triggerOnClose, ref lastHash);
			HashUtils.ContentHashOnto(triggerUIContextName, ref lastHash);
			HashUtils.ContentHashOnto(triggerSpecial, ref lastHash);
			HashUtils.ContentHashOnto(startStepID, ref lastHash);
			HashUtils.ContentHashOnto(Reward, ref lastHash);
			HashUtils.ContentHashOnto(FinishRsp, ref lastHash);
			HashUtils.ContentHashOnto(SkipGroup, ref lastHash);
			HashUtils.ContentHashOnto(SkipReward, ref lastHash);
		}
	}
}
