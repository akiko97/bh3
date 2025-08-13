namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class PlayerLevelMetaData : IHashable
	{
		public readonly int level;

		public readonly int exp;

		public readonly int stamina;

		public readonly int numFriends;

		public readonly int avatarLevelLimit;

		public readonly int scBonus;

		public readonly int staminaBonus;

		public PlayerLevelMetaData(int level, int exp, int stamina, int numFriends, int avatarLevelLimit, int scBonus, int staminaBonus)
		{
			this.level = level;
			this.exp = exp;
			this.stamina = stamina;
			this.numFriends = numFriends;
			this.avatarLevelLimit = avatarLevelLimit;
			this.scBonus = scBonus;
			this.staminaBonus = staminaBonus;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(level, ref lastHash);
			HashUtils.ContentHashOnto(exp, ref lastHash);
			HashUtils.ContentHashOnto(stamina, ref lastHash);
			HashUtils.ContentHashOnto(numFriends, ref lastHash);
			HashUtils.ContentHashOnto(avatarLevelLimit, ref lastHash);
			HashUtils.ContentHashOnto(scBonus, ref lastHash);
			HashUtils.ContentHashOnto(staminaBonus, ref lastHash);
		}
	}
}
