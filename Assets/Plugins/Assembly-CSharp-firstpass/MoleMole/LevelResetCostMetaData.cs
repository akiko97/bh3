using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class LevelResetCostMetaData : IHashable
	{
		public readonly int times;

		public readonly List<int> costList;

		public LevelResetCostMetaData(int times, List<int> costList)
		{
			this.times = times;
			this.costList = costList;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(times, ref lastHash);
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
