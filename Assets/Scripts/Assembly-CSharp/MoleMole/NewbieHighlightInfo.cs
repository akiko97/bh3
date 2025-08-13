using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class NewbieHighlightInfo
	{
		private enum CallbackType
		{
			ButtonCallback = 0,
			PanelCallback = 1
		}

		private NewbieDialogContext _newbieDialogContext;

		public Transform originTrans;

		private List<MonoBehaviour> _enabledScripts;

		private Transform _highlightTrans;

		private List<EventTrigger.Entry> _eventTriggers;

		private List<string> _ignoreScriptList;

		public NewbieHighlightInfo(NewbieDialogContext newbieDialogContext, Transform originTrans, Transform newParent, bool disableHighlightInvoke, Action<BaseEventData> preCallback, Action<BaseEventData> pointerDownCallback, Action<BaseEventData> pointerUpCallback)
		{
			_ignoreScriptList = new List<string>();
			_ignoreScriptList.Add("ImageForSmoothMask");
			_ignoreScriptList.Add("MonoButtonWwiseEvent");
			_newbieDialogContext = newbieDialogContext;
			this.originTrans = originTrans;
			GameObject gameObject = CopyOriginTransform(originTrans);
			_highlightTrans = gameObject.transform;
			_highlightTrans.SetParent(newParent);
			_highlightTrans.SetSiblingIndex(1);
			_highlightTrans.localScale = Vector3.one;
			BindOriginTransformHelper();
			BindViewCallback(disableHighlightInvoke, preCallback, pointerDownCallback, pointerUpCallback);
		}

		public void Recover()
		{
			UnbindOriginTransformHelper();
			UnbindViewCallback();
			if (_highlightTrans != null && _highlightTrans.gameObject != null)
			{
				UnityEngine.Object.Destroy(_highlightTrans.gameObject);
			}
		}

		private GameObject CopyOriginTransform(Transform originTrans)
		{
			_enabledScripts = new List<MonoBehaviour>();
			MonoBehaviour[] componentsInChildren = originTrans.gameObject.GetComponentsInChildren<MonoBehaviour>();
			MonoBehaviour[] array = componentsInChildren;
			foreach (MonoBehaviour monoBehaviour in array)
			{
				if (monoBehaviour.GetType().Namespace == "MoleMole" && monoBehaviour.enabled && !_ignoreScriptList.Contains(monoBehaviour.GetType().Name))
				{
					_enabledScripts.Add(monoBehaviour);
					monoBehaviour.enabled = false;
				}
			}
			Vector3 position = new Vector3(originTrans.position.x, originTrans.position.y, originTrans.position.z);
			Transform parent = originTrans.parent;
			MonoBehaviour[] components = parent.gameObject.GetComponents<MonoBehaviour>();
			List<MonoBehaviour> list = new List<MonoBehaviour>();
			MonoBehaviour[] array2 = components;
			foreach (MonoBehaviour monoBehaviour2 in array2)
			{
				if (monoBehaviour2 is LayoutGroup && monoBehaviour2.enabled)
				{
					list.Add(monoBehaviour2);
					monoBehaviour2.enabled = false;
				}
			}
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(originTrans.gameObject, position, originTrans.rotation);
			foreach (MonoBehaviour enabledScript in _enabledScripts)
			{
				enabledScript.enabled = true;
			}
			foreach (MonoBehaviour item in list)
			{
				item.enabled = true;
			}
			Button componentInChildren = gameObject.transform.GetComponentInChildren<Button>();
			if (componentInChildren != null)
			{
				componentInChildren.interactable = true;
			}
			Image component = gameObject.transform.GetComponent<Image>();
			if (component != null)
			{
				component.raycastTarget = true;
			}
			Image[] componentsInChildren2 = gameObject.transform.GetComponentsInChildren<Image>();
			foreach (Image image in componentsInChildren2)
			{
				image.raycastTarget = true;
			}
			return gameObject;
		}

		private void BindOriginTransformHelper()
		{
			if (originTrans.gameObject.GetComponent<MonoNewbieOriginTransformHelper>() == null)
			{
				MonoNewbieOriginTransformHelper monoNewbieOriginTransformHelper = originTrans.gameObject.AddComponent<MonoNewbieOriginTransformHelper>();
				monoNewbieOriginTransformHelper.newbieDialogContext = _newbieDialogContext;
			}
		}

		private void UnbindOriginTransformHelper()
		{
			MonoNewbieOriginTransformHelper component = originTrans.gameObject.GetComponent<MonoNewbieOriginTransformHelper>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
		}

		private void BindViewCallback(bool disableHighlightInvoke, Action<BaseEventData> preCallback, Action<BaseEventData> pointerDownCallback, Action<BaseEventData> pointerUpCallback)
		{
			Button componentInChildren = _highlightTrans.GetComponentInChildren<Button>();
			if (componentInChildren != null)
			{
				componentInChildren.onClick = new Button.ButtonClickedEvent();
			}
			BindViewCallback(disableHighlightInvoke, (!(componentInChildren != null)) ? _highlightTrans : componentInChildren.gameObject.transform, preCallback, pointerDownCallback, pointerUpCallback, componentInChildren != null);
		}

		private void BindViewCallback(bool disableHighlightInvoke, Transform trans, Action<BaseEventData> preCallback, Action<BaseEventData> pointerDownCallback, Action<BaseEventData> pointerUpCallback, bool isButton)
		{
			if (trans == null)
			{
				return;
			}
			EventTrigger component = trans.gameObject.GetComponent<EventTrigger>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			MonoEventTrigger monoEventTrigger = trans.gameObject.GetComponent<MonoEventTrigger>();
			if (monoEventTrigger == null)
			{
				monoEventTrigger = trans.gameObject.AddComponent<MonoEventTrigger>();
			}
			List<EventTrigger.Entry> list = new List<EventTrigger.Entry>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(delegate(BaseEventData evtData)
			{
				preCallback(evtData);
			});
			if (!disableHighlightInvoke)
			{
				entry.callback.AddListener(delegate(BaseEventData evtData)
				{
					InvokeOriginPointerDownEvent((PointerEventData)evtData);
				});
			}
			entry.callback.AddListener(delegate(BaseEventData evtData)
			{
				pointerDownCallback(evtData);
			});
			list.Add(entry);
			EventTrigger.Entry entry2 = new EventTrigger.Entry();
			entry2.eventID = EventTriggerType.PointerUp;
			if (!disableHighlightInvoke)
			{
				entry2.callback.AddListener(delegate(BaseEventData evtData)
				{
					if (isButton)
					{
						InvokeOriginButtonEvent();
					}
					else
					{
						InvokeOriginPanelEvent(evtData);
					}
				});
			}
			entry2.callback.AddListener(delegate(BaseEventData evtData)
			{
				pointerUpCallback(evtData);
			});
			list.Add(entry2);
			monoEventTrigger.triggers = list;
			_eventTriggers = list;
		}

		private void UnbindViewCallback()
		{
			if (_eventTriggers != null)
			{
				_eventTriggers.Clear();
				_eventTriggers = null;
			}
		}

		private void InvokeOriginButtonEvent()
		{
			if (originTrans == null || originTrans.gameObject == null)
			{
				return;
			}
			Button[] componentsInChildren = originTrans.gameObject.GetComponentsInChildren<Button>(true);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].onClick.Invoke();
				MonoButtonWwiseEvent component = componentsInChildren[0].GetComponent<MonoButtonWwiseEvent>();
				if (component != null)
				{
					component.OnPointerClick(null);
				}
			}
			InvokeOriginPointerUpEvent(null);
		}

		private void InvokeOriginPanelEvent(BaseEventData data = null)
		{
			if (!(originTrans == null) && !(originTrans.gameObject == null))
			{
				ExecuteEvents.Execute(originTrans.gameObject, data, ExecuteEvents.pointerClickHandler);
				InvokeOriginPointerUpEvent(null);
			}
		}

		private void InvokeOriginPointerDownEvent(PointerEventData eventData)
		{
			if (originTrans == null || _enabledScripts == null)
			{
				return;
			}
			foreach (MonoBehaviour enabledScript in _enabledScripts)
			{
				if (enabledScript is IPointerDownHandler)
				{
					((IPointerDownHandler)enabledScript).OnPointerDown(eventData);
				}
			}
		}

		private void InvokeOriginPointerUpEvent(PointerEventData eventData)
		{
			if (originTrans == null || _enabledScripts == null)
			{
				return;
			}
			foreach (MonoBehaviour enabledScript in _enabledScripts)
			{
				if (enabledScript is IPointerUpHandler)
				{
					((IPointerUpHandler)enabledScript).OnPointerUp(eventData);
				}
			}
		}
	}
}
