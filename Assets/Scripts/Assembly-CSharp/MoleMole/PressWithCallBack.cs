using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	public class PressWithCallBack : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IPointerExitHandler
	{
		public delegate void OnPress(Transform trans, bool isPress);

		public OnPress onPress;

		[SerializeField]
		private bool _isPress;

		public bool IsPress
		{
			get
			{
				return _isPress;
			}
			set
			{
				_isPress = value;
				if (onPress != null)
				{
					onPress(base.transform, _isPress);
				}
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			IsPress = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			IsPress = false;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			IsPress = false;
		}
	}
}
