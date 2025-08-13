using FullInspector;

namespace MoleMole
{
	public class LevelTutorialSwapAttack : BaseLevelTutorial
	{
		[ShowInInspector]
		private bool _finished;

		public uint targetSwapAvatarId;

		public uint sourceSwapAvatarId;

		public LevelTutorialSwapAttack(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			targetSwapAvatarId = 0u;
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(_helper.levelActor.runtimeID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtStageReady)
			{
				return ListenStageReady((EvtStageReady)evt);
			}
			if (evt is EvtAvatarSwapInEnd)
			{
				return ListenSwapInEnd((EvtAvatarSwapInEnd)evt);
			}
			return false;
		}

		private void SetupAvatarId()
		{
			foreach (BaseMonoAvatar allPlayerAvatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(allPlayerAvatar.GetRuntimeID());
				if (actor != null && !Singleton<AvatarManager>.Instance.IsLocalAvatar(allPlayerAvatar.GetRuntimeID()))
				{
					targetSwapAvatarId = actor.runtimeID;
				}
			}
			sourceSwapAvatarId = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
		}

		private bool ListenStageReady(EvtStageReady evt)
		{
			bool flag = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars().Count > 1;
			SetupAvatarId();
			if (flag && base.step == 0 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep1);
			}
			return false;
		}

		private bool ListenSwapInEnd(EvtAvatarSwapInEnd evt)
		{
			if (base.step == 4 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone() && targetSwapAvatarId != 0)
			{
				ActiveCurrentStep();
				targetSwapAvatarId = sourceSwapAvatarId;
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep5);
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
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
			PauseGame();
		}

		private void ShowTutorialStep2()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
			ActiveCurrentStep();
		}

		private void ShowTutorialStep3()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
			ActiveCurrentStep();
		}

		private void ShowTutorialStep4()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
			ActiveCurrentStep();
		}

		public void ShowTutorialStep5()
		{
			MoveToNextStep();
		}

		public void OnTutorialStep1Done()
		{
			MoveToNextStep();
			if (base.step == 1 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep2);
			}
		}

		public void OnTutorialStep5Done()
		{
			ResumeGame();
			MoveToNextStep();
		}

		public void OnTutorialStep2Done()
		{
			MoveToNextStep();
			if (base.step == 2 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep3);
			}
		}

		public void OnTutorialStep3Done()
		{
			MoveToNextStep();
			if (base.step == 3 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep4);
			}
		}

		public void OnTutorialStep4Done()
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
