using UnityEngine;

namespace MoleMole
{
	public class MonoAvatarExpGrow : MonoBehaviour
	{
		public MonoMaskSliderGrow[] sliders;

		public void PlayAvatarExpSliderGrow()
		{
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			for (int i = 0; i < instance.memberList.Count; i++)
			{
				AvatarDataItem avatarDataItem = instance.memberList[i];
				AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarDataItem.avatarID);
				MonoMaskSliderGrow monoMaskSliderGrow = sliders[i];
				int level = avatarDataItem.level;
				int exp = avatarDataItem.exp;
				int level2 = avatarByID.level;
				int exp2 = avatarByID.exp;
				monoMaskSliderGrow.Play(exp, exp2, UIUtil.GetAvatarMaxExpList(avatarDataItem, level, level2), ShowAvatarLevelUpHint, ShowCanUnlockSkillDialog);
			}
		}

		private void ShowAvatarLevelUpHint(Transform sliderTrans)
		{
			sliderTrans.Find("LevelUpHint").GetComponent<Animation>().Play();
		}

		private void ShowCanUnlockSkillDialog(Transform sliderTrans)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AvatarLevelUp));
		}
	}
}
