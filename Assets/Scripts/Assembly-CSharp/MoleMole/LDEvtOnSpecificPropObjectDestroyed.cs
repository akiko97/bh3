namespace MoleMole
{
	public class LDEvtOnSpecificPropObjectDestroyed : BaseLDEvent
	{
		private PropObjectActor _propObjectActor;

		public LDEvtOnSpecificPropObjectDestroyed(double runtimeID)
		{
			_propObjectActor = Singleton<EventManager>.Instance.GetActor<PropObjectActor>((uint)runtimeID);
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtKilled && evt.targetID == _propObjectActor.runtimeID)
			{
				Done();
			}
		}
	}
}
