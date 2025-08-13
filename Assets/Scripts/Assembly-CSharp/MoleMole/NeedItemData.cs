namespace MoleMole
{
	public class NeedItemData
	{
		public readonly int itemMetaID;

		public readonly int itemNum;

		public readonly StorageDataItemBase itemData;

		public bool enough;

		public NeedItemData(int itemMetaID, int itemNum)
		{
			this.itemMetaID = itemMetaID;
			this.itemNum = itemNum;
			itemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(itemMetaID);
			itemData.number = itemNum;
		}
	}
}
