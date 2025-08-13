using UnityEngine;

namespace MoleMole
{
	public class MonoStageObject : MonoBehaviour
	{
		public void TriggerDestroy()
		{
			Object.Destroy(base.gameObject);
		}

		public void TriggerLevelState(string state)
		{
		}

		public void TriggerStageTransitFinish()
		{
			base.gameObject.SetActive(false);
		}
	}
}
