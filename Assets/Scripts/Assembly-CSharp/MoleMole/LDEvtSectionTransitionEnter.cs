namespace MoleMole
{
	public class LDEvtSectionTransitionEnter : BaseLDEvent
	{
		public LDEvtSectionTransitionEnter()
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel();
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EnterTransition));
			Singleton<LevelDesignManager>.Instance.MuteInput();
		}

		public override void Core()
		{
			if (!Singleton<MainUIManager>.Instance.GetInLevelUICanvas().IsStageTransitPanelFading() && Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelTransiting)
			{
				Done();
			}
		}
	}
}
