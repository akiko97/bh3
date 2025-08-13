namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterConfigMetaData : IHashable
	{
		public readonly string monsterName;

		public readonly string typeName;

		public readonly string categoryName;

		public readonly string subTypeName;

		public readonly float HP;

		public readonly float attack;

		public readonly float defense;

		public readonly int nature;

		public readonly string configFile;

		public readonly string configType;

		public readonly string AIName;

		public MonsterConfigMetaData(string monsterName, string typeName, string categoryName, string subTypeName, float HP, float attack, float defense, int nature, string configFile, string configType, string AIName)
		{
			this.monsterName = monsterName;
			this.typeName = typeName;
			this.categoryName = categoryName;
			this.subTypeName = subTypeName;
			this.HP = HP;
			this.attack = attack;
			this.defense = defense;
			this.nature = nature;
			this.configFile = configFile;
			this.configType = configType;
			this.AIName = AIName;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(monsterName, ref lastHash);
			HashUtils.ContentHashOnto(typeName, ref lastHash);
			HashUtils.ContentHashOnto(categoryName, ref lastHash);
			HashUtils.ContentHashOnto(subTypeName, ref lastHash);
			HashUtils.ContentHashOnto(HP, ref lastHash);
			HashUtils.ContentHashOnto(attack, ref lastHash);
			HashUtils.ContentHashOnto(defense, ref lastHash);
			HashUtils.ContentHashOnto(nature, ref lastHash);
			HashUtils.ContentHashOnto(configFile, ref lastHash);
			HashUtils.ContentHashOnto(configType, ref lastHash);
			HashUtils.ContentHashOnto(AIName, ref lastHash);
		}
	}
}
