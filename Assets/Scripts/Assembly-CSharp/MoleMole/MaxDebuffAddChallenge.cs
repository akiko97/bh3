using System.Collections.Generic;
using FullInspector;

namespace MoleMole
{
	public class MaxDebuffAddChallenge : BaseLevelChallenge
	{
		public readonly int targetDebuffAddNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private List<uint> _addDebuffMonsterList;

		public MaxDebuffAddChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			targetDebuffAddNum = _metaData.paramList[0];
			_addDebuffMonsterList = new List<uint>();
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
			return string.Format("[{0}/{1}]", _addDebuffMonsterList.Count, targetDebuffAddNum);
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtBuffAdd>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtBuffAdd>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtBuffAdd)
			{
				return ListenBuffAdd((EvtBuffAdd)evt);
			}
			return false;
		}

		private bool ListenBuffAdd(EvtBuffAdd evt)
		{
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
			bool flag = (evt.abilityState & (AbilityState.Bleed | AbilityState.Stun | AbilityState.Paralyze | AbilityState.Burn | AbilityState.Poisoned | AbilityState.Frozen | AbilityState.MoveSpeedDown | AbilityState.AttackSpeedDown | AbilityState.Weak | AbilityState.Fragile | AbilityState.TargetLocked | AbilityState.Tied)) != 0;
			if (actor != null && actor is MonsterActor && flag)
			{
				if (!_addDebuffMonsterList.Contains(actor.runtimeID))
				{
					_addDebuffMonsterList.Add(actor.runtimeID);
				}
				if (_addDebuffMonsterList.Count >= targetDebuffAddNum)
				{
					Finish();
				}
			}
			return false;
		}

		private void Finish()
		{
			_finished = true;
			OnDecided();
		}
	}
}
