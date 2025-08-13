using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginParticleRotateAxis : BaseMonoEffectPlugin
	{
		public int maxParticleCount = 32;

		public ParticleSystem[] targetParticleSystems;

		public Vector3 axis = Vector3.forward;

		private List<ParticleSystem.Particle[]> _particles;

		protected override void Awake()
		{
			base.Awake();
			_particles = new List<ParticleSystem.Particle[]>();
			for (int i = 0; i < targetParticleSystems.Length; i++)
			{
				_particles.Add(new ParticleSystem.Particle[maxParticleCount]);
			}
		}

		private void SetParticleAxis()
		{
			for (int i = 0; i < targetParticleSystems.Length; i++)
			{
				ParticleSystem particleSystem = targetParticleSystems[i];
				if (!(particleSystem == null) && particleSystem.IsAlive())
				{
					ParticleSystem.Particle[] array = _particles[i];
					particleSystem.GetParticles(array);
					for (int j = 0; j < array.Length; j++)
					{
						array[j].axisOfRotation = axis;
					}
				}
			}
		}

		private void Update()
		{
			SetParticleAxis();
		}

		public override void SetDestroy()
		{
		}

		public override bool IsToBeRemove()
		{
			return false;
		}
	}
}
