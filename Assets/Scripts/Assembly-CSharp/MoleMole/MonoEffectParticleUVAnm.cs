using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class MonoEffectParticleUVAnm : BaseMonoEffectPlugin
	{
		public int Tiles_X = 3;

		public int Tiles_Y = 2;

		public AnimationCurve FrameOverTime = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		public bool Continuous;

		[Range(1f, 1000f)]
		public int Cycles = 1;

		[Header("Particle System To Use For UV Animation")]
		public ParticleSystem TargetParticleSystem;

		private int _totalFrames;

		private float _step_X;

		private float _step_Y;

		private Material _material;

		private bool _prepared;

		protected override void Awake()
		{
			base.Awake();
			if (TargetParticleSystem == null)
			{
				TargetParticleSystem = GetComponent<ParticleSystem>();
			}
			Preparation();
		}

		public override void Setup()
		{
			base.Setup();
		}

		protected override void OnValidate()
		{
			base.OnValidate();
		}

		private void Preparation()
		{
			if (!(TargetParticleSystem == null))
			{
				ParticleSystemRenderer component = TargetParticleSystem.GetComponent<ParticleSystemRenderer>();
				if (Application.isPlaying)
				{
					GraphicsUtils.CreateAndAssignInstancedMaterial(component, component.sharedMaterial);
					_material = component.sharedMaterial;
				}
				else
				{
					_material = component.sharedMaterial;
				}
				_totalFrames = Tiles_X * Tiles_Y;
				_step_X = 1f / (float)Tiles_X;
				_step_Y = 1f / (float)Tiles_Y;
				_material.SetTextureScale("_MainTex", new Vector2(_step_X, _step_Y));
				_prepared = true;
			}
		}

		public void Update()
		{
			if (!_prepared)
			{
				Preparation();
			}
			if (_prepared && TargetParticleSystem.particleCount != 0)
			{
				float time = TargetParticleSystem.time * (float)Cycles;
				time = FrameOverTime.Evaluate(time);
				if (Continuous)
				{
					ContinuousUpdate(time);
				}
				else
				{
					DiscreteUpdate(time);
				}
			}
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override void SetDestroy()
		{
		}

		private void DiscreteUpdate(float time)
		{
			int num = (int)(time * (float)_totalFrames / (TargetParticleSystem.startLifetime + 0.001f));
			num %= _totalFrames;
			float x = (float)(num % Tiles_X) * _step_X;
			float y = (float)(Tiles_Y - 1 - num / Tiles_X) * _step_Y;
			_material.SetTextureScale("_MainTex", new Vector2(_step_X, _step_Y));
			_material.SetTextureOffset("_MainTex", new Vector2(x, y));
		}

		private void ContinuousUpdate(float time)
		{
			float num = time * (float)_totalFrames / TargetParticleSystem.startLifetime;
			num %= (float)_totalFrames;
			float x = num % (float)Tiles_X * _step_X;
			float y = ((float)(Tiles_Y - 1) - num / (float)Tiles_X) * _step_Y;
			_material.SetTextureScale("_MainTex", new Vector2(_step_X, _step_Y));
			_material.SetTextureOffset("_MainTex", new Vector2(x, y));
		}
	}
}
