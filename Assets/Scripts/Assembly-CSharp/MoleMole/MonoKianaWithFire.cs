using UnityEngine;

namespace MoleMole
{
	public class MonoKianaWithFire : MonoKiana_C5
	{
		public GameObject fireObject;

		private void SetBackFireVisible(int show)
		{
			bool active = show != 0;
			if (fireObject != null)
			{
				fireObject.SetActive(active);
			}
		}
	}
}
