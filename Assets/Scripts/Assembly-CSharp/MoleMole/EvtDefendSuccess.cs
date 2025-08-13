namespace MoleMole
{
	public class EvtDefendSuccess : BaseEvent, IEvtWithOtherID, IEvtWithAnimEventID
	{
		public readonly uint attackerID;

		public readonly string skillID;

		public EvtDefendSuccess(uint targetID, uint attackerID, string skillID)
			: base(targetID)
		{
			this.attackerID = attackerID;
			this.skillID = skillID;
		}

		public uint GetOtherID()
		{
			return attackerID;
		}

		public string GetAnimEventID()
		{
			return skillID;
		}
	}
}
