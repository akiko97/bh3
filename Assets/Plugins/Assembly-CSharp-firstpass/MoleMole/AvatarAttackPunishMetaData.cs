namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarAttackPunishMetaData : IHashable
	{
		public readonly int LevelDifference;

		public readonly float DamageReduceRate;

		public readonly float AttackRatioReduce;

		public AvatarAttackPunishMetaData(int LevelDifference, float DamageReduceRate, float AttackRatioReduce)
		{
			this.LevelDifference = LevelDifference;
			this.DamageReduceRate = DamageReduceRate;
			this.AttackRatioReduce = AttackRatioReduce;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(LevelDifference, ref lastHash);
			HashUtils.ContentHashOnto(DamageReduceRate, ref lastHash);
			HashUtils.ContentHashOnto(AttackRatioReduce, ref lastHash);
		}
	}
}
