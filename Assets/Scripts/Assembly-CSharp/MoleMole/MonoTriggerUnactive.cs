using UnityEngine;

namespace MoleMole
{
	public class MonoTriggerUnactive : MonoBehaviour
	{
		public void TriggerUnactive()
		{
			base.gameObject.SetActive(false);
		}
	}
}
