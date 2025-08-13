using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class MaxMonsterKilledInWitchTimeChallenge : BaseLevelChallenge
	{
		public readonly int targetKilledAmount;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _tempKilledAmount;

		[ShowInInspector]
		private int _tempMaxKilledAmount;

		private bool _inStastics;

		public MaxMonsterKilledInWitchTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_tempKilledAmount = 0;
			_tempMaxKilledAmount = 0;
			targetKilledAmount = _metaData.paramList[0];
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
			return string.Format("[{0}/{1}]", _tempMaxKilledAmount, targetKilledAmount);
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
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelBuffState>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtLevelBuffState)
			{
				return ListenLevelBuffState((EvtLevelBuffState)evt);
			}
			if (evt is EvtKilled)
			{
				return ListenKilled((EvtKilled)evt);
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

		private bool ListenKilled(EvtKilled evt)
		{
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.killerID);
			MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			if (actor == null || actor2 == null || !_inStastics)
			{
				return false;
			}
			_tempKilledAmount++;
			_tempMaxKilledAmount = Mathf.Max(_tempMaxKilledAmount, _tempKilledAmount);
			if (_tempKilledAmount >= targetKilledAmount)
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
