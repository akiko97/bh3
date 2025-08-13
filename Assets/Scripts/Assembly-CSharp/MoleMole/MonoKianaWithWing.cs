using UnityEngine;

namespace MoleMole
{
	public class MonoKianaWithWing : MonoKiana_C5
	{
		public Renderer wing;

		public override void Awake()
		{
			base.Awake();
		}

		public override void Revive(Vector3 revivePosition)
		{
			base.Revive(revivePosition);
			EnableWing();
		}

		private void EnableWing()
		{
			wing.enabled = true;
		}

		[AnimationCallback]
		private void TriggerEnableWing(int enable)
		{
			wing.enabled = ((enable != 0) ? true : false);
		}
	}
}
