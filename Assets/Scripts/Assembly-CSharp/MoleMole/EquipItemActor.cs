namespace MoleMole
{
	public class EquipItemActor : BaseGoodsActor
	{
		public int rarity { get; set; }

		public override void DoGoodsLogic(uint avatarRuntimeID)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.PickupEquipItem(rarity, runtimeID);
			if (_entity.DropItemMetaID != -1)
			{
				Singleton<LevelScoreManager>.Instance.AddDropItemToShow(_entity.DropItemMetaID, _entity.DropItemLevel, _entity.DropItemNum);
			}
			Kill();
		}
	}
}
