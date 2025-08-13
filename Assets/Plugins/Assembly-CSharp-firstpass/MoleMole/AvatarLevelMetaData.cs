namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarLevelMetaData : IHashable
	{
		public readonly int level;

		public readonly int exp;

		public readonly int cost;

		public readonly float avatarAssistConf;

		public AvatarLevelMetaData(int level, int exp, int cost, float avatarAssistConf)
		{
			this.level = level;
			this.exp = exp;
			this.cost = cost;
			this.avatarAssistConf = avatarAssistConf;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(level, ref lastHash);
			HashUtils.ContentHashOnto(exp, ref lastHash);
			HashUtils.ContentHashOnto(cost, ref lastHash);
			HashUtils.ContentHashOnto(avatarAssistConf, ref lastHash);
		}
	}
}
