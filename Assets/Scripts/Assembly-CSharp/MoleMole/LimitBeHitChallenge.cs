using FullInspector;

namespace MoleMole
{
	public class LimitBeHitChallenge : BaseLevelChallenge
	{
		public readonly int targetNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _beHitNum;

		public LimitBeHitChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = true;
			_beHitNum = 0;
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
			return string.Format("[{0}/{1}]", _beHitNum, targetNum);
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
			return false;
		}

		private bool ListenBeingHit(EvtBeingHit evt)
		{
			if (evt.attackData.rejected)
			{
				return false;
			}
			if (!evt.attackData.isAnimEventAttack)
			{
				return false;
			}
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
			if (actor != null && actor is AvatarActor && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && evt.attackData.isInComboCount)
			{
				_beHitNum++;
				if (_beHitNum > targetNum)
				{
					Fail();
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
