namespace MoleMole
{
	public class CgDataItem
	{
		public int cgID;

		public int levelID;

		public string cgPath;

		public string cgIconPath;

		public CgDataItem(int cgID, int levelID, string cgPath, string iconPath = "")
		{
			this.cgID = cgID;
			this.levelID = levelID;
			this.cgPath = cgPath;
			cgIconPath = iconPath;
		}

		public CgDataItem(CgMetaData cgMetaData)
		{
			cgID = cgMetaData.CgID;
			levelID = cgMetaData.levelID;
			cgPath = cgMetaData.CgPath;
			cgIconPath = cgMetaData.CgIconSpritePath;
		}

		public override string ToString()
		{
			return string.Format("<CgDataItem>\ncgID: {0}\nlevelID: {1}\nCgPaht: {2}\nCgIconSpritePath : {3}", cgID, levelID, cgPath, cgIconPath);
		}
	}
}
