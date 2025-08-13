using System.Collections.Generic;

namespace MoleMole
{
	public class SPMedicActor : BaseGoodsActor
	{
		public float healSP;

		public override void DoGoodsLogic(uint avatarRuntimeID)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.FireEffect("Ability_HealSP_Pick");
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			foreach (BaseMonoAvatar item in allPlayerAvatars)
			{
				AvatarActor avatarActor = (AvatarActor)Singleton<EventManager>.Instance.GetActor(item.GetRuntimeID());
				avatarActor.HealSP(healSP);
			}
			Kill();
		}
	}
}
