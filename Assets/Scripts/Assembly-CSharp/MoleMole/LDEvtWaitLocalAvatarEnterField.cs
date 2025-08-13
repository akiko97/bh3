namespace MoleMole
{
	public class LDEvtWaitLocalAvatarEnterField : BaseLDEvent
	{
		private TriggerFieldActor _fieldActor;

		public LDEvtWaitLocalAvatarEnterField(double runtimeID)
		{
			_fieldActor = Singleton<EventManager>.Instance.GetActor<TriggerFieldActor>((uint)runtimeID);
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtFieldEnter && _fieldActor != null && _fieldActor.runtimeID == evt.targetID)
			{
				EvtFieldEnter evtFieldEnter = (EvtFieldEnter)evt;
				if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evtFieldEnter.otherID))
				{
					Done();
				}
			}
		}
	}
}
