using FullInspector;

namespace MoleMole
{
	public class LimitBeHitDownChallenge : BaseLevelChallenge
	{
		public readonly int targetDownNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _beHitDownNum;

		public LimitBeHitDownChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = true;
			_beHitDownNum = 0;
			targetDownNum = _metaData.paramList[0];
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
			return string.Format("[{0}/{1}]", _beHitDownNum, targetDownNum);
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
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
			if (actor != null && actor is AvatarActor && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID))
			{
				if (evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.KnockDown)
				{
					_beHitDownNum++;
				}
				if (_beHitDownNum > targetDownNum)
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
