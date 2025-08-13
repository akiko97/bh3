using LuaInterface;

namespace MoleMole
{
	public class LDEvtWaitLocalAvatarEnterFieldTable : BaseLDEvent
	{
		private LuaTable _fieldIDTable;

		public LDEvtWaitLocalAvatarEnterFieldTable(LuaTable fieldIDTable)
		{
			_fieldIDTable = fieldIDTable;
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (!(evt is EvtFieldEnter))
			{
				return;
			}
			EvtFieldEnter evtFieldEnter = (EvtFieldEnter)evt;
			if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(evtFieldEnter.otherID))
			{
				return;
			}
			foreach (object value in _fieldIDTable.Values)
			{
				if ((double)evt.targetID == (double)value)
				{
					Done();
				}
			}
		}

		public override void Dispose()
		{
			_fieldIDTable.Dispose();
		}
	}
}
