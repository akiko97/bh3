using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinExtendGradeMetaData : IHashable
	{
		public class CabinExtendNeedItem
		{
			public readonly int itemMetaID;

			public readonly int itemNum;

			public CabinExtendNeedItem(int itemMetaID, int itemNum)
			{
				this.itemMetaID = itemMetaID;
				this.itemNum = itemNum;
			}

			public CabinExtendNeedItem(string nodeString)
			{
				char[] seperator = new char[1] { ':' };
				List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
				itemMetaID = int.Parse(stringListFromString[0]);
				itemNum = int.Parse(stringListFromString[1]);
			}
		}

		public readonly int cabinType;

		public readonly int extendGrade;

		public readonly int cabinLevelMax;

		public readonly int scoinNeed;

		public readonly List<CabinExtendNeedItem> itemListNeed;

		public readonly string buildingPath;

		public CabinExtendGradeMetaData(int cabinType, int extendGrade, int cabinLevelMax, int scoinNeed, List<CabinExtendNeedItem> itemListNeed, string buildingPath)
		{
			this.cabinType = cabinType;
			this.extendGrade = extendGrade;
			this.cabinLevelMax = cabinLevelMax;
			this.scoinNeed = scoinNeed;
			this.itemListNeed = itemListNeed;
			this.buildingPath = buildingPath;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(cabinType, ref lastHash);
			HashUtils.ContentHashOnto(extendGrade, ref lastHash);
			HashUtils.ContentHashOnto(cabinLevelMax, ref lastHash);
			HashUtils.ContentHashOnto(scoinNeed, ref lastHash);
			if (itemListNeed != null)
			{
				foreach (CabinExtendNeedItem item in itemListNeed)
				{
					HashUtils.ContentHashOnto(item.itemMetaID, ref lastHash);
					HashUtils.ContentHashOnto(item.itemNum, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(buildingPath, ref lastHash);
		}
	}
}
