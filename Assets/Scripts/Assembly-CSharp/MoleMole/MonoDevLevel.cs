using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using MoleMole.Config;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class MonoDevLevel : MonoTheLevelV1
	{
		private const float KeyRotationDelta = 300f;

		[HideInInspector]
		public List<int> monsterInstanceIds;

		[HideInInspector]
		public List<DevMonsterData> monsterDevDatas;

		[HideInInspector]
		public List<string> avatarTypeNames;

		[HideInInspector]
		public List<DevAvatarData> avatarDevDatas;

		[HideInInspector]
		public DevStageData stageDevData;

		[HideInInspector]
		public int avatarCount;

		[HideInInspector]
		public string LEVEL_PATH;

		[HideInInspector]
		public LevelActor.Mode LEVEL_MODE;

		[HideInInspector]
		public int LEVEL_HARDLEVEL;

		[HideInInspector]
		public int LEVEL_DIFFICULTY;

		[HideInInspector]
		public int TEAM_LEVEL = 1;

		[HideInInspector]
		public LevelManager _levelManager;

		[HideInInspector]
		public bool pariticleMode;

		[HideInInspector]
		public bool useFakeHelper;

		[HideInInspector]
		public bool isBenchmark;

		private bool isRotating;

		public new void Awake()
		{
			AwakeTryLoadFromFile();
			GlobalVars.DISABLE_NETWORK_DEBUG = true;
			QualitySettings.vSyncCount = 1;
			Singleton<LevelManager>.Create();
			Singleton<EffectManager>.Create();
			Singleton<RuntimeIDManager>.Instance.InitAtAwake();
			Singleton<StageManager>.Instance.InitAtAwake();
			Singleton<AvatarManager>.Instance.InitAtAwake();
			Singleton<CameraManager>.Instance.InitAtAwake();
			Singleton<MonsterManager>.Instance.InitAtAwake();
			Singleton<PropObjectManager>.Instance.InitAtAwake();
			Singleton<DynamicObjectManager>.Instance.InitAtAwake();
			Singleton<EffectManager>.Instance.InitAtAwake();
			Singleton<EventManager>.Instance.InitAtAwake();
			HandleBeforeLevelDesignAwake();
			Singleton<LevelDesignManager>.Instance.InitAtAwake();
			Singleton<AuxObjectManager>.Instance.InitAtAwake();
			Singleton<DetourManager>.Instance.InitAtAwake();
			Singleton<ShaderDataManager>.Instance.InitAtAwake();
			Singleton<CinemaDataManager>.Instance.InitAtAwake();
			_levelManager = Singleton<LevelManager>.Instance;
			_levelManager.CreateBehaviorManager();
			MonoLevelEntity monoLevelEntity = base.gameObject.AddComponent<MonoLevelEntity>();
			_levelManager.levelEntity = monoLevelEntity;
			monoLevelEntity.Init(562036737u);
			LevelActor levelActor = Singleton<EventManager>.Instance.CreateActor<LevelActor>(monoLevelEntity);
			_levelManager.levelActor = levelActor;
			levelActor.PostInit();
			levelActor.AddPlugin(new DevLevelActorPlugin(this));
			levelActor.RemovePlugin<LevelMissionStatisticsPlugin>();
			PostAwakeTryLoadFromFile();
		}

		public new void Start()
		{
			UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>("UI/UIToolkits/FPSIndicator"));
			PreStartHandleBenchmark();
			Singleton<StageManager>.Instance.InitAtStart();
			Singleton<AvatarManager>.Instance.InitAtStart();
			HandleAlreadyLoadedCameras();
			Singleton<MonsterManager>.Instance.InitAtStart();
			Singleton<PropObjectManager>.Instance.InitAtStart();
			Singleton<DynamicObjectManager>.Instance.InitAtStart();
			Singleton<EffectManager>.Instance.InitAtStart();
			Singleton<EventManager>.Instance.InitAtStart();
			Singleton<AuxObjectManager>.Instance.InitAtStart();
			Singleton<DetourManager>.Instance.InitAtStart();
			HandleAlreadyLoadedPrefabs();
			BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
			if (baseMonoAvatar != null)
			{
				Singleton<WwiseAudioManager>.Instance.SetListenerFollowing(baseMonoAvatar.transform, new Vector3(0f, 2f, 0f));
			}
			Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(new string[4] { "All_In_One_Bank", "BK_Global", "BK_Events", "Test_3D_Att" });
			PostFXBase postFXBase = UnityEngine.Object.FindObjectOfType<PostFXBase>();
			if (postFXBase != null)
			{
				postFXBase.originalEnabled = true;
			}
			PostStartHandleBenchmark();
		}

		private void AwakeTryLoadFromFile()
		{
			if (DevLevelConfigData.configFromScene)
			{
				LEVEL_PATH = DevLevelConfigData.LEVEL_PATH;
				LEVEL_MODE = DevLevelConfigData.LEVEL_MODE;
				stageDevData = DevLevelConfigData.stageDevData;
				avatarDevDatas = DevLevelConfigData.avatarDevDatas;
				monsterDevDatas = DevLevelConfigData.monsterDevDatas;
				isBenchmark = DevLevelConfigData.isBenchmark;
				GlobalVars.IS_BENCHMARK = isBenchmark;
				avatarCount = DevLevelConfigData.avatarDevDatas.Count;
				if (isBenchmark)
				{
					pariticleMode = true;
				}
				avatarTypeNames = new List<string>();
				{
					foreach (DevAvatarData avatarDevData in avatarDevDatas)
					{
						avatarTypeNames.Add(avatarDevData.avatarType);
					}
					return;
				}
			}
			MainUIData.USE_VIEW_CACHING = false;
			GeneralLogicManager.InitAll();
			GlobalDataManager.Refresh();
		}

		private void PostAwakeTryLoadFromFile()
		{
			if (!DevLevelConfigData.configFromScene)
			{
				return;
			}
			TryDestroyTypeAll<BaseMonoAvatar>();
			TryDestroyTypeAll<BaseMonoMonster>();
			TryDestroyTypeAll<MonoBasePerpStage>();
			TryDestroyTypeAll<MonoStageEnv>();
			Resources.UnloadUnusedAssets();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			StageEntry stageEntryByName = StageData.GetStageEntryByName(stageDevData.stageName);
			GameObject gameObject = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(stageEntryByName.GetPerpStagePrefabPath()));
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.position -= gameObject.transform.Find(stageEntryByName.LocationPointName).position;
			StageManager.SetPerpstageNodeVisibility(gameObject.GetComponent<MonoBasePerpStage>(), stageEntryByName, false, false);
			StageManager.SetPerpstageNodeVisibility(gameObject.GetComponent<MonoBasePerpStage>(), stageEntryByName, true, true);
			UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(stageEntryByName.GetEnvPrefabPath()));
			DevAvatarData devAvatarData = avatarDevDatas[0];
			string prefabResPath = AvatarData.GetPrefabResPath(devAvatarData.avatarType);
			GameObject original = Miscs.LoadResource<GameObject>(prefabResPath);
			UnityEngine.Object.Instantiate(original);
			monsterInstanceIds = new List<int>();
			foreach (DevMonsterData monsterDevData in monsterDevDatas)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(MonsterData.GetPrefabResPath(monsterDevData.monsterName, monsterDevData.typeName)));
				monsterInstanceIds.Add(gameObject2.GetInstanceID());
			}
		}

		private void PreStartHandleBenchmark()
		{
			if (!isBenchmark)
			{
				return;
			}
			foreach (DevAvatarData avatarDevData in avatarDevDatas)
			{
				if (!Miscs.ArrayContains(avatarDevData.avatarTestSkills, "Test_UnlockAllAniSkill"))
				{
					Miscs.ArrayAppend(ref avatarDevData.avatarTestSkills, "Test_UnlockAllAniSkill");
				}
				if (!Miscs.ArrayContains(avatarDevData.avatarTestSkills, "Test_Undamagable"))
				{
					Miscs.ArrayAppend(ref avatarDevData.avatarTestSkills, "Test_Undamagable");
				}
			}
			foreach (DevMonsterData monsterDevData in monsterDevDatas)
			{
				if (!Miscs.ArrayContains(monsterDevData.abilities, "Test_Undamagable"))
				{
					Miscs.ArrayAppend(ref monsterDevData.abilities, "Test_Undamagable");
				}
			}
		}

		private void PostStartHandleBenchmark()
		{
			if (!isBenchmark)
			{
				return;
			}
			Singleton<AvatarManager>.Instance.SetAutoBattle(true);
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			for (int i = 0; i < allPlayerAvatars.Count; i++)
			{
				if (!string.IsNullOrEmpty(avatarDevDatas[i].avatarAI))
				{
					ExternalBehaviorTree externalBehaviorTree = Miscs.LoadResource<ExternalBehaviorTree>(avatarDevDatas[i].avatarAI);
					((BTreeAvatarAIController)allPlayerAvatars[i].GetActiveAIController()).autoBattleBehavior = externalBehaviorTree;
					((BTreeAvatarAIController)allPlayerAvatars[i].GetActiveAIController()).autoMoveBehvior = externalBehaviorTree;
					((BTreeAvatarAIController)allPlayerAvatars[i].GetActiveAIController()).supporterBehavior = externalBehaviorTree;
					allPlayerAvatars[i].GetComponent<BehaviorTree>().ExternalBehavior = externalBehaviorTree;
					allPlayerAvatars[i].GetComponent<BehaviorTree>().EnableBehavior();
				}
			}
			Screen.sleepTimeout = -1;
			SuperDebug.CloseAllDebugs();
			GameObject gameObject = new GameObject();
			gameObject.name = "__Benchmark";
			gameObject.AddComponent<MonoBenchmarkSwitches>();
		}

		private void HandleBeforeLevelDesignAwake()
		{
			Singleton<LevelScoreManager>.Create();
			FriendDetailDataItem friend = null;
			if (true)
			{
				DevAvatarData devAvatarData = avatarDevDatas[0];
				AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(avatarTypeNames[0], devAvatarData.avatarLevel, devAvatarData.avatarStar);
				SetUpAvatarDataItem(dummyAvatarDataItem, devAvatarData);
				friend = new FriendDetailDataItem(0, "FakeHelper", 1, dummyAvatarDataItem);
			}
			Singleton<LevelScoreManager>.Instance.SetDevLevelBeginIntent(LEVEL_PATH, LEVEL_MODE, LEVEL_HARDLEVEL, LEVEL_DIFFICULTY + 1, friend);
			Singleton<PlayerModule>.Instance.playerData.teamLevel = TEAM_LEVEL;
		}

		private void HandleAlreadyLoadedCameras()
		{
			RuntimeIDManager instance = Singleton<RuntimeIDManager>.Instance;
			MonoMainCamera monoMainCamera = UnityEngine.Object.FindObjectOfType<MonoMainCamera>();
			uint nextRuntimeID = instance.GetNextRuntimeID(2);
			monoMainCamera.Init(nextRuntimeID);
			Singleton<CameraManager>.Instance.RegisterCameraData(1u, monoMainCamera, nextRuntimeID);
			MonoInLevelUICamera monoInLevelUICamera = UnityEngine.Object.FindObjectOfType<MonoInLevelUICamera>();
			nextRuntimeID = instance.GetNextRuntimeID(2);
			monoInLevelUICamera.Init(nextRuntimeID);
			Singleton<CameraManager>.Instance.RegisterCameraData(2u, monoInLevelUICamera, nextRuntimeID);
		}

		private void HandleAlreadyLoadedPrefabs()
		{
			RuntimeIDManager instance = Singleton<RuntimeIDManager>.Instance;
			StageEntry stageEntryByName = StageData.GetStageEntryByName(stageDevData.stageName);
			MonoBasePerpStage monoBasePerpStage = UnityEngine.Object.FindObjectOfType<MonoBasePerpStage>();
			if (monoBasePerpStage != null)
			{
				monoBasePerpStage.Init(stageEntryByName, (string)null);
			}
			MonoStageEnv stageEnv = UnityEngine.Object.FindObjectOfType<MonoStageEnv>();
			Singleton<StageManager>.Instance.RegisterStage(stageEntryByName, monoBasePerpStage, stageEnv);
			LevelActor levelActor = (LevelActor)Singleton<EventManager>.Instance.GetActor(562036737u);
			levelActor.levelMode = LEVEL_MODE;
			BaseMonoAvatar baseMonoAvatar = UnityEngine.Object.FindObjectOfType<BaseMonoAvatar>();
			if (baseMonoAvatar != null)
			{
				DevAvatarData devAvatarData = avatarDevDatas[0];
				AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(avatarTypeNames[0], devAvatarData.avatarLevel, devAvatarData.avatarStar);
				SetUpAvatarDataItem(dummyAvatarDataItem, devAvatarData);
				uint nextRuntimeID = instance.GetNextRuntimeID(3);
				baseMonoAvatar.Init(true, nextRuntimeID, dummyAvatarDataItem.AvatarRegistryKey, dummyAvatarDataItem.GetWeapon().ID, baseMonoAvatar.transform.position, baseMonoAvatar.transform.forward, true);
				LoadAvatarWwiseSoundBank(baseMonoAvatar);
				AvatarActor avatarActor = Singleton<EventManager>.Instance.CreateActor<AvatarActor>(baseMonoAvatar);
				avatarActor.InitAvatarDataItem(dummyAvatarDataItem, true, false, true, true);
				avatarActor.PostInit();
				Singleton<AvatarManager>.Instance.RegisterAvatar(baseMonoAvatar, true, true, false);
				SetUpAvatarSkill(avatarActor, devAvatarData);
				CheatForPariticleMode(avatarActor);
				for (int i = 1; i < avatarCount; i++)
				{
					DevAvatarData devAvatarData2 = avatarDevDatas[i];
					AvatarDataItem dummyAvatarDataItem2 = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(avatarTypeNames[i], devAvatarData2.avatarLevel, devAvatarData2.avatarStar);
					SetUpAvatarDataItem(dummyAvatarDataItem2, devAvatarData2);
					uint nextRuntimeID2 = instance.GetNextRuntimeID(3);
					Singleton<AvatarManager>.Instance.CreateAvatar(dummyAvatarDataItem2, false, baseMonoAvatar.XZPosition, baseMonoAvatar.FaceDirection, nextRuntimeID2, false, false);
					AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(nextRuntimeID2);
					SetUpAvatarSkill(actor, devAvatarData2);
					CheatForPariticleMode(actor);
				}
				if (useFakeHelper)
				{
					CreateFakeFriendAvatar();
				}
				Singleton<CinemaDataManager>.Instance.Preload(baseMonoAvatar);
			}
			BaseMonoMonster[] array = UnityEngine.Object.FindObjectsOfType<BaseMonoMonster>();
			foreach (BaseMonoMonster baseMonoMonster in array)
			{
				DevMonsterData devMonsterData = null;
				int instanceID = baseMonoMonster.gameObject.GetInstanceID();
				for (int k = 0; k < monsterInstanceIds.Count; k++)
				{
					if (monsterInstanceIds[k] == instanceID)
					{
						devMonsterData = monsterDevDatas[k];
						break;
					}
				}
				if (devMonsterData == null)
				{
					UnityEngine.Object.Destroy(baseMonoMonster.gameObject);
					continue;
				}
				uint nextRuntimeID3 = instance.GetNextRuntimeID(4);
				string monsterName;
				string typeName;
				if (devMonsterData.uniqueMonsterID != 0)
				{
					UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(devMonsterData.uniqueMonsterID);
					monsterName = uniqueMonsterMetaData.monsterName;
					typeName = uniqueMonsterMetaData.typeName;
				}
				else
				{
					monsterName = devMonsterData.monsterName;
					typeName = devMonsterData.typeName;
				}
				baseMonoMonster.Init(isElite: devMonsterData.isElite, monsterName: monsterName, typeName: typeName, runtimeID: nextRuntimeID3, initPos: baseMonoMonster.transform.position, uniqueMonsterID: devMonsterData.uniqueMonsterID, overrideAIName: null, checkOutsideWall: true, disableBehaviorWhenInit: true);
				for (int l = 0; l < baseMonoMonster.config.CommonArguments.RequestSoundBankNames.Length; l++)
				{
					Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(baseMonoMonster.config.CommonArguments.RequestSoundBankNames[l]);
				}
				MonsterActor monsterActor = Singleton<EventManager>.Instance.CreateActor<MonsterActor>(baseMonoMonster);
				monsterActor.InitLevelData(devMonsterData.level, devMonsterData.isElite);
				monsterActor.PostInit();
				Singleton<MonsterManager>.Instance.RegisterMonster(baseMonoMonster);
				if (devMonsterData.abilities.Length > 0)
				{
					for (int m = 0; m < devMonsterData.abilities.Length; m++)
					{
						string value = devMonsterData.abilities[m];
						if (!string.IsNullOrEmpty(value))
						{
							monsterActor.abilityPlugin.InsertPreInitAbility(AbilityData.GetAbilityConfig(devMonsterData.abilities[m]));
						}
					}
				}
				if (devMonsterData.isStationary)
				{
					MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(baseMonoMonster.GetRuntimeID());
					actor2.baseMaxHP = (actor2.HP = (actor2.maxHP = 999999f));
					baseMonoMonster.GetActiveAIController().SetActive(false);
				}
			}
			Singleton<LevelManager>.Instance.levelActor.SuddenLevelStart();
			Singleton<EventManager>.Instance.FireEvent(new EvtStageReady
			{
				isBorn = true
			});
			Singleton<DetourManager>.Instance.LoadNavMeshRelatedLevel(stageDevData.stageName);
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel();
		}

		public void ReplaceInstanceID(int oldID, int newID)
		{
			if (!monsterInstanceIds.Contains(oldID))
			{
				return;
			}
			for (int i = 0; i < monsterInstanceIds.Count; i++)
			{
				if (monsterInstanceIds[i] == oldID)
				{
					monsterInstanceIds[i] = newID;
				}
			}
		}

		public new void Update()
		{
			Singleton<StageManager>.Instance.Core();
			Singleton<AvatarManager>.Instance.Core();
			Singleton<CameraManager>.Instance.Core();
			Singleton<MonsterManager>.Instance.Core();
			Singleton<PropObjectManager>.Instance.Core();
			Singleton<DynamicObjectManager>.Instance.Core();
			Singleton<EffectManager>.Instance.Core();
			Singleton<EventManager>.Instance.Core();
			Singleton<LevelDesignManager>.Instance.Core();
			Singleton<AuxObjectManager>.Instance.Core();
			Singleton<DetourManager>.Instance.Core();
			if (Singleton<WwiseAudioManager>.Instance != null)
			{
				Singleton<WwiseAudioManager>.Instance.Core();
			}
		}

		public new void OnDestroy()
		{
			Singleton<LevelManager>.Instance.Destroy();
			Singleton<LevelManager>.Destroy();
		}

		private void UpdateForKeyboradInput()
		{
			Vector2 zero = Vector2.zero;
			if (Input.GetKeyUp(KeyCode.Semicolon))
			{
				Singleton<CameraManager>.Instance.GetMainCamera().SetRotateToFaceDirection();
			}
			if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				Singleton<AvatarManager>.Instance.GetLocalAvatar().SetTrigger("TriggerHit");
			}
			if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				localAvatar.SetTrigger("TriggerHit");
				localAvatar.SetTrigger("TriggerKnockDown");
			}
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				Singleton<AvatarManager>.Instance.GetLocalAvatar().SetDied(KillEffect.KillNow);
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
				if (allMonsters.Count > 0)
				{
					Singleton<CameraManager>.Instance.EnableBossCamera(allMonsters[0].GetRuntimeID());
				}
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Singleton<CameraManager>.Instance.DisableBossCamera();
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				Singleton<CameraManager>.Instance.controlledRotateKeepManual = !Singleton<CameraManager>.Instance.controlledRotateKeepManual;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				Singleton<CameraManager>.Instance.EnableCrowdCamera();
			}
			if (Input.GetKey(KeyCode.Keypad4))
			{
				if (!isRotating)
				{
					isRotating = true;
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
				}
				zero += new Vector2(-300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime, 0f);
			}
			if (Input.GetKey(KeyCode.Keypad6))
			{
				if (!isRotating)
				{
					isRotating = true;
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
				}
				zero += new Vector2(300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime, 0f);
			}
			if (Input.GetKey(KeyCode.Keypad8))
			{
				if (!isRotating)
				{
					isRotating = true;
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
				}
				zero += new Vector2(0f, 300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.Keypad2))
			{
				if (!isRotating)
				{
					isRotating = true;
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
				}
				zero += new Vector2(0f, -300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.Keypad7))
			{
				if (!isRotating)
				{
					isRotating = true;
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
				}
				zero += new Vector2(-300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime, 300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.Keypad9))
			{
				if (!isRotating)
				{
					isRotating = true;
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
				}
				zero += new Vector2(300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime, 300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.Keypad1))
			{
				if (!isRotating)
				{
					isRotating = true;
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
				}
				zero += new Vector2(-300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime, -300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.Keypad3))
			{
				if (!isRotating)
				{
					isRotating = true;
					Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
				}
				zero += new Vector2(300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime, -300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime);
			}
			if (Input.GetKeyUp(KeyCode.Keypad4) || Input.GetKeyUp(KeyCode.Keypad6) || Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Keypad5) || Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Keypad9) || Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Keypad3))
			{
				isRotating = false;
				Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStop();
			}
			if (isRotating)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().SetFollowControledRotationData(zero);
			}
			if (Input.GetKey(KeyCode.KeypadPlus) && !Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().PlayStoryCameraState(20001, true, false);
			}
			if (Input.GetKey(KeyCode.KeypadMinus) && Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().storyState.QuitStoryStateWithFade(0.5f, true, true);
			}
			if (Input.GetKey(KeyCode.KeypadMultiply) && Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().storyState.QuitStoryStateWithLerp();
			}
			if (Input.GetKey(KeyCode.X))
			{
				Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel(3f);
			}
			if (Input.GetKey(KeyCode.C))
			{
				Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(3f);
			}
		}

		private void SetUpAvatarDataItem(AvatarDataItem avatarDataItem, DevAvatarData devAvatarData)
		{
			avatarDataItem.equipsMap[(EquipmentSlot)1] = Singleton<StorageModule>.Instance.GetDummyWeaponDataItem(devAvatarData.avatarWeapon, devAvatarData.avatarWeaponLevel);
			if (devAvatarData.avatarStigmata[0] != -1)
			{
				avatarDataItem.equipsMap[(EquipmentSlot)2] = Singleton<StorageModule>.Instance.GetDummyStigmataDataItem(devAvatarData.avatarStigmata[0], devAvatarData.avatarStigmataLevels[0]);
			}
			else
			{
				avatarDataItem.equipsMap[(EquipmentSlot)2] = null;
			}
			if (devAvatarData.avatarStigmata[1] != -1)
			{
				avatarDataItem.equipsMap[(EquipmentSlot)3] = Singleton<StorageModule>.Instance.GetDummyStigmataDataItem(devAvatarData.avatarStigmata[1], devAvatarData.avatarStigmataLevels[1]);
			}
			else
			{
				avatarDataItem.equipsMap[(EquipmentSlot)3] = null;
			}
			if (devAvatarData.avatarStigmata[2] != -1)
			{
				avatarDataItem.equipsMap[(EquipmentSlot)4] = Singleton<StorageModule>.Instance.GetDummyStigmataDataItem(devAvatarData.avatarStigmata[2], devAvatarData.avatarStigmataLevels[2]);
			}
			else
			{
				avatarDataItem.equipsMap[(EquipmentSlot)4] = null;
			}
		}

		private void SetUpAvatarSkill(AvatarActor actor, DevAvatarData devAvatarData)
		{
			if (devAvatarData.avatarTestSkills.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < devAvatarData.avatarTestSkills.Length; i++)
			{
				string value = devAvatarData.avatarTestSkills[i];
				if (!string.IsNullOrEmpty(value))
				{
					actor.abilityPlugin.InsertPreInitAbility(AbilityData.GetAbilityConfig(devAvatarData.avatarTestSkills[i]));
				}
			}
		}

		private void CheatForPariticleMode(AvatarActor actor)
		{
			if (!pariticleMode)
			{
				return;
			}
			foreach (AvatarActor.SKillInfo skillInfo in actor.skillInfoList)
			{
				skillInfo.CD = 0f;
			}
			actor.baseMaxHP = (actor.HP = (actor.maxHP = 999999f));
			actor.baseMaxSP = (actor.SP = (actor.maxSP = 999999f));
			actor.ChangeSwitchInCDTime(0.1f);
		}

		private void CreateFakeFriendAvatar()
		{
			bool leaderSkillOn = false;
			FriendDetailDataItem friendDetailItem = Singleton<LevelScoreManager>.Instance.friendDetailItem;
			AvatarDataItem leaderAvatar = friendDetailItem.leaderAvatar;
			Singleton<AvatarManager>.Instance.CreateAvatar(leaderAvatar, false, InLevelData.CREATE_INIT_POS, InLevelData.CREATE_INIT_FORWARD, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3), false, leaderSkillOn, true);
		}

		private void TryDestroyType<T>() where T : Component
		{
			T[] array = UnityEngine.Object.FindObjectsOfType<T>();
			T[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				T val = array2[i];
				if (val != null)
				{
					UnityEngine.Object.DestroyImmediate(val.gameObject);
				}
			}
		}

		private void TryDestroyTypeAll<T>() where T : Component
		{
			T[] array = UnityEngine.Object.FindObjectsOfType<T>();
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array[i].gameObject);
			}
		}

		private void LoadAvatarWwiseSoundBank(BaseMonoAvatar avatar)
		{
			int i = 0;
			for (int num = avatar.config.CommonArguments.RequestSoundBankNames.Length; i < num; i++)
			{
				Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(avatar.config.CommonArguments.RequestSoundBankNames[i]);
			}
		}
	}
}
