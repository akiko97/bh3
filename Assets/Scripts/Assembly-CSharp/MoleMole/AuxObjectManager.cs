using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class AuxObjectManager
	{
		private Dictionary<uint, List<MonoAuxObject>> _auxObjectMap;

		public void InitAtAwake()
		{
			_auxObjectMap = new Dictionary<uint, List<MonoAuxObject>>();
		}

		public void InitAtStart()
		{
		}

		public void Core()
		{
		}

		public void Destroy()
		{
		}

		public GameObject LoadAuxObjectProto(string name)
		{
			return Miscs.LoadResource<GameObject>(AuxObjectData.GetAuxObjectPrefabPath(name));
		}

		public MonoAuxObject CreateSimpleAuxObject(string name, uint ownerID)
		{
			return LoadOrAddAuxObject<MonoAuxObject>(name, ownerID, true);
		}

		public T CreateAuxObject<T>(string name) where T : MonoAuxObject
		{
			return LoadOrAddAuxObject<T>(name, 562036737u, false);
		}

		public T CreateAuxObject<T>(string name, uint ownerID) where T : MonoAuxObject
		{
			return LoadOrAddAuxObject<T>(name, ownerID, false);
		}

		private T LoadOrAddAuxObject<T>(string name, uint ownerID, bool addComponent) where T : MonoAuxObject
		{
			GameObject original = LoadAuxObjectProto(name);
			GameObject gameObject = Object.Instantiate(original);
			T val = gameObject.GetComponent<T>();
			if (val == null && addComponent)
			{
				val = gameObject.AddComponent<T>();
			}
			val.entryName = name;
			val.ownerID = ownerID;
			if (!_auxObjectMap.ContainsKey(ownerID))
			{
				_auxObjectMap.Add(ownerID, new List<MonoAuxObject>());
			}
			_auxObjectMap[ownerID].Add(val);
			return val;
		}

		public MonoAuxObject GetAuxObject(uint ownerID, string entryName)
		{
			return GetAuxObject<MonoAuxObject>(ownerID, entryName);
		}

		public T GetAuxObject<T>(uint ownerID, string entryName) where T : MonoAuxObject
		{
			if (!_auxObjectMap.ContainsKey(ownerID))
			{
				return (T)null;
			}
			List<MonoAuxObject> list = _auxObjectMap[ownerID];
			bool flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == null)
				{
					flag = true;
				}
				else if (list[i].entryName == entryName)
				{
					return (T)list[i];
				}
			}
			if (flag)
			{
				list.RemoveAllNulls();
			}
			return (T)null;
		}

		public List<T> GetAuxObjects<T>(uint ownerID) where T : MonoAuxObject
		{
			List<T> list = new List<T>();
			if (!_auxObjectMap.ContainsKey(ownerID))
			{
				return list;
			}
			List<MonoAuxObject> list2 = _auxObjectMap[ownerID];
			bool flag = false;
			for (int i = 0; i < list2.Count; i++)
			{
				if (list2[i] == null)
				{
					flag = true;
					continue;
				}
				T component = list2[i].GetComponent<T>();
				if (component != null)
				{
					list.Add(component);
				}
			}
			if (flag)
			{
				list2.RemoveAllNulls();
			}
			return list;
		}

		public void ClearAuxObjects<T>(uint ownerID) where T : MonoAuxObject
		{
			List<T> auxObjects = GetAuxObjects<T>(ownerID);
			for (int i = 0; i < auxObjects.Count; i++)
			{
				T val = auxObjects[i];
				val.SetDestroy();
			}
		}

		public void ClearHitBoxDetectByOwnerEvade(uint ownerID)
		{
			List<MonoAnimatedHitboxDetect> auxObjects = GetAuxObjects<MonoAnimatedHitboxDetect>(ownerID);
			for (int i = 0; i < auxObjects.Count; i++)
			{
				if (!auxObjects[i].dontDestroyWhenOwnerEvade)
				{
					auxObjects[i].SetDestroy();
				}
			}
		}
	}
}
