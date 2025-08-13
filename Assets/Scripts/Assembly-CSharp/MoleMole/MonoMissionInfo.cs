using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoMissionInfo : MonoBehaviour
	{
		public const string ITEM_ICON_PREFAB_PATH = "ItemIconPrefabPath";

		private const string MATERIAL_GRAY_PATH = "Material/ImageGrayscale";

		private List<RewardUIData> missionRewardList = new List<RewardUIData>();

		[SerializeField]
		private Sprite _normalBG;

		[SerializeField]
		private Sprite _readyBG;

		private static Material _grayMat;

		private FetchRewardCallBack _onFetchRewardBtnClick;

		private GoMissionCallBack _onGoMissionBtnClick;

		private MissionDataItem _missionData;

		private UnityEventBase _leftClickEvent;

		private UnityEventBase _rightClickEvent;

		private RewardUIData _leftData;

		private RewardUIData _rightData;

		private Transform _time_root;

		private Text _timeNumber_comp;

		private Text _timeLable_comp;

		public Transform Time_root
		{
			get
			{
				if (_time_root == null)
				{
					_time_root = base.transform.Find("LeftTime");
				}
				return _time_root;
			}
		}

		public Text TimeNumber_comp
		{
			get
			{
				if (_timeNumber_comp == null)
				{
					_timeNumber_comp = base.transform.Find("LeftTime/TimeValue").GetComponent<Text>();
				}
				return _timeNumber_comp;
			}
		}

		public Text TimeLable_comp
		{
			get
			{
				if (_timeLable_comp == null)
				{
					_timeLable_comp = base.transform.Find("LeftTime/Label").GetComponent<Text>();
				}
				return _timeLable_comp;
			}
		}

		public MissionDataItem GetMissionData()
		{
			return _missionData;
		}

		public void SetupView(MissionDataItem missionData)
		{
			if (_grayMat == null)
			{
				_grayMat = Miscs.LoadResource<Material>("Material/ImageGrayscale");
			}
			_missionData = missionData;
			ClearAllViews();
			SetupBGView();
			SetupTypeView();
			SetupMissionIconView();
			SetupStatusInfoView();
			SetupProgressView();
			SetupTitleView();
			SetupRewardView();
			SetupButtonsView();
			SetupTimeView();
		}

		public void RegisterCallBacks(FetchRewardCallBack onFetch, GoMissionCallBack onGo)
		{
			_onFetchRewardBtnClick = onFetch;
			_onGoMissionBtnClick = onGo;
		}

		public void OnFetchRewardBtnClick()
		{
			if (_onFetchRewardBtnClick != null)
			{
				_onFetchRewardBtnClick(_missionData);
			}
		}

		public void OnGoMissionBtnClick()
		{
			if (_onGoMissionBtnClick != null)
			{
				_onGoMissionBtnClick(_missionData);
			}
		}

		private void SetupButtonsView()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			if ((int)_missionData.status == 3)
			{
				if (missionRewardList.Count > 0)
				{
					base.transform.Find("Buttons/Fetch").gameObject.SetActive(true);
				}
			}
			else if ((int)_missionData.status == 2 && _missionData.metaData.LinkType != 0)
			{
				base.transform.Find("Buttons/Go").gameObject.SetActive(true);
			}
		}

		private void SetupTitleView()
		{
			base.transform.Find("Title/title").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(_missionData.metaData.title);
			base.transform.Find("Title/splash").GetComponent<Image>().material = ((!IsMissionActive()) ? _grayMat : null);
			Text component = base.transform.Find("Title/description").GetComponent<Text>();
			component.text = LocalizationGeneralLogic.GetText(_missionData.metaData.description);
			string str = ((!IsMissionActive()) ? "#a0a0a0FF" : "00CAFFFF");
			Color color;
			UIUtil.TryParseHexString(str, out color);
			component.color = color;
		}

		private void SetupProgressView()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if ((int)_missionData.status == 2)
			{
				int progress = _missionData.progress;
				int totalProgress = _missionData.metaData.totalProgress;
				Transform transform = base.transform.Find("ProgressBar");
				transform.gameObject.SetActive(true);
				transform.GetComponent<MonoMaskSlider>().UpdateValue(progress, totalProgress, 0f);
				Transform transform2 = base.transform.Find("ProgressText");
				transform2.gameObject.SetActive(true);
				transform2.Find("current").GetComponent<Text>().text = progress.ToString();
				transform2.Find("total").GetComponent<Text>().text = totalProgress.ToString();
			}
		}

		private void SetupStatusInfoView()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Invalid comparison between Unknown and I4
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Invalid comparison between Unknown and I4
			if ((int)_missionData.status == 3)
			{
				base.transform.Find("StatusInfo/Finish").gameObject.SetActive(true);
			}
			else if ((int)_missionData.status == 5)
			{
				base.transform.Find("StatusInfo/Closed").gameObject.SetActive(true);
			}
			else if ((int)_missionData.status == 1)
			{
				base.transform.Find("StatusInfo/NotBegin").gameObject.SetActive(true);
			}
		}

		private void SetupMissionIconView()
		{
			Image component = base.transform.Find("MissionIcon/Color").GetComponent<Image>();
			component.sprite = Miscs.GetSpriteByPrefab(_missionData.metaData.thumb);
			Material material = ((!IsMissionActive()) ? _grayMat : null);
			component.material = material;
		}

		private void SetupBGView()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Invalid comparison between Unknown and I4
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Invalid comparison between Unknown and I4
			Image component = GetComponent<Image>();
			if (IsMissionActive())
			{
				if ((int)_missionData.status == 2)
				{
					component.sprite = _normalBG;
				}
				else if ((int)_missionData.status == 3)
				{
					component.sprite = _readyBG;
				}
				component.material = null;
			}
			else
			{
				component.sprite = _normalBG;
				component.material = _grayMat;
			}
		}

		private void SetupTypeView()
		{
			if (_missionData.metaData.type == 1)
			{
				if (_missionData.metaData.subType == 1)
				{
					base.transform.Find("TypeInfo/Branch").gameObject.SetActive(true);
				}
				else
				{
					base.transform.Find("TypeInfo/Linear").gameObject.SetActive(true);
				}
			}
			else if (_missionData.metaData.type == 2)
			{
				base.transform.Find("TypeInfo/Bounty").gameObject.SetActive(true);
			}
			else if (_missionData.metaData.type == 3)
			{
				base.transform.Find("TypeInfo/Timed").gameObject.SetActive(true);
			}
			else if (_missionData.metaData.type == 4)
			{
				base.transform.Find("TypeInfo/Touch").gameObject.SetActive(true);
			}
		}

		private void SetupRewardView()
		{
			RefreshRewardList();
			if (missionRewardList.Count == 0)
			{
				return;
			}
			RewardUIData rewardUIData = null;
			_leftData = null;
			_rightData = null;
			RewardUIData rewardUIData2;
			if (missionRewardList.Count == 1)
			{
				rewardUIData2 = null;
				rewardUIData = missionRewardList[0];
			}
			else
			{
				rewardUIData2 = missionRewardList[0];
				rewardUIData = missionRewardList[1];
			}
			_leftData = rewardUIData2;
			_rightData = rewardUIData;
			if (rewardUIData2 != null)
			{
				base.transform.Find("Rewards/Left").gameObject.SetActive(true);
				base.transform.Find("Rewards/Left/RewardItem/Icon").GetComponent<Image>().sprite = rewardUIData2.GetIconSprite();
				Text component = base.transform.Find("Rewards/Left/RewardItem/Number").GetComponent<Text>();
				component.text = string.Format("{0}", rewardUIData2.value);
				Text component2 = base.transform.Find("Rewards/Left/RewardItem/x").GetComponent<Text>();
				string str = ((!IsMissionActive()) ? "##96b1c0FF" : "43C6FCFF");
				Color color;
				UIUtil.TryParseHexString(str, out color);
				component2.color = color;
				base.transform.Find("Rewards/Left/RewardItem/Icon").GetComponent<Image>().material = ((!IsMissionActive()) ? _grayMat : null);
				if (rewardUIData2.rewardType == ResourceType.Item)
				{
					SetRarity(base.transform.Find("Rewards/Left/RewardItem"), rewardUIData2);
				}
				Button component3 = base.transform.Find("Rewards/Left/ShowDetailBtn").GetComponent<Button>();
				_leftClickEvent = component3.onClick;
				_leftClickEvent.RemoveAllListeners();
				component3.onClick.AddListener(ShowDetailDialog_Left);
			}
			if (rewardUIData != null)
			{
				base.transform.Find("Rewards/Right").gameObject.SetActive(true);
				base.transform.Find("Rewards/Right/RewardItem/Icon").GetComponent<Image>().sprite = rewardUIData.GetIconSprite();
				Text component4 = base.transform.Find("Rewards/Right/RewardItem/Number").GetComponent<Text>();
				component4.text = string.Format("{0}", rewardUIData.value);
				Text component5 = base.transform.Find("Rewards/Right/RewardItem/x").GetComponent<Text>();
				string str2 = ((!IsMissionActive()) ? "##96b1c0FF" : "43C6FCFF");
				Color color2;
				UIUtil.TryParseHexString(str2, out color2);
				component5.color = color2;
				base.transform.Find("Rewards/Right/RewardItem/Icon").GetComponent<Image>().material = ((!IsMissionActive()) ? _grayMat : null);
				if (rewardUIData.rewardType == ResourceType.Item)
				{
					SetRarity(base.transform.Find("Rewards/Right/RewardItem"), rewardUIData);
				}
				Button component6 = base.transform.Find("Rewards/Right/ShowDetailBtn").GetComponent<Button>();
				_rightClickEvent = component6.onClick;
				_rightClickEvent.RemoveAllListeners();
				component6.onClick.AddListener(ShowDetailDialog_Right);
			}
		}

		private void SetRarity(Transform tran, RewardUIData data)
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID, data.level);
			if (dummyStorageDataItem is AvatarFragmentDataItem)
			{
				return;
			}
			if (dummyStorageDataItem is AvatarCardDataItem)
			{
				tran.Find("x").gameObject.SetActive(false);
				tran.Find("Number").gameObject.SetActive(false);
				Transform transform = tran.Find("AvatarStar");
				transform.gameObject.SetActive(true);
				AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(data.itemID).avatarID);
				transform.GetComponent<MonoAvatarStar>().SetupView(dummyAvatarDataItem.star);
				return;
			}
			Transform transform2 = tran.Find("Star");
			transform2.gameObject.SetActive(true);
			int maxStar = dummyStorageDataItem.rarity;
			if (dummyStorageDataItem is WeaponDataItem)
			{
				maxStar = (dummyStorageDataItem as WeaponDataItem).GetMaxRarity();
			}
			else if (dummyStorageDataItem is StigmataDataItem)
			{
				maxStar = (dummyStorageDataItem as StigmataDataItem).GetMaxRarity();
			}
			transform2.GetComponent<MonoItemIconStar>().SetupView(dummyStorageDataItem.rarity, maxStar);
		}

		private void SetupItemFrame(Image frameImage, RewardUIData data)
		{
			switch (data.rewardType)
			{
			case ResourceType.Hcoin:
				frameImage.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ItemFrame/FrameComPurple");
				break;
			case ResourceType.Item:
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID);
				frameImage.sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[dummyStorageDataItem.rarity]);
				break;
			}
			default:
				frameImage.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ItemFrame/FrameComBlue");
				break;
			}
		}

		private void SetupTimeView()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Invalid comparison between Unknown and I4
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Invalid comparison between Unknown and I4
			if (_missionData.metaData.type == 3)
			{
				if ((int)_missionData.status == 1)
				{
					DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp((uint)_missionData.beginTime);
					Time_root.gameObject.SetActive(true);
					SetLeftTimeUI(TimeUtil.Now, dateTimeFromTimeStamp);
				}
				else if ((int)_missionData.status == 2 || (int)_missionData.status == 3)
				{
					DateTime dateTimeFromTimeStamp2 = Miscs.GetDateTimeFromTimeStamp((uint)_missionData.endTime);
					Time_root.gameObject.SetActive(true);
					SetLeftTimeUI(TimeUtil.Now, dateTimeFromTimeStamp2);
				}
				else
				{
					Time_root.gameObject.SetActive(false);
				}
			}
		}

		private void Update()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Invalid comparison between Unknown and I4
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Invalid comparison between Unknown and I4
			if (_missionData.metaData.type != 3)
			{
				return;
			}
			if ((int)_missionData.status == 1)
			{
				if (DateTime.Compare(TimeUtil.Now, Miscs.GetDateTimeFromTimeStamp((uint)_missionData.beginTime)) < 0)
				{
					DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp((uint)_missionData.beginTime);
					SetLeftTimeUI(TimeUtil.Now, dateTimeFromTimeStamp);
				}
			}
			else if (((int)_missionData.status == 2 || (int)_missionData.status == 3) && DateTime.Compare(TimeUtil.Now, Miscs.GetDateTimeFromTimeStamp((uint)_missionData.endTime)) < 0)
			{
				DateTime dateTimeFromTimeStamp2 = Miscs.GetDateTimeFromTimeStamp((uint)_missionData.endTime);
				SetLeftTimeUI(TimeUtil.Now, dateTimeFromTimeStamp2);
			}
		}

		private void SetLeftTimeUI(DateTime from, DateTime to)
		{
			string label;
			int num = Miscs.GetDiffTimeToShow(from, to, out label);
			if (num <= 0)
			{
				num = 1;
			}
			TimeNumber_comp.text = num.ToString();
			TimeLable_comp.text = label;
		}

		private bool IsTimedMissionNotBegin()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			return _missionData.metaData.type == 3 && (int)_missionData.status == 1;
		}

		private void RefreshRewardList()
		{
			missionRewardList.Clear();
			RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(_missionData.metaData.rewardId);
			if (rewardDataByKey.RewardExp > 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData(rewardDataByKey.RewardExp);
				playerExpData.itemID = rewardDataByKey.RewardID;
				missionRewardList.Add(playerExpData);
			}
			if (rewardDataByKey.RewardSCoin > 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData(rewardDataByKey.RewardSCoin);
				scoinData.itemID = rewardDataByKey.RewardID;
				missionRewardList.Add(scoinData);
			}
			if (rewardDataByKey.RewardHCoin > 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData(rewardDataByKey.RewardHCoin);
				hcoinData.itemID = rewardDataByKey.RewardID;
				missionRewardList.Add(hcoinData);
			}
			if (rewardDataByKey.RewardStamina > 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData(rewardDataByKey.RewardStamina);
				staminaData.itemID = rewardDataByKey.RewardID;
				missionRewardList.Add(staminaData);
			}
			if (rewardDataByKey.RewardSkillPoint > 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData(rewardDataByKey.RewardSkillPoint);
				skillPointData.itemID = rewardDataByKey.RewardID;
				missionRewardList.Add(skillPointData);
			}
			if (rewardDataByKey.RewardFriendPoint > 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData(rewardDataByKey.RewardFriendPoint);
				friendPointData.itemID = rewardDataByKey.RewardID;
				missionRewardList.Add(friendPointData);
			}
			if (rewardDataByKey.RewardItem1ID > 0)
			{
				RewardUIData item = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem1Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem1ID, rewardDataByKey.RewardItem1Level);
				missionRewardList.Add(item);
			}
			if (rewardDataByKey.RewardItem2ID > 0)
			{
				RewardUIData item2 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem2Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem2ID, rewardDataByKey.RewardItem2Level);
				missionRewardList.Add(item2);
			}
			if (rewardDataByKey.RewardItem3ID > 0)
			{
				RewardUIData item3 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem3Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem3ID, rewardDataByKey.RewardItem3Level);
				missionRewardList.Add(item3);
			}
			if (rewardDataByKey.RewardItem4ID > 0)
			{
				RewardUIData item4 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem4Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem4ID, rewardDataByKey.RewardItem4Level);
				missionRewardList.Add(item4);
			}
			if (rewardDataByKey.RewardItem5ID > 0)
			{
				RewardUIData item5 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem5Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem5ID, rewardDataByKey.RewardItem5Level);
				missionRewardList.Add(item5);
			}
		}

		private bool IsMissionActive()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Invalid comparison between Unknown and I4
			return (int)_missionData.status == 2 || (int)_missionData.status == 3;
		}

		private void ClearAllViews()
		{
			base.transform.Find("Buttons/Fetch").gameObject.SetActive(false);
			base.transform.Find("Buttons/Go").gameObject.SetActive(false);
			base.transform.Find("ProgressBar").gameObject.SetActive(false);
			base.transform.Find("ProgressText").gameObject.SetActive(false);
			base.transform.Find("StatusInfo/Finish").gameObject.SetActive(false);
			base.transform.Find("StatusInfo/Closed").gameObject.SetActive(false);
			base.transform.Find("StatusInfo/NotBegin").gameObject.SetActive(false);
			base.transform.Find("TypeInfo/Linear").gameObject.SetActive(false);
			base.transform.Find("TypeInfo/Branch").gameObject.SetActive(false);
			base.transform.Find("TypeInfo/Bounty").gameObject.SetActive(false);
			base.transform.Find("TypeInfo/Timed").gameObject.SetActive(false);
			base.transform.Find("TypeInfo/Touch").gameObject.SetActive(false);
			base.transform.Find("Rewards/Left").gameObject.SetActive(false);
			base.transform.Find("Rewards/Right").gameObject.SetActive(false);
			base.transform.Find("Rewards/Left/RewardItem/Star").gameObject.SetActive(false);
			base.transform.Find("Rewards/Left/RewardItem/AvatarStar").gameObject.SetActive(false);
			base.transform.Find("Rewards/Right/RewardItem/Star").gameObject.SetActive(false);
			base.transform.Find("Rewards/Right/RewardItem/AvatarStar").gameObject.SetActive(false);
			Time_root.gameObject.SetActive(false);
			base.transform.Find("Rewards/Left/RewardItem/x").gameObject.SetActive(true);
			base.transform.Find("Rewards/Left/RewardItem/Number").gameObject.SetActive(true);
			base.transform.Find("Rewards/Right/RewardItem/x").gameObject.SetActive(true);
			base.transform.Find("Rewards/Right/RewardItem/Number").gameObject.SetActive(true);
			ClearEvents();
		}

		private void ShowDetailDialog_Left()
		{
			if (_leftData != null)
			{
				ShowDetailDialog(_leftData);
			}
		}

		private void ShowDetailDialog_Right()
		{
			if (_rightData != null)
			{
				ShowDetailDialog(_rightData);
			}
		}

		private void ShowDetailDialog(RewardUIData data)
		{
			UIUtil.ShowResourceDetail(data);
		}

		private void OnDestroy()
		{
			ClearEvents();
		}

		private void ClearEvents()
		{
			if (_leftClickEvent != null)
			{
				_leftClickEvent.RemoveAllListeners();
				_leftClickEvent = null;
			}
			if (_rightClickEvent != null)
			{
				_rightClickEvent.RemoveAllListeners();
				_rightClickEvent = null;
			}
		}
	}
}
