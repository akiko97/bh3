namespace MoleMole
{
	public class LDEvtMonsterHPRatio : BaseLDEvent
	{
		private MonsterActor _monsterActor;

		private float _ratio;

		public LDEvtMonsterHPRatio(double runtimeID, double ratio)
		{
			_monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>((uint)runtimeID);
			_ratio = (float)ratio;
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit && _monsterActor != null && _monsterActor.runtimeID == evt.targetID && (float)_monsterActor.HP / (float)_monsterActor.maxHP < _ratio)
			{
				Done();
			}
		}
	}
}
