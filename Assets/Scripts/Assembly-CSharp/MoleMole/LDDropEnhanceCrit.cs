using UnityEngine;

namespace MoleMole
{
	public class LDDropEnhanceCrit : LDDropDataItem
	{
		public string abilityName = "Goods_EnhanceCrit";

		public int dropNum = 1;

		public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = false)
		{
			for (int i = 0; i < dropNum; i++)
			{
				Singleton<DynamicObjectManager>.Instance.CreateGood(562036737u, "Crit", abilityName, 0f, initPos, initDir, actDropAnim);
			}
		}
	}
}
