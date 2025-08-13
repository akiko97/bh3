using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using CinemaDirector;
using LuaInterface;
using MoleMole.Config;
using UniRx;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class LevelDesignManager
	{
		private class LDEvtTrigger
		{
			public BaseLDEvent ldEvent;

			public LuaFunction callback;

			public bool isCallbackCoroutine;

			public LDEvtTrigger(BaseLDEvent ldEvent, LuaFunction callback, bool isCallbackCoroutine)
			{
				this.ldEvent = ldEvent;
				this.callback = callback;
				this.isCallbackCoroutine = isCallbackCoroutine;
			}
		}

		private class EvtTrigger
		{
			public Type evtType;

			public LuaFunction callback;

			public bool isCallbackCoroutine;

			public EvtTrigger(string evtTypeName, LuaFunction callback, bool isCallbackCoroutine)
			{
				evtType = Type.GetType("MoleMole." + evtTypeName);
				this.callback = callback;
				this.isCallbackCoroutine = isCallbackCoroutine;
			}
		}

		public enum LDState
		{
			Start = 0,
			Running = 1,
			End = 2,
			Paused = 3
		}

		private static string OVERRIDE_NAME = "OverrideName";

		private MonoSpawnPoint centerPoint;

		private LuaState _luaState;

		private LuaFunction _ldMain;

		private List<Tuple<LuaThread, int>> _luaManualCoroutines;

		private List<LDEvtTrigger> _ldEvtTriggers;

		private List<EvtTrigger> _evtTriggers;

		private List<BaseLDEvent> _allLDEvents;

		public LDState state { get; private set; }

		public void CreateCoroutine(LuaFunction luaFunc)
		{
			MakeAndStartCoroutine(luaFunc);
		}

		public void ListenLDEvent(LuaTable table, LuaFunction callback)
		{
			BaseLDEvent ldEvent = CreateLDEventFromTable(table);
			_ldEvtTriggers.Add(new LDEvtTrigger(ldEvent, callback, false));
		}

		public void ListenLDEventCoroutine(LuaTable table, LuaFunction callback)
		{
			BaseLDEvent ldEvent = CreateLDEventFromTable(table);
			_ldEvtTriggers.Add(new LDEvtTrigger(ldEvent, callback, true));
		}

		public void ListenEvent(string evtTypeName, LuaFunction callback)
		{
			EvtTrigger item = new EvtTrigger(evtTypeName, callback, false);
			_evtTriggers.Add(item);
		}

		public uint CreateMonster(string monsterName, string typeName, int level, Vector3 spawnPos, bool isElite, LuaTable skillConfigsTable, LuaTable dropTable, float avatarExpReward = 0f, bool checkOutsideWall = true, bool disableBehaviorWhenInit = false, int tagID = 0)
		{
			MonsterManager instance = Singleton<MonsterManager>.Instance;
			bool checkOutsideWall2 = checkOutsideWall;
			uint monsterID = instance.CreateMonster(monsterName, typeName, level, true, spawnPos, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), isElite, 0u, checkOutsideWall2, disableBehaviorWhenInit, tagID);
			return CreateMonsterInternal(monsterID, skillConfigsTable, dropTable, avatarExpReward);
		}

		public string GetDropEquipItemTypeName(int metaId)
		{
			ItemMetaData itemMetaData = ItemMetaDataReader.TryGetItemMetaDataByKey(metaId);
			if (itemMetaData != null)
			{
				return "material";
			}
			WeaponMetaData weaponMetaData = WeaponMetaDataReader.TryGetWeaponMetaDataByKey(metaId);
			if (weaponMetaData != null)
			{
				return "weapon";
			}
			StigmataMetaData stigmataMetaData = StigmataMetaDataReader.TryGetStigmataMetaDataByKey(metaId);
			if (stigmataMetaData != null)
			{
				return "stigmata";
			}
			AvatarCardMetaData avatarCardMetaData = AvatarCardMetaDataReader.TryGetAvatarCardMetaDataByKey(metaId);
			if (avatarCardMetaData != null)
			{
				return "avatar_card";
			}
			AvatarFragmentMetaData avatarFragmentMetaData = AvatarFragmentMetaDataReader.TryGetAvatarFragmentMetaDataByKey(metaId);
			if (avatarFragmentMetaData != null)
			{
				return "avatar_fragment";
			}
			EndlessToolMetaData endlessToolMetaData = EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey(metaId);
			if (endlessToolMetaData != null)
			{
				return "endless_tool";
			}
			return string.Empty;
		}

		public uint CreateUniqueMonster(uint uniqueMonsterID, int level, Vector3 spawnPos, bool isElite, LuaTable skillConfigsTable, LuaTable dropTable, float avatarExpReward = 0f, bool checkOutsideWall = true, bool disableBehaviorWhenInit = false, int tagID = 0)
		{
			UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
			string monsterName = uniqueMonsterMetaData.monsterName;
			string typeName = uniqueMonsterMetaData.typeName;
			uint monsterID = Singleton<MonsterManager>.Instance.CreateMonster(monsterName, typeName, level, true, spawnPos, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), isElite, uniqueMonsterID, checkOutsideWall, disableBehaviorWhenInit, tagID);
			return CreateMonsterInternal(monsterID, skillConfigsTable, dropTable, avatarExpReward);
		}

		private uint CreateMonsterInternal(uint monsterID, LuaTable skillConfigsTable, LuaTable dropTable, float avatarExpReward)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monsterID);
			if (skillConfigsTable != null)
			{
				foreach (DictionaryEntry item in skillConfigsTable)
				{
					string abilityName = (string)item.Key;
					LuaTable luaTable = (LuaTable)item.Value;
					string text = (string)luaTable[OVERRIDE_NAME];
					ConfigAbility abilityConfig = ((text != null) ? AbilityData.GetAbilityConfig(abilityName, text) : AbilityData.GetAbilityConfig(abilityName));
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					foreach (DictionaryEntry item2 in luaTable)
					{
						string text2 = (string)item2.Key;
						if (!(text2 == OVERRIDE_NAME))
						{
							if (item2.Value is double)
							{
								dictionary.Add(text2, (float)(double)item2.Value);
							}
							else if (item2.Value is string)
							{
								dictionary.Add(text2, (string)item2.Value);
							}
						}
					}
					actor.abilityPlugin.AddAbility(abilityConfig, dictionary);
				}
			}
			BaseMonoMonster monster = actor.monster;
			Vector3 forward = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition - monster.XZPosition;
			monster.transform.forward = forward;
			List<LDDropDataItem> list = new List<LDDropDataItem>();
			foreach (object value in dropTable.Values)
			{
				list.Add(value as LDDropDataItem);
			}
			actor.dropDataItems = list;
			actor.avatarExpReward = ((!float.IsNaN(avatarExpReward)) ? Mathf.Max(0f, avatarExpReward) : 0f);
			return monsterID;
		}

		public bool HasAbilityOverrideName(string abilityName, string overrideName)
		{
			bool result = false;
			ConfigOverrideGroup value = null;
			AbilityData.GetAbilityGroupMap().TryGetValue(abilityName, out value);
			if (value != null && value.Overrides != null)
			{
				result = value.Overrides.ContainsKey(overrideName);
			}
			return result;
		}

		public int GetMonsterCount()
		{
			return Singleton<MonsterManager>.Instance.MonsterCount();
		}

		public uint GetNearestMonsterID()
		{
			uint result = 0u;
			float num = float.MaxValue;
			foreach (BaseMonoMonster allMonster in Singleton<MonsterManager>.Instance.GetAllMonsters())
			{
				if (allMonster.IsActive())
				{
					float num2 = Vector3.Distance(allMonster.XZPosition, Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition);
					if (num2 < num)
					{
						result = allMonster.GetRuntimeID();
						num = num2;
					}
				}
			}
			return result;
		}

		public Vector3 GetRandomInSightPoint(LuaTable pointList)
		{
			List<Vector3> list = new List<Vector3>();
			foreach (object value in pointList.Values)
			{
				int namedSpawnPointIx = Singleton<StageManager>.Instance.GetStageEnv().GetNamedSpawnPointIx(value as string);
				Vector3 position = Singleton<StageManager>.Instance.GetStageEnv().spawnPoints[namedSpawnPointIx].transform.position;
				if (IsPointInCameraFov(position))
				{
					list.Add(position);
				}
			}
			if (list.Count > 0)
			{
				return list[UnityEngine.Random.Range(0, list.Count - 1)];
			}
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			Vector3 vector = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition - mainCamera.XZPosition;
			return Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition + vector.normalized;
		}

		public uint CreateStageExitField(string spawnName)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			return Singleton<DynamicObjectManager>.Instance.CreateStageExitField(562036737u, spawnPoint.transform.position, spawnPoint.transform.forward);
		}

		public uint CreateMonsterExitField(string spawnName, bool forDefendMode)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			return Singleton<DynamicObjectManager>.Instance.CreateMonsterExitField(562036737u, spawnPoint.transform.position, spawnPoint.transform.forward, forDefendMode);
		}

		public void CreateStage(string stageTypeName, LuaTable spawnNames, string baseWeatherName, bool continued)
		{
			List<string> list = new List<string>();
			foreach (object value in spawnNames.Values)
			{
				list.Add(value as string);
			}
			Singleton<StageManager>.Instance.CreateStage(stageTypeName, list, baseWeatherName, continued);
		}

		public void SetStageNodeVisible(string nodeNames, bool isVisible)
		{
			StageManager.SetPerpstageNodeVisibilityByNode(Singleton<StageManager>.Instance.GetPerpStage(), nodeNames, isVisible);
		}

		public uint CreateBarrier(string type, float length, string spawnName)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			return Singleton<DynamicObjectManager>.Instance.CreateBarrierField(562036737u, type, spawnPoint.transform.position, spawnPoint.transform.forward, length);
		}

		public uint CreateNavigationArrow(string spawnName_start, string spawnName_end)
		{
			Vector3 position = GetSpawnPoint(spawnName_start).transform.position;
			position.y += 0.1f;
			Vector3 position2 = GetSpawnPoint(spawnName_end).transform.position;
			Vector3 forward = -(position2 - position).normalized;
			return Singleton<DynamicObjectManager>.Instance.CreateNavigationArrow(562036737u, position, forward);
		}

		public uint CreateNavigationArrow(string spawnName)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			Vector3 position = spawnPoint.transform.position;
			position.y += 0.1f;
			Vector3 forward = -spawnPoint.transform.forward;
			return Singleton<DynamicObjectManager>.Instance.CreateNavigationArrow(562036737u, position, forward);
		}

		public uint CreatePropObject(string propName, string spawnName, LuaTable dropTable, float hp, float attack, bool appearAnim = false)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			uint num = Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, propName, hp, attack, spawnPoint.transform.position, spawnPoint.transform.forward, appearAnim);
			InitPropObject(num, dropTable);
			return num;
		}

		private void InitPropObject(uint propID, LuaTable dropTable)
		{
			PropObjectActor actor = Singleton<EventManager>.Instance.GetActor<PropObjectActor>(propID);
			List<LDDropDataItem> list = new List<LDDropDataItem>();
			foreach (object value in dropTable.Values)
			{
				list.Add(value as LDDropDataItem);
			}
			actor.dropDataItems = list;
		}

		public void RemoveBarrier(uint runtimeID)
		{
			BaseMonoDynamicObject dynamicObjectByRuntimeID = Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(runtimeID);
			dynamicObjectByRuntimeID.GetComponentInChildren<FadeAnimation>().StartFadeOut(dynamicObjectByRuntimeID.SetDied);
		}

		public void MoveLocalAvatarTo(string spawnName)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			localAvatar.transform.position = spawnPoint.transform.position;
			localAvatar.transform.forward = spawnPoint.transform.forward;
			Singleton<CameraManager>.Instance.GetMainCamera().SuddenSwitchFollowAvatar(localAvatar.GetRuntimeID());
		}

		public void SetRotateToFaceDirection()
		{
			Singleton<CameraManager>.Instance.GetMainCamera().SetRotateToFaceDirection();
		}

		public void SetLocalAvatarFaceTo(string spawnName)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			Vector3 forward = spawnPoint.transform.position - localAvatar.transform.position;
			localAvatar.SteerFaceDirectionTo(forward);
		}

		public void SetMonsterUnremovable(uint runtimeID, bool unremovable)
		{
			BaseMonoMonster monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(runtimeID);
			monsterByRuntimeID.isStaticInScene = unremovable;
		}

		public void SetEntityIsGhost(uint runtimeID, bool isGhost)
		{
			BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(runtimeID);
			if (entity != null)
			{
				(entity as BaseMonoAbilityEntity).SetCountedIsGhost(isGhost);
			}
		}

		public void SetEntityAllowSelected(uint runtimeID, bool allowSelected)
		{
			BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(runtimeID);
			if (entity != null)
			{
				(entity as BaseMonoAbilityEntity).SetCountedDenySelect(!allowSelected, true);
			}
		}

		public void ShowSubHpBar(uint runtimeID)
		{
			MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor(runtimeID) as MonsterActor;
			if (monsterActor != null)
			{
				monsterActor.showSubHpBarWhenAttackLanded = true;
			}
		}

		public void SetMonsterWarningRange(uint runtimeID, float warningRange, float escapeRange)
		{
			MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor(runtimeID) as MonsterActor;
			monsterActor.EnableWarningFieldActor(warningRange, escapeRange);
		}

		public void SetMonsterTrigger(uint runtimeID, string triggerName)
		{
			BaseMonoMonster baseMonoMonster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
			if (baseMonoMonster != null)
			{
				baseMonoMonster.SetTrigger(triggerName);
			}
		}

		public void SetMonsterAnimatorSpeed(uint runtimeID, float speed)
		{
			BaseMonoMonster baseMonoMonster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
			if (!(baseMonoMonster != null))
			{
				return;
			}
			if (speed != 1f)
			{
				if (baseMonoMonster.timeScaleStack.IsOccupied(7))
				{
					baseMonoMonster.SetTimeScale(speed, 7);
				}
				else
				{
					baseMonoMonster.PushTimeScale(speed, 7);
				}
			}
			else
			{
				baseMonoMonster.PopTimeScale(7);
			}
		}

		public void SetStageAnimatorSpeed(float speed)
		{
			if (Singleton<StageManager>.Instance.GetPerpStage().gameObject.GetComponent<Animator>() != null)
			{
				Singleton<StageManager>.Instance.GetPerpStage().gameObject.GetComponent<Animator>().speed = speed;
			}
		}

		public void SetMonsterSteerTo(uint runtimeID, Vector3 pos)
		{
			BaseMonoMonster baseMonoMonster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
			if (!(baseMonoMonster == null))
			{
				Vector3 dir = pos - baseMonoMonster.transform.position;
				baseMonoMonster.SteerFaceDirectionTo(dir);
			}
		}

		public void SetMonsterFaceTo(uint runtimeID, string spawnName)
		{
			Vector3 vector = Vector3.zero;
			Vector3 one = Vector3.one;
			BaseMonoMonster baseMonoMonster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
			if (baseMonoMonster == null)
			{
				return;
			}
			one = baseMonoMonster.XZPosition;
			if (spawnName == "Avatar")
			{
				BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
				if (baseMonoAvatar != null)
				{
					vector = baseMonoAvatar.XZPosition;
				}
			}
			else
			{
				MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
				vector = spawnPoint.transform.position;
			}
			baseMonoMonster.SteerFaceDirectionTo(vector - one);
		}

		public uint RegisterStageEnvTriggerField(string fieldName)
		{
			MonoStageEnv stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
			GameObject gameObject = stageEnv.transform.Find(fieldName).gameObject;
			return Singleton<DynamicObjectManager>.Instance.RegisterStageEnvTriggerField(562036737u, gameObject);
		}

		public void LevelEndWithResult(bool isWin, int endCgId = 0)
		{
			EvtLevelState.State state = (isWin ? EvtLevelState.State.EndWin : EvtLevelState.State.EndLose);
			EvtLevelState.LevelEndReason reason = (isWin ? EvtLevelState.LevelEndReason.EndWin : EvtLevelState.LevelEndReason.EndLoseNotMeetCondition);
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(state, reason, endCgId));
		}

		public void BattleBegin()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.BattleBegin));
		}

		public void StartTutorial()
		{
			if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelTutorialHelperPlugin>())
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtTutorialState(EvtTutorialState.State.Start));
			}
		}

		public string[] GetAppliedToolList()
		{
			List<string> list = Singleton<LevelScoreManager>.Instance.appliedToolList;
			if (list != null)
			{
				list.Insert(0, list.Count.ToString());
			}
			else
			{
				list = new List<string>(new string[1] { "0" });
			}
			return list.ToArray();
		}

		public void EndlessLevelEnd(string result, bool collectAllGoods = true)
		{
			Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(LevelEndNewIter(result, true, collectAllGoods));
		}

		public void LevelEndNew(string result, bool collectAllGoods = true, int endCgId = 0)
		{
			MonoLevelEntity levelEntity = Singleton<LevelManager>.Instance.levelEntity;
			bool collectAllGoods2 = collectAllGoods;
			levelEntity.StartCoroutine(LevelEndNewIter(result, false, collectAllGoods2, endCgId));
		}

		private IEnumerator LevelEndNewIter(string result, bool isEndlessLevel = false, bool collectAllGoods = true, int endCgId = 0)
		{
			result = ((result != null) ? result : "win");
			MainCameraActor mainCameraActor = Singleton<EventManager>.Instance.GetActor<MainCameraActor>(Singleton<CameraManager>.Instance.GetMainCamera().GetRuntimeID());
			MuteInput();
			StopLevelDesign();
			Singleton<AvatarManager>.Instance.HideHelperAvatar(true);
			ClearPalsyBombProps();
			CameraActorLastKillCloseUpPlugin lastKillPlugin = mainCameraActor.GetPlugin<CameraActorLastKillCloseUpPlugin>();
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (lastKillPlugin != null && (result != "win" || !lastKillPlugin.IsPending()))
			{
				mainCameraActor.RemovePlugin(lastKillPlugin);
				lastKillPlugin = null;
			}
			bool wasInLastKill = false;
			while ((lastKillPlugin != null && lastKillPlugin.IsPending()) || mainCamera.followState.slowMotionKillState.active)
			{
				wasInLastKill = true;
				yield return null;
			}
			lastKillPlugin = mainCameraActor.GetPlugin<CameraActorLastKillCloseUpPlugin>();
			if (lastKillPlugin != null)
			{
				Debug.LogError("removing last kill plugin again, which should not happen. monster count: " + Singleton<MonsterManager>.Instance.MonsterCount());
				mainCameraActor.RemovePlugin(lastKillPlugin);
			}
			if (wasInLastKill)
			{
				while (Singleton<MonsterManager>.Instance.MonsterCount() > 0)
				{
					yield return null;
				}
			}
			else
			{
				ClearAllMonsters();
			}
			if (result == "win" && collectAllGoods)
			{
				CollectAllGoods();
			}
			while (mainCamera.avatarAnimState.active || mainCamera.levelAnimState.active)
			{
				yield return null;
			}
			if (Singleton<LevelManager>.Instance.IsPaused())
			{
				Singleton<LevelManager>.Instance.SetPause(false);
			}
			Singleton<LevelManager>.Instance.SetMutePause(true);
			SetInLevelUIActive(false);
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			Singleton<LevelManager>.Instance.levelActor.abilityPlugin.StopAndDropAll();
			foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
			{
				if (Singleton<AvatarManager>.Instance.IsPlayerAvatar(avatar))
				{
					Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID()).abilityPlugin.StopAndDropAll();
					avatar.SetHasAdditiveVelocity(false);
					avatar.SetNeedOverrideVelocity(false);
				}
			}
			yield return new WaitForSeconds(1.4f);
			localAvatar.CleanOwnedObjects();
			Singleton<DynamicObjectManager>.Instance.CleanWhenStageChange();
			Singleton<CameraManager>.Instance.GetMainCamera().TransitToStatic();
			float time = 0.45f;
			MonoInLevelUICanvas mainCanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
			mainCanvas.SetWhiteTransitPanelActive(true);
			yield return new WaitForSeconds(time);
			localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (!localAvatar.IsAlive())
			{
				result = "lose";
			}
			LevelEndWithResult(result == "win", endCgId);
			if (!isEndlessLevel && localAvatar.IsAlive())
			{
				string animationName = ((!(result == "win")) ? localAvatar.config.LevelEndAnimation.LevelLoseAnim : localAvatar.config.LevelEndAnimation.LevelWinAnim);
				string avatarStateName = ((!(result == "win")) ? "Fail" : "Victory");
				MonoStageEnv env = Singleton<StageManager>.Instance.GetStageEnv();
				int bornIx = env.GetNamedSpawnPointIx("Born");
				if (bornIx == -1)
				{
					bornIx = 0;
				}
				MonoSpawnPoint spawn = env.spawnPoints[bornIx];
				MonoSimpleAnimation cameraAnimation = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoSimpleAnimation>(animationName, localAvatar.GetRuntimeID());
				localAvatar.DisableRootMotionAndCollision();
				localAvatar.transform.position = spawn.XZPosition;
				Vector3 forward = spawn.transform.forward;
				forward.y = 0f;
				localAvatar.transform.forward = forward;
				GameObject anchor = new GameObject("CameraAnchor");
				anchor.transform.position = spawn.transform.position;
				anchor.transform.rotation = spawn.transform.rotation;
				cameraAnimation.SetOwnedParent(anchor.transform);
				mainCamera.PlayAvatarCameraAnimationThenTransitToFollow(cameraAnimation.GetComponent<Animation>(), localAvatar, MainCameraFollowState.EnterPolarMode.AlongTargetPolar, false);
				localAvatar.PlayState(avatarStateName);
				if (avatarStateName == "Fail")
				{
					if (localAvatar is MonoBronya)
					{
						((MonoBronya)localAvatar).SetMCVisible(false);
					}
					localAvatar.DetachWeapon();
				}
				while (cameraAnimation != null && cameraAnimation.GetComponent<Animation>() != null && cameraAnimation.GetComponent<Animation>().isPlaying)
				{
					yield return null;
				}
			}
			Singleton<LevelManager>.Instance.SetMutePause(false);
		}

		public void CollectAllGoods()
		{
			List<MonoGoods> allMonoGoods = Singleton<DynamicObjectManager>.Instance.GetAllMonoGoods();
			foreach (MonoGoods item in allMonoGoods)
			{
				if (!item.IsActive())
				{
					continue;
				}
				item.forceFlyToAvatar = true;
				item.muteSound = true;
				item.state = MonoGoods.GoodsState.Attract;
				item.SetAttractTimerActive(true);
				if (string.IsNullOrEmpty(item.AttachEffectPattern) || Singleton<EventManager>.Instance.GetActor<EquipItemActor>(item.GetRuntimeID()) == null)
				{
					continue;
				}
				foreach (MonoEffect outsideEffect in item.OutsideEffects)
				{
					if (outsideEffect != null)
					{
						outsideEffect.SetDestroyImmediately();
					}
				}
			}
		}

		public Vector3 GetSpawnPointPos(string spawnName)
		{
			Vector3 xZPosition = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
			switch (spawnName)
			{
			case "OnAvatar":
				return xZPosition;
			case "TutorialPos":
				return xZPosition - xZPosition.normalized * 2.1f;
			default:
				return GetSpawnPoint(spawnName).transform.position;
			}
		}

		private MonoSpawnPoint GetSpawnPoint(string spawnName)
		{
			MonoStageEnv stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
			int num;
			if (spawnName != null)
			{
				num = stageEnv.GetNamedSpawnPointIx(spawnName);
				if (num < 0)
				{
					num = UnityEngine.Random.Range(0, Singleton<StageManager>.Instance.GetStageEnv().spawnPoints.Length);
				}
			}
			else
			{
				num = UnityEngine.Random.Range(0, Singleton<StageManager>.Instance.GetStageEnv().spawnPoints.Length);
			}
			return stageEnv.spawnPoints[num];
		}

		public void SetLevelDefendModePluginStart(int targetValue)
		{
			if (targetValue > 0)
			{
				Singleton<LevelManager>.Instance.levelActor.SetLevelDefendModePluginStart(targetValue);
			}
		}

		public void SetLevelDefendModePluginStart()
		{
			Singleton<LevelManager>.Instance.levelActor.SetLevelDefendModePluginStart();
		}

		public void LevelDefendModePluginStop()
		{
			Singleton<LevelManager>.Instance.levelActor.SetLevelDefendModePluginStop();
		}

		public void LevelDefendModePluginReset(int targetValue)
		{
			Singleton<LevelManager>.Instance.levelActor.SetLevelDefendModePluginReset(targetValue);
		}

		public int GetLevelDefendModeMonsterEnterAmount()
		{
			return Singleton<LevelManager>.Instance.levelActor.GetLevelDefendModeMonsterEnterAmount();
		}

		public int GetLevelDefendModeMonsterKillAmount()
		{
			return Singleton<LevelManager>.Instance.levelActor.GetLevelDefendModeMonsterKillAmount();
		}

		public void AddLevelDefendModeData(string modeType, int modeValue, bool isKey)
		{
			if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDefendModePlugin>())
			{
				DefendModeType modeType2 = (DefendModeType)(int)Enum.Parse(typeof(DefendModeType), modeType);
				LevelDefendModePlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDefendModePlugin>();
				if (plugin != null)
				{
					plugin.AddModeData(modeType2, modeValue, isKey);
				}
			}
		}

		public void AddLevelDefendModeData(int uniqueID, bool isKey)
		{
			if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDefendModePlugin>())
			{
				LevelDefendModePlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDefendModePlugin>();
				if (plugin != null)
				{
					plugin.AddModeData(uniqueID, isKey);
				}
			}
		}

		public void AddLevelDefendModeData(string modeType, int targetModeValue, int currentModeValue, bool isKey)
		{
			if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDefendModePlugin>())
			{
				DefendModeType modeType2 = (DefendModeType)(int)Enum.Parse(typeof(DefendModeType), modeType);
				LevelDefendModePlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDefendModePlugin>();
				if (plugin != null)
				{
					plugin.AddModeData(modeType2, targetModeValue, currentModeValue, isKey);
				}
			}
		}

		public void SetLevelDefendModeStop()
		{
			if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDefendModePlugin>())
			{
				LevelDefendModePlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDefendModePlugin>();
				plugin.SetActive(false);
				plugin.Stop();
			}
		}

		public void MainCameraFollowLookAt(uint runtimeID, bool mute = false)
		{
			BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(runtimeID);
			if (entity != null && entity.IsActive())
			{
				Singleton<CameraManager>.Instance.GetMainCamera().FollowLookAtPosition(entity.XZPosition, mute);
			}
		}

		public void MainCameraFollowLookAtPoint(string pointName, bool mute = false)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(pointName);
			Singleton<CameraManager>.Instance.GetMainCamera().FollowLookAtPosition(spawnPoint.transform.position, mute);
		}

		public void MainCameraFollowSetFar()
		{
			Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.Far);
		}

		public void MainCameraFollowSetFurther()
		{
			Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.Furter);
		}

		public void MainCameraFollowSetNear()
		{
			Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.Near);
		}

		public void MainCameraFollowSetHigh()
		{
			Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.High);
		}

		public void MainCameraFollowSetHigher()
		{
			Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.Higher);
		}

		public bool IsPointInCameraFov(Vector3 pos)
		{
			bool flag = false;
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			Vector3 vector = pos - mainCamera.transform.position;
			float num = Vector3.Angle(vector, mainCamera.transform.forward);
			return num < mainCamera.cameraComponent.fieldOfView - 10f;
		}

		public void SetupLastKilLCameraCloseUp()
		{
			MainCameraActor actor = Singleton<EventManager>.Instance.GetActor<MainCameraActor>(Singleton<CameraManager>.Instance.GetMainCamera().GetRuntimeID());
			actor.SetupLastKillCloseUp();
		}

		public void PreloadMonster(string monsterName, string typeName, bool disableBehaviorWhenInit = false)
		{
			Singleton<MonsterManager>.Instance.PreloadMonster(monsterName, typeName, 0u);
		}

		public void PreloadUniqueMonster(uint uniqueMonsterID, bool disableBehaviorWhenInit = false)
		{
			UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
			string monsterName = uniqueMonsterMetaData.monsterName;
			string typeName = uniqueMonsterMetaData.typeName;
			Singleton<MonsterManager>.Instance.PreloadMonster(monsterName, typeName, uniqueMonsterID);
		}

		public void PlayCameraAnimationOnEnv(string animationName, bool enterLerp, bool exitLerp, bool pauseLevel, CameraAnimationCullingType cullType = CameraAnimationCullingType.CullNothing)
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (!mainCamera.levelAnimState.active)
			{
				MonoSimpleAnimation monoSimpleAnimation = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoSimpleAnimation>(animationName, 562036737u);
				Transform transform = Singleton<StageManager>.Instance.GetStageEnv().transform;
				monoSimpleAnimation.transform.parent = transform;
				monoSimpleAnimation.transform.localPosition = Vector3.zero;
				monoSimpleAnimation.transform.localRotation = Quaternion.identity;
				mainCamera.PlayLevelAnimationThenTransitBack(monoSimpleAnimation.GetComponent<Animation>(), true, enterLerp, exitLerp, pauseLevel, cullType);
			}
		}

		public void PlayCameraAnimationOnAnimatorEntity(string animationName, uint entityID, bool enterLerp, bool exitLerp, bool pauselevel, bool diableEntityFadeMatieral = false, bool cullingAvatar = false)
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (mainCamera.levelAnimState.active)
			{
				return;
			}
			MonoSimpleAnimation monoSimpleAnimation = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoSimpleAnimation>(animationName, 562036737u);
			BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(entityID);
			monoSimpleAnimation.transform.parent = entity.transform;
			monoSimpleAnimation.transform.localPosition = Vector3.zero;
			monoSimpleAnimation.transform.localRotation = Quaternion.identity;
			Action startCallback = null;
			Action endCallback = null;
			BaseMonoAnimatorEntity animatorEntity = entity as BaseMonoAnimatorEntity;
			BaseAbilityActor abilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(entity.GetRuntimeID());
			if (animatorEntity != null)
			{
				startCallback = delegate
				{
					if (diableEntityFadeMatieral)
					{
						animatorEntity.SetMonsterMaterialFadeEnabled(false);
					}
					if (abilityActor != null)
					{
						abilityActor.isInLevelAnim = true;
					}
					Singleton<EffectManager>.Instance.SetAllAliveEffectPause(true);
				};
				endCallback = delegate
				{
					if (diableEntityFadeMatieral)
					{
						animatorEntity.SetMonsterMaterialFadeEnabled(true);
					}
					if (abilityActor != null)
					{
						abilityActor.isInLevelAnim = false;
					}
					Singleton<EffectManager>.Instance.SetAllAliveEffectPause(false);
				};
			}
			CameraAnimationCullingType cullType = (cullingAvatar ? CameraAnimationCullingType.CullAvatars : CameraAnimationCullingType.CullNothing);
			mainCamera.PlayLevelAnimationThenTransitBack(monoSimpleAnimation.GetComponent<Animation>(), true, enterLerp, exitLerp, pauselevel, cullType, startCallback, endCallback);
		}

		public void PlayCameraAnimationOnAnimatorEntityThenStay(string animationName, uint entityID, bool pauseLevel)
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (!mainCamera.levelAnimState.active)
			{
				MonoSimpleAnimation monoSimpleAnimation = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoSimpleAnimation>(animationName, 562036737u);
				BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(entityID);
				monoSimpleAnimation.transform.parent = entity.transform;
				monoSimpleAnimation.transform.localPosition = Vector3.zero;
				monoSimpleAnimation.transform.localRotation = Quaternion.identity;
				mainCamera.PlayAvatarCameraAnimationThenStay(monoSimpleAnimation.GetComponent<Animation>(), Singleton<AvatarManager>.Instance.GetLocalAvatar());
			}
		}

		public string GetMonsterConfigField(string category, string subType, string query)
		{
			ConfigMonster monsterConfig = MonsterData.GetMonsterConfig(category, subType, string.Empty);
			query = query.Trim();
			char[] separator = new char[1] { '.' };
			string[] array = query.Split(separator);
			if (array.Length <= 0)
			{
				return string.Empty;
			}
			return GetClassFieldOrDictValue(monsterConfig, array, 0);
		}

		public string GetAvatarConfigField(string type, string query)
		{
			ConfigAvatar avatarConfig = AvatarData.GetAvatarConfig(type);
			query = query.Trim();
			char[] separator = new char[1] { '.' };
			string[] array = query.Split(separator);
			if (array.Length <= 0)
			{
				return string.Empty;
			}
			return GetClassFieldOrDictValue(avatarConfig, array, 0);
		}

		public string GetClassFieldOrDictValue(object obj, string[] fieldStrs, int currentIndex)
		{
			if (fieldStrs.Length - 1 < currentIndex)
			{
				return obj.ToString();
			}
			string text = fieldStrs[currentIndex];
			Type type = obj.GetType();
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				IDictionary dictionary = (IDictionary)obj;
				if (!dictionary.Contains(text))
				{
					return string.Empty;
				}
				return GetClassFieldOrDictValue(dictionary[text], fieldStrs, currentIndex + 1);
			}
			FieldInfo field = type.GetField(text);
			if (field == null)
			{
				return string.Empty;
			}
			return GetClassFieldOrDictValue(field.GetValue(obj), fieldStrs, currentIndex + 1);
		}

		public void SetMonsterHPRatio(uint monsterRuntimeID, float ratio)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monsterRuntimeID);
			if (actor != null)
			{
				actor.SetMonsterHPRatio(ratio);
			}
		}

		public void SetMonsterAttackRatio(uint monsterRuntimeID, float ratio)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monsterRuntimeID);
			if (actor != null)
			{
				actor.SetMonsterAttackRatio(ratio);
			}
		}

		public void SetAvatarAttackRatio(float ratio)
		{
			foreach (BaseMonoAvatar allAvatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
			{
				Singleton<EventManager>.Instance.GetActor<AvatarActor>(allAvatar.GetRuntimeID()).SetAvatarAttackRatio(ratio);
			}
		}

		public bool IsMonsterAlive(uint runtimeID)
		{
			return Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID) != null;
		}

		public int GetNPCHardLevel()
		{
			return Singleton<LevelScoreManager>.Instance.NPCHardLevel;
		}

		public bool GetIsDebugDynamicHardLevel()
		{
			return Singleton<LevelScoreManager>.Instance.isDebugDynamicLevel;
		}

		public object GetMonsterBasicParameters(string MonsterName, string ConfigTypeName, int level, bool isElite)
		{
			MonsterConfigMetaData monsterConfigMetaData = MonsterData.GetMonsterConfigMetaData(MonsterName, ConfigTypeName);
			ConfigMonster monsterConfig = MonsterData.GetMonsterConfig(MonsterName, ConfigTypeName, string.Empty);
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			NPCLevelMetaData nPCLevelMetaDataByKey = NPCLevelMetaDataReader.GetNPCLevelMetaDataByKey(level);
			num = monsterConfigMetaData.HP * nPCLevelMetaDataByKey.HPRatio;
			num2 = monsterConfigMetaData.defense * nPCLevelMetaDataByKey.DEFRatio;
			num3 = monsterConfigMetaData.attack * nPCLevelMetaDataByKey.ATKRatio;
			if (isElite)
			{
				num *= monsterConfig.EliteArguments.HPRatio;
				num2 *= monsterConfig.EliteArguments.DefenseRatio;
				num3 *= monsterConfig.EliteArguments.AttackRatio;
			}
			return new float[3] { num, num2, num3 };
		}

		public LuaTable GetLevelLuaTableByLevelName(string levelScript)
		{
			object[] array = _luaState.DoString(Miscs.LoadTextFileToString(levelScript), levelScript, null);
			return (LuaTable)array[0];
		}

		public void KillAllMonsters(bool dropReward)
		{
			MonsterActor[] actorByCategory = Singleton<EventManager>.Instance.GetActorByCategory<MonsterActor>(4);
			foreach (MonsterActor monsterActor in actorByCategory)
			{
				if (!monsterActor.monster.isStaticInScene)
				{
					monsterActor.needDropReward = dropReward;
					monsterActor.ForceKill();
				}
			}
		}

		public void ClearAllMonsters()
		{
			MonsterActor[] actorByCategory = Singleton<EventManager>.Instance.GetActorByCategory<MonsterActor>(4);
			foreach (MonsterActor monsterActor in actorByCategory)
			{
				if (!monsterActor.monster.isStaticInScene)
				{
					monsterActor.ForceRemoveImmediatelly();
				}
			}
		}

		public void ClearPalsyBombProps()
		{
			foreach (BaseMonoPropObject allPropObject in Singleton<PropObjectManager>.Instance.GetAllPropObjects())
			{
				if (allPropObject is MonoPalsyBombProp)
				{
					((MonoPalsyBombProp)allPropObject).SetDied(KillEffect.KillImmediately);
				}
			}
		}

		public void SetMonsterEnabled(uint monsterID, bool enabled)
		{
			if (Singleton<EventManager>.Instance.GetActor(monsterID) != null)
			{
				BaseMonoMonster monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(monsterID);
				if (!enabled)
				{
					monsterByRuntimeID.OrderMove = false;
					monsterByRuntimeID.ClearHitTrigger();
					monsterByRuntimeID.ClearAttackTriggers();
					monsterByRuntimeID.SetUseAIController(false);
				}
				else
				{
					monsterByRuntimeID.SetUseAIController(true);
				}
			}
		}

		public void SetMonsterMaterialFadeEnabled(uint monsterID, bool enabled)
		{
			if (Singleton<EventManager>.Instance.GetActor(monsterID) != null)
			{
				BaseMonoMonster monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(monsterID);
				monsterByRuntimeID.SetMonsterMaterialFadeEnabled(enabled);
			}
		}

		public void SetInLevelUIActive(bool isActive)
		{
			MonoInLevelUICanvas inLevelUICanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
			if (inLevelUICanvas != null)
			{
				inLevelUICanvas.SetInLevelUIActive(isActive);
			}
		}

		public void SetEnvCollisionActive(bool isActive)
		{
			if (Singleton<StageManager>.Instance.GetStageEnv().transform.FindChild("Collision") != null)
			{
				Singleton<StageManager>.Instance.GetStageEnv().transform.FindChild("Collision").gameObject.SetActive(isActive);
			}
		}

		public void SwitchPlayMode(string playMode)
		{
			Singleton<LevelManager>.Instance.levelActor.levelMode = (LevelActor.Mode)(int)Enum.Parse(typeof(LevelActor.Mode), playMode);
			foreach (BaseMonoAvatar allPlayerAvatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
			{
				allPlayerAvatar.RefreshController();
			}
		}

		public void SetNatureBonusFactor(float upFactor, float downFactor)
		{
			Singleton<LevelManager>.Instance.levelActor.upLevelNatureBonusFactor = Mathf.Clamp(upFactor, 0f, upFactor);
			Singleton<LevelManager>.Instance.levelActor.downLevelNatureBonusFactor = Mathf.Clamp(downFactor, 0f, downFactor);
		}

		public void StartInLevelLevelTimer()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (!levelActor.HasPlugin<LevelActorTimerPlugin>())
			{
				levelActor.AddPlugin(new LevelActorTimerPlugin(levelActor));
			}
			LevelActorTimerPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorTimerPlugin>();
			plugin.StartTiming();
		}

		public void StopInLevelLevelTimer()
		{
			LevelActorTimerPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorTimerPlugin>();
			if (plugin != null)
			{
				plugin.StopTiming();
			}
		}

		public void SetInLevelTimeCountDown(float time)
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (levelActor.HasPlugin<LevelActorCountDownPlugin>())
			{
				levelActor.GetPlugin<LevelActorCountDownPlugin>().ResetPlugin(time);
			}
			else
			{
				levelActor.AddPlugin(new LevelActorCountDownPlugin(levelActor, time));
			}
			SetInLevelTimeCountDownVisible(true);
		}

		public void SetInLevelTimeCountDownSpeedRatio(float ratioInNormalTime, float ratioInWitchTime)
		{
			LevelActorCountDownPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>();
			if (plugin != null)
			{
				plugin.SetCountDownSpeedRatio(ratioInNormalTime, ratioInWitchTime);
			}
		}

		public void RemoveInLevelTimeCountDown()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (levelActor.HasPlugin<LevelActorCountDownPlugin>())
			{
				levelActor.RemovePlugin<LevelActorCountDownPlugin>();
				SetInLevelTimeCountDownVisible(false);
			}
		}

		public void AddTheRemainTimeInLevel(float timeDelta)
		{
			LevelActorCountDownPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>();
			if (plugin != null)
			{
				plugin.AddRemainTime(timeDelta);
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowAddTimeText, timeDelta));
			}
		}

		public void StopInLevelTimeCountDown()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (levelActor.HasPlugin<LevelActorCountDownPlugin>())
			{
				Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>().isTiming = false;
			}
		}

		public bool HasLevelActorCountDownPlugin()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			return levelActor.HasPlugin<LevelActorCountDownPlugin>();
		}

		public void StartInLevelTimeCountDown()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (levelActor.HasPlugin<LevelActorCountDownPlugin>())
			{
				Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>().isTiming = true;
			}
		}

		public void SetInLevelTimesUpWin(bool timesUpWin)
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (levelActor.HasPlugin<LevelActorCountDownPlugin>())
			{
				Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>().timeUpWin = timesUpWin;
			}
		}

		public float GetInLevelTimeCountDown()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			return Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>().countDownTimer;
		}

		public float GetEndlessLevelTimeCountDown()
		{
			return Singleton<LevelScoreManager>.Instance.levelTimer;
		}

		public int GetEndlessGroupLevel()
		{
			if (Singleton<EndlessModule>.Instance == null)
			{
				return 0;
			}
			return Singleton<EndlessModule>.Instance.currentGroupLevel;
		}

		public int GetEndlessRandomSeed()
		{
			if (Singleton<EndlessModule>.Instance == null)
			{
				return 0;
			}
			return Singleton<EndlessModule>.Instance.randomSeed;
		}

		private void SetInLevelTimeCountDownVisible(bool isVisible)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetTimeCountDownTextActive, isVisible));
		}

		public bool IsPlayerBehaviourDone(string key)
		{
			return Singleton<PlayerModule>.Instance.IsBehaviourDone(key);
		}

		public void SetPlayerBehaviourDone(string key)
		{
			Singleton<PlayerModule>.Instance.SetBehaviourDone(key);
		}

		public object LoadLDDropDataItem(string goodsTypeName)
		{
			LDDropDataItem lDDropDataItemByName = LDDropDataItem.GetLDDropDataItemByName(goodsTypeName);
			return lDDropDataItemByName.Clone();
		}

		public void CreateDropGoods(LuaTable dropTable, string spawnName, bool actDropAnim = false)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			Vector3 position = spawnPoint.transform.position;
			position.y = 0.4f;
			foreach (object value in dropTable.Values)
			{
				(value as LDDropDataItem).CreateDropGoods(position, spawnPoint.transform.forward, actDropAnim);
			}
		}

		public void CreateDropGoodsBetweenTwoPoint(LuaTable dropTable, string spawnName1, string spawnName2, int dropNum, bool actDropAnim = false)
		{
			Vector3 position = GetSpawnPoint(spawnName1).transform.position;
			Vector3 position2 = GetSpawnPoint(spawnName2).transform.position;
			for (int i = 0; i < dropNum; i++)
			{
				foreach (object value in dropTable.Values)
				{
					(value as LDDropDataItem).CreateDropGoods(position + (position2 - position) * i / (dropNum - 1), Vector3.forward, actDropAnim);
				}
			}
		}

		public void CreateDropGoodsByMonsterID(LuaTable dropTable, uint monsterID, bool actDropAnim = true)
		{
			BaseMonoMonster monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(monsterID);
			Vector3 dropPosition = monsterByRuntimeID.GetDropPosition();
			foreach (object value in dropTable.Values)
			{
				(value as LDDropDataItem).CreateDropGoods(dropPosition, Vector3.forward, actDropAnim);
			}
		}

		public void SetupLevelReward(LuaTable dropTable, LuaTable otherReward)
		{
			List<DropItem> configLevelDrops = Singleton<LevelScoreManager>.Instance.configLevelDrops;
			if (configLevelDrops != null)
			{
				for (int i = 0; i < configLevelDrops.Count; i++)
				{
					dropTable[i] = configLevelDrops[i];
				}
			}
			otherReward["AvatarExpInside"] = Singleton<LevelScoreManager>.Instance.configAvatarExpInside;
			otherReward["ScoinInside"] = Singleton<LevelScoreManager>.Instance.configScoinInside;
		}

		public string GetLevelDifficulty()
		{
			switch (Singleton<LevelScoreManager>.Instance.difficulty)
			{
			case 1:
				return "Normal";
			case 2:
				return "Hard";
			case 3:
				return "Hell";
			default:
				return "Normal";
			}
		}

		public int GetCurrentProgress()
		{
			return Singleton<LevelScoreManager>.Instance.progress;
		}

		public int GetLevelMode()
		{
			return (int)Singleton<LevelScoreManager>.Instance.levelMode;
		}

		public void SetupLevelDamageStastics()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.SetupLevelDamageStastics();
		}

		public void RemoveLevelDamageStastics()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (levelActor.HasPlugin<LevelDamageStasticsPlugin>())
			{
				levelActor.RemoveLevelDamageStastics();
			}
		}

		public void ControlLevelDamageStastics(string type)
		{
			string text = "DamageStastics";
			DamageStastcisControlType type2 = (DamageStastcisControlType)(int)Enum.Parse(typeof(DamageStastcisControlType), text + type);
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.ControlLevelDamageStastics(type2);
		}

		public void CreateEffectPatternWithMapKey(string effectPattern, string effectListMapKey)
		{
			Singleton<EffectManager>.Instance.CreateUniqueIndexedEffectPattern(effectPattern, effectListMapKey, Singleton<LevelManager>.Instance.levelEntity);
		}

		public void TryClearEffectListByMapKey(string effectListMapKey)
		{
			Singleton<EffectManager>.Instance.TrySetDestroyUniqueIndexedEffectPattern(effectListMapKey);
		}

		public void SetStageBaseRenderingDataWithTransition(string stageRenderingDataName, float transitDuration)
		{
			Singleton<StageManager>.Instance.GetPerpStage().SetBaseRenderingData(stageRenderingDataName, transitDuration);
		}

		public int PushRenderingDataWithTransition(string stageRenderingDataName, float transitDuration)
		{
			return Singleton<StageManager>.Instance.GetPerpStage().PushRenderingData(stageRenderingDataName, transitDuration);
		}

		public void PopRenderingData(int ix)
		{
			Singleton<StageManager>.Instance.GetPerpStage().PopRenderingData(ix);
		}

		public void ResetStageBaseRenderingData(float duration)
		{
			Singleton<StageManager>.Instance.GetPerpStage().ResetBaseRenderingData(duration);
		}

		public string GetStageBaseRenderingDataName()
		{
			return Singleton<StageManager>.Instance.GetPerpStage().GetCurrentBaseWeatherName();
		}

		public void SetStageBaseWeatherWithTransition(string weatherName, float transitDuration)
		{
			Singleton<StageManager>.Instance.GetPerpStage().SetBaseWeather(weatherName, transitDuration);
		}

		public void ResetStageBaseWeather(float transitDuration)
		{
			Singleton<StageManager>.Instance.GetPerpStage().ResetBaseWeather(transitDuration);
		}

		public string GetStageBaseWeatherName()
		{
			return Singleton<StageManager>.Instance.GetPerpStage().GetCurrentBaseWeatherName();
		}

		public int PushWeatherWithTransition(string stageWeatherName, float transitDuration)
		{
			return Singleton<StageManager>.Instance.GetPerpStage().PushWeather(stageWeatherName, transitDuration);
		}

		public void PopWeather(int ix)
		{
			Singleton<StageManager>.Instance.GetPerpStage().PopWeather(ix);
		}

		public void SetAvatarBeAttackMaxNum(int maxNum)
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.SetAvatarBeAttackMaxNum(maxNum);
		}

		public void DisableAvatarRootMotion()
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (localAvatar != null)
			{
				localAvatar.SetNeedOverrideVelocity(true);
			}
		}

		public void ForceUseAvatarAI()
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (localAvatar != null)
			{
				localAvatar.ForceUseAIController();
			}
		}

		public uint GetLocalAvatarID()
		{
			return Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
		}

		public void DisableAllMonsterRootMotionAndAI()
		{
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			for (int i = 0; i < allMonsters.Count; i++)
			{
				BaseMonoMonster baseMonoMonster = allMonsters[i];
				if (baseMonoMonster != null)
				{
					baseMonoMonster.SetNeedOverrideVelocity(true);
					baseMonoMonster.SetUseAIController(false);
					MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(baseMonoMonster.GetRuntimeID());
					actor.config.CommonArguments.BePushedSpeedRatio = 0f;
					actor.config.CommonArguments.BePushedSpeedRatioThrow = 0f;
				}
			}
		}

		public void DisableMonsterRootMotionAndAI(uint runtimeID)
		{
			BaseMonoMonster baseMonoMonster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
			if (baseMonoMonster != null)
			{
				baseMonoMonster.SetNeedOverrideVelocity(true);
				baseMonoMonster.SetUseAIController(false);
				MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(baseMonoMonster.GetRuntimeID());
				actor.config.CommonArguments.BePushedSpeedRatio = 0f;
				actor.config.CommonArguments.BePushedSpeedRatioThrow = 0f;
			}
		}

		public void SetupBGM(LuaTable table, string initState)
		{
		}

		public void SetBGMState(string bgmState)
		{
			if (bgmState == "Battle")
			{
				Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_ResumeVol_B");
			}
			else if (bgmState == "NonBattle")
			{
				Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_TurnDownVol_B");
			}
		}

		public void PlayBGMByName(string bgmName)
		{
			Singleton<WwiseAudioManager>.Instance.SetSwitch("Game_Stage_Type", bgmName);
			Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_Start");
		}

		public void ResetAudioListener()
		{
			Singleton<WwiseAudioManager>.Instance.ResetListener();
		}

		public void PlayBGMByNameWithDelay(string bgmName, float delay)
		{
		}

		public void AddBGMByName(string bgmName)
		{
		}

		public void StopBGMWithDelay(string bgmName, float delay)
		{
		}

		public void DebugDestroyDynamicObject(uint objectID)
		{
			BaseMonoDynamicObject dynamicObjectByRuntimeID = Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(objectID);
			dynamicObjectByRuntimeID.SetDied();
			dynamicObjectByRuntimeID.gameObject.SetActive(false);
		}

		public void DebugSetLocalAvatar(string avatarTypeName, LuaTable abilities, int weaponID, int level = 1, int star = 0)
		{
			Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(DebugSetLocalAvatarIter(avatarTypeName, abilities, weaponID, level, star));
		}

		public void DebugSetLocalAvatarByAvatarModule(string avatarTypeName, LuaTable abilities)
		{
			List<AvatarDataItem> userAvatarList = Singleton<AvatarModule>.Instance.UserAvatarList;
			foreach (AvatarDataItem item in userAvatarList)
			{
				if (item.AvatarRegistryKey == avatarTypeName)
				{
					AvatarDataItem avatarDataItem = item.Clone();
					Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(DebugSetLocalAvatarIter(avatarDataItem, abilities));
					break;
				}
			}
		}

		public int DebugGetLocalAvatarLevel()
		{
			return Singleton<EventManager>.Instance.GetActor<AvatarActor>(GetLocalAvatarID()).level;
		}

		public int DebugGetLocalAvatarWeaponRarity()
		{
			return Singleton<EventManager>.Instance.GetActor<AvatarActor>(GetLocalAvatarID()).avatarDataItem.GetWeapon().rarity;
		}

		private IEnumerator DebugSetLocalAvatarIter(string avatarTypeName, LuaTable abilities, int weaponID, int level, int star)
		{
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			BaseMonoAvatar targetAvatar = null;
			foreach (BaseMonoAvatar avatar in allAvatars)
			{
				if (Singleton<AvatarManager>.Instance.IsLocalAvatar(avatar.GetRuntimeID()))
				{
					continue;
				}
				targetAvatar = avatar;
				break;
			}
			uint targetAvatarID = targetAvatar.GetRuntimeID();
			AvatarActor targetAvatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(targetAvatarID);
			Vector3 localAvatarXZPosition = targetAvatar.XZPosition;
			Vector3 localAvatarForward = targetAvatar.FaceDirection;
			Singleton<EventManager>.Instance.MaskEventType(typeof(EvtAvatarCreated));
			Singleton<EventManager>.Instance.MaskEventType(typeof(EvtKilled));
			targetAvatarActor.ForceKill(562036737u, KillEffect.KillImmediately);
			Singleton<AvatarManager>.Instance.RemoveAvatarByRuntimeID(targetAvatarID);
			yield return null;
			AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(avatarTypeName, level, star);
			ConfigAvatar avatarConfig = AvatarData.GetAvatarConfig(avatarTypeName);
			WeaponDataItem weaponData = ((weaponID != 0) ? Singleton<StorageModule>.Instance.GetDummyWeaponDataItem(weaponID, 1) : Singleton<StorageModule>.Instance.GetDummyFirstWeaponDataByRole(avatarConfig.CommonArguments.RoleName, 1));
			avatarDataItem.equipsMap[(EquipmentSlot)1] = weaponData;
			targetAvatarID = Singleton<AvatarManager>.Instance.CreateAvatar(avatarDataItem, false, localAvatarXZPosition, localAvatarForward, targetAvatarID, false, false);
			targetAvatar = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(targetAvatarID);
			targetAvatar.gameObject.SetActive(false);
			targetAvatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(targetAvatarID);
			foreach (object obj in abilities.Values)
			{
				targetAvatarActor.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(obj.ToString()));
			}
			foreach (MonoAvatarButton button in Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.avatarButtonContainer.avatarBtnList)
			{
				if (button.avatarRuntimeID == targetAvatarID)
				{
					button.SetupAvatar(targetAvatarID);
				}
			}
			Singleton<EventManager>.Instance.UnmaskEventType(typeof(EvtAvatarCreated));
			Singleton<EventManager>.Instance.UnmaskEventType(typeof(EvtKilled));
			Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), targetAvatarID, true);
		}

		private IEnumerator DebugSetLocalAvatarIter(AvatarDataItem avatarDataItem, LuaTable abilities)
		{
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			BaseMonoAvatar targetAvatar = null;
			foreach (BaseMonoAvatar avatar in allAvatars)
			{
				if (Singleton<AvatarManager>.Instance.IsLocalAvatar(avatar.GetRuntimeID()))
				{
					continue;
				}
				targetAvatar = avatar;
				break;
			}
			uint targetAvatarID = targetAvatar.GetRuntimeID();
			AvatarActor targetAvatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(targetAvatarID);
			Vector3 localAvatarXZPosition = targetAvatar.XZPosition;
			Vector3 localAvatarForward = targetAvatar.FaceDirection;
			Singleton<EventManager>.Instance.MaskEventType(typeof(EvtAvatarCreated));
			Singleton<EventManager>.Instance.MaskEventType(typeof(EvtKilled));
			targetAvatarActor.ForceKill(562036737u, KillEffect.KillImmediately);
			Singleton<AvatarManager>.Instance.RemoveAvatarByRuntimeID(targetAvatarID);
			yield return null;
			targetAvatarID = Singleton<AvatarManager>.Instance.CreateAvatar(avatarDataItem, false, localAvatarXZPosition, localAvatarForward, targetAvatarID, false, false);
			targetAvatar = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(targetAvatarID);
			targetAvatar.gameObject.SetActive(false);
			targetAvatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(targetAvatarID);
			foreach (object obj in abilities.Values)
			{
				targetAvatarActor.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(obj.ToString()));
			}
			foreach (MonoAvatarButton button in Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.avatarButtonContainer.avatarBtnList)
			{
				if (button.avatarRuntimeID == targetAvatarID)
				{
					button.SetupAvatar(targetAvatarID);
				}
			}
			Singleton<EventManager>.Instance.UnmaskEventType(typeof(EvtAvatarCreated));
			Singleton<EventManager>.Instance.UnmaskEventType(typeof(EvtKilled));
			Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), targetAvatarID, true);
		}

		public void DebugSetBehaviorTree(uint runtimeID, string treeAssetPath)
		{
			BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(runtimeID);
			BehaviorTree component = entity.GetComponent<BehaviorTree>();
			if (!(component == null))
			{
				ExternalBehaviorTree externalBehavior = Miscs.LoadResource<ExternalBehaviorTree>(treeAssetPath);
				component.ExternalBehavior = externalBehavior;
				component.EnableBehavior();
			}
		}

		public void DebugSetAvatarAutoMoveBehavior(uint runtimeID, string spawnPoint)
		{
			BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
			BTreeAvatarAIController bTreeAvatarAIController = avatarByRuntimeID.GetActiveAIController() as BTreeAvatarAIController;
			bTreeAvatarAIController.ChangeToMoveBehavior(GetSpawnPointPos(spawnPoint));
		}

		public void DebugSetAvatarAutoMoveBehaviorWithPosition(uint runtimeID, Vector3 position)
		{
			BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
			BTreeAvatarAIController bTreeAvatarAIController = avatarByRuntimeID.GetActiveAIController() as BTreeAvatarAIController;
			bTreeAvatarAIController.ChangeToMoveBehavior(position);
		}

		public void DebugSetAvatarSupporterBehavior(uint runtimeID)
		{
			BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
			BTreeAvatarAIController bTreeAvatarAIController = avatarByRuntimeID.GetActiveAIController() as BTreeAvatarAIController;
			bTreeAvatarAIController.ChangeToSupporterBehavior();
		}

		public void DebugSetAvatarAutoBattleBehavior(uint runtimeID)
		{
			BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
			BTreeAvatarAIController bTreeAvatarAIController = avatarByRuntimeID.GetActiveAIController() as BTreeAvatarAIController;
			bTreeAvatarAIController.ChangeToAutoBattleBehavior();
		}

		public bool HasHelperAvatar()
		{
			if (Singleton<LevelScoreManager>.Instance.friendDetailItem != null)
			{
				return true;
			}
			return false;
		}

		public bool CreateHelperAvatar()
		{
			if (Singleton<LevelScoreManager>.Instance.friendDetailItem != null)
			{
				Singleton<AvatarManager>.Instance.ShowHelperAvater();
				return true;
			}
			return false;
		}

		public void DestroyHelperAvatar()
		{
			Singleton<AvatarManager>.Instance.HideHelperAvatar(false);
		}

		public void CreateGood(string goodType, string spawnName, string abilityname, float argument = 1f, bool actDropAnim = false)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			Singleton<DynamicObjectManager>.Instance.CreateGood(562036737u, goodType, abilityname, argument, spawnPoint.transform.position, spawnPoint.transform.forward, actDropAnim);
		}

		public uint CreateSpikeProp(string spawnName, int number, float attack, float rotation)
		{
			return CreateUnitFieldProp(spawnName, 1, number, "Trap_Spike", attack, rotation);
		}

		public uint SetSpikePropContineousState(uint runtimeID, bool isActive)
		{
			MonoSpikeTriggerProp monoSpikeTriggerProp = (MonoSpikeTriggerProp)Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
			monoSpikeTriggerProp.SetContinuousState(isActive);
			return runtimeID;
		}

		public void SetSpikeProp(uint runtimeID, float effectDuration, float CD)
		{
			MonoSpikeTriggerProp monoSpikeTriggerProp = (MonoSpikeTriggerProp)Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
			monoSpikeTriggerProp.SetSpikePropDurationAndCD(effectDuration, CD);
		}

		public uint CreateFireProp(string spawnName, int numberX, int numberZ, float attack, float rotation)
		{
			return CreateUnitFieldProp(spawnName, numberX, numberZ, "Trap_Fire", attack, rotation);
		}

		public void EnableFireProp(uint runtimeID, float effectDuration, float CD)
		{
			MonoFireTriggerProp monoFireTriggerProp = (MonoFireTriggerProp)Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
			monoFireTriggerProp.DisableProp();
			monoFireTriggerProp.EnableFire(effectDuration, CD);
		}

		public void DisableFireProp(uint runtimeID)
		{
			MonoFireTriggerProp monoFireTriggerProp = (MonoFireTriggerProp)Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
			monoFireTriggerProp.ForceDisableFire();
		}

		public uint CreatePalsyProp(string spawnName)
		{
			return CreatePropObject(spawnName, "Trap_Palsy");
		}

		public uint CreateSlowProp(string spawnName, int numberX, int numberZ, float rotation = 0f)
		{
			return CreateUnitFieldProp(spawnName, numberX, numberZ, "Trap_Slow", 0f, rotation);
		}

		private uint CreateUnitFieldProp(string spawnName, int numberX, int numberZ, string propName, float rotation = 0f)
		{
			ConfigPropObject propObjectConfig = PropObjectData.GetPropObjectConfig(propName);
			float attack = propObjectConfig.PropArguments.Attack;
			return CreateUnitFieldProp(spawnName, numberX, numberZ, propName, attack, rotation);
		}

		private uint CreateUnitFieldProp(string spawnName, int numberX, int numberZ, string propName, float atk, float rotation = 0f)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			ConfigPropObject propObjectConfig = PropObjectData.GetPropObjectConfig(propName);
			float hP = propObjectConfig.PropArguments.HP;
			uint num = Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, propName, hP, atk, spawnPoint.transform.position, spawnPoint.transform.forward);
			MonoTriggerUnitFieldProp monoTriggerUnitFieldProp = (MonoTriggerUnitFieldProp)Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(num);
			monoTriggerUnitFieldProp.InitUnitFieldPropRange(numberX, numberZ);
			monoTriggerUnitFieldProp.transform.RotateAround(monoTriggerUnitFieldProp.transform.position, Vector3.up, rotation);
			monoTriggerUnitFieldProp.EnableProp();
			return num;
		}

		public uint CreatePalsyBombProp(string spawnName)
		{
			return CreatePropObject(spawnName, "Trap_Palsy_Bomb");
		}

		public uint CreatePropObject(string spawnName, string propName)
		{
			ConfigPropObject propObjectConfig = PropObjectData.GetPropObjectConfig(propName);
			float hP = propObjectConfig.PropArguments.HP;
			float attack = propObjectConfig.PropArguments.Attack;
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			return Singleton<PropObjectManager>.Instance.CreatePropObject(562036737u, propName, hP, attack, spawnPoint.transform.position, spawnPoint.transform.forward);
		}

		public void EnterLevelTransition()
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel();
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EnterTransition));
		}

		public void ExitLevelTransition()
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel();
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition));
		}

		public void Fade(bool waitFadeOutEnd = false, bool waitFadeInEnd = false)
		{
			MonoInLevelUICanvas inLevelUICanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
			Action fadeEndCallback = delegate
			{
				if (waitFadeOutEnd)
				{
					Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition));
				}
				else
				{
					MonoInLevelUICanvas inLevelUICanvas2 = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
					Action fadeEndCallback2 = delegate
					{
						if (waitFadeInEnd)
						{
							Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition));
						}
					};
					inLevelUICanvas2.FadeInStageTransitPanel(0.18f, false, null, fadeEndCallback2);
				}
			};
			inLevelUICanvas.FadeOutStageTransitPanel(0.18f, false, null, fadeEndCallback);
		}

		public void MuteInput()
		{
			Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.JoystickVisible, false));
		}

		public void RecoveryInput()
		{
			Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.JoystickVisible, true));
		}

		public void ShowLevelDisplayText(string textMapKey, LuaTable paramTable = null)
		{
			string empty = string.Empty;
			if (paramTable != null)
			{
				object[] array = new object[paramTable.Values.Count];
				paramTable.Values.CopyTo(array, 0);
				empty = LocalizationGeneralLogic.GetTextWithParamArray(textMapKey, array);
			}
			else
			{
				empty = LocalizationGeneralLogic.GetText(textMapKey);
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLevelDisplayText, empty));
		}

		public Transform GetLocalAvatar()
		{
			return Singleton<AvatarManager>.Instance.GetLocalAvatar().transform;
		}

		public string GetAvatarShortName(Transform actor)
		{
			return actor.GetComponent<BaseMonoAvatar>().AvatarTypeName;
		}

		public Cutscene TriggerCinema(string name, Transform actor)
		{
			AvatarCinemaType type = name.ToEnum(AvatarCinemaType.Victory);
			ICinema cinemaDataByAvatar = Singleton<CinemaDataManager>.Instance.GetCinemaDataByAvatar(GetAvatarShortName(actor), type);
			cinemaDataByAvatar.Init(actor);
			cinemaDataByAvatar.Play();
			return cinemaDataByAvatar.GetCutscene();
		}

		public void EnableBossCamera(uint targetId)
		{
			Singleton<CameraManager>.Instance.EnableBossCamera(targetId);
		}

		public void DisableBossCamera()
		{
			Singleton<CameraManager>.Instance.DisableBossCamera();
		}

		public void EnableCrowdCamera()
		{
			Singleton<CameraManager>.Instance.EnableCrowdCamera();
		}

		public void DisableCrowdCamera()
		{
			Singleton<CameraManager>.Instance.DisableCrowdCamera();
		}

		public void EnterStoryMode(int plotID, bool lerpIn = true, bool lerpOut = true, bool needFadeIn = true, bool backFollow = true, bool pauseLevel = false)
		{
			if (!Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
			{
				SetPause(true);
				Singleton<CameraManager>.Instance.GetMainCamera().PlayStoryCameraState(plotID, lerpIn, lerpOut, needFadeIn, backFollow, pauseLevel);
			}
		}

		public bool AllowEnterStoryMode(int plotID)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between Unknown and I4
			PlotMetaData plotMetaDataByKey = PlotMetaDataReader.GetPlotMetaDataByKey(plotID);
			if (plotMetaDataByKey != null)
			{
				if (!Singleton<LevelModule>.Instance.ContainLevelById(plotMetaDataByKey.levelID))
				{
					return true;
				}
				LevelDataItem levelDataItem = Singleton<LevelModule>.Instance.TryGetLevelById(plotMetaDataByKey.levelID);
				if (levelDataItem != null)
				{
					return (int)levelDataItem.LevelType == 1;
				}
			}
			return true;
		}

		public void EnterStoryModeWithFollowingAnimation(int plotID, string followAnimationName)
		{
			if (!string.IsNullOrEmpty(followAnimationName))
			{
				if (AuxObjectData.ContainAuxObjectPrefabPath(followAnimationName))
				{
					EnterStoryMode(plotID, true, false, false);
				}
				else
				{
					EnterStoryMode(plotID);
				}
			}
		}

		public void ExitStoryMode()
		{
			if (!Singleton<CameraManager>.Instance.GetMainCamera().IsInTransitionLerp() && Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().storyState.StartQuit();
			}
		}

		public void SetCameraLocateRatio(float ratio)
		{
			Singleton<CameraManager>.Instance.GetMainCamera().SetUserDefinedCameraLocateRatio(ratio);
		}

		public void TriggerAbilityOnMonster(uint targetID, string abilityName, float abilityArgument = 0f)
		{
			Singleton<EventManager>.Instance.GetActor<MonsterActor>(targetID).abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig(abilityName), targetID, abilityArgument);
		}

		public void TriggerAbilityOnAvatar(bool toAllAvatars, string abilityName, float abilityArgument = 0f)
		{
			LevelActor levelActor = (LevelActor)Singleton<EventManager>.Instance.GetActor(562036737u);
			if (toAllAvatars)
			{
				foreach (BaseMonoAvatar allAvatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
				{
					if (levelActor.abilityPlugin != null)
					{
						levelActor.abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig(abilityName), allAvatar.GetRuntimeID(), abilityArgument);
					}
				}
				return;
			}
			if (levelActor.abilityPlugin != null)
			{
				levelActor.abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig(abilityName), Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), abilityArgument);
			}
		}

		public bool HasAbility(string abilityName)
		{
			if (string.IsNullOrEmpty(abilityName))
			{
				return false;
			}
			foreach (BaseMonoAvatar allAvatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(allAvatar.GetRuntimeID());
				if (actor != null && actor.abilityPlugin != null && actor.abilityPlugin.HasAbility(abilityName))
				{
					return true;
				}
			}
			return false;
		}

		public void SetAbilitySpecials(string abilityName, LuaTable overrideTable)
		{
			foreach (BaseMonoAvatar allAvatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
			{
				if (!Singleton<EventManager>.Instance.GetActor<AvatarActor>(allAvatar.GetRuntimeID()).abilityPlugin.HasAbility(abilityName))
				{
					continue;
				}
				List<ActorAbility> appliedAbilities = Singleton<EventManager>.Instance.GetActor<AvatarActor>(allAvatar.GetRuntimeID()).abilityPlugin.GetAppliedAbilities();
				foreach (ActorAbility item in appliedAbilities)
				{
					if (!(item.config.AbilityName == abilityName))
					{
						continue;
					}
					foreach (DictionaryEntry item2 in overrideTable)
					{
						string key = (string)item2.Key;
						if (item2.Value is double)
						{
							item.SetOverrideMapValue(key, (float)(double)item2.Value);
						}
						else if (item2.Value is string)
						{
							item.SetOverrideMapValue(key, (string)item2.Value);
						}
					}
				}
			}
		}

		public void AddHintArrowForPath(string spawnName)
		{
			MonoSpawnPoint spawnPoint = GetSpawnPoint(spawnName);
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.AddHintArrowForPath(spawnPoint);
		}

		public void RemoveHintArrowForPath()
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.RemoveHintArrowForPath();
		}

		public void SetMuteAvatarVoice(bool mute)
		{
			string empty = string.Empty;
			empty = ((!mute) ? "UI_Exit_StoryMode" : "UI_Enter_StoryMode");
			if (!string.IsNullOrEmpty(empty))
			{
				Singleton<WwiseAudioManager>.Instance.Post(empty);
			}
		}

		public bool AllowSkipVideo(int cgId)
		{
			return Singleton<CGModule>.Instance.GetFinishedCGIDList().Contains(cgId);
		}

		public bool LoadVideo(int cgId)
		{
			CgDataItem cgDataItem = Singleton<CGModule>.Instance.GetCgDataItem(cgId);
			if (cgDataItem != null)
			{
				Singleton<CGModule>.Instance.MarkCGIDFinish(cgDataItem.cgID);
				BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
				if (mainCanvas is MonoInLevelUICanvas)
				{
					MonoInLevelUICanvas monoInLevelUICanvas = mainCanvas as MonoInLevelUICanvas;
					monoInLevelUICanvas.LoadVideo(cgDataItem);
				}
				return true;
			}
			return false;
		}

		public bool PlayVideo(int cgId, bool withFade = false)
		{
			CgDataItem cgDataItem = Singleton<CGModule>.Instance.GetCgDataItem(cgId);
			if (cgDataItem != null)
			{
				Singleton<CGModule>.Instance.MarkCGIDFinish(cgDataItem.cgID);
				BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
				if (mainCanvas is MonoLoadingCanvas)
				{
					mainCanvas.PlayVideo(cgDataItem);
				}
				else if (mainCanvas is MonoInLevelUICanvas)
				{
					if (withFade)
					{
						(mainCanvas as MonoInLevelUICanvas).StartPlayVideo(cgDataItem);
					}
					else
					{
						mainCanvas.PlayVideo(cgDataItem);
					}
				}
				else
				{
					mainCanvas.PlayVideo(cgDataItem);
				}
				return true;
			}
			return false;
		}

		public string GetCurrentStageName()
		{
			return Singleton<StageManager>.Instance.GetStageTypeName();
		}

		public string GetCurrentStageFirstName()
		{
			return Singleton<StageManager>.Instance.GetStageTypeName().Split('_')[0];
		}

		public object GetPointsAroundSpecificPoint(string targetPoint, int num)
		{
			List<MonoSpawnPoint> list = new List<MonoSpawnPoint>(Singleton<StageManager>.Instance.GetStageEnv().spawnPoints);
			centerPoint = GetSpawnPoint(targetPoint);
			list.Sort(CompareByDistance);
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = list[i].name;
			}
			return array;
		}

		private int CompareByDistance(MonoSpawnPoint pointA, MonoSpawnPoint pointB)
		{
			if (Vector3.Distance(pointA.transform.localPosition, centerPoint.transform.localPosition) < Vector3.Distance(pointB.transform.localPosition, centerPoint.transform.localPosition))
			{
				return -1;
			}
			return 1;
		}

		public void RestartLuaLogic(string luaPath)
		{
			RemoveLevelDamageStastics();
			Singleton<LevelScoreManager>.Instance.luaFile = luaPath;
			InitAtAwake();
			LevelDesignStart();
		}

		public bool IsLevelDone()
		{
			return Singleton<LevelScoreManager>.Instance.IsLevelDone();
		}

		public int GetServerDayOfWeek()
		{
			return (int)TimeUtil.Now.DayOfWeek;
		}

		public void DoTutorialStep(int tutorialStepID, bool toPauseGame, float holdSeconds = 0f)
		{
			TutorialStepData tutorialStepDataByKey = TutorialStepDataReader.GetTutorialStepDataByKey(tutorialStepID);
			string targetUIPath = tutorialStepDataByKey.targetUIPath;
			Transform highlightTrans = null;
			if (targetUIPath != string.Empty)
			{
				BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
				highlightTrans = sceneCanvas.transform.FindChild(targetUIPath);
			}
			NewbieDialogContext newbieDialogContext = new NewbieDialogContext();
			newbieDialogContext.destroyByOthers = true;
			newbieDialogContext.delayInputTime = 0.5f;
			newbieDialogContext.disableMask = tutorialStepDataByKey.stepType == 2;
			newbieDialogContext.highlightTrans = highlightTrans;
			newbieDialogContext.highlightPath = targetUIPath;
			newbieDialogContext.bubblePosType = (NewbieDialogContext.BubblePosType)tutorialStepDataByKey.bubblePosType;
			newbieDialogContext.handIconPosType = (NewbieDialogContext.HandIconPosType)tutorialStepDataByKey.handIconPosType;
			newbieDialogContext.disableHighlightEffect = !tutorialStepDataByKey.playEffect;
			newbieDialogContext.guideDesc = LocalizationGeneralLogic.GetText(tutorialStepDataByKey.guideDesc);
			newbieDialogContext.delayShowTime = tutorialStepDataByKey.delayTime;
			NewbieDialogContext newbieDialogContext2 = newbieDialogContext;
			if (toPauseGame && !Singleton<LevelManager>.Instance.IsPaused())
			{
				Singleton<LevelManager>.Instance.SetPause(true);
			}
			DateTime pointerDownTime = TimeUtil.Now;
			newbieDialogContext2.pointerDownCallback = delegate
			{
				pointerDownTime = TimeUtil.Now;
			};
			newbieDialogContext2.pointerUpCallback = delegate
			{
				bool flag = TimeUtil.Now >= pointerDownTime.AddSeconds(holdSeconds);
				if (flag && toPauseGame && Singleton<LevelManager>.Instance.IsPaused())
				{
					Singleton<LevelManager>.Instance.SetPause(false);
				}
				return flag;
			};
			Singleton<MainUIManager>.Instance.ShowDialog(newbieDialogContext2);
		}

		public void ClearLevelCombo()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.ResetCombo();
		}

		public string GetLevelStasticsResult()
		{
			string empty = string.Empty;
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			LevelDamageStasticsPlugin plugin = levelActor.GetPlugin<LevelDamageStasticsPlugin>();
			empty = empty + "Time: " + Mathf.FloorToInt(plugin.stageTime);
			empty = empty + " DMG: " + Mathf.FloorToInt(plugin.avatarDamage);
			return empty + " MDMG: " + Mathf.FloorToInt(plugin.monsterDamage);
		}

		public void DebugShowLevelDisplayText(string text, LuaTable paramTable = null)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLevelDisplayText, text));
		}

		public void InitAtAwake()
		{
			_luaManualCoroutines = new List<Tuple<LuaThread, int>>();
			_ldEvtTriggers = new List<LDEvtTrigger>();
			_evtTriggers = new List<EvtTrigger>();
			_allLDEvents = new List<BaseLDEvent>();
			_luaState = new LuaState();
			_luaState["LevelDesign"] = this;
			string luaFile = Singleton<LevelScoreManager>.Instance.luaFile;
			object[] array = _luaState.DoString(Miscs.LoadTextFileToString(luaFile), luaFile, null);
			LuaTable luaTable = (LuaTable)array[0];
			_ldMain = (LuaFunction)luaTable["main"];
			state = LDState.Start;
		}

		public void InitAtStart()
		{
			Singleton<CameraManager>.Instance.GetMainCamera().gameObject.SetActive(true);
			Singleton<CameraManager>.Instance.GetInLevelUICamera().gameObject.SetActive(true);
		}

		public void Core()
		{
			if (state == LDState.End)
			{
				return;
			}
			for (int i = 0; i < _allLDEvents.Count; i++)
			{
				_allLDEvents[i].Core();
				if (_allLDEvents[i].isDone)
				{
					_allLDEvents.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < _ldEvtTriggers.Count; j++)
			{
				if (_ldEvtTriggers[j].ldEvent.isDone)
				{
					if (_ldEvtTriggers[j].isCallbackCoroutine)
					{
						MakeAndStartCoroutine(_ldEvtTriggers[j].callback);
					}
					else
					{
						_ldEvtTriggers[j].callback.Call();
					}
					if (_ldEvtTriggers.Count == 0)
					{
						break;
					}
					_ldEvtTriggers[j].ldEvent.Dispose();
					_ldEvtTriggers.RemoveAt(j);
					j--;
				}
			}
		}

		public void DispatchLevelDesignListenEvent(BaseEvent evt)
		{
			for (int i = 0; i < _allLDEvents.Count; i++)
			{
				_allLDEvents[i].OnEvent(evt);
			}
			Type type = evt.GetType();
			for (int j = 0; j < _evtTriggers.Count; j++)
			{
				if (_evtTriggers[j].evtType == type && !_evtTriggers[j].isCallbackCoroutine)
				{
					_evtTriggers[j].callback.Call(evt);
				}
			}
		}

		private LuaThread MakeAndStartCoroutine(LuaFunction luaFunc)
		{
			LuaThread luaThread = new LuaThread(_luaState, luaFunc);
			int item = Singleton<ApplicationManager>.Instance.StartCoroutineManual(LuaCoroutineIter(luaThread));
			_luaManualCoroutines.Add(Tuple.Create(luaThread, item));
			return luaThread;
		}

		public BaseLDEvent CreateLDEventFromTable(LuaTable luaTable)
		{
			object[] array = new object[luaTable.Values.Count];
			luaTable.Values.CopyTo(array, 0);
			List<object> list = new List<object>(array);
			Type type = Type.GetType("MoleMole." + (string)list[0]);
			list.RemoveAt(0);
			BaseLDEvent baseLDEvent = (BaseLDEvent)Activator.CreateInstance(type, list.ToArray());
			_allLDEvents.Add(baseLDEvent);
			return baseLDEvent;
		}

		private IEnumerator LuaCoroutineIter(LuaThread luaThread)
		{
			luaThread.Start();
			while (true)
			{
				if (state == LDState.Paused)
				{
					yield return null;
				}
				int ret = luaThread.Resume();
				if (ret != 1)
				{
					break;
				}
				LuaTable yieldRet = luaThread.translator.getTable(luaThread.L, -1);
				BaseLDEvent ldEvent = CreateLDEventFromTable(yieldRet);
				while (!ldEvent.isDone)
				{
					yield return null;
				}
			}
		}

		public void Destroy()
		{
			StopLevelDesign();
			_luaState.Dispose();
		}

		public void StopLevelDesign()
		{
			if (state != LDState.End)
			{
				state = LDState.End;
				ClearLuaCoroutines();
				ClearAllEventsAndTriggers();
			}
		}

		private void ClearLuaCoroutines()
		{
			for (int i = 0; i < _luaManualCoroutines.Count; i++)
			{
				Tuple<LuaThread, int> tuple = _luaManualCoroutines[i];
				tuple.Item1.Dispose();
				Singleton<ApplicationManager>.Instance.StopCoroutineManual(tuple.Item2);
			}
			_luaManualCoroutines.Clear();
		}

		private void ClearAllEventsAndTriggers()
		{
			_ldEvtTriggers.Clear();
			_evtTriggers.Clear();
			for (int i = 0; i < _allLDEvents.Count; i++)
			{
				_allLDEvents[i].Dispose();
			}
			_allLDEvents.Clear();
		}

		public void LevelDesignStart()
		{
			LevelActor levelActor = (LevelActor)Singleton<EventManager>.Instance.GetActor(562036737u);
			levelActor.levelMode = Singleton<LevelScoreManager>.Instance.levelMode;
			MakeAndStartCoroutine(_ldMain);
			state = LDState.Running;
		}

		public void SetPause(bool pause)
		{
			state = ((!pause) ? LDState.Running : LDState.Paused);
		}

		public void LevelDesignEndWithResult(EvtLevelState.LevelEndReason reason = EvtLevelState.LevelEndReason.EndUncertainReason, int endCgId = 0)
		{
			EndLevel(reason, endCgId);
		}

		private void EndLevel(EvtLevelState.LevelEndReason reason = EvtLevelState.LevelEndReason.EndUncertainReason, int endCgId = 0)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Invalid comparison between Unknown and I4
			StopLevelDesign();
			Singleton<EventManager>.Instance.DropEventsAndStop();
			Singleton<LevelManager>.Instance.SetPause(true);
			LevelActorCountDownPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>();
			bool flag = reason == EvtLevelState.LevelEndReason.EndWin;
			if (plugin != null)
			{
				flag &= Singleton<AvatarManager>.Instance.GetLocalAvatar().IsActive();
			}
			if (flag)
			{
				AddGalTouchGoodFeel();
			}
			if ((int)Singleton<LevelScoreManager>.Instance.LevelType == 4)
			{
				Singleton<MainUIManager>.Instance.ShowPage(new EndlessFloorEndPageContext(reason));
			}
			else
			{
				bool forceEnableWhenSetup = !Singleton<AvatarManager>.Instance.GetLocalAvatar().IsActive();
				Singleton<MainUIManager>.Instance.ShowPage(new LevelEndPageContext(reason, forceEnableWhenSetup, endCgId));
			}
			Singleton<WwiseAudioManager>.Instance.SetSwitch("Level_Result", (reason != EvtLevelState.LevelEndReason.EndWin) ? "Lose" : "Win");
			Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_Off");
			Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_End");
		}

		private void AddGalTouchGoodFeel()
		{
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			int i = 0;
			for (int count = allPlayerAvatars.Count; i < count; i++)
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(allPlayerAvatars[i].GetRuntimeID());
				if (actor != null)
				{
					int avatarID = actor.avatarDataItem.avatarID;
					int characterHeartLevel = Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel(avatarID);
					int amount = GalTouchData.QueryBattleGain(characterHeartLevel);
					Singleton<GalTouchModule>.Instance.IncreaseBattleGoodFeel(avatarID, amount);
				}
			}
		}

		public void CleanWhenStageChange()
		{
			ClearAllEventsAndTriggers();
		}
	}
}
