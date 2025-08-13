using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSubSkillMetaData : IHashable
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

		public class UpLevelStarNeed
		{
			public readonly int level;

			public readonly int starNeed;

			public UpLevelStarNeed(int level, int starNeed)
			{
				this.level = level;
				this.starNeed = starNeed;
			}

			public UpLevelStarNeed(string nodeString)
			{
				char[] seperator = new char[1] { ':' };
				List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
				level = int.Parse(stringListFromString[0]);
				starNeed = int.Parse(stringListFromString[1]);
			}
		}

		public readonly int avatarSubSkillId;

		public readonly string name;

		public readonly string info;

		public readonly string desc;

		public readonly int skillId;

		public readonly string iconPath;

		public readonly int canTry;

		public readonly int isEx;

		public readonly int unlockLv;

		public readonly int unlockStar;

		public readonly int unlockPoint;

		public readonly int unlockScoin;

		public readonly List<SkillUpLevelNeedItem> unlockItemList;

		public readonly int maxLv;

		public readonly int preLvPoint;

		public readonly int unlockLvAdd;

		public readonly float paramBase_1;

		public readonly float paramAdd_1;

		public readonly float paramBase_2;

		public readonly float paramAdd_2;

		public readonly float paramBase_3;

		public readonly float paramAdd_3;

		public readonly int lvUpScoinType;

		public readonly int lvUpItemType;

		public readonly int showOrder;

		public readonly List<UpLevelStarNeed> upLevelStarNeedList;

		public AvatarSubSkillMetaData(int avatarSubSkillId, string name, string info, string desc, int skillId, string iconPath, int canTry, int isEx, int unlockLv, int unlockStar, int unlockPoint, int unlockScoin, List<SkillUpLevelNeedItem> unlockItemList, int maxLv, int preLvPoint, int unlockLvAdd, float paramBase_1, float paramAdd_1, float paramBase_2, float paramAdd_2, float paramBase_3, float paramAdd_3, int lvUpScoinType, int lvUpItemType, int showOrder, List<UpLevelStarNeed> upLevelStarNeedList)
		{
			this.avatarSubSkillId = avatarSubSkillId;
			this.name = name;
			this.info = info;
			this.desc = desc;
			this.skillId = skillId;
			this.iconPath = iconPath;
			this.canTry = canTry;
			this.isEx = isEx;
			this.unlockLv = unlockLv;
			this.unlockStar = unlockStar;
			this.unlockPoint = unlockPoint;
			this.unlockScoin = unlockScoin;
			this.unlockItemList = unlockItemList;
			this.maxLv = maxLv;
			this.preLvPoint = preLvPoint;
			this.unlockLvAdd = unlockLvAdd;
			this.paramBase_1 = paramBase_1;
			this.paramAdd_1 = paramAdd_1;
			this.paramBase_2 = paramBase_2;
			this.paramAdd_2 = paramAdd_2;
			this.paramBase_3 = paramBase_3;
			this.paramAdd_3 = paramAdd_3;
			this.lvUpScoinType = lvUpScoinType;
			this.lvUpItemType = lvUpItemType;
			this.showOrder = showOrder;
			this.upLevelStarNeedList = upLevelStarNeedList;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(avatarSubSkillId, ref lastHash);
			HashUtils.ContentHashOnto(name, ref lastHash);
			HashUtils.ContentHashOnto(info, ref lastHash);
			HashUtils.ContentHashOnto(desc, ref lastHash);
			HashUtils.ContentHashOnto(skillId, ref lastHash);
			HashUtils.ContentHashOnto(iconPath, ref lastHash);
			HashUtils.ContentHashOnto(canTry, ref lastHash);
			HashUtils.ContentHashOnto(isEx, ref lastHash);
			HashUtils.ContentHashOnto(unlockLv, ref lastHash);
			HashUtils.ContentHashOnto(unlockStar, ref lastHash);
			HashUtils.ContentHashOnto(unlockPoint, ref lastHash);
			HashUtils.ContentHashOnto(unlockScoin, ref lastHash);
			if (unlockItemList != null)
			{
				foreach (SkillUpLevelNeedItem unlockItem in unlockItemList)
				{
					HashUtils.ContentHashOnto(unlockItem.itemMetaID, ref lastHash);
					HashUtils.ContentHashOnto(unlockItem.itemNum, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(maxLv, ref lastHash);
			HashUtils.ContentHashOnto(preLvPoint, ref lastHash);
			HashUtils.ContentHashOnto(unlockLvAdd, ref lastHash);
			HashUtils.ContentHashOnto(paramBase_1, ref lastHash);
			HashUtils.ContentHashOnto(paramAdd_1, ref lastHash);
			HashUtils.ContentHashOnto(paramBase_2, ref lastHash);
			HashUtils.ContentHashOnto(paramAdd_2, ref lastHash);
			HashUtils.ContentHashOnto(paramBase_3, ref lastHash);
			HashUtils.ContentHashOnto(paramAdd_3, ref lastHash);
			HashUtils.ContentHashOnto(lvUpScoinType, ref lastHash);
			HashUtils.ContentHashOnto(lvUpItemType, ref lastHash);
			HashUtils.ContentHashOnto(showOrder, ref lastHash);
			if (upLevelStarNeedList == null)
			{
				return;
			}
			foreach (UpLevelStarNeed upLevelStarNeed in upLevelStarNeedList)
			{
				HashUtils.ContentHashOnto(upLevelStarNeed.level, ref lastHash);
				HashUtils.ContentHashOnto(upLevelStarNeed.starNeed, ref lastHash);
			}
		}
	}
}
