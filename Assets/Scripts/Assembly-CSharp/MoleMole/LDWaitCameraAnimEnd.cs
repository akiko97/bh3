namespace MoleMole
{
	public class LDWaitCameraAnimEnd : BaseLDEvent
	{
		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtCamearaAnimState)
			{
				EvtCamearaAnimState evtCamearaAnimState = evt as EvtCamearaAnimState;
				if (evtCamearaAnimState != null && evtCamearaAnimState.CameraAnimState == EvtCamearaAnimState.State.Finish)
				{
					Done();
				}
			}
		}
	}
}
