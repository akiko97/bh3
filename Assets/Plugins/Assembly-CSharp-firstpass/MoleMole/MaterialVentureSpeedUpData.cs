namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MaterialVentureSpeedUpData : IHashable
	{
		public readonly int MaterialID;

		public readonly int SpeedUpTime;

		public MaterialVentureSpeedUpData(int MaterialID, int SpeedUpTime)
		{
			this.MaterialID = MaterialID;
			this.SpeedUpTime = SpeedUpTime;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(MaterialID, ref lastHash);
			HashUtils.ContentHashOnto(SpeedUpTime, ref lastHash);
		}
	}
}
