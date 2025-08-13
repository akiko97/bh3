using UnityEngine;

namespace MoleMole
{
	public class LDDropHPMedic : LDDropDataItem
	{
		public float healHP;

		public int dropNum = 1;

		public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true)
		{
			for (int i = 0; i < dropNum; i++)
			{
				Singleton<DynamicObjectManager>.Instance.CreateHPMedic(562036737u, initPos, initDir, healHP, actDropAnim);
			}
		}
	}
}
