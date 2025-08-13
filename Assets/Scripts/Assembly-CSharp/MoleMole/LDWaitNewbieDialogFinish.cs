using UnityEngine;

namespace MoleMole
{
	public class LDWaitNewbieDialogFinish : BaseLDEvent
	{
		private Transform newbie;

		public LDWaitNewbieDialogFinish()
		{
			newbie = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().transform.Find("Dialogs/NewbieDialog(Clone)");
		}

		public override void Core()
		{
			if (Singleton<MainUIManager>.Instance.GetInLevelUICanvas().transform.Find("Dialogs/NewbieDialog(Clone)") == null)
			{
				Done();
			}
		}

		private void PlayBossBornSound()
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			MonoEntityAudio component = localAvatar.GetComponent<MonoEntityAudio>();
			if (component != null)
			{
				component.PostBossBorn();
			}
		}
	}
}
