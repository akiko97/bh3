using UnityEngine;

namespace MoleMole
{
	public class LDDropShielded : LDDropDataItem
	{
		public string abilityName = "Goods_Invincible";

		public int dropNum = 1;

		public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true)
		{
			for (int i = 0; i < dropNum; i++)
			{
				Singleton<DynamicObjectManager>.Instance.CreateGood(562036737u, "Shielded", abilityName, 0f, initPos, initDir, actDropAnim);
			}
		}
	}
}
