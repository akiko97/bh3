using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class PlayerDataItem
	{
		public string token;

		public int userId;

		public string nickname;

		public string selfDesc;

		public int teamLevel;

		public int teamExp;

		public int hcoin;

		public int scoin;

		public int stamina;

		public int staminaRecoverLeftTime;

		public int skillPoint;

		public int skillPointRecoverLeftTime;

		public int skillPointLimit;

		public int equipmentSizeLimit;

		public int friendsPoint;

		public int maxFriend = 2;

		public int endlessMinPlayerLevel = 10;

		public int endlessMaxProgress = 99;

		public int disjoin_equipment_back_exp_percent;

		public int staminaRecoverConfigTime;

		public int skillPointRecoverConfigTime;

		public int reviveHcoinCost;

		public int sameTypePowerUpRataInt;

		public Dictionary<int, int> costAddByAvatarStar;

		public int avatarCombatBaseWeight;

		public int avatarCombatBaseStarRate;

		public int avatarCombatBaseLevelRate;

		public int avatarCombatBaseUnlockStarRate;

		public int avatarCombatSkillWeight;

		public int avatarCombatIslandWeight;

		public int avatarCombatWeaponWeight;

		public int avatarCombatWeaponRarityRate;

		public int avatarCombatWeaponSubRarityRate;

		public int avatarCombatWeaponLevelRate;

		public int avatarCombatStigmataWeight;

		public int avatarCombatStigmataRarityRate;

		public int avatarCombatStigmataSubRarityRate;

		public int avatarCombatStigmataLevelRate;

		public int avatarCombatStigmataSuitNumRate;

		public Dictionary<StageType, List<int>> teamDict;

		public Dictionary<int, int> gachaTicketPriceDict;

		public int offlineFriendsPoint;

		public int powerUpScoinCostRate;

		public int endlessUseItemCDTime;

		public GetSignInRewardStatusRsp signInStatus;

		public int minLevelToGenerateInviteCode;

		public int maxLevelToAcceptInvite;

		private PlayerLevelMetaData _metaData;

		public DateTime nextStaminaRecoverDatetime;

		public DateTime nextSkillPtRecoverDatetime;

		public CacheData<PlayerScoinExchangeInfo> scoinExchangeCache;

		public CacheData<PlayerStaminaExchangeInfo> staminaExchangeCache;

		public CacheData<PlayerSkillPointExchangeInfo> skillPointExchangeCache;

		public PlayerUITempSaveData uiTempSaveData;

		public CacheDataUtil _cacheDataUtil;

		public bool initByGetMainDataRsp;

		public int maxFriendFinal
		{
			get
			{
				return maxFriend + Singleton<IslandModule>.Instance.GetMaxFriendAdd();
			}
		}

		public int TeamMaxExp
		{
			get
			{
				return _metaData.exp;
			}
		}

		public int TeamNeedExp
		{
			get
			{
				return TeamMaxExp - teamExp;
			}
		}

		public int NumFriends
		{
			get
			{
				return _metaData.numFriends;
			}
		}

		public string NickNameText
		{
			get
			{
				return (!string.IsNullOrEmpty(nickname)) ? nickname : LocalizationGeneralLogic.GetText("Menu_DefaultNickname", userId);
			}
		}

		public int MaxStamina
		{
			get
			{
				return _metaData.stamina;
			}
		}

		public int AvatarLevelLimit
		{
			get
			{
				return _metaData.avatarLevelLimit;
			}
		}

		public string SelfDescText
		{
			get
			{
				return (!string.IsNullOrEmpty(selfDesc)) ? selfDesc : LocalizationGeneralLogic.GetText("Menu_DefaultSelfDesc");
			}
		}

		public PlayerDataItem(int teamLevel = 1)
		{
			this.teamLevel = teamLevel;
			OnLevelChange(this.teamLevel, this.teamLevel);
			costAddByAvatarStar = new Dictionary<int, int>();
			teamDict = new Dictionary<StageType, List<int>>();
			gachaTicketPriceDict = new Dictionary<int, int>();
			scoinExchangeCache = new CacheData<PlayerScoinExchangeInfo>();
			staminaExchangeCache = new CacheData<PlayerStaminaExchangeInfo>();
			skillPointExchangeCache = new CacheData<PlayerSkillPointExchangeInfo>();
			_cacheDataUtil = new CacheDataUtil();
			_cacheDataUtil.CreateCacheUtil(ECacheData.Stamina, staminaExchangeCache, Singleton<NetworkManager>.Instance.RequestGetStaminaExchangeInfo, 17);
			uiTempSaveData = new PlayerUITempSaveData();
			signInStatus = null;
			initByGetMainDataRsp = false;
		}

		public void OnStaminaRecoverTimeChange(int preValue, int newValue)
		{
			nextStaminaRecoverDatetime = TimeUtil.Now.AddSeconds(staminaRecoverLeftTime);
		}

		public DateTime GetStaminaFullTime()
		{
			int num = staminaRecoverLeftTime + staminaRecoverConfigTime * (MaxStamina - stamina - 1);
			return TimeUtil.Now.AddSeconds(num);
		}

		public void OnSkillPointRecoverTimeChange(int preValue, int newValue)
		{
			nextSkillPtRecoverDatetime = TimeUtil.Now.AddSeconds(skillPointRecoverLeftTime);
		}

		public DateTime GetSkillPointFullTime()
		{
			int num = skillPointRecoverLeftTime + skillPointRecoverConfigTime * (skillPointLimit - skillPoint - 1);
			return TimeUtil.Now.AddSeconds(num);
		}

		public void OnLevelChange(int preValue, int newValue)
		{
			if (_metaData == null || newValue != preValue)
			{
				_metaData = PlayerLevelMetaDataReader.GetPlayerLevelMetaDataByKey(newValue);
			}
			if (newValue != preValue)
			{
				Singleton<IslandModule>.Instance.OnPlayerLevelChanged(newValue, preValue);
				Singleton<NetworkManager>.Instance.RequestGetInviteeFriend();
			}
		}

		public void OnCoinChange(int preValue, int newValue)
		{
			WwiseAudioManager instance = Singleton<WwiseAudioManager>.Instance;
			if (preValue > newValue && instance != null)
			{
				instance.Post("UI_Gen_Buy_Item");
			}
		}

		public bool IsStaminaFull()
		{
			return stamina >= MaxStamina;
		}

		public bool IsSkillPointFull()
		{
			return skillPoint >= skillPointLimit;
		}

		public List<int> GetMemberList(StageType levelType)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (!teamDict.ContainsKey(levelType))
			{
				teamDict.Add(levelType, new List<int>());
			}
			return teamDict[levelType];
		}

		public void SetTeamMember(StageType levelType, List<int> newMemberList)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			teamDict[levelType] = newMemberList;
		}

		public void SetTeamMember(StageType levelType, int num, int avatarId)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (num > teamDict[levelType].Count)
			{
				teamDict[levelType].Add(avatarId);
			}
			else
			{
				teamDict[levelType][num - 1] = avatarId;
			}
		}

		public void RemoveTeamMember(StageType levelType, int num)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			teamDict[levelType].RemoveAt(num - 1);
		}

		public void SwitchTeamMember(StageType levelType, int numLeft, int numRight)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			if (teamDict.ContainsKey(levelType))
			{
				int value = teamDict[levelType][numLeft - 1];
				teamDict[levelType][numLeft - 1] = teamDict[levelType][numRight - 1];
				teamDict[levelType][numRight - 1] = value;
			}
		}

		public bool HasTeamMember(StageType levelType, int num)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (!teamDict.ContainsKey(levelType))
			{
				return false;
			}
			return num <= teamDict[levelType].Count;
		}

		public override string ToString()
		{
			return "PlayerDataItem: " + userId;
		}
	}
}
