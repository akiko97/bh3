namespace MoleMole
{
	public class EvtFieldClear : BaseEvent
	{
		public EvtFieldClear(uint targetID)
			: base(targetID)
		{
		}

		public override string ToString()
		{
			return string.Format("{0} clear field", GetDebugName(targetID));
		}
	}
}
