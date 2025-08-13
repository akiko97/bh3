using System.Collections.Generic;

namespace MoleMole
{
	public class LimitAvatarChallege : BaseLevelChallenge
	{
		public readonly int targetNum;

		public LimitAvatarChallege(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			targetNum = _metaData.paramList[0];
		}

		public override bool IsFinished()
		{
			List<AvatarDataItem> memberList = Singleton<LevelScoreManager>.Instance.memberList;
			return memberList.Count <= targetNum;
		}

		public override string GetProcessMsg()
		{
			if (IsFinished())
			{
				return "Succ";
			}
			return "Fail";
		}
	}
}
