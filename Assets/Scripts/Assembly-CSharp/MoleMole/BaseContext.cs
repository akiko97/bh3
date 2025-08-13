using System;
using System.Collections.Generic;
using System.IO;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoleMole
{
	public abstract class BaseContext
	{
		public ContextPattern config;

		public UIType uiType;

		protected Queue<Notify> _notifyQueue;

		protected bool findViewSavedInScene;

		private List<UnityEventBase> _bindedEvents;

		public GameObject view { get; protected set; }

		public bool IsActive { get; private set; }

		public bool EnableTutorial { get; set; }

		public BaseContext()
		{
			_notifyQueue = new Queue<Notify>();
			_bindedEvents = new List<UnityEventBase>();
			EnableTutorial = true;
		}

		public virtual void StartUp(Transform canvasTrans, Transform parentTrans = null)
		{
			if (view == null && findViewSavedInScene)
			{
				FindViewSavedInScene(canvasTrans, parentTrans);
			}
			if (view == null)
			{
				Transform parentTransform = GetParentTransform(canvasTrans, parentTrans);
				GameObject go = Singleton<MainUIManager>.Instance.LoadInstancedView(this);
				InstantiateView(go, parentTransform);
			}
			else
			{
				AddContextIdentifier();
				OnViewSet();
			}
			if (Singleton<MainUIManager>.Instance.GetMainCanvas() is MonoMainCanvas)
			{
				Singleton<MiHoYoGameData>.Instance.GeneralLocalData.AddContextShowCount(this);
			}
		}

		protected virtual void FindViewSavedInScene(Transform canvasTrans, Transform parentTrans)
		{
			parentTrans = GetParentTransform(canvasTrans, parentTrans) ?? canvasTrans;
			string fileName = Path.GetFileName(config.viewPrefabPath);
			Transform transform = parentTrans.Find(fileName);
			if (transform != null)
			{
				view = transform.gameObject;
				view.name += "(Clone)";
				view.SetActive(true);
			}
		}

		private void InstantiateView(GameObject go, Transform viewParent = null)
		{
			view = go;
			if (viewParent != null)
			{
				view.transform.SetParent(viewParent, false);
			}
			AddContextIdentifier();
			OnViewSet();
		}

		private void AddContextIdentifier()
		{
			if (view.GetComponent<ContextIdentifier>() == null)
			{
				ContextIdentifier contextIdentifier = view.AddComponent<ContextIdentifier>();
				contextIdentifier.context = this;
			}
		}

		private void OnViewSet()
		{
			if (this is BasePageContext)
			{
				view.transform.SetAsFirstSibling();
			}
			IsActive = true;
			Singleton<NotifyManager>.Instance.RegisterContext(this);
			SetupView();
			BindViewCallbacks();
			SetViewButtonSoundEffects();
			if (EnableTutorial)
			{
				TryToDoTutorial();
			}
		}

		protected void TryToDoTutorial()
		{
			if (Singleton<TutorialModule>.Instance != null)
			{
				Singleton<TutorialModule>.Instance.TryToDoTutoialWhenShowContext(this);
			}
		}

		public bool HandleNotify(Notify cmd)
		{
			if (IsActive)
			{
				if (view == null)
				{
					return false;
				}
				return DoHandleNotify(cmd);
			}
			lock (_notifyQueue)
			{
				_notifyQueue.Enqueue(cmd);
			}
			return false;
		}

		private bool DoHandleNotify(Notify cmd)
		{
			try
			{
				bool flag = false;
				return (cmd.type != NotifyTypes.NetwrokPacket) ? (flag | OnNotify(cmd)) : (flag | OnPacket(cmd.body as NetPacketV1));
			}
			catch (Exception ex)
			{
				SuperDebug.VeryImportantError("Exception: " + ex.ToString());
				return false;
			}
		}

		public virtual bool OnNotify(Notify ntf)
		{
			return false;
		}

		public virtual bool OnPacket(NetPacketV1 ntf)
		{
			return false;
		}

		protected virtual bool SetupView()
		{
			return false;
		}

		private void UnbindView()
		{
			for (int i = 0; i < _bindedEvents.Count; i++)
			{
				_bindedEvents[i].RemoveAllListeners();
			}
			_bindedEvents.Clear();
		}

		public virtual void Destroy()
		{
			UnbindView();
			Singleton<MainUIManager>.Instance.ReleaseInstancedView(this);
			IsActive = false;
			Singleton<NotifyManager>.Instance.RemoveContext(this);
			if ((uiType == UIType.Page || uiType == UIType.SpecialDialog) && config.cacheType != ViewCacheType.AlwaysCached)
			{
				Singleton<MainUIManager>.Instance.CheckResouceBeforeLoad();
			}
		}

		public virtual void DestroyContextOnly()
		{
			UnbindView();
			IsActive = false;
			Singleton<NotifyManager>.Instance.RemoveContext(this);
		}

		public virtual void SetActive(bool enabled)
		{
			IsActive = enabled;
			if ((bool)view)
			{
				view.SetActive(enabled);
				PostProcessOfSetActive(enabled);
				if (enabled)
				{
					TryToDoTutorial();
				}
			}
		}

		private void PostProcessOfSetActive(bool enabled)
		{
			if (enabled)
			{
				if (_notifyQueue.Count == 0)
				{
					return;
				}
				Notify[] array;
				lock (_notifyQueue)
				{
					array = _notifyQueue.ToArray();
					_notifyQueue.Clear();
				}
				for (int i = 0; i < array.Length; i++)
				{
					DoHandleNotify(array[i]);
				}
			}
			OnSetActive(enabled);
		}

		protected virtual void OnSetActive(bool enabled)
		{
		}

		protected virtual void BindViewCallbacks()
		{
		}

		protected void BindViewCallback(Button button, UnityAction callback)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(callback);
			_bindedEvents.Add(button.onClick);
		}

		protected void BindViewCallback(Toggle toggle, UnityAction<bool> callback)
		{
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(callback);
			_bindedEvents.Add(toggle.onValueChanged);
		}

		protected void BindViewCallback(Transform trans, EventTriggerType eventType, Action<BaseEventData> callback)
		{
			MonoEventTrigger monoEventTrigger = trans.gameObject.GetComponent<MonoEventTrigger>();
			if (monoEventTrigger == null)
			{
				monoEventTrigger = trans.gameObject.AddComponent<MonoEventTrigger>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = eventType;
			entry.callback.AddListener(delegate(BaseEventData evtData)
			{
				callback(evtData);
			});
			monoEventTrigger.AddTrigger(entry);
		}

		private void SetViewButtonSoundEffects()
		{
			Button[] componentsInChildren = view.GetComponentsInChildren<Button>(true);
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				MonoButtonWwiseEvent component = componentsInChildren[i].GetComponent<MonoButtonWwiseEvent>();
				if (!(component != null))
				{
					component = componentsInChildren[i].gameObject.AddComponent<MonoButtonWwiseEvent>();
					component = componentsInChildren[i].GetComponent<MonoButtonWwiseEvent>();
					component.eventName = "UI_Click";
				}
			}
		}

		public void SetView(GameObject newView)
		{
			view = newView;
			OnViewSet();
		}

		public virtual Transform GetParentTransform(Transform canvasTrans, Transform viewParent)
		{
			if (viewParent == null)
			{
				return GetUIHolder(canvasTrans, uiType);
			}
			return viewParent;
		}

		private Transform GetUIHolder(Transform canvasTrans, UIType uiType)
		{
			switch (uiType)
			{
			case UIType.Page:
				return canvasTrans.Find("Pages");
			case UIType.SpecialDialog:
				return canvasTrans.Find("SpecialDialogs");
			case UIType.SuspendBar:
				return canvasTrans.Find("SuspendBars");
			case UIType.Dialog:
				return canvasTrans.Find("Dialogs");
			case UIType.MostFront:
				return canvasTrans;
			case UIType.Root:
				return null;
			default:
				return null;
			}
		}
	}
}
