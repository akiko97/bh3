using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	public class MonoButtonWwiseEvent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public string eventName;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				Singleton<WwiseAudioManager>.Instance.Post(eventName);
			}
		}
	}
}
