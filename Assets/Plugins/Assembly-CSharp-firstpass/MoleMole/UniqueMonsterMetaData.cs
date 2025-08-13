using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class UniqueMonsterMetaData : IHashable
	{
		public readonly uint ID;

		public readonly string name;

		public readonly string monsterName;

		public readonly string typeName;

		public readonly float HPRatio;

		public readonly float attackRatio;

		public readonly float defenseRatio;

		public readonly float moveSpeedRatio;

		public readonly List<float> ATKRatios;

		public readonly string configType;

		public readonly string AIName;

		public readonly List<string> attackCDNames;

		public readonly List<float> attackCDs;

		public readonly string abilities;

		public readonly int hpPhaseNum;

		public readonly List<float> scale;

		public UniqueMonsterMetaData(uint ID, string name, string monsterName, string typeName, float HPRatio, float attackRatio, float defenseRatio, float moveSpeedRatio, List<float> ATKRatios, string configType, string AIName, List<string> attackCDNames, List<float> attackCDs, string abilities, int hpPhaseNum, List<float> scale)
		{
			this.ID = ID;
			this.name = name;
			this.monsterName = monsterName;
			this.typeName = typeName;
			this.HPRatio = HPRatio;
			this.attackRatio = attackRatio;
			this.defenseRatio = defenseRatio;
			this.moveSpeedRatio = moveSpeedRatio;
			this.ATKRatios = ATKRatios;
			this.configType = configType;
			this.AIName = AIName;
			this.attackCDNames = attackCDNames;
			this.attackCDs = attackCDs;
			this.abilities = abilities;
			this.hpPhaseNum = hpPhaseNum;
			this.scale = scale;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(name, ref lastHash);
			HashUtils.ContentHashOnto(monsterName, ref lastHash);
			HashUtils.ContentHashOnto(typeName, ref lastHash);
			HashUtils.ContentHashOnto(HPRatio, ref lastHash);
			HashUtils.ContentHashOnto(attackRatio, ref lastHash);
			HashUtils.ContentHashOnto(defenseRatio, ref lastHash);
			HashUtils.ContentHashOnto(moveSpeedRatio, ref lastHash);
			if (ATKRatios != null)
			{
				foreach (float aTKRatio in ATKRatios)
				{
					float value = aTKRatio;
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(configType, ref lastHash);
			HashUtils.ContentHashOnto(AIName, ref lastHash);
			if (attackCDNames != null)
			{
				foreach (string attackCDName in attackCDNames)
				{
					HashUtils.ContentHashOnto(attackCDName, ref lastHash);
				}
			}
			if (attackCDs != null)
			{
				foreach (float attackCD in attackCDs)
				{
					float value2 = attackCD;
					HashUtils.ContentHashOnto(value2, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(abilities, ref lastHash);
			HashUtils.ContentHashOnto(hpPhaseNum, ref lastHash);
			if (scale == null)
			{
				return;
			}
			foreach (float item in scale)
			{
				float value3 = item;
				HashUtils.ContentHashOnto(value3, ref lastHash);
			}
		}
	}
}
