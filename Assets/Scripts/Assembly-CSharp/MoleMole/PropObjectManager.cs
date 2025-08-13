using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class PropObjectManager
	{
		private Dictionary<uint, BaseMonoPropObject> _propObjects;

		private List<BaseMonoPropObject> _propLs;

		private List<uint> _propsToDestroyOnStageChange;

		private PropObjectManager()
		{
			_propObjects = new Dictionary<uint, BaseMonoPropObject>();
			_propLs = new List<BaseMonoPropObject>();
			_propsToDestroyOnStageChange = new List<uint>();
		}

		public void InitAtAwake()
		{
		}

		public void InitAtStart()
		{
		}

		public void Core()
		{
			RemoveAllRemovables();
		}

		public void Destroy()
		{
			for (int i = 0; i < _propLs.Count; i++)
			{
				if (_propLs[i] != null)
				{
					Object.DestroyImmediate(_propLs[i]);
				}
			}
		}

		public BaseMonoPropObject GetPropObjectByRuntimeID(uint runtimeID)
		{
			return _propObjects[runtimeID];
		}

		public BaseMonoPropObject TryGetPropObjectByRuntimeID(uint runtimeID)
		{
			BaseMonoPropObject value;
			_propObjects.TryGetValue(runtimeID, out value);
			return value;
		}

		public List<BaseMonoPropObject> GetAllPropObjects()
		{
			List<BaseMonoPropObject> list = new List<BaseMonoPropObject>();
			list.AddRange(_propObjects.Values);
			return list;
		}

		public uint CreatePropObject(uint ownerID, string propName, float HP, float attack, Vector3 initPos, Vector3 initDir, bool appearAnim = false)
		{
			ConfigPropObject propObjectConfig = PropObjectData.GetPropObjectConfig(propName);
			GameObject original = Miscs.LoadResource<GameObject>(propObjectConfig.PrefabPath);
			BaseMonoPropObject component = Object.Instantiate(original).GetComponent<BaseMonoPropObject>();
			uint nextRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(7);
			component.transform.position = initPos;
			component.transform.forward = initDir;
			component.Init(ownerID, nextRuntimeID, propObjectConfig.Name, appearAnim);
			if (propName == "JokeBox" && attack <= 0f)
			{
				(component as MonoBarrelProp)._toExplode = false;
			}
			_propObjects.Add(nextRuntimeID, component);
			_propLs.Add(component);
			PropObjectActor propObjectActor = Singleton<EventManager>.Instance.CreateActor<PropObjectActor>(component);
			propObjectActor.ownerID = ownerID;
			propObjectActor.InitProp(HP, attack);
			propObjectActor.PostInit();
			return nextRuntimeID;
		}

		public void RemoveAllRemovables()
		{
			for (int i = 0; i < _propLs.Count; i++)
			{
				BaseMonoPropObject baseMonoPropObject = _propLs[i];
				if (baseMonoPropObject.IsToBeRemove())
				{
					RemovePropObjectByRuntimeID(baseMonoPropObject.GetRuntimeID(), i);
					i--;
				}
			}
		}

		public void RemoveAllPropObjects()
		{
			int num;
			for (num = 0; num < _propLs.Count; num++)
			{
				BaseMonoPropObject baseMonoPropObject = _propLs[num];
				if (!baseMonoPropObject.IsToBeRemove())
				{
					baseMonoPropObject.SetDied(KillEffect.KillImmediately);
				}
				RemovePropObjectByRuntimeID(baseMonoPropObject.GetRuntimeID(), num);
				num--;
			}
		}

		private void RemovePropObjectByRuntimeID(uint runtimeID, int lsIx)
		{
			Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
			if (_propObjects[runtimeID] != null)
			{
				Object.Destroy(_propObjects[runtimeID].gameObject);
			}
			_propObjects.Remove(runtimeID);
			_propLs.RemoveAt(lsIx);
		}

		public void CleanWhenStageChange()
		{
			RemoveAllPropObjects();
			_propsToDestroyOnStageChange.Clear();
		}

		public void RegisterDestroyOnStageChange(uint propID)
		{
			_propsToDestroyOnStageChange.Add(propID);
		}
	}
}
