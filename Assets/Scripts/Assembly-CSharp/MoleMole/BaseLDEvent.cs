using System;

namespace MoleMole
{
	public abstract class BaseLDEvent : IDisposable
	{
		public bool isDone { get; private set; }

		public void Done()
		{
			isDone = true;
		}

		public virtual void Core()
		{
		}

		public virtual void OnEvent(BaseEvent evt)
		{
		}

		public virtual void Dispose()
		{
		}
	}
}
