using UnityEngine;

namespace MoleMole
{
	public class LDDropCoin : LDDropDataItem
	{
		public float scoinReward;

		public int dropNum = 1;

		public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true)
		{
			for (int i = 0; i < dropNum; i++)
			{
				Singleton<DynamicObjectManager>.Instance.CreateCoin(562036737u, initPos, initDir, scoinReward, actDropAnim);
			}
		}
	}
}
