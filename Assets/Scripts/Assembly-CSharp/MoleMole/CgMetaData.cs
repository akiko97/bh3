namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CgMetaData : IHashable
	{
		public readonly int CgID;

		public readonly int levelID;

		public readonly string CgPath;

		public readonly string CgIconSpritePath;

		public CgMetaData(int CgID, int levelID, string CgPath, string CgIconSpritePath)
		{
			this.CgID = CgID;
			this.levelID = levelID;
			this.CgPath = CgPath;
			this.CgIconSpritePath = CgIconSpritePath;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(CgID, ref lastHash);
			HashUtils.ContentHashOnto(levelID, ref lastHash);
			HashUtils.ContentHashOnto(CgPath, ref lastHash);
			HashUtils.ContentHashOnto(CgIconSpritePath, ref lastHash);
		}
	}
}
