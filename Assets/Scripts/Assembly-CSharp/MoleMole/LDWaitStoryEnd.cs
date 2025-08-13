namespace MoleMole
{
	public class LDWaitStoryEnd : BaseLDEvent
	{
		public override void OnEvent(BaseEvent evt)
		{
			EvtStoryState evtStoryState = evt as EvtStoryState;
			if (evtStoryState != null && evtStoryState.StoryState == EvtStoryState.State.Finish)
			{
				Done();
			}
		}
	}
}
