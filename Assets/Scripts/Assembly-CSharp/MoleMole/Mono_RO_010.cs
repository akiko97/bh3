using MoleMole.Config;

namespace MoleMole
{
	public class Mono_RO_010 : BaseMonoRobot
	{
		public override void SetDied(KillEffect killEffect)
		{
			SetLocomotionRandom(2);
			base.SetDied(killEffect);
		}
	}
}
