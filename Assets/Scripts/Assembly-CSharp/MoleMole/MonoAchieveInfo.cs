using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAchieveInfo : MonoBehaviour
	{
		public Image background;

		public GameObject iconGameObject;

		public Text title;

		public Text description;

		public GameObject[] rewardObjects;

		public Button fetchButton;

		public Text progressPercentageText;

		public MonoMaskSlider progressSlider;

		public GameObject succIcon;

		public GameObject fetchIcon;

		public UnityAction<MissionDataItem> _fetchRewardClicked;

		private MissionDataItem _missionDataItem;

		private List<RewardUIData> achieveRewardList = new List<RewardUIData>();

		public int id
		{
			get
			{
				if (_missionDataItem == null)
				{
					return 0;
				}
				return _missionDataItem.id;
			}
		}

		public void SetupView(MissionDataItem item)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Invalid comparison between Unknown and I4
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Invalid comparison between Unknown and I4
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Invalid comparison between Unknown and I4
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Invalid comparison between Unknown and I4
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Invalid comparison between Unknown and I4
			_missionDataItem = item;
			title.text = LocalizationGeneralLogic.GetText(item.metaData.title);
			description.text = LocalizationGeneralLogic.GetText(item.metaData.description);
			fetchButton.gameObject.SetActive((int)item.status == 3);
			background.color = (((int)item.status != 5) ? Color.white : Color.gray);
			fetchIcon.SetActive((int)item.status == 5);
			int num = (((int)item.status != 3 && (int)item.status != 5) ? ((int)((float)item.progress * 100f / (float)item.metaData.totalProgress)) : 100);
			progressPercentageText.text = num + "%";
			progressSlider.UpdateValue((float)num * 0.01f, 1f, 0f);
			RefreshRewardList(item);
			if (!string.IsNullOrEmpty(item.metaData.thumb) && iconGameObject != null)
			{
				GameObject gameObject = Resources.Load<GameObject>(item.metaData.thumb);
				if (gameObject != null)
				{
					GameObject gameObject2 = Object.Instantiate(gameObject);
					gameObject2.transform.SetParent(iconGameObject.transform.parent);
					RectTransform rectTransform = gameObject2.transform as RectTransform;
					RectTransform rectTransform2 = iconGameObject.transform as RectTransform;
					if (rectTransform != null && rectTransform2 != null)
					{
						rectTransform.localPosition = rectTransform2.localPosition;
						rectTransform.localRotation = rectTransform2.localRotation;
						rectTransform.localScale = rectTransform2.localScale;
					}
					Object.DestroyImmediate(iconGameObject);
					iconGameObject = gameObject2;
				}
			}
			int i = 0;
			for (int num2 = rewardObjects.Length; i < num2; i++)
			{
				if (i < achieveRewardList.Count)
				{
					rewardObjects[i].SetActive(true);
					SetupRewardView(achieveRewardList[i], rewardObjects[i].transform);
				}
				else
				{
					rewardObjects[i].SetActive(false);
				}
			}
		}

		public void SetupRewardView(RewardUIData data, Transform rewardTrans)
		{
			Text component = rewardTrans.Find("Num/Number").GetComponent<Text>();
			Image component2 = rewardTrans.Find("Icon").GetComponent<Image>();
			Image component3 = rewardTrans.Find("BG").GetComponent<Image>();
			MonoItemIconStar component4 = rewardTrans.Find("Stars").GetComponent<MonoItemIconStar>();
			component2.sprite = data.GetIconSprite();
			component.text = data.value.ToString();
			bool flag = data.rewardType == ResourceType.Item;
			component4.gameObject.SetActive(flag);
			if (flag)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID);
				string hexString = MiscData.Config.ItemRarityColorList[dummyStorageDataItem.rarity];
				component3.color = Miscs.ParseColor(hexString);
			}
			BindViewCallback(rewardTrans.Find("Button").GetComponent<Button>(), delegate
			{
				ShowRewardDetail(data);
			});
		}

		private void BindViewCallback(Button button, UnityAction callback)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(callback);
		}

		private void ShowRewardDetail(RewardUIData data)
		{
			UIUtil.ShowResourceDetail(data);
		}

		private void RefreshRewardList(MissionDataItem item)
		{
			achieveRewardList.Clear();
			RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(item.metaData.rewardId);
			if (rewardDataByKey.RewardExp > 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData(rewardDataByKey.RewardExp);
				playerExpData.itemID = rewardDataByKey.RewardID;
				achieveRewardList.Add(playerExpData);
			}
			if (rewardDataByKey.RewardSCoin > 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData(rewardDataByKey.RewardSCoin);
				scoinData.itemID = rewardDataByKey.RewardID;
				achieveRewardList.Add(scoinData);
			}
			if (rewardDataByKey.RewardHCoin > 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData(rewardDataByKey.RewardHCoin);
				hcoinData.itemID = rewardDataByKey.RewardID;
				achieveRewardList.Add(hcoinData);
			}
			if (rewardDataByKey.RewardStamina > 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData(rewardDataByKey.RewardStamina);
				staminaData.itemID = rewardDataByKey.RewardID;
				achieveRewardList.Add(staminaData);
			}
			if (rewardDataByKey.RewardSkillPoint > 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData(rewardDataByKey.RewardSkillPoint);
				skillPointData.itemID = rewardDataByKey.RewardID;
				achieveRewardList.Add(skillPointData);
			}
			if (rewardDataByKey.RewardFriendPoint > 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData(rewardDataByKey.RewardFriendPoint);
				friendPointData.itemID = rewardDataByKey.RewardID;
				achieveRewardList.Add(friendPointData);
			}
			if (rewardDataByKey.RewardItem1ID > 0)
			{
				RewardUIData item2 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem1Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem1ID, rewardDataByKey.RewardItem1Level);
				achieveRewardList.Add(item2);
			}
			if (rewardDataByKey.RewardItem2ID > 0)
			{
				RewardUIData item3 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem2Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem2ID, rewardDataByKey.RewardItem2Level);
				achieveRewardList.Add(item3);
			}
			if (rewardDataByKey.RewardItem3ID > 0)
			{
				RewardUIData item4 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem3Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem3ID, rewardDataByKey.RewardItem3Level);
				achieveRewardList.Add(item4);
			}
			if (rewardDataByKey.RewardItem4ID > 0)
			{
				RewardUIData item5 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem4Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem4ID, rewardDataByKey.RewardItem4Level);
				achieveRewardList.Add(item5);
			}
			if (rewardDataByKey.RewardItem5ID > 0)
			{
				RewardUIData item6 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem5Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem5ID, rewardDataByKey.RewardItem5Level);
				achieveRewardList.Add(item6);
			}
		}

		public void SetupFetchRewardButtonClickCallback(UnityAction<MissionDataItem> callback)
		{
			_fetchRewardClicked = callback;
		}

		public void OnFetchButtonClicked()
		{
			if (_fetchRewardClicked != null)
			{
				_fetchRewardClicked(_missionDataItem);
			}
		}

		private void OnDestroy()
		{
			background = null;
			iconGameObject = null;
			title = null;
			description = null;
			rewardObjects = null;
			fetchButton = null;
			progressPercentageText = null;
			progressSlider = null;
			succIcon = null;
			fetchIcon = null;
		}
	}
}
