namespace MoleMole.Config
{
	public class ConfigMonsterSkill
	{
		public string[] AnimatorStateNames;

		public string AnimatorEventPattern;

		public float AnimDefenceRatio;

		public float AnimDefenceNormalizedTimeStart;

		public float AnimDefenceNormalizedTimeStop = 1f;

		public float AttackNormalizedTimeStart;

		public float AttackNormalizedTimeStop;

		public bool HighSpeedMovement;

		public bool SteerToTargetOnEnter;

		public float MassRatio = 1f;

		public bool NeedClearEffect;

		public bool Unselectable;
	}
}
