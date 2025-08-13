using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class SignInDialogContext : BaseSequenceDialogContext
	{
		private int _dayNum;

		private int _monthNum;

		private int _daysOfMonth;

		private List<RewardData> _signInRewardItemList;

		private GetSignInRewardStatusRsp _signInRewardStatus;

		public SignInDialogContext(GetSignInRewardStatusRsp rsp)
		{
			config = new ContextPattern
			{
				contextName = "SignInDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/SignInDialog",
				cacheType = ViewCacheType.DontCache
			};
			_signInRewardStatus = rsp;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 122:
				return OnGetSignInRewardStatusRsp(pkt.getData<GetSignInRewardStatusRsp>());
			case 124:
				return OnGetSignInRewardRsp(pkt.getData<GetSignInRewardRsp>());
			default:
				return false;
			}
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/GetRewardBtn").GetComponent<Button>(), OnGetRewardBtnClick);
		}

		protected override bool SetupView()
		{
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			DateTime now = TimeUtil.Now;
			_dayNum = (int)_signInRewardStatus.next_sign_in_day;
			_monthNum = now.Month;
			_daysOfMonth = DateTime.DaysInMonth(now.Year, now.Month);
			_signInRewardItemList = new List<RewardData>();
			if (_monthNum % 2 == 0)
			{
				List<EvenSignInRewardMetaData> itemList = EvenSignInRewardMetaDataReader.GetItemList();
				for (int i = 0; i < itemList.Count && i < _daysOfMonth; i++)
				{
					EvenSignInRewardMetaData evenSignInRewardMetaData = itemList[i];
					_signInRewardItemList.Add(RewardDataReader.GetRewardDataByKey(evenSignInRewardMetaData.rewardItemID));
				}
			}
			else
			{
				List<OddSignInRewardMetaData> itemList2 = OddSignInRewardMetaDataReader.GetItemList();
				for (int j = 0; j < itemList2.Count && j < _daysOfMonth; j++)
				{
					OddSignInRewardMetaData oddSignInRewardMetaData = itemList2[j];
					_signInRewardItemList.Add(RewardDataReader.GetRewardDataByKey(oddSignInRewardMetaData.rewardItemID));
				}
			}
			base.view.transform.Find("Dialog/Content/Title/Month").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.MonthTextIDList[_monthNum]);
			base.view.transform.Find("Dialog/Content/Title/DayNum").GetComponent<Text>().text = _dayNum.ToString();
			base.view.transform.Find("Dialog/Content/MonthPanel").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/GetRewardBtn").gameObject.SetActive(false);
			if (_signInRewardStatus == null || (int)_signInRewardStatus.retcode != 0)
			{
				LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext(122);
				Singleton<MainUIManager>.Instance.ShowWidget(widget);
			}
			else
			{
				SetupTheRewardPanel();
			}
			return false;
		}

		private void OnGetRewardBtnClick()
		{
			if (_signInRewardStatus != null && _signInRewardStatus.is_need_sign_in)
			{
				Singleton<NetworkManager>.Instance.RequestGetSignInReward();
			}
		}

		private bool OnGetSignInRewardStatusRsp(GetSignInRewardStatusRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				_signInRewardStatus = rsp;
				if (rsp.is_need_sign_in)
				{
					SetupTheRewardPanel();
				}
			}
			return false;
		}

		private bool OnGetSignInRewardRsp(GetSignInRewardRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				SignInRewardGotDialogContext signInRewardGotDialogContext = new SignInRewardGotDialogContext(rsp, (int)_signInRewardStatus.next_sign_in_day);
				signInRewardGotDialogContext.RegisterCallBack(OnRewardGotConfirm);
				Singleton<MainUIManager>.Instance.ShowDialog(signInRewardGotDialogContext);
				Singleton<NetworkManager>.Instance.RequestGetSignInRewardStatus();
			}
			else
			{
				Destroy();
			}
			return false;
		}

		private void SetupTheRewardPanel()
		{
			if (_signInRewardStatus == null)
			{
				LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext(122);
				Singleton<MainUIManager>.Instance.ShowWidget(widget);
				return;
			}
			base.view.transform.Find("Dialog/Content/MonthPanel").gameObject.SetActive(true);
			base.view.transform.Find("Dialog/Content/GetRewardBtn").gameObject.SetActive(true);
			MonoGridScroller component = base.view.transform.Find("Dialog/Content/MonthPanel/ScrollView").GetComponent<MonoGridScroller>();
			int num = ((_daysOfMonth % 6 != 0) ? (_daysOfMonth / 6 + 1) : (_daysOfMonth / 6));
			int num2 = (int)(_signInRewardStatus.next_sign_in_day - 1) / 6;
			component.Init(OnScrollerChanged, _signInRewardItemList.Count, new Vector2(0f, 1f - (float)num2 / (float)num));
		}

		private void OnScrollerChanged(Transform trans, int index)
		{
			RewardData rewardData = _signInRewardItemList[index];
			MonoSignInRewardItemIconButton component = trans.GetComponent<MonoSignInRewardItemIconButton>();
			component.SetupView(rewardData, index + 1 < _signInRewardStatus.next_sign_in_day, index + 1 == _signInRewardStatus.next_sign_in_day);
			component.SetClickCallback(OnItemClick);
		}

		private void OnItemClick(RewardData rewardData)
		{
		}

		private void OnRewardGotConfirm()
		{
			Destroy();
		}
	}
}
