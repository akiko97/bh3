using System.Collections.Generic;
using LuaInterface;

namespace MoleMole
{
	public class LDEvtOnMultiPropObjectDestroyed : BaseLDEvent
	{
		private List<PropObjectActor> _propObjectActorList = new List<PropObjectActor>();

		public LDEvtOnMultiPropObjectDestroyed(LuaTable propIDTable)
		{
			foreach (object value in propIDTable.Values)
			{
				_propObjectActorList.Add(Singleton<EventManager>.Instance.GetActor<PropObjectActor>((uint)(double)value));
			}
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (_propObjectActorList.Count <= 0)
			{
				return;
			}
			if (evt is EvtKilled)
			{
				for (int i = 0; i < _propObjectActorList.Count; i++)
				{
					if (_propObjectActorList[i].runtimeID == evt.targetID)
					{
						_propObjectActorList.RemoveAt(i);
					}
				}
			}
			if (_propObjectActorList.Count == 0)
			{
				Done();
			}
		}
	}
}
