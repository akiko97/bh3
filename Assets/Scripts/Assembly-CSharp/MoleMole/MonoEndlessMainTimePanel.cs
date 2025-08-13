using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoEndlessMainTimePanel : MonoBehaviour
	{
		public enum EndlessStage
		{
			Pre = 0,
			Current = 1,
			Post = 2
		}

		private const string PRE_STAGE_COLOR = "EndlessPreStageColor";

		private const string CURRENT_STAGE_COLOR = "EndlessCurrentStageColor";

		private const string POST_STAGE_COLOR = "EndlessPostStageColor";

		private EndlessActivityStatus _endlessStatus;

		private float _intervalTimer;

		private float _updateInterval = 60f;

		[SerializeField]
		private Transform _prepareTime;

		[SerializeField]
		private Transform _inProgressTime;

		[SerializeField]
		private Transform _settleTime;

		[SerializeField]
		private Transform _prepareLabel;

		[SerializeField]
		private Transform _inProgressLabel;

		[SerializeField]
		private Transform _settleLabel;

		private void Start()
		{
			_endlessStatus = Singleton<EndlessModule>.Instance.GetEndlessActivityStatus();
			_intervalTimer = _updateInterval;
		}

		private void Update()
		{
			_intervalTimer += Time.deltaTime;
			if (_intervalTimer > _updateInterval)
			{
				UpdateTimePanel();
				_intervalTimer = 0f;
			}
		}

		private void UpdateTimePanel()
		{
			EndlessActivityStatus endlessStatus = _endlessStatus;
			_endlessStatus = Singleton<EndlessModule>.Instance.GetEndlessActivityStatus();
			switch (_endlessStatus)
			{
			case EndlessActivityStatus.InProgress:
				if (_endlessStatus != endlessStatus)
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessActivityBegin));
				}
				SetTheRemainTime(_prepareTime, _prepareLabel, new TimeSpan(0, 0, 0, 0), EndlessStage.Pre);
				SetTheRemainTime(_inProgressTime, _inProgressLabel, Singleton<EndlessModule>.Instance.EndTime - TimeUtil.Now);
				SetTheRemainTime(_settleTime, _settleLabel, Singleton<EndlessModule>.Instance.SettlementTime - Singleton<EndlessModule>.Instance.EndTime, EndlessStage.Post);
				break;
			case EndlessActivityStatus.WaitToSettlement:
				if (_endlessStatus != endlessStatus)
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessActivityEnd));
				}
				SetTheRemainTime(_prepareTime, _prepareLabel, new TimeSpan(0, 0, 0, 0), EndlessStage.Pre);
				SetTheRemainTime(_inProgressTime, _inProgressLabel, new TimeSpan(0, 0, 0, 0), EndlessStage.Pre);
				SetTheRemainTime(_settleTime, _settleLabel, Singleton<EndlessModule>.Instance.SettlementTime - TimeUtil.Now);
				break;
			case EndlessActivityStatus.WaitToStart:
				if (_endlessStatus != endlessStatus)
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessActivitySettlement));
				}
				SetTheRemainTime(_prepareTime, _prepareLabel, Singleton<EndlessModule>.Instance.BeginTime - TimeUtil.Now);
				SetTheRemainTime(_inProgressTime, _inProgressLabel, Singleton<EndlessModule>.Instance.EndTime - Singleton<EndlessModule>.Instance.BeginTime, EndlessStage.Post);
				SetTheRemainTime(_settleTime, _settleLabel, Singleton<EndlessModule>.Instance.SettlementTime - Singleton<EndlessModule>.Instance.EndTime, EndlessStage.Post);
				break;
			}
		}

		private void SetTheRemainTime(Transform timeTrans, Transform labelTrans, TimeSpan timeSpan, EndlessStage stage = EndlessStage.Current)
		{
			Vector3 localScale = new Vector3(1f, 1f, 1f);
			Color color = MiscData.GetColor("EndlessCurrentStageColor");
			switch (stage)
			{
			case EndlessStage.Pre:
				localScale = new Vector3(0.9f, 0.9f, 0.9f);
				color = MiscData.GetColor("EndlessPreStageColor");
				break;
			case EndlessStage.Post:
				localScale = new Vector3(0.9f, 0.9f, 0.9f);
				color = MiscData.GetColor("EndlessPostStageColor");
				break;
			}
			timeTrans.localScale = localScale;
			labelTrans.localScale = localScale;
			labelTrans.GetComponent<Text>().color = color;
			timeTrans.Find("Day").gameObject.SetActive(timeSpan.Days > 0);
			timeTrans.Find("DayText").gameObject.SetActive(timeSpan.Days > 0);
			timeTrans.Find("Day").GetComponent<Text>().text = timeSpan.Days.ToString();
			timeTrans.Find("Day").GetComponent<Text>().color = color;
			timeTrans.Find("DayText").GetComponent<Text>().color = color;
			timeTrans.Find("Hrs").gameObject.SetActive(true);
			timeTrans.Find("HrsText").gameObject.SetActive(true);
			timeTrans.Find("Hrs").GetComponent<Text>().text = string.Format("{0:D2}", timeSpan.Hours);
			timeTrans.Find("Hrs").GetComponent<Text>().color = color;
			timeTrans.Find("HrsText").GetComponent<Text>().color = color;
			timeTrans.Find("Min").gameObject.SetActive(true);
			timeTrans.Find("MinText").gameObject.SetActive(true);
			timeTrans.Find("Min").GetComponent<Text>().text = ((timeSpan.Minutes > 0 || timeSpan.Seconds <= 0) ? string.Format("{0:D2}", timeSpan.Minutes) : "01");
			timeTrans.Find("Min").GetComponent<Text>().color = color;
			timeTrans.Find("MinText").GetComponent<Text>().color = color;
			timeTrans.Find("Sec").gameObject.SetActive(false);
			timeTrans.Find("SecText").gameObject.SetActive(false);
			timeTrans.Find("Sec").GetComponent<Text>().color = color;
			timeTrans.Find("SecText").GetComponent<Text>().color = color;
		}
	}
}
