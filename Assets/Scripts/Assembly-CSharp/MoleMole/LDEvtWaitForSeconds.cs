using UnityEngine;

namespace MoleMole
{
	public class LDEvtWaitForSeconds : BaseLDEvent
	{
		private float timeLeft;

		public LDEvtWaitForSeconds(double t)
		{
			timeLeft = (float)t;
		}

		public override void Core()
		{
			timeLeft -= Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime;
			if (timeLeft <= 0f)
			{
				Done();
			}
		}
	}
}
