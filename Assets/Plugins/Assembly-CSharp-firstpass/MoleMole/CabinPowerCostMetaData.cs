namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinPowerCostMetaData : IHashable
	{
		public readonly int Level;

		public readonly int MaxPowerCost;

		public CabinPowerCostMetaData(int Level, int MaxPowerCost)
		{
			this.Level = Level;
			this.MaxPowerCost = MaxPowerCost;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(Level, ref lastHash);
			HashUtils.ContentHashOnto(MaxPowerCost, ref lastHash);
		}
	}
}
