using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class EndlessSettlementDialogContext : BaseDialogContext
	{
		private const string PROMOTE_TITLE_ID = "Menu_Desc_EndlessPromoteTitle";

		private const string PROMOTE_SUBTITLE_ID = "Menu_Desc_EndlessPromoteSubtitle";

		private const string PROMOTE_LABEL_ID = "Menu_Title_EndlessPromote";

		private const string STAY_TITLE_ID = "Menu_Desc_EndlessStayTitle";

		private const string STAY_SUBTITLE_ID = "Menu_Desc_EndlessStaySubtitle";

		private const string STAY_LABEL_ID = "Menu_Title_EndlessStay";

		private const string DEMOTE_TITLE_ID = "Menu_Desc_EndlessDemoteTitle";

		private const string DEMOTE_SUBTITLE_ID = "Menu_Desc_EndlessDemoteSubtitle";

		private const string DEMOTE_LABEL_ID = "Menu_Title_EndlessDemote";

		private const string PROMOTE_EFFECT_PREFAT_PATH = "UI/Menus/Widget/Storage/UpgradingBigSuccess";

		private const string PROMOTE_TO_MAX_EFFECT_PREFAT_PATH = "UI/Menus/Widget/Storage/UpgradingLargeSuccess";

		private const string STAY_EFFECT_PREFAT_PATH = "UI/Menus/Widget/Storage/UpgradingBigSuccess";

		private const string DEMOTE_EFFECT_PREFAT_PATH = "UI/Menus/Widget/Storage/UpgradingBigSuccess";

		private GetLastEndlessRewardDataRsp _rewardData;

		private List<RewardUIData> _gotRewardList = new List<RewardUIData>();

		private Color _groupBGColor;

		public EndlessSettlementDialogContext(GetLastEndlessRewardDataRsp rewardData)
		{
			config = new ContextPattern
			{
				contextName = "EndlessSettlementDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/EndlessSettlement"
			};
			_rewardData = rewardData;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.EndlessSettlementAnimationEnd)
			{
				return PlayEffect();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), OnBGClick);
		}

		protected override bool SetupView()
		{
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Expected I4, but got Unknown
			base.view.transform.Find("Btn").gameObject.SetActive(false);
			_groupBGColor = Miscs.ParseColor(MiscData.Config.EndlessGroupBGColor[(int)_rewardData.group_level]);
			SetupRewardTilte();
			base.view.transform.Find("Dialog/Content/GroupPanel/GroupBGL/GroupIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.EndlessGroupSelectPrefabPath[(int)_rewardData.group_level]);
			Transform transform = base.view.transform.Find("Dialog/Content/GetProps");
			InitRewardList();
			for (int i = 1; i <= 3; i++)
			{
				Transform transform2 = transform.Find(i.ToString());
				if (i > _gotRewardList.Count)
				{
					transform2.gameObject.SetActive(false);
					continue;
				}
				RewardUIData rewardUIData = _gotRewardList[i - 1];
				transform2.Find("Image").GetComponent<Image>().sprite = rewardUIData.GetIconSprite();
				transform2.Find("Num").GetComponent<Text>().text = rewardUIData.value.ToString();
			}
			Transform transform3 = base.view.transform.Find("Dialog/Content/GroupPanel/Ranking");
			transform3.Find("Up").gameObject.SetActive(false);
			transform3.Find("Flat").gameObject.SetActive(false);
			transform3.Find("Down").gameObject.SetActive(false);
			EndlessRewardType reward_type = _rewardData.reward_type;
			switch ((int)reward_type - 1)
			{
			case 0:
				transform3.Find("Up").gameObject.SetActive(true);
				break;
			case 1:
				transform3.Find("Flat").gameObject.SetActive(true);
				break;
			case 2:
				transform3.Find("Down").gameObject.SetActive(true);
				break;
			}
			base.view.transform.Find("Dialog/Content/GroupPanel/Label").gameObject.SetActive(false);
			return false;
		}

		private bool PlayEffect()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected I4, but got Unknown
			string path = string.Empty;
			int num = (int)_rewardData.group_level;
			EndlessRewardType reward_type = _rewardData.reward_type;
			switch ((int)reward_type - 1)
			{
			case 0:
			{
				int count = EndlessGroupMetaDataReader.GetItemList().Count;
				if (_rewardData.group_level >= count - 1)
				{
					path = "UI/Menus/Widget/Storage/UpgradingLargeSuccess";
					num = count;
				}
				else
				{
					path = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
					num++;
				}
				break;
			}
			case 1:
				path = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
				break;
			case 2:
				path = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
				if (num > 1)
				{
					num--;
				}
				break;
			}
			Transform transform = Object.Instantiate(Resources.Load<GameObject>(path)).transform;
			transform.SetParent(base.view.transform, false);
			base.view.transform.Find("Dialog/Content/GroupPanel/GroupBGL/GroupIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.EndlessGroupSelectPrefabPath[num]);
			base.view.transform.Find("Btn").gameObject.SetActive(true);
			return false;
		}

		public void OnBGClick()
		{
			Destroy();
		}

		private void SetupRewardTilte()
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected I4, but got Unknown
			base.view.transform.Find("Dialog/Content/Title/GradientMask/Gradient").GetComponent<Gradient>().topColor = _groupBGColor;
			base.view.transform.Find("Dialog/Content/GroupPanel/GroupBGL").GetComponent<Image>().color = _groupBGColor;
			base.view.transform.Find("Dialog/Content/GroupPanel/GroupName").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey((int)_rewardData.group_level).groupName);
			EndlessRewardType reward_type = _rewardData.reward_type;
			switch ((int)reward_type - 1)
			{
			case 0:
				base.view.transform.Find("Dialog/Content/Title/MainTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessPromoteTitle");
				base.view.transform.Find("Dialog/Content/Title/SubTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessPromoteSubtitle");
				base.view.transform.Find("Dialog/Content/GroupPanel/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_EndlessPromote");
				break;
			case 1:
				base.view.transform.Find("Dialog/Content/Title/MainTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessStayTitle");
				base.view.transform.Find("Dialog/Content/Title/SubTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessStaySubtitle");
				base.view.transform.Find("Dialog/Content/GroupPanel/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_EndlessStay");
				break;
			case 2:
				base.view.transform.Find("Dialog/Content/Title/MainTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessDemoteTitle");
				base.view.transform.Find("Dialog/Content/Title/SubTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessDemoteSubtitle");
				base.view.transform.Find("Dialog/Content/GroupPanel/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_EndlessDemote");
				break;
			}
		}

		private void InitRewardList()
		{
			proto.RewardData val = _rewardData.reward_list[0];
			_gotRewardList.Clear();
			if (val.exp != 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int)val.exp);
				_gotRewardList.Add(playerExpData);
			}
			if (val.scoin != 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData((int)val.scoin);
				_gotRewardList.Add(scoinData);
			}
			if (val.hcoin != 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData((int)val.hcoin);
				_gotRewardList.Add(hcoinData);
			}
			if (val.stamina != 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData((int)val.stamina);
				_gotRewardList.Add(staminaData);
			}
			if (val.skill_point != 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData((int)val.skill_point);
				_gotRewardList.Add(skillPointData);
			}
			if (val.friends_point != 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData((int)val.friends_point);
				_gotRewardList.Add(friendPointData);
			}
			foreach (RewardItemData item2 in val.item_list)
			{
				RewardUIData item = new RewardUIData(ResourceType.Item, (int)item2.num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int)item2.id, (int)item2.level);
				_gotRewardList.Add(item);
			}
			foreach (RewardUIData gotReward in _gotRewardList)
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
	}
}
