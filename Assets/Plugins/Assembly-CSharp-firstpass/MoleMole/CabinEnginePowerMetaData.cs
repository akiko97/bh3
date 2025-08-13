namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinEnginePowerMetaData : IHashable
	{
		public readonly int level;

		public readonly int powerCost;

		public CabinEnginePowerMetaData(int level, int powerCost)
		{
			this.level = level;
			this.powerCost = powerCost;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(level, ref lastHash);
			HashUtils.ContentHashOnto(powerCost, ref lastHash);
		}
	}
}
