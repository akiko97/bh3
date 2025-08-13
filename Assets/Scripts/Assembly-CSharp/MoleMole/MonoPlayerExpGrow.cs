using UnityEngine;

namespace MoleMole
{
	public class MonoPlayerExpGrow : MonoBehaviour
	{
		public MonoMaskSliderGrow slider;

		public void PlayPlayerExpSliderGrow()
		{
			int playerLevelBefore = Singleton<LevelScoreManager>.Instance.playerLevelBefore;
			int playerExpBefore = Singleton<LevelScoreManager>.Instance.playerExpBefore;
			int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
			int teamExp = Singleton<PlayerModule>.Instance.playerData.teamExp;
			slider.Play(playerExpBefore, teamExp, UIUtil.GetPlayerMaxExpList(playerLevelBefore, teamLevel), ShowLevelUpHint, ShowLevelUpDialog);
		}

		private void ShowLevelUpHint(Transform sliderTrans)
		{
			base.transform.Find("LevelUpHint").GetComponent<Animation>().Play();
		}

		private void ShowLevelUpDialog(Transform sliderTrans)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PlayerLevelUp));
		}
	}
}
