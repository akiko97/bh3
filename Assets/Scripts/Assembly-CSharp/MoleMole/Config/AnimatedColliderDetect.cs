namespace MoleMole.Config
{
	public class AnimatedColliderDetect : ConfigEntityAttackPattern
	{
		public string ColliderEntryName;

		public bool Follow;

		public string FollowAttachPoint;

		public bool DestroyOnOwnerBeHitCanceled;

		public bool IgnoreTimeScale;

		public bool FollowOwnerTimeScale;

		public bool dontDestroyWhenEvade;

		public bool brokenEnemyDragged;

		public bool EnableHitWallStop;

		public bool DestroyOnHitWall;

		public string HitWallDestroyEffect;

		public AnimatedColliderDetect()
		{
			patternMethod = ComplexAttackPattern.AnimatedColliderDetectAttack;
		}
	}
}
