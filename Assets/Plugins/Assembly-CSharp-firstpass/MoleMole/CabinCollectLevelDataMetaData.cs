namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinCollectLevelDataMetaData : IHashable
	{
		public readonly int level;

		public readonly float scoinGrowthBase;

		public readonly float minIntervalTime;

		public readonly float scoinStorageBase;

		public readonly int extraScoinRatioBase;

		public readonly float extraScoinRatioAddBase;

		public CabinCollectLevelDataMetaData(int level, float scoinGrowthBase, float minIntervalTime, float scoinStorageBase, int extraScoinRatioBase, float extraScoinRatioAddBase)
		{
			this.level = level;
			this.scoinGrowthBase = scoinGrowthBase;
			this.minIntervalTime = minIntervalTime;
			this.scoinStorageBase = scoinStorageBase;
			this.extraScoinRatioBase = extraScoinRatioBase;
			this.extraScoinRatioAddBase = extraScoinRatioAddBase;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(level, ref lastHash);
			HashUtils.ContentHashOnto(scoinGrowthBase, ref lastHash);
			HashUtils.ContentHashOnto(minIntervalTime, ref lastHash);
			HashUtils.ContentHashOnto(scoinStorageBase, ref lastHash);
			HashUtils.ContentHashOnto(extraScoinRatioBase, ref lastHash);
			HashUtils.ContentHashOnto(extraScoinRatioAddBase, ref lastHash);
		}
	}
}
