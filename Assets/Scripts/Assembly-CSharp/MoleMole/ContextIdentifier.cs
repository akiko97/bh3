using UnityEngine;

namespace MoleMole
{
	public class ContextIdentifier : MonoBehaviour
	{
		public BaseContext context;

		private void OnDestroy()
		{
			context = null;
		}
	}
}
