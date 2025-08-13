using UnityEngine;

namespace MoleMole
{
	public class LDDropEquipItem : LDDropDataItem
	{
		public int metaId = 1;

		public int level = 1;

		public int dropNum = 1;

		public int rarity = 1;

		public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true)
		{
			for (int i = 0; i < dropNum; i++)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId);
				if (dummyStorageDataItem is WeaponDataItem)
				{
					Singleton<DynamicObjectManager>.Instance.CreateEquipItem(562036737u, metaId, initPos, initDir, actDropAnim, level);
				}
				else if (dummyStorageDataItem is StigmataDataItem)
				{
					Singleton<DynamicObjectManager>.Instance.CreateStigmataItem(562036737u, metaId, initPos, initDir, actDropAnim, level);
				}
				else if (dummyStorageDataItem is MaterialDataItem)
				{
					Singleton<DynamicObjectManager>.Instance.CreateMaterialItem(562036737u, metaId, initPos, initDir, actDropAnim, level);
				}
				else if (dummyStorageDataItem is AvatarFragmentDataItem)
				{
					Singleton<DynamicObjectManager>.Instance.CreateAvatarFragmentItem(562036737u, metaId, initPos, initDir, actDropAnim, level);
				}
			}
			Singleton<LevelScoreManager>.Instance.AddDropItem(metaId, level, dropNum);
		}
	}
}
