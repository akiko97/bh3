using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class LimitTotalDamageTakenChallenge : BaseLevelChallenge
	{
		public readonly float targetDamageTaken;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private float _tempDamageTaken;

		public LimitTotalDamageTakenChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = true;
			_tempDamageTaken = 0f;
			targetDamageTaken = _metaData.paramList[0];
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
			return string.Format("[{0}/{1}]", Mathf.FloorToInt(_tempDamageTaken), Mathf.FloorToInt(targetDamageTaken));
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
			if (actor != null && actor is AvatarActor && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && evt.attackData.IsFinalResolved())
			{
				_tempDamageTaken += evt.attackData.GetTotalDamage();
				if (_tempDamageTaken > targetDamageTaken)
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
