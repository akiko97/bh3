using UnityEngine;

namespace MoleMole
{
	public abstract class BaseActor
	{
		public uint runtimeID;

		public uint ownerID = 562036737u;

		public GameObject gameObject;

		public abstract void Init(BaseMonoEntity entity);

		public virtual bool IsActive()
		{
			return gameObject != null && gameObject.activeSelf;
		}

		public bool IsEntityExists()
		{
			return gameObject != null;
		}

		public virtual bool OnEvent(BaseEvent evt)
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

		public virtual void OnRemoval()
		{
		}

		protected void MarkImportantEventIsHandled(BaseEvent evt)
		{
			if (Singleton<MPEventManager>.Instance != null)
			{
				Singleton<MPEventManager>.Instance.MarkEventReplicate(evt);
			}
		}
	}
}
