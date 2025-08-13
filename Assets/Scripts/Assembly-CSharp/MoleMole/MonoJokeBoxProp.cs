using MoleMole.Config;

namespace MoleMole
{
	public class MonoJokeBoxProp : MonoBarrelProp
	{
		private const string TRIGGER_BORN = "BornTrigger";

		private KillEffect _killEffect;

		private bool _isAlive;

		public override void Init(uint runtimeID)
		{
			base.Init(runtimeID);
			DestroyDelay = 2.3f;
			_isAlive = true;
		}

		public override void SetDied(KillEffect killEffect)
		{
			SetCountedDenySelect(true, true);
			_isAlive = false;
			_killEffect = killEffect;
			SetTrigger("BornTrigger");
		}

		public override bool IsActive()
		{
			return _isAlive;
		}

		[AnimationCallback]
		public void BornEndTriggerDied()
		{
			base.SetDied(_killEffect);
		}
	}
}
