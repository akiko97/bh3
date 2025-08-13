using System.Collections.Generic;
using FullInspector;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class MaxDamageInCertainTimeChallenge : BaseLevelChallenge
	{
		public readonly float targetDamage;

		public readonly float targetTimeWindow;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private float _tempDamageInWindow;

		[ShowInInspector]
		private float _tempMaxDamageInWindow;

		[ShowInInspector]
		private bool _inStastics;

		[ShowInInspector]
		private float _stasticsTimer;

		private List<Tuple<float, float>> _damageLs;

		public MaxDamageInCertainTimeChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_tempDamageInWindow = 0f;
			_tempMaxDamageInWindow = 0f;
			_stasticsTimer = 0f;
			targetTimeWindow = _metaData.paramList[0];
			targetDamage = _metaData.paramList[1];
			_damageLs = new List<Tuple<float, float>>();
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
			return string.Format("[{0}/{1}]", Mathf.FloorToInt(_tempMaxDamageInWindow), Mathf.FloorToInt(targetDamage));
		}

		public void StartStastics()
		{
			_inStastics = true;
			_tempDamageInWindow = 0f;
		}

		public void StopStastics()
		{
			_inStastics = false;
		}

		private void UpdateStastics(float currentTime, float window)
		{
			for (int i = 0; i < _damageLs.Count; i++)
			{
				Tuple<float, float> tuple = _damageLs[i];
				if (tuple != null && currentTime - tuple.Item1 > window)
				{
					_damageLs[i] = null;
				}
			}
		}

		private bool CheckStastics(float targetDamage)
		{
			_tempDamageInWindow = 0f;
			foreach (Tuple<float, float> damageL in _damageLs)
			{
				if (damageL != null)
				{
					_tempDamageInWindow += damageL.Item2;
				}
			}
			_tempMaxDamageInWindow = Mathf.Max(_tempMaxDamageInWindow, _tempDamageInWindow);
			return _tempDamageInWindow >= targetDamage;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelState>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelState>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return ListenBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtLevelState)
			{
				return ListenLevelStatge((EvtLevelState)evt);
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

		private bool ListenBeingHit(EvtBeingHit evt)
		{
			if (evt.attackData.rejected)
			{
				return false;
			}
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.sourceID);
			if (actor == null || !(actor is AvatarActor) || !_inStastics)
			{
				return false;
			}
			int index = _damageLs.SeekAddPosition();
			_damageLs[index] = Tuple.Create(_stasticsTimer, evt.attackData.GetTotalDamage());
			return false;
		}

		public override void Core()
		{
			if (_inStastics)
			{
				_stasticsTimer += Time.deltaTime;
				UpdateStastics(_stasticsTimer, targetTimeWindow);
				if (CheckStastics(targetDamage))
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
