using System;
using UnityEngine;

namespace MoleMole
{
	public class LDEvtFirstSection : BaseLDEvent
	{
		private BaseMonoAvatar _localAvatar;

		public LDEvtFirstSection(string sectionLevelAnim)
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (!levelActor.HasPlugin<DevLevelActorPlugin>())
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DestroyLoadingScene));
				Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel();
			}
			if (!string.IsNullOrEmpty(sectionLevelAnim))
			{
				Singleton<LevelDesignManager>.Instance.PlayCameraAnimationOnEnv(sectionLevelAnim, false, false, true, CameraAnimationCullingType.CullAvatars);
			}
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Combine(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(WaitAppearAnimCallback));
			_localAvatar = localAvatar;
		}

		public override void Core()
		{
			if (_localAvatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AllowTriggerInput) && Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning)
			{
				BaseMonoAvatar localAvatar = _localAvatar;
				localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Remove(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(WaitAppearAnimCallback));
				Done();
			}
		}

		private void WaitAppearAnimCallback(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			if (fromState.tagHash == AvatarData.AVATAR_APPEAR_TAG)
			{
				BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Remove(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(WaitAppearAnimCallback));
				Done();
			}
		}
	}
}
