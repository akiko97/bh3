using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSubSkillLevelMetaData : IHashable
	{
		public class SkillUpLevelNeedItem
		{
			public readonly int itemMetaID;

			public readonly int itemNum;

			public SkillUpLevelNeedItem(int itemMetaID, int itemNum)
			{
				this.itemMetaID = itemMetaID;
				this.itemNum = itemNum;
			}

			public SkillUpLevelNeedItem(string nodeString)
			{
				char[] seperator = new char[1] { ':' };
				List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
				itemMetaID = int.Parse(stringListFromString[0]);
				itemNum = int.Parse(stringListFromString[1]);
			}
		}

		public readonly int unlockLv;

		public readonly List<int> needScoinList;

		public readonly List<List<SkillUpLevelNeedItem>> needItemList;

		public AvatarSubSkillLevelMetaData(int unlockLv, List<int> needScoinList, List<List<SkillUpLevelNeedItem>> needItemList)
		{
			this.unlockLv = unlockLv;
			this.needScoinList = needScoinList;
			this.needItemList = needItemList;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(unlockLv, ref lastHash);
			if (needScoinList != null)
			{
				foreach (int needScoin in needScoinList)
				{
					HashUtils.ContentHashOnto(needScoin, ref lastHash);
				}
			}
			if (needItemList == null)
			{
				return;
			}
			foreach (List<SkillUpLevelNeedItem> needItem in needItemList)
			{
				if (needItem == null)
				{
					continue;
				}
				foreach (SkillUpLevelNeedItem item in needItem)
				{
					HashUtils.ContentHashOnto(item.itemMetaID, ref lastHash);
					HashUtils.ContentHashOnto(item.itemNum, ref lastHash);
				}
			}
		}
	}
}
