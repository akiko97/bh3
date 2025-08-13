namespace MoleMole.Config
{
	public class SelectEnemyByEllipse : AvatarAttackTargetSelect
	{
		public float TargetSelectionEccentricity;

		public SelectEnemyByEllipse()
		{
			selectMethod = AvatarAttackTargetSelectPattern.SelectEnemyByEllipse;
		}
	}
}
