using FullInspector;

namespace MoleMole
{
	public class LimitBeAddedDebuffChallenge : BaseLevelChallenge
	{
		public readonly int targetBeAddedDebuffNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _tempBeAddedDebuffNum;

		public LimitBeAddedDebuffChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = true;
			_tempBeAddedDebuffNum = 0;
			targetBeAddedDebuffNum = _metaData.paramList[0];
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
			return "Fail";
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
			if (actor != null && actor is AvatarActor && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && flag)
			{
				_tempBeAddedDebuffNum++;
				if (_tempBeAddedDebuffNum > targetBeAddedDebuffNum)
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
