using UnityEngine;

namespace MoleMole
{
	public class LDDropBoostSpeed : LDDropDataItem
	{
		public string abilityName = "Goods_BoostSpeed";

		public int dropNum = 1;

		public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = false)
		{
			for (int i = 0; i < dropNum; i++)
			{
				Singleton<DynamicObjectManager>.Instance.CreateGood(562036737u, "Boost", abilityName, 0f, initPos, initDir, actDropAnim);
			}
		}
	}
}
