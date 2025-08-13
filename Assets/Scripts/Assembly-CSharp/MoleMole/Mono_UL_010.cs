using UnityEngine;

namespace MoleMole
{
	public sealed class Mono_UL_010 : BaseMonoUlysses
	{
		public ParticleSystem headParticle;

		public override void OnTimeScaleChanged(float newTimeScale)
		{
			base.OnTimeScaleChanged(newTimeScale);
			headParticle.playbackSpeed = newTimeScale;
		}
	}
}
