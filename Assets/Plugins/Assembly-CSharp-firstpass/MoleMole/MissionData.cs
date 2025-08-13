using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MissionData : IHashable
	{
		public readonly int id;

		public readonly string comment;

		public readonly int type;

		public readonly int subType;

		public readonly string title;

		public readonly string description;

		public readonly string thumb;

		public readonly int finishWay;

		public readonly int finishParaInt;

		public readonly string finishParaStr;

		public readonly int totalProgress;

		public readonly int rewardId;

		public readonly bool isAutoReward;

		public readonly int LinkType;

		public readonly int LinkParam;

		public readonly List<int> LinkParams;

		public readonly int PreviewTime;

		public readonly int NoDisplay;

		public MissionData(int id, string comment, int type, int subType, string title, string description, string thumb, int finishWay, int finishParaInt, string finishParaStr, int totalProgress, int rewardId, bool isAutoReward, int LinkType, int LinkParam, List<int> LinkParams, int PreviewTime, int NoDisplay)
		{
			this.id = id;
			this.comment = comment;
			this.type = type;
			this.subType = subType;
			this.title = title;
			this.description = description;
			this.thumb = thumb;
			this.finishWay = finishWay;
			this.finishParaInt = finishParaInt;
			this.finishParaStr = finishParaStr;
			this.totalProgress = totalProgress;
			this.rewardId = rewardId;
			this.isAutoReward = isAutoReward;
			this.LinkType = LinkType;
			this.LinkParam = LinkParam;
			this.LinkParams = LinkParams;
			this.PreviewTime = PreviewTime;
			this.NoDisplay = NoDisplay;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(id, ref lastHash);
			HashUtils.ContentHashOnto(comment, ref lastHash);
			HashUtils.ContentHashOnto(type, ref lastHash);
			HashUtils.ContentHashOnto(subType, ref lastHash);
			HashUtils.ContentHashOnto(title, ref lastHash);
			HashUtils.ContentHashOnto(description, ref lastHash);
			HashUtils.ContentHashOnto(thumb, ref lastHash);
			HashUtils.ContentHashOnto(finishWay, ref lastHash);
			HashUtils.ContentHashOnto(finishParaInt, ref lastHash);
			HashUtils.ContentHashOnto(finishParaStr, ref lastHash);
			HashUtils.ContentHashOnto(totalProgress, ref lastHash);
			HashUtils.ContentHashOnto(rewardId, ref lastHash);
			HashUtils.ContentHashOnto(isAutoReward, ref lastHash);
			HashUtils.ContentHashOnto(LinkType, ref lastHash);
			HashUtils.ContentHashOnto(LinkParam, ref lastHash);
			if (LinkParams != null)
			{
				foreach (int linkParam in LinkParams)
				{
					HashUtils.ContentHashOnto(linkParam, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(PreviewTime, ref lastHash);
			HashUtils.ContentHashOnto(NoDisplay, ref lastHash);
		}
	}
}
