namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarFragmentMetaData : IHashable
	{
		public readonly int ID;

		public readonly string name;

		public readonly int rarity;

		public readonly int maxRarity;

		public readonly int cost;

		public readonly int maxLv;

		public readonly int expType;

		public readonly float sellPriceBase;

		public readonly float sellPriceAdd;

		public readonly float gearExpProvideBase;

		public readonly float gearExpPorvideAdd;

		public readonly string ItemType;

		public readonly int BaseType;

		public readonly string displayTitle;

		public readonly string displayDescription;

		public readonly int indexID;

		public readonly string iconPath;

		public readonly string imagePath;

		public readonly float characterExpProvide;

		public readonly int stimimaProvide;

		public AvatarFragmentMetaData(int ID, string name, int rarity, int maxRarity, int cost, int maxLv, int expType, float sellPriceBase, float sellPriceAdd, float gearExpProvideBase, float gearExpPorvideAdd, string ItemType, int BaseType, string displayTitle, string displayDescription, int indexID, string iconPath, string imagePath, float characterExpProvide, int stimimaProvide)
		{
			this.ID = ID;
			this.name = name;
			this.rarity = rarity;
			this.maxRarity = maxRarity;
			this.cost = cost;
			this.maxLv = maxLv;
			this.expType = expType;
			this.sellPriceBase = sellPriceBase;
			this.sellPriceAdd = sellPriceAdd;
			this.gearExpProvideBase = gearExpProvideBase;
			this.gearExpPorvideAdd = gearExpPorvideAdd;
			this.ItemType = ItemType;
			this.BaseType = BaseType;
			this.displayTitle = displayTitle;
			this.displayDescription = displayDescription;
			this.indexID = indexID;
			this.iconPath = iconPath;
			this.imagePath = imagePath;
			this.characterExpProvide = characterExpProvide;
			this.stimimaProvide = stimimaProvide;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(name, ref lastHash);
			HashUtils.ContentHashOnto(rarity, ref lastHash);
			HashUtils.ContentHashOnto(maxRarity, ref lastHash);
			HashUtils.ContentHashOnto(cost, ref lastHash);
			HashUtils.ContentHashOnto(maxLv, ref lastHash);
			HashUtils.ContentHashOnto(expType, ref lastHash);
			HashUtils.ContentHashOnto(sellPriceBase, ref lastHash);
			HashUtils.ContentHashOnto(sellPriceAdd, ref lastHash);
			HashUtils.ContentHashOnto(gearExpProvideBase, ref lastHash);
			HashUtils.ContentHashOnto(gearExpPorvideAdd, ref lastHash);
			HashUtils.ContentHashOnto(ItemType, ref lastHash);
			HashUtils.ContentHashOnto(BaseType, ref lastHash);
			HashUtils.ContentHashOnto(displayTitle, ref lastHash);
			HashUtils.ContentHashOnto(displayDescription, ref lastHash);
			HashUtils.ContentHashOnto(indexID, ref lastHash);
			HashUtils.ContentHashOnto(iconPath, ref lastHash);
			HashUtils.ContentHashOnto(imagePath, ref lastHash);
			HashUtils.ContentHashOnto(characterExpProvide, ref lastHash);
			HashUtils.ContentHashOnto(stimimaProvide, ref lastHash);
		}
	}
}
