using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoSubMonsterHPBarContainer : MonoBehaviour
	{
		public GameObject SubMonsterHPBarGO;

		public MonsterActor localAvatarLockedMonsterActor;

		private Dictionary<uint, MonoSubMonsterHPBar> _avatarHPBarMap;

		private Dictionary<uint, MonoSubMonsterHPBar> _subMonsterHPBarMap;

		private void Awake()
		{
			_avatarHPBarMap = new Dictionary<uint, MonoSubMonsterHPBar>();
			_subMonsterHPBarMap = new Dictionary<uint, MonoSubMonsterHPBar>();
		}

		public void OnTargetMonsterChange(MonsterActor targetBefore, MonsterActor targetAfter)
		{
			localAvatarLockedMonsterActor = targetAfter;
			if (IsLocalAvatarLockedMonsterBoss())
			{
				SetAllSubHPBarDisable();
			}
			if (localAvatarLockedMonsterActor != null && _subMonsterHPBarMap.ContainsKey(localAvatarLockedMonsterActor.runtimeID))
			{
				_subMonsterHPBarMap[localAvatarLockedMonsterActor.runtimeID].SetDisable();
			}
		}

		public void OnAttackLandedEvt(EvtAttackLanded evt)
		{
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) != 3 || Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.attackeeID) != 4)
			{
				return;
			}
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.targetID);
			if (actor == null || actor.runtimeID == Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID() || actor is AvatarMirrorActor)
			{
				return;
			}
			BaseMonoAvatar baseMonoAvatar = actor.entity as BaseMonoAvatar;
			if (!baseMonoAvatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection))
			{
				return;
			}
			MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.attackeeID);
			if (localAvatarLockedMonsterActor != null && localAvatarLockedMonsterActor.runtimeID == actor2.runtimeID)
			{
				return;
			}
			if (IsMonsterBoss(actor2))
			{
				SetAllSubHPBarDisable();
				return;
			}
			MonoSubMonsterHPBar monoSubMonsterHPBar;
			if (_subMonsterHPBarMap.ContainsKey(actor2.runtimeID))
			{
				monoSubMonsterHPBar = _subMonsterHPBarMap[actor2.runtimeID];
			}
			else
			{
				GameObject availableSubMonsterHPBar = GetAvailableSubMonsterHPBar();
				availableSubMonsterHPBar.transform.SetParent(base.transform, false);
				monoSubMonsterHPBar = availableSubMonsterHPBar.GetComponent<MonoSubMonsterHPBar>();
			}
			if (_avatarHPBarMap.ContainsKey(actor.runtimeID) && _avatarHPBarMap[actor.runtimeID] != monoSubMonsterHPBar)
			{
				_avatarHPBarMap[actor.runtimeID].RemoveAttacker(actor);
			}
			monoSubMonsterHPBar.SetupView(actor, actor2, 0.1f, OnHideHPBar);
			_avatarHPBarMap[actor.runtimeID] = monoSubMonsterHPBar;
			_subMonsterHPBarMap[actor2.runtimeID] = monoSubMonsterHPBar;
		}

		private void SetAllSubHPBarDisable()
		{
			foreach (KeyValuePair<uint, MonoSubMonsterHPBar> item in _avatarHPBarMap)
			{
				item.Value.SetDisable();
			}
		}

		private void OnHideHPBar(MonoSubMonsterHPBar hpBar)
		{
			Dictionary<uint, MonoSubMonsterHPBar> dictionary = new Dictionary<uint, MonoSubMonsterHPBar>();
			foreach (KeyValuePair<uint, MonoSubMonsterHPBar> item in _avatarHPBarMap)
			{
				if (item.Value != hpBar)
				{
					dictionary[item.Key] = item.Value;
				}
			}
			_avatarHPBarMap = dictionary;
			_subMonsterHPBarMap.Remove(hpBar.attackee.runtimeID);
		}

		private bool IsLocalAvatarLockedMonsterBoss()
		{
			if (localAvatarLockedMonsterActor == null)
			{
				return false;
			}
			if (localAvatarLockedMonsterActor.uniqueMonsterID != 0)
			{
				UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(localAvatarLockedMonsterActor.uniqueMonsterID);
				if (uniqueMonsterMetaData != null && uniqueMonsterMetaData.hpPhaseNum > 1)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsMonsterBoss(MonsterActor monster)
		{
			if (monster == null)
			{
				return false;
			}
			if (monster.uniqueMonsterID != 0)
			{
				UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(monster.uniqueMonsterID);
				if (uniqueMonsterMetaData != null && uniqueMonsterMetaData.hpPhaseNum > 1)
				{
					return true;
				}
			}
			return false;
		}

		private GameObject GetAvailableSubMonsterHPBar()
		{
			foreach (Transform item in base.transform)
			{
				if (!item.GetComponent<MonoSubMonsterHPBar>().enable)
				{
					return item.gameObject;
				}
			}
			return Object.Instantiate(SubMonsterHPBarGO);
		}
	}
}
