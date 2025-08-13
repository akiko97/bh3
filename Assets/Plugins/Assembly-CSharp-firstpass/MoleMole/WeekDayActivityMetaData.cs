using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class WeekDayActivityMetaData : IHashable
	{
		public readonly int weekDayActivityID;

		public readonly int seriesID;

		public readonly int activityType;

		public readonly string title;

		public readonly string desc;

		public readonly string descLock;

		public readonly string smallImgPath;

		public readonly string bgImgPath;

		public readonly string enterImgPath;

		public readonly string levelPanelPath;

		public readonly List<int> levelIDList;

		public readonly int maxEnterTimes;

		public readonly int minPlayerLevel;

		public readonly int displayLeftTime;

		public readonly int showActivityShopEntry;

		public WeekDayActivityMetaData(int weekDayActivityID, int seriesID, int activityType, string title, string desc, string descLock, string smallImgPath, string bgImgPath, string enterImgPath, string levelPanelPath, List<int> levelIDList, int maxEnterTimes, int minPlayerLevel, int displayLeftTime, int showActivityShopEntry)
		{
			this.weekDayActivityID = weekDayActivityID;
			this.seriesID = seriesID;
			this.activityType = activityType;
			this.title = title;
			this.desc = desc;
			this.descLock = descLock;
			this.smallImgPath = smallImgPath;
			this.bgImgPath = bgImgPath;
			this.enterImgPath = enterImgPath;
			this.levelPanelPath = levelPanelPath;
			this.levelIDList = levelIDList;
			this.maxEnterTimes = maxEnterTimes;
			this.minPlayerLevel = minPlayerLevel;
			this.displayLeftTime = displayLeftTime;
			this.showActivityShopEntry = showActivityShopEntry;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(weekDayActivityID, ref lastHash);
			HashUtils.ContentHashOnto(seriesID, ref lastHash);
			HashUtils.ContentHashOnto(activityType, ref lastHash);
			HashUtils.ContentHashOnto(title, ref lastHash);
			HashUtils.ContentHashOnto(desc, ref lastHash);
			HashUtils.ContentHashOnto(descLock, ref lastHash);
			HashUtils.ContentHashOnto(smallImgPath, ref lastHash);
			HashUtils.ContentHashOnto(bgImgPath, ref lastHash);
			HashUtils.ContentHashOnto(enterImgPath, ref lastHash);
			HashUtils.ContentHashOnto(levelPanelPath, ref lastHash);
			if (levelIDList != null)
			{
				foreach (int levelID in levelIDList)
				{
					HashUtils.ContentHashOnto(levelID, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(maxEnterTimes, ref lastHash);
			HashUtils.ContentHashOnto(minPlayerLevel, ref lastHash);
			HashUtils.ContentHashOnto(displayLeftTime, ref lastHash);
			HashUtils.ContentHashOnto(showActivityShopEntry, ref lastHash);
		}
	}
}
