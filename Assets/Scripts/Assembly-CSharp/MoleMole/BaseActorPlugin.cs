namespace MoleMole
{
	public class BaseActorPlugin
	{
		public virtual void OnAdded()
		{
		}

		public virtual void OnRemoved()
		{
		}

		public virtual bool OnEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual bool OnPostEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual bool OnResolvedEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual bool ListenEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual void Core()
		{
		}
	}
}
