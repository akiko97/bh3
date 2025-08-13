namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarDefencePunishMetaData : IHashable
	{
		public readonly int LevelDifference;

		public readonly float DamageIncreaseRate;

		public readonly float AttackRatioIncrease;

		public AvatarDefencePunishMetaData(int LevelDifference, float DamageIncreaseRate, float AttackRatioIncrease)
		{
			this.LevelDifference = LevelDifference;
			this.DamageIncreaseRate = DamageIncreaseRate;
			this.AttackRatioIncrease = AttackRatioIncrease;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(LevelDifference, ref lastHash);
			HashUtils.ContentHashOnto(DamageIncreaseRate, ref lastHash);
			HashUtils.ContentHashOnto(AttackRatioIncrease, ref lastHash);
		}
	}
}
