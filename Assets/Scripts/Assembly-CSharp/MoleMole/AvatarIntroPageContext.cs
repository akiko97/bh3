using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class AvatarIntroPageContext : BasePageContext
	{
		private const string AVATAR_MODEL_PARA_KEY = "LvUpTab";

		public readonly AvatarDataItem avatarData;

		private MonoAvatarRotatePanel _avatarRotatePanel;

		public AvatarIntroPageContext(AvatarDataItem avatarData)
		{
			config = new ContextPattern
			{
				contextName = "AvatarIntroPageContext",
				viewPrefabPath = "UI/Menus/Page/AvatarIntroPage"
			};
			showSpaceShip = true;
			this.avatarData = avatarData;
		}

		protected override bool SetupView()
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(avatarData.avatarID).avatarCardID);
			base.view.transform.Find("AvatarFigurePanel/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetImagePath());
			SetupAvatarProfile();
			SetupAvatarInfo();
			if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
			{
				base.view.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn");
			}
			return false;
		}

		private void SetupAvatarProfile()
		{
			Transform transform = base.view.transform.Find("AvatarDetailProfile");
			SetupClassName(transform.Find("ClassName"), avatarData);
			transform.Find("AvatarStar").GetComponent<MonoAvatarStar>().SetupView(avatarData.star);
			transform.Find("Desc").GetComponent<Text>().text = avatarData.Desc;
		}

		private void SetupAvatarInfo()
		{
			Transform transform = base.view.transform.Find("InfoPanel/Attributes");
			transform.Find("BasicStatus").GetComponent<MonoAttributeDisplay>().SetupView(avatarData);
			Transform transform2 = base.view.transform.Find("InfoPanel/Skills/Info");
			SetupSkillIcon(avatarData.GetUltraSkill(), transform2.Find("UltraSkill"));
			SetupSkillIcon(avatarData.GetLeaderSkill(), transform2.Find("LeaderSkill"));
		}

		private void SetupClassName(Transform parent, AvatarDataItem avatarSelected)
		{
			parent.Find("FirstName").GetComponent<Text>().text = avatarSelected.ClassFirstName;
			parent.Find("FirstName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnFirstName;
			parent.Find("LastName").GetComponent<Text>().text = avatarSelected.ClassLastName;
			parent.Find("LastName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnLastName;
		}

		private void SetupSkillIcon(AvatarSkillDataItem skillData, Transform skillTrans)
		{
			skillTrans.Find("Info/NameText").GetComponent<Text>().text = skillData.SkillName;
			skillTrans.Find("Info/Desc").GetComponent<Text>().text = skillData.SkillInfo;
			Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(skillData.IconPath);
			skillTrans.Find("SkillIcon/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
			string key = "SkillBtnBlue";
			skillTrans.Find("SkillIcon/Icon/Image").GetComponent<Image>().material = null;
			skillTrans.Find("SkillIcon/Icon/BG").GetComponent<Image>().color = MiscData.GetColor(key);
		}
	}
}
