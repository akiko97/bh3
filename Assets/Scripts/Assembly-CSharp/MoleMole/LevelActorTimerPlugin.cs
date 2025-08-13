using UnityEngine;

namespace MoleMole
{
	public class LevelActorTimerPlugin : BaseActorPlugin
	{
		private LevelActor _levelActor;

		private SafeFloat _timer = 0f;

		private bool _timing;

		public float Timer
		{
			get
			{
				return _timer;
			}
		}

		public LevelActorTimerPlugin(LevelActor levelActor)
		{
			_levelActor = levelActor;
			_timing = false;
		}

		public void StartTiming()
		{
			_timing = true;
		}

		public void StopTiming()
		{
			_timing = false;
		}

		public override void OnAdded()
		{
			_timer = 0f;
		}

		public override void Core()
		{
			if (_timing)
			{
				float oldTimer = _timer;
				_timer = (float)_timer + Time.deltaTime * _levelActor.levelEntity.TimeScale;
				SetTimingText(oldTimer, _timer);
			}
			base.Core();
		}

		private void SetTimingText(float oldTimer, float newTimer)
		{
			if (Mathf.CeilToInt(oldTimer) != Mathf.CeilToInt(newTimer))
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetTimerText, newTimer));
			}
		}
	}
}
