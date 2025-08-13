using FullInspector;

namespace MoleMole
{
	public class MaxQTETriggeredChallenge : BaseLevelChallenge
	{
		public readonly int targetNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _qteTiggeredNum;

		public MaxQTETriggeredChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			targetNum = _metaData.paramList[0];
			_qteTiggeredNum = 0;
		}

		public override bool IsFinished()
		{
			_finished = _qteTiggeredNum >= targetNum;
			return _finished;
		}

		public override string GetProcessMsg()
		{
			if (IsFinished())
			{
				return "Succ";
			}
			return string.Format("[{0}/{1}]", _qteTiggeredNum, targetNum);
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtQTEFire>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtQTEFire>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtQTEFire)
			{
				return ListenQTEFire((EvtQTEFire)evt);
			}
			return false;
		}

		private bool ListenQTEFire(EvtQTEFire evt)
		{
			if (Singleton<AvatarManager>.Instance.IsPlayerAvatar(evt.targetID))
			{
				_qteTiggeredNum++;
			}
			if (_qteTiggeredNum >= targetNum)
			{
				Finish();
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
