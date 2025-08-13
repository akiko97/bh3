using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSettingPushTab : MonoBehaviour
	{
		public Transform staminaSettingBtn;

		public Transform skillPointSettingBtn;

		public Transform expeditionEndSettingBtn;

		public Transform cabinLevelUpSettingBtn;

		public Transform activitySettingBtn;

		private ConfigNotificationSetting _modifiedSettingConfig;

		public void SetupView()
		{
			_modifiedSettingConfig = new ConfigNotificationSetting();
			RecoverOriginState();
			Transform settingTrans = base.gameObject.transform.FindChild("Content/NotificationSetting/ThirdLine/ActivityNotification").transform;
			SetSettingEnable(settingTrans, false);
		}

		public bool CheckNeedSave()
		{
			return !NotificationSettingData.IsValueEqualToPersonalNotificationConfig(_modifiedSettingConfig);
		}

		public void OnNoSaveBtnClick()
		{
			RecoverOriginState();
		}

		public void OnSaveBtnClick()
		{
			NotificationSettingData.SavePersonalNotificationConfig(_modifiedSettingConfig);
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess")));
		}

		public void OnEnergyFullNotificationClick(bool willBeOn)
		{
			_modifiedSettingConfig.StaminaFullNotificaltion = willBeOn;
			SetChoiceBtn(staminaSettingBtn, _modifiedSettingConfig.StaminaFullNotificaltion);
		}

		public void OnSkillPointFullNotificationClick(bool willBeOn)
		{
			_modifiedSettingConfig.SkillPointFullNotification = willBeOn;
			SetChoiceBtn(skillPointSettingBtn, _modifiedSettingConfig.SkillPointFullNotification);
		}

		public void OnExpeditionEndNotificationClick(bool willBeOn)
		{
			_modifiedSettingConfig.VentureDoneNotification = willBeOn;
			SetChoiceBtn(expeditionEndSettingBtn, _modifiedSettingConfig.VentureDoneNotification);
		}

		public void OnCabinLevelUpNotificationClick(bool willBeOn)
		{
			_modifiedSettingConfig.CabinLevelUpNotification = willBeOn;
			SetChoiceBtn(cabinLevelUpSettingBtn, _modifiedSettingConfig.CabinLevelUpNotification);
		}

		public void OnActivityNotificationClick(bool willBeOn)
		{
			_modifiedSettingConfig.ActivityNotification = willBeOn;
			SetChoiceBtn(activitySettingBtn, _modifiedSettingConfig.ActivityNotification);
		}

		private void RecoverOriginState()
		{
			NotificationSettingData.ApplyNotificationSettingConfig();
			NotificationSettingData.CopyPersonalNotificationConfig(ref _modifiedSettingConfig);
			SetChoiceBtn(staminaSettingBtn, _modifiedSettingConfig.StaminaFullNotificaltion);
			SetChoiceBtn(skillPointSettingBtn, _modifiedSettingConfig.SkillPointFullNotification);
			SetChoiceBtn(expeditionEndSettingBtn, _modifiedSettingConfig.VentureDoneNotification);
			SetChoiceBtn(cabinLevelUpSettingBtn, _modifiedSettingConfig.CabinLevelUpNotification);
			SetChoiceBtn(activitySettingBtn, _modifiedSettingConfig.ActivityNotification);
		}

		private void SetChoiceBtn(Transform btn, bool active)
		{
			if (!(btn == null))
			{
				Transform transform = btn.Find("Choice/On");
				Transform transform2 = btn.Find("Choice/Off");
				transform.gameObject.SetActive(active);
				transform2.gameObject.SetActive(!active);
			}
		}

		private void SetSettingEnable(Transform settingTrans, bool enable)
		{
			Transform transform = settingTrans.FindChild("Choice/On");
			Transform transform2 = settingTrans.FindChild("Choice/Off");
			Transform transform3 = settingTrans.FindChild("Label");
			Transform transform4 = transform.FindChild("Text");
			Transform transform5 = transform2.FindChild("Text");
			if (!enable)
			{
				transform.FindChild("Blue").gameObject.SetActive(false);
				transform.FindChild("Grey").gameObject.SetActive(false);
				transform.FindChild("Disable").gameObject.SetActive(true);
				transform2.FindChild("Grey").gameObject.SetActive(false);
				transform2.FindChild("Disable").gameObject.SetActive(true);
				transform3.GetComponent<Text>().color = MiscData.GetColor("NotificationSettingDisableText");
				transform4.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform5.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
			}
			else
			{
				transform.FindChild("Blue").gameObject.SetActive(true);
				transform.FindChild("Grey").gameObject.SetActive(true);
				transform2.FindChild("Grey").gameObject.SetActive(true);
				transform.FindChild("Disable").gameObject.SetActive(false);
				transform2.FindChild("Disable").gameObject.SetActive(false);
				transform3.GetComponent<Text>().color = Color.white;
				transform4.GetComponent<Text>().color = Color.white;
				transform5.GetComponent<Text>().color = Color.white;
			}
			transform.GetComponent<Button>().interactable = enable;
			transform2.GetComponent<Button>().interactable = enable;
		}
	}
}
