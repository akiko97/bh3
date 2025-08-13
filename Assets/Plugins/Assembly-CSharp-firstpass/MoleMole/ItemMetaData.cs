namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ItemMetaData : IHashable
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

		public readonly int dropStageID1;

		public readonly int dropStageID2;

		public readonly int dropStageID3;

		public readonly int dropStageID4;

		public readonly int dropStageID5;

		public readonly int dropStageID6;

		public readonly string displayBGDescription;

		public ItemMetaData(int ID, string name, int rarity, int maxRarity, int cost, int maxLv, int expType, float sellPriceBase, float sellPriceAdd, float gearExpProvideBase, float gearExpPorvideAdd, string ItemType, int BaseType, string displayTitle, string displayDescription, int indexID, string iconPath, string imagePath, float characterExpProvide, int stimimaProvide, int dropStageID1, int dropStageID2, int dropStageID3, int dropStageID4, int dropStageID5, int dropStageID6, string displayBGDescription)
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
			this.dropStageID1 = dropStageID1;
			this.dropStageID2 = dropStageID2;
			this.dropStageID3 = dropStageID3;
			this.dropStageID4 = dropStageID4;
			this.dropStageID5 = dropStageID5;
			this.dropStageID6 = dropStageID6;
			this.displayBGDescription = displayBGDescription;
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
			HashUtils.ContentHashOnto(dropStageID1, ref lastHash);
			HashUtils.ContentHashOnto(dropStageID2, ref lastHash);
			HashUtils.ContentHashOnto(dropStageID3, ref lastHash);
			HashUtils.ContentHashOnto(dropStageID4, ref lastHash);
			HashUtils.ContentHashOnto(dropStageID5, ref lastHash);
			HashUtils.ContentHashOnto(dropStageID6, ref lastHash);
			HashUtils.ContentHashOnto(displayBGDescription, ref lastHash);
		}
	}
}
