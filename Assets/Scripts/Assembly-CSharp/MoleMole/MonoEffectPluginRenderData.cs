using System;
using MoleMole.Config;

namespace MoleMole
{
	public class MonoEffectPluginRenderData : BaseMonoEffectPlugin
	{
		[Serializable]
		public class TintCameraWithDelay : ConfigTintCamera
		{
			public float Delay;
		}

		public TintCameraWithDelay[] tintEntries;

		private EntityTimer[] _timers;

		protected override void Awake()
		{
			base.Awake();
			_timers = new EntityTimer[tintEntries.Length];
			for (int i = 0; i < _timers.Length; i++)
			{
				_timers[i] = new EntityTimer(tintEntries[i].Delay, _effect);
				_timers[i].Reset(false);
			}
		}

		public override void Setup()
		{
			for (int i = 0; i < _timers.Length; i++)
			{
				_timers[i].Reset(true);
			}
		}

		private void Update()
		{
			for (int i = 0; i < _timers.Length; i++)
			{
				_timers[i].Core(1f);
				if (_timers[i].isTimeUp)
				{
					TintCameraWithDelay tintCameraWithDelay = tintEntries[i];
					Singleton<StageManager>.Instance.GetPerpStage().TriggerTint(tintCameraWithDelay.RenderDataName, tintCameraWithDelay.Duration, tintCameraWithDelay.TransitDuration);
					_timers[i].Reset(false);
				}
			}
		}

		public override void SetDestroy()
		{
			for (int i = 0; i < _timers.Length; i++)
			{
				_timers[i].Reset(false);
			}
		}

		public override bool IsToBeRemove()
		{
			return false;
		}
	}
}
