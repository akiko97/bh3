using MoleMole.Config;

namespace MoleMole
{
	public class Mono_RO_030 : BaseMonoRobot
	{
		public override void SetDied(KillEffect killEffect)
		{
			SetLocomotionRandom(2);
			base.SetDied(killEffect);
		}
	}
}
