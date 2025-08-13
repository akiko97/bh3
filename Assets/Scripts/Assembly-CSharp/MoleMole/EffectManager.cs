using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class EffectManager
	{
		private const string Effect_OUTSIDE_POOL_ROOT_NAME = "EffectOutsidePool";

		private Dictionary<uint, BaseMonoEffect> _effectDict;

		private List<BaseMonoEffect> _effectLs;

		private HashSet<uint> _managedEffectSet;

		private Dictionary<string, GameObject> _effectPrefabCache;

		private Dictionary<string, int> _uniqueEffectPatternMap;

		private List<List<MonoEffect>> _indexedEffectPatterns;

		public EffectPool _effectPool;

		private Transform _effectRootOutsidePool;

		public bool mute;

		private EffectManager()
		{
			_effectDict = new Dictionary<uint, BaseMonoEffect>();
			_effectLs = new List<BaseMonoEffect>();
			_managedEffectSet = new HashSet<uint>();
			_effectPrefabCache = new Dictionary<string, GameObject>();
			_indexedEffectPatterns = new List<List<MonoEffect>>();
			_uniqueEffectPatternMap = new Dictionary<string, int>();
		}

		public void InitAtAwake()
		{
			EnableEffectPool();
		}

		public void EnableEffectPool()
		{
			if (_effectPool == null)
			{
				_effectPool = new EffectPool();
				_effectPool.SetInitialInstanceCount(1);
			}
		}

		public void DisableEffectPool()
		{
			if (_effectPool != null)
			{
				_effectPool.CleanAll(true);
				_effectPool = null;
			}
		}

		public bool IsEffectPoolEnabled()
		{
			return _effectPool != null;
		}

		public void InitAtStart()
		{
		}

		public void Core()
		{
			RemoveAllRemoveableEffects();
		}

		public int EffectCount()
		{
			return _effectDict.Count;
		}

		private bool RemoveEffectByRuntimeID(uint runtimeID, int lsIx)
		{
			BaseMonoEffect baseMonoEffect = _effectDict[runtimeID];
			if (baseMonoEffect != null)
			{
				if (_effectPool != null && baseMonoEffect.isFromEffectPool)
				{
					_effectPool.Despawn(baseMonoEffect.gameObject);
				}
				else
				{
					UnityEngine.Object.Destroy(baseMonoEffect.gameObject);
				}
			}
			bool result = _effectDict.Remove(runtimeID);
			_effectLs.RemoveAt(lsIx);
			return result;
		}

		public void PreloadEffectGroup(string effectGroupName, bool isCommon = false)
		{
			if (_effectPool != null)
			{
				_effectPool.PreloadGroup(effectGroupName, isCommon);
			}
		}

		public void PreloadEffectGroup(string[] effectGroupNames, bool isCommon = false)
		{
			if (_effectPool != null)
			{
				for (int i = 0; i < effectGroupNames.Length; i++)
				{
					_effectPool.PreloadGroup(effectGroupNames[i], isCommon);
				}
			}
		}

		public void UnloadEffectGroups(string[] effectGroupNames)
		{
			if (_effectPool != null)
			{
				_effectPool.UnloadGroup(effectGroupNames);
			}
		}

		public void ReloadEffectPool()
		{
			if (_effectPool == null)
			{
				return;
			}
			_effectPool.ResetClearGroupRefCounts();
			PreloadCommonEffectGroups();
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			foreach (BaseMonoAvatar item in allPlayerAvatars)
			{
				PreloadEffectGroup(item.config.CommonArguments.PreloadEffectPatternGroups);
			}
			List<MonsterManager.PreloadMonsterItem> preloadedMonsters = Singleton<MonsterManager>.Instance.GetPreloadedMonsters();
			foreach (MonsterManager.PreloadMonsterItem item2 in preloadedMonsters)
			{
				PreloadEffectGroup(item2.config.CommonArguments.PreloadEffectPatternGroups);
			}
			_effectPool.UnloadNonRefedGroups();
		}

		public BaseMonoEffect CreateEffectInstance(string effectPath, bool isLocal, Vector3 initPos, Vector3 faceDir, Vector3 initScale)
		{
			BaseMonoEffect baseMonoEffect = null;
			if (mute)
			{
				return null;
			}
			GameObject value;
			_effectPrefabCache.TryGetValue(effectPath, out value);
			if (value == null)
			{
				value = Miscs.LoadResource<GameObject>(EffectData.GetPrefabResPath(effectPath));
				_effectPrefabCache.Add(effectPath, value);
			}
			bool flag = false;
			GameObject gameObject = null;
			if (_effectPool != null)
			{
				GameObject gameObject2 = _effectPool.Spawn(value);
				if (gameObject2 != null)
				{
					gameObject = gameObject2;
				}
			}
			flag = gameObject != null;
			if (!flag)
			{
				gameObject = UnityEngine.Object.Instantiate(value);
				gameObject.transform.parent = GetEffectRootOutsidePool();
				gameObject.name = value.name;
			}
			baseMonoEffect = gameObject.GetComponent<BaseMonoEffect>();
			baseMonoEffect.Init(effectPath, Singleton<RuntimeIDManager>.Instance.GetNextNonSyncedRuntimeID(5), initPos, faceDir, initScale, flag);
			baseMonoEffect.Setup();
			if (baseMonoEffect != null)
			{
				_effectLs.Add(baseMonoEffect);
				_effectDict.Add(baseMonoEffect.GetRuntimeID(), baseMonoEffect);
				return baseMonoEffect;
			}
			throw new Exception("Invalid Type or State!");
		}

		public void AddEffectPrototype(string effectPath, GameObject prototype)
		{
			if (!_effectPrefabCache.ContainsKey(effectPath))
			{
				_effectPrefabCache.Add(effectPath, prototype);
			}
		}

		private void RemoveAllRemoveableEffects()
		{
			for (int i = 0; i < _effectLs.Count; i++)
			{
				BaseMonoEffect baseMonoEffect = _effectLs[i];
				if (baseMonoEffect.IsToBeRemove())
				{
					RemoveEffectByRuntimeID(baseMonoEffect.GetRuntimeID(), i);
					i--;
				}
			}
		}

		private void RemoveAllEffects()
		{
			int num;
			for (num = 0; num < _effectLs.Count; num++)
			{
				BaseMonoEffect baseMonoEffect = _effectLs[num];
				RemoveEffectByRuntimeID(baseMonoEffect.GetRuntimeID(), num);
				num--;
			}
		}

		public void TriggerEntityEffectPatternRaw(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity, out List<MonoEffect> effects)
		{
			MonoEffectOverride component = entity.GetComponent<MonoEffectOverride>();
			if (component != null && component.effectOverrides.ContainsKey(patternName))
			{
				patternName = component.effectOverrides[patternName];
			}
			EffectPattern effectPattern = EffectData.GetEffectPattern(patternName);
			effects = new List<MonoEffect>();
			if (effectPattern.randomOneFromSubs)
			{
				int[] array = new int[effectPattern.subEffects.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = i;
				}
				array.Shuffle();
				for (int j = 0; j < array.Length; j++)
				{
					if (!(component != null) || string.IsNullOrEmpty(effectPattern.subEffects[j].predicate) || component.effectPredicates.Contains(effectPattern.subEffects[j].predicate))
					{
						BaseMonoEffect baseMonoEffect = CreateEffectInstanceBySubEffectConfig(effectPattern.subEffects[j], initPos, initDir, initScale, entity);
						if (baseMonoEffect != null && baseMonoEffect is MonoEffect)
						{
							effects.Add((MonoEffect)baseMonoEffect);
							break;
						}
					}
				}
				return;
			}
			if (effectPattern.subEffects.Length == 1)
			{
				BaseMonoEffect baseMonoEffect2 = CreateEffectInstanceBySubEffectConfig(effectPattern.subEffects[0], initPos, initDir, initScale, entity);
				if (baseMonoEffect2 != null && baseMonoEffect2 is MonoEffect)
				{
					effects.Add((MonoEffect)baseMonoEffect2);
				}
				return;
			}
			for (int k = 0; k < effectPattern.subEffects.Length; k++)
			{
				if (!(component != null) || string.IsNullOrEmpty(effectPattern.subEffects[k].predicate) || component.effectPredicates.Contains(effectPattern.subEffects[k].predicate))
				{
					BaseMonoEffect baseMonoEffect3 = CreateEffectInstanceBySubEffectConfig(effectPattern.subEffects[k], initPos, initDir, initScale, entity);
					if (baseMonoEffect3 != null && baseMonoEffect3 is MonoEffect)
					{
						effects.Add((MonoEffect)baseMonoEffect3);
					}
				}
			}
		}

		private BaseMonoEffect CreateEffectInstanceBySubEffectConfig(SubEffect subEffect, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity)
		{
			Vector3 initPos2 = initPos;
			Vector3 initDir2 = initDir;
			Vector3 initScale2 = initScale;
			if (subEffect.onCreate != null)
			{
				subEffect.onCreate.Process(ref initPos2, ref initDir2, ref initScale2, entity);
			}
			return CreateEffectInstance(subEffect.prefabPath, true, initPos2, initDir2, initScale);
		}

		public void TriggerEntityEffectPatternFromTo(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity fromEntity, BaseMonoEntity toEntity)
		{
			List<MonoEffect> effects;
			TriggerEntityEffectPatternRaw(patternName, initPos, initDir, initScale, fromEntity, out effects);
			for (int i = 0; i < effects.Count; i++)
			{
				MonoEffect monoEffect = effects[i];
				monoEffect.SetOwner(fromEntity);
				monoEffect.SetupPluginFromTo(toEntity);
			}
		}

		public void TriggerEntityEffectPattern(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity)
		{
			List<MonoEffect> effects;
			TriggerEntityEffectPatternRaw(patternName, initPos, initDir, initScale, entity, out effects);
			for (int i = 0; i < effects.Count; i++)
			{
				MonoEffect monoEffect = effects[i];
				monoEffect.SetOwner(entity);
				monoEffect.SetupPlugin();
			}
		}

		public List<MonoEffect> TriggerEntityEffectPatternReturnValue(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity)
		{
			List<MonoEffect> effects;
			TriggerEntityEffectPatternRaw(patternName, initPos, initDir, initScale, entity, out effects);
			for (int i = 0; i < effects.Count; i++)
			{
				MonoEffect monoEffect = effects[i];
				monoEffect.SetOwner(entity);
				monoEffect.SetupPlugin();
			}
			return effects;
		}

		public void TriggerEntityEffectPattern(string patternName, BaseMonoEntity entity, bool ignoreYPosition = true)
		{
			if (ignoreYPosition)
			{
				TriggerEntityEffectPattern(patternName, entity.XZPosition, entity.transform.forward, Vector3.one, entity);
			}
			else
			{
				TriggerEntityEffectPattern(patternName, entity.transform.position, entity.transform.forward, Vector3.one, entity);
			}
		}

		public List<MonoEffect> TriggerEntityEffectPatternReturnValue(string patternName, BaseMonoEntity entity, bool ignoreYPosition = true)
		{
			if (ignoreYPosition)
			{
				return TriggerEntityEffectPatternReturnValue(patternName, entity.XZPosition, entity.transform.forward, Vector3.one, entity);
			}
			return TriggerEntityEffectPatternReturnValue(patternName, entity.transform.position, entity.transform.forward, Vector3.one, entity);
		}

		public GameObject CreateGroupedEffectPattern(string patternName, BaseMonoEntity entity = null)
		{
			if (entity == null)
			{
				entity = Singleton<LevelManager>.Instance.levelEntity;
			}
			GameObject gameObject = new GameObject(patternName);
			gameObject.transform.position = InLevelData.CREATE_INIT_POS;
			Vector3 position = gameObject.transform.position;
			Vector3 forward = gameObject.transform.forward;
			List<MonoEffect> effects;
			TriggerEntityEffectPatternRaw(patternName, position, forward, Vector3.one, entity, out effects);
			for (int i = 0; i < effects.Count; i++)
			{
				MonoEffect monoEffect = effects[i];
				monoEffect.SetOwner(entity);
				monoEffect.SetupPlugin();
				monoEffect.transform.parent = gameObject.transform;
				_managedEffectSet.Add(monoEffect.GetRuntimeID());
			}
			return gameObject;
		}

		public void ClearEffectsByOwner(uint entityID)
		{
			foreach (MonoEffect value in _effectDict.Values)
			{
				if (value != null && value.owner != null && value.owner.GetRuntimeID() == entityID && !_managedEffectSet.Contains(value.GetRuntimeID()))
				{
					value.SetDestroy();
				}
			}
		}

		public void ClearEffectsByOwnerImmediately(uint entityID)
		{
			foreach (MonoEffect value in _effectDict.Values)
			{
				if (value != null && value.owner != null && value.owner.GetRuntimeID() == entityID && !_managedEffectSet.Contains(value.GetRuntimeID()))
				{
					value.SetDestroyImmediately();
				}
			}
		}

		public List<MonoEffect> GetEffectsByOwner(uint entityID)
		{
			List<MonoEffect> list = new List<MonoEffect>();
			foreach (MonoEffect value in _effectDict.Values)
			{
				if (value.owner != null && value.owner.GetRuntimeID() == entityID)
				{
					list.Add(value);
				}
			}
			return list;
		}

		public int CreateIndexedEntityEffectPattern(string patternName, BaseMonoEntity entity)
		{
			return CreateIndexedEntityEffectPattern(patternName, entity.XZPosition, entity.transform.forward, Vector3.one, entity);
		}

		public int CreateIndexedEntityEffectPatternWithOffset(string patternName, BaseMonoEntity entity, Vector3 offset)
		{
			return CreateIndexedEntityEffectPattern(patternName, entity.XZPosition + offset, entity.transform.forward, Vector3.one, entity);
		}

		public int CreateIndexedEntityEffectPattern(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity)
		{
			List<MonoEffect> effects;
			TriggerEntityEffectPatternRaw(patternName, initPos, initDir, initScale, entity, out effects);
			for (int i = 0; i < effects.Count; i++)
			{
				MonoEffect monoEffect = effects[i];
				monoEffect.SetOwner(entity);
				monoEffect.SetupPlugin();
				_managedEffectSet.Add(monoEffect.GetRuntimeID());
			}
			int num = _indexedEffectPatterns.SeekAddPosition();
			_indexedEffectPatterns[num] = effects;
			return num;
		}

		public void SetIndexedEntityEffectPatternActive(int patternIx, bool active)
		{
			if (patternIx >= _indexedEffectPatterns.Count)
			{
				return;
			}
			List<MonoEffect> list = _indexedEffectPatterns[patternIx];
			for (int i = 0; i < list.Count; i++)
			{
				if (!(list[i] == null))
				{
					list[i].gameObject.SetActive(active);
				}
			}
		}

		public List<MonoEffect> GetIndexedEntityEffectPattern(int patternIx)
		{
			if (patternIx < _indexedEffectPatterns.Count)
			{
				return _indexedEffectPatterns[patternIx];
			}
			return null;
		}

		public void SetDestroyIndexedEffectPattern(int patternIx)
		{
			if (patternIx >= _indexedEffectPatterns.Count)
			{
				return;
			}
			List<MonoEffect> list = _indexedEffectPatterns[patternIx];
			for (int i = 0; i < list.Count; i++)
			{
				if (!(list[i] == null))
				{
					list[i].SetDestroy();
					_managedEffectSet.Remove(list[i].GetRuntimeID());
				}
			}
			_indexedEffectPatterns[patternIx] = null;
		}

		public void SetDestroyImmediatelyIndexedEffectPattern(int patternIx)
		{
			if (patternIx >= _indexedEffectPatterns.Count)
			{
				return;
			}
			List<MonoEffect> list = _indexedEffectPatterns[patternIx];
			for (int i = 0; i < list.Count; i++)
			{
				if (!(list[i] == null))
				{
					list[i].SetDestroyImmediately();
					_managedEffectSet.Remove(list[i].GetRuntimeID());
				}
			}
			_indexedEffectPatterns[patternIx] = null;
		}

		public void ResetEffectPatternPosition(int patternIx, Vector3 pos)
		{
			if (patternIx >= _indexedEffectPatterns.Count)
			{
				return;
			}
			List<MonoEffect> list = _indexedEffectPatterns[patternIx];
			for (int i = 0; i < list.Count; i++)
			{
				if (!(list[i] == null))
				{
					list[i].transform.position = pos;
				}
			}
		}

		public void CreateUniqueIndexedEffectPattern(string patternName, string uniqueKeyName, BaseMonoEntity entity)
		{
			int value = CreateIndexedEntityEffectPattern(patternName, entity);
			if (!_uniqueEffectPatternMap.ContainsKey(uniqueKeyName))
			{
				_uniqueEffectPatternMap.Add(uniqueKeyName, value);
			}
		}

		public void SetDestroyUniqueIndexedEffectPattern(string uniqueKeyName)
		{
			int destroyIndexedEffectPattern = _uniqueEffectPatternMap[uniqueKeyName];
			SetDestroyIndexedEffectPattern(destroyIndexedEffectPattern);
			_uniqueEffectPatternMap.Remove(uniqueKeyName);
		}

		public bool TrySetDestroyUniqueIndexedEffectPattern(string uniqueKeyName)
		{
			if (!_uniqueEffectPatternMap.ContainsKey(uniqueKeyName))
			{
				return false;
			}
			SetDestroyUniqueIndexedEffectPattern(uniqueKeyName);
			return true;
		}

		public void Clear()
		{
			RemoveAllEffects();
			_effectPool.CleanAll(false);
			ClearInternal();
		}

		public void Destroy()
		{
			RemoveAllEffects();
			DisableEffectPool();
			ClearInternal();
		}

		private void ClearInternal()
		{
			_effectDict.Clear();
			_effectLs.Clear();
			_managedEffectSet.Clear();
			_effectPrefabCache.Clear();
			_uniqueEffectPatternMap.Clear();
			_indexedEffectPatterns.Clear();
			if (_effectRootOutsidePool != null)
			{
				UnityEngine.Object.Destroy(_effectRootOutsidePool.gameObject);
				_effectRootOutsidePool = null;
			}
		}

		private void ClearEffectPrefabCache()
		{
			_effectPrefabCache.Clear();
		}

		private void PreloadCommonEffectGroups()
		{
			PreloadEffectGroup("InLevelUI_Effects", true);
			PreloadEffectGroup("Common_Effects", true);
			PreloadEffectGroup("Monster_Common_Effects", true);
			PreloadEffectGroup("Ability_Effects", true);
			PreloadEffectGroup("DynamicObject_Effects", true);
			PreloadEffectGroup("PropCommon_Effects", true);
		}

		private Transform GetEffectRootOutsidePool()
		{
			if (_effectRootOutsidePool != null)
			{
				return _effectRootOutsidePool;
			}
			GameObject gameObject = GameObject.Find("EffectOutsidePool");
			if (gameObject != null)
			{
				_effectRootOutsidePool = gameObject.transform;
				return _effectRootOutsidePool;
			}
			_effectRootOutsidePool = new GameObject("EffectOutsidePool").transform;
			return _effectRootOutsidePool;
		}

		public void SetAllParticleSystemVisible(bool visible)
		{
			ParticleSystem[] array = UnityEngine.Object.FindObjectsOfType<ParticleSystem>();
			foreach (ParticleSystem particleSystem in array)
			{
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				emission.enabled = visible;
			}
		}

		public void SetEffectPoolEnabled(bool enabled)
		{
			if (_effectPool != null)
			{
				_effectPool.SetEnabled(enabled);
			}
		}

		public List<BaseMonoEffect> GetEffectList()
		{
			return _effectLs;
		}

		public void SetAllAliveEffectPause(bool pause)
		{
			List<BaseMonoEffect> effectList = GetEffectList();
			foreach (BaseMonoEffect item in effectList)
			{
				MonoEffect monoEffect = item as MonoEffect;
				if (monoEffect != null)
				{
					if (pause)
					{
						monoEffect.Pause();
					}
					else
					{
						monoEffect.Resume();
					}
				}
			}
		}
	}
}
