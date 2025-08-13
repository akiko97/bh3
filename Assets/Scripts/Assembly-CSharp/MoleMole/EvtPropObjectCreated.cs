namespace MoleMole
{
	public class EvtPropObjectCreated : BaseEvent
	{
		public uint objectID;

		public EvtPropObjectCreated(uint ownerID, uint objectID)
			: base(ownerID)
		{
			this.objectID = objectID;
		}

		public override string ToString()
		{
			return string.Format("{0} created {1}", GetDebugName(targetID), GetDebugName(objectID));
		}
	}
}
