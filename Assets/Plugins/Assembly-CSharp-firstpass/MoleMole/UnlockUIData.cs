namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class UnlockUIData : IHashable
	{
		public readonly int id;

		public readonly string comment;

		public readonly string unlockTarget;

		public readonly int unlockByMission;

		public readonly int OnDoing;

		public readonly int OnFinish;

		public readonly int OnClose;

		public readonly int unlockByTutorial;

		public UnlockUIData(int id, string comment, string unlockTarget, int unlockByMission, int OnDoing, int OnFinish, int OnClose, int unlockByTutorial)
		{
			this.id = id;
			this.comment = comment;
			this.unlockTarget = unlockTarget;
			this.unlockByMission = unlockByMission;
			this.OnDoing = OnDoing;
			this.OnFinish = OnFinish;
			this.OnClose = OnClose;
			this.unlockByTutorial = unlockByTutorial;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(id, ref lastHash);
			HashUtils.ContentHashOnto(comment, ref lastHash);
			HashUtils.ContentHashOnto(unlockTarget, ref lastHash);
			HashUtils.ContentHashOnto(unlockByMission, ref lastHash);
			HashUtils.ContentHashOnto(OnDoing, ref lastHash);
			HashUtils.ContentHashOnto(OnFinish, ref lastHash);
			HashUtils.ContentHashOnto(OnClose, ref lastHash);
			HashUtils.ContentHashOnto(unlockByTutorial, ref lastHash);
		}
	}
}
