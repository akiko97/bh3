using FullInspector;

namespace MoleMole
{
	[fiInspectorOnly]
	public abstract class BaseLevelChallenge
	{
		public readonly int challengeId;

		protected LevelChallengeHelperPlugin _helper;

		protected LevelChallengeMetaData _metaData;

		public bool active;

		public BaseLevelChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
		{
			_helper = helper;
			_metaData = metaData;
			challengeId = _metaData.challengeId;
			active = true;
		}

		public abstract bool IsFinished();

		public abstract string GetProcessMsg();

		public virtual void Core()
		{
		}

		public virtual void OnDecided()
		{
			active = false;
		}

		public virtual void OnAdded()
		{
		}

		public virtual bool OnEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual bool OnPostEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual bool ListenEvent(BaseEvent evt)
		{
			return false;
		}
	}
}
