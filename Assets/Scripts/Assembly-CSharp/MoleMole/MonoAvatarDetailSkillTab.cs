using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAvatarDetailSkillTab : MonoBehaviour
	{
		private FriendDetailDataItem _userData;

		private AvatarDataItem _avatarData;

		private bool _isRemoteAvatar;

		public void SetupView(AvatarDataItem avatarData, AvatarSkillDataItem selectedSkillData = null)
		{
			_isRemoteAvatar = false;
			_avatarData = avatarData;
			base.transform.Find("ListPanel/Info/Content/SkillPoint").gameObject.SetActive(true);
			SetupSkillPoint();
			if (selectedSkillData != null)
			{
				SetupSelectSkillView(selectedSkillData);
			}
			else
			{
				SetupSkillListView();
			}
		}

		public void SetupView(FriendDetailDataItem userData, AvatarSkillDataItem selectedSkillData = null)
		{
			_isRemoteAvatar = true;
			_userData = userData;
			_avatarData = _userData.leaderAvatar;
			base.transform.Find("ListPanel/Info/Content/SkillPoint").gameObject.SetActive(false);
			if (selectedSkillData != null)
			{
				SetupSelectSkillView(selectedSkillData);
			}
			else
			{
				SetupSkillListView();
			}
		}

		public void OnBackPage()
		{
			if (base.transform.Find("ListPanel/Info/Content/SubSkill").gameObject.activeSelf)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectAvtarSkillIconChange, 0));
			}
			else
			{
				Singleton<MainUIManager>.Instance.BackPage();
			}
		}

		public void SetupSkillPoint()
		{
			base.transform.Find("ListPanel/Info/Content/SkillPoint/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.skillPoint.ToString();
			base.transform.Find("ListPanel/Info/Content/SkillPoint/MaxNum").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.skillPointLimit.ToString();
		}

		public void OnSkillPointButtonClick()
		{
			if (!_isRemoteAvatar)
			{
				if (Singleton<PlayerModule>.Instance.playerData.skillPointExchangeCache.Value != null)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new SkillPointExchangeDialogContext());
					return;
				}
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShouldShowSPExchange));
				Singleton<NetworkManager>.Instance.RequestGetSkillPointExchangeInfo();
			}
		}

		private void SetupSkillListView()
		{
			base.transform.Find("SelectedSkill").gameObject.SetActive(false);
			base.transform.Find("ListPanel/Info/Content/SubSkill").gameObject.SetActive(false);
			Transform transform = base.transform.Find("ListPanel/Info/Content/Skill");
			transform.gameObject.SetActive(true);
			transform.Find("Center").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarClassSkillIconPath[_avatarData.ClassId]);
			foreach (AvatarSkillDataItem skillData in _avatarData.skillDataList)
			{
				string text = "Skill_" + skillData.ShowOrder;
				Transform transform2 = transform.Find(text);
				if (!(transform2 == null))
				{
					transform2.GetComponent<MonoAvatarSkillIconButton>().SetupView(_avatarData, skillData, _isRemoteAvatar);
				}
			}
			base.transform.Find("ListPanel").GetComponent<Animation>().Play();
		}

		private void SetupSelectSkillView(AvatarSkillDataItem selectedSkillData)
		{
			base.transform.Find("ListPanel/Info/Content/Skill").gameObject.SetActive(false);
			Transform transform = base.transform.Find("SelectedSkill");
			transform.gameObject.SetActive(true);
			SetupSelectSkillDetailView(transform, selectedSkillData);
			Transform transform2 = base.transform.Find("ListPanel/Info/Content/SubSkill");
			transform2.gameObject.SetActive(true);
			SetupSelectSkillSubSkillListView(transform2, selectedSkillData);
			base.transform.Find("ListPanel").GetComponent<Animation>().Play();
		}

		private void SetupSelectSkillSubSkillListView(Transform trans, AvatarSkillDataItem selectedSkillData)
		{
			trans.Find("ParentSkill/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(selectedSkillData.IconPath);
			trans.Find("ParentSkill/Icon").GetComponent<Image>().color = MiscData.GetColor("Blue");
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(1);
			hashSet.Add(2);
			hashSet.Add(3);
			HashSet<int> hashSet2 = hashSet;
			foreach (AvatarSubSkillDataItem avatarSubSkill in selectedSkillData.avatarSubSkillList)
			{
				string text = "SubSkill_" + avatarSubSkill.ShowOrder;
				Transform transform = trans.Find(text);
				if (!(transform == null))
				{
					transform.gameObject.SetActive(true);
					transform.GetComponent<MonoAvatarSubSkillIconButton>().SetupView(_avatarData, selectedSkillData, avatarSubSkill, _isRemoteAvatar);
					hashSet2.Remove(avatarSubSkill.ShowOrder);
				}
			}
			foreach (int item in hashSet2)
			{
				string text2 = "SubSkill_" + item;
				trans.Find(text2).gameObject.SetActive(false);
			}
		}

		private void SetupSelectSkillDetailView(Transform trans, AvatarSkillDataItem selectedSkillData)
		{
			trans.Find("Content/NameText").GetComponent<Text>().text = selectedSkillData.SkillName;
			trans.Find("Content/DescText").GetComponent<Text>().text = selectedSkillData.SkillInfo;
			if (string.IsNullOrEmpty(selectedSkillData.SkillStep))
			{
				trans.Find("Content/Step").gameObject.SetActive(false);
				return;
			}
			trans.Find("Content/Step").gameObject.SetActive(true);
			trans.Find("Content/Step/Table").GetComponent<MonoAvatarSkillStep>().SetupView(_avatarData, selectedSkillData.SkillStep);
		}
	}
}
