using System;

namespace MoleMole
{
	[Serializable]
	public class Notify
	{
		public NotifyTypes type;

		public object body;

		public Notify(NotifyTypes type, object body = null)
		{
			this.type = type;
			this.body = body;
		}
	}
}
