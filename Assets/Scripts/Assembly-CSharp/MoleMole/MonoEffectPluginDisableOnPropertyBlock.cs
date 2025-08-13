using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginDisableOnPropertyBlock : BaseMonoEffectPlugin
	{
		[Header("Target Renderer Game Object Name")]
		public string RendererGOName;

		[Header("Target Property Name")]
		public string PropertyName;

		[Header("Float Threshold Value")]
		public float Threshold;

		[Header("Reverse Threshold Comparision")]
		public bool Reverse;

		private bool _disabled;

		private ParticleSystemRenderer[] _allRenderers;

		private Renderer _targetRenderer;

		private int _targetPropertyHash;

		private MaterialPropertyBlock _block;

		protected override void Awake()
		{
			base.Awake();
			_allRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
			_disabled = false;
			_block = new MaterialPropertyBlock();
			_effect.disableGORecursively = false;
		}

		public void SetupRenderer(BaseMonoEntity owner)
		{
			BaseMonoAnimatorEntity baseMonoAnimatorEntity = (BaseMonoAnimatorEntity)owner;
			_targetRenderer = null;
			for (int i = 0; i < baseMonoAnimatorEntity.renderers.Length; i++)
			{
				Renderer renderer = baseMonoAnimatorEntity.renderers[i];
				if (renderer.gameObject.name == RendererGOName)
				{
					_targetRenderer = renderer;
				}
			}
			_targetPropertyHash = Shader.PropertyToID(PropertyName);
		}

		private void LateUpdate()
		{
			if (_targetRenderer == null)
			{
				return;
			}
			_targetRenderer.GetPropertyBlock(_block);
			float value = _block.GetFloat(_targetPropertyHash);
			if (!_disabled)
			{
				if (!RendererActive() || CheckThreshold(value))
				{
					for (int i = 0; i < _allRenderers.Length; i++)
					{
						_allRenderers[i].enabled = false;
					}
					_disabled = true;
				}
			}
			else if (RendererActive() && !CheckThreshold(value))
			{
				for (int j = 0; j < _allRenderers.Length; j++)
				{
					_allRenderers[j].enabled = true;
				}
				_disabled = false;
			}
		}

		private bool RendererActive()
		{
			return _targetRenderer.gameObject.activeInHierarchy && _targetRenderer.enabled;
		}

		private bool CheckThreshold(float value)
		{
			return (!Reverse) ? (value > Threshold) : (value < Threshold);
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
