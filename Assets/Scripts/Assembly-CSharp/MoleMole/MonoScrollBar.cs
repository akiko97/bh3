using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Scrollbar))]
	public class MonoScrollBar : MonoBehaviour
	{
		public void SetVisible(bool isVisible)
		{
			base.gameObject.SetActive(isVisible);
		}
	}
}
