using System;
using System.Collections.Generic;
using MoleMole.Config;
using UniRx;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class AvatarManager
	{
		private uint _localAvatarRuntimeID;

		private uint _helperAvatarRuntimeID;

		private Dictionary<uint, BaseMonoAvatar> _avatarDict;

		private List<BaseMonoAvatar> _avatarLs;

		private List<Tuple<string, GameObject>> _preloadedAvatar;

		private List<BaseMonoAvatar> _playerAvatars;

		public Action<BaseMonoAvatar, BaseMonoAvatar> onLocalAvatarChanged;

		public bool isAutoBattle { get; private set; }

		private AvatarManager()
		{
			_avatarDict = new Dictionary<uint, BaseMonoAvatar>();
			_avatarLs = new List<BaseMonoAvatar>();
			_preloadedAvatar = new List<Tuple<string, GameObject>>();
			_playerAvatars = new List<BaseMonoAvatar>();
			_localAvatarRuntimeID = 0u;
		}

		public void InitAtAwake()
		{
		}

		public void InitAtStart()
		{
		}

		public void Core()
		{
			RemoveAllRemoveableAvatars();
		}

		public List<BaseMonoAvatar> GetAllAvatars()
		{
			return _avatarLs;
		}

		public int GetActiveAvatarCount()
		{
			int num = 0;
			for (int i = 0; i < _avatarLs.Count; i++)
			{
				if (_avatarLs[i].IsActive())
				{
					num++;
				}
			}
			return num;
		}

		public List<BaseMonoAvatar> GetAllPlayerAvatars()
		{
			return _playerAvatars;
		}

		public bool IsPlayerAvatar(BaseMonoAvatar avatar)
		{
			return _playerAvatars.Contains(avatar);
		}

		public bool IsPlayerAvatar(uint avatarID)
		{
			for (int i = 0; i < _playerAvatars.Count; i++)
			{
				if (_playerAvatars[i].GetRuntimeID() == avatarID)
				{
					return true;
				}
			}
			return false;
		}

		public BaseMonoAvatar GetAvatarByRuntimeID(uint runtimeID)
		{
			return _avatarDict[runtimeID];
		}

		public BaseMonoAvatar TryGetAvatarByRuntimeID(uint runtimeID)
		{
			BaseMonoAvatar value;
			_avatarDict.TryGetValue(runtimeID, out value);
			return value;
		}

		public BaseMonoAvatar GetLocalAvatar()
		{
			return _avatarDict[_localAvatarRuntimeID];
		}

		public BaseMonoAvatar TryGetLocalAvatar()
		{
			return (!_avatarDict.ContainsKey(_localAvatarRuntimeID)) ? null : _avatarDict[_localAvatarRuntimeID];
		}

		public BaseMonoAvatar GetHelperAvatar()
		{
			return (_helperAvatarRuntimeID == 0 || !_avatarDict.ContainsKey(_helperAvatarRuntimeID)) ? null : _avatarDict[_helperAvatarRuntimeID];
		}

		public BaseMonoAvatar GetTeamLeader()
		{
			return GetAllAvatars().Find((BaseMonoAvatar avatar) => avatar.isLeader);
		}

		public bool RemoveAvatarByRuntimeID(uint runtimeID)
		{
			BaseMonoAvatar baseMonoAvatar = _avatarDict[runtimeID];
			Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
			UnityEngine.Object.Destroy(baseMonoAvatar.gameObject);
			bool flag = true;
			flag &= _playerAvatars.Remove(baseMonoAvatar);
			flag &= _avatarDict.Remove(runtimeID);
			return flag & _avatarLs.Remove(baseMonoAvatar);
		}

		public void PreloadAvatar(string avatarType, bool useLow)
		{
			GameObject original = Miscs.LoadResource<GameObject>(AvatarData.GetPrefabResPath(avatarType, useLow));
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, InLevelData.CREATE_INIT_POS, Quaternion.Euler(0f, 200f, 0f));
			gameObject.GetComponent<BaseMonoAnimatorEntity>().Preload();
			_preloadedAvatar.Add(new Tuple<string, GameObject>(avatarType, gameObject));
		}

		public void PreloadTeamAvatars()
		{
			List<AvatarDataItem> list = new List<AvatarDataItem>();
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			if (instance != null && instance.memberList != null)
			{
				list.AddRange(instance.memberList);
			}
			bool useLow = !GlobalVars.AVATAR_USE_DYNAMIC_BONE || Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi;
			for (int i = 0; i < list.Count; i++)
			{
				PreloadAvatar(list[i].AvatarRegistryKey, useLow);
			}
			if (instance != null && instance.friendDetailItem != null)
			{
				FriendDetailDataItem friendDetailItem = instance.friendDetailItem;
				AvatarDataItem leaderAvatar = friendDetailItem.leaderAvatar;
				list.Add(leaderAvatar);
				PreloadAvatar(leaderAvatar.AvatarRegistryKey, true);
			}
		}

		public uint CreateAvatar(AvatarDataItem avatarDataItem, bool isLocal, Vector3 initPos, Vector3 initDir, uint runtimeID, bool isLeader, bool leaderSkillOn, bool isHelper = false, bool useLow = false)
		{
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Invalid comparison between Unknown and I4
			BaseMonoAvatar baseMonoAvatar = null;
			string avatarRegistryKey = avatarDataItem.AvatarRegistryKey;
			GameObject gameObject = null;
			int index = -1;
			for (int i = 0; i < _preloadedAvatar.Count; i++)
			{
				if (_preloadedAvatar[i].Item1 == avatarRegistryKey)
				{
					gameObject = _preloadedAvatar[i].Item2;
					index = i;
					break;
				}
			}
			if (gameObject != null)
			{
				gameObject.GetComponent<BaseMonoAvatar>().Enable();
				_preloadedAvatar.RemoveAt(index);
			}
			else
			{
				useLow = useLow || !GlobalVars.AVATAR_USE_DYNAMIC_BONE || Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi;
				GameObject original = Miscs.LoadResource<GameObject>(AvatarData.GetPrefabResPath(avatarRegistryKey, useLow));
				gameObject = (GameObject)UnityEngine.Object.Instantiate(original, InLevelData.CREATE_INIT_POS, Quaternion.Euler(0f, 200f, 0f));
			}
			baseMonoAvatar = gameObject.GetComponent<BaseMonoAvatar>();
			if (runtimeID == 0)
			{
				runtimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3);
			}
			baseMonoAvatar.Init(isLocal, runtimeID, avatarDataItem.AvatarRegistryKey, avatarDataItem.GetWeapon().ID, initPos, initDir, isLeader);
			bool isPlayerAvatar = !isHelper;
			RegisterAvatar(baseMonoAvatar, isLocal, isPlayerAvatar, isHelper);
			AvatarActor avatarActor = Singleton<EventManager>.Instance.CreateActor<AvatarActor>(baseMonoAvatar);
			avatarActor.InitAvatarDataItem(avatarDataItem, isLocal, isHelper, isLeader, leaderSkillOn);
			avatarActor.InitGalTouchBuff(avatarDataItem);
			avatarActor.PostInit();
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			if ((int)instance.LevelType == 4)
			{
				EndlessAvatarHp endlessAvatarHPData = Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(avatarDataItem.avatarID);
				avatarActor.HP = (float)avatarActor.maxHP * (float)endlessAvatarHPData.hp_percent / 100f;
				avatarActor.SP = (float)avatarActor.maxSP * (float)endlessAvatarHPData.sp_percent / 100f;
			}
			ConfigAvatar config = baseMonoAvatar.config;
			for (int j = 0; j < config.CommonArguments.PreloadEffectPatternGroups.Length; j++)
			{
				Singleton<EffectManager>.Instance.PreloadEffectGroup(config.CommonArguments.PreloadEffectPatternGroups[j]);
			}
			if (baseMonoAvatar is MonoBronya)
			{
				if (avatarActor.HasAppliedAbilityName("Weapon_Additional_BronyaLaser"))
				{
					Singleton<EffectManager>.Instance.PreloadEffectGroup("Bronya_Laser_Effects");
				}
				else
				{
					Singleton<EffectManager>.Instance.PreloadEffectGroup("Bronya_Gun_Effects");
				}
			}
			for (int k = 0; k < config.CommonArguments.RequestSoundBankNames.Length; k++)
			{
				Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(config.CommonArguments.RequestSoundBankNames[k]);
			}
			return baseMonoAvatar.GetRuntimeID();
		}

		public uint CreateAvatarMirror(BaseMonoAvatar owner, Vector3 initPos, Vector3 initDir, string AIName, float hpRatio)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(AvatarData.GetPrefabResPath(owner.AvatarTypeName, true)), initPos, Quaternion.LookRotation(initDir));
			BaseMonoAvatar component = gameObject.GetComponent<BaseMonoAvatar>();
			component.Init(false, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3), owner.AvatarTypeName, owner.EquipedWeaponID, initPos, initDir, false);
			RegisterAvatar(component, false, false, false);
			AvatarMirrorActor avatarMirrorActor = Singleton<EventManager>.Instance.CreateActor<AvatarMirrorActor>(component);
			avatarMirrorActor.InitFromAvatarActor(Singleton<EventManager>.Instance.GetActor<AvatarActor>(owner.GetRuntimeID()), hpRatio);
			avatarMirrorActor.PostInit();
			component.TriggerSwitchIn();
			BTreeAvatarAIController bTreeAvatarAIController = component.GetActiveAIController() as BTreeAvatarAIController;
			if (string.IsNullOrEmpty(AIName))
			{
				bTreeAvatarAIController.SetActive(false);
			}
			else
			{
				bTreeAvatarAIController.ChangeBehavior(AIName);
				component.ForceUseAIController();
			}
			return component.GetRuntimeID();
		}

		public void RegisterAvatar(BaseMonoAvatar avatar, bool isLocal, bool isPlayerAvatar, bool isHelperAvatar)
		{
			_avatarLs.Add(avatar);
			_avatarDict.Add(avatar.GetRuntimeID(), avatar);
			if (isLocal)
			{
				_localAvatarRuntimeID = avatar.GetRuntimeID();
			}
			if (isHelperAvatar)
			{
				_helperAvatarRuntimeID = avatar.GetRuntimeID();
			}
			if (isPlayerAvatar)
			{
				_playerAvatars.Add(avatar);
				avatar.onDie = (Action<BaseMonoAvatar>)Delegate.Combine(avatar.onDie, new Action<BaseMonoAvatar>(OnPlayerAvatarDie));
			}
		}

		private void RemoveAllRemoveableAvatars()
		{
			for (int i = 0; i < _avatarLs.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = _avatarLs[i];
				if (baseMonoAvatar.IsToBeRemove())
				{
					RemoveAvatarByRuntimeID(baseMonoAvatar.GetRuntimeID());
					i--;
				}
			}
		}

		public void RemoveAllAvatars()
		{
			int num;
			for (num = 0; num < _avatarLs.Count; num++)
			{
				BaseMonoAvatar baseMonoAvatar = _avatarLs[num];
				if (!baseMonoAvatar.IsToBeRemove())
				{
					baseMonoAvatar.SetDied(KillEffect.KillImmediately);
				}
				RemoveAvatarByRuntimeID(baseMonoAvatar.GetRuntimeID());
				num--;
			}
		}

		public void SetPause(bool pause)
		{
			foreach (BaseMonoAvatar value in _avatarDict.Values)
			{
				value.SetPause(pause);
			}
		}

		public void SetAutoBattle(bool isAuto)
		{
			isAutoBattle = isAuto;
			GetLocalAvatar().RefreshController();
		}

		public void SetLocalAvatar(uint runtimeID)
		{
			uint localAvatarRuntimeID = _localAvatarRuntimeID;
			_localAvatarRuntimeID = runtimeID;
			if (onLocalAvatarChanged != null)
			{
				onLocalAvatarChanged(_avatarDict[localAvatarRuntimeID], _avatarDict[_localAvatarRuntimeID]);
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtLocalAvatarChanged(runtimeID, localAvatarRuntimeID));
		}

		public bool IsLocalAvatar(uint runtimeID)
		{
			return runtimeID == _localAvatarRuntimeID;
		}

		public bool IsHelperAvatar(uint runtimeID)
		{
			return runtimeID == _helperAvatarRuntimeID;
		}

		public void CreateTeamAvatars()
		{
			List<AvatarDataItem> memberList = Singleton<LevelScoreManager>.Instance.memberList;
			for (int i = 0; i < memberList.Count; i++)
			{
				bool isLocal = i == 0;
				bool flag = i == 0;
				Singleton<AvatarManager>.Instance.CreateAvatar(memberList[i], isLocal, InLevelData.CREATE_INIT_POS, InLevelData.CREATE_INIT_FORWARD, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3), flag, flag);
			}
			CreateAvatarHelper();
			_preloadedAvatar.Clear();
		}

		private void CreateAvatarHelper()
		{
			if (Singleton<LevelScoreManager>.Instance.friendDetailItem != null && _helperAvatarRuntimeID == 0)
			{
				FriendDetailDataItem friendDetailItem = Singleton<LevelScoreManager>.Instance.friendDetailItem;
				AvatarDataItem leaderAvatar = friendDetailItem.leaderAvatar;
				bool leaderSkillOn = Singleton<FriendModule>.Instance.IsMyFriend(friendDetailItem.uid);
				Singleton<AvatarManager>.Instance.CreateAvatar(leaderAvatar, false, InLevelData.CREATE_INIT_POS, InLevelData.CREATE_INIT_FORWARD, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3), false, leaderSkillOn, true, true);
			}
		}

		public void ShowHelperAvater()
		{
			BaseMonoAvatar helperAvatar = GetHelperAvatar();
			if (!(helperAvatar != null))
			{
				return;
			}
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(helperAvatar.GetRuntimeID());
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			int num = 0;
			foreach (BaseMonoAvatar item in allPlayerAvatars)
			{
				AvatarActor actor2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(item.GetRuntimeID());
				if (actor2 != null)
				{
					num = Math.Max(actor2.level, num);
				}
			}
			float num2 = ((num <= (int)actor.level) ? (AvatarLevelMetaDataReader.GetAvatarLevelMetaDataByKey(num).avatarAssistConf / AvatarLevelMetaDataReader.GetAvatarLevelMetaDataByKey(actor.level).avatarAssistConf) : 1f);
			actor.attack = (float)actor.attack * num2;
			actor.GetPlugin<AvatarHelperStatePlugin>().TriggerSwitchIn();
		}

		public void HideHelperAvatar(bool force)
		{
			BaseMonoAvatar helperAvatar = GetHelperAvatar();
			if (helperAvatar != null)
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(helperAvatar.GetRuntimeID());
				actor.GetPlugin<AvatarHelperStatePlugin>().TriggerSwitchOut(force);
			}
		}

		public void InitAvatarsPos(List<MonoSpawnPoint> avatarSpawnPointList)
		{
			Vector3[] array = new Vector3[3]
			{
				Vector3.zero,
				Vector3.zero,
				Vector3.zero
			};
			switch (Singleton<LevelManager>.Instance.levelActor.levelMode)
			{
			case LevelActor.Mode.Single:
				array = InLevelData.SINGLE_MODE_AVATAR_INIT_POS_LIST;
				break;
			case LevelActor.Mode.Multi:
			case LevelActor.Mode.NetworkedMP:
				array = InLevelData.MUTIL_MODE_AVATAR_INIT_POS_LIST;
				break;
			case LevelActor.Mode.MultiRemote:
				array = InLevelData.MUTIL_REMOTE_MODE_AVATAR_INIT_POS_LIST;
				break;
			}
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			if (Singleton<LevelManager>.Instance.levelActor.levelMode != LevelActor.Mode.NetworkedMP)
			{
				allPlayerAvatars.Sort(Compare);
			}
			for (int i = 0; i < allPlayerAvatars.Count; i++)
			{
				if (allPlayerAvatars[i].IsActive())
				{
					if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.MultiRemote)
					{
						allPlayerAvatars[i].transform.position = avatarSpawnPointList[i].transform.TransformPoint(array[i]);
						allPlayerAvatars[i].transform.forward = avatarSpawnPointList[i].transform.forward;
					}
					else
					{
						allPlayerAvatars[i].transform.position = avatarSpawnPointList[0].transform.TransformPoint(array[i]);
						allPlayerAvatars[i].transform.forward = avatarSpawnPointList[0].transform.forward;
					}
				}
				else
				{
					allPlayerAvatars[i].transform.position = InLevelData.CREATE_INIT_POS;
				}
			}
		}

		private int Compare(BaseMonoAvatar a, BaseMonoAvatar b)
		{
			if (a.GetRuntimeID() == _localAvatarRuntimeID && b.GetRuntimeID() != _localAvatarRuntimeID)
			{
				return -1;
			}
			if (a.GetRuntimeID() != _localAvatarRuntimeID && b.GetRuntimeID() == _localAvatarRuntimeID)
			{
				return 1;
			}
			if (a.IsActive() && !b.IsActive())
			{
				return -1;
			}
			if (!a.IsActive() && b.IsActive())
			{
				return 1;
			}
			return 0;
		}

		public void SetMuteAllAvatarControl(bool mute)
		{
			foreach (BaseMonoAvatar value in _avatarDict.Values)
			{
				value.SetCountedMuteControl(mute);
				if (value.IsActive())
				{
					value.OrderMove = false;
					value.ClearAttackTriggers();
				}
			}
		}

		public void Destroy()
		{
			for (int i = 0; i < _preloadedAvatar.Count; i++)
			{
				if (_preloadedAvatar[i] != null && _preloadedAvatar[i].Item2 != null)
				{
					UnityEngine.Object.DestroyImmediate(_preloadedAvatar[i].Item2);
				}
			}
			for (int j = 0; j < _avatarLs.Count; j++)
			{
				if (_avatarLs[j] != null)
				{
					UnityEngine.Object.DestroyImmediate(_avatarLs[j]);
				}
			}
			onLocalAvatarChanged = null;
		}

		public void SetAllAvatarVisibility(bool visible)
		{
			for (int i = 0; i < _avatarLs.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = _avatarLs[i];
				for (int j = 0; j < baseMonoAvatar.renderers.Length; j++)
				{
					baseMonoAvatar.renderers[j].enabled = visible;
				}
			}
		}

		public void SetAvatarVisibility(bool visible, BaseMonoAvatar avatar)
		{
			if (_avatarLs.Contains(avatar))
			{
				for (int i = 0; i < avatar.renderers.Length; i++)
				{
					avatar.renderers[i].enabled = visible;
				}
			}
		}

		public BaseMonoAvatar GetFirstAliveAvatar()
		{
			List<BaseMonoAvatar> playerAvatars = _playerAvatars;
			for (int i = 0; i < playerAvatars.Count; i++)
			{
				if (playerAvatars[i].IsAlive())
				{
					return playerAvatars[i];
				}
			}
			return null;
		}

		private void OnPlayerAvatarDie(BaseMonoAvatar avatar)
		{
			if (!IsLocalAvatar(avatar.GetRuntimeID()))
			{
				avatar.gameObject.SetActive(false);
			}
			else
			{
				Singleton<LevelManager>.Instance.gameMode.HandleLocalPlayerAvatarDie(avatar);
			}
		}
	}
}
