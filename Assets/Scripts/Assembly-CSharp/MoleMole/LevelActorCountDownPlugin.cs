using System;
using UnityEngine;

namespace MoleMole
{
	public class LevelActorCountDownPlugin : BaseActorPlugin
	{
		private LevelActor _levelActor;

		public SafeFloat totalTime = 0f;

		[HideInInspector]
		public SafeFloat countDownTimer = 0f;

		[HideInInspector]
		public Action<float, float> OnTimingChange;

		[HideInInspector]
		public Action<bool> OnTimingVisibleChange;

		public bool isTiming;

		public bool timeUpWin;

		private float countDownSpeedRatio;

		private float speedRatioInNormalTime;

		private float speedRatioInWitchTime;

		public bool IsLevelTimeUp { get; private set; }

		public LevelActorCountDownPlugin(LevelActor levelActor, float totalTime, bool timesUpWin = false)
		{
			_levelActor = levelActor;
			timeUpWin = timesUpWin;
			this.totalTime = totalTime;
			isTiming = false;
			IsLevelTimeUp = false;
			countDownSpeedRatio = 1f;
			speedRatioInNormalTime = 1f;
			speedRatioInWitchTime = 1f;
			OnTimingChange = (Action<float, float>)Delegate.Combine(OnTimingChange, new Action<float, float>(SetTimingText));
			SetTimingText(0f, totalTime);
		}

		public override void OnAdded()
		{
			countDownTimer = totalTime;
		}

		public override void OnRemoved()
		{
		}

		public void AddRemainTime(float timeDelta)
		{
			float num = (float)countDownTimer + timeDelta;
			if (num <= 0f)
			{
				num = 0f;
				isTiming = false;
				IsLevelTimeUp = true;
				OnTimingChange = (Action<float, float>)Delegate.Remove(OnTimingChange, new Action<float, float>(SetTimingText));
				Singleton<EventManager>.Instance.FireEvent(new EvtLevelTimesUp(562036737u));
			}
			DelegateUtils.UpdateField(ref countDownTimer, num, OnTimingChange);
		}

		public void ResetPlugin(float totalTime)
		{
			this.totalTime = totalTime;
			isTiming = false;
			IsLevelTimeUp = false;
			countDownTimer = this.totalTime;
			SetTimingText(totalTime, totalTime);
		}

		public override void Core()
		{
			if (isTiming && (float)countDownTimer > 0f)
			{
				float num = (float)countDownTimer - Time.deltaTime * _levelActor.levelEntity.TimeScale * _levelActor.levelEntity.AuxTimeScale * countDownSpeedRatio;
				if (num <= 0f)
				{
					num = 0f;
					isTiming = false;
					IsLevelTimeUp = true;
					OnTimingChange = (Action<float, float>)Delegate.Remove(OnTimingChange, new Action<float, float>(SetTimingText));
					Singleton<EventManager>.Instance.FireEvent(new EvtLevelTimesUp(562036737u));
				}
				DelegateUtils.UpdateField(ref countDownTimer, num, OnTimingChange);
			}
			base.Core();
		}

		private void SetTimingText(float oldTimer, float newTimer)
		{
			if (Mathf.CeilToInt(oldTimer) != Mathf.CeilToInt(newTimer))
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetTimeCountDownText, newTimer));
			}
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtLevelBuffState)
			{
				if ((evt as EvtLevelBuffState).state == LevelBuffState.Start && (evt as EvtLevelBuffState).levelBuff == LevelBuffType.WitchTime)
				{
					countDownSpeedRatio = speedRatioInWitchTime;
				}
				if ((evt as EvtLevelBuffState).state == LevelBuffState.Stop && (evt as EvtLevelBuffState).levelBuff == LevelBuffType.WitchTime)
				{
					countDownSpeedRatio = speedRatioInNormalTime;
				}
			}
			return false;
		}

		public void SetCountDownSpeedRatio(float ratioInNormalTime, float ratioInWitchTime)
		{
			speedRatioInNormalTime = ratioInNormalTime;
			speedRatioInWitchTime = ratioInWitchTime;
			if (Singleton<LevelManager>.Instance.levelActor.IsLevelBuffActive(LevelBuffType.WitchTime))
			{
				countDownSpeedRatio = ratioInWitchTime;
			}
			else
			{
				countDownSpeedRatio = speedRatioInNormalTime;
			}
		}
	}
}
