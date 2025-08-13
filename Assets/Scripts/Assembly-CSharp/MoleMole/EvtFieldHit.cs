namespace MoleMole
{
	public class EvtFieldHit : BaseEvent
	{
		public string animEventID;

		public EvtFieldHit(uint targetID, string animEventID)
			: base(targetID)
		{
			this.animEventID = animEventID;
		}

		public override string ToString()
		{
			return string.Format("{0} field hits target within by {1}", GetDebugName(targetID), animEventID);
		}
	}
}
