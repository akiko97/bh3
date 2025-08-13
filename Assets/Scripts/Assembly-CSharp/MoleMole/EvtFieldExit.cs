namespace MoleMole
{
	public class EvtFieldExit : BaseEvent, IEvtWithOtherID
	{
		public readonly uint otherID;

		public EvtFieldExit(uint targetID, uint otherID)
			: base(targetID)
		{
			this.otherID = otherID;
		}

		public override string ToString()
		{
			return string.Format("{0} exiting field {1}", GetDebugName(otherID), GetDebugName(targetID));
		}

		public uint GetOtherID()
		{
			return otherID;
		}
	}
}
