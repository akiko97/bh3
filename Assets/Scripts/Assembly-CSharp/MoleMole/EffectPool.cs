using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class EffectPool
	{
		private class ProtoPoolItem
		{
			public bool isCommon;

			public List<GameObject> occupiedList = new List<GameObject>();

			public List<GameObject> availableList = new List<GameObject>();

			public bool ReachMaxCount()
			{
				return availableList.Count == 0 && occupiedList.Count + availableList.Count >= ProtoPoolItemMaxCount;
			}
		}

		private class PreloadGroupEntry
		{
			public string groupName;

			public int refCount;

			public bool isCommon;

			public PreloadGroupEntry(string name, bool isCommon)
			{
				groupName = name;
				refCount = 1;
				this.isCommon = isCommon;
			}
		}

		private const string ROOT_NAME = "EffectPool";

		private static int ProtoPoolItemMaxCount = 10;

		private int _initialInstanceCount = 1;

		private Dictionary<string, ProtoPoolItem> dictInstanceCache = new Dictionary<string, ProtoPoolItem>();

		private Transform _instanceRoot;

		private List<PreloadGroupEntry> _preloadedEffectGroupNames = new List<PreloadGroupEntry>();

		public void SetInitialInstanceCount(int cnt)
		{
			_initialInstanceCount = cnt;
		}

		public void PreloadGroup(string groupName, bool isCommon)
		{
			for (int i = 0; i < _preloadedEffectGroupNames.Count; i++)
			{
				if (_preloadedEffectGroupNames[i].groupName == groupName)
				{
					_preloadedEffectGroupNames[i].refCount++;
					return;
				}
			}
			_preloadedEffectGroupNames.Add(new PreloadGroupEntry(groupName, isCommon));
			if (!EffectData.HasEffectPattern(groupName))
			{
				return;
			}
			EffectPattern[] effectGroupPatterns = EffectData.GetEffectGroupPatterns(groupName);
			foreach (EffectPattern effectPattern in effectGroupPatterns)
			{
				for (int k = 0; k < effectPattern.subEffects.Length; k++)
				{
					SubEffect subEffect = effectPattern.subEffects[k];
					GameObject gameObject = Miscs.LoadResource<GameObject>(EffectData.GetPrefabResPath(subEffect.prefabPath));
					if (!(gameObject == null))
					{
						Singleton<EffectManager>.Instance.AddEffectPrototype(subEffect.prefabPath, gameObject);
						PreloadSingleEffect(gameObject, isCommon);
					}
				}
			}
		}

		public IEnumerator PreloadGroupAsync(string groupName, bool isCommon)
		{
			for (int ix = 0; ix < _preloadedEffectGroupNames.Count; ix++)
			{
				if (_preloadedEffectGroupNames[ix].groupName == groupName)
				{
					_preloadedEffectGroupNames[ix].refCount++;
					yield break;
				}
			}
			_preloadedEffectGroupNames.Add(new PreloadGroupEntry(groupName, isCommon));
			if (!EffectData.HasEffectPattern(groupName))
			{
				yield break;
			}
			EffectPattern[] effectPatterns = EffectData.GetEffectGroupPatterns(groupName);
			foreach (EffectPattern effectPattern in effectPatterns)
			{
				for (int jx = 0; jx < effectPattern.subEffects.Length; jx++)
				{
					SubEffect subEffect = effectPattern.subEffects[jx];
					GameObject proto = Miscs.LoadResource<GameObject>(EffectData.GetPrefabResPath(subEffect.prefabPath));
					if (!(proto == null))
					{
						Singleton<EffectManager>.Instance.AddEffectPrototype(subEffect.prefabPath, proto);
						PreloadSingleEffect(proto, isCommon);
						yield return null;
					}
				}
			}
		}

		public void UnloadGroup(string[] groupNames)
		{
			for (int i = 0; i < _preloadedEffectGroupNames.Count; i++)
			{
				PreloadGroupEntry preloadGroupEntry = _preloadedEffectGroupNames[i];
				if (Miscs.ArrayContains(groupNames, preloadGroupEntry.groupName))
				{
					preloadGroupEntry.refCount--;
					if (preloadGroupEntry.refCount == 0)
					{
						_preloadedEffectGroupNames[i] = null;
						DestroyGroupByName(preloadGroupEntry.groupName);
					}
				}
			}
			_preloadedEffectGroupNames.RemoveAllNulls();
		}

		public void ResetClearGroupRefCounts()
		{
			for (int i = 0; i < _preloadedEffectGroupNames.Count; i++)
			{
				_preloadedEffectGroupNames[i].refCount = 0;
			}
		}

		public void UnloadNonRefedGroups()
		{
			for (int i = 0; i < _preloadedEffectGroupNames.Count; i++)
			{
				PreloadGroupEntry preloadGroupEntry = _preloadedEffectGroupNames[i];
				if (preloadGroupEntry.refCount == 0)
				{
					_preloadedEffectGroupNames[i] = null;
					DestroyGroupByName(preloadGroupEntry.groupName);
				}
			}
			_preloadedEffectGroupNames.RemoveAllNulls();
		}

		public void DestroyGroupByName(string groupName)
		{
			if (!EffectData.HasEffectPattern(groupName))
			{
				return;
			}
			EffectPattern[] effectGroupPatterns = EffectData.GetEffectGroupPatterns(groupName);
			foreach (EffectPattern effectPattern in effectGroupPatterns)
			{
				for (int j = 0; j < effectPattern.subEffects.Length; j++)
				{
					CleanByEffectName(Miscs.GetBaseName(effectPattern.subEffects[j].prefabPath));
				}
			}
		}

		public GameObject Spawn(GameObject proto)
		{
			GameObject result = null;
			ProtoPoolItem poolItem = GetPoolItem(proto);
			if (poolItem.ReachMaxCount())
			{
				return result;
			}
			if (poolItem.availableList.Count == 0)
			{
				result = Object.Instantiate(proto);
				result.name = proto.name;
				result.transform.parent = GetEffectPoolRoot();
			}
			else
			{
				result = poolItem.availableList[0];
				poolItem.availableList.RemoveAt(0);
			}
			poolItem.occupiedList.Add(result);
			result.SetActive(true);
			return result;
		}

		public void Despawn(GameObject inst)
		{
			inst.SetActive(false);
			if (dictInstanceCache.ContainsKey(inst.name))
			{
				ProtoPoolItem protoPoolItem = dictInstanceCache[inst.name];
				protoPoolItem.occupiedList.Remove(inst);
				protoPoolItem.availableList.Add(inst);
			}
		}

		public void CleanByEffectName(string name)
		{
			ProtoPoolItem value;
			dictInstanceCache.TryGetValue(name, out value);
			if (value != null)
			{
				int i = 0;
				for (int count = value.availableList.Count; i < count; i++)
				{
					Object.Destroy(value.availableList[i]);
				}
				int j = 0;
				for (int count2 = value.occupiedList.Count; j < count2; j++)
				{
					Object.Destroy(value.occupiedList[j]);
				}
				dictInstanceCache.Remove(name);
			}
		}

		public void CleanAll(bool includeCommon)
		{
			List<string> list = new List<string>(dictInstanceCache.Keys);
			foreach (string item in list)
			{
				ProtoPoolItem protoPoolItem = dictInstanceCache[item];
				if (includeCommon || !protoPoolItem.isCommon)
				{
					int i = 0;
					for (int count = protoPoolItem.availableList.Count; i < count; i++)
					{
						Object.Destroy(protoPoolItem.availableList[i]);
					}
					protoPoolItem.availableList.Clear();
					int j = 0;
					for (int count2 = protoPoolItem.occupiedList.Count; j < count2; j++)
					{
						Object.Destroy(protoPoolItem.occupiedList[j]);
					}
					protoPoolItem.occupiedList.Clear();
					dictInstanceCache.Remove(item);
				}
			}
			if (includeCommon)
			{
				dictInstanceCache.Clear();
				if (_instanceRoot != null)
				{
					Object.Destroy(_instanceRoot.gameObject);
					_instanceRoot = null;
				}
				_preloadedEffectGroupNames.Clear();
				return;
			}
			for (int k = 0; k < _preloadedEffectGroupNames.Count; k++)
			{
				PreloadGroupEntry preloadGroupEntry = _preloadedEffectGroupNames[k];
				if (!preloadGroupEntry.isCommon)
				{
					_preloadedEffectGroupNames[k] = null;
				}
			}
			_preloadedEffectGroupNames.RemoveAllNulls();
		}

		private void PreloadSingleEffect(GameObject proto, bool isCommon)
		{
			if (!dictInstanceCache.ContainsKey(proto.name))
			{
				ProtoPoolItem protoPoolItem = new ProtoPoolItem();
				protoPoolItem.isCommon = isCommon;
				dictInstanceCache.Add(proto.name, protoPoolItem);
				for (int i = 0; i < _initialInstanceCount; i++)
				{
					GameObject gameObject = Object.Instantiate(proto);
					gameObject.transform.parent = GetEffectPoolRoot();
					gameObject.name = proto.name;
					gameObject.SetActive(false);
					protoPoolItem.availableList.Add(gameObject);
				}
			}
		}

		private ProtoPoolItem GetPoolItem(GameObject proto)
		{
			if (!dictInstanceCache.ContainsKey(proto.name))
			{
				PreloadSingleEffect(proto, false);
			}
			return dictInstanceCache[proto.name];
		}

		private Transform GetEffectPoolRoot()
		{
			if (_instanceRoot != null)
			{
				return _instanceRoot;
			}
			GameObject gameObject = GameObject.Find("EffectPool");
			if (gameObject != null)
			{
				_instanceRoot = gameObject.transform;
				return _instanceRoot;
			}
			_instanceRoot = new GameObject("EffectPool").transform;
			Object.DontDestroyOnLoad(_instanceRoot);
			return _instanceRoot;
		}

		public void SetEnabled(bool enabled)
		{
			Transform effectPoolRoot = GetEffectPoolRoot();
			if (effectPoolRoot != null)
			{
				effectPoolRoot.gameObject.SetActive(enabled);
			}
		}

		public List<MonoEffect> GetEffectAliveEffect()
		{
			List<MonoEffect> result = new List<MonoEffect>();
			Transform effectPoolRoot = GetEffectPoolRoot();
			if (effectPoolRoot != null)
			{
				effectPoolRoot.GetComponentsInChildren(false, result);
			}
			return result;
		}
	}
}
