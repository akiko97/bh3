using System;

namespace MoleMole
{
	public class BaseEvent
	{
		public uint targetID;

		public bool requireHandle;

		public bool requireResolve;

		[NonSerialized]
		public BaseEvent parent;

		public EventRemoteState remoteState;

		public int fromPeerID;

		public bool resolved { get; private set; }

		protected BaseEvent(uint targetID)
		{
			this.targetID = targetID;
		}

		protected BaseEvent(uint targetID, bool requireHandle, bool requireResolve)
		{
			this.targetID = targetID;
			this.requireHandle = requireHandle;
			this.requireResolve = requireResolve;
		}

		public BaseEvent()
		{
		}

		public virtual void Resolve()
		{
			resolved = true;
		}

		public override string ToString()
		{
			return (parent != null) ? string.Format("{0} <- ({1})", base.ToString(), parent.ToString()) : base.ToString();
		}

		protected string GetDebugName(uint runtimeID)
		{
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(runtimeID);
			if (actor != null)
			{
				return string.Format("<{0}({1:x})>", Truncate(actor.gameObject.name), actor.runtimeID);
			}
			return "<unknown!>";
		}

		protected string GetDebugName(object obj)
		{
			if (obj == null)
			{
				return "<null>";
			}
			return obj.ToString();
		}

		private string Truncate(string str)
		{
			return (str.Length <= 10) ? str : str.Substring(0, 10);
		}
	}
}
