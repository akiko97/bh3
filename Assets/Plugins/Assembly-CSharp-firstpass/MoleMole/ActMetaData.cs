namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ActMetaData : IHashable
	{
		public readonly int actId;

		public readonly int chapterId;

		public readonly int numInChapter;

		public readonly string actName;

		public readonly int actType;

		public readonly string levelPannelPath;

		public readonly string smallImgPath;

		public readonly string bgImgPath;

		public ActMetaData(int actId, int chapterId, int numInChapter, string actName, int actType, string levelPannelPath, string smallImgPath, string bgImgPath)
		{
			this.actId = actId;
			this.chapterId = chapterId;
			this.numInChapter = numInChapter;
			this.actName = actName;
			this.actType = actType;
			this.levelPannelPath = levelPannelPath;
			this.smallImgPath = smallImgPath;
			this.bgImgPath = bgImgPath;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(actId, ref lastHash);
			HashUtils.ContentHashOnto(chapterId, ref lastHash);
			HashUtils.ContentHashOnto(numInChapter, ref lastHash);
			HashUtils.ContentHashOnto(actName, ref lastHash);
			HashUtils.ContentHashOnto(actType, ref lastHash);
			HashUtils.ContentHashOnto(levelPannelPath, ref lastHash);
			HashUtils.ContentHashOnto(smallImgPath, ref lastHash);
			HashUtils.ContentHashOnto(bgImgPath, ref lastHash);
		}
	}
}
