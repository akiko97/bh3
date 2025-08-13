using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class AvatarSkillDialogContext : BaseDialogContext
	{
		public readonly AvatarDataItem avatarData;

		public readonly AvatarSkillDataItem skillData;

		public AvatarSkillDialogContext(AvatarDataItem avatarData, AvatarSkillDataItem skillData)
		{
			config = new ContextPattern
			{
				contextName = "AvatarSkillDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AvatarSkillDialogV2",
				ignoreNotify = true
			};
			this.avatarData = avatarData;
			this.skillData = skillData;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/SingleButton/Btn").GetComponent<Button>(), OnBtnClick);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/NameRow/NameText").GetComponent<Text>().text = skillData.SkillName;
			base.view.transform.Find("Dialog/Content/VerticalLayout/DescText").GetComponent<Text>().text = skillData.SkillInfo;
			Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(skillData.IconPath);
			base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
			base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/RemainSkillPoint/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.skillPoint.ToString();
			SetupLackInfo();
			if (string.IsNullOrEmpty(skillData.SkillStep))
			{
				base.view.transform.Find("Dialog/Content/VerticalLayout/Step").gameObject.SetActive(false);
			}
			else
			{
				base.view.transform.Find("Dialog/Content/VerticalLayout/Step").gameObject.SetActive(true);
				base.view.transform.Find("Dialog/Content/VerticalLayout/Step/Table").GetComponent<MonoAvatarSkillStep>().SetupView(avatarData, skillData.SkillStep);
			}
			base.view.transform.Find("Dialog/Content/SingleButton").gameObject.SetActive(true);
			string textID = ((!skillData.CanTry) ? "Menu_OK" : "Menu_TrySkill");
			base.view.transform.Find("Dialog/Content/SingleButton/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID);
			return false;
		}

		public void Close()
		{
			Destroy();
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		public void OnBtnClick()
		{
			if (skillData.CanTry)
			{
				Singleton<LevelScoreManager>.Create();
				Singleton<LevelScoreManager>.Instance.SetTryLevelBeginIntent(avatarData.avatarID, "Lua/Levels/Common/LevelInfinityTest.lua", skillData.skillID);
				Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true);
			}
			else
			{
				Close();
			}
		}

		private void SetupLackInfo()
		{
			bool flag = avatarData.level < skillData.UnLockLv;
			bool flag2 = avatarData.star < skillData.UnLockStar;
			Transform transform = base.view.transform.Find("Dialog/Content/VerticalLayout/StarLack");
			Transform transform2 = base.view.transform.Find("Dialog/Content/VerticalLayout/LevelLack");
			if (flag2)
			{
				transform.gameObject.SetActive(true);
				transform2.gameObject.SetActive(false);
				transform.Find("UnLockStar").GetComponent<MonoAvatarStar>().SetupView(skillData.UnLockStar);
			}
			else if (flag)
			{
				transform.gameObject.SetActive(false);
				transform2.gameObject.SetActive(true);
				transform2.Find("LvNeed").GetComponent<Text>().text = skillData.UnLockLv.ToString();
			}
			else
			{
				transform.gameObject.SetActive(false);
				transform2.gameObject.SetActive(false);
			}
		}
	}
}
