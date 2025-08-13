namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterSkillMetaData : IHashable
	{
		public readonly int monsterSkillID;

		public readonly string displayName;

		public readonly string displayDetail;

		public MonsterSkillMetaData(int monsterSkillID, string displayName, string displayDetail)
		{
			this.monsterSkillID = monsterSkillID;
			this.displayName = displayName;
			this.displayDetail = displayDetail;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(monsterSkillID, ref lastHash);
			HashUtils.ContentHashOnto(displayName, ref lastHash);
			HashUtils.ContentHashOnto(displayDetail, ref lastHash);
		}
	}
}
