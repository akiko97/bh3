using MoleMole.Config;

namespace MoleMole
{
	public abstract class BaseMonoUlysses : BaseMonoMonster
	{
		public override void SetDied(KillEffect killEffect)
		{
			SetLocomotionRandom(2);
			base.SetDied(killEffect);
		}
	}
}
