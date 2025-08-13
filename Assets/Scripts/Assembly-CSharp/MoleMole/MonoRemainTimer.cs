using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoRemainTimer : MonoBehaviour
	{
		private bool _isUpdating;

		private DateTime _targetTime;

		private Action _timeoutCallBack;

		private Action _updateAction;

		private int _timeSpan;

		private bool _hideTime;

		private Transform _Day;

		private Transform _Hrs;

		private Transform _Min;

		private Transform _Sec;

		private Transform _DayText;

		private Transform _HrsText;

		private Transform _MinText;

		private Transform _SecText;

		private void Awake()
		{
			InitTransforms();
		}

		public void SetTargetTime(DateTime targetTime, Action updateAction = null, Action timeoutCallBack = null, bool hideTime = false)
		{
			InitTransforms();
			_targetTime = targetTime;
			_timeoutCallBack = timeoutCallBack;
			_updateAction = updateAction;
			_isUpdating = true;
			_hideTime = hideTime;
			SetRemainTime();
		}

		public void SetTargetTime(int timeSpan)
		{
			InitTransforms();
			_timeSpan = timeSpan;
			_isUpdating = false;
			SetRemainTime();
		}

		private void Update()
		{
			if (!_isUpdating)
			{
				return;
			}
			if (TimeUtil.Now > _targetTime)
			{
				_isUpdating = false;
				if (_timeoutCallBack != null)
				{
					_timeoutCallBack();
				}
			}
			SetRemainTime();
			if (_updateAction != null)
			{
				_updateAction();
			}
		}

		private void SetRemainTime()
		{
			if (!_hideTime)
			{
				TimeSpan timeSpan = ((!_isUpdating) ? new TimeSpan(0, 0, _timeSpan) : (_targetTime - TimeUtil.Now));
				_Day.GetComponent<Text>().text = string.Format("{0:D2}", timeSpan.Days);
				_Hrs.GetComponent<Text>().text = string.Format("{0:D2}", timeSpan.Hours);
				_Min.GetComponent<Text>().text = string.Format("{0:D2}", timeSpan.Minutes);
				_Sec.GetComponent<Text>().text = string.Format("{0:D2}", timeSpan.Seconds);
				_Day.gameObject.SetActive(true);
				_DayText.gameObject.SetActive(true);
				_Hrs.gameObject.SetActive(true);
				_HrsText.gameObject.SetActive(true);
				_Min.gameObject.SetActive(true);
				_MinText.gameObject.SetActive(true);
				_Sec.gameObject.SetActive(true);
				_SecText.gameObject.SetActive(true);
				if (timeSpan.TotalDays >= 1.0)
				{
					_Min.gameObject.SetActive(false);
					_MinText.gameObject.SetActive(false);
					_Sec.gameObject.SetActive(false);
					_SecText.gameObject.SetActive(false);
				}
				else if (timeSpan.TotalHours >= 1.0)
				{
					_Day.gameObject.SetActive(false);
					_DayText.gameObject.SetActive(false);
					_Sec.gameObject.SetActive(false);
					_SecText.gameObject.SetActive(false);
				}
				else
				{
					_Day.gameObject.SetActive(false);
					_DayText.gameObject.SetActive(false);
					_Hrs.gameObject.SetActive(false);
					_HrsText.gameObject.SetActive(false);
				}
			}
		}

		private void InitTransforms()
		{
			if (_Day == null)
			{
				_Day = base.transform.Find("Day");
			}
			if (_Hrs == null)
			{
				_Hrs = base.transform.Find("Hrs");
			}
			if (_Min == null)
			{
				_Min = base.transform.Find("Min");
			}
			if (_Sec == null)
			{
				_Sec = base.transform.Find("Sec");
			}
			if (_DayText == null)
			{
				_DayText = base.transform.Find("DayText");
			}
			if (_HrsText == null)
			{
				_HrsText = base.transform.Find("HrsText");
			}
			if (_MinText == null)
			{
				_MinText = base.transform.Find("MinText");
			}
			if (_SecText == null)
			{
				_SecText = base.transform.Find("SecText");
			}
			if (_hideTime)
			{
				_Day.gameObject.SetActive(false);
				_DayText.gameObject.SetActive(false);
				_Hrs.gameObject.SetActive(false);
				_HrsText.gameObject.SetActive(false);
				_Min.gameObject.SetActive(false);
				_MinText.gameObject.SetActive(false);
				_Sec.gameObject.SetActive(false);
				_SecText.gameObject.SetActive(false);
			}
		}
	}
}
