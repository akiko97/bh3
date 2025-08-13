using System.Collections.Generic;

namespace MoleMole
{
	public class NotifyManager
	{
		private List<BaseContext> _contextList;

		private List<BaseModule> _moduleList;

		private NotifyManager()
		{
			_contextList = new List<BaseContext>();
			_moduleList = new List<BaseModule>();
		}

		public void ClearAllContext()
		{
			_contextList.Clear();
		}

		public void RegisterContext(BaseContext context)
		{
			if (!context.config.ignoreNotify && !_contextList.Contains(context))
			{
				_contextList.Add(context);
			}
		}

		public void RemoveContext(BaseContext context)
		{
			if (!context.config.ignoreNotify && _contextList.Contains(context))
			{
				_contextList.Remove(context);
			}
		}

		public bool FireNotify(Notify cmd)
		{
			bool flag = false;
			if (cmd.type == NotifyTypes.NetwrokPacket)
			{
				NetPacketV1 pkt = cmd.body as NetPacketV1;
				flag |= HandlePacketForModules(pkt);
			}
			BaseContext[] array = new BaseContext[_contextList.Count];
			_contextList.CopyTo(array);
			BaseContext[] array2 = array;
			foreach (BaseContext baseContext in array2)
			{
				if (baseContext != null)
				{
					flag |= baseContext.HandleNotify(cmd);
				}
			}
			return false;
		}

		public void RegisterModule(BaseModule listener)
		{
			if (!_moduleList.Contains(listener))
			{
				_moduleList.Add(listener);
			}
		}

		private bool HandlePacketForModules(NetPacketV1 pkt)
		{
			bool flag = false;
			for (int i = 0; i < _moduleList.Count; i++)
			{
				flag |= _moduleList[i].OnPacket(pkt);
			}
			return flag;
		}
	}
}
