using FullInspector;

namespace MoleMole
{
	public class MaxMonsterKilledBySwitchAttackChallenge : BaseLevelChallenge
	{
		public readonly int targetKilledNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _tempKilledNum;

		public MaxMonsterKilledBySwitchAttackChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
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
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.killerID);
			MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			if (actor != null && actor2 != null && (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) ? true : false))
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
