namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class SeriesMetaData : IHashable
	{
		public readonly int id;

		public readonly string title;

		public readonly int enableNuclear;

		public readonly float nucProb0;

		public readonly float nucProbDelta;

		public readonly float nucProbStart;

		public readonly float nucTime;

		public SeriesMetaData(int id, string title, int enableNuclear, float nucProb0, float nucProbDelta, float nucProbStart, float nucTime)
		{
			this.id = id;
			this.title = title;
			this.enableNuclear = enableNuclear;
			this.nucProb0 = nucProb0;
			this.nucProbDelta = nucProbDelta;
			this.nucProbStart = nucProbStart;
			this.nucTime = nucTime;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(id, ref lastHash);
			HashUtils.ContentHashOnto(title, ref lastHash);
			HashUtils.ContentHashOnto(enableNuclear, ref lastHash);
			HashUtils.ContentHashOnto(nucProb0, ref lastHash);
			HashUtils.ContentHashOnto(nucProbDelta, ref lastHash);
			HashUtils.ContentHashOnto(nucProbStart, ref lastHash);
			HashUtils.ContentHashOnto(nucTime, ref lastHash);
		}
	}
}
