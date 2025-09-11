using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoNotificationServices : MonoBehaviour
	{
		private int _notificationId;

		private List<int> _notificationIdList = new List<int>();

		private void Awake()
		{
		//	CleanNotification();
		}

		private void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				//AddGameLocalNotifications();
			}
			else
			{
				//CleanNotification();
			}
		}

		public void OnApplicationQuit()
		{
			//CleanNotification();
			//AddGameLocalNotifications();
		}

		private void AddGameLocalNotifications()
		{
			if (Singleton<NetworkManager>.Instance.alreadyLogin)
			{
				AddStaminaFullNotification();
				AddSkillPointFullNotification();
				AddActivityNotification();
				AddVentureDoneNotification();
				AddCabinLevelUpNotification();
			}
		}

		private void AddStaminaFullNotification()
		{
			bool staminaFullNotificaltion = Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.StaminaFullNotificaltion;
			if (!Singleton<PlayerModule>.Instance.playerData.IsStaminaFull() && staminaFullNotificaltion)
			{
				DateTime staminaFullTime = Singleton<PlayerModule>.Instance.playerData.GetStaminaFullTime();
				string text = LocalizationGeneralLogic.GetText("Menu_SettingEnergyFullNotification");
				string text2 = LocalizationGeneralLogic.GetText("Notification_FullStamina");
				AddLocalNotification(staminaFullTime, text, text2);
			}
		}

		private void AddSkillPointFullNotification()
		{
			bool skillPointFullNotification = Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.SkillPointFullNotification;
			if (!Singleton<PlayerModule>.Instance.playerData.IsSkillPointFull() && skillPointFullNotification)
			{
				DateTime skillPointFullTime = Singleton<PlayerModule>.Instance.playerData.GetSkillPointFullTime();
				string text = LocalizationGeneralLogic.GetText("Menu_SettingSkillPointFullNotification");
				string text2 = LocalizationGeneralLogic.GetText("Notification_FullSkillPoint");
				AddLocalNotification(skillPointFullTime, text, text2);
			}
		}

		private void AddActivityNotification()
		{
		}

		private void AddVentureDoneNotification()
		{
			if (!Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.VentureDoneNotification)
			{
				return;
			}
			List<VentureDataItem> ventureList = Singleton<IslandModule>.Instance.GetVentureList();
			foreach (VentureDataItem item in ventureList)
			{
				if (item.status == VentureDataItem.VentureStatus.InProgress)
				{
					string text = LocalizationGeneralLogic.GetText("Menu_SettingExpeditionEndNotification");
					string text2 = LocalizationGeneralLogic.GetText("Notification_VentureDone");
					AddLocalNotification(item.endTime, text, text2);
				}
			}
		}

		private void AddCabinLevelUpNotification()
		{
			if (!Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.CabinLevelUpNotification)
			{
				return;
			}
			List<CabinDataItemBase> cabinList = Singleton<IslandModule>.Instance.GetCabinList();
			foreach (CabinDataItemBase item in cabinList)
			{
				if (item.IsUpLevel())
				{
					string text = LocalizationGeneralLogic.GetText("Menu_SettingCabinLevelUpNotification");
					string text2 = LocalizationGeneralLogic.GetText("Notification_CabinLevelUp");
					AddLocalNotification(item.levelUpEndTime, text, text2);
				}
			}
		}

		private void AddLocalNotification(DateTime time, string title, string text)
		{
			if (!(time < DateTime.Now))
			{
				int num = ++_notificationId;
				_notificationIdList.Add(num);
				//LocalNotificationPlugin.SendNotification(num, time - DateTime.Now, title, text);
			}
		}

		private void CleanNotification()
		{
			//LocalNotificationPlugin.ClearNotifications();
			foreach (int notificationId in _notificationIdList)
			{
				//LocalNotificationPlugin.CancelNotification(notificationId);
			}
			_notificationIdList.Clear();
		}
	}
}
