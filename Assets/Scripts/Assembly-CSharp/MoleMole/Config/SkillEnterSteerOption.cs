namespace MoleMole.Config
{
	public class SkillEnterSteerOption
	{
		public enum EnterSteerType
		{
			Instant = 0,
			LimitSteerRatioAndNormalizedEnd = 1
		}

		public EnterSteerType SteerType;

		public float MaxSteeringAngle = 180f;

		public float SteerLerpRatio = 3f;

		public float MaxSteerNormalizedTimeStart;

		public float MaxSteerNormalizedTimeEnd = 0.5f;

		public bool MuteSteerWhenNoEnemy;
	}
}
