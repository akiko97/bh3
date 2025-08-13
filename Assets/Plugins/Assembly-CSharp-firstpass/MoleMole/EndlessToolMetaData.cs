namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class EndlessToolMetaData : IHashable
	{
		public readonly int ID;

		public readonly string comment;

		public readonly string name;

		public readonly int rarity;

		public readonly int type;

		public readonly int paramIntMin;

		public readonly int paramIntMax;

		public readonly int paramTime;

		public readonly string paramStr;

		public readonly int showIcon;

		public readonly string iconPath;

		public readonly string smallIconPath;

		public readonly string effectPath;

		public readonly string description;

		public readonly string report;

		public EndlessToolMetaData(int ID, string comment, string name, int rarity, int type, int paramIntMin, int paramIntMax, int paramTime, string paramStr, int showIcon, string iconPath, string smallIconPath, string effectPath, string description, string report)
		{
			this.ID = ID;
			this.comment = comment;
			this.name = name;
			this.rarity = rarity;
			this.type = type;
			this.paramIntMin = paramIntMin;
			this.paramIntMax = paramIntMax;
			this.paramTime = paramTime;
			this.paramStr = paramStr;
			this.showIcon = showIcon;
			this.iconPath = iconPath;
			this.smallIconPath = smallIconPath;
			this.effectPath = effectPath;
			this.description = description;
			this.report = report;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(comment, ref lastHash);
			HashUtils.ContentHashOnto(name, ref lastHash);
			HashUtils.ContentHashOnto(rarity, ref lastHash);
			HashUtils.ContentHashOnto(type, ref lastHash);
			HashUtils.ContentHashOnto(paramIntMin, ref lastHash);
			HashUtils.ContentHashOnto(paramIntMax, ref lastHash);
			HashUtils.ContentHashOnto(paramTime, ref lastHash);
			HashUtils.ContentHashOnto(paramStr, ref lastHash);
			HashUtils.ContentHashOnto(showIcon, ref lastHash);
			HashUtils.ContentHashOnto(iconPath, ref lastHash);
			HashUtils.ContentHashOnto(smallIconPath, ref lastHash);
			HashUtils.ContentHashOnto(effectPath, ref lastHash);
			HashUtils.ContentHashOnto(description, ref lastHash);
			HashUtils.ContentHashOnto(report, ref lastHash);
		}
	}
}
