using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MailDetailDialogContext : BaseDialogContext
	{
		private const string MAIL_REWARD_HC_PATH = "SpriteOutput/Mail/RewardIcons/IconStatusBarHCoin";

		private const string MAIL_REWARD_SC_PATH = "SpriteOutput/Mail/RewardIcons/IconStatusBarSCoin";

		private MailDataItem _mailData;

		public MailDetailDialogContext(MailDataItem mailData)
		{
			config = new ContextPattern
			{
				contextName = "MailDetailDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/MailDetailDialog"
			};
			_mailData = mailData;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>(), OnOKBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Destroy);
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), Destroy);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = _mailData.title;
			base.view.transform.Find("Dialog/Content/Sender/Sender").GetComponent<Text>().text = _mailData.sender;
			base.view.transform.Find("Dialog/Content/Time/Time").GetComponent<Text>().text = Miscs.GetTimeString(_mailData.time);
			base.view.transform.Find("Dialog/Content/MailContentScrollView/Content/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(_mailData.content);
			Transform transform = base.view.transform.Find("Dialog/Content/Items");
			transform.gameObject.SetActive(_mailData.hasAttachment);
			if (_mailData.hasAttachment)
			{
				int count = _mailData.attachment.itemList.Count;
				transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(OnScrollChange, count);
				base.view.transform.Find("Dialog/Content/MailContentScrollView").GetComponent<LayoutElement>().preferredHeight = 150f;
			}
			else
			{
				base.view.transform.Find("Dialog/Content/MailContentScrollView").GetComponent<LayoutElement>().preferredHeight = 350f;
			}
			string textID = ((!_mailData.hasAttachment) ? "Menu_Close" : "Menu_Action_Get");
			base.view.transform.Find("Dialog/Content/ActionBtns/OK/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID);
			return false;
		}

		private void OnOKBtnClick()
		{
			if (_mailData.hasAttachment)
			{
				Singleton<NetworkManager>.Instance.RequestGetOneMailAttachment(_mailData);
			}
			Destroy();
		}

		private void OnScrollChange(Transform trans, int index)
		{
			RewardUIData rewardUIData = _mailData.attachment.itemList[index];
			Image component = trans.Find("ItemIcon").GetComponent<Image>();
			Image component2 = trans.Find("ItemIcon/Icon").GetComponent<Image>();
			component.gameObject.SetActive(true);
			trans.Find("SelectedMark").gameObject.SetActive(false);
			trans.Find("ProtectedMark").gameObject.SetActive(false);
			trans.Find("InteractiveMask").gameObject.SetActive(false);
			trans.Find("NotEnough").gameObject.SetActive(false);
			trans.Find("Star").gameObject.SetActive(false);
			trans.Find("StigmataType").gameObject.SetActive(false);
			trans.Find("UnidentifyText").gameObject.SetActive(false);
			trans.Find("QuestionMark").gameObject.SetActive(false);
			trans.Find("ItemIcon").GetComponent<Image>().color = Color.white;
			trans.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[0]);
			if (rewardUIData.rewardType == ResourceType.Item)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(rewardUIData.itemID, rewardUIData.level);
				dummyStorageDataItem.number = rewardUIData.value;
				MonoItemIconButton component3 = trans.GetComponent<MonoItemIconButton>();
				component3.SetupView(dummyStorageDataItem);
				component3.SetClickCallback(OnItemClick);
			}
			else
			{
				component2.sprite = rewardUIData.GetIconSprite();
				trans.Find("Text").GetComponent<Text>().text = "×" + rewardUIData.value;
			}
		}

		private void OnItemClick(StorageDataItemBase itemData, bool selected)
		{
			UIUtil.ShowItemDetail(itemData);
		}
	}
}
