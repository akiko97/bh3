using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoRankButton : MonoBehaviour
	{
		private const float CHECK_INTERVAL = 60f;

		private const int MAX_NUM_TOOLS_TO_SHOW = 5;

		private EndlessPlayerData _endlessPlayerData;

		private EndlessToolDataItem _selectToolData;

		private Action<EndlessPlayerData> _clickCallBack;

		private DateTime _frozenEndTime;

		private float _checkTimer = 60f;

		private bool _interactable;

		public void SetupView(int rank, EndlessPlayerData endlessData, string playerName, bool isSelect = false, Action<EndlessPlayerData> buttonClickCallback = null, EndlessToolDataItem selectToolData = null)
		{
			_endlessPlayerData = endlessData;
			_clickCallBack = buttonClickCallback;
			_selectToolData = selectToolData;
			_interactable = selectToolData != null && !selectToolData.ApplyToSelf;
			base.transform.Find("Me").gameObject.SetActive(rank == Singleton<EndlessModule>.Instance.CurrentRank);
			base.transform.Find("Rank").GetComponent<Text>().text = rank.ToString();
			base.transform.Find("PlayerName").GetComponent<Text>().text = playerName;
			base.transform.Find("FloorNum").GetComponent<Text>().text = ((endlessData.progress >= 1) ? endlessData.progress.ToString() : "-");
			base.transform.Find("FloorLabel").gameObject.SetActive(endlessData.progress >= 1);
			List<EndlessToolDataItem> list = new List<EndlessToolDataItem>();
			List<EndlessWaitEffectItem> wait_effect_item_list = _endlessPlayerData.wait_effect_item_list;
			foreach (EndlessWaitEffectItem item in wait_effect_item_list)
			{
				EndlessToolDataItem endlessToolDataItem = new EndlessToolDataItem((int)item.item_id);
				if (endlessToolDataItem.ShowIcon)
				{
					list.Add(endlessToolDataItem);
				}
			}
			if (list.Count > 5)
			{
				list = list.GetRange(list.Count - 5, 5);
			}
			Transform transform = base.transform.Find("ApplyedToolsList");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (i >= list.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				child.gameObject.SetActive(true);
				child.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(list[list.Count - i - 1].GetSmallIconPath());
			}
			SetFrozenTime();
			base.transform.Find("SelectedMask").gameObject.SetActive(isSelect);
			CheckInteractable();
		}

		private void Update()
		{
			if (_endlessPlayerData != null)
			{
				_checkTimer += Time.deltaTime;
				if (!(_checkTimer < 60f))
				{
					_checkTimer = 0f;
					SetFrozenTime();
					CheckInteractable();
				}
			}
		}

		public void OnClick()
		{
			if (_clickCallBack != null)
			{
				_clickCallBack(_endlessPlayerData);
			}
		}

		private void SetFrozenTime()
		{
			_frozenEndTime = Singleton<EndlessModule>.Instance.GetFrozenEndTime((int)_endlessPlayerData.uid);
			base.transform.Find("FrozenInfo").gameObject.SetActive(_frozenEndTime > TimeUtil.Now);
			if (_frozenEndTime > TimeUtil.Now)
			{
				SetTheRemainTime(_frozenEndTime - TimeUtil.Now);
			}
		}

		private void CheckInteractable()
		{
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Invalid comparison between Unknown and I4
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Invalid comparison between Unknown and I4
			if (!_interactable || _endlessPlayerData == null)
			{
				base.transform.Find("BlockPanel").gameObject.SetActive(true);
				return;
			}
			bool flag = true;
			DateTime dateTime = TimeUtil.Now.AddDays(-1.0);
			if (_endlessPlayerData.hidden_expire_timeSpecified)
			{
				dateTime = Miscs.GetDateTimeFromTimeStamp(_endlessPlayerData.hidden_expire_time);
			}
			if (_endlessPlayerData.uid == (uint)Singleton<PlayerModule>.Instance.playerData.userId)
			{
				flag = false;
				base.transform.Find("FrozenInfo").gameObject.SetActive(false);
			}
			else if (_frozenEndTime > TimeUtil.Now || (_endlessPlayerData.hidden_expire_timeSpecified && dateTime > TimeUtil.Now))
			{
				flag = false;
			}
			if (_endlessPlayerData.progress < 2 && _selectToolData != null && (int)_selectToolData.ToolType == 4)
			{
				flag = false;
			}
			if (_selectToolData != null && (int)_selectToolData.ToolType == 3)
			{
				flag = false;
			}
			base.transform.Find("BlockPanel").gameObject.SetActive(!flag);
		}

		public void SetInteractable(bool interactable)
		{
			_interactable = interactable;
			CheckInteractable();
		}

		private void SetTheRemainTime(TimeSpan timeSpan)
		{
			base.transform.Find("FrozenInfo/RemainTime/Time/Hrs").gameObject.SetActive(true);
			base.transform.Find("FrozenInfo/RemainTime/Time/HrsText").gameObject.SetActive(true);
			base.transform.Find("FrozenInfo/RemainTime/Time/Hrs").GetComponent<Text>().text = timeSpan.Hours.ToString();
			base.transform.Find("FrozenInfo/RemainTime/Time/Min").gameObject.SetActive(true);
			base.transform.Find("FrozenInfo/RemainTime/Time/MinText").gameObject.SetActive(true);
			base.transform.Find("FrozenInfo/RemainTime/Time/Min").GetComponent<Text>().text = ((timeSpan.Minutes > 0 || timeSpan.Seconds <= 0) ? string.Format("{0:D2}", timeSpan.Minutes) : "01");
			base.transform.Find("FrozenInfo/Slider").GetComponent<Slider>().value = (float)timeSpan.TotalSeconds / (float)Singleton<PlayerModule>.Instance.playerData.endlessUseItemCDTime;
		}
	}
}
