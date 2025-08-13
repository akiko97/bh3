using FullInspector;

namespace MoleMole
{
	public class MaxBoxOpenedChallenge : BaseLevelChallenge
	{
		public readonly int targetBoxOpenedNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _tempBoxOpenedNum;

		public MaxBoxOpenedChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_tempBoxOpenedNum = 0;
			targetBoxOpenedNum = _metaData.paramList[0];
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
			return string.Format("[{0}/{1}]", _tempBoxOpenedNum, targetBoxOpenedNum);
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
			BaseActor actor2 = Singleton<EventManager>.Instance.GetActor(evt.targetID);
			if (actor != null && actor is AvatarActor && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && actor2 != null && actor2 is PropObjectActor)
			{
				PropObjectActor propObjectActor = actor2 as PropObjectActor;
				if (propObjectActor != null && propObjectActor.config.Name.Contains("Box"))
				{
					_tempBoxOpenedNum++;
					if (_tempBoxOpenedNum >= targetBoxOpenedNum)
					{
						Finish();
					}
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
