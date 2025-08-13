using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	public class MonoEventTrigger : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IEndDragHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
	{
		[NonSerialized]
		public List<EventTrigger.Entry> triggers = new List<EventTrigger.Entry>();

		private void OnDisable()
		{
			ClearTriggers();
		}

		public void AddTrigger(EventTrigger.Entry entry)
		{
			int index = triggers.SeekAddPosition();
			triggers[index] = entry;
		}

		public void ClearTriggers()
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				triggers[i] = null;
			}
		}

		private void Execute(EventTriggerType id, BaseEventData eventData)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				EventTrigger.Entry entry = triggers[i];
				if (entry != null && entry.eventID == id && entry.callback != null)
				{
					entry.callback.Invoke(eventData);
				}
			}
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerEnter, eventData);
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerExit, eventData);
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			Execute(EventTriggerType.Drag, eventData);
		}

		public virtual void OnDrop(PointerEventData eventData)
		{
			Execute(EventTriggerType.Drop, eventData);
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerDown, eventData);
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerUp, eventData);
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerClick, eventData);
		}

		public virtual void OnSelect(BaseEventData eventData)
		{
			Execute(EventTriggerType.Select, eventData);
		}

		public virtual void OnDeselect(BaseEventData eventData)
		{
			Execute(EventTriggerType.Deselect, eventData);
		}

		public virtual void OnScroll(PointerEventData eventData)
		{
			Execute(EventTriggerType.Scroll, eventData);
		}

		public virtual void OnMove(AxisEventData eventData)
		{
			Execute(EventTriggerType.Move, eventData);
		}

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			Execute(EventTriggerType.UpdateSelected, eventData);
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			Execute(EventTriggerType.InitializePotentialDrag, eventData);
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			Execute(EventTriggerType.BeginDrag, eventData);
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			Execute(EventTriggerType.EndDrag, eventData);
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			Execute(EventTriggerType.Submit, eventData);
		}

		public virtual void OnCancel(BaseEventData eventData)
		{
			Execute(EventTriggerType.Cancel, eventData);
		}
	}
}
