namespace MoleMole
{
	public class EvtBulletHit : BaseEvent, IEvtWithOtherID, IEvtWithHitCollision, IEvtWithRemoteID
	{
		public uint otherID;

		public AttackResult.HitCollsion hitCollision;

		public bool hitEnvironment;

		public bool hitGround;

		public bool cannotBeReflected;

		public bool selfExplode;

		public uint ownerID;

		public EvtBulletHit()
		{
		}

		public EvtBulletHit(uint targetID, uint otherID)
			: base(targetID)
		{
			this.otherID = otherID;
		}

		public EvtBulletHit(uint targetID)
			: base(targetID)
		{
			otherID = 562036737u;
			hitEnvironment = true;
		}

		public override string ToString()
		{
			return string.Format("{0} bullet hits {1}(SelfExplode : {2})", GetDebugName(otherID), GetDebugName(targetID), selfExplode.ToString());
		}

		public uint GetOtherID()
		{
			return otherID;
		}

		public AttackResult.HitCollsion GetHitCollision()
		{
			return hitCollision;
		}

		public uint GetChannelID()
		{
			return ownerID;
		}

		public uint GetSenderID()
		{
			return ownerID;
		}

		public uint GetRemoteID()
		{
			return otherID;
		}
	}
}
