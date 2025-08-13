namespace MoleMole.Config
{
	public class TargetLockedAnimatedColliderDetect : ConfigEntityAttackPattern
	{
		public string ColliderEntryName;

		public float MaxLockDistance;

		public float ScatteringDistance;

		public bool StopOnFirstContact;

		public bool LockX;

		public bool dontDestroyWhenEvade;

		public bool DestroyOnOwnerBeHitCanceled;

		public bool brokenEnemyDragged;

		public TargetLockedAnimatedColliderDetect()
		{
			patternMethod = ComplexAttackPattern.TargetLockedAnimatedColliderDetectAttack;
		}
	}
}
