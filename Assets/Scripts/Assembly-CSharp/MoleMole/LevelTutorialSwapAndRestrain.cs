using System;
using System.Collections.Generic;
using FullInspector;
using MoleMole.Config;

namespace MoleMole
{
	public class LevelTutorialSwapAndRestrain : BaseLevelTutorial
	{
		[ShowInInspector]
		private bool _finished;

		public uint targetSwapAvatarId;

		public uint sourceSwapAvatarId;

		[ShowInInspector]
		private int _killAmount;

		[ShowInInspector]
		private int _monsterCreatedAmount;

		public bool isFirstDead;

		private bool _isTutorialAvailable;

		public LevelTutorialSwapAndRestrain(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			isFirstDead = false;
			targetSwapAvatarId = 0u;
			_killAmount = 0;
			_monsterCreatedAmount = 0;
			_isTutorialAvailable = false;
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtTutorialState>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapInEnd>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtTutorialState>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarSwapInEnd>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtTutorialState)
			{
				return ListenTutorialState((EvtTutorialState)evt);
			}
			if (evt is EvtAvatarSwapInEnd)
			{
				return ListenSwapInEnd((EvtAvatarSwapInEnd)evt);
			}
			if (evt is EvtAvatarCreated)
			{
				return ListenAvatarCreated((EvtAvatarCreated)evt);
			}
			if (evt is EvtKilled)
			{
				return ListenKilled((EvtKilled)evt);
			}
			if (evt is EvtMonsterCreated)
			{
				return ListenMonsterCreated((EvtMonsterCreated)evt);
			}
			return false;
		}

		private bool ListenMonsterCreated(EvtMonsterCreated evt)
		{
			_monsterCreatedAmount++;
			return false;
		}

		private bool ListenKilled(EvtKilled evt)
		{
			bool flag = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 4;
			bool flag2 = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3;
			if (flag && flag2)
			{
				return false;
			}
			if (flag)
			{
				_killAmount++;
				if (_killAmount == 4 && base.step == 13 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
				{
					ActiveCurrentStep();
					WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep13);
				}
			}
			if (flag2)
			{
				if (base.step == 2 && !IsAllStepDone())
				{
					isFirstDead = true;
					ActiveCurrentStep();
					WaitShowTutorialStep(0f, ShowTutorialStep2);
				}
				if (base.step == 13 && !IsAllStepDone())
				{
					Finish();
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, true));
				}
			}
			return false;
		}

		private bool ListenAvatarCreated(EvtAvatarCreated evt)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.onAttackTargetChanged = (Action<BaseMonoEntity>)Delegate.Combine(localAvatar.onAttackTargetChanged, new Action<BaseMonoEntity>(OnUpdateAttackTarget));
			return false;
		}

		private void OnUpdateAttackTarget(BaseMonoEntity entity)
		{
			if (!(entity == null) && _isTutorialAvailable)
			{
				MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(entity.GetRuntimeID());
				BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				AvatarActor actor2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(localAvatar.GetRuntimeID());
				EntityNature nature = (EntityNature)actor.metaConfig.nature;
				EntityNature attribute = (EntityNature)actor2.avatarDataItem.Attribute;
				float natureDamageBonusRatio = DamageModelLogic.GetNatureDamageBonusRatio(nature, attribute, actor);
				if (actor != null && actor2 != null && base.step == 0 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone() && natureDamageBonusRatio > 1f && base.step == 0 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
				{
					ActiveCurrentStep();
					WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep0);
				}
				if (actor != null && actor2 != null && _killAmount >= 2 && _monsterCreatedAmount > 2 && natureDamageBonusRatio > 1f && base.step == 2 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
				{
					ActiveCurrentStep();
					WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep2);
				}
			}
		}

		private void SetupAvatarId()
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			foreach (BaseMonoAvatar allPlayerAvatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(allPlayerAvatar.GetRuntimeID());
				if (actor != null && !Singleton<AvatarManager>.Instance.IsLocalAvatar(allPlayerAvatar.GetRuntimeID()))
				{
					targetSwapAvatarId = actor.runtimeID;
				}
			}
			sourceSwapAvatarId = localAvatar.GetRuntimeID();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, false));
		}

		private bool ListenTutorialState(EvtTutorialState evt)
		{
			if (evt != null && evt.state == EvtTutorialState.State.Start)
			{
				List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
				if (allPlayerAvatars.Count == 2 && allPlayerAvatars[0].AvatarTypeName == "Kiana_C2_PT" && allPlayerAvatars[1].AvatarTypeName == "Mei_C2_CK")
				{
					_isTutorialAvailable = true;
				}
				if (_isTutorialAvailable)
				{
					SetupAvatarId();
				}
				else
				{
					Finish();
				}
			}
			return false;
		}

		private bool ListenSwapInEnd(EvtAvatarSwapInEnd evt)
		{
			targetSwapAvatarId = sourceSwapAvatarId;
			if (IsInStep(10) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep10);
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

		private void ShowTutorialStep0()
		{
			PauseGame();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep0Done()
		{
			MoveToNextStep();
			if (IsInStep(1) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep1);
			}
		}

		private void ShowTutorialStep1()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep1Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void ShowTutorialStep2()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
			PauseGame();
		}

		public void OnTutorialStep2Done()
		{
			MoveToNextStep();
			if (IsInStep(3) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep3);
			}
		}

		private void ShowTutorialStep3()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep3Done()
		{
			MoveToNextStep();
			if (IsInStep(4) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep4);
			}
		}

		private void ShowTutorialStep4()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep4Done()
		{
			MoveToNextStep();
			if (IsInStep(5) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep5);
			}
		}

		private void ShowTutorialStep5()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep5Done()
		{
			MoveToNextStep();
			if (IsInStep(6) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep6);
			}
		}

		private void ShowTutorialStep6()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, true));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep6Done()
		{
			MoveToNextStep();
			if (IsInStep(7) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep7);
			}
		}

		private void ShowTutorialStep7()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep7Done()
		{
			MoveToNextStep();
			if (IsInStep(8) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep8);
			}
		}

		private void ShowTutorialStep8()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep8Done()
		{
			MoveToNextStep();
			if (IsInStep(9) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep9);
			}
		}

		private void ShowTutorialStep9()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep9Done()
		{
			ResumeGame();
			MoveToNextStep();
		}

		private void ShowTutorialStep10()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
			WaitShowTutorialStep(0.5f, delegate
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, false));
			});
			PauseGame();
		}

		public void OnTutorialStep10Done()
		{
			MoveToNextStep();
			if (IsInStep(11) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep11);
			}
		}

		private void ShowTutorialStep11()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep11Done()
		{
			MoveToNextStep();
			if (IsInStep(12) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep12);
			}
		}

		private void ShowTutorialStep12()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
		}

		public void OnTutorialStep12Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void ShowTutorialStep13()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, true));
			PauseGame();
		}

		public void OnTutorialStep13Done()
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
