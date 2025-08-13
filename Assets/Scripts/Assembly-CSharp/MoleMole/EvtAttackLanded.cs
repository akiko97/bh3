namespace MoleMole
{
	public class EvtAttackLanded : BaseEvent, IEvtWithOtherID, IEvtWithAttackResult, IEvtWithAnimEventID, IEvtWithHitCollision, IEvtWithRemoteID
	{
		public uint attackeeID;

		public string animEventID;

		public AttackResult attackResult;

		public EvtAttackLanded()
		{
		}

		public EvtAttackLanded(uint fromID, uint attackeeID, string animEventID, AttackResult attackResult)
			: base(fromID)
		{
			this.attackeeID = attackeeID;
			this.animEventID = animEventID;
			this.attackResult = attackResult;
		}

		public override string ToString()
		{
			return string.Format("{0} attack Landed on {1}, skill {2}", GetDebugName(targetID), GetDebugName(attackeeID), animEventID);
		}

		public uint GetOtherID()
		{
			return attackeeID;
		}

		public string GetAnimEventID()
		{
			return animEventID;
		}

		public AttackResult GetAttackResult()
		{
			return attackResult;
		}

		public AttackResult.HitCollsion GetHitCollision()
		{
			return attackResult.hitCollision;
		}

		public uint GetChannelID()
		{
			return targetID;
		}

		public uint GetRemoteID()
		{
			return targetID;
		}

		public uint GetSenderID()
		{
			return attackeeID;
		}
	}
}
