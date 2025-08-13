namespace MoleMole
{
	public class LDWaitVideoEnd : BaseLDEvent
	{
		public override void OnEvent(BaseEvent evt)
		{
			EvtVideoState evtVideoState = evt as EvtVideoState;
			if (evtVideoState != null && evtVideoState.VideoState == EvtVideoState.State.Finish)
			{
				Done();
			}
		}
	}
}
