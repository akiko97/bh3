using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoActivityInfoPanel : MonoBehaviour
	{
		private WeekDayActivityDataItem _activityData;

		private Text _timeValueText;

		private Text _labelText;

		private Text _inProgressDescText;

		private Text _waitToStartDescText;

		private Text _lockDescText;

		private Text _leftEnterTimeText;

		private Text _minuteText;

		private Text _secondText;

		private Text _milliSecondText;

		private GameObject _contentGameObject;

		private GameObject _nuclearExPanelGameObject;

		private GameObject _lockGameObject;

		private GameObject _waitToStartGameObject;

		private GameObject _overGameObject;

		private GameObject _inProgressGameObject;

		private GameObject _leftTimeGameObject;

		private GameObject _normalGameObject;

		private GameObject _greyGameObject;

		private GameObject _nuclearExPanelLeftTimeGameObject;

		private void Awake()
		{
			_timeValueText = base.transform.Find("Content/WaitToStart/LeftTime/TimeValue").GetComponent<Text>();
			_labelText = base.transform.Find("Content/WaitToStart/LeftTime/Label").GetComponent<Text>();
			_inProgressDescText = base.transform.Find("Content/InProgress/Desc").GetComponent<Text>();
			_waitToStartDescText = base.transform.Find("Content/WaitToStart/Desc").GetComponent<Text>();
			_lockDescText = base.transform.Find("Content/Lock/Desc").GetComponent<Text>();
			_leftEnterTimeText = base.transform.Find("Content/InProgress/LeftEnterTime/Text").GetComponent<Text>();
			_minuteText = base.transform.Find("NuclearExPanel/LeftTime/Minute").GetComponent<Text>();
			_secondText = base.transform.Find("NuclearExPanel/LeftTime/Second").GetComponent<Text>();
			_milliSecondText = base.transform.Find("NuclearExPanel/LeftTime/MilliSecond").GetComponent<Text>();
			_contentGameObject = base.transform.Find("Content").gameObject;
			_nuclearExPanelGameObject = base.transform.Find("NuclearExPanel").gameObject;
			_lockGameObject = base.transform.Find("Content/Lock").gameObject;
			_waitToStartGameObject = base.transform.Find("Content/WaitToStart").gameObject;
			_overGameObject = base.transform.Find("Content/Over").gameObject;
			_inProgressGameObject = base.transform.Find("Content/InProgress").gameObject;
			_leftTimeGameObject = base.transform.Find("Content/WaitToStart/LeftTime").gameObject;
			_normalGameObject = base.transform.Find("Title/Normal").gameObject;
			_greyGameObject = base.transform.Find("Title/Grey").gameObject;
			_nuclearExPanelLeftTimeGameObject = base.transform.Find("NuclearExPanel/LeftTime").gameObject;
			_inProgressGameObject.transform.Find("StaminaCost").gameObject.SetActive(false);
		}

		public void Update()
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Invalid comparison between Unknown and I4
			if (_activityData == null)
			{
				return;
			}
			if (_activityData.GetStatus() == ActivityDataItemBase.Status.WaitToStart)
			{
				string label;
				_timeValueText.text = Miscs.GetTimeSpanToShow(_activityData.beginTime, out label).ToString();
				_labelText.text = label;
			}
			if ((int)_activityData.GetActivityType() == 3)
			{
				if (_activityData.GetStatus() == ActivityDataItemBase.Status.InProgress)
				{
					SetupNuclearCountDown();
				}
				else
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RefreshChapterSelectPage));
				}
			}
		}

		public void SetupView(WeekDayActivityDataItem activityData)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			_activityData = activityData;
			base.gameObject.SetActive(true);
			if ((int)_activityData.GetActivityType() == 3)
			{
				SetupNuclearActivityInfo();
			}
			else
			{
				SetupDefaultActivityInfo();
			}
		}

		private void SetupNuclearActivityInfo()
		{
			_contentGameObject.SetActive(false);
			_nuclearExPanelGameObject.SetActive(true);
			SetupNuclearCountDown();
		}

		private void SetupDefaultActivityInfo()
		{
			_contentGameObject.SetActive(true);
			_nuclearExPanelGameObject.SetActive(false);
			SetupTitleView(_activityData);
			_lockGameObject.SetActive(false);
			_waitToStartGameObject.SetActive(false);
			_overGameObject.SetActive(false);
			_inProgressGameObject.SetActive(false);
			switch (_activityData.GetStatus())
			{
			case ActivityDataItemBase.Status.InProgress:
				_inProgressGameObject.SetActive(true);
				_inProgressDescText.text = _activityData.GetActivityDescription();
				_leftEnterTimeText.text = (_activityData.maxEnterTimes - _activityData.enterTimes).ToString();
				break;
			case ActivityDataItemBase.Status.WaitToStart:
			{
				_waitToStartGameObject.SetActive(true);
				_waitToStartDescText.text = _activityData.GetActivityDescription();
				string label;
				_timeValueText.text = Miscs.GetTimeSpanToShow(_activityData.beginTime, out label).ToString();
				_labelText.text = label;
				break;
			}
			case ActivityDataItemBase.Status.Over:
				_overGameObject.SetActive(true);
				break;
			case ActivityDataItemBase.Status.Locked:
				_lockGameObject.SetActive(true);
				_lockDescText.text = LocalizationGeneralLogic.GetText("Menu_ActivityLock", _activityData.GetMinPlayerLevelLimit());
				break;
			case ActivityDataItemBase.Status.Unavailable:
				_waitToStartGameObject.SetActive(true);
				_waitToStartDescText.text = _activityData.GetActivityLockDescription();
				_leftTimeGameObject.SetActive(false);
				break;
			}
		}

		private void SetupTitleView(WeekDayActivityDataItem activityData)
		{
			bool flag = activityData.GetStatus() == ActivityDataItemBase.Status.InProgress;
			_normalGameObject.SetActive(flag);
			_greyGameObject.SetActive(!flag);
		}

		private void SetupNuclearCountDown()
		{
			TimeSpan timeSpan = _activityData.endTime.Subtract(TimeUtil.Now);
			if (timeSpan < TimeSpan.Zero)
			{
				_nuclearExPanelLeftTimeGameObject.SetActive(false);
				return;
			}
			_nuclearExPanelLeftTimeGameObject.SetActive(true);
			_minuteText.text = string.Format("{0:D2}", (int)timeSpan.TotalMinutes);
			_secondText.text = string.Format("{0:D2}", timeSpan.Seconds);
			_milliSecondText.text = string.Format("{0:D3}", timeSpan.Milliseconds);
		}
	}
}
