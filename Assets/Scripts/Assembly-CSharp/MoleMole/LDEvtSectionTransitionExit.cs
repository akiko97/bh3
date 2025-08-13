namespace MoleMole
{
	public class LDEvtSectionTransitionExit : BaseLDEvent
	{
		public LDEvtSectionTransitionExit(string sectionLevelAnim)
		{
			if (!string.IsNullOrEmpty(sectionLevelAnim))
			{
				Singleton<LevelDesignManager>.Instance.PlayCameraAnimationOnEnv(sectionLevelAnim, false, false, true, CameraAnimationCullingType.CullAvatars);
			}
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel();
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition));
		}

		public override void Core()
		{
			if (!Singleton<MainUIManager>.Instance.GetInLevelUICanvas().IsStageTransitPanelFading() && Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition));
				Singleton<LevelDesignManager>.Instance.RecoveryInput();
				Done();
			}
		}
	}
}
