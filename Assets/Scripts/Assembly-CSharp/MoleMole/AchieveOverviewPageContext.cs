using System.Collections.Generic;
using System.Linq;
using MoleMole.Config;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class AchieveOverviewPageContext : BasePageContext
	{
		private MonoGridScroller _achieveScroller;

		private MonoScrollerFadeManager _fadeMgr;

		private Dictionary<int, RectTransform> _dictBeforeFetch;

		public AchieveOverviewPageContext()
		{
			config = new ContextPattern
			{
				contextName = "AchieveOverviewPageContext",
				viewPrefabPath = "UI/Menus/Page/Achieve/AchieveOverviewPage"
			};
		}

		protected override void BindViewCallbacks()
		{
		}

		public override bool OnNotify(Notify ntf)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			if (ntf.type == NotifyTypes.MissionUpdated)
			{
				SetupView();
			}
			else if (ntf.type == NotifyTypes.MissionRewardGot)
			{
				OnMissionRewardGot((GetMissionRewardRsp)ntf.body);
			}
			return false;
		}

		protected override bool SetupView()
		{
			if (base.view == null || base.view.transform == null)
			{
				return false;
			}
			FetchWidget();
			SetupAchieveInfoScroller();
			return false;
		}

		private void FetchWidget()
		{
			_achieveScroller = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoGridScroller>();
		}

		private void SetupAchieveInfoScroller()
		{
			Dictionary<int, MissionDataItem> missionDict = Singleton<MissionModule>.Instance.GetMissionDict();
			if (missionDict == null)
			{
				return;
			}
			List<MissionDataItem> fromList = missionDict.Values.ToList();
			List<MissionDataItem> displayList = SelectAchieveToDisplay(fromList);
			displayList.Sort(delegate(MissionDataItem lhs, MissionDataItem rhs)
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Invalid comparison between Unknown and I4
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Invalid comparison between Unknown and I4
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Invalid comparison between Unknown and I4
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Invalid comparison between Unknown and I4
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Invalid comparison between Unknown and I4
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Invalid comparison between Unknown and I4
				int num = 0;
				int num2 = 0;
				if ((int)lhs.status == 3)
				{
					num = 0;
				}
				else if ((int)lhs.status == 2)
				{
					num = 1;
				}
				else if ((int)lhs.status == 5)
				{
					num = 2;
				}
				if ((int)rhs.status == 3)
				{
					num2 = 0;
				}
				else if ((int)rhs.status == 2)
				{
					num2 = 1;
				}
				else if ((int)rhs.status == 5)
				{
					num2 = 2;
				}
				return (num != num2) ? (num - num2) : (lhs.id - rhs.id);
			});
			if (_achieveScroller == null)
			{
				return;
			}
			_achieveScroller.Init(delegate(Transform trans, int index)
			{
				MonoAchieveInfo component = trans.GetComponent<MonoAchieveInfo>();
				if (!(component == null))
				{
					component.SetupView(displayList[index]);
					component.SetupFetchRewardButtonClickCallback(OnFetchRewardButtonClicked);
				}
			}, displayList.Count);
			_fadeMgr = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_fadeMgr.Init(_achieveScroller.GetItemDict(), _dictBeforeFetch, IsMissionEqual);
			_fadeMgr.Play();
			_dictBeforeFetch = null;
		}

		private List<MissionDataItem> SelectAchieveToDisplay(List<MissionDataItem> fromList)
		{
			List<MissionDataItem> list = new List<MissionDataItem>();
			int i = 0;
			for (int count = fromList.Count; i < count; i++)
			{
				LinearMissionData linearMissionData = LinearMissionDataReader.TryGetLinearMissionDataByKey(fromList[i].metaData.id);
				if (linearMissionData != null && linearMissionData.IsAchievement == 1)
				{
					list.Add(fromList[i]);
				}
			}
			return list;
		}

		private void OnFetchRewardButtonClicked(MissionDataItem data)
		{
			if (data == null)
			{
				return;
			}
			Singleton<NetworkManager>.Instance.RequestGetMissionReward((uint)data.id);
			Transform transform = base.view.transform.Find("MissionList/ScrollView");
			if (transform == null)
			{
				return;
			}
			Dictionary<int, RectTransform> itemDict = transform.GetComponent<MonoGridScroller>().GetItemDict();
			if (itemDict != null)
			{
				_dictBeforeFetch = itemDict.ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			}
		}

		private void OnMissionRewardGot(GetMissionRewardRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				AchieveRewardGotContext dialogContext = new AchieveRewardGotContext(rsp.reward_list);
				Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
			}
		}

		private bool IsMissionEqual(RectTransform missionNew, RectTransform missionOld)
		{
			if (missionNew == null || missionOld == null)
			{
				return false;
			}
			MonoAchieveInfo component = missionOld.GetComponent<MonoAchieveInfo>();
			MonoAchieveInfo component2 = missionNew.GetComponent<MonoAchieveInfo>();
			return component2.id == component.id;
		}
	}
}
