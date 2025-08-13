using UnityEngine;

namespace MoleMole
{
	public class MonoItemStatus : MonoBehaviour
	{
		public bool isValid = true;

		private void Awake()
		{
			isValid = true;
		}
	}
}
