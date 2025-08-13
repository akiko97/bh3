using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class TutorialStepData : IHashable
	{
		public readonly int id;

		public readonly string comment;

		public readonly int stepType;

		public readonly string targetUIPath;

		public readonly int handIconPosType;

		public readonly bool playEffect;

		public readonly string scrollUIPath;

		public readonly string guideDesc;

		public readonly int bubblePosType;

		public readonly int nextStepID;

		public readonly float delayTime;

		public readonly List<int> FinishOnStart;

		public readonly List<int> FinishOnEnd;

		public TutorialStepData(int id, string comment, int stepType, string targetUIPath, int handIconPosType, bool playEffect, string scrollUIPath, string guideDesc, int bubblePosType, int nextStepID, float delayTime, List<int> FinishOnStart, List<int> FinishOnEnd)
		{
			this.id = id;
			this.comment = comment;
			this.stepType = stepType;
			this.targetUIPath = targetUIPath;
			this.handIconPosType = handIconPosType;
			this.playEffect = playEffect;
			this.scrollUIPath = scrollUIPath;
			this.guideDesc = guideDesc;
			this.bubblePosType = bubblePosType;
			this.nextStepID = nextStepID;
			this.delayTime = delayTime;
			this.FinishOnStart = FinishOnStart;
			this.FinishOnEnd = FinishOnEnd;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(id, ref lastHash);
			HashUtils.ContentHashOnto(comment, ref lastHash);
			HashUtils.ContentHashOnto(stepType, ref lastHash);
			HashUtils.ContentHashOnto(targetUIPath, ref lastHash);
			HashUtils.ContentHashOnto(handIconPosType, ref lastHash);
			HashUtils.ContentHashOnto(playEffect, ref lastHash);
			HashUtils.ContentHashOnto(scrollUIPath, ref lastHash);
			HashUtils.ContentHashOnto(guideDesc, ref lastHash);
			HashUtils.ContentHashOnto(bubblePosType, ref lastHash);
			HashUtils.ContentHashOnto(nextStepID, ref lastHash);
			HashUtils.ContentHashOnto(delayTime, ref lastHash);
			if (FinishOnStart != null)
			{
				foreach (int item in FinishOnStart)
				{
					HashUtils.ContentHashOnto(item, ref lastHash);
				}
			}
			if (FinishOnEnd == null)
			{
				return;
			}
			foreach (int item2 in FinishOnEnd)
			{
				HashUtils.ContentHashOnto(item2, ref lastHash);
			}
		}
	}
}
