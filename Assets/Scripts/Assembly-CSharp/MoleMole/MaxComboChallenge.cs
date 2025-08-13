using System;
using FullInspector;

namespace MoleMole
{
	public class MaxComboChallenge : BaseLevelChallenge
	{
		public readonly int targetMaxCombo;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _maxCombo;

		public MaxComboChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_maxCombo = 0;
			targetMaxCombo = _metaData.paramList[0];
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
			return string.Format("[{0}/{1}]", _maxCombo, targetMaxCombo);
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelState>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Remove(levelActor.onLevelComboChanged, new Action<int, int>(OnLevelComboChanged));
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtLevelState)
			{
				return ListenLevelState((EvtLevelState)evt);
			}
			return false;
		}

		private bool ListenLevelState(EvtLevelState evt)
		{
			if (evt.state == EvtLevelState.State.Start)
			{
				LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
				levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Combine(levelActor.onLevelComboChanged, new Action<int, int>(OnLevelComboChanged));
				Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelState>(_helper.levelActor.runtimeID);
			}
			return false;
		}

		private void OnLevelComboChanged(int from, int to)
		{
			if (to <= from)
			{
				return;
			}
			if (_maxCombo < to)
			{
				_maxCombo = to;
				if (_maxCombo >= targetMaxCombo)
				{
					Finish();
				}
			}
		}

		private void Finish()
		{
			_finished = true;
			OnDecided();
		}
	}
}
