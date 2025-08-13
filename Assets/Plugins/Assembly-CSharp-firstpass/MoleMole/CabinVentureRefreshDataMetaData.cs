using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinVentureRefreshDataMetaData : IHashable
	{
		public readonly int level;

		public readonly List<int> needHcoinList;

		public CabinVentureRefreshDataMetaData(int level, List<int> needHcoinList)
		{
			this.level = level;
			this.needHcoinList = needHcoinList;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(level, ref lastHash);
			if (needHcoinList == null)
			{
				return;
			}
			foreach (int needHcoin in needHcoinList)
			{
				HashUtils.ContentHashOnto(needHcoin, ref lastHash);
			}
		}
	}
}
