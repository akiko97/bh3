using MoleMole.Config;

namespace MoleMole
{
	public class Mono_DG_020 : BaseMonoDeadGal
	{
		public override void SetDied(KillEffect killEffect)
		{
			base.SetDied(killEffect);
			SetLocomotionRandom(2);
		}
	}
}
