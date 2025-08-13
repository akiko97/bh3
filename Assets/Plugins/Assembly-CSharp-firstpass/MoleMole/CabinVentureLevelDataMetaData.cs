using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinVentureLevelDataMetaData : IHashable
	{
		public class UnlockVentureItem
		{
			public readonly int level;

			public readonly int difficulty;

			public UnlockVentureItem(int level, int difficulty)
			{
				this.level = level;
				this.difficulty = difficulty;
			}

			public UnlockVentureItem(string nodeString)
			{
				char[] seperator = new char[1] { ':' };
				List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
				level = int.Parse(stringListFromString[0]);
				difficulty = int.Parse(stringListFromString[1]);
			}
		}

		public readonly int level;

		public readonly int maxVentureNumBase;

		public readonly int maxVentureInProgressNumBase;

		public readonly List<UnlockVentureItem> UnlockVentureList;

		public readonly int refreshType;

		public CabinVentureLevelDataMetaData(int level, int maxVentureNumBase, int maxVentureInProgressNumBase, List<UnlockVentureItem> UnlockVentureList, int refreshType)
		{
			this.level = level;
			this.maxVentureNumBase = maxVentureNumBase;
			this.maxVentureInProgressNumBase = maxVentureInProgressNumBase;
			this.UnlockVentureList = UnlockVentureList;
			this.refreshType = refreshType;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(level, ref lastHash);
			HashUtils.ContentHashOnto(maxVentureNumBase, ref lastHash);
			HashUtils.ContentHashOnto(maxVentureInProgressNumBase, ref lastHash);
			if (UnlockVentureList != null)
			{
				foreach (UnlockVentureItem unlockVenture in UnlockVentureList)
				{
					HashUtils.ContentHashOnto(unlockVenture.level, ref lastHash);
					HashUtils.ContentHashOnto(unlockVenture.difficulty, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(refreshType, ref lastHash);
		}
	}
}
