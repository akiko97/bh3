using System.Collections.Generic;
using LuaInterface;

namespace MoleMole
{
	public class LDEvtOnMultiMonsterKilled : BaseLDEvent
	{
		private List<MonsterActor> _monsterActorList = new List<MonsterActor>();

		public LDEvtOnMultiMonsterKilled(LuaTable monsterIDTable)
		{
			foreach (object value in monsterIDTable.Values)
			{
				_monsterActorList.Add(Singleton<EventManager>.Instance.GetActor<MonsterActor>((uint)(double)value));
			}
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (_monsterActorList.Count <= 0)
			{
				return;
			}
			if (evt is EvtKilled)
			{
				for (int i = 0; i < _monsterActorList.Count; i++)
				{
					if (_monsterActorList[i].runtimeID == evt.targetID)
					{
						_monsterActorList.RemoveAt(i);
					}
				}
			}
			if (_monsterActorList.Count == 0)
			{
				Done();
			}
		}
	}
}
