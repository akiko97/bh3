using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinLevelMetaData : IHashable
	{
		public class CabinUpLevelNeedItem
		{
			public readonly int itemMetaID;

			public readonly int itemNum;

			public CabinUpLevelNeedItem(int itemMetaID, int itemNum)
			{
				this.itemMetaID = itemMetaID;
				this.itemNum = itemNum;
			}

			public CabinUpLevelNeedItem(string nodeString)
			{
				char[] seperator = new char[1] { ':' };
				List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
				itemMetaID = int.Parse(stringListFromString[0]);
				itemNum = int.Parse(stringListFromString[1]);
			}
		}

		public readonly int cabinType;

		public readonly string cabinName;

		public readonly int level;

		public readonly int playerLevelNeed;

		public readonly int scoinNeed;

		public readonly List<CabinUpLevelNeedItem> itemListNeed;

		public readonly int upLevelTimeNeed;

		public CabinLevelMetaData(int cabinType, string cabinName, int level, int playerLevelNeed, int scoinNeed, List<CabinUpLevelNeedItem> itemListNeed, int upLevelTimeNeed)
		{
			this.cabinType = cabinType;
			this.cabinName = cabinName;
			this.level = level;
			this.playerLevelNeed = playerLevelNeed;
			this.scoinNeed = scoinNeed;
			this.itemListNeed = itemListNeed;
			this.upLevelTimeNeed = upLevelTimeNeed;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(cabinType, ref lastHash);
			HashUtils.ContentHashOnto(cabinName, ref lastHash);
			HashUtils.ContentHashOnto(level, ref lastHash);
			HashUtils.ContentHashOnto(playerLevelNeed, ref lastHash);
			HashUtils.ContentHashOnto(scoinNeed, ref lastHash);
			if (itemListNeed != null)
			{
				foreach (CabinUpLevelNeedItem item in itemListNeed)
				{
					HashUtils.ContentHashOnto(item.itemMetaID, ref lastHash);
					HashUtils.ContentHashOnto(item.itemNum, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(upLevelTimeNeed, ref lastHash);
		}
	}
}
