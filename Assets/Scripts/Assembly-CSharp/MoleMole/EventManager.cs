using System;
using System.Collections.Generic;
using System.Diagnostics;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class EventManager
	{
		private class ListenerRegistry
		{
			public uint listenerID;

			public uint registerCount;

			public ListenerRegistry(uint listenerID)
			{
				this.listenerID = listenerID;
				registerCount = 1u;
			}
		}

		private Dictionary<uint, BaseActor> _actors;

		private List<BaseActor> _actorList;

		private List<BaseActor>[] _categoryActors;

		private List<BaseEvent> _queuedEvents;

		private Dictionary<Type, List<ListenerRegistry>> _evtListeners;

		private BaseEvent _curParentEvt;

		private List<Type> _maskedEvtTypes;

		private bool _isStopped;

		public List<uint> _actorIdsToCleanUp;

		private bool _dispatchPaused;

		private BaseActor[] _actorsBuffer = new BaseActor[64];

		protected EventManager()
		{
			_actors = new Dictionary<uint, BaseActor>();
			_actorList = new List<BaseActor>();
			_queuedEvents = new List<BaseEvent>();
			_evtListeners = new Dictionary<Type, List<ListenerRegistry>>();
			_maskedEvtTypes = new List<Type>();
			_actorIdsToCleanUp = new List<uint>();
			_categoryActors = new List<BaseActor>[8];
			for (int i = 1; i < _categoryActors.Length; i++)
			{
				_categoryActors[i] = new List<BaseActor>();
			}
		}

		public void InitAtAwake()
		{
		}

		public void InitAtStart()
		{
		}

		public void Core()
		{
			if (_isStopped)
			{
				return;
			}
			for (int i = 0; i < _actorIdsToCleanUp.Count; i++)
			{
				RemoveAllListener(_actorIdsToCleanUp[i]);
			}
			if (_actorIdsToCleanUp.Count > 0)
			{
				_actorIdsToCleanUp.Clear();
			}
			if (_actorList.Count > _actorsBuffer.Length)
			{
				_actorsBuffer = new BaseActor[_actorsBuffer.Length * 2];
			}
			int count = _actorList.Count;
			_actorList.CopyTo(_actorsBuffer);
			for (int j = 0; j < count; j++)
			{
				BaseActor baseActor = _actorsBuffer[j];
				baseActor.Core();
			}
			if (_dispatchPaused)
			{
				return;
			}
			int num = 0;
			do
			{
				int count2 = _queuedEvents.Count;
				for (int k = 0; k < count2; k++)
				{
					DispatchEvent(_queuedEvents[k]);
					if (_isStopped)
					{
						return;
					}
				}
				for (int l = 0; l < count2; l++)
				{
					DispatchListenEvent(_queuedEvents[l]);
					if (_isStopped)
					{
						return;
					}
					Singleton<LevelDesignManager>.Instance.DispatchLevelDesignListenEvent(_queuedEvents[l]);
				}
				_queuedEvents.RemoveRange(0, count2);
				num++;
			}
			while (_queuedEvents.Count > 0);
		}

		public void Destroy()
		{
			DropEventsAndStop();
		}

		public void TryRemoveActor(uint runtimeID)
		{
			BaseActor value;
			if (_actors.TryGetValue(runtimeID, out value))
			{
				value.OnRemoval();
				RemoveActor(value);
			}
		}

		private void RemoveActor(BaseActor actor)
		{
			_actorIdsToCleanUp.Add(actor.runtimeID);
			_actorList.Remove(actor);
			_actors.Remove(actor.runtimeID);
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(actor.runtimeID);
			_categoryActors[num].Remove(actor);
			Singleton<LevelManager>.Instance.gameMode.DestroyRuntimeID(actor.runtimeID);
		}

		protected virtual void DispatchEvent(BaseEvent evt)
		{
			if (_actors.ContainsKey(evt.targetID))
			{
				BaseActor value;
				_actors.TryGetValue(evt.targetID, out value);
				_curParentEvt = evt;
				bool flag = value.OnEvent(evt);
				_curParentEvt = null;
				if (!evt.requireHandle)
				{
				}
			}
		}

		public void SetPauseDispatching(bool paused)
		{
			_dispatchPaused = paused;
		}

		public BaseMonoEntity GetEntity(uint runtimeID)
		{
			if (runtimeID == 562036737)
			{
				return Singleton<LevelManager>.Instance.levelEntity;
			}
			BaseMonoEntity result = null;
			switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(runtimeID))
			{
			case 3:
				result = Singleton<AvatarManager>.Instance.TryGetAvatarByRuntimeID(runtimeID);
				break;
			case 4:
				result = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
				break;
			case 2:
				result = Singleton<CameraManager>.Instance.GetCameraByRuntimeID(runtimeID);
				break;
			case 6:
				result = Singleton<DynamicObjectManager>.Instance.TryGetDynamicObjectByRuntimeID(runtimeID);
				break;
			case 7:
				result = Singleton<PropObjectManager>.Instance.TryGetPropObjectByRuntimeID(runtimeID);
				break;
			}
			return result;
		}

		protected virtual void DispatchListenEvent(BaseEvent evt)
		{
			List<ListenerRegistry> value;
			_evtListeners.TryGetValue(evt.GetType(), out value);
			if (value != null)
			{
				ListenerRegistry[] array = value.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					_curParentEvt = evt;
					_actors[array[i].listenerID].ListenEvent(evt);
					_curParentEvt = null;
				}
			}
		}

		public virtual void FireEvent(BaseEvent evt, MPEventDispatchMode mode = MPEventDispatchMode.Normal)
		{
			if (!_isStopped && (_maskedEvtTypes.Count <= 0 || !_maskedEvtTypes.Contains(evt.GetType())))
			{
				evt.parent = _curParentEvt;
				_queuedEvents.Add(evt);
			}
		}

		public BaseActor GetActor(uint runtimeID)
		{
			BaseActor value;
			_actors.TryGetValue(runtimeID, out value);
			return value;
		}

		public T GetActor<T>(uint runtimeID) where T : BaseActor
		{
			BaseActor value;
			_actors.TryGetValue(runtimeID, out value);
			return value as T;
		}

		public T[] GetActorByCategory<T>(ushort category) where T : BaseActor
		{
			if (category == 1)
			{
				return new T[0];
			}
			T[] array = new T[_categoryActors[category].Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (T)_categoryActors[category][i];
			}
			return array;
		}

		public void RegisterEventListener<T>(uint id)
		{
			Type typeFromHandle = typeof(T);
			if (!_evtListeners.ContainsKey(typeFromHandle))
			{
				_evtListeners.Add(typeFromHandle, new List<ListenerRegistry>());
			}
			List<ListenerRegistry> list = _evtListeners[typeFromHandle];
			bool flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].listenerID == id)
				{
					list[i].registerCount++;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(new ListenerRegistry(id));
			}
		}

		public void RemoveEventListener<T>(uint id)
		{
			Type typeFromHandle = typeof(T);
			List<ListenerRegistry> value;
			_evtListeners.TryGetValue(typeFromHandle, out value);
			if (value == null || value.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < value.Count; i++)
			{
				ListenerRegistry listenerRegistry = value[i];
				if (listenerRegistry.listenerID == id)
				{
					listenerRegistry.registerCount--;
					if (listenerRegistry.registerCount == 0)
					{
						value.Remove(listenerRegistry);
					}
					break;
				}
			}
		}

		private void RemoveAllListener(uint id)
		{
			foreach (List<ListenerRegistry> value in _evtListeners.Values)
			{
				for (int i = 0; i < value.Count; i++)
				{
					if (value[i].listenerID == id)
					{
						value.RemoveAt(i);
					}
				}
			}
		}

		public void MaskEventType(Type t)
		{
			_maskedEvtTypes.Add(t);
		}

		public void UnmaskEventType(Type t)
		{
			_maskedEvtTypes.Remove(t);
		}

		public void DropEventsAndStop()
		{
			_isStopped = true;
			_queuedEvents.Clear();
		}

		public T CreateActor<T>(BaseMonoEntity entity) where T : BaseActor, new()
		{
			T val = new T();
			val.runtimeID = entity.GetRuntimeID();
			val.gameObject = entity.gameObject;
			val.Init(entity);
			_actorList.Add(val);
			_actors.Add(entity.GetRuntimeID(), val);
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(val.runtimeID);
			_categoryActors[num].Add(val);
			ProcessInitedActor(val);
			Singleton<LevelManager>.Instance.gameMode.RegisterRuntimeID(val.runtimeID);
			return val;
		}

		protected virtual void ProcessInitedActor(BaseActor actor)
		{
		}

		protected void InjectEvent(BaseEvent evt)
		{
			_queuedEvents.Add(evt);
		}

		private string GetDebugEventString(BaseEvent evt)
		{
			string text = evt.ToString();
			if (evt.remoteState != EventRemoteState.Idle)
			{
				text = string.Format("[{0}]", evt.remoteState) + text;
			}
			if (evt is BaseLevelEvent)
			{
				text = "+++LevelEvent+++: " + text;
			}
			if (evt.parent != null)
			{
				text = text + " <---- " + evt.parent.GetType().ToString();
			}
			return text;
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("NG_HSOD_DEBUG")]
		private void DebugLogEvent(BaseEvent evt)
		{
		}

		public T[] GetEnemyActorsOf<T>(BaseActor actor) where T : BaseActor
		{
			return Singleton<LevelManager>.Instance.gameMode.GetEnemyActorsOf<T>(actor);
		}

		public T[] GetAlliedActorsOf<T>(BaseActor actor) where T : BaseActor
		{
			return Singleton<LevelManager>.Instance.gameMode.GetAlliedActorsOf<T>(actor);
		}

		public LayerMask GetAbilityTargettingMask(uint ownerID, MixinTargetting targetting)
		{
			return Singleton<LevelManager>.Instance.gameMode.GetAbilityTargettingMask(ownerID, targetting);
		}

		public LayerMask GetAbilityHitboxTargettingMask(uint ownerID, MixinTargetting targetting)
		{
			return Singleton<LevelManager>.Instance.gameMode.GetAbilityHitboxTargettingMask(ownerID, targetting);
		}
	}
}
