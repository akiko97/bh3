using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	public class LongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IPointerExitHandler
	{
		private Action<UnityEngine.Object> OnLongPress;

		private bool isPointDown;

		private bool isLongPressTrigged;

		private float pressStarTime;

		private float LongPressThreshold = 1f;

		private void Start()
		{
		}

		private void Update()
		{
			if (isPointDown && !isLongPressTrigged && Time.time - pressStarTime >= LongPressThreshold)
			{
				isLongPressTrigged = true;
				if (OnLongPress != null)
				{
					OnLongPress(null);
				}
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			isPointDown = true;
			pressStarTime = Time.time;
			isLongPressTrigged = false;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			isPointDown = false;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			isPointDown = false;
		}

		public void SetLongPressAction(Action<UnityEngine.Object> action)
		{
			OnLongPress = action;
		}

		public void SetLongPressThreshold(float threshold)
		{
			LongPressThreshold = threshold;
		}
	}
}
