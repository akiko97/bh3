using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ReviveCostTypeMetaData : IHashable
	{
		public readonly int reviveTimes;

		public readonly List<int> costList;

		public ReviveCostTypeMetaData(int reviveTimes, List<int> costList)
		{
			this.reviveTimes = reviveTimes;
			this.costList = costList;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(reviveTimes, ref lastHash);
			if (costList == null)
			{
				return;
			}
			foreach (int cost in costList)
			{
				HashUtils.ContentHashOnto(cost, ref lastHash);
			}
		}
	}
}
