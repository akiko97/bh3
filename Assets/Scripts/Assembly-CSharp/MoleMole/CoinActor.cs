namespace MoleMole
{
	public class CoinActor : BaseGoodsActor
	{
		public float scoinReward;

		public override void DoGoodsLogic(uint avatarRuntimeID)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.PickupCoin(runtimeID);
			Singleton<LevelScoreManager>.Instance.scoinInside += scoinReward;
			Kill();
		}
	}
}
