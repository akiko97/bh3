using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class SignInRewardGotDialogContext : BaseDialogContext
	{
		public delegate void OnDialogDestroy();

		private OnDialogDestroy _onDestroy;

		private GetSignInRewardRsp _signInRewardRsp;

		private int _signInTimes;

		private SequenceAnimationManager _animationManager;

		private List<RewardUIData> _missionRewardList = new List<RewardUIData>();

		public SignInRewardGotDialogContext(GetSignInRewardRsp rsp, int times = 1)
		{
			config = new ContextPattern
			{
				contextName = "SignInRewardGotDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/SignInRewardGotDialog"
			};
			_signInRewardRsp = rsp;
			_signInTimes = times;
		}

		public void RegisterCallBack(OnDialogDestroy callback)
		{
			_onDestroy = callback;
		}

		protected override bool SetupView()
		{
			_animationManager = new SequenceAnimationManager();
			SetupTitle();
			InitRewardList();
			SetupContents();
			_animationManager.AddAnimation(base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<MonoAnimationinSequence>());
			_animationManager.StartPlay(0.5f);
			Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/OKBtn").GetComponent<Button>(), OnBGBtnClick);
		}

		private void OnBGBtnClick()
		{
			Destroy();
			if (_onDestroy != null)
			{
				_onDestroy();
			}
		}

		private void SetupContents()
		{
			int count = _missionRewardList.Count;
			if (count != 0 && count == 1)
			{
				RewardUIData data = _missionRewardList[0];
				Transform line = base.view.transform.Find("Dialog/Content/RewardPanel");
				SetupLine(line, data);
			}
		}

		private void SetupTitle()
		{
			int month = TimeUtil.Now.Month;
			base.view.transform.Find("Dialog/Content/Title/Month").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.MonthTextIDList[month]);
			base.view.transform.Find("Dialog/Content/Title/DayNum").GetComponent<Text>().text = _signInTimes.ToString();
		}

		private void SetupLine(Transform line, RewardUIData data)
		{
			line.gameObject.SetActive(true);
			line.Find("Image").GetComponent<Image>().sprite = data.GetIconSprite();
			line.Find("Desc").GetComponent<Text>().text = GetDesc(data.valueLabelTextID, data.itemID);
			line.Find("Number").GetComponent<Text>().text = string.Format("×{0}", data.value);
		}

		private void InitRewardList()
		{
			if (_signInRewardRsp == null)
			{
				return;
			}
			proto.RewardData val = _signInRewardRsp.reward_list[0];
			_missionRewardList.Clear();
			if (val.exp != 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int)val.exp);
				_missionRewardList.Add(playerExpData);
			}
			if (val.scoin != 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData((int)val.scoin);
				_missionRewardList.Add(scoinData);
			}
			if (val.hcoin != 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData((int)val.hcoin);
				_missionRewardList.Add(hcoinData);
			}
			if (val.stamina != 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData((int)val.stamina);
				_missionRewardList.Add(staminaData);
			}
			if (val.skill_point != 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData((int)val.skill_point);
				_missionRewardList.Add(skillPointData);
			}
			if (val.friends_point != 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData((int)val.friends_point);
				_missionRewardList.Add(friendPointData);
			}
			foreach (RewardItemData item2 in val.item_list)
			{
				RewardUIData item = new RewardUIData(ResourceType.Item, (int)item2.num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int)item2.id, (int)item2.level);
				_missionRewardList.Add(item);
			}
			foreach (RewardUIData missionReward in _missionRewardList)
			{
			}
		}

		private string GetDesc(string textID, int id)
		{
			if (textID == RewardUIData.ITEM_ICON_TEXT_ID)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(id);
				return dummyStorageDataItem.GetDisplayTitle();
			}
			return LocalizationGeneralLogic.GetText(textID);
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.UnlockAvatar)
			{
				int avatarID = (int)ntf.body;
				Singleton<MainUIManager>.Instance.ShowDialog(new AvatarUnlockDialogContext(avatarID));
			}
			return false;
		}
	}
}
