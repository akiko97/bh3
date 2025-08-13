using UnityEngine;

namespace MoleMole
{
	public class LDDropSPMedic : LDDropDataItem
	{
		public float healSP;

		public int dropNum = 1;

		public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true)
		{
			for (int i = 0; i < dropNum; i++)
			{
				Singleton<DynamicObjectManager>.Instance.CreateSPMedic(562036737u, initPos, initDir, healSP, actDropAnim);
			}
		}
	}
}
