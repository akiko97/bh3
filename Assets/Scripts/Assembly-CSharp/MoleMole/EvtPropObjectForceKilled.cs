namespace MoleMole
{
	public class EvtPropObjectForceKilled : BaseEvent
	{
		public EvtPropObjectForceKilled(uint targetID)
			: base(targetID)
		{
		}

		public override string ToString()
		{
			return string.Format("{0} force killed", GetDebugName(targetID));
		}
	}
}
