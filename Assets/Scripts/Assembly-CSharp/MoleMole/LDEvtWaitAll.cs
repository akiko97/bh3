using System.Collections.Generic;
using LuaInterface;

namespace MoleMole
{
	public class LDEvtWaitAll : BaseLDEvent
	{
		private List<BaseLDEvent> _LDEventList;

		public LDEvtWaitAll(LuaTable LDEventTable)
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
			foreach (BaseLDEvent lDEvent in _LDEventList)
			{
				if (lDEvent.isDone)
				{
					_LDEventList.Remove(lDEvent);
				}
			}
			if (_LDEventList.Count == 0)
			{
				Done();
			}
		}
	}
}
