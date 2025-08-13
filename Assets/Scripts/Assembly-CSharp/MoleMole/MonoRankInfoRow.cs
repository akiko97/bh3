using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoRankInfoRow : MonoBehaviour
	{
		private const int MAX_NUM_TOOLS_TO_SHOW = 5;

		private const int INVISIBLE_ITEM_ID = 70015;

		private EndlessPlayerData _endlessPlayerData;

		public void SetupView(int rank, EndlessPlayerData endlessData, string playerName, EndlessMainPageContext.ViewStatus viewStatus = EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
		{
			_endlessPlayerData = endlessData;
			base.transform.Find("Rank").GetComponent<Text>().text = rank.ToString();
			base.transform.Find("PlayerName").GetComponent<Text>().text = playerName;
			base.transform.Find("FloorNum").GetComponent<Text>().text = ((endlessData.progress >= 1) ? endlessData.progress.ToString() : "-");
			base.transform.Find("FloorLabel").gameObject.SetActive(endlessData.progress >= 1);
			if (viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
			{
				base.transform.Find("Me").gameObject.SetActive(rank == Singleton<EndlessModule>.Instance.CurrentRank);
			}
			else
			{
				base.transform.Find("Me").gameObject.SetActive(false);
			}
			Transform transform = base.transform.Find("ApplyedToolsList");
			List<EndlessWaitBurstBomb> wait_burst_bomb_list = endlessData.wait_burst_bomb_list;
			int i = 0;
			foreach (EndlessWaitBurstBomb item in wait_burst_bomb_list)
			{
				EndlessToolDataItem toolData = new EndlessToolDataItem((int)item.item_id);
				if (i < transform.childCount)
				{
					Transform child = transform.GetChild(i);
					child.gameObject.SetActive(true);
					child.GetComponent<MonoAppliedEndlessTool>().SetupView(toolData, item.burst_time, null);
					i++;
				}
			}
			List<EndlessWaitEffectItem> wait_effect_item_list = endlessData.wait_effect_item_list;
			foreach (EndlessWaitEffectItem item2 in wait_effect_item_list)
			{
				EndlessToolDataItem endlessToolDataItem = new EndlessToolDataItem((int)item2.item_id);
				if (endlessToolDataItem.ShowIcon && i < transform.childCount)
				{
					Transform child2 = transform.GetChild(i);
					child2.gameObject.SetActive(true);
					child2.GetComponent<MonoAppliedEndlessTool>().SetupView(endlessToolDataItem, item2.expire_time, OnEndlessToolTimeOut);
					i++;
				}
			}
			if (viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup && Singleton<EndlessModule>.Instance.PlayerInvisible((int)endlessData.uid))
			{
				EndlessToolDataItem endlessToolDataItem2 = new EndlessToolDataItem(70015);
				if (endlessToolDataItem2.ShowIcon && i < transform.childCount)
				{
					Transform child3 = transform.GetChild(i);
					child3.gameObject.SetActive(true);
					child3.GetComponent<MonoAppliedEndlessTool>().SetupView(endlessToolDataItem2, endlessData.hidden_expire_time, OnEndlessToolTimeOut);
					i++;
				}
			}
			for (; i < transform.childCount; i++)
			{
				Transform child4 = transform.GetChild(i);
				child4.gameObject.SetActive(false);
			}
		}

		private void OnEndlessToolTimeOut(int itemID)
		{
			_endlessPlayerData.wait_effect_item_list.RemoveAll((EndlessWaitEffectItem item) => item.item_id == itemID);
			_endlessPlayerData.wait_burst_bomb_list.RemoveAll((EndlessWaitBurstBomb item) => item.item_id == itemID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessAppliedToolChange));
		}
	}
}
