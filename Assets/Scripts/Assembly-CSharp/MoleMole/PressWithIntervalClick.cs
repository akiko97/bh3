using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Button))]
	public class PressWithIntervalClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IPointerExitHandler
	{
		public float intervalSeconds = 0.2f;

		private bool _isPointDown;

		private float _timer;

		private Button _btn;

		private void Awake()
		{
			_btn = GetComponent<Button>();
			_timer = 0f;
		}

		private void Update()
		{
			if (_btn.interactable && _isPointDown && _timer > 0f)
			{
				_timer -= Time.deltaTime;
				if (_timer <= 0f)
				{
					_btn.onClick.Invoke();
					_timer = intervalSeconds;
				}
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_btn.interactable)
			{
				_isPointDown = true;
				_timer = intervalSeconds;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_btn.interactable)
			{
				_isPointDown = false;
				_timer = 0f;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_isPointDown = false;
			_timer = 0f;
		}
	}
}
