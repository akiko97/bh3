namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ShopGoodsPriceRateMetaData : IHashable
	{
		public readonly int BuyTimes;

		public readonly int PriceType1;

		public readonly int PriceType2;

		public readonly int PriceType3;

		public readonly int PriceType4;

		public readonly int PriceType5;

		public readonly int PriceType6;

		public readonly int PriceType7;

		public readonly int PriceType8;

		public readonly int PriceType9;

		public readonly int PriceType10;

		public ShopGoodsPriceRateMetaData(int BuyTimes, int PriceType1, int PriceType2, int PriceType3, int PriceType4, int PriceType5, int PriceType6, int PriceType7, int PriceType8, int PriceType9, int PriceType10)
		{
			this.BuyTimes = BuyTimes;
			this.PriceType1 = PriceType1;
			this.PriceType2 = PriceType2;
			this.PriceType3 = PriceType3;
			this.PriceType4 = PriceType4;
			this.PriceType5 = PriceType5;
			this.PriceType6 = PriceType6;
			this.PriceType7 = PriceType7;
			this.PriceType8 = PriceType8;
			this.PriceType9 = PriceType9;
			this.PriceType10 = PriceType10;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(BuyTimes, ref lastHash);
			HashUtils.ContentHashOnto(PriceType1, ref lastHash);
			HashUtils.ContentHashOnto(PriceType2, ref lastHash);
			HashUtils.ContentHashOnto(PriceType3, ref lastHash);
			HashUtils.ContentHashOnto(PriceType4, ref lastHash);
			HashUtils.ContentHashOnto(PriceType5, ref lastHash);
			HashUtils.ContentHashOnto(PriceType6, ref lastHash);
			HashUtils.ContentHashOnto(PriceType7, ref lastHash);
			HashUtils.ContentHashOnto(PriceType8, ref lastHash);
			HashUtils.ContentHashOnto(PriceType9, ref lastHash);
			HashUtils.ContentHashOnto(PriceType10, ref lastHash);
		}
	}
}
