using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class RedeemDialogContext : BaseDialogContext
	{
		public enum RedeemStatus
		{
			Error = 0,
			ShowInfo = 1,
			RedeemSuccess = 2
		}

		private const string DROP_ITEM_ANIMATION_NAME = "DropItemScale10";

		private string _redeemCode;

		private RedeemStatus _redeemStatus;

		private GetRedeemCodeInfoRsp _redeemInfo;

		private string _errorCode;

		private SequenceAnimationManager _animationManager;

		private List<RewardUIData> _redeemRewardList;

		public RedeemDialogContext(string redeemCode, RedeemStatus status, GetRedeemCodeInfoRsp redeemInfo = null, string errorCode = null)
		{
			config = new ContextPattern
			{
				contextName = "RedeemDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/RedeemDialog"
			};
			_redeemCode = redeemCode;
			_redeemStatus = status;
			_redeemInfo = redeemInfo;
			_errorCode = errorCode;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 214)
			{
				OnExchangeRedeemCodeRsp(pkt.getData<ExchangeRedeemCodeRsp>());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/Error/BackBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/Info/DoubleButton/CancelBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/Info/DoubleButton/OKBtn").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/Complete/BackBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			switch (_redeemStatus)
			{
			case RedeemStatus.Error:
				SetupErrorContext();
				break;
			case RedeemStatus.ShowInfo:
				SetupRedeemInfo();
				break;
			case RedeemStatus.RedeemSuccess:
				SetupRedeemSuccess();
				break;
			}
			return false;
		}

		private bool OnExchangeRedeemCodeRsp(ExchangeRedeemCodeRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				SetupRedeemSuccess();
			}
			else
			{
				_errorCode = LocalizationGeneralLogic.GetNetworkErrCodeOutput((object)(ExchangeRedeemCodeRsp.Retcode)1) + '\n';
				_errorCode += LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				SetupErrorContext();
			}
			return false;
		}

		public void OnOKButtonCallBack()
		{
			Singleton<NetworkManager>.Instance.RequestExchangeRedeemCode(_redeemCode);
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public void Close()
		{
			Destroy();
		}

		private void SetupErrorContext()
		{
			base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(true);
			base.view.transform.Find("Dialog/Content/Info").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/Complete").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/Error/DescText").GetComponent<Text>().text = _errorCode;
		}

		private void SetupRedeemInfo()
		{
			base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/Info").gameObject.SetActive(true);
			base.view.transform.Find("Dialog/Content/Complete").gameObject.SetActive(false);
			Transform transform = base.view.transform.Find("Dialog/Content/Info");
			transform.Find("SubTitle").GetComponent<Text>().text = _redeemInfo.desc;
			SetupRewardList();
			transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(OnScrollChange, _redeemRewardList.Count, new Vector2(0f, 0f));
			_animationManager = new SequenceAnimationManager();
			_animationManager.AddAllChildrenInTransform(transform.Find("ScrollView/Content"));
			_animationManager.StartPlay(0.1f);
		}

		private void SetupRedeemSuccess()
		{
			base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/Info").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/Complete").gameObject.SetActive(true);
		}

		private void OnScrollChange(Transform trans, int index)
		{
			RewardUIData rewardUIData = _redeemRewardList[index];
			if (rewardUIData.rewardType == ResourceType.Item)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(rewardUIData.itemID, rewardUIData.level);
				dummyStorageDataItem.number = rewardUIData.value;
				trans.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, OnDropItemButtonClick, true);
			}
			else
			{
				HideRewardTransSomePart(trans);
				trans.GetComponent<MonoLevelDropIconButton>().Clear();
				trans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = rewardUIData.GetIconSprite();
				trans.Find("BG/Desc").GetComponent<Text>().text = "x" + rewardUIData.value;
			}
			trans.GetComponent<MonoAnimationinSequence>().animationName = "DropItemScale10";
		}

		private void SetupRewardList()
		{
			_redeemRewardList = new List<RewardUIData>();
			if (_redeemInfo.reward_list.Count < 1)
			{
				return;
			}
			proto.RewardData val = _redeemInfo.reward_list[0];
			if (val.exp != 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int)val.exp);
				_redeemRewardList.Add(playerExpData);
			}
			if (val.scoin != 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData((int)val.scoin);
				_redeemRewardList.Add(scoinData);
			}
			if (val.hcoin != 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData((int)val.hcoin);
				_redeemRewardList.Add(hcoinData);
			}
			if (val.stamina != 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData((int)val.stamina);
				_redeemRewardList.Add(staminaData);
			}
			if (val.skill_point != 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData((int)val.skill_point);
				_redeemRewardList.Add(skillPointData);
			}
			if (val.friends_point != 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData((int)val.friends_point);
				_redeemRewardList.Add(friendPointData);
			}
			foreach (RewardItemData item2 in val.item_list)
			{
				RewardUIData item = new RewardUIData(ResourceType.Item, (int)item2.num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int)item2.id, (int)item2.level);
				_redeemRewardList.Add(item);
			}
		}

		private void HideRewardTransSomePart(Transform rewardTrans)
		{
			rewardTrans.Find("BG/UnidentifyText").gameObject.SetActive(false);
			rewardTrans.Find("NewMark").gameObject.SetActive(false);
			rewardTrans.Find("AvatarStar").gameObject.SetActive(false);
			rewardTrans.Find("Star").gameObject.SetActive(false);
			rewardTrans.Find("StigmataType").gameObject.SetActive(false);
			rewardTrans.Find("FragmentIcon").gameObject.SetActive(false);
		}

		private void OnDropItemButtonClick(StorageDataItemBase itemData)
		{
			UIUtil.ShowItemDetail(itemData, true);
		}
	}
}
