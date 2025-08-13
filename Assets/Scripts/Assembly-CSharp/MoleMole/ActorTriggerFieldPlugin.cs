using System.Collections.Generic;
using FullInspector;

namespace MoleMole
{
	public class ActorTriggerFieldPlugin : BaseActorPlugin
	{
		[ShowInInspector]
		public List<uint> insideIDs = new List<uint>();

		protected BasePluggedActor _actor;

		public ActorTriggerFieldPlugin(BasePluggedActor owner)
		{
			_actor = owner;
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
			if (evt is EvtFieldClear)
			{
				return OnFieldClear((EvtFieldClear)evt);
			}
			return false;
		}

		private bool OnFieldEnter(EvtFieldEnter evt)
		{
			if (!insideIDs.Contains(evt.otherID))
			{
				insideIDs.Add(evt.otherID);
			}
			return true;
		}

		private bool OnFieldExit(EvtFieldExit evt)
		{
			if (insideIDs.Contains(evt.otherID))
			{
				insideIDs.Remove(evt.otherID);
			}
			return true;
		}

		private bool OnFieldClear(EvtFieldClear evt)
		{
			insideIDs.Clear();
			return true;
		}
	}
}
