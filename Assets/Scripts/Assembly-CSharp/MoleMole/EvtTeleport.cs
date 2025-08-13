namespace MoleMole
{
	public class EvtTeleport : BaseEvent
	{
		public EvtTeleport(uint targetID)
			: base(targetID)
		{
		}

		public override string ToString()
		{
			return string.Format("{0} teleport", GetDebugName(targetID));
		}
	}
}
