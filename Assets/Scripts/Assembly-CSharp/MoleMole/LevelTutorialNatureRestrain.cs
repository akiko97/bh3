using System;
using FullInspector;
using MoleMole.Config;

namespace MoleMole
{
	public class LevelTutorialNatureRestrain : BaseLevelTutorial
	{
		[ShowInInspector]
		private bool _finished;

		public LevelTutorialNatureRestrain(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
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
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarCreated>(_helper.levelActor.runtimeID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtAvatarCreated)
			{
				return ListenAvatarCreated((EvtAvatarCreated)evt);
			}
			if (evt is EvtAvatarSwapInEnd)
			{
				return ListenAvatarSwapInEnd((EvtAvatarSwapInEnd)evt);
			}
			if (evt is EvtAvatarSwapOutStart)
			{
				return ListenAvatarSwapOutStart((EvtAvatarSwapOutStart)evt);
			}
			return false;
		}

		private bool ListenAvatarCreated(EvtAvatarCreated evt)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.onAttackTargetChanged = (Action<BaseMonoEntity>)Delegate.Combine(localAvatar.onAttackTargetChanged, new Action<BaseMonoEntity>(OnUpdateAttackTarget));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, false));
			return false;
		}

		private bool ListenAvatarSwapInEnd(EvtAvatarSwapInEnd evt)
		{
			BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(evt.targetID);
			if (avatarByRuntimeID != null)
			{
				avatarByRuntimeID.onAttackTargetChanged = (Action<BaseMonoEntity>)Delegate.Combine(avatarByRuntimeID.onAttackTargetChanged, new Action<BaseMonoEntity>(OnUpdateAttackTarget));
			}
			return false;
		}

		private bool ListenAvatarSwapOutStart(EvtAvatarSwapOutStart evt)
		{
			BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(evt.targetID);
			if (avatarByRuntimeID != null)
			{
				avatarByRuntimeID.onAttackTargetChanged = (Action<BaseMonoEntity>)Delegate.Remove(avatarByRuntimeID.onAttackTargetChanged, new Action<BaseMonoEntity>(OnUpdateAttackTarget));
			}
			return false;
		}

		private bool ListenAttackStart(EvtAttackStart evt)
		{
			return false;
		}

		private void OnUpdateAttackTarget(BaseMonoEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(entity.GetRuntimeID());
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			AvatarActor actor2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(localAvatar.GetRuntimeID());
			if (actor != null && actor2 != null && base.step == 0 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				EntityNature nature = (EntityNature)actor.metaConfig.nature;
				EntityNature attribute = (EntityNature)actor2.avatarDataItem.Attribute;
				float natureDamageBonusRatio = DamageModelLogic.GetNatureDamageBonusRatio(nature, attribute, actor);
				if (natureDamageBonusRatio > 1f)
				{
					ActiveCurrentStep();
					WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep1);
				}
			}
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
			PauseGame();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
		}

		public void OnTutorialStep1Done()
		{
			MoveToNextStep();
			if (IsInStep(1) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep2);
			}
		}

		private void ShowTutorialStep2()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
		}

		public void OnTutorialStep2Done()
		{
			MoveToNextStep();
			if (IsInStep(2) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep3);
			}
		}

		private void ShowTutorialStep3()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
		}

		public void OnTutorialStep3Done()
		{
			MoveToNextStep();
			if (IsInStep(3) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep4);
			}
		}

		private void ShowTutorialStep4()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
			ActiveCurrentStep();
		}

		public void OnTutorialStep4Done()
		{
			MoveToNextStep();
			if (IsInStep(4) && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep4);
			}
		}

		private void ShowTutorialStep5()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
			ActiveCurrentStep();
		}

		public void OnTutorialStep5Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void Finish()
		{
			_finished = true;
			OnDecided();
		}

		private void Fail()
		{
			_finished = false;
			OnDecided();
		}
	}
}
