using System.Collections.Generic;
using LuaInterface;

namespace MoleMole
{
	public class LDEvtWaitAny : BaseLDEvent
	{
		private List<BaseLDEvent> _LDEventList;

		public LDEvtWaitAny(LuaTable LDEventTable)
		{
			_LDEventList = new List<BaseLDEvent>();
			foreach (object value in LDEventTable.Values)
			{
				BaseLDEvent item = Singleton<LevelDesignManager>.Instance.CreateLDEventFromTable((LuaTable)value);
				_LDEventList.Add(item);
			}
		}

		public override void Core()
		{
			bool flag = false;
			foreach (BaseLDEvent lDEvent in _LDEventList)
			{
				if (lDEvent.isDone)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			foreach (BaseLDEvent lDEvent2 in _LDEventList)
			{
				lDEvent2.Done();
			}
			Done();
		}
	}
}
