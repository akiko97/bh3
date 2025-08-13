using System;
using UnityEngine;

namespace MoleMole
{
	public class EntityTimer
	{
		public float timer;

		public float timespan;

		public bool isTimeUp;

		public Action timeupAction;

		private BaseMonoEntity _timeScaleEntity;

		public bool isActive { get; private set; }

		public EntityTimer()
			: this(0f, Singleton<LevelManager>.Instance.levelEntity, false)
		{
			SetActive(false);
		}

		public EntityTimer(float timespan)
			: this(timespan, Singleton<LevelManager>.Instance.levelEntity, false)
		{
		}

		public EntityTimer(float timespan, BaseMonoEntity timeScaleEntity)
			: this(timespan, timeScaleEntity, false)
		{
		}

		public EntityTimer(float timespan, BaseMonoEntity timeScaleEntity, bool active)
		{
			_timeScaleEntity = timeScaleEntity;
			this.timespan = timespan;
			Reset(active);
		}

		public void Reset()
		{
			timer = 0f;
			isTimeUp = false;
		}

		public void Reset(bool active)
		{
			Reset();
			SetActive(active);
		}

		public void SetActive(bool active)
		{
			isActive = active;
		}

		public float GetTimingRatio()
		{
			return Mathf.Clamp01(timer / timespan);
		}

		public void Core(float deltaTimeRatio = 1f)
		{
			if (!isActive || isTimeUp)
			{
				return;
			}
			float num = Time.deltaTime * _timeScaleEntity.TimeScale * deltaTimeRatio;
			timer += num;
			if (timer > timespan)
			{
				timer = timespan;
				isTimeUp = true;
				if (timeupAction != null)
				{
					timeupAction();
				}
			}
		}
	}
}
