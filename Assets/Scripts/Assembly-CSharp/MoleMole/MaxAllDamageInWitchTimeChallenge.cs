using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class MaxAllDamageInWitchTimeChallenge : BaseLevelChallenge
	{
		public readonly float targetDamage;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private float _tempAllDamage;

		private bool _inStastics;

		public MaxAllDamageInWitchTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_tempAllDamage = 0f;
			targetDamage = _metaData.paramList[0];
			_inStastics = false;
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
			return string.Format("[{0}/{1}]", Mathf.FloorToInt(_tempAllDamage), Mathf.FloorToInt(targetDamage));
		}

		public void StartStastics()
		{
			_inStastics = true;
		}

		public void StopStastics()
		{
			_inStastics = false;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelBuffState>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return ListenBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtLevelBuffState)
			{
				return ListenLevelBuffState((EvtLevelBuffState)evt);
			}
			return false;
		}

		private bool ListenLevelBuffState(EvtLevelBuffState evt)
		{
			if (evt.state == LevelBuffState.Start && evt.levelBuff == LevelBuffType.WitchTime)
			{
				StartStastics();
			}
			if (evt.state == LevelBuffState.Stop && evt.levelBuff == LevelBuffType.WitchTime)
			{
				StopStastics();
			}
			return false;
		}

		private bool ListenBeingHit(EvtBeingHit evt)
		{
			if (evt.attackData.rejected)
			{
				return false;
			}
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.sourceID);
			if (actor == null || !(actor is AvatarActor) || !_inStastics || !Singleton<LevelManager>.Instance.levelActor.IsLevelBuffActive(LevelBuffType.WitchTime))
			{
				return false;
			}
			_tempAllDamage += evt.attackData.damage;
			if (_tempAllDamage >= targetDamage)
			{
				Finish();
			}
			return true;
		}

		private void Finish()
		{
			_finished = true;
			StopStastics();
			OnDecided();
		}
	}
}
