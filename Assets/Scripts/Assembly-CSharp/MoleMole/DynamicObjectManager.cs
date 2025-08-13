using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class DynamicObjectManager
	{
		public class PreloadDynamicObjectPrototype
		{
			public string type;

			public GameObject gameObj;

			public PreloadDynamicObjectPrototype(string type, GameObject gameObj)
			{
				this.type = type;
				this.gameObj = gameObj;
			}
		}

		private Dictionary<uint, BaseMonoDynamicObject> _dynamicObjects;

		private List<PreloadDynamicObjectPrototype> _preloadDynamicObjectProtos;

		private List<BaseMonoDynamicObject> _dynamicLs;

		private DynamicObjectManager()
		{
			_dynamicObjects = new Dictionary<uint, BaseMonoDynamicObject>();
			_dynamicLs = new List<BaseMonoDynamicObject>();
		}

		public void InitAtAwake()
		{
		}

		public void InitAtStart()
		{
			PreloadDynamicObjectPrototypes();
		}

		public void Core()
		{
			RemoveAllRemoveables();
		}

		public void Destroy()
		{
			for (int i = 0; i < _dynamicLs.Count; i++)
			{
				if (_dynamicLs[i] != null)
				{
					Object.DestroyImmediate(_dynamicLs[i]);
				}
			}
			_preloadDynamicObjectProtos.Clear();
			_preloadDynamicObjectProtos = null;
		}

		public BaseMonoDynamicObject GetDynamicObjectByRuntimeID(uint runtimeID)
		{
			return _dynamicObjects[runtimeID];
		}

		public BaseMonoDynamicObject TryGetDynamicObjectByRuntimeID(uint runtimeID)
		{
			BaseMonoDynamicObject value;
			_dynamicObjects.TryGetValue(runtimeID, out value);
			return value;
		}

		public List<MonoGoods> GetAllMonoGoods()
		{
			List<MonoGoods> list = new List<MonoGoods>();
			foreach (KeyValuePair<uint, BaseMonoDynamicObject> dynamicObject in _dynamicObjects)
			{
				if (dynamicObject.Value is MonoGoods)
				{
					list.Add((MonoGoods)dynamicObject.Value);
				}
			}
			return list;
		}

		public List<BaseMonoDynamicObject> GetAllNavigationArrows()
		{
			List<BaseMonoDynamicObject> list = new List<BaseMonoDynamicObject>();
			for (int i = 0; i < _dynamicLs.Count; i++)
			{
				if (_dynamicLs[i].dynamicType == BaseMonoDynamicObject.DynamicType.NavigationArrow)
				{
					list.Add(_dynamicLs[i]);
				}
			}
			return list;
		}

		public void CleanWhenStageChange()
		{
			foreach (KeyValuePair<uint, BaseMonoDynamicObject> dynamicObject in _dynamicObjects)
			{
				if (!dynamicObject.Value.IsToBeRemove() && !dynamicObject.Value.IsOwnerStaticInScene())
				{
					dynamicObject.Value.SetDied();
				}
			}
		}

		public void SetDynamicObjectsVisibility(bool visible)
		{
			foreach (KeyValuePair<uint, BaseMonoDynamicObject> dynamicObject in _dynamicObjects)
			{
				if (!dynamicObject.Value.IsToBeRemove())
				{
					dynamicObject.Value.gameObject.SetActive(visible);
				}
			}
		}

		public void SetDynamicObjectsVisibility<T>(bool visible) where T : BaseMonoDynamicObject
		{
			foreach (KeyValuePair<uint, BaseMonoDynamicObject> dynamicObject in _dynamicObjects)
			{
				if (dynamicObject.Value is T && !dynamicObject.Value.IsToBeRemove())
				{
					dynamicObject.Value.gameObject.SetActive(visible);
				}
			}
		}

		public void SetDynamicObjectsVisibilityExept<T>(bool visible) where T : BaseMonoDynamicObject
		{
			foreach (KeyValuePair<uint, BaseMonoDynamicObject> dynamicObject in _dynamicObjects)
			{
				if (!(dynamicObject.Value is T) && !dynamicObject.Value.IsToBeRemove())
				{
					dynamicObject.Value.gameObject.SetActive(visible);
				}
			}
		}

		private BaseMonoDynamicObject CreateDynamicObjectEntityInstance(uint ownerID, string type, uint runtimeID)
		{
			GameObject dynamicObjectPrototype = GetDynamicObjectPrototype(type);
			GameObject gameObject = Object.Instantiate(dynamicObjectPrototype);
			BaseMonoDynamicObject component = gameObject.GetComponent<BaseMonoDynamicObject>();
			component.Init(runtimeID, ownerID);
			_dynamicObjects.Add(runtimeID, component);
			_dynamicLs.Add(component);
			return component;
		}

		private BaseMonoDynamicObject CreateDynamicObjectEntityInstance(uint ownerID, string type, Vector3 initPos, Vector3 initDir, uint runtimeID)
		{
			BaseMonoDynamicObject baseMonoDynamicObject = CreateDynamicObjectEntityInstance(ownerID, type, runtimeID);
			baseMonoDynamicObject.transform.position = initPos;
			baseMonoDynamicObject.transform.forward = initDir;
			return baseMonoDynamicObject;
		}

		private T CreateDynamicObjectEntityInstance<T>(uint ownerID, string type, uint runtimeID) where T : BaseMonoDynamicObject
		{
			return (T)CreateDynamicObjectEntityInstance(ownerID, type, runtimeID);
		}

		private T CreateDynamicObjectEntityInstance<T>(uint ownerID, string type, Vector3 initPos, Vector3 initDir, uint runtimeID) where T : BaseMonoDynamicObject
		{
			return (T)CreateDynamicObjectEntityInstance(ownerID, type, initPos, initDir, runtimeID);
		}

		private GameObject GetDynamicObjectPrototype(string type)
		{
			foreach (PreloadDynamicObjectPrototype preloadDynamicObjectProto in _preloadDynamicObjectProtos)
			{
				if (preloadDynamicObjectProto.type == type)
				{
					return preloadDynamicObjectProto.gameObj;
				}
			}
			return PreloadDynamicObject(type);
		}

		private void PreloadDynamicObjectPrototypes()
		{
			_preloadDynamicObjectProtos = new List<PreloadDynamicObjectPrototype>();
			ConfigDynamicObjectRegistry dynamicObjectRegistry = DynamicObjectData.GetDynamicObjectRegistry("Entities/DynamicObject/Data/DynamicObject_Level");
			for (int i = 0; i < dynamicObjectRegistry.entries.Length; i++)
			{
				DynamicObjectEntry dynamicObjectEntry = dynamicObjectRegistry.entries[i];
				PreloadDynamicObject(dynamicObjectEntry.name, dynamicObjectEntry.prefabPath);
			}
		}

		private GameObject PreloadDynamicObject(string type, string prefabPath)
		{
			GameObject gameObject = Miscs.LoadResource<GameObject>(prefabPath);
			if (gameObject != null)
			{
				_preloadDynamicObjectProtos.Add(new PreloadDynamicObjectPrototype(type, gameObject));
			}
			return gameObject;
		}

		private GameObject PreloadDynamicObject(string type)
		{
			return PreloadDynamicObject(type, DynamicObjectData.dynamicObjectDict[type]);
		}

		private BaseMonoDynamicObject RegisterAsDynamicObject(uint ownerID, GameObject go)
		{
			uint nextRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(6);
			BaseMonoDynamicObject component = go.GetComponent<BaseMonoDynamicObject>();
			component.Init(nextRuntimeID, ownerID);
			_dynamicObjects.Add(nextRuntimeID, component);
			_dynamicLs.Add(component);
			return component;
		}

		private void RemoveAllRemoveables()
		{
			for (int i = 0; i < _dynamicLs.Count; i++)
			{
				BaseMonoDynamicObject baseMonoDynamicObject = _dynamicLs[i];
				if (baseMonoDynamicObject.IsToBeRemove())
				{
					RemoveDynamicObjectByRuntimeID(baseMonoDynamicObject.GetRuntimeID(), i);
					i--;
				}
			}
		}

		public void RemoveAllDynamicObjects()
		{
			int num;
			for (num = 0; num < _dynamicLs.Count; num++)
			{
				BaseMonoDynamicObject baseMonoDynamicObject = _dynamicLs[num];
				if (!baseMonoDynamicObject.IsToBeRemove())
				{
					baseMonoDynamicObject.SetDied();
				}
				RemoveDynamicObjectByRuntimeID(baseMonoDynamicObject.GetRuntimeID(), num);
				num--;
			}
		}

		private void RemoveDynamicObjectByRuntimeID(uint runtimeID, int lsIx)
		{
			Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
			if (_dynamicObjects[runtimeID] != null)
			{
				Object.Destroy(_dynamicObjects[runtimeID].gameObject);
			}
			_dynamicObjects.Remove(runtimeID);
			_dynamicLs.RemoveAt(lsIx);
		}

		public uint CreateStageExitField(uint ownerID, Vector3 initPos, Vector3 initDir)
		{
			MonoTriggerField monoTriggerField = CreateDynamicObjectEntityInstance<MonoTriggerField>(ownerID, "StageExitField", initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			monoTriggerField.SetCollisionMask(1 << InLevelData.AVATAR_LAYER);
			StageExitFieldActor stageExitFieldActor = Singleton<EventManager>.Instance.CreateActor<StageExitFieldActor>(monoTriggerField);
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Prop_LevelGoal", monoTriggerField);
			return stageExitFieldActor.runtimeID;
		}

		public uint CreateMonsterExitField(uint ownerID, Vector3 initPos, Vector3 initDir, bool forDefendMode = false)
		{
			MonoTriggerField monoTriggerField = CreateDynamicObjectEntityInstance<MonoTriggerField>(ownerID, "StageMonsterExitField", initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			monoTriggerField.SetCollisionMask(1 << InLevelData.MONSTER_LAYER);
			MonsterExitFieldActor monsterExitFieldActor = Singleton<EventManager>.Instance.CreateActor<MonsterExitFieldActor>(monoTriggerField);
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Prop_LevelMonsterGoal", monoTriggerField);
			if (forDefendMode)
			{
				Singleton<LevelManager>.Instance.levelActor.AddTriggerFieldInDefendMode(monsterExitFieldActor);
			}
			return monsterExitFieldActor.runtimeID;
		}

		public uint CreateEvadeDummy(uint ownerID, string evadeDummyName, Vector3 initPos, Vector3 initDir)
		{
			BaseMonoDynamicObject baseMonoDynamicObject = CreateDynamicObjectEntityInstance(ownerID, evadeDummyName, initPos, initDir, GetNextNonSyncedDynamicObjectRuntimeID());
			baseMonoDynamicObject.dynamicType = BaseMonoDynamicObject.DynamicType.EvadeDummy;
			EvadeEntityDummy evadeEntityDummy = Singleton<EventManager>.Instance.CreateActor<EvadeEntityDummy>(baseMonoDynamicObject);
			evadeEntityDummy.Setup(ownerID);
			return evadeEntityDummy.runtimeID;
		}

		public uint CreateBarrierField(uint ownerID, string type, Vector3 initPos, Vector3 initDir, float length)
		{
			Vector3 vector = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition - initPos;
			initDir = ((!(Vector3.Angle(vector, initDir) > 90f) && !(Vector3.Angle(vector, initDir) < -90f)) ? initDir : (-initDir));
			MonoWall monoWall = CreateDynamicObjectEntityInstance<MonoWall>(ownerID, "Barrier", initPos, initDir, GetNextNonSyncedDynamicObjectRuntimeID());
			monoWall.dynamicType = BaseMonoDynamicObject.DynamicType.Barrier;
			monoWall.SetCollisionMask(1 << InLevelData.AVATAR_LAYER);
			Vector3 localScale = monoWall.transform.localScale;
			localScale.x = length;
			monoWall.transform.localScale = localScale;
			return monoWall.GetRuntimeID();
		}

		public uint CreateNavigationArrow(uint ownerID, Vector3 pos, Vector3 forward)
		{
			BaseMonoDynamicObject baseMonoDynamicObject = CreateDynamicObjectEntityInstance(ownerID, "NavigationArrow", pos, forward, GetNextNonSyncedDynamicObjectRuntimeID());
			baseMonoDynamicObject.dynamicType = BaseMonoDynamicObject.DynamicType.NavigationArrow;
			return baseMonoDynamicObject.GetRuntimeID();
		}

		public void SetParticleColorByRarity(GameObject obj, int rarity)
		{
			string hexString = MiscData.Config.ItemRarityColorList[Mathf.Clamp(rarity, 0, MiscData.Config.ItemRarityColorList.Count - 1)];
			Color startColor = Miscs.ParseColor(hexString);
			ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				Renderer component = componentsInChildren[i].GetComponent<Renderer>();
				if (component == null || component.material.shader.name.IndexOf("Channel Mix") == -1)
				{
					componentsInChildren[i].startColor = startColor;
				}
			}
		}

		public uint CreateEquipItem(uint ownerID, int metaId, Vector3 initPos, Vector3 initDir, bool actDropAnim, int level = 1)
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId);
			int rarity = dummyStorageDataItem.rarity;
			string type = ((rarity > 2) ? "EquipItem_02" : "EquipItem_01");
			BaseMonoDynamicObject baseMonoDynamicObject = CreateDynamicObjectEntityInstance(ownerID, type, initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			MonoGoods monoGoods = baseMonoDynamicObject as MonoGoods;
			monoGoods.actDropAnim = actDropAnim;
			monoGoods.DropItemMetaID = metaId;
			monoGoods.DropItemLevel = level;
			monoGoods.DropItemNum = 1;
			if (!string.IsNullOrEmpty(monoGoods.InsideEffectPattern))
			{
				List<MonoEffect> list = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(monoGoods.InsideEffectPattern, baseMonoDynamicObject);
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					SetParticleColorByRarity(list[i].gameObject, rarity);
				}
			}
			EquipItemActor equipItemActor = Singleton<EventManager>.Instance.CreateActor<EquipItemActor>(baseMonoDynamicObject);
			equipItemActor.rarity = rarity;
			return equipItemActor.runtimeID;
		}

		public uint CreateStigmataItem(uint ownerID, int metaId, Vector3 initPos, Vector3 initDir, bool actDropAnim, int level = 1)
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId);
			int rarity = dummyStorageDataItem.rarity;
			string type = ((rarity > 2) ? "StigmataItem_02" : "StigmataItem_01");
			BaseMonoDynamicObject baseMonoDynamicObject = CreateDynamicObjectEntityInstance(ownerID, type, initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			MonoGoods monoGoods = baseMonoDynamicObject as MonoGoods;
			monoGoods.actDropAnim = actDropAnim;
			monoGoods.DropItemMetaID = metaId;
			monoGoods.DropItemLevel = level;
			monoGoods.DropItemNum = 1;
			EquipItemActor equipItemActor = Singleton<EventManager>.Instance.CreateActor<EquipItemActor>(baseMonoDynamicObject);
			equipItemActor.rarity = rarity;
			return equipItemActor.runtimeID;
		}

		public uint CreateMaterialItem(uint ownerID, int metaId, Vector3 initPos, Vector3 initDir, bool actDropAnim, int level = 1)
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId);
			int rarity = dummyStorageDataItem.rarity;
			string type = ((rarity > 2) ? "MaterialItem_02" : "MaterialItem_01");
			BaseMonoDynamicObject baseMonoDynamicObject = CreateDynamicObjectEntityInstance(ownerID, type, initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			MonoGoods monoGoods = baseMonoDynamicObject as MonoGoods;
			monoGoods.actDropAnim = actDropAnim;
			monoGoods.DropItemMetaID = metaId;
			monoGoods.DropItemLevel = level;
			monoGoods.DropItemNum = 1;
			if (!string.IsNullOrEmpty(monoGoods.InsideEffectPattern))
			{
				List<MonoEffect> list = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(monoGoods.InsideEffectPattern, baseMonoDynamicObject);
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					SetParticleColorByRarity(list[i].gameObject, rarity);
				}
			}
			EquipItemActor equipItemActor = Singleton<EventManager>.Instance.CreateActor<EquipItemActor>(baseMonoDynamicObject);
			equipItemActor.rarity = rarity;
			return equipItemActor.runtimeID;
		}

		public uint CreateAvatarFragmentItem(uint ownerID, int metaId, Vector3 initPos, Vector3 initDir, bool actDropAnim, int level = 1)
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId);
			int rarity = dummyStorageDataItem.rarity;
			BaseMonoDynamicObject baseMonoDynamicObject = CreateDynamicObjectEntityInstance(ownerID, "AvatarFragmentItem", initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			MonoGoods monoGoods = baseMonoDynamicObject as MonoGoods;
			monoGoods.actDropAnim = actDropAnim;
			monoGoods.DropItemMetaID = metaId;
			monoGoods.DropItemLevel = level;
			monoGoods.DropItemNum = 1;
			EquipItemActor equipItemActor = Singleton<EventManager>.Instance.CreateActor<EquipItemActor>(baseMonoDynamicObject);
			equipItemActor.rarity = rarity;
			return equipItemActor.runtimeID;
		}

		public uint RegisterStageEnvTriggerField(uint ownerID, GameObject go)
		{
			MonoTriggerField monoTriggerField = go.GetComponent<MonoTriggerField>();
			if (monoTriggerField == null)
			{
				monoTriggerField = go.AddComponent<MonoTriggerField>();
				BaseMonoDynamicObject entity = RegisterAsDynamicObject(ownerID, go);
				Singleton<EventManager>.Instance.CreateActor<TriggerFieldActor>(entity);
			}
			monoTriggerField.SetCollisionMask(1 << InLevelData.AVATAR_LAYER);
			Collider component = monoTriggerField.GetComponent<Collider>();
			component.enabled = false;
			component.enabled = true;
			return monoTriggerField.GetRuntimeID();
		}

		public AbilityTriggerField CreateAbilityTriggerField(Vector3 initPos, Vector3 initDir, BaseAbilityActor owner, float uniformScale, MixinTargetting targetting, uint runtimeID, bool followOwner = false)
		{
			MonoTriggerField entity = CreateDynamicObjectEntityInstance<MonoTriggerField>(owner.runtimeID, "UnitField", initPos, initDir, runtimeID);
			AbilityTriggerField abilityTriggerField = Singleton<EventManager>.Instance.CreateActor<AbilityTriggerField>(entity);
			abilityTriggerField.Setup(owner, uniformScale, targetting, followOwner);
			return abilityTriggerField;
		}

		public AbilityTriggerBullet CreateAbilityLinearTriggerBullet(string bulletType, BaseAbilityActor owner, float speed, MixinTargetting targetting, bool ignoreTimeScale, uint runtimeID, float aliveDuration = -1f)
		{
			MonoTriggerBullet entity = CreateDynamicObjectEntityInstance<MonoTriggerBullet>(owner.runtimeID, bulletType, runtimeID);
			AbilityTriggerBullet abilityTriggerBullet = Singleton<EventManager>.Instance.CreateActor<AbilityTriggerBullet>(entity);
			abilityTriggerBullet.Setup(owner, speed, targetting, ignoreTimeScale, aliveDuration);
			return abilityTriggerBullet;
		}

		public uint CreateStoryScreen(uint ownerID, string type, Vector3 pos, Vector3 dir, int plotID)
		{
			MonoStoryScreen monoStoryScreen = CreateDynamicObjectEntityInstance<MonoStoryScreen>(ownerID, type, pos, dir, GetNextNonSyncedDynamicObjectRuntimeID());
			monoStoryScreen.SetupView(plotID);
			return monoStoryScreen.GetRuntimeID();
		}

		public uint CreateHPMedic(uint ownerID, Vector3 initPos, Vector3 initDir, float healHP, bool actDropAnim)
		{
			MonoGoods monoGoods = CreateDynamicObjectEntityInstance<MonoGoods>(ownerID, "HPMedic", initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			monoGoods.actDropAnim = actDropAnim;
			TriggerGoodsAttachEffectPattern(monoGoods);
			HPMedicActor hPMedicActor = Singleton<EventManager>.Instance.CreateActor<HPMedicActor>(monoGoods);
			hPMedicActor.healHP = healHP;
			return hPMedicActor.runtimeID;
		}

		public uint CreateSPMedic(uint ownerID, Vector3 initPos, Vector3 initDir, float healSP, bool actDropAnim)
		{
			MonoGoods monoGoods = CreateDynamicObjectEntityInstance<MonoGoods>(ownerID, "SPMedic", initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			monoGoods.actDropAnim = actDropAnim;
			if (!string.IsNullOrEmpty(monoGoods.AttachEffectPattern))
			{
				Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(monoGoods.AttachEffectPattern, monoGoods);
			}
			SPMedicActor sPMedicActor = Singleton<EventManager>.Instance.CreateActor<SPMedicActor>(monoGoods);
			sPMedicActor.healSP = healSP;
			return sPMedicActor.runtimeID;
		}

		public uint CreateCoin(uint ownerID, Vector3 initPos, Vector3 initDir, float scoinReward, bool actDropAnim)
		{
			MonoGoods monoGoods = CreateDynamicObjectEntityInstance<MonoGoods>(ownerID, "Coin", initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			monoGoods.actDropAnim = actDropAnim;
			TriggerGoodsAttachEffectPattern(monoGoods);
			CoinActor coinActor = Singleton<EventManager>.Instance.CreateActor<CoinActor>(monoGoods);
			coinActor.scoinReward = scoinReward;
			return coinActor.runtimeID;
		}

		private void TriggerGoodsAttachEffectPattern(MonoGoods entity)
		{
			if (string.IsNullOrEmpty(entity.AttachEffectPattern))
			{
				return;
			}
			bool flag = true;
			GraphicsRecommendGrade graphicsRecommendGrade = GraphicsSettingData.GetGraphicsRecommendGrade();
			if (graphicsRecommendGrade == GraphicsRecommendGrade.Off || graphicsRecommendGrade == GraphicsRecommendGrade.Low)
			{
				ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
				if (!personalGraphicsSetting.IsUserDefinedGrade)
				{
					flag = false;
				}
				else if (personalGraphicsSetting.RecommendGrade != GraphicsRecommendGrade.High)
				{
					flag = false;
				}
			}
			if (flag)
			{
				Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(entity.AttachEffectPattern, entity);
			}
		}

		public uint CreateGood(uint ownerID, string goodType, string abilityName, float argument, Vector3 initPos, Vector3 initDir, bool actDropAnimation, bool forceFlyToAvatar = false)
		{
			MonoGoods monoGoods = CreateDynamicObjectEntityInstance<MonoGoods>(ownerID, goodType, initPos, initDir, GetNextSyncedDynamicObjectRuntimeID());
			monoGoods.actDropAnim = actDropAnimation;
			monoGoods.forceFlyToAvatar = forceFlyToAvatar;
			TriggerGoodsAttachEffectPattern(monoGoods);
			AbilityGoodActor abilityGoodActor = Singleton<EventManager>.Instance.CreateActor<AbilityGoodActor>(monoGoods);
			abilityGoodActor.abilityName = abilityName;
			abilityGoodActor.abilityArgument = argument;
			return 0u;
		}

		public uint GetNextSyncedDynamicObjectRuntimeID()
		{
			return Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(6);
		}

		public uint GetNextNonSyncedDynamicObjectRuntimeID()
		{
			return Singleton<RuntimeIDManager>.Instance.GetNextNonSyncedRuntimeID(6);
		}
	}
}
