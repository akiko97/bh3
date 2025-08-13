using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class EquipmentLevelMetaData : IHashable
	{
		public readonly int level;

		public readonly List<int> expList;

		public readonly int weaponUpgradeCost;

		public readonly int weaponEvoCost;

		public readonly int stigmataUpgradeCost;

		public readonly int stigmataEvoCost;

		public EquipmentLevelMetaData(int level, List<int> expList, int weaponUpgradeCost, int weaponEvoCost, int stigmataUpgradeCost, int stigmataEvoCost)
		{
			this.level = level;
			this.expList = expList;
			this.weaponUpgradeCost = weaponUpgradeCost;
			this.weaponEvoCost = weaponEvoCost;
			this.stigmataUpgradeCost = stigmataUpgradeCost;
			this.stigmataEvoCost = stigmataEvoCost;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(level, ref lastHash);
			if (expList != null)
			{
				foreach (int exp in expList)
				{
					HashUtils.ContentHashOnto(exp, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(weaponUpgradeCost, ref lastHash);
			HashUtils.ContentHashOnto(weaponEvoCost, ref lastHash);
			HashUtils.ContentHashOnto(stigmataUpgradeCost, ref lastHash);
			HashUtils.ContentHashOnto(stigmataEvoCost, ref lastHash);
		}
	}
}
