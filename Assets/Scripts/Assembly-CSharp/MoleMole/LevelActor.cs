using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class LevelActor : BaseAbilityActor
	{
		public enum Mode
		{
			Single = 0,
			Multi = 1,
			MultiRemote = 2,
			NetworkedMP = 3
		}

		public enum LevelState
		{
			LevelLoaded = 0,
			LevelStarted = 1,
			LevelRunning = 2,
			LevelEnded = 3,
			LevelTransiting = 4
		}

		public enum LevelMinSwapTimerState
		{
			Idle = 0,
			Running = 1
		}

		public enum ComboTimerState
		{
			Running = 0,
			Pause = 1
		}

		public enum AvatarSwapState
		{
			Idle = 0,
			WaitingForEvent = 1,
			Started = 2
		}

		private const float LEVEL_MIN_SWAP_TIME = 0.5f;

		private const float LEVEL_COMBO_WINDOW_TIME = 2.8f;

		private const int MAX_FRAME_INDEX = 20;

		public MonoLevelEntity levelEntity;

		public Mode levelMode;

		public Action comboTimeUPCallback;

		private float _levelMinSwapTimer;

		public LevelMinSwapTimerState _swapTimerState;

		public SafeInt32 levelCombo = 0;

		private SafeFloat _levelComboTimer = 0f;

		private ComboTimerState _comboTimerState;

		public float upLevelNatureBonusFactor = 1f;

		public float downLevelNatureBonusFactor = 1f;

		private bool _countAnimEventComboOnceInOneFrame = true;

		private int _frameIndex;

		private HashSet<KeyValuePair<int, string>> _frameHitSet;

		public Action<int, int> onLevelComboChanged;

		public LevelAntiCheatPlugin antiCheatPlugin;

		private LevelDamageStasticsPlugin _damagePlugin;

		private LevelMissionStatisticsPlugin _levelMissionStatisticsPlugin;

		private LevelAIPlugin _levelMonsterAIPlugin;

		private LevelDefendModePlugin _levelDefendModePlugin;

		public BaseLevelBuff[] levelBuffs;

		public LevelBuffWitchTime witchTimeLevelBuff;

		public LevelBuffStopWorld stopWorldLevelBuff;

		private uint _swapOutID;

		private uint _swapInID;

		private AvatarSwapState _avatarSwapState;

		private float _timeSlowTimer;

		private Action _timeSlowDoneCallback;

		public LevelState levelState { get; private set; }

		public void SuddenLevelStart()
		{
			levelState = LevelState.LevelStarted;
			Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(new string[1] { "BK_InLevel_Common" });
		}

		public void SuddenLevelEnd()
		{
			levelState = LevelState.LevelEnded;
			Singleton<WwiseAudioManager>.Instance.ClearManualPrepareBank();
		}

		public override void OnRemoval()
		{
			base.OnRemoval();
			onLevelComboChanged = null;
		}

		public override void Init(BaseMonoEntity entity)
		{
			levelEntity = (MonoLevelEntity)entity;
			commonConfig = levelEntity.commonConfig;
			base.Init(entity);
			runtimeID = 562036737u;
			levelState = LevelState.LevelLoaded;
			_damagePlugin = new LevelDamageStasticsPlugin(this);
			if (Singleton<LevelScoreManager>.Instance.collectAntiCheatData)
			{
				antiCheatPlugin = new LevelAntiCheatPlugin(_damagePlugin);
				AddPlugin(antiCheatPlugin);
			}
			_frameHitSet = new HashSet<KeyValuePair<int, string>>();
			_frameIndex = 0;
			LevelChallengeHelperPlugin plugin = new LevelChallengeHelperPlugin(this);
			AddPlugin(plugin);
			int levelId = Singleton<LevelScoreManager>.Instance.LevelId;
			if (levelId != 0)
			{
				int count = Singleton<LevelTutorialModule>.Instance.GetUnFinishedTutorialIDList(levelId).Count;
				if (count > 0)
				{
					LevelTutorialHelperPlugin plugin2 = new LevelTutorialHelperPlugin(this);
					AddPlugin(plugin2);
				}
			}
			_levelMissionStatisticsPlugin = new LevelMissionStatisticsPlugin(this);
			AddPlugin(_levelMissionStatisticsPlugin);
			_levelMonsterAIPlugin = new LevelAIPlugin(this);
			AddPlugin(_levelMonsterAIPlugin);
			InitAdditionalLevelActorPlugins();
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackLanded>(runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtReviveAvatar>(runtimeID);
			AvatarManager instance = Singleton<AvatarManager>.Instance;
			instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>)Delegate.Combine(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(OnLocalAvatarChanged));
		}

		public void SetLevelDefendModePluginStart(int targetValue)
		{
			if (_levelDefendModePlugin != null && HasPlugin<LevelDefendModePlugin>())
			{
				_levelDefendModePlugin.Reset(targetValue);
			}
			else
			{
				_levelDefendModePlugin = new LevelDefendModePlugin(this, targetValue);
				AddPlugin(_levelDefendModePlugin);
			}
			_levelDefendModePlugin.SetActive(true);
		}

		public void SetLevelDefendModePluginStart()
		{
			if (_levelDefendModePlugin != null && HasPlugin<LevelDefendModePlugin>())
			{
				_levelDefendModePlugin.Reset();
			}
			else
			{
				_levelDefendModePlugin = new LevelDefendModePlugin(this);
				AddPlugin(_levelDefendModePlugin);
			}
			_levelDefendModePlugin.SetActive(true);
		}

		public void SetLevelDefendModePluginReset(int targetValue)
		{
			if (_levelDefendModePlugin != null && HasPlugin<LevelDefendModePlugin>())
			{
				_levelDefendModePlugin.Reset(targetValue);
			}
		}

		public void SetLevelDefendModePluginStop()
		{
			if (_levelDefendModePlugin != null && HasPlugin<LevelDefendModePlugin>())
			{
				_levelDefendModePlugin.Stop();
				RemovePlugin(_levelDefendModePlugin);
				_levelDefendModePlugin = null;
			}
		}

		public int GetLevelDefendModeMonsterEnterAmount()
		{
			if (_levelDefendModePlugin != null && HasPlugin<LevelDefendModePlugin>())
			{
				return _levelDefendModePlugin.MonsterEnterAmount;
			}
			return 0;
		}

		public int GetLevelDefendModeMonsterKillAmount()
		{
			if (_levelDefendModePlugin != null && HasPlugin<LevelDefendModePlugin>())
			{
				return _levelDefendModePlugin.MonsterKillAmount;
			}
			return 0;
		}

		public void AddTriggerFieldInDefendMode(TriggerFieldActor triggerFieldActor)
		{
			if (_levelDefendModePlugin != null && HasPlugin<LevelDefendModePlugin>())
			{
				_levelDefendModePlugin.AddTriggerFieldActor(triggerFieldActor);
			}
		}

		public void ControlLevelDamageStastics(DamageStastcisControlType type)
		{
			if (HasPlugin<LevelDamageStasticsPlugin>())
			{
				_damagePlugin.ControlDamageStastics(type);
			}
		}

		public void SetupLevelDamageStastics()
		{
			AddPlugin(_damagePlugin);
		}

		public void RemoveLevelDamageStastics()
		{
			RemovePlugin(_damagePlugin);
		}

		public void StartLevelBuff(BaseLevelBuff buff, float duration, bool allowRefresh, bool enteringTimeSlow, LevelBuffSide side, uint ownerRuntimeID, bool notStartEffect)
		{
			if (buff.isActive)
			{
				if (buff == witchTimeLevelBuff)
				{
					witchTimeLevelBuff.Refresh(duration, side, ownerID, enteringTimeSlow, allowRefresh, notStartEffect);
				}
				else if (buff != stopWorldLevelBuff)
				{
				}
			}
			else if (buff == witchTimeLevelBuff)
			{
				witchTimeLevelBuff.Setup(enteringTimeSlow, duration, side, notStartEffect);
				AddPlugin(buff);
			}
			else if (buff == stopWorldLevelBuff)
			{
				stopWorldLevelBuff.Setup(enteringTimeSlow, duration, ownerRuntimeID);
				AddPlugin(stopWorldLevelBuff);
			}
			buff.isActive = true;
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(buff.levelBuffType, LevelBuffState.Start, side, ownerRuntimeID));
		}

		public bool IsLevelBuffActive(LevelBuffType levelBuffType)
		{
			bool result = false;
			for (int i = 0; i < levelBuffs.Length; i++)
			{
				if (levelBuffs[i].levelBuffType == levelBuffType)
				{
					result = levelBuffs[i].isActive;
					break;
				}
			}
			return result;
		}

		public virtual void StopLevelBuff(BaseLevelBuff buff)
		{
			RemovePlugin(buff);
			buff.isActive = false;
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(buff.levelBuffType, LevelBuffState.Stop, buff.levelBuffSide, runtimeID));
		}

		public override bool OnEventWithPlugins(BaseEvent evt)
		{
			if (evt is EvtLevelState)
			{
				return OnLevelState((EvtLevelState)evt);
			}
			if (evt is EvtAvatarCreated)
			{
				return OnCreateAvatar((EvtAvatarCreated)evt);
			}
			if (evt is EvtMonsterCreated)
			{
				return OnCreateMonster((EvtMonsterCreated)evt);
			}
			if (evt is EvtStageCreated)
			{
				return OnCreateStage((EvtStageCreated)evt);
			}
			if (evt is EvtStageReady)
			{
				return OnStageReady((EvtStageReady)evt);
			}
			if (evt is EvtStageTriggerCreated)
			{
				return OnStageTriggerCreated((EvtStageTriggerCreated)evt);
			}
			if (evt is EvtDynamicObjectCreated)
			{
				return OnDynamicObjectCreated((EvtDynamicObjectCreated)evt);
			}
			if (evt is EvtHittingOther)
			{
				return OnHittingOther((EvtHittingOther)evt);
			}
			return false;
		}

		public override bool OnEventResolves(BaseEvent evt)
		{
			if (evt is EvtHittingOther)
			{
				return OnHittingOtherResolve((EvtHittingOther)evt);
			}
			return false;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			bool flag = base.ListenEvent(evt);
			if (evt is EvtKilled)
			{
				flag |= ListenKill((EvtKilled)evt);
			}
			else if (evt is EvtBeingHit)
			{
				flag |= ListenBeingHit((EvtBeingHit)evt);
			}
			else if (evt is EvtAttackStart)
			{
				flag |= ListenAttackStart((EvtAttackStart)evt);
			}
			else if (evt is EvtAttackLanded)
			{
				flag |= ListenAttackLanded((EvtAttackLanded)evt);
			}
			else
			{
				if (evt is EvtReviveAvatar)
				{
					return ListenReviveAvatar((EvtReviveAvatar)evt);
				}
				if (evt is EvtAvatarSwapOutStart)
				{
					return ListenSwapOutAvatarStart((EvtAvatarSwapOutStart)evt);
				}
			}
			return flag;
		}

		public bool OnCreateAvatar(EvtAvatarCreated evt)
		{
			BaseMonoAvatar baseMonoAvatar = (BaseMonoAvatar)Singleton<EventManager>.Instance.GetEntity(evt.avatarID);
			if (!Singleton<AvatarManager>.Instance.IsPlayerAvatar(baseMonoAvatar))
			{
				if (Singleton<AvatarManager>.Instance.IsHelperAvatar(evt.avatarID))
				{
					baseMonoAvatar.gameObject.SetActive(false);
				}
				return false;
			}
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.avatarID))
			{
				if (false)
				{
					baseMonoAvatar.PlayState("Story");
				}
				else
				{
					baseMonoAvatar.TriggerAppear();
					baseMonoAvatar.RefreshController();
				}
			}
			else if (levelMode == Mode.Single)
			{
				baseMonoAvatar.gameObject.SetActive(false);
			}
			else
			{
				baseMonoAvatar.TriggerAppear();
				baseMonoAvatar.RefreshController();
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnAvatarCreate, evt.avatarID));
			return true;
		}

		public bool OnCreateMonster(EvtMonsterCreated evt)
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.AddHintArrow(evt.monsterID);
			return true;
		}

		private bool OnStageTriggerCreated(EvtStageTriggerCreated evt)
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.AddHintArrow(evt.triggerRuntimeID);
			return true;
		}

		private bool OnDynamicObjectCreated(EvtDynamicObjectCreated evt)
		{
			if (evt.dynamicType == BaseMonoDynamicObject.DynamicType.Barrier && levelMode != Mode.MultiRemote)
			{
				DragAllAvatarsNearLocalAvatar();
			}
			return true;
		}

		private void OnLocalAvatarChanged(BaseMonoAvatar from, BaseMonoAvatar to)
		{
			Singleton<WwiseAudioManager>.Instance.SetListenerFollowing(to.transform, new Vector3(0f, 2f, 0f));
			Singleton<CameraManager>.Instance.GetMainCamera().SetupFollowAvatar(to.GetRuntimeID());
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().OnUpdateLocalAvatar(to.GetRuntimeID(), from.GetRuntimeID());
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().OnUpdateLocalAvatarAbilityDisplay(to.GetRuntimeID(), from.GetRuntimeID());
			to.RefreshController();
		}

		private void DragAllAvatarsNearLocalAvatar()
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			BaseMonoAvatar helperAvatar = Singleton<AvatarManager>.Instance.GetHelperAvatar();
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			List<BaseMonoAvatar> list = new List<BaseMonoAvatar>();
			if (helperAvatar != null)
			{
				list.Add(helperAvatar);
			}
			foreach (BaseMonoAvatar item in allPlayerAvatars)
			{
				if (item != localAvatar)
				{
					list.Add(item);
				}
			}
			foreach (BaseMonoAvatar item2 in list)
			{
				if (Physics.Linecast(localAvatar.transform.position, item2.transform.position, 1 << InLevelData.OBSTACLE_COLLIDER_LAYER))
				{
					item2.transform.position = localAvatar.transform.position;
				}
			}
		}

		private bool ListenSwapOutAvatarStart(EvtAvatarSwapOutStart evt)
		{
			if (levelMode == Mode.Single && evt.targetID == _swapOutID && _avatarSwapState == AvatarSwapState.WaitingForEvent)
			{
				SwapLocalAvatar(_swapOutID, _swapInID);
			}
			return true;
		}

		public bool ListenReviveAvatar(EvtReviveAvatar evt)
		{
			AvatarActor avatarActor = (AvatarActor)Singleton<EventManager>.Instance.GetActor(evt.avatarID);
			Vector3 revivePosition;
			if (evt.isRevivePosAssigned)
			{
				revivePosition = evt.revivePosition;
			}
			else
			{
				BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				revivePosition = ((!(localAvatar != null) || !localAvatar.IsActive()) ? evt.revivePosition : localAvatar.XZPosition);
			}
			avatarActor.Revive(revivePosition);
			Singleton<CameraManager>.Instance.GetMainCamera().SetFailPostFX(false);
			abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig("Level_AvatarReviveInvincible"), avatarActor.runtimeID, 2f);
			return true;
		}

		private string GetFootStepNameFromStageTypeName(string stageTypeName)
		{
			string result = "Tile";
			if (stageTypeName.IndexOf("Spaceship") != -1)
			{
				result = "Metal";
			}
			else if (stageTypeName.IndexOf("ME") != -1)
			{
				result = "Concrete";
			}
			else if (stageTypeName.IndexOf("NZ_Town") != -1)
			{
				result = "Grass";
			}
			return result;
		}

		public bool OnCreateStage(EvtStageCreated evt)
		{
			bool sendStageReady;
			HandleAvatarCreationForStageCreation(evt, out sendStageReady);
			if (evt.isBorn)
			{
				Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(new string[1] { "BK_InLevel_Common" });
			}
			string footStepNameFromStageTypeName = GetFootStepNameFromStageTypeName(Singleton<StageManager>.Instance.GetStageTypeName());
			if (footStepNameFromStageTypeName != null)
			{
				List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
				int i = 0;
				for (int count = allAvatars.Count; i < count; i++)
				{
					Singleton<WwiseAudioManager>.Instance.SetSwitch("Terrain_Type", footStepNameFromStageTypeName, allAvatars[i].gameObject);
				}
			}
			Singleton<DetourManager>.Instance.LoadNavMeshRelatedLevel(Singleton<StageManager>.Instance.GetStageTypeName());
			if (sendStageReady)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtStageReady
				{
					isBorn = evt.isBorn
				});
			}
			return true;
		}

		protected virtual void HandleAvatarCreationForStageCreation(EvtStageCreated evt, out bool sendStageReady)
		{
			List<MonoSpawnPoint> list = new List<MonoSpawnPoint>();
			foreach (string avatarSpawnName in evt.avatarSpawnNameList)
			{
				int namedSpawnPointIx = Singleton<StageManager>.Instance.GetStageEnv().GetNamedSpawnPointIx(avatarSpawnName);
				list.Add(Singleton<StageManager>.Instance.GetStageEnv().spawnPoints[namedSpawnPointIx]);
			}
			if (evt.isBorn)
			{
				Singleton<AvatarManager>.Instance.CreateTeamAvatars();
				Singleton<AvatarManager>.Instance.InitAvatarsPos(list);
				Singleton<MonsterManager>.Instance.InitMonstersPos(evt.offset);
			}
			else
			{
				Singleton<AvatarManager>.Instance.InitAvatarsPos(list);
				Singleton<MonsterManager>.Instance.InitMonstersPos(evt.offset);
			}
			sendStageReady = true;
		}

		private bool OnStageReady(EvtStageReady evt)
		{
			if (levelState == LevelState.LevelStarted)
			{
				levelState = LevelState.LevelRunning;
			}
			if (evt.isBorn)
			{
				Singleton<StageManager>.Instance.ApplyActiveStageEffectSettingAndStartCheckingForChange();
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.PostStageReady));
			return true;
		}

		public bool OnLevelState(EvtLevelState evt)
		{
			if (evt.state == EvtLevelState.State.Start)
			{
				if (levelState == LevelState.LevelLoaded)
				{
					levelState = LevelState.LevelStarted;
				}
				Singleton<LevelDesignManager>.Instance.LevelDesignStart();
			}
			else if (evt.state == EvtLevelState.State.EndWin || evt.state == EvtLevelState.State.EndLose)
			{
				levelState = LevelState.LevelEnded;
				Singleton<LevelDesignManager>.Instance.LevelDesignEndWithResult(evt.levelEndReason, evt.cgId);
				Singleton<WwiseAudioManager>.Instance.ClearManualPrepareBank();
			}
			else if (evt.state == EvtLevelState.State.EnterTransition)
			{
				levelState = LevelState.LevelTransiting;
			}
			else if (evt.state == EvtLevelState.State.ExitTransition)
			{
				levelState = LevelState.LevelRunning;
			}
			else if (evt.state == EvtLevelState.State.PostStageReady)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PostStageReady));
			}
			return true;
		}

		private bool ListenKill(EvtKilled evt)
		{
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
			if (num == 3)
			{
				if (!Singleton<AvatarManager>.Instance.IsPlayerAvatar(evt.targetID))
				{
					return false;
				}
				BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars().Find((BaseMonoAvatar avatar) => avatar.IsAlive());
				if (baseMonoAvatar == null)
				{
					Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(runtimeID);
				}
			}
			Singleton<DetourManager>.Instance.RemoveDetourElement(evt.targetID);
			return true;
		}

		public bool ListenBeingHit(EvtBeingHit evt)
		{
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
			if (num == 4 || num == 3 || num == 7)
			{
				if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Mute)
				{
					return true;
				}
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowDamegeText, evt));
			}
			return true;
		}

		public void TriggerSwapLocalAvatar(uint swapOutID, uint swapInID, bool force)
		{
			if ((!force && _swapTimerState == LevelMinSwapTimerState.Running) || _avatarSwapState == AvatarSwapState.Started)
			{
				return;
			}
			if (levelMode == Mode.Single)
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(swapOutID);
				if (actor.AllowOtherSwitchIn)
				{
					actor.avatar.TriggerSwitchOut(BaseMonoAvatar.AvatarSwapOutType.Delayed);
					SwapLocalAvatar(swapOutID, swapInID);
					return;
				}
				_swapOutID = swapOutID;
				_swapInID = swapInID;
				_avatarSwapState = AvatarSwapState.WaitingForEvent;
				actor.avatar.TriggerSwitchOut(force ? BaseMonoAvatar.AvatarSwapOutType.Force : BaseMonoAvatar.AvatarSwapOutType.Normal);
			}
			else if (levelMode == Mode.Multi || levelMode == Mode.MultiRemote)
			{
				SwapLocalAvatar(swapOutID, swapInID);
			}
		}

		public void SwapLocalAvatar(uint swapOutID, uint swapInID)
		{
			_avatarSwapState = AvatarSwapState.Started;
			BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(swapOutID);
			BaseMonoAvatar avatarByRuntimeID2 = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(swapInID);
			if (levelMode == Mode.Single)
			{
				SingleModeSwapTo(avatarByRuntimeID.XZPosition, avatarByRuntimeID.FaceDirection, avatarByRuntimeID2);
			}
			else if (levelMode == Mode.Multi || levelMode == Mode.MultiRemote)
			{
				MultiModeSwap(avatarByRuntimeID, avatarByRuntimeID2);
			}
		}

		public void SingleModeSwapTo(Vector3 xzPosition, Vector3 forward, BaseMonoAvatar swapInAvatar)
		{
			_swapTimerState = LevelMinSwapTimerState.Running;
			_levelMinSwapTimer = 0.5f;
			if (swapInAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.OffStage)
			{
				swapInAvatar.transform.position = xzPosition;
				swapInAvatar.SteerFaceDirectionTo(forward);
				swapInAvatar.gameObject.SetActive(true);
				swapInAvatar.TriggerSwitchIn();
			}
			else if (swapInAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.SwitchingOut)
			{
				swapInAvatar.TriggerSwitchIn();
			}
			else if (swapInAvatar.IsSwitchOutTriggerSet())
			{
				swapInAvatar.ResetTriggerSwitchOut();
			}
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_SwitchRole", swapInAvatar.XZPosition, swapInAvatar.FaceDirection, Vector3.one, levelEntity);
			if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(swapInAvatar.GetRuntimeID()))
			{
				Singleton<AvatarManager>.Instance.SetLocalAvatar(swapInAvatar.GetRuntimeID());
			}
		}

		private void MultiModeSwap(BaseMonoAvatar swapOutAvatar, BaseMonoAvatar swapInAvatar)
		{
			_swapTimerState = LevelMinSwapTimerState.Running;
			_levelMinSwapTimer = 0.5f;
			Singleton<AvatarManager>.Instance.SetLocalAvatar(swapInAvatar.GetRuntimeID());
			Singleton<CameraManager>.Instance.GetMainCamera().SuddenSwitchFollowAvatar(swapInAvatar.GetRuntimeID(), true);
			if (swapOutAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.OnStage)
			{
				swapOutAvatar.RefreshController();
				swapOutAvatar.OrderMove = false;
			}
		}

		private bool ListenAttackStart(EvtAttackStart evt)
		{
			return false;
		}

		private bool ListenAttackLanded(EvtAttackLanded evt)
		{
			KeyValuePair<int, string> item = new KeyValuePair<int, string>(_frameIndex, evt.animEventID);
			if (_countAnimEventComboOnceInOneFrame && _frameHitSet.Contains(item))
			{
				return true;
			}
			if (evt.attackResult.isAnimEventAttack && evt.attackResult.isInComboCount && !evt.attackResult.rejected && Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3 && Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID))
			{
				_frameHitSet.Add(item);
				ResetComboTimer();
				DelegateUtils.UpdateField(ref levelCombo, (int)levelCombo + 1, onLevelComboChanged);
				if ((int)Singleton<LevelScoreManager>.Instance.maxComboNum < (int)levelCombo)
				{
					Singleton<LevelScoreManager>.Instance.maxComboNum = levelCombo;
				}
			}
			return true;
		}

		public void TimeSlow(float duration)
		{
			TimeSlow(duration, 0.05f, null);
		}

		public void ResetCombo()
		{
			ResetComboTimer();
			DelegateUtils.UpdateField(ref levelCombo, 0, onLevelComboChanged);
		}

		public void TimeSlow(float duration, float slowRatio, Action doneCallback)
		{
			if (_timeSlowTimer > 0f && _timeSlowDoneCallback != null)
			{
				_timeSlowDoneCallback();
				_timeSlowDoneCallback = null;
			}
			_timeSlowTimer = duration;
			_timeSlowDoneCallback = doneCallback;
			Time.timeScale = slowRatio;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
			Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSlow_Start");
		}

		public void SetLevelComboTimerState(ComboTimerState state)
		{
			_comboTimerState = state;
		}

		public override void Core()
		{
			_frameIndex++;
			if (_frameIndex > 20)
			{
				_frameHitSet.Clear();
				_frameIndex = 0;
			}
			base.Core();
			if (_timeSlowTimer > 0f)
			{
				_timeSlowTimer -= Time.unscaledDeltaTime;
				if (_timeSlowTimer <= 0f)
				{
					Time.timeScale = 1f;
					Time.fixedDeltaTime = 0.02f * Time.timeScale;
					Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSlow_End");
					if (_timeSlowDoneCallback != null)
					{
						_timeSlowDoneCallback();
						_timeSlowDoneCallback = null;
					}
				}
			}
			if (_swapTimerState == LevelMinSwapTimerState.Running)
			{
				_levelMinSwapTimer -= levelEntity.TimeScale * Time.deltaTime;
				if (_levelMinSwapTimer < 0f)
				{
					_swapTimerState = LevelMinSwapTimerState.Idle;
					_levelMinSwapTimer = 0f;
				}
			}
			if (_avatarSwapState == AvatarSwapState.Started)
			{
				_avatarSwapState = AvatarSwapState.Idle;
			}
			if (!((float)_levelComboTimer > 0f))
			{
				return;
			}
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (localAvatar != null)
			{
				string currentSkillID = localAvatar.CurrentSkillID;
				if (!string.IsNullOrEmpty(currentSkillID) && localAvatar.config.Skills.ContainsKey(currentSkillID))
				{
					float comboTimerPauseNormalizedTimeStart = localAvatar.config.Skills[currentSkillID].ComboTimerPauseNormalizedTimeStart;
					float comboTimerPauseNormalizedTimeStop = localAvatar.config.Skills[currentSkillID].ComboTimerPauseNormalizedTimeStop;
					float currentNormalizedTime = localAvatar.GetCurrentNormalizedTime();
					if (comboTimerPauseNormalizedTimeStart < comboTimerPauseNormalizedTimeStop)
					{
						if (currentNormalizedTime > comboTimerPauseNormalizedTimeStart && currentNormalizedTime < comboTimerPauseNormalizedTimeStop)
						{
							_comboTimerState = ComboTimerState.Pause;
						}
						else if (currentNormalizedTime > comboTimerPauseNormalizedTimeStop)
						{
							_comboTimerState = ComboTimerState.Running;
						}
					}
				}
			}
			if (_comboTimerState == ComboTimerState.Running)
			{
				_levelComboTimer = (float)_levelComboTimer - levelEntity.TimeScale * Time.deltaTime;
			}
			if ((float)_levelComboTimer < 0f)
			{
				if (comboTimeUPCallback != null)
				{
					comboTimeUPCallback();
				}
				else
				{
					DelegateUtils.UpdateField(ref levelCombo, 0, onLevelComboChanged);
				}
			}
		}

		public void SetAvatarBeAttackMaxNum(int maxNum)
		{
			_levelMonsterAIPlugin.SetAvatarBeAttackMaxNum(maxNum);
		}

		public void UntargetEntity(BaseMonoEntity target)
		{
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
			for (int i = 0; i < allAvatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = allAvatars[i];
				BaseMonoEntity baseMonoEntity = baseMonoAvatar.AttackTarget;
				if (baseMonoEntity is MonoBodyPartEntity)
				{
					baseMonoEntity = ((MonoBodyPartEntity)baseMonoEntity).owner;
				}
				if (baseMonoAvatar != null && baseMonoAvatar.IsActive() && baseMonoEntity == target)
				{
					baseMonoAvatar.SetAttackTarget(null);
				}
			}
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			for (int j = 0; j < allMonsters.Count; j++)
			{
				BaseMonoMonster baseMonoMonster = allMonsters[j];
				if (baseMonoMonster != null && baseMonoMonster.IsActive() && baseMonoMonster.AttackTarget == target)
				{
					baseMonoMonster.SetAttackTarget(null);
				}
			}
		}

		public void ResetComboTimer()
		{
			_levelComboTimer = (2.8f + GetProperty("Actor_ComboTimerDelta")) * GetProperty("Actor_ComboTimerRatio");
		}

		public override void HealHP(float amount)
		{
		}

		public override void HealSP(float amount)
		{
		}

		protected override void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
		{
		}

		protected override void OnAbilityStateRemove(AbilityState state)
		{
		}

		public override void ForceKill(uint killerID, KillEffect killEffect)
		{
		}

		public void ReviveAvatarByID(uint runtimeID, Vector3 revivePosition)
		{
			HideDeadBody();
			BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
			Singleton<EventManager>.Instance.FireEvent(new EvtReviveAvatar(avatarByRuntimeID.GetRuntimeID(), true, revivePosition));
		}

		private void HideDeadBody()
		{
			foreach (BaseMonoAvatar allPlayerAvatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
			{
				if (!allPlayerAvatar.IsAlive())
				{
					allPlayerAvatar.gameObject.SetActive(false);
				}
			}
		}

		private bool OnHittingOther(EvtHittingOther evt)
		{
			return true;
		}

		private bool OnHittingOtherResolve(EvtHittingOther evt)
		{
			evt.Resolve();
			Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.toID, runtimeID, evt.animEventID, evt.attackData));
			return true;
		}

		protected virtual void InitAdditionalLevelActorPlugins()
		{
			witchTimeLevelBuff = new LevelBuffWitchTime(this);
			stopWorldLevelBuff = new LevelBuffStopWorld(this);
			levelBuffs = new BaseLevelBuff[2];
			levelBuffs[0] = witchTimeLevelBuff;
			levelBuffs[1] = stopWorldLevelBuff;
			AddPlugin(new LevelAbilityHelperPlugin(this));
		}
	}
}
