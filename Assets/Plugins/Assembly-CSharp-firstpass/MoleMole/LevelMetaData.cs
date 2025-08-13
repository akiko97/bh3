using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class LevelMetaData : IHashable
	{
		public class LevelChallengeMetaNode
		{
			public readonly int challengeId;

			public readonly int rewardId;

			public LevelChallengeMetaNode(int challengeId, int rewardId)
			{
				this.challengeId = challengeId;
				this.rewardId = rewardId;
			}

			public LevelChallengeMetaNode(string nodeString)
			{
				char[] seperator = new char[1] { ':' };
				List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
				challengeId = int.Parse(stringListFromString[0]);
				rewardId = int.Parse(stringListFromString[1]);
			}
		}

		public readonly int levelId;

		public readonly string name;

		public readonly int chapterId;

		public readonly int actId;

		public readonly int sectionId;

		public readonly int difficulty;

		public readonly int type;

		public readonly int tag;

		public readonly int battleType;

		public readonly int enterTimes;

		public readonly int resetCostType;

		public readonly int resetTimes;

		public readonly int staminaCost;

		public readonly int playerExpReward;

		public readonly int avatarExpReward;

		public readonly float avatarExpInside;

		public readonly int scoinReward;

		public readonly float scoinInside;

		public readonly int maxAvatarExpReward;

		public readonly int maxScoinReward;

		public readonly int maxProgress;

		public readonly List<int> displayDropList;

		public readonly string dropList;

		public readonly string firstDisplayDropList;

		public readonly string firstDropList;

		public readonly int recommendPlayerLevel;

		public readonly int unlockPlayerLevel;

		public readonly int unlockStarNum;

		public readonly List<int> preLevelID;

		public readonly string displayTitle;

		public readonly string displayDetail;

		public readonly string briefPicPath;

		public readonly string detailPicPath;

		public readonly string luaFile;

		public readonly List<LevelChallengeMetaNode> challengeList;

		public readonly int fastBonusTime;

		public readonly int sonicBonusTime;

		public readonly int hardLevel;

		public readonly int reviveTimes;

		public readonly int reviveCostType;

		public readonly int MinEnterNum;

		public readonly List<string> loseDescList;

		public readonly int UseDropSeq;

		public LevelMetaData(int levelId, string name, int chapterId, int actId, int sectionId, int difficulty, int type, int tag, int battleType, int enterTimes, int resetCostType, int resetTimes, int staminaCost, int playerExpReward, int avatarExpReward, float avatarExpInside, int scoinReward, float scoinInside, int maxAvatarExpReward, int maxScoinReward, int maxProgress, List<int> displayDropList, string dropList, string firstDisplayDropList, string firstDropList, int recommendPlayerLevel, int unlockPlayerLevel, int unlockStarNum, List<int> preLevelID, string displayTitle, string displayDetail, string briefPicPath, string detailPicPath, string luaFile, List<LevelChallengeMetaNode> challengeList, int fastBonusTime, int sonicBonusTime, int hardLevel, int reviveTimes, int reviveCostType, int MinEnterNum, List<string> loseDescList, int UseDropSeq)
		{
			this.levelId = levelId;
			this.name = name;
			this.chapterId = chapterId;
			this.actId = actId;
			this.sectionId = sectionId;
			this.difficulty = difficulty;
			this.type = type;
			this.tag = tag;
			this.battleType = battleType;
			this.enterTimes = enterTimes;
			this.resetCostType = resetCostType;
			this.resetTimes = resetTimes;
			this.staminaCost = staminaCost;
			this.playerExpReward = playerExpReward;
			this.avatarExpReward = avatarExpReward;
			this.avatarExpInside = avatarExpInside;
			this.scoinReward = scoinReward;
			this.scoinInside = scoinInside;
			this.maxAvatarExpReward = maxAvatarExpReward;
			this.maxScoinReward = maxScoinReward;
			this.maxProgress = maxProgress;
			this.displayDropList = displayDropList;
			this.dropList = dropList;
			this.firstDisplayDropList = firstDisplayDropList;
			this.firstDropList = firstDropList;
			this.recommendPlayerLevel = recommendPlayerLevel;
			this.unlockPlayerLevel = unlockPlayerLevel;
			this.unlockStarNum = unlockStarNum;
			this.preLevelID = preLevelID;
			this.displayTitle = displayTitle;
			this.displayDetail = displayDetail;
			this.briefPicPath = briefPicPath;
			this.detailPicPath = detailPicPath;
			this.luaFile = luaFile;
			this.challengeList = challengeList;
			this.fastBonusTime = fastBonusTime;
			this.sonicBonusTime = sonicBonusTime;
			this.hardLevel = hardLevel;
			this.reviveTimes = reviveTimes;
			this.reviveCostType = reviveCostType;
			this.MinEnterNum = MinEnterNum;
			this.loseDescList = loseDescList;
			this.UseDropSeq = UseDropSeq;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(levelId, ref lastHash);
			HashUtils.ContentHashOnto(name, ref lastHash);
			HashUtils.ContentHashOnto(chapterId, ref lastHash);
			HashUtils.ContentHashOnto(actId, ref lastHash);
			HashUtils.ContentHashOnto(sectionId, ref lastHash);
			HashUtils.ContentHashOnto(difficulty, ref lastHash);
			HashUtils.ContentHashOnto(type, ref lastHash);
			HashUtils.ContentHashOnto(tag, ref lastHash);
			HashUtils.ContentHashOnto(battleType, ref lastHash);
			HashUtils.ContentHashOnto(enterTimes, ref lastHash);
			HashUtils.ContentHashOnto(resetCostType, ref lastHash);
			HashUtils.ContentHashOnto(resetTimes, ref lastHash);
			HashUtils.ContentHashOnto(staminaCost, ref lastHash);
			HashUtils.ContentHashOnto(playerExpReward, ref lastHash);
			HashUtils.ContentHashOnto(avatarExpReward, ref lastHash);
			HashUtils.ContentHashOnto(avatarExpInside, ref lastHash);
			HashUtils.ContentHashOnto(scoinReward, ref lastHash);
			HashUtils.ContentHashOnto(scoinInside, ref lastHash);
			HashUtils.ContentHashOnto(maxAvatarExpReward, ref lastHash);
			HashUtils.ContentHashOnto(maxScoinReward, ref lastHash);
			HashUtils.ContentHashOnto(maxProgress, ref lastHash);
			if (displayDropList != null)
			{
				foreach (int displayDrop in displayDropList)
				{
					HashUtils.ContentHashOnto(displayDrop, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(dropList, ref lastHash);
			HashUtils.ContentHashOnto(firstDisplayDropList, ref lastHash);
			HashUtils.ContentHashOnto(firstDropList, ref lastHash);
			HashUtils.ContentHashOnto(recommendPlayerLevel, ref lastHash);
			HashUtils.ContentHashOnto(unlockPlayerLevel, ref lastHash);
			HashUtils.ContentHashOnto(unlockStarNum, ref lastHash);
			if (preLevelID != null)
			{
				foreach (int item in preLevelID)
				{
					HashUtils.ContentHashOnto(item, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(displayTitle, ref lastHash);
			HashUtils.ContentHashOnto(displayDetail, ref lastHash);
			HashUtils.ContentHashOnto(briefPicPath, ref lastHash);
			HashUtils.ContentHashOnto(detailPicPath, ref lastHash);
			HashUtils.ContentHashOnto(luaFile, ref lastHash);
			if (challengeList != null)
			{
				foreach (LevelChallengeMetaNode challenge in challengeList)
				{
					HashUtils.ContentHashOnto(challenge.challengeId, ref lastHash);
					HashUtils.ContentHashOnto(challenge.rewardId, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(fastBonusTime, ref lastHash);
			HashUtils.ContentHashOnto(sonicBonusTime, ref lastHash);
			HashUtils.ContentHashOnto(hardLevel, ref lastHash);
			HashUtils.ContentHashOnto(reviveTimes, ref lastHash);
			HashUtils.ContentHashOnto(reviveCostType, ref lastHash);
			HashUtils.ContentHashOnto(MinEnterNum, ref lastHash);
			if (loseDescList != null)
			{
				foreach (string loseDesc in loseDescList)
				{
					HashUtils.ContentHashOnto(loseDesc, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(UseDropSeq, ref lastHash);
		}
	}
}
