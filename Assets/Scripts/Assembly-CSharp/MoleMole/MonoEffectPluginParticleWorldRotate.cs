using System;
using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginParticleWorldRotate : BaseMonoEffectPlugin
	{
		[Header("Target Particle Systems To Rotate")]
		public ParticleSystem[] targetParticleSystems;

		private void OnAwake()
		{
			SyncStartRotation();
		}

		private void OnEanble()
		{
			SyncStartRotation();
		}

		private void Start()
		{
			for (int i = 0; i < targetParticleSystems.Length; i++)
			{
				targetParticleSystems[i].Clear();
			}
		}

		private void Update()
		{
			SyncStartRotation();
		}

		private void SyncStartRotation()
		{
			for (int i = 0; i < targetParticleSystems.Length; i++)
			{
				targetParticleSystems[i].startRotation = base.transform.rotation.eulerAngles.y * ((float)Math.PI / 180f);
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
