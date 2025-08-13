using FullInspector;

namespace MoleMole
{
	public class MonsterLastKilledBySwitchInAttackChallenge : BaseLevelChallenge
	{
		[ShowInInspector]
		private bool _finished;

		public MonsterLastKilledBySwitchInAttackChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
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
			return string.Format(string.Empty);
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return ListenKilled((EvtKilled)evt);
			}
			return false;
		}

		private bool ListenKilled(EvtKilled evt)
		{
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.killerID);
			MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (actor != null && actor2 != null && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) && Singleton<MonsterManager>.Instance.LivingMonsterCount() == 0 && mainCamera.followState.slowMotionKillState.active)
			{
				if (string.IsNullOrEmpty(evt.killerAnimEventID) || !actor.config.AnimEvents.ContainsKey(evt.killerAnimEventID))
				{
					return false;
				}
				AttackResult.AttackCategoryTag[] categoryTag = actor.config.AnimEvents[evt.killerAnimEventID].AttackProperty.CategoryTag;
				if (categoryTag == null)
				{
					return false;
				}
				bool flag = false;
				for (int i = 0; i < categoryTag.Length; i++)
				{
					if (categoryTag[i] == AttackResult.AttackCategoryTag.SwitchIn)
					{
						flag = true;
						break;
					}
				}
				if (flag)
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
