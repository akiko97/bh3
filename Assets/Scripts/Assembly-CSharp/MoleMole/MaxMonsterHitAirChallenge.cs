using FullInspector;

namespace MoleMole
{
	public class MaxMonsterHitAirChallenge : BaseLevelChallenge
	{
		public readonly int targetHitAirAmount;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _tempHitAirAmount;

		public MaxMonsterHitAirChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_tempHitAirAmount = 0;
			targetHitAirAmount = _metaData.paramList[0];
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
			return string.Format("[{0}/{1}]", _tempHitAirAmount, targetHitAirAmount);
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
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.sourceID);
			MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			if (actor == null || actor2 == null)
			{
				return false;
			}
			BaseMonoMonster monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(actor2.runtimeID);
			if (monsterByRuntimeID.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
			{
				return false;
			}
			if (evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowAirBlow || evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow || evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUp || evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow)
			{
				_tempHitAirAmount++;
				if (_tempHitAirAmount >= targetHitAirAmount)
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
