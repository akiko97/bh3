namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarStarMetaData : IHashable
	{
		public readonly int avatarID;

		public readonly int star;

		public readonly int upgradeFragment;

		public readonly int recountFragment;

		public readonly string iconPath;

		public readonly string iconPathInLevel;

		public readonly string figurePath;

		public readonly float hpBase;

		public readonly float hpAdd;

		public readonly float spBase;

		public readonly float spAdd;

		public readonly float atkBase;

		public readonly float atkAdd;

		public readonly float dfsBase;

		public readonly float dfsAdd;

		public readonly float crtBase;

		public readonly float crtAdd;

		public AvatarStarMetaData(int avatarID, int star, int upgradeFragment, int recountFragment, string iconPath, string iconPathInLevel, string figurePath, float hpBase, float hpAdd, float spBase, float spAdd, float atkBase, float atkAdd, float dfsBase, float dfsAdd, float crtBase, float crtAdd)
		{
			this.avatarID = avatarID;
			this.star = star;
			this.upgradeFragment = upgradeFragment;
			this.recountFragment = recountFragment;
			this.iconPath = iconPath;
			this.iconPathInLevel = iconPathInLevel;
			this.figurePath = figurePath;
			this.hpBase = hpBase;
			this.hpAdd = hpAdd;
			this.spBase = spBase;
			this.spAdd = spAdd;
			this.atkBase = atkBase;
			this.atkAdd = atkAdd;
			this.dfsBase = dfsBase;
			this.dfsAdd = dfsAdd;
			this.crtBase = crtBase;
			this.crtAdd = crtAdd;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(avatarID, ref lastHash);
			HashUtils.ContentHashOnto(star, ref lastHash);
			HashUtils.ContentHashOnto(upgradeFragment, ref lastHash);
			HashUtils.ContentHashOnto(recountFragment, ref lastHash);
			HashUtils.ContentHashOnto(iconPath, ref lastHash);
			HashUtils.ContentHashOnto(iconPathInLevel, ref lastHash);
			HashUtils.ContentHashOnto(figurePath, ref lastHash);
			HashUtils.ContentHashOnto(hpBase, ref lastHash);
			HashUtils.ContentHashOnto(hpAdd, ref lastHash);
			HashUtils.ContentHashOnto(spBase, ref lastHash);
			HashUtils.ContentHashOnto(spAdd, ref lastHash);
			HashUtils.ContentHashOnto(atkBase, ref lastHash);
			HashUtils.ContentHashOnto(atkAdd, ref lastHash);
			HashUtils.ContentHashOnto(dfsBase, ref lastHash);
			HashUtils.ContentHashOnto(dfsAdd, ref lastHash);
			HashUtils.ContentHashOnto(crtBase, ref lastHash);
			HashUtils.ContentHashOnto(crtAdd, ref lastHash);
		}
	}
}
