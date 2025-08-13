namespace MoleMole
{
	public class EvtFieldEnter : BaseEvent, IEvtWithOtherID
	{
		public readonly uint otherID;

		public EvtFieldEnter(uint targetID, uint otherID)
			: base(targetID)
		{
			this.otherID = otherID;
		}

		public uint GetOtherID()
		{
			return otherID;
		}

		public override string ToString()
		{
			return string.Format("{0} entering field {1}", GetDebugName(otherID), GetDebugName(targetID));
		}
	}
}
