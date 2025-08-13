namespace MoleMole
{
	public class EvtShieldBroken : BaseEvent
	{
		public EvtShieldBroken(uint targetID)
			: base(targetID)
		{
		}

		public override string ToString()
		{
			return string.Format("{0} shield broken", GetDebugName(targetID));
		}
	}
}
