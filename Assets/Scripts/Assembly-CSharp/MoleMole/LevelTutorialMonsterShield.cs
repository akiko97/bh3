using FullInspector;

namespace MoleMole
{
	public class LevelTutorialMonsterShield : BaseLevelTutorial
	{
		[ShowInInspector]
		private bool _finished;

		private string _monsterSubTypeName;

		private EntityTimer _pauseTimer;

		public LevelTutorialMonsterShield(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_monsterSubTypeName = "UL_040";
			_pauseTimer = new EntityTimer(3f);
			_pauseTimer.Reset(false);
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(_helper.levelActor.runtimeID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterShield, this));
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtMonsterCreated)
			{
				return ListenMonsterCreated((EvtMonsterCreated)evt);
			}
			if (evt is EvtAttackStart)
			{
				return ListenAttackStart((EvtAttackStart)evt);
			}
			return false;
		}

		private bool ListenAttackStart(EvtAttackStart evt)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			if (actor != null && actor.metaConfig.subTypeName == _monsterSubTypeName && actor.entity.CurrentSkillID == "ATK01" && base.step == 1 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep2);
			}
			return false;
		}

		private bool ListenMonsterCreated(EvtMonsterCreated evt)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.monsterID);
			if (actor != null && actor.metaConfig.subTypeName == _monsterSubTypeName && base.step == 0 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep1);
			}
			return false;
		}

		public override void Core()
		{
			if (!active)
			{
				return;
			}
			if (_pauseTimer.isActive)
			{
				_pauseTimer.Core(1f);
				if (_pauseTimer.isTimeUp)
				{
					_pauseTimer.Reset(false);
					OnTutorialStep2Done();
				}
			}
			if (IsAllStepDone())
			{
				Finish();
			}
		}

		private void ShowTutorialStep1()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterShield, this));
			ActiveCurrentStep();
			PauseGame();
		}

		public void OnTutoriaStep1Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void ShowTutorialStep2()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterShield, this));
			ActiveCurrentStep();
			PauseGame();
			_pauseTimer.Reset(true);
		}

		public void OnTutorialStep2Done()
		{
			MoveToNextStep();
			if (base.step == 2 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep3);
			}
		}

		private void ShowTutorialStep3()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterShield, this));
			ActiveCurrentStep();
		}

		public void OnTutorialStep3Done()
		{
			MoveToNextStep();
			ResumeGame();
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
