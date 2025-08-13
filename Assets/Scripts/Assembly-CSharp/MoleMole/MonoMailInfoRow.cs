using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoMailInfoRow : MonoBehaviour
	{
		private const string MAIL_OPEN_ICON_PATH = "SpriteOutput/Mail/IconMailOpen";

		private const string MAIL_UNREAD_ICON_PATH = "SpriteOutput/Mail/IconMailUnread";

		private const string MAIL_REWARD_HC_PATH = "SpriteOutput/Mail/RewardIcons/IconStatusBarHCoin";

		private const string MAIL_REWARD_SC_PATH = "SpriteOutput/Mail/RewardIcons/IconStatusBarSCoin";

		private const string MAIL_UNREND_TEXT_ID = "Menu_Desc_MailUnRead";

		private const string MAIL_READED_TEXT_ID = "Menu_Desc_MailReaded";

		private MailDataItem _mailData;

		private Action<MailDataItem> _checkBtnCallBack;

		private Action<MailDataItem> _getBtnCallBack;

		public void SetupView(MailDataItem mailData, Action<MailDataItem> checkBtnCallBack, Action<MailDataItem> getBtnCallBack)
		{
			_mailData = mailData;
			_checkBtnCallBack = checkBtnCallBack;
			_getBtnCallBack = getBtnCallBack;
			Image component = base.transform.Find("ItemIconButton/ItemIcon").GetComponent<Image>();
			Image component2 = base.transform.Find("ItemIconButton/ItemIcon/Icon").GetComponent<Image>();
			ResetIconImageSize();
			if (_mailData.hasAttachment)
			{
				RewardUIData rewardUIData = _mailData.attachment.itemList[0];
				component.color = MiscData.GetColor("TotalWhite");
				if (rewardUIData.rewardType == ResourceType.Item)
				{
					component2.transform.GetComponent<MonoImageFitter>().enabled = true;
					StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(rewardUIData.itemID);
					component.sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[dummyStorageDataItem.rarity]);
				}
				else
				{
					component.sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[0]);
					component2.transform.GetComponent<MonoImageFitter>().enabled = false;
				}
				component2.sprite = rewardUIData.GetIconSprite();
				base.transform.Find("ItemIconButton/Text").GetComponent<Text>().text = "×" + rewardUIData.value;
			}
			else
			{
				component2.transform.GetComponent<MonoImageFitter>().enabled = true;
				if (Singleton<MailModule>.Instance.IsMailRead(_mailData))
				{
					component.color = MiscData.GetColor("MailUnreadGrey");
					component2.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Mail/IconMailOpen");
					base.transform.Find("ItemIconButton/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_MailReaded");
				}
				else
				{
					component.color = MiscData.GetColor("Blue");
					component2.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Mail/IconMailUnread");
					base.transform.Find("ItemIconButton/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_MailUnRead");
				}
			}
			base.transform.Find("ItemIconButton/NewMark").gameObject.SetActive(Singleton<MailModule>.Instance.IsMailNew(_mailData));
			base.transform.Find("Time/Time").GetComponent<Text>().text = Miscs.GetBeforeTimeToShow(_mailData.time);
			base.transform.Find("Info/Content").GetComponent<Text>().text = GetMailContentAbstract();
			base.transform.Find("Info/Sender").GetComponent<Text>().text = _mailData.sender;
			base.transform.Find("Title/Text").GetComponent<Text>().text = _mailData.title;
			base.transform.Find("ActionBtns/GetBtn").gameObject.SetActive(_mailData.hasAttachment);
		}

		public void OnCheckBtnClick()
		{
			if (_checkBtnCallBack != null)
			{
				_checkBtnCallBack(_mailData);
			}
		}

		public void OnGetBtnClick()
		{
			if (_getBtnCallBack != null)
			{
				_getBtnCallBack(_mailData);
			}
		}

		private void ResetIconImageSize()
		{
			Image component = base.transform.Find("ItemIconButton/ItemIcon").GetComponent<Image>();
			Image component2 = base.transform.Find("ItemIconButton/ItemIcon/Icon").GetComponent<Image>();
			component2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, component.rectTransform.rect.width);
			component2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, component.rectTransform.rect.height);
		}

		private string GetMailContentAbstract()
		{
			if (string.IsNullOrEmpty(_mailData.content))
			{
				return string.Empty;
			}
			string input = UIUtil.ProcessStrWithNewLine(_mailData.content);
			string[] array = Regex.Split(input, Environment.NewLine);
			if (array.Length <= 0)
			{
				return string.Empty;
			}
			string text = array[0];
			text = ((text.Length <= 20) ? text.Substring(0, text.Length - 1) : text.Substring(0, (text.Length <= 20) ? text.Length : 20));
			return text + "...";
		}

		public MailCacheKey GetMailCacheKey()
		{
			return _mailData.GetKeyForMailCache();
		}
	}
}
