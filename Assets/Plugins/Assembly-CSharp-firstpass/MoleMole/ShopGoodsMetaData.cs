namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ShopGoodsMetaData : IHashable
	{
		public readonly int ID;

		public readonly int ItemID;

		public readonly int ItemLevel;

		public readonly int ItemNum;

		public readonly int HCoinCost;

		public readonly int SCoinCost;

		public readonly int CostItemId;

		public readonly int CostItemNum;

		public readonly int CostItemId2;

		public readonly int CostItemNum2;

		public readonly int CostItemId3;

		public readonly int CostItemNum3;

		public readonly int CostItemId4;

		public readonly int CostItemNum4;

		public readonly int CostItemId5;

		public readonly int CostItemNum5;

		public readonly int MaxBuyTimes;

		public readonly int PriceRateID;

		public readonly int Discount;

		public readonly bool IsSuperWorth;

		public ShopGoodsMetaData(int ID, int ItemID, int ItemLevel, int ItemNum, int HCoinCost, int SCoinCost, int CostItemId, int CostItemNum, int CostItemId2, int CostItemNum2, int CostItemId3, int CostItemNum3, int CostItemId4, int CostItemNum4, int CostItemId5, int CostItemNum5, int MaxBuyTimes, int PriceRateID, int Discount, bool IsSuperWorth)
		{
			this.ID = ID;
			this.ItemID = ItemID;
			this.ItemLevel = ItemLevel;
			this.ItemNum = ItemNum;
			this.HCoinCost = HCoinCost;
			this.SCoinCost = SCoinCost;
			this.CostItemId = CostItemId;
			this.CostItemNum = CostItemNum;
			this.CostItemId2 = CostItemId2;
			this.CostItemNum2 = CostItemNum2;
			this.CostItemId3 = CostItemId3;
			this.CostItemNum3 = CostItemNum3;
			this.CostItemId4 = CostItemId4;
			this.CostItemNum4 = CostItemNum4;
			this.CostItemId5 = CostItemId5;
			this.CostItemNum5 = CostItemNum5;
			this.MaxBuyTimes = MaxBuyTimes;
			this.PriceRateID = PriceRateID;
			this.Discount = Discount;
			this.IsSuperWorth = IsSuperWorth;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(ItemID, ref lastHash);
			HashUtils.ContentHashOnto(ItemLevel, ref lastHash);
			HashUtils.ContentHashOnto(ItemNum, ref lastHash);
			HashUtils.ContentHashOnto(HCoinCost, ref lastHash);
			HashUtils.ContentHashOnto(SCoinCost, ref lastHash);
			HashUtils.ContentHashOnto(CostItemId, ref lastHash);
			HashUtils.ContentHashOnto(CostItemNum, ref lastHash);
			HashUtils.ContentHashOnto(CostItemId2, ref lastHash);
			HashUtils.ContentHashOnto(CostItemNum2, ref lastHash);
			HashUtils.ContentHashOnto(CostItemId3, ref lastHash);
			HashUtils.ContentHashOnto(CostItemNum3, ref lastHash);
			HashUtils.ContentHashOnto(CostItemId4, ref lastHash);
			HashUtils.ContentHashOnto(CostItemNum4, ref lastHash);
			HashUtils.ContentHashOnto(CostItemId5, ref lastHash);
			HashUtils.ContentHashOnto(CostItemNum5, ref lastHash);
			HashUtils.ContentHashOnto(MaxBuyTimes, ref lastHash);
			HashUtils.ContentHashOnto(PriceRateID, ref lastHash);
			HashUtils.ContentHashOnto(Discount, ref lastHash);
			HashUtils.ContentHashOnto(IsSuperWorth, ref lastHash);
		}
	}
}
