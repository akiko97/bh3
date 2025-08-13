namespace MoleMole
{
	public class LDWaitSpecificMonsterKilled : BaseLDEvent
	{
		private MonsterActor _monsterActor;

		public LDWaitSpecificMonsterKilled(double runtimeID)
		{
			_monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>((uint)runtimeID);
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtKilled && evt.targetID == _monsterActor.runtimeID)
			{
				Done();
			}
		}
	}
}
