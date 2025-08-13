using UnityEngine;

namespace MoleMole
{
	public abstract class LDDropDataItem
	{
		public abstract void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true);

		public LDDropDataItem Clone()
		{
			return (LDDropDataItem)MemberwiseClone();
		}

		public static LDDropDataItem GetLDDropDataItemByName(string typeName)
		{
			switch (typeName)
			{
			case "HPMedic":
				return new LDDropHPMedic();
			case "SPMedic":
				return new LDDropSPMedic();
			case "EquipItem":
				return new LDDropEquipItem();
			case "Coin":
				return new LDDropCoin();
			case "Boost":
				return new LDDropBoostSpeed();
			case "Crit":
				return new LDDropEnhanceCrit();
			case "Shielded":
				return new LDDropShielded();
			default:
				return null;
			}
		}
	}
}
