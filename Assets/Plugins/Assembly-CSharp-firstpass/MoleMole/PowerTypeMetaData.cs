namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class PowerTypeMetaData : IHashable
	{
		public readonly int type;

		public readonly float powerConf;

		public PowerTypeMetaData(int type, float powerConf)
		{
			this.type = type;
			this.powerConf = powerConf;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(type, ref lastHash);
			HashUtils.ContentHashOnto(powerConf, ref lastHash);
		}
	}
}
