using FullInspector;

namespace MoleMole
{
	public class LevelTutorialPlayerTeaching : BaseLevelTutorial
	{
		[ShowInInspector]
		private bool _finished;

		private MonsterActor _witchTimeActor;

		private bool _witchTimeAttacking;

		private bool _lastMonsterBorn;

		public LevelTutorialPlayerTeaching(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_witchTimeAttacking = false;
			_lastMonsterBorn = false;
			_witchTimeActor = null;
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtTutorialState>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtStageCreated>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtTutorialState)
			{
				return ListenTutorialState((EvtTutorialState)evt);
			}
			if (evt is EvtStageCreated)
			{
				return ListenStageCreated((EvtStageCreated)evt);
			}
			if (evt is EvtFieldEnter)
			{
				return ListenFieldEnter((EvtFieldEnter)evt);
			}
			if (evt is EvtMonsterCreated)
			{
				return ListenMonsterCreated((EvtMonsterCreated)evt);
			}
			if (evt is EvtKilled)
			{
				return ListenMonsterKilled((EvtKilled)evt);
			}
			if (evt is EvtAttackStart)
			{
				return ListenMonsterAttackStart((EvtAttackStart)evt);
			}
			if (evt is EvtAvatarCreated)
			{
				return OnAvatarCreated((EvtAvatarCreated)evt);
			}
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID))
			{
			}
			return false;
		}

		private bool OnAvatarCreated(EvtAvatarCreated evt)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(localAvatar.GetRuntimeID());
			if (actor != null)
			{
				actor.AddAbilityState(AbilityState.Undamagable, true);
			}
			return false;
		}

		private bool ListenTutorialState(EvtTutorialState evt)
		{
			if (evt != null && evt.state == EvtTutorialState.State.Start && base.step == 0 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep1);
			}
			return false;
		}

		private bool ListenStageCreated(EvtStageCreated evt)
		{
			WaitShowTutorialStep(0.09f, SetupController);
			return false;
		}

		private void SetupController()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EvadeBtnVisible, false));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AttackBtnVisible, false));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.JoystickVisible, false));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PauseBtnVisible, false));
		}

		private bool ListenFieldEnter(EvtFieldEnter evt)
		{
			return false;
		}

		private bool ListenMonsterCreated(EvtMonsterCreated evt)
		{
			if (base.step == 4 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep5);
			}
			else if (base.step == 9 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep9);
			}
			else if (base.step == 15 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				_lastMonsterBorn = true;
			}
			return false;
		}

		private bool ListenMonsterKilled(EvtKilled evt)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			if (actor != null && base.step == 7 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep8);
			}
			return false;
		}

		private bool ListenMonsterAttackStart(EvtAttackStart evt)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			if (actor != null && base.step == 10 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				_witchTimeActor = actor;
				_witchTimeAttacking = true;
			}
			if (actor != null && base.step == 15 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone() && _lastMonsterBorn)
			{
				ActiveCurrentStep();
				_witchTimeActor = actor;
				_witchTimeAttacking = true;
			}
			return false;
		}

		public override void Core()
		{
			if (active && _witchTimeAttacking)
			{
				if (_witchTimeActor != null && base.step == 10 && GetCurrentStepState() == StepState.Active && !IsAllStepDone() && _witchTimeActor.entity.GetCurrentNormalizedTime() > GetDelayTime(base.step))
				{
					DoneCurrentStep();
					_witchTimeAttacking = false;
					WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep10);
				}
				if (_witchTimeActor != null && base.step == 15 && GetCurrentStepState() == StepState.Active && !IsAllStepDone() && _witchTimeActor.entity.GetCurrentNormalizedTime() > GetDelayTime(base.step))
				{
					DoneCurrentStep();
					_witchTimeAttacking = false;
					WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep15);
				}
				if (IsAllStepDone())
				{
					Finish();
				}
			}
		}

		private void ShowTutorialStep1()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
			PauseGame();
		}

		public void OnTutorialStep1Done()
		{
			MoveToNextStep();
			if (base.step == 1 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep2);
			}
		}

		private void ShowTutorialStep2()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
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
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.JoystickVisible, true));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
		}

		public void OnTutorialStep3Done()
		{
			MoveToNextStep();
			if (base.step == 3 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep4);
			}
		}

		private void ShowTutorialStep4()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
		}

		public void OnTutorialStpe4Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void ShowTutorialStep5()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
			PauseGame();
		}

		public void OnTutorialStep5Done()
		{
			MoveToNextStep();
			if (base.step == 5 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep6);
			}
		}

		private void ShowTutorialStep6()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
		}

		public void OnTutorialStep6Done()
		{
			MoveToNextStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AttackBtnVisible, true));
			if (base.step == 6 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep7);
			}
		}

		private void ShowTutorialStep7()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
		}

		public void OnTutorialStpe7Done()
		{
			MoveToNextStep();
			ResumeGame();
			if (base.step == 7 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep8);
			}
		}

		private void ShowTutorialStep8()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
			PauseGame();
		}

		public void OnTutoriaStep8Done()
		{
			MoveToNextStep();
			if (base.step == 8 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep8);
			}
		}

		private void ShowTutorialStep8_1()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
		}

		public void OnTutorialStep8_1Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void ShowTutorialStep9()
		{
			ActiveCurrentStep();
			MoveToNextStep();
		}

		public void OnTutoriaStep9Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void ShowTutorialStep10()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EvadeBtnVisible, true));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
			PauseGame();
		}

		public void OnTutoriaStep10Done()
		{
			SetControllerEnable(true);
			MoveToNextStep();
			if (base.step == 11 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep11);
			}
		}

		private void ShowTutorialStep11()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
		}

		public void OnTutoriaStep11Done()
		{
			MoveToNextStep();
			ResumeGame();
			if (base.step == 12 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep12);
			}
		}

		private void ShowTutorialStep12()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
			PauseGame();
		}

		public void OnTutoriaStep12Done()
		{
			MoveToNextStep();
			if (base.step == 13 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep13);
			}
		}

		public void ShowTutorialStep13()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
		}

		public void OnTutorialStep13Done()
		{
			MoveToNextStep();
			if (base.step == 14 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep14);
			}
		}

		private void ShowTutorialStep14()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
		}

		public void OnTutorialStep14Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void ShowTutorialStep15()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
			PauseGame();
		}

		public void OnTutorialStep15Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		public void ShowTutorialStep16()
		{
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
			PauseGame();
		}

		public void OnTutorialStep16Done()
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
