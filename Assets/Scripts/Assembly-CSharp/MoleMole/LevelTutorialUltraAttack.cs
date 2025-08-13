using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class LevelTutorialUltraAttack : BaseLevelTutorial
	{
		private const float DEFAULT_SP_PERCENT = 0.3f;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private AvatarActor _actor;

		private bool _isHealingSP;

		public LevelTutorialUltraAttack(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
			: base(helper, metaData)
		{
			_actor = null;
			_isHealingSP = false;
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtTutorialState>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtTutorialState>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(_helper.levelActor.runtimeID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtTutorialState)
			{
				return ListenTutorialState((EvtTutorialState)evt);
			}
			if (evt is EvtMonsterCreated)
			{
				return ListenMonsterCreated((EvtMonsterCreated)evt);
			}
			return false;
		}

		private bool ListenTutorialState(EvtTutorialState evt)
		{
			if (evt != null && evt.state == EvtTutorialState.State.Start)
			{
				_actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
				if (_actor == null)
				{
					return false;
				}
				WaitShowTutorialStep(0.5f, StartCheckStageReady);
			}
			return false;
		}

		private void StartCheckStageReady()
		{
			bool flag = !_actor.IsSkillLocked("SKL02");
			if (IsInStep(0) && GetStepState(0) == StepState.Sleep && !IsAllStepDone() && flag)
			{
				if (_actor != null)
				{
					_actor.HealSP((float)_actor.maxSP * 0.3f);
				}
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowStep1Tutorial);
			}
		}

		private bool ListenMonsterCreated(EvtMonsterCreated evt)
		{
			if (IsInStep(1) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowStep2Tutorial);
			}
			return false;
		}

		private void ShowStep1Tutorial()
		{
			MoveToNextStep();
		}

		public void OnStep1Done()
		{
			ResumeGame();
			MoveToNextStep();
		}

		private void ShowStep2Tutorial()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
			PauseGame();
			ActiveCurrentStep();
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
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
			ActiveCurrentStep();
		}

		public void OnStep3Done()
		{
			MoveToNextStep();
			ChargeSPToFull();
		}

		private void ShowStep4Tutorial()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
			PauseGame();
			ActiveCurrentStep();
		}

		public void OnStep4Done()
		{
			ResumeGame();
			MoveToNextStep();
			if (IsInStep(4) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				WaitShowTutorialStep(GetDelayTime(base.step), ShowStep5Tutorial);
			}
		}

		private void ShowStep5Tutorial()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
			ActiveCurrentStep();
			WaitShowTutorialStep(2.5f, OnStep5Done);
		}

		public void OnStep5Done()
		{
			MoveToNextStep();
		}

		private void ChargeSPToFull()
		{
			if (_actor != null)
			{
				_isHealingSP = true;
			}
		}

		public override void Core()
		{
			base.Core();
			if (!active || _actor == null)
			{
				return;
			}
			if (_isHealingSP)
			{
				_actor.HealSP(Time.smoothDeltaTime * 100f);
				if ((float)_actor.SP >= (float)_actor.maxSP)
				{
					_isHealingSP = false;
					if (IsInStep(3) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
					{
						ActiveCurrentStep();
						WaitShowTutorialStep(GetDelayTime(base.step), ShowStep4Tutorial);
					}
				}
			}
			if (IsAllStepDone())
			{
				Finish();
			}
		}

		private void Finish()
		{
			_finished = true;
			Singleton<LevelTutorialModule>.Instance.MarkTutorialIDFinish(tutorialId);
			OnDecided();
		}
	}
}
