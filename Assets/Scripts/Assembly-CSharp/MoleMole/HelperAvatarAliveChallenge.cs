using FullInspector;

namespace MoleMole
{
	public class HelperAvatarAliveChallenge : BaseLevelChallenge
	{
		[ShowInInspector]
		private bool _finished;

		private bool _hasHelperAvatar;

		public HelperAvatarAliveChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = true;
			_hasHelperAvatar = false;
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override string GetProcessMsg()
		{
			if (IsFinished())
			{
				return "Doing";
			}
			return "Fail";
		}

		public override void OnAdded()
		{
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return ListenKilled((EvtKilled)evt);
			}
			if (evt is EvtStageReady)
			{
				return ListenStageReady((EvtStageReady)evt);
			}
			return false;
		}

		private bool ListenStageReady(EvtStageReady evt)
		{
			BaseMonoAvatar helperAvatar = Singleton<AvatarManager>.Instance.GetHelperAvatar();
			_hasHelperAvatar = helperAvatar != null;
			_finished = _hasHelperAvatar;
			return false;
		}

		private bool ListenKilled(EvtKilled evt)
		{
			if (!_hasHelperAvatar)
			{
				return false;
			}
			if (Singleton<AvatarManager>.Instance.IsHelperAvatar(evt.targetID))
			{
				Fail();
			}
			return false;
		}

		private void Fail()
		{
			_finished = false;
			OnDecided();
		}
	}
}
