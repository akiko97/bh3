namespace MoleMole
{
	public class LDWaitTransitionEnd : BaseLDEvent
	{
		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtLevelState)
			{
				EvtLevelState evtLevelState = evt as EvtLevelState;
				if (evtLevelState.state == EvtLevelState.State.ExitTransition)
				{
					Done();
				}
			}
		}
	}
}
