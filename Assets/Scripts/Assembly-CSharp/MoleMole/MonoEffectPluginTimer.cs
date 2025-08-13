using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginTimer : BaseMonoEffectPlugin
	{
		[Header("How long does this effect last.")]
		public float EffectTime;

		[Header("Kill Immediately instead of stop emitting")]
		public bool KillImmediately;

		private float _timer;

		private bool _needStop;

		private bool _isToBeRemove;

		protected override void Awake()
		{
			base.Awake();
			_timer = 0f;
			_needStop = true;
			_isToBeRemove = false;
		}

		public override void Setup()
		{
			_timer = 0f;
			_needStop = true;
			_isToBeRemove = false;
		}

		public void Update()
		{
			if (_timer < EffectTime)
			{
				_timer += Time.deltaTime * _effect.TimeScale;
			}
			if (!(_timer >= EffectTime))
			{
				return;
			}
			if (_needStop)
			{
				if (KillImmediately || _effect.mainParticleSystem == null)
				{
					_effect.SetDestroyImmediately();
				}
				else
				{
					_effect.SetDestroy();
				}
			}
			_needStop = false;
		}

		public override bool IsToBeRemove()
		{
			return _isToBeRemove;
		}

		public override void SetDestroy()
		{
		}
	}
}
