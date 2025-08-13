using UnityEngine;

namespace MoleMole
{
	public class Mono_UL_041 : Mono_UL_040
	{
		public ParticleSystem weaponParticle;

		public override void OnTimeScaleChanged(float newTimeScale)
		{
			base.OnTimeScaleChanged(newTimeScale);
			weaponParticle.playbackSpeed = newTimeScale;
		}
	}
}
