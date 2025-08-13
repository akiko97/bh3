namespace MoleMole
{
	public class LDEvtOnLoadingSceneDestroyed : BaseLDEvent
	{
		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtLoadingState)
			{
				EvtLoadingState evtLoadingState = evt as EvtLoadingState;
				if (evtLoadingState != null && evtLoadingState.state == EvtLoadingState.State.Destroy)
				{
					Done();
				}
			}
		}
	}
}
