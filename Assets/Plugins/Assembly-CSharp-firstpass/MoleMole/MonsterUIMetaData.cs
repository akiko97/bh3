using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterUIMetaData : IHashable
	{
		public readonly int monsterID;

		public readonly string name;

		public readonly string displayTitle;

		public readonly string displayIntroduction;

		public readonly int monsterType;

		public readonly string displayType;

		public readonly float HP;

		public readonly float attack;

		public readonly float defence;

		public readonly float speed;

		public readonly float range;

		public readonly List<int> monsterSkillIDList;

		public readonly string prefabPath;

		public MonsterUIMetaData(int monsterID, string name, string displayTitle, string displayIntroduction, int monsterType, string displayType, float HP, float attack, float defence, float speed, float range, List<int> monsterSkillIDList, string prefabPath)
		{
			this.monsterID = monsterID;
			this.name = name;
			this.displayTitle = displayTitle;
			this.displayIntroduction = displayIntroduction;
			this.monsterType = monsterType;
			this.displayType = displayType;
			this.HP = HP;
			this.attack = attack;
			this.defence = defence;
			this.speed = speed;
			this.range = range;
			this.monsterSkillIDList = monsterSkillIDList;
			this.prefabPath = prefabPath;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(monsterID, ref lastHash);
			HashUtils.ContentHashOnto(name, ref lastHash);
			HashUtils.ContentHashOnto(displayTitle, ref lastHash);
			HashUtils.ContentHashOnto(displayIntroduction, ref lastHash);
			HashUtils.ContentHashOnto(monsterType, ref lastHash);
			HashUtils.ContentHashOnto(displayType, ref lastHash);
			HashUtils.ContentHashOnto(HP, ref lastHash);
			HashUtils.ContentHashOnto(attack, ref lastHash);
			HashUtils.ContentHashOnto(defence, ref lastHash);
			HashUtils.ContentHashOnto(speed, ref lastHash);
			HashUtils.ContentHashOnto(range, ref lastHash);
			if (monsterSkillIDList != null)
			{
				foreach (int monsterSkillID in monsterSkillIDList)
				{
					HashUtils.ContentHashOnto(monsterSkillID, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(prefabPath, ref lastHash);
		}
	}
}
