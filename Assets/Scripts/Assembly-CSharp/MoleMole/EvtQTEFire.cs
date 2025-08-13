namespace MoleMole
{
	public class EvtQTEFire : BaseEvent
	{
		public string QTEName;

		public EvtQTEFire(uint targetID, string qteName)
			: base(targetID)
		{
			QTEName = qteName;
		}

		public override string ToString()
		{
			return string.Format("{0} qte fired {1}", GetDebugName(targetID), QTEName);
		}
	}
}
