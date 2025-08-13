using System;
using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class LevelTutorialMonsterRobotDodge : BaseLevelTutorial
	{
		[ShowInInspector]
		private bool _finished;

		private string _aniamtorDodgeName;

		public LevelTutorialMonsterRobotDodge(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_aniamtorDodgeName = "SwordReflect";
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override void OnAdded()
		{
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterRobotDodge, this));
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Remove(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(OnLocalAvatarAnimatorStateChnage));
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtAvatarCreated)
			{
				return ListenAvatarCreated((EvtAvatarCreated)evt);
			}
			if (evt is EvtAvatarSwapInEnd)
			{
				return ListenAvatarSwapIn((EvtAvatarSwapInEnd)evt);
			}
			if (evt is EvtAvatarSwapOutStart)
			{
				return ListenAvatarSwapOut((EvtAvatarSwapOutStart)evt);
			}
			return false;
		}

		private bool ListenAvatarCreated(EvtAvatarCreated evt)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Combine(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(OnLocalAvatarAnimatorStateChnage));
			return false;
		}

		private bool ListenAvatarSwapOut(EvtAvatarSwapOutStart evt)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID))
			{
				BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Remove(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(OnLocalAvatarAnimatorStateChnage));
			}
			return false;
		}

		private bool ListenAvatarSwapIn(EvtAvatarSwapInEnd evt)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID))
			{
				BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Combine(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(OnLocalAvatarAnimatorStateChnage));
			}
			return false;
		}

		private void OnLocalAvatarAnimatorStateChnage(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			if (toState.IsName(_aniamtorDodgeName) && base.step == 0 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep1);
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
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterRobotDodge, this));
			PauseGame();
		}

		public void OnTutoriaStep1Done()
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
