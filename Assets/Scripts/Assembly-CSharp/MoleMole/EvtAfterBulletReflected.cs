namespace MoleMole
{
	public class EvtAfterBulletReflected : BaseEvent, IEvtWithOtherID
	{
		public readonly uint bulletID;

		public readonly uint launcherID;

		public AttackData attackData;

		public EvtAfterBulletReflected(uint targetID, uint bulletID, uint launcherID, AttackData attackData)
			: base(targetID)
		{
			this.bulletID = bulletID;
			this.launcherID = launcherID;
			this.attackData = attackData;
		}

		public override string ToString()
		{
			return string.Format("{0} bullet hits {1}", GetDebugName(bulletID), GetDebugName(targetID));
		}

		public uint GetOtherID()
		{
			return bulletID;
		}
	}
}
