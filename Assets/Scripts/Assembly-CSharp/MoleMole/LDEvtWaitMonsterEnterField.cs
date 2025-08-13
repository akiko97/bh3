namespace MoleMole
{
	public class LDEvtWaitMonsterEnterField : BaseLDEvent
	{
		private MonsterExitFieldActor _fieldActor;

		public LDEvtWaitMonsterEnterField(double runtimeID)
		{
			_fieldActor = Singleton<EventManager>.Instance.GetActor<MonsterExitFieldActor>((uint)runtimeID);
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtFieldEnter && _fieldActor != null && _fieldActor.runtimeID == evt.targetID)
			{
				EvtFieldEnter evtFieldEnter = (EvtFieldEnter)evt;
				if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evtFieldEnter.otherID) == 4)
				{
					Done();
				}
			}
		}
	}
}
