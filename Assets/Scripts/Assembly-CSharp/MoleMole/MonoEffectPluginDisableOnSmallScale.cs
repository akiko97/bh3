using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginDisableOnSmallScale : BaseMonoEffectPlugin
	{
		private bool _disabled;

		private ParticleSystemRenderer[] _allRenderers;

		protected override void Awake()
		{
			base.Awake();
			_allRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
			_disabled = false;
			_effect.disableGORecursively = false;
		}

		private void Update()
		{
			if (!_disabled)
			{
				if (base.transform.lossyScale.x < 0.2f)
				{
					for (int i = 0; i < _allRenderers.Length; i++)
					{
						_allRenderers[i].enabled = false;
					}
					_disabled = true;
				}
			}
			else if (base.transform.lossyScale.x > 0.2f)
			{
				for (int j = 0; j < _allRenderers.Length; j++)
				{
					_allRenderers[j].enabled = true;
				}
				_disabled = false;
			}
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override void SetDestroy()
		{
		}
	}
}
