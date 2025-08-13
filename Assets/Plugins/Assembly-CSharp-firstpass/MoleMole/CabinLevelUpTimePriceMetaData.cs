namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinLevelUpTimePriceMetaData : IHashable
	{
		public readonly int timeMin;

		public readonly int timeMax;

		public readonly int price;

		public CabinLevelUpTimePriceMetaData(int timeMin, int timeMax, int price)
		{
			this.timeMin = timeMin;
			this.timeMax = timeMax;
			this.price = price;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(timeMin, ref lastHash);
			HashUtils.ContentHashOnto(timeMax, ref lastHash);
			HashUtils.ContentHashOnto(price, ref lastHash);
		}
	}
}
