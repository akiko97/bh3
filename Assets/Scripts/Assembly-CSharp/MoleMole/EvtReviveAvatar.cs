using UnityEngine;

namespace MoleMole
{
	public class EvtReviveAvatar : BaseEvent
	{
		public readonly uint avatarID;

		public bool isRevivePosAssigned;

		public Vector3 revivePosition;

		public EvtReviveAvatar(uint avatarID, bool isRevivePosAssigned, Vector3 revivePositon)
			: base(avatarID)
		{
			this.avatarID = avatarID;
			this.isRevivePosAssigned = isRevivePosAssigned;
			if (isRevivePosAssigned)
			{
				revivePosition = revivePositon;
			}
			else
			{
				revivePosition = new Vector3(0f, 0f, 0f);
			}
		}

		public override string ToString()
		{
			return string.Format("revive avatar: avatarId:{0} isRevivePosAssigned:{1} revivePosition:{2}", avatarID, isRevivePosAssigned, revivePosition);
		}
	}
}
