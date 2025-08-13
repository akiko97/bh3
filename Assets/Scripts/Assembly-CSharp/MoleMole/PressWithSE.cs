using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	public class PressWithSE : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		public string SEEventName;

		public void OnPointerDown(PointerEventData eventData)
		{
			Singleton<WwiseAudioManager>.Instance.Post(SEEventName);
		}
	}
}
