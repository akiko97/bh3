using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class LevelChallengeMetaData : IHashable
	{
		public readonly int challengeId;

		public readonly int conditionId;

		public readonly List<int> paramList;

		public readonly string diaplayTarget;

		public LevelChallengeMetaData(int challengeId, int conditionId, List<int> paramList, string diaplayTarget)
		{
			this.challengeId = challengeId;
			this.conditionId = conditionId;
			this.paramList = paramList;
			this.diaplayTarget = diaplayTarget;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(challengeId, ref lastHash);
			HashUtils.ContentHashOnto(conditionId, ref lastHash);
			if (paramList != null)
			{
				foreach (int param in paramList)
				{
					HashUtils.ContentHashOnto(param, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(diaplayTarget, ref lastHash);
		}
	}
}
