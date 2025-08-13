using System;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoAppliedEndlessItem : MonoBehaviour
	{
		public Image timerMask;

		private EndlessToolDataItem _toolData;

		private Action<int> _timeEndCallBack;

		private DateTime _targetTime;

		private int _timeSpan;

		private bool _isUpdating;

		private bool _fillDirectionReverse;

		private EndlessPlayerData _selfEndlessData;

		public void SetupView(EndlessToolDataItem itemData)
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Invalid comparison between Unknown and I4
			_toolData = itemData;
			base.transform.Find("VerticalLayout/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_toolData.GetSmallIconPath());
			base.transform.Find("VerticalLayout/TopLine/NameText").GetComponent<Text>().text = _toolData.GetDisplayTitle();
			base.transform.Find("VerticalLayout/AbstractText").GetComponent<Text>().text = _toolData.GetDescription();
			bool flag = false;
			_selfEndlessData = Singleton<EndlessModule>.Instance.GetSelfEndlessData();
			if ((int)itemData.ToolType == 5 && _selfEndlessData.hidden_expire_timeSpecified)
			{
				_targetTime = Miscs.GetDateTimeFromTimeStamp(_selfEndlessData.hidden_expire_time);
				_timeSpan = itemData.GetTimeSpanInSeconds();
				_fillDirectionReverse = true;
				_isUpdating = true;
				_timeEndCallBack = OnEndlessToolTimeOut;
				flag = true;
			}
			if (!flag)
			{
				foreach (EndlessWaitBurstBomb item in _selfEndlessData.wait_burst_bomb_list)
				{
					if (item.item_id == (uint)itemData.ID)
					{
						_targetTime = Miscs.GetDateTimeFromTimeStamp(item.burst_time);
						_timeSpan = itemData.GetTimeSpanInSeconds();
						_fillDirectionReverse = false;
						_isUpdating = true;
						_timeEndCallBack = null;
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				foreach (EndlessWaitEffectItem item2 in _selfEndlessData.wait_effect_item_list)
				{
					if (item2.item_id == (uint)itemData.ID)
					{
						_targetTime = Miscs.GetDateTimeFromTimeStamp(item2.expire_time);
						_timeSpan = itemData.GetTimeSpanInSeconds();
						_fillDirectionReverse = true;
						_isUpdating = item2.expire_timeSpecified;
						_timeEndCallBack = OnEndlessToolTimeOut;
						flag = true;
						break;
					}
				}
			}
			timerMask.gameObject.SetActive(_isUpdating);
			base.transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(itemData.GetSmallIconPath());
			timerMask.gameObject.SetActive(_timeSpan > 0);
			timerMask.fillAmount = 0f;
		}

		private void Update()
		{
			if (!_isUpdating)
			{
				return;
			}
			if (TimeUtil.Now > _targetTime)
			{
				if (_timeEndCallBack != null)
				{
					_timeEndCallBack(_toolData.ID);
				}
				_isUpdating = false;
			}
			float num = Mathf.Clamp01((float)(_targetTime - TimeUtil.Now).TotalSeconds / (float)_timeSpan);
			if (timerMask != null)
			{
				timerMask.fillAmount = ((!_fillDirectionReverse) ? num : (1f - num));
			}
		}

		private void OnEndlessToolTimeOut(int itemID)
		{
			_selfEndlessData.wait_effect_item_list.RemoveAll((EndlessWaitEffectItem item) => item.item_id == itemID);
			_selfEndlessData.wait_burst_bomb_list.RemoveAll((EndlessWaitBurstBomb item) => item.item_id == itemID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessAppliedToolChange));
		}
	}
}
