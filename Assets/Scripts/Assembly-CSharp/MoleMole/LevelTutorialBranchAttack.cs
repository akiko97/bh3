using FullInspector;

namespace MoleMole
{
	public class LevelTutorialBranchAttack : BaseLevelTutorial
	{
		public enum TutorialBranchAttackMode
		{
			Start = 0,
			ReadyForTiming = 1,
			Attacking = 2,
			AfterAttack = 3,
			Done = 4
		}

		[ShowInInspector]
		private bool _finished;

		private float _delayTime;

		private TutorialBranchAttackMode _mode;

		private BaseMonoAvatar _avatar;

		private bool _isBranchAttackDone;

		private bool _isInBranchAttack;

		public LevelTutorialBranchAttack(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_isBranchAttackDone = false;
			_isInBranchAttack = false;
			_delayTime = GetDelayTime(base.step);
			_mode = TutorialBranchAttackMode.Start;
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtTutorialState>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtTutorialState>(_helper.levelActor.runtimeID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtAttackStart)
			{
				return ListenAttackStart((EvtAttackStart)evt);
			}
			if (evt is EvtTutorialState)
			{
				return ListenTutorialState((EvtTutorialState)evt);
			}
			return false;
		}

		private bool ListenTutorialState(EvtTutorialState evt)
		{
			if (evt != null && evt.state == EvtTutorialState.State.Start)
			{
				_avatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				if (_avatar == null)
				{
					return false;
				}
				WaitShowTutorialStep(0f, StartCheckStageReady);
			}
			return false;
		}

		private void StartCheckStageReady()
		{
			bool locomotionBool = _avatar.GetLocomotionBool("AbilityUnlockBranchAttack");
			bool flag = _avatar.AvatarTypeName == "Kiana_C2_PT";
			if (locomotionBool && flag && IsInStep(0) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowStep1Tutorial);
			}
		}

		private bool ListenAttackStart(EvtAttackStart evt)
		{
			if (_mode != TutorialBranchAttackMode.Start)
			{
				return false;
			}
			_avatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (_avatar == null)
			{
				return false;
			}
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID))
			{
				bool flag = _avatar.CurrentSkillID == "ATK02_New";
				bool locomotionBool = _avatar.GetLocomotionBool("AbilityUnlockBranchAttack");
				if (flag && locomotionBool && IsInStep(4))
				{
					_delayTime = GetDelayTime(base.step);
					EnterReadyForBranchAttack();
				}
			}
			return false;
		}

		private void EnterReadyForBranchAttack()
		{
			_mode = TutorialBranchAttackMode.ReadyForTiming;
			ActiveCurrentStep();
			SetControllerEnable(false);
		}

		public override void Core()
		{
			if (!active)
			{
				return;
			}
			if (_mode == TutorialBranchAttackMode.ReadyForTiming)
			{
				float currentNormalizedTime = _avatar.GetCurrentNormalizedTime();
				if (currentNormalizedTime > _delayTime)
				{
					_mode = TutorialBranchAttackMode.Attacking;
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EvadeBtnVisible, false));
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.UltraBtnVisible, false));
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, false));
					ShowStep5Tutorial();
				}
			}
			else if (_mode == TutorialBranchAttackMode.Done)
			{
				Finish();
			}
			if (_isInBranchAttack && IsInStep(4) && !IsAllStepDone() && _avatar.CurrentSkillID == "ATK04")
			{
				_isBranchAttackDone = true;
				_isInBranchAttack = false;
			}
		}

		private void ShowStep1Tutorial()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
			PauseGame();
		}

		public void OnStep1Done()
		{
			MoveToNextStep();
			if (IsInStep(1) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowStep2Tutorial);
			}
		}

		private void ShowStep2Tutorial()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
		}

		public void OnStep2Done()
		{
			MoveToNextStep();
			if (IsInStep(2) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowStep3Tutorial);
			}
		}

		private void ShowStep3Tutorial()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
		}

		public void OnStep3Done()
		{
			MoveToNextStep();
			if (IsInStep(3) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowStep2Tutorial);
			}
		}

		private void ShowStep4Tutorial()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
		}

		public void OnStep4Done()
		{
			ResumeGame();
			MoveToNextStep();
		}

		private void ShowStep5Tutorial()
		{
			SetControllerEnable(true);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
			_isInBranchAttack = true;
			PauseGame();
		}

		public void OnStep5PointerDown()
		{
			ResumeGame();
		}

		public bool OnStep5PoointerUp()
		{
			if (!_isBranchAttackDone)
			{
				PauseGame();
			}
			else
			{
				_mode = TutorialBranchAttackMode.AfterAttack;
				MoveToNextStep();
				if (base.step == 5 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
				{
					ActiveCurrentStep();
					WaitShowTutorialStep(GetDelayTime(base.step), ShowStep6Tutorial);
				}
			}
			return _isBranchAttackDone;
		}

		public void OnStep5Done()
		{
			_mode = TutorialBranchAttackMode.AfterAttack;
			MoveToNextStep();
			ResumeGame();
			if (base.step == 5 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowStep6Tutorial);
			}
		}

		private void ShowStep6Tutorial()
		{
			PauseGame();
			ActiveCurrentStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
		}

		public void OnStep6Done()
		{
			MoveToNextStep();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EvadeBtnVisible, true));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.UltraBtnVisible, true));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, true));
			ResumeGame();
			_mode = TutorialBranchAttackMode.Done;
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
		}
	}
}
