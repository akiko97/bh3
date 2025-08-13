namespace MoleMole
{
	public class LDEvtEnterField : BaseLDEvent
	{
		private TriggerFieldActor _fieldActor;

		private uint _otherID;

		public LDEvtEnterField(double runtimeID, double otherID)
		{
			_fieldActor = Singleton<EventManager>.Instance.GetActor<TriggerFieldActor>((uint)runtimeID);
			_otherID = (uint)otherID;
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtFieldEnter && _fieldActor != null && _fieldActor.runtimeID == evt.targetID)
			{
				EvtFieldEnter evtFieldEnter = (EvtFieldEnter)evt;
				if (evtFieldEnter.otherID == _otherID)
				{
					Done();
				}
			}
		}
	}
}
