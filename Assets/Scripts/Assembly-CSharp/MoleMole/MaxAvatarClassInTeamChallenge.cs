using System.Collections.Generic;
using FullInspector;
using MoleMole.Config;

namespace MoleMole
{
	public class MaxAvatarClassInTeamChallenge : BaseLevelChallenge
	{
		public readonly int targetNum;

		public readonly EntityClass targetClass;

		[ShowInInspector]
		protected bool _finished;

		public MaxAvatarClassInTeamChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			targetNum = _metaData.paramList[0];
			targetClass = (EntityClass)_metaData.paramList[1];
		}

		public override void OnAdded()
		{
			_finished = IsFinished();
		}

		public override bool IsFinished()
		{
			int amountOfSameEntityClassAvatarInTeam = GetAmountOfSameEntityClassAvatarInTeam(targetClass);
			return amountOfSameEntityClassAvatarInTeam >= targetNum;
		}

		public override string GetProcessMsg()
		{
			if (IsFinished())
			{
				return "Succ";
			}
			return "Fail";
		}

		private int GetAmountOfSameEntityClassAvatarInTeam(EntityClass entityClass)
		{
			List<AvatarDataItem> memberList = Singleton<LevelScoreManager>.Instance.memberList;
			int num = 0;
			foreach (AvatarDataItem item in memberList)
			{
				if (item.ClassId == (int)entityClass)
				{
					num++;
				}
			}
			return num;
		}
	}
}
