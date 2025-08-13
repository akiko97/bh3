using MoleMole.Config;

namespace MoleMole
{
	public abstract class BaseMonoRobot : BaseMonoMonster
	{
		public override void Awake()
		{
			base.Awake();
		}

		public override void SetDied(KillEffect killEffect)
		{
			SetLocomotionRandom(2);
			base.SetDied(killEffect);
		}
	}
}
