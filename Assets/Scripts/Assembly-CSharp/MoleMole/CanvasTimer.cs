using System;
using UnityEngine;

namespace MoleMole
{
	public class CanvasTimer
	{
		public float timespan;

		public float triggerCD;

		public Action timeUpCallback;

		public Action timeTriggerCallback;

		public bool infiniteTimeSpan;

		private float _timer;

		private float _triggerTimer;

		private bool isRunning;

		public bool IsTimeUp { get; private set; }

		public CanvasTimer()
		{
			_triggerTimer = 0f;
			_timer = 0f;
			IsTimeUp = false;
			isRunning = true;
		}

		public void Destroy()
		{
			IsTimeUp = true;
			timeUpCallback = null;
			timeTriggerCallback = null;
		}

		public void StartRun(bool reset = false)
		{
			isRunning = true;
			if (reset)
			{
				_triggerTimer = 0f;
				_timer = 0f;
				IsTimeUp = false;
			}
		}

		public void StopRun()
		{
			isRunning = false;
		}

		public void Core()
		{
			if (IsTimeUp || !isRunning)
			{
				return;
			}
			if (!infiniteTimeSpan)
			{
				_timer += Time.deltaTime;
				if (_timer > timespan)
				{
					_timer = timespan;
					IsTimeUp = true;
					if (timeUpCallback != null)
					{
						timeUpCallback();
					}
				}
			}
			if (!(triggerCD > 0f))
			{
				return;
			}
			if (_triggerTimer > triggerCD)
			{
				_triggerTimer -= triggerCD;
				if (timeTriggerCallback != null)
				{
					timeTriggerCallback();
				}
			}
			_triggerTimer += Time.deltaTime;
		}
	}
}
