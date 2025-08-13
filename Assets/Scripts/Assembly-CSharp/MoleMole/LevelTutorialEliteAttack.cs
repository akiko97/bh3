using FullInspector;

namespace MoleMole
{
	public class LevelTutorialEliteAttack : BaseLevelTutorial
	{
		[ShowInInspector]
		private bool _finished;

		public LevelTutorialEliteAttack(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtShieldBroken>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtShieldBroken>(_helper.levelActor.runtimeID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialEliteAttack, this));
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return ListenMonsterBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtShieldBroken)
			{
				return ListenShieldBroken((EvtShieldBroken)evt);
			}
			return false;
		}

		private bool ListenMonsterBeingHit(EvtBeingHit evt)
		{
			bool flag = Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.sourceID);
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			if (flag && actor != null)
			{
				BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(evt.sourceID);
				if (avatarByRuntimeID != null && avatarByRuntimeID.GetAttackTarget() != null && actor.isElite && base.step == 0 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
				{
					ActiveCurrentStep();
					WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep1);
				}
			}
			return false;
		}

		private bool ListenShieldBroken(EvtShieldBroken evt)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			if (actor != null && actor.isElite && base.step == 1 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(0.5f, ShowTutorialStep2);
			}
			return false;
		}

		public override void Core()
		{
			if (active && IsAllStepDone())
			{
				Finish();
			}
		}

		private void ShowTutorialStep1()
		{
			NotifyStep(NotifyTypes.TutorialEliteAttack);
			PauseGame();
			ActiveCurrentStep();
		}

		private void ShowTutorialStep2()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialEliteAttack, this));
			PauseGame();
		}

		public void OnTutorialStep1Done()
		{
			ResumeGame();
			MoveToNextStep();
		}

		public void OnTutorialStep2Done()
		{
			ResumeGame();
			MoveToNextStep();
		}

		private void Finish()
		{
			_finished = true;
			Singleton<LevelTutorialModule>.Instance.MarkTutorialIDFinish(tutorialId);
			OnDecided();
		}

		private void Fail()
		{
			_finished = false;
			OnDecided();
		}
	}
}
