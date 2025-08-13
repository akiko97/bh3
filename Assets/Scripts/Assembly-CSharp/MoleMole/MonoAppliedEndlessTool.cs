using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAppliedEndlessTool : MonoBehaviour
	{
		public Image timerMask;

		private EndlessToolDataItem _toolData;

		private Action<int> _timeEndCallBack;

		private DateTime _targetTime;

		private int _timeSpan;

		private bool _isUpdating;

		private bool _fillDirectionReverse;

		public void SetupView(EndlessToolDataItem toolData, uint timestamp, Action<int> endCallBack)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Invalid comparison between Unknown and I4
			_toolData = toolData;
			if (timestamp == 0)
			{
				timerMask.gameObject.SetActive(false);
				_isUpdating = false;
				return;
			}
			_targetTime = Miscs.GetDateTimeFromTimeStamp(timestamp);
			_timeEndCallBack = endCallBack;
			_timeSpan = _toolData.GetTimeSpanInSeconds();
			_isUpdating = _timeSpan > 0;
			_fillDirectionReverse = (((int)toolData.ToolType != 4) ? true : false);
			base.transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(toolData.GetSmallIconPath());
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
	}
}
