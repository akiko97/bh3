using System.Collections.Generic;
using FullInspector;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class MaxMonsterKilledInCertainTimeChallenge : BaseLevelChallenge
	{
		public readonly int targetKilledAmount;

		public readonly float targetTimeWindow;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _tempKilledAmountInWindow;

		[ShowInInspector]
		private int _tempMaxKilledAmountInWindow;

		[ShowInInspector]
		private bool _inStastics;

		[ShowInInspector]
		private float _stasticsTimer;

		private List<Tuple<float, int>> _killedAmountLs;

		public MaxMonsterKilledInCertainTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_tempKilledAmountInWindow = 0;
			_tempMaxKilledAmountInWindow = 0;
			_stasticsTimer = 0f;
			targetTimeWindow = _metaData.paramList[0];
			targetKilledAmount = _metaData.paramList[1];
			_killedAmountLs = new List<Tuple<float, int>>();
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
			return string.Format("[{0}/{1}]", _tempMaxKilledAmountInWindow, targetKilledAmount);
		}

		public void StartStastics()
		{
			_inStastics = true;
			_tempKilledAmountInWindow = 0;
		}

		public void StopStastics()
		{
			_inStastics = false;
		}

		private void UpdateStastics(float currentTime, float window)
		{
			for (int i = 0; i < _killedAmountLs.Count; i++)
			{
				Tuple<float, int> tuple = _killedAmountLs[i];
				if (tuple != null && currentTime - tuple.Item1 > window)
				{
					_killedAmountLs[i] = null;
				}
			}
		}

		private bool CheckStastics(int targetKilledAmount)
		{
			_tempKilledAmountInWindow = 0;
			foreach (Tuple<float, int> killedAmountL in _killedAmountLs)
			{
				if (killedAmountL != null)
				{
					_tempKilledAmountInWindow += killedAmountL.Item2;
				}
			}
			_tempMaxKilledAmountInWindow = Mathf.Max(_tempMaxKilledAmountInWindow, _tempKilledAmountInWindow);
			return _tempKilledAmountInWindow >= targetKilledAmount;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelState>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelState>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtLevelState)
			{
				return ListenLevelStatge((EvtLevelState)evt);
			}
			if (evt is EvtKilled)
			{
				return ListenKilled((EvtKilled)evt);
			}
			return false;
		}

		private bool ListenLevelStatge(EvtLevelState evt)
		{
			if (evt.state == EvtLevelState.State.Start)
			{
				StartStastics();
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
			int index = _killedAmountLs.SeekAddPosition();
			_killedAmountLs[index] = Tuple.Create(_stasticsTimer, 1);
			return false;
		}

		public override void Core()
		{
			if (_inStastics)
			{
				_stasticsTimer += Time.deltaTime;
				UpdateStastics(_stasticsTimer, targetTimeWindow);
				if (CheckStastics(targetKilledAmount))
				{
					Finish();
				}
			}
		}

		private void Finish()
		{
			_finished = true;
			StopStastics();
			OnDecided();
		}
	}
}
