using System.Collections.Generic;

namespace MoleMole
{
	public class LevelChallengeDataItem
	{
		public int challengeId;

		private LevelChallengeMetaData _metaData;

		public bool Finished;

		public string DisplayTarget
		{
			get
			{
				return LocalizationGeneralLogic.GetTextWithParamArray(_metaData.diaplayTarget, _metaData.paramList.ToArray());
			}
		}

		public LevelChallengeDataItem(int challengeId, LevelMetaData levelMeta, int rewardId = 0)
		{
			this.challengeId = challengeId;
			_metaData = LevelChallengeMetaDataReader.GetLevelChallengeMetaDataByKey(challengeId);
			if (IsFinishStageFastChallenge() || IsFinishStageVeryFastChallenge())
			{
				int item = ((!IsFinishStageFastChallenge()) ? levelMeta.sonicBonusTime : levelMeta.fastBonusTime);
				_metaData = new LevelChallengeMetaData(_metaData.challengeId, _metaData.conditionId, new List<int> { item }, _metaData.diaplayTarget);
			}
			Finished = false;
		}

		public bool IsSpecialChallenge()
		{
			return IsFinishStageNomalChallenge() || IsFinishStageFastChallenge() || IsFinishStageVeryFastChallenge();
		}

		public bool IsFinishStageNomalChallenge()
		{
			return _metaData.conditionId == 27;
		}

		public bool IsFinishStageFastChallenge()
		{
			return _metaData.conditionId == 28;
		}

		public bool IsFinishStageVeryFastChallenge()
		{
			return _metaData.conditionId == 29;
		}
	}
}
