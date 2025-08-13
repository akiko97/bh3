namespace MoleMole
{
	public class LimitAvatarKilledChallenge : BaseLevelChallenge
	{
		public readonly int targetNum;

		private int _avatarKilledNum;

		public LimitAvatarKilledChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			targetNum = _metaData.paramList[0];
			_avatarKilledNum = 0;
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
				return ListenKill((EvtKilled)evt);
			}
			return false;
		}

		private bool ListenKill(EvtKilled evt)
		{
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
			if (num == 3)
			{
				if (!Singleton<AvatarManager>.Instance.IsPlayerAvatar(evt.targetID))
				{
					return false;
				}
				_avatarKilledNum++;
			}
			return true;
		}

		public override bool IsFinished()
		{
			return _avatarKilledNum <= targetNum;
		}

		public override string GetProcessMsg()
		{
			if (IsFinished())
			{
				return "Doing";
			}
			return "Fail";
		}
	}
}
