using FullInspector;

namespace MoleMole
{
	public class LimitTrapTriggeredChallenge : BaseLevelChallenge
	{
		public readonly int targetNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _trapTriggeredNum;

		public LimitTrapTriggeredChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = true;
			_trapTriggeredNum = 0;
			targetNum = _metaData.paramList[0];
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override string GetProcessMsg()
		{
			if (!IsFinished())
			{
				return "Fail";
			}
			return string.Format("[{0}/{1}]", _trapTriggeredNum, targetNum);
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return ListenBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtFieldEnter)
			{
				return ListenFieldEnter((EvtFieldEnter)evt);
			}
			return false;
		}

		private bool ListenFieldEnter(EvtFieldEnter evt)
		{
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
			BaseActor actor2 = Singleton<EventManager>.Instance.GetActor(evt.otherID);
			if (actor != null && actor is PropObjectActor && actor2 != null && actor2 is AvatarActor && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor2.runtimeID))
			{
				PropObjectActor propObjectActor = actor as PropObjectActor;
				if (propObjectActor != null && propObjectActor.entity is MonoTriggerProp)
				{
					_trapTriggeredNum++;
					if (_trapTriggeredNum >= targetNum)
					{
						Fail();
					}
				}
			}
			return false;
		}

		private bool ListenBeingHit(EvtBeingHit evt)
		{
			if (evt.attackData.rejected)
			{
				return false;
			}
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
			BaseActor actor2 = Singleton<EventManager>.Instance.GetActor(evt.sourceID);
			if (actor != null && actor is AvatarActor && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && actor2 != null && actor2 is PropObjectActor)
			{
				PropObjectActor propObjectActor = actor2 as PropObjectActor;
				if (propObjectActor != null && propObjectActor.entity is MonoTriggerProp)
				{
					_trapTriggeredNum++;
					if (_trapTriggeredNum >= targetNum)
					{
						Fail();
					}
				}
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
