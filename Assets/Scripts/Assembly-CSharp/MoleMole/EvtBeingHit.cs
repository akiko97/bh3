using MoleMole.Config;

namespace MoleMole
{
	public class EvtBeingHit : BaseEvent, IEvtWithOtherID, IEvtWithAttackResult, IEvtWithAnimEventID, IEvtWithHitCollision, IEvtWithRemoteID
	{
		public uint sourceID;

		public string animEventID;

		public AttackData attackData;

		public BeHitEffect beHitEffect;

		public float resolvedDamage;

		public EvtBeingHit()
		{
			requireResolve = true;
		}

		public EvtBeingHit(uint toID, uint fromID, string animEventID, AttackData attackData)
			: base(toID, false, true)
		{
			this.animEventID = animEventID;
			sourceID = fromID;
			this.attackData = attackData;
		}

		public override string ToString()
		{
			return string.Format("{0} being hit by {1} on skill {2}, caused damage {3}", GetDebugName(targetID), GetDebugName(sourceID), animEventID, attackData.damage);
		}

		public uint GetOtherID()
		{
			return sourceID;
		}

		public string GetAnimEventID()
		{
			return animEventID;
		}

		public AttackResult.HitCollsion GetHitCollision()
		{
			return attackData.hitCollision;
		}

		public AttackResult GetAttackResult()
		{
			return attackData;
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
			return sourceID;
		}
	}
}
