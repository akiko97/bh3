namespace MoleMole
{
	public class EvtEvadeSuccess : BaseEvent, IEvtWithOtherID, IEvtWithAttackResult, IEvtWithAnimEventID, IEvtWithRemoteID
	{
		public uint attackerID;

		public string skillID;

		public AttackData attackData;

		public EvtEvadeSuccess()
		{
		}

		public EvtEvadeSuccess(uint targetID, uint attackerID, string skillID, AttackData attackData)
			: base(targetID)
		{
			this.attackerID = attackerID;
			this.skillID = skillID;
			this.attackData = attackData;
		}

		public uint GetOtherID()
		{
			return attackerID;
		}

		public string GetAnimEventID()
		{
			return skillID;
		}

		public AttackResult GetAttackResult()
		{
			return attackData;
		}

		public uint GetChannelID()
		{
			return targetID;
		}

		public uint GetSenderID()
		{
			return attackerID;
		}

		public uint GetRemoteID()
		{
			return targetID;
		}
	}
}
