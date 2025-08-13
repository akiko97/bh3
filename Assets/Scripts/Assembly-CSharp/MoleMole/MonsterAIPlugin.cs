using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using FullInspector;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonsterAIPlugin : BaseActorPlugin
	{
		private enum WarningFieldState
		{
			None = 0,
			Outside = 1,
			Inside = 2
		}

		public const int MAX_THREAT_LEVEL = 100;

		public const int BEING_HIT_TREAT_DELTA = 1;

		public const int SKILL_START_THREAT_DELTA = -5;

		public const int SKILL_START_CUR_TARGET_THREAT_DELTA = -15;

		public const int INIT_NEAREST_AVATAR_THREAT = 20;

		public const int CHANGE_TARGET_THREAT_DIFF = 10;

		public const AbilityState SHOULD_INTERRUPT_DEBUFF = AbilityState.Stun | AbilityState.Paralyze | AbilityState.Frozen;

		protected BehaviorTree _aiTree;

		protected MonsterActor _owner;

		private LevelActor _levelActor;

		[ShowInInspector]
		private AbilityTriggerField _warningFieldActor;

		[ShowInInspector]
		private float _warningRadius = 1f;

		[ShowInInspector]
		private float _escapeRadius = 1f;

		[ShowInInspector]
		private WarningFieldState _warningFieldState;

		public SortedList<uint, int> _threatTable;

		public MonsterAIPlugin(MonsterActor owner)
		{
			_owner = owner;
			_aiTree = _owner.entity.GetComponent<BehaviorTree>();
			_levelActor = Singleton<LevelManager>.Instance.levelActor;
			_threatTable = new SortedList<uint, int>();
		}

		public override void OnAdded()
		{
			if (_levelActor.levelState == LevelActor.LevelState.LevelRunning)
			{
				Preparation();
			}
			else
			{
				Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(_owner.runtimeID);
			}
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_owner.runtimeID);
			if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
			{
				AvatarManager instance = Singleton<AvatarManager>.Instance;
				instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>)Delegate.Combine(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(OnLocalAvatarChanged));
			}
			MonsterActor owner = _owner;
			owner.onAbilityStateAdd = (Action<AbilityState, bool>)Delegate.Combine(owner.onAbilityStateAdd, new Action<AbilityState, bool>(OnAbilityStateAdd));
			MonsterActor owner2 = _owner;
			owner2.onAbilityStateRemove = (Action<AbilityState>)Delegate.Combine(owner2.onAbilityStateRemove, new Action<AbilityState>(OnAbilityStateRemove));
			BaseMonoMonster monster = _owner.monster;
			monster.onHitStateChanged = (Action<BaseMonoMonster, bool, bool>)Delegate.Combine(monster.onHitStateChanged, new Action<BaseMonoMonster, bool, bool>(OnHitStateChanged));
		}

		private void OnLocalAvatarChanged(BaseMonoAvatar from, BaseMonoAvatar to)
		{
			if (_owner.monster.AttackTarget == from)
			{
				_owner.monster.SetAttackTarget(to);
			}
		}

		private void Preparation()
		{
		}

		public override void OnRemoved()
		{
			if (_warningFieldState > WarningFieldState.None)
			{
				Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(_owner.runtimeID);
				Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(_owner.runtimeID);
				_warningFieldActor.Kill();
				_warningFieldState = WarningFieldState.None;
			}
			if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
			{
				AvatarManager instance = Singleton<AvatarManager>.Instance;
				instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>)Delegate.Remove(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(OnLocalAvatarChanged));
			}
			MonsterActor owner = _owner;
			owner.onAbilityStateAdd = (Action<AbilityState, bool>)Delegate.Remove(owner.onAbilityStateAdd, new Action<AbilityState, bool>(OnAbilityStateAdd));
			MonsterActor owner2 = _owner;
			owner2.onAbilityStateRemove = (Action<AbilityState>)Delegate.Remove(owner2.onAbilityStateRemove, new Action<AbilityState>(OnAbilityStateRemove));
			BaseMonoMonster monster = _owner.monster;
			monster.onHitStateChanged = (Action<BaseMonoMonster, bool, bool>)Delegate.Remove(monster.onHitStateChanged, new Action<BaseMonoMonster, bool, bool>(OnHitStateChanged));
		}

		public void InitWarningField(float warningRadius, float escapeRadius)
		{
			if (_warningFieldState == WarningFieldState.None && warningRadius > 0f && escapeRadius >= warningRadius)
			{
				_warningRadius = warningRadius;
				_escapeRadius = escapeRadius;
				_warningFieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(_owner.monster.transform.position, _owner.monster.transform.forward, _owner, _warningRadius, MixinTargetting.Enemy, Singleton<DynamicObjectManager>.Instance.GetNextNonSyncedDynamicObjectRuntimeID(), true);
				_warningFieldState = WarningFieldState.Outside;
				_owner.monster.OrderMove = false;
				_owner.monster.ClearHitTrigger();
				_owner.monster.ClearAttackTriggers();
				_owner.monster.SetUseAIController(false);
				Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(_owner.runtimeID);
				Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(_owner.runtimeID);
			}
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtAttackStart)
			{
				return OnAttackStart((EvtAttackStart)evt);
			}
			return false;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtStageReady)
			{
				return ListenStageReady((EvtStageReady)evt);
			}
			if (evt is EvtKilled)
			{
				return ListenKilled((EvtKilled)evt);
			}
			if (evt is EvtFieldEnter)
			{
				return OnFieldEnter((EvtFieldEnter)evt);
			}
			if (evt is EvtFieldExit)
			{
				return OnFieldExit((EvtFieldExit)evt);
			}
			return false;
		}

		private bool OnFieldEnter(EvtFieldEnter evt)
		{
			if (_warningFieldActor == null || _warningFieldActor.runtimeID != evt.targetID)
			{
				return false;
			}
			if (_warningFieldState != WarningFieldState.Outside)
			{
				return false;
			}
			_owner.monster.SetUseAIController(true);
			_warningFieldActor.triggerField.transform.localScale = Vector3.one * _escapeRadius;
			_warningFieldState = WarningFieldState.Inside;
			return true;
		}

		private bool OnFieldExit(EvtFieldExit evt)
		{
			if (_warningFieldActor == null || _warningFieldActor.runtimeID != evt.targetID)
			{
				return false;
			}
			if (_warningFieldState != WarningFieldState.Inside)
			{
				return false;
			}
			_owner.monster.OrderMove = false;
			_owner.monster.ClearHitTrigger();
			_owner.monster.ClearAttackTriggers();
			_owner.monster.SetUseAIController(false);
			_warningFieldActor.triggerField.transform.localScale = Vector3.one * _warningRadius;
			_warningFieldState = WarningFieldState.Outside;
			return true;
		}

		private bool OnAttackStart(EvtAttackStart evt)
		{
			for (int i = 0; i < _threatTable.Count; i++)
			{
				uint key = _threatTable.Keys[i];
				_threatTable[key] = Mathf.Clamp(_threatTable[key] + -5, 0, 100);
			}
			if (_owner.monster.AttackTarget != null && _threatTable.ContainsKey(_owner.monster.AttackTarget.GetRuntimeID()))
			{
				uint runtimeID = _owner.monster.AttackTarget.GetRuntimeID();
				_threatTable[runtimeID] = Mathf.Clamp(_threatTable[runtimeID] + -15, 0, 100);
			}
			ThreatTableChanged();
			return false;
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.sourceID) == 3)
			{
				if (_threatTable.ContainsKey(evt.sourceID))
				{
					_threatTable[evt.sourceID] = Mathf.Clamp(_threatTable[evt.sourceID] + 1, 0, 100);
				}
				else
				{
					_threatTable[evt.sourceID] = 1;
				}
				ThreatTableChanged();
				return true;
			}
			return false;
		}

		public void InitNearestAvatarThreat(uint avatarID)
		{
			if (_threatTable.ContainsKey(avatarID))
			{
				_threatTable[avatarID] = 20;
			}
			else
			{
				_threatTable.Add(avatarID, 20);
			}
			ThreatTableChanged();
		}

		private void ThreatTableChanged()
		{
			if (_owner.monster.AttackTarget != null && _owner.monster.AttackTarget.IsActive())
			{
				uint runtimeID = _owner.monster.AttackTarget.GetRuntimeID();
				uint num = RetargetByThreat(runtimeID);
				if (runtimeID != num)
				{
					_aiTree.SendEvent("AIThreatRetarget_1", (object)Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(num));
				}
			}
		}

		public int GetThreat(uint avatarID)
		{
			if (_threatTable.ContainsKey(avatarID))
			{
				return _threatTable[avatarID];
			}
			return 0;
		}

		public uint RetargetByThreat(uint curTargetID)
		{
			int num = 0;
			uint num2 = 0u;
			for (int i = 0; i < _threatTable.Count; i++)
			{
				int num3 = _threatTable.Values[i];
				if (num3 <= num)
				{
					continue;
				}
				uint runtimeID = _threatTable.Keys[i];
				if (Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID) != null)
				{
					BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
					if (avatarByRuntimeID != null && avatarByRuntimeID.IsActive())
					{
						num = num3;
						num2 = _threatTable.Keys[i];
					}
				}
			}
			if (num2 == curTargetID)
			{
				return curTargetID;
			}
			float num4 = GetThreat(curTargetID);
			if ((float)num > num4 + 10f)
			{
				return num2;
			}
			return curTargetID;
		}

		private bool ListenKilled(EvtKilled evt)
		{
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3)
			{
				if (_threatTable.ContainsKey(evt.targetID))
				{
					_threatTable.Remove(evt.targetID);
				}
				return true;
			}
			return false;
		}

		private bool ListenStageReady(EvtStageReady evt)
		{
			if (evt.isBorn)
			{
				Preparation();
				Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(_owner.runtimeID);
			}
			return true;
		}

		private void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
		{
			if ((state & (AbilityState.Stun | AbilityState.Paralyze | AbilityState.Frozen)) != AbilityState.None)
			{
				InterruptMainAI(true);
			}
		}

		private void OnAbilityStateRemove(AbilityState state)
		{
			if ((state & (AbilityState.Stun | AbilityState.Paralyze | AbilityState.Frozen)) != AbilityState.None)
			{
				InterruptMainAI(false);
			}
		}

		private void OnHitStateChanged(BaseMonoMonster monster, bool fromHitState, bool toHitState)
		{
			if (!fromHitState && toHitState)
			{
				InterruptMainAISub(true);
			}
			else if (fromHitState && !toHitState)
			{
				InterruptMainAISub(false);
			}
		}

		private void InterruptMainAI(bool Interruption)
		{
			_aiTree.SendEvent("Interruption", (object)Interruption, (object)Interruption);
		}

		private void InterruptMainAISub(bool Interruption)
		{
			List<SharedVariable> allVariables = _aiTree.GetAllVariables();
			for (int i = 0; i < allVariables.Count; i++)
			{
				if (allVariables[i].Name == "_CommonAIType" && (int)allVariables[i].GetValue() != 0)
				{
					return;
				}
			}
			_aiTree.SendEvent("Interruption", (object)Interruption);
		}

		public void RestartMainAI()
		{
			InterruptMainAI(true);
			InterruptMainAI(false);
		}
	}
}
