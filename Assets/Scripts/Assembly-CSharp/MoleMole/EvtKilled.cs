namespace MoleMole
{
	public class EvtKilled : BaseEvent, IEvtWithOtherID
	{
		public uint killerID;

		public string killerAnimEventID;

		public EvtKilled(uint targetID)
			: base(targetID)
		{
			killerID = 562036737u;
			killerAnimEventID = null;
		}

		public EvtKilled(uint targetID, uint killerID, string killerAnimEventID)
			: base(targetID)
		{
			this.killerID = killerID;
			this.killerAnimEventID = killerAnimEventID;
		}

		public override string ToString()
		{
			return string.Format("{0} get killed", GetDebugName(targetID));
		}

		public uint GetOtherID()
		{
			return killerID;
		}
	}
}
