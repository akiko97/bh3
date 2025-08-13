using FullInspector;

namespace MoleMole
{
	public class SpecialLevelTimeChallenge : BaseLevelChallenge
	{
		public readonly float targetLevelTime;

		[ShowInInspector]
		private bool _finished;

		public SpecialLevelTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = true;
			if (metaData.conditionId != 27)
			{
				LevelMetaData levelMetaDataByKey = LevelMetaDataReader.GetLevelMetaDataByKey(Singleton<LevelScoreManager>.Instance.LevelId);
				targetLevelTime = ((metaData.conditionId != 29) ? levelMetaDataByKey.fastBonusTime : levelMetaDataByKey.sonicBonusTime);
			}
			else
			{
				targetLevelTime = _metaData.paramList[0];
			}
		}

		public override bool IsFinished()
		{
			LevelActorTimerPlugin plugin = _helper.levelActor.GetPlugin<LevelActorTimerPlugin>();
			if (plugin == null)
			{
				_finished = false;
			}
			else if (_metaData.conditionId == 27)
			{
				_finished = true;
			}
			else
			{
				_finished = plugin.Timer <= targetLevelTime;
			}
			return _finished;
		}

		public override string GetProcessMsg()
		{
			if (_metaData.conditionId == 27)
			{
				return string.Format(string.Empty);
			}
			LevelActorTimerPlugin plugin = _helper.levelActor.GetPlugin<LevelActorTimerPlugin>();
			float num = ((plugin == null) ? 0f : plugin.Timer);
			return string.Format("[{0}/{1}]", (int)num, (int)targetLevelTime);
		}
	}
}
