using FullInspector;

namespace MoleMole
{
	public class MaxMonsterKilledChallenge : BaseLevelChallenge
	{
		public readonly int targetKilledNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _tempKilledNum;

		public MaxMonsterKilledChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_tempKilledNum = 0;
			targetKilledNum = _metaData.paramList[0];
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override string GetProcessMsg()
		{
			if (IsFinished())
			{
				return "Succ";
			}
			return string.Format("[{0}/{1}]", _tempKilledNum, targetKilledNum);
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return ListenKilled((EvtKilled)evt);
			}
			return false;
		}

		private bool ListenKilled(EvtKilled evt)
		{
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.killerID);
			if (actor != null && actor is AvatarActor && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID))
			{
				_tempKilledNum++;
				if (_tempKilledNum >= targetKilledNum)
				{
					Finish();
				}
			}
			return false;
		}

		private void Finish()
		{
			_finished = true;
			OnDecided();
		}
	}
}
