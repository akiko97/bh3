using System.Collections.Generic;
using FullInspector;
using MoleMole.Config;

namespace MoleMole
{
	public class MaxAvatarNatureInTeamChallenge : BaseLevelChallenge
	{
		public readonly int targetNum;

		public readonly EntityNature targetNature;

		[ShowInInspector]
		protected bool _finished;

		public MaxAvatarNatureInTeamChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			targetNum = _metaData.paramList[0];
			targetNature = (EntityNature)_metaData.paramList[1];
		}

		public override void OnAdded()
		{
			_finished = IsFinished();
		}

		public override string GetProcessMsg()
		{
			if (IsFinished())
			{
				return "Succ";
			}
			return "Fail";
		}

		private int GetAmountOfSameEntityNatureAvatarInTeam(EntityNature entityNature)
		{
			List<AvatarDataItem> memberList = Singleton<LevelScoreManager>.Instance.memberList;
			int num = 0;
			foreach (AvatarDataItem item in memberList)
			{
				if (item.Attribute == (int)targetNature)
				{
					num++;
				}
			}
			return num;
		}

		public override bool IsFinished()
		{
			int amountOfSameEntityNatureAvatarInTeam = GetAmountOfSameEntityNatureAvatarInTeam(targetNature);
			return amountOfSameEntityNatureAvatarInTeam >= targetNum;
		}
	}
}
