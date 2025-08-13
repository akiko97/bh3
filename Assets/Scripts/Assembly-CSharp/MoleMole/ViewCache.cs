using System.Collections.Generic;
using FullInspector;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class ViewCache
	{
		[ShowInInspector]
		private SimpleLRU<ViewCacheEntry> _lruCache;

		[ShowInInspector]
		private List<ViewCacheEntry> _alwaysCache;

		public ViewCache(int lruCapacity)
		{
			_lruCache = new SimpleLRU<ViewCacheEntry>(lruCapacity);
			_alwaysCache = new List<ViewCacheEntry>();
		}

		public GameObject LoadInstancedView(ContextPattern config)
		{
			if (config.dontDestroyView || config.cacheType == ViewCacheType.DontCache)
			{
				return LoadAndInstantiateView(config.viewPrefabPath);
			}
			bool flag = false;
			ViewCacheEntry viewCacheEntry = null;
			for (int i = 0; i < _lruCache.count; i++)
			{
				if (_lruCache[i] != null && _lruCache[i].viewPrefabPath == config.viewPrefabPath)
				{
					viewCacheEntry = _lruCache[i];
					flag = true;
					break;
				}
			}
			if (viewCacheEntry == null)
			{
				for (int j = 0; j < _alwaysCache.Count; j++)
				{
					if (_alwaysCache[j].viewPrefabPath == config.viewPrefabPath)
					{
						viewCacheEntry = _alwaysCache[j];
						flag = true;
						break;
					}
				}
			}
			if (viewCacheEntry == null)
			{
				GameObject instancedGameObject = LoadAndInstantiateView(config.viewPrefabPath);
				ViewCacheEntry viewCacheEntry2 = new ViewCacheEntry();
				viewCacheEntry2.viewPrefabPath = config.viewPrefabPath;
				viewCacheEntry2.instancedGameObject = instancedGameObject;
				viewCacheEntry2.isInUse = true;
				viewCacheEntry = viewCacheEntry2;
			}
			else
			{
				if (viewCacheEntry.isInUse)
				{
					return LoadAndInstantiateView(config.viewPrefabPath);
				}
				viewCacheEntry.isInUse = true;
				viewCacheEntry.instancedGameObject.SetActive(true);
			}
			if (config.cacheType == ViewCacheType.AlwaysCached)
			{
				if (!flag)
				{
					_alwaysCache.Add(viewCacheEntry);
				}
			}
			else if (config.cacheType == ViewCacheType.LRUCached)
			{
				ViewCacheEntry outdated;
				_lruCache.Touch(viewCacheEntry, out outdated);
				if (outdated != null)
				{
					Object.Destroy(outdated.instancedGameObject);
				}
			}
			return viewCacheEntry.instancedGameObject;
		}

		public void ReleaseInstancedView(GameObject view, ContextPattern config)
		{
			if (view == null || string.IsNullOrEmpty(config.viewPrefabPath) || config.dontDestroyView)
			{
				return;
			}
			if (config.cacheType == ViewCacheType.DontCache)
			{
				Object.Destroy(view);
			}
			else if (config.cacheType == ViewCacheType.LRUCached)
			{
				for (int i = 0; i < _lruCache.count; i++)
				{
					if (_lruCache[i] != null && _lruCache[i].viewPrefabPath == config.viewPrefabPath && _lruCache[i].instancedGameObject == view)
					{
						_lruCache[i].isInUse = false;
						view.SetActive(false);
						return;
					}
				}
				Object.Destroy(view);
			}
			else
			{
				if (config.cacheType != ViewCacheType.AlwaysCached)
				{
					return;
				}
				for (int j = 0; j < _alwaysCache.Count; j++)
				{
					if (_alwaysCache[j].viewPrefabPath == config.viewPrefabPath && _alwaysCache[j].instancedGameObject == view)
					{
						_alwaysCache[j].isInUse = false;
						view.SetActive(false);
						return;
					}
				}
				Object.Destroy(view);
			}
		}

		public void ClearLRUCache()
		{
			for (int i = 0; i < _lruCache.count; i++)
			{
				if (_lruCache[i] != null && !string.IsNullOrEmpty(_lruCache[i].viewPrefabPath) && !_lruCache[i].isInUse)
				{
					Object.DestroyImmediate(_lruCache[i].instancedGameObject);
					_lruCache.MarkClear(i);
				}
			}
			_lruCache.Rebuild();
		}

		public void Reset()
		{
			for (int i = 0; i < _lruCache.count; i++)
			{
				if (_lruCache[i] != null && !_lruCache[i].isInUse && _lruCache[i].instancedGameObject != null)
				{
					Object.Destroy(_lruCache[i].instancedGameObject);
				}
			}
			for (int j = 0; j < _alwaysCache.Count; j++)
			{
				if (!_alwaysCache[j].isInUse && _alwaysCache[j].instancedGameObject != null)
				{
					Object.Destroy(_alwaysCache[j].instancedGameObject);
				}
			}
			_lruCache.Clear();
			_alwaysCache.Clear();
		}

		private GameObject LoadAndInstantiateView(string path)
		{
			GameObject original = Miscs.LoadResource<GameObject>(path);
			return Object.Instantiate(original);
		}
	}
}
