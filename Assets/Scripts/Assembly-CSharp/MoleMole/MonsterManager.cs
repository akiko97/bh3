using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonsterManager
	{
		public class PreloadMonsterItem
		{
			public string name;

			public bool occupied;

			public GameObject gameObj;

			public ConfigMonster config;

			public PreloadMonsterItem(string name, GameObject gameObj, ConfigMonster config)
			{
				this.name = name;
				this.gameObj = gameObj;
				this.config = config;
				occupied = true;
			}
		}

		private Dictionary<uint, BaseMonoMonster> _monsterDict;

		private List<BaseMonoMonster> _monsterLs;

		private List<PreloadMonsterItem> _preloadedMonsters;

		private MonsterManager()
		{
			_monsterDict = new Dictionary<uint, BaseMonoMonster>();
			_monsterLs = new List<BaseMonoMonster>();
			_preloadedMonsters = new List<PreloadMonsterItem>();
		}

		public void InitAtAwake()
		{
		}

		public void InitAtStart()
		{
		}

		public void Core()
		{
			RemoveAllRemoveableMonsters();
		}

		public int MonsterCount()
		{
			return _monsterDict.Count;
		}

		public int LivingMonsterCount()
		{
			int num = 0;
			foreach (BaseMonoMonster value in _monsterDict.Values)
			{
				if (value.IsActive())
				{
					num++;
				}
			}
			return num;
		}

		public List<BaseMonoMonster> GetAllMonsters()
		{
			return _monsterLs;
		}

		public BaseMonoMonster GetMonsterByRuntimeID(uint runtimeID)
		{
			return _monsterDict[runtimeID];
		}

		public BaseMonoMonster TryGetMonsterByRuntimeID(uint runtimeID)
		{
			BaseMonoMonster value;
			_monsterDict.TryGetValue(runtimeID, out value);
			return value;
		}

		private bool RemoveMonsterByRuntimeID(uint runtimeID, int lsIx)
		{
			Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
			BaseMonoMonster baseMonoMonster = _monsterDict[runtimeID];
			bool result = _monsterDict.Remove(runtimeID);
			_monsterLs.RemoveAt(lsIx);
			Object.Destroy(baseMonoMonster.gameObject);
			return result;
		}

		public void UnOccupyAllPreloadedMonsters()
		{
			for (int i = 0; i < _preloadedMonsters.Count; i++)
			{
				_preloadedMonsters[i].occupied = false;
			}
		}

		public void DestroyUnOccupiedPreloadMonsters()
		{
			for (int i = 0; i < _preloadedMonsters.Count; i++)
			{
				if (!_preloadedMonsters[i].occupied)
				{
					Object.Destroy(_preloadedMonsters[i].gameObj);
					_preloadedMonsters[i] = null;
				}
			}
			_preloadedMonsters.RemoveAllNulls();
		}

		public List<PreloadMonsterItem> GetPreloadedMonsters()
		{
			return _preloadedMonsters;
		}

		public void PreloadMonster(string monsterName, string typeName, uint uniqueMonsterID = 0, bool disableBehaviorWhenInit = false)
		{
			string text = monsterName + typeName + uniqueMonsterID + disableBehaviorWhenInit;
			for (int i = 0; i < _preloadedMonsters.Count; i++)
			{
				if (_preloadedMonsters[i].name == text && !_preloadedMonsters[i].occupied)
				{
					_preloadedMonsters[i].occupied = true;
					return;
				}
			}
			GameObject original = Miscs.LoadResource<GameObject>(MonsterData.GetPrefabResPath(monsterName, typeName, !GlobalVars.MONSTER_USE_DYNAMIC_BONE || Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi));
			GameObject gameObject = (GameObject)Object.Instantiate(original, InLevelData.CREATE_INIT_POS, Quaternion.identity);
			BaseMonoMonster component = gameObject.GetComponent<BaseMonoMonster>();
			component.PreInit(monsterName, typeName, uniqueMonsterID, disableBehaviorWhenInit);
			gameObject.SetActive(false);
			PreloadMonsterItem item = new PreloadMonsterItem(text, gameObject, component.config);
			_preloadedMonsters.Add(item);
			ConfigMonster monsterConfig = MonsterData.GetMonsterConfig(monsterName, typeName, string.Empty);
			for (int j = 0; j < monsterConfig.CommonArguments.PreloadEffectPatternGroups.Length; j++)
			{
				Singleton<EffectManager>.Instance.PreloadEffectGroup(monsterConfig.CommonArguments.PreloadEffectPatternGroups[j]);
			}
			for (int k = 0; k < monsterConfig.CommonArguments.RequestSoundBankNames.Length; k++)
			{
				Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(component.config.CommonArguments.RequestSoundBankNames[k]);
			}
		}

		public IEnumerator PreloadMonsterAsync(string monsterName, string typeName, uint uniqueMonsterID = 0, bool disableBehaviorWhenInit = false)
		{
			string preloadKey = monsterName + typeName + uniqueMonsterID + disableBehaviorWhenInit;
			for (int ix = 0; ix < _preloadedMonsters.Count; ix++)
			{
				if (_preloadedMonsters[ix].name == preloadKey && !_preloadedMonsters[ix].occupied)
				{
					_preloadedMonsters[ix].occupied = true;
					yield break;
				}
			}
			AsyncAssetRequst resReq = Miscs.LoadResourceAsync(MonsterData.GetPrefabResPath(monsterName, typeName, !GlobalVars.MONSTER_USE_DYNAMIC_BONE || Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi));
			yield return resReq.operation;
			GameObject monsterObj = (GameObject)Object.Instantiate((GameObject)resReq.asset, InLevelData.CREATE_INIT_POS, Quaternion.identity);
			BaseMonoMonster monster = monsterObj.GetComponent<BaseMonoMonster>();
			monster.PreInit(monsterName, typeName, uniqueMonsterID, disableBehaviorWhenInit);
			monsterObj.SetActive(false);
			PreloadMonsterItem item = new PreloadMonsterItem(preloadKey, monsterObj, monster.config);
			_preloadedMonsters.Add(item);
			ConfigMonster monsterConfig = MonsterData.GetMonsterConfig(monsterName, typeName, string.Empty);
			for (int i = 0; i < monsterConfig.CommonArguments.PreloadEffectPatternGroups.Length; i++)
			{
				Singleton<EffectManager>.Instance.PreloadEffectGroup(monsterConfig.CommonArguments.PreloadEffectPatternGroups[i]);
			}
			for (int j = 0; j < monsterConfig.CommonArguments.RequestSoundBankNames.Length; j++)
			{
				Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(monster.config.CommonArguments.RequestSoundBankNames[j]);
			}
		}

		public IEnumerator PreloadMonstersAsync(List<KeyValuePair<string, string>> monsters)
		{
			for (int i = 0; i < monsters.Count; i++)
			{
				KeyValuePair<string, string> keyValuePair = monsters[i];
				string monsterName = keyValuePair.Key;
				string typeName = keyValuePair.Value;
				yield return Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(PreloadMonsterAsync(monsterName, typeName, 0u));
			}
		}

		public IEnumerator PreloadUniqueMonstersAsync(List<uint> monsters)
		{
			for (int i = 0; i < monsters.Count; i++)
			{
				uint uniqueMonsterID = monsters[i];
				UniqueMonsterMetaData metaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
				string monsterName = metaData.monsterName;
				string typeName = metaData.typeName;
				yield return Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(PreloadMonsterAsync(monsterName, typeName, uniqueMonsterID));
			}
		}

		public uint CreateMonster(string monsterName, string typeName, int level, bool isLocal, Vector3 initPos, uint runtimeID, bool isElite = false, uint uniqueMonsterID = 0, bool checkOutsideWall = true, bool disableBehaviorWhenInit = false, int tagID = 0)
		{
			BaseMonoMonster baseMonoMonster = null;
			GameObject gameObject = null;
			if (uniqueMonsterID != 0)
			{
				UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
				monsterName = uniqueMonsterMetaData.monsterName;
				typeName = uniqueMonsterMetaData.typeName;
			}
			string text = monsterName + typeName + uniqueMonsterID + disableBehaviorWhenInit;
			int index = 0;
			int i = 0;
			for (int count = _preloadedMonsters.Count; i < count; i++)
			{
				if (_preloadedMonsters[i].name == text)
				{
					gameObject = _preloadedMonsters[i].gameObj;
					index = i;
					break;
				}
			}
			if (gameObject != null)
			{
				gameObject.SetActive(true);
				_preloadedMonsters.RemoveAt(index);
			}
			else
			{
				GameObject original = Miscs.LoadResource<GameObject>(MonsterData.GetPrefabResPath(monsterName, typeName, !GlobalVars.MONSTER_USE_DYNAMIC_BONE || Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi));
				gameObject = (GameObject)Object.Instantiate(original, InLevelData.CREATE_INIT_POS, Quaternion.identity);
			}
			baseMonoMonster = gameObject.GetComponent<BaseMonoMonster>();
			if (runtimeID == 0)
			{
				runtimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4);
			}
			BaseMonoMonster baseMonoMonster2 = baseMonoMonster;
			bool checkOutsideWall2 = checkOutsideWall;
			baseMonoMonster2.Init(monsterName, typeName, runtimeID, initPos, uniqueMonsterID, null, checkOutsideWall2, isElite, disableBehaviorWhenInit, tagID);
			RegisterMonster(baseMonoMonster);
			MonsterActor monsterActor = Singleton<EventManager>.Instance.CreateActor<MonsterActor>(baseMonoMonster);
			monsterActor.InitLevelData(level, isElite);
			monsterActor.PostInit();
			int j = 0;
			for (int num = baseMonoMonster.config.CommonArguments.RequestSoundBankNames.Length; j < num; j++)
			{
				Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(baseMonoMonster.config.CommonArguments.RequestSoundBankNames[j]);
			}
			return baseMonoMonster.GetRuntimeID();
		}

		public uint CreateMonsterMirror(BaseMonoMonster owner, Vector3 initPos, Vector3 initDir, string AIName, float hpRatio, bool disableBehaviorWhenInit = false)
		{
			GameObject original = Miscs.LoadResource<GameObject>(MonsterData.GetPrefabResPath(owner.MonsterName, owner.TypeName));
			GameObject gameObject = (GameObject)Object.Instantiate(original, initPos, Quaternion.LookRotation(initDir));
			BaseMonoMonster component = gameObject.GetComponent<BaseMonoMonster>();
			bool disableBehaviorWhenInit2 = disableBehaviorWhenInit;
			component.Init(owner.MonsterName, owner.TypeName, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), initPos, owner.uniqueMonsterID, AIName, true, false, disableBehaviorWhenInit2);
			RegisterMonster(component);
			MonsterMirrorActor monsterMirrorActor = Singleton<EventManager>.Instance.CreateActor<MonsterMirrorActor>(component);
			monsterMirrorActor.InitFromMonsterActor(Singleton<EventManager>.Instance.GetActor<MonsterActor>(owner.GetRuntimeID()), hpRatio);
			monsterMirrorActor.PostInit();
			int i = 0;
			for (int num = component.config.CommonArguments.RequestSoundBankNames.Length; i < num; i++)
			{
				Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(component.config.CommonArguments.RequestSoundBankNames[i]);
			}
			return component.GetRuntimeID();
		}

		public void InitMonstersPos(Vector3 monsterOffset)
		{
			List<BaseMonoMonster> allMonsters = GetAllMonsters();
			for (int i = 0; i < allMonsters.Count; i++)
			{
				allMonsters[i].transform.position += monsterOffset;
			}
		}

		public void RegisterMonster(BaseMonoMonster monster)
		{
			_monsterDict.Add(monster.GetRuntimeID(), monster);
			_monsterLs.Add(monster);
		}

		private void RemoveAllRemoveableMonsters()
		{
			for (int i = 0; i < _monsterLs.Count; i++)
			{
				BaseMonoMonster baseMonoMonster = _monsterLs[i];
				if (baseMonoMonster.IsToBeRemove())
				{
					RemoveMonsterByRuntimeID(baseMonoMonster.GetRuntimeID(), i);
					i--;
				}
			}
		}

		public void RemoveAllMonsters()
		{
			int num;
			for (num = 0; num < _monsterLs.Count; num++)
			{
				BaseMonoMonster baseMonoMonster = _monsterLs[num];
				if (!baseMonoMonster.IsToBeRemove())
				{
					baseMonoMonster.SetDied(KillEffect.KillImmediately);
				}
				RemoveMonsterByRuntimeID(baseMonoMonster.GetRuntimeID(), num);
				num--;
			}
		}

		public void SetPause(bool pause)
		{
			foreach (BaseMonoMonster value in _monsterDict.Values)
			{
				value.SetPause(pause);
			}
		}

		public void Destroy()
		{
			for (int i = 0; i < _preloadedMonsters.Count; i++)
			{
				if (_preloadedMonsters[i] != null && _preloadedMonsters[i].gameObj != null)
				{
					Object.DestroyImmediate(_preloadedMonsters[i].gameObj);
				}
			}
			for (int j = 0; j < _monsterLs.Count; j++)
			{
				if (_monsterLs[j] != null)
				{
					Object.DestroyImmediate(_monsterLs[j]);
				}
			}
		}
	}
}
