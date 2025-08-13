using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginFastDestroy : BaseMonoEffectPlugin
	{
		[Header("Remove on set destroy")]
		public bool removeImmediatelyOnSetDestroy;

		[Header("Fast Destroy Duration")]
		public float fastDestroyDuration = 0.1f;

		private float _timer;

		public override void Setup()
		{
			_timer = 0f;
		}

		private void Update()
		{
			if (_timer > 0f)
			{
				_timer -= Time.deltaTime * _effect.TimeScale;
			}
		}

		public override void SetDestroy()
		{
			_timer = ((!removeImmediatelyOnSetDestroy) ? fastDestroyDuration : (-1f));
		}

		public override bool IsToBeRemove()
		{
			return _timer < 0f;
		}
	}
}
