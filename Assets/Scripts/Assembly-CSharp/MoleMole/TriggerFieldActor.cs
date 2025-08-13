using System.Collections.Generic;
using FullInspector;

namespace MoleMole
{
	public class TriggerFieldActor : BaseActor
	{
		[ShowInInspector]
		public List<uint> _insideRuntimes = new List<uint>();

		public override void Init(BaseMonoEntity entity)
		{
			runtimeID = entity.GetRuntimeID();
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtFieldEnter)
			{
				return OnFieldEnter((EvtFieldEnter)evt);
			}
			if (evt is EvtFieldExit)
			{
				return OnFieldExit((EvtFieldExit)evt);
			}
			return false;
		}

		public virtual bool OnFieldEnter(EvtFieldEnter evt)
		{
			_insideRuntimes.Add(evt.otherID);
			return true;
		}

		public virtual bool OnFieldExit(EvtFieldExit evt)
		{
			_insideRuntimes.Remove(evt.otherID);
			return true;
		}

		public virtual void Kill()
		{
			Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID));
		}
	}
}
