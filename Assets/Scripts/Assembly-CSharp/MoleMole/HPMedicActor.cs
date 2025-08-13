using System.Collections.Generic;

namespace MoleMole
{
	public class HPMedicActor : BaseGoodsActor
	{
		public float healHP;

		public override void DoGoodsLogic(uint avatarRuntimeID)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.PickHPMedic(runtimeID);
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			foreach (BaseMonoAvatar item in allPlayerAvatars)
			{
				AvatarActor avatarActor = (AvatarActor)Singleton<EventManager>.Instance.GetActor(item.GetRuntimeID());
				avatarActor.HealHP(healHP);
			}
			Kill();
		}
	}
}
