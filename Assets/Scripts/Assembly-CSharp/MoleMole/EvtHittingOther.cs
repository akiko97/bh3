using UnityEngine;

namespace MoleMole
{
	public class EvtHittingOther : BaseEvent, IEvtWithOtherID, IEvtWithAttackResult, IEvtWithAnimEventID, IEvtWithHitCollision, IEvtWithRemoteID
	{
		public uint toID;

		public string animEventID;

		public AttackResult.HitCollsion hitCollision;

		public AttackData attackData;

		public EvtHittingOther()
		{
			requireResolve = true;
		}

		public EvtHittingOther(uint fromID, uint toID, string animEventID)
			: base(fromID, false, true)
		{
			this.animEventID = animEventID;
			this.toID = toID;
		}

		public EvtHittingOther(uint fromID, uint toID, string animEventID, Vector3 hitPoint, Vector3 hitForward)
			: this(fromID, toID, animEventID)
		{
			hitCollision = new AttackResult.HitCollsion
			{
				hitPoint = hitPoint,
				hitDir = hitForward
			};
		}

		public EvtHittingOther(uint fromID, uint toID, AttackData attackData)
			: base(fromID, false, true)
		{
			this.toID = toID;
			this.attackData = attackData;
			animEventID = null;
		}

		public EvtHittingOther(uint fromID, uint toID, string animEventID, AttackData attackData)
			: base(fromID, false, true)
		{
			this.toID = toID;
			this.animEventID = animEventID;
			this.attackData = attackData;
		}

		public override string ToString()
		{
			return string.Format("{0} hitting {1} by skill {2}, caused damage {3}", GetDebugName(targetID), GetDebugName(toID), GetDebugName(animEventID), (attackData != null) ? attackData.damage.ToString() : "<null>");
		}

		public string GetAnimEventID()
		{
			return animEventID;
		}

		public uint GetOtherID()
		{
			return toID;
		}

		public AttackResult.HitCollsion GetHitCollision()
		{
			return hitCollision;
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
			return toID;
		}

		public uint GetSenderID()
		{
			return targetID;
		}
	}
}
