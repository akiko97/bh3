using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class EndlessModule : BaseModule
	{
		private const int INVISIBLE_ITEM_ID = 70015;

		private int UID;

		public GetEndlessDataRsp endlessData;

		private Dictionary<int, EndlessPlayerData> endlessPlayerDataDict;

		private Dictionary<int, PlayerFriendBriefData> playerDataDict;

		public GetEndlessTopGroupRsp topGroupData;

		private DateTime _lastTimeGetTopGroupData = DateTime.Now.AddDays(-1.0);

		private List<int> _playerRankList;

		public Stack<EndlessWarInfo> warInfoList;

		private Dictionary<int, EndlessItem> _playerItemDict;

		private Dictionary<int, EndlessItemFrozenInfo> _itemFrozenInfoDict;

		private Dictionary<int, EndlessAvatarHp> avatarHPDict;

		public EndlessToolDataItem justBurstBombData;

		private List<int> _promoteRankList;

		private List<int> _normalRankList;

		private List<int> _demoteRankList;

		private List<EndlessToolDataItem> _waitToEffectToolDataList;

		public int TopGroupLevel
		{
			get
			{
				return EndlessGroupMetaDataReader.GetItemList().Count;
			}
		}

		public DateTime BeginTime
		{
			get
			{
				return Miscs.GetDateTimeFromTimeStamp(endlessData.begin_time);
			}
		}

		public DateTime EndTime
		{
			get
			{
				return Miscs.GetDateTimeFromTimeStamp(endlessData.end_time);
			}
		}

		public DateTime SettlementTime
		{
			get
			{
				return Miscs.GetDateTimeFromTimeStamp(endlessData.close_time);
			}
		}

		public int CurrentRewardID
		{
			get
			{
				SetupRankList();
				EndlessGroupMetaData endlessGroupMetaDataByKey = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey((int)endlessData.group_level);
				foreach (int promoteRank in _promoteRankList)
				{
					if (UID == promoteRank)
					{
						return endlessGroupMetaDataByKey.prototeRewardID;
					}
				}
				foreach (int normalRank in _normalRankList)
				{
					if (UID == normalRank)
					{
						return endlessGroupMetaDataByKey.normalRewardID;
					}
				}
				foreach (int demoteRank in _demoteRankList)
				{
					if (UID == demoteRank)
					{
						return endlessGroupMetaDataByKey.demoteRewardID;
					}
				}
				return -1;
			}
		}

		public RankUpDownStatus CurrentRankStatus
		{
			get
			{
				SetupRankList();
				foreach (int promoteRank in _promoteRankList)
				{
					if (UID == promoteRank)
					{
						return RankUpDownStatus.Up;
					}
				}
				foreach (int normalRank in _normalRankList)
				{
					if (UID == normalRank)
					{
						return RankUpDownStatus.Stay;
					}
				}
				foreach (int demoteRank in _demoteRankList)
				{
					if (UID == demoteRank)
					{
						return RankUpDownStatus.Down;
					}
				}
				return RankUpDownStatus.Stay;
			}
		}

		public int CurrentRank
		{
			get
			{
				SetupRankList();
				List<int> rankListSorted = GetRankListSorted();
				for (int i = 0; i < rankListSorted.Count; i++)
				{
					if (UID == rankListSorted[i])
					{
						return i + 1;
					}
				}
				return 100;
			}
		}

		public int CurrentFinishProgress
		{
			get
			{
				return (int)endlessPlayerDataDict[UID].progress;
			}
		}

		public int currentGroupLevel
		{
			get
			{
				return (int)endlessData.group_level;
			}
		}

		public int randomSeed
		{
			get
			{
				if (endlessData.random_seedSpecified)
				{
					return (int)endlessData.random_seed;
				}
				return 0;
			}
		}

		public int maxLevelEverReach
		{
			get
			{
				return (int)endlessPlayerDataDict[UID].max_progress;
			}
		}

		private EndlessModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			UID = Singleton<PlayerModule>.Instance.playerData.userId;
			endlessData = null;
			endlessPlayerDataDict = new Dictionary<int, EndlessPlayerData>();
			playerDataDict = new Dictionary<int, PlayerFriendBriefData>();
			warInfoList = new Stack<EndlessWarInfo>();
			_playerItemDict = new Dictionary<int, EndlessItem>();
			_itemFrozenInfoDict = new Dictionary<int, EndlessItemFrozenInfo>();
			_promoteRankList = new List<int>();
			_normalRankList = new List<int>();
			_demoteRankList = new List<int>();
			_waitToEffectToolDataList = new List<EndlessToolDataItem>();
			_playerRankList = new List<int>();
			InitAvatarHPDict();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 140:
				return OnGetEndlessDataRsp(pkt.getData<GetEndlessDataRsp>());
			case 150:
				return OnGetEndlessAvatarHpRsp(pkt.getData<GetEndlessAvatarHpRsp>());
			case 151:
				return OnEndlessPlayerDataUpdateNotify(pkt.getData<EndlessPlayerDataUpdateNotify>());
			case 142:
				return OnEndlessStageBeginRsp(pkt.getData<EndlessStageBeginRsp>());
			case 144:
				return OnEndlessStageEndRsp(pkt.getData<EndlessStageEndRsp>());
			case 146:
				return OnGetLastEndlessRewardDataRsp(pkt.getData<GetLastEndlessRewardDataRsp>());
			case 153:
				return OnEndlessWarInfoNotify(pkt.getData<EndlessWarInfoNotify>());
			case 148:
				return OnUseEndlessItemRsp(pkt.getData<UseEndlessItemRsp>());
			case 152:
				return OnEndlessItemDataUpdateNotify(pkt.getData<EndlessItemDataUpdateNotify>());
			case 220:
				return OnGetEndlessTopGroupRsp(pkt.getData<GetEndlessTopGroupRsp>());
			default:
				return false;
			}
		}

		private bool OnGetEndlessDataRsp(GetEndlessDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				ClearData();
				endlessData = rsp;
				foreach (EndlessPlayerData item in rsp.endless_data_list)
				{
					if (item.uid == UID && item.is_just_bomb_burstSpecified)
					{
						EndlessPlayerData selfEndlessData = GetSelfEndlessData();
						EndlessWaitBurstBomb val = null;
						EndlessWaitBurstBomb bombData;
						foreach (EndlessWaitBurstBomb item2 in selfEndlessData.wait_burst_bomb_list)
						{
							bombData = item2;
							if (item.wait_burst_bomb_list.FindLast((EndlessWaitBurstBomb data) => data.item_id == bombData.item_id) == null)
							{
								val = bombData;
								break;
							}
						}
						justBurstBombData = new EndlessToolDataItem((int)val.item_id);
					}
					endlessPlayerDataDict[(int)item.uid] = item;
				}
				foreach (PlayerFriendBriefData item3 in rsp.brief_data_list)
				{
					playerDataDict[(int)item3.uid] = item3;
				}
				foreach (EndlessWarInfo item4 in rsp.war_info_list)
				{
					warInfoList.Push(item4);
				}
				foreach (EndlessItemFrozenInfo item5 in rsp.item_frozen_list)
				{
					_itemFrozenInfoDict[(int)item5.target_uid] = item5;
				}
				_playerItemDict.Clear();
				foreach (EndlessItem item6 in rsp.item_list)
				{
					if (EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey((int)item6.item_id) != null)
					{
						_playerItemDict[(int)item6.item_id] = item6;
					}
				}
			}
			return false;
		}

		private void ClearData()
		{
			endlessPlayerDataDict.Clear();
			playerDataDict.Clear();
			warInfoList.Clear();
			_playerItemDict.Clear();
			_itemFrozenInfoDict.Clear();
			_promoteRankList.Clear();
			_normalRankList.Clear();
			_demoteRankList.Clear();
			_waitToEffectToolDataList.Clear();
			_playerRankList.Clear();
			endlessData = null;
		}

		private int GroupRankSortComparor(int palyerAUid, int PlayerBUid)
		{
			EndlessPlayerData val = endlessPlayerDataDict[palyerAUid];
			EndlessPlayerData val2 = endlessPlayerDataDict[PlayerBUid];
			if (val.progress != val2.progress)
			{
				return (int)(val2.progress - val.progress);
			}
			if (val.progress_time != val2.progress_time)
			{
				return (int)(val.progress_time - val2.progress_time);
			}
			return (int)(val.uid - val2.uid);
		}

		private int TopGroupRankSortComparor(EndlessPlayerData playerA, EndlessPlayerData playerB)
		{
			if (playerA.progress != playerB.progress)
			{
				return (int)(playerB.progress - playerA.progress);
			}
			if (playerA.progress_time != playerB.progress_time)
			{
				return (int)(playerA.progress_time - playerB.progress_time);
			}
			return (int)(playerA.uid - playerB.uid);
		}

		private bool OnGetEndlessAvatarHpRsp(GetEndlessAvatarHpRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Dictionary<int, EndlessAvatarHp> dictionary = new Dictionary<int, EndlessAvatarHp>();
				foreach (EndlessAvatarHp item in rsp.avatar_hp_list)
				{
					dictionary[(int)item.avatar_id] = item;
				}
				foreach (AvatarDataItem userAvatar in Singleton<AvatarModule>.Instance.UserAvatarList)
				{
					int avatarID = userAvatar.avatarID;
					if (dictionary.ContainsKey(avatarID))
					{
						avatarHPDict[avatarID] = dictionary[avatarID];
						continue;
					}
					EndlessAvatarHp val = avatarHPDict[avatarID];
					val.hp_percent = 100u;
					val.is_die = false;
				}
			}
			return false;
		}

		private bool OnEndlessPlayerDataUpdateNotify(EndlessPlayerDataUpdateNotify rsp)
		{
			if (rsp.player_data.uid == UID && rsp.player_data.is_just_bomb_burstSpecified)
			{
				EndlessPlayerData selfEndlessData = GetSelfEndlessData();
				EndlessWaitBurstBomb val = null;
				EndlessWaitBurstBomb bombData;
				foreach (EndlessWaitBurstBomb item in selfEndlessData.wait_burst_bomb_list)
				{
					bombData = item;
					if (rsp.player_data.wait_burst_bomb_list.FindLast((EndlessWaitBurstBomb data) => data.item_id == bombData.item_id) == null)
					{
						val = bombData;
						break;
					}
				}
				justBurstBombData = new EndlessToolDataItem((int)val.item_id);
			}
			endlessPlayerDataDict[(int)rsp.player_data.uid] = rsp.player_data;
			foreach (PlayerFriendBriefData item2 in rsp.brief_data_list)
			{
				playerDataDict[(int)item2.uid] = item2;
			}
			return false;
		}

		private bool OnEndlessStageBeginRsp(EndlessStageBeginRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && rsp.progressSpecified)
			{
				endlessPlayerDataDict[UID].progress = rsp.progress;
			}
			return false;
		}

		private bool OnEndlessStageEndRsp(EndlessStageEndRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && rsp.progressSpecified)
			{
				endlessPlayerDataDict[UID].progress = rsp.progress;
			}
			return false;
		}

		private bool OnGetLastEndlessRewardDataRsp(GetLastEndlessRewardDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && rsp.reward_list.Count > 0)
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData = rsp;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			return false;
		}

		private bool OnEndlessWarInfoNotify(EndlessWarInfoNotify rsp)
		{
			warInfoList.Push(rsp.war_info);
			return false;
		}

		private bool OnUseEndlessItemRsp(UseEndlessItemRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if ((int)rsp.retcode == 0 && rsp.target_uidSpecified)
			{
				EndlessItemFrozenInfo val = new EndlessItemFrozenInfo();
				val.target_uid = rsp.target_uid;
				val.frozen_time = rsp.item_frozen_time;
				_itemFrozenInfoDict[(int)rsp.target_uid] = val;
			}
			return false;
		}

		private bool OnEndlessItemDataUpdateNotify(EndlessItemDataUpdateNotify rsp)
		{
			foreach (EndlessItem item in rsp.item_list)
			{
				if (EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey((int)item.item_id) != null)
				{
					if (item.num < 1)
					{
						_playerItemDict.Remove((int)item.item_id);
					}
					else
					{
						_playerItemDict[(int)item.item_id] = item;
					}
				}
			}
			return false;
		}

		private bool OnGetEndlessTopGroupRsp(GetEndlessTopGroupRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				_lastTimeGetTopGroupData = TimeUtil.Now;
				topGroupData = rsp;
			}
			return false;
		}

		private void InitAvatarHPDict()
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			avatarHPDict = new Dictionary<int, EndlessAvatarHp>();
			foreach (AvatarDataItem userAvatar in Singleton<AvatarModule>.Instance.UserAvatarList)
			{
				EndlessAvatarHp val = new EndlessAvatarHp();
				val.avatar_id = (uint)userAvatar.avatarID;
				val.hp_percent = 100u;
				val.next_recover_time = (uint)TimeUtil.Now.AddDays(1.0).Ticks;
				val.is_die = false;
				avatarHPDict[userAvatar.avatarID] = val;
			}
		}

		public int GetAvatarRemainHP(int avatarId)
		{
			EndlessAvatarHp value;
			avatarHPDict.TryGetValue(avatarId, out value);
			if (value == null)
			{
				return -1;
			}
			return (int)value.hp_percent;
		}

		public EndlessAvatarHp GetEndlessAvatarHPData(int avatarId)
		{
			EndlessAvatarHp value;
			avatarHPDict.TryGetValue(avatarId, out value);
			if (value == null)
			{
				return null;
			}
			return value;
		}

		public void SetAvatarHP(int avatarHPPercent, int avatarId)
		{
			EndlessAvatarHp value;
			avatarHPDict.TryGetValue(avatarId, out value);
			if (value != null)
			{
				value.hp_percent = (uint)avatarHPPercent;
			}
		}

		public void CheckAllAvatarHPChanged()
		{
			foreach (EndlessAvatarHp value in avatarHPDict.Values)
			{
				CheckAvatarHPChanged(value);
			}
		}

		public DateTime CheckAvatarHPChanged(EndlessAvatarHp avatarHPData)
		{
			if (avatarHPData.hp_percent < 100 && avatarHPData.next_recover_timeSpecified)
			{
				DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(avatarHPData.next_recover_time);
				if (TimeUtil.Now >= dateTimeFromTimeStamp)
				{
					Singleton<NetworkManager>.Instance.RequestEndlessAvatarHp();
				}
				return dateTimeFromTimeStamp;
			}
			return DateTime.MinValue;
		}

		public PlayerFriendBriefData GetPlayerBriefData(int playerUid)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			if (!playerDataDict.ContainsKey(playerUid))
			{
				PlayerFriendBriefData val = new PlayerFriendBriefData();
				val.nickname = LocalizationGeneralLogic.GetText("Menu_DefaultNickname", playerUid);
				playerDataDict.Add(playerUid, val);
			}
			return playerDataDict[playerUid];
		}

		public EndlessPlayerData GetPlayerEndlessData(int playerUid)
		{
			return endlessPlayerDataDict[playerUid];
		}

		public EndlessPlayerData GetSelfEndlessData()
		{
			return endlessPlayerDataDict[UID];
		}

		public PlayerFriendBriefData GetTopGroupPlayerBriefData(int playerUid)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			foreach (PlayerFriendBriefData item in topGroupData.brief_data_list)
			{
				if (item.uid != playerUid)
				{
					continue;
				}
				return item;
			}
			PlayerFriendBriefData val = new PlayerFriendBriefData();
			val.nickname = LocalizationGeneralLogic.GetText("Menu_DefaultNickname", playerUid);
			return val;
		}

		public EndlessPlayerData GetTopGroupPlayerEndlessData(int playerUid)
		{
			foreach (EndlessPlayerData item in topGroupData.endless_data_list)
			{
				if (item.uid == playerUid)
				{
					return item;
				}
			}
			return null;
		}

		public EndlessActivityStatus GetEndlessActivityStatus()
		{
			if (TimeUtil.Now < BeginTime)
			{
				return EndlessActivityStatus.WaitToStart;
			}
			if (TimeUtil.Now < EndTime)
			{
				return EndlessActivityStatus.InProgress;
			}
			if (TimeUtil.Now < SettlementTime)
			{
				return EndlessActivityStatus.WaitToSettlement;
			}
			if (TimeUtil.Now >= SettlementTime)
			{
				Singleton<NetworkManager>.Instance.RequestEndlessData();
				return EndlessActivityStatus.None;
			}
			return EndlessActivityStatus.None;
		}

		public DateTime GetFrozenEndTime(int targetUid)
		{
			EndlessItemFrozenInfo value;
			_itemFrozenInfoDict.TryGetValue(targetUid, out value);
			if (value == null)
			{
				return TimeUtil.Now.AddDays(-1.0);
			}
			return Miscs.GetDateTimeFromTimeStamp(value.frozen_time);
		}

		public List<EndlessItem> GetPlayerEndlessItemList()
		{
			List<EndlessItem> list = new List<EndlessItem>(_playerItemDict.Values);
			list.Sort((EndlessItem left, EndlessItem right) => (int)(left.item_id - right.item_id));
			return list;
		}

		public List<int> GetRankListSorted()
		{
			_playerRankList.Clear();
			foreach (EndlessPlayerData value in endlessPlayerDataDict.Values)
			{
				_playerRankList.Add((int)value.uid);
			}
			_playerRankList.Sort(GroupRankSortComparor);
			return _playerRankList;
		}

		public List<EndlessToolDataItem> GetAppliedToolDataList()
		{
			_waitToEffectToolDataList.Clear();
			List<EndlessWaitEffectItem> wait_effect_item_list = endlessPlayerDataDict[UID].wait_effect_item_list;
			List<EndlessWaitBurstBomb> wait_burst_bomb_list = endlessPlayerDataDict[UID].wait_burst_bomb_list;
			for (int i = 0; i < wait_effect_item_list.Count; i++)
			{
				EndlessWaitEffectItem val = wait_effect_item_list[i];
				EndlessToolDataItem item = new EndlessToolDataItem((int)val.item_id);
				_waitToEffectToolDataList.Add(item);
			}
			foreach (EndlessWaitBurstBomb item3 in wait_burst_bomb_list)
			{
				EndlessToolDataItem item2 = new EndlessToolDataItem((int)item3.item_id);
				_waitToEffectToolDataList.Add(item2);
			}
			if (SelfInvisible())
			{
				_waitToEffectToolDataList.Add(new EndlessToolDataItem(70015));
			}
			return _waitToEffectToolDataList;
		}

		private void SetupRankList()
		{
			EndlessGroupMetaData endlessGroupMetaDataByKey = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey((int)endlessData.group_level);
			int num = 0;
			int num2 = 0;
			List<int> rankListSorted = GetRankListSorted();
			for (num2 = 0; num + num2 < rankListSorted.Count && GetPlayerEndlessData(rankListSorted[num + num2]).progress != 0 && num + num2 < endlessGroupMetaDataByKey.promoteRank; num2++)
			{
			}
			_promoteRankList = rankListSorted.GetRange(num, num2);
			num += num2;
			for (num2 = 0; num + num2 < rankListSorted.Count && (endlessGroupMetaDataByKey.groupLevel <= 1 || GetPlayerEndlessData(rankListSorted[num + num2]).progress != 0) && num + num2 < endlessGroupMetaDataByKey.demoteRank - 1; num2++)
			{
			}
			_normalRankList = rankListSorted.GetRange(num, num2);
			num += num2;
			for (num2 = 0; num + num2 < rankListSorted.Count; num2++)
			{
			}
			_demoteRankList = rankListSorted.GetRange(num, num2);
		}

		private void SetupTopGroupRankList()
		{
			if (CanSeeTopGroupInfo())
			{
				EndlessGroupMetaData endlessGroupMetaDataByKey = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(TopGroupLevel);
				int num = 0;
				int num2 = 0;
				List<EndlessPlayerData> endless_data_list = topGroupData.endless_data_list;
				endless_data_list.Sort(TopGroupRankSortComparor);
				for (num2 = 0; num + num2 < endless_data_list.Count && endless_data_list[num + num2].progress != 0 && num + num2 < endlessGroupMetaDataByKey.promoteRank; num2++)
				{
				}
				_promoteRankList.Clear();
				for (int i = 0; i < num2; i++)
				{
					_promoteRankList.Add((int)endless_data_list[num + i].uid);
				}
				num += num2;
				for (num2 = 0; num + num2 < endless_data_list.Count && (endlessGroupMetaDataByKey.groupLevel <= 1 || endless_data_list[num + num2].progress != 0) && num + num2 < endlessGroupMetaDataByKey.demoteRank - 1; num2++)
				{
				}
				_normalRankList.Clear();
				for (int j = 0; j < num2; j++)
				{
					_normalRankList.Add((int)endless_data_list[num + j].uid);
				}
				num += num2;
				for (num2 = 0; num + num2 < endless_data_list.Count; num2++)
				{
				}
				_demoteRankList.Clear();
				for (int k = 0; k < num2; k++)
				{
					_demoteRankList.Add((int)endless_data_list[num + k].uid);
				}
			}
		}

		public List<int> GetPromoteRank(EndlessMainPageContext.ViewStatus viewStatus = EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
		{
			switch (viewStatus)
			{
			case EndlessMainPageContext.ViewStatus.ShowCurrentGroup:
				SetupRankList();
				break;
			case EndlessMainPageContext.ViewStatus.ShowTopGroup:
				SetupTopGroupRankList();
				break;
			}
			return _promoteRankList;
		}

		public List<int> GetNormalRank(EndlessMainPageContext.ViewStatus viewStatus = EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
		{
			switch (viewStatus)
			{
			case EndlessMainPageContext.ViewStatus.ShowCurrentGroup:
				SetupRankList();
				break;
			case EndlessMainPageContext.ViewStatus.ShowTopGroup:
				SetupTopGroupRankList();
				break;
			}
			return _normalRankList;
		}

		public List<int> GetDemoteRank(EndlessMainPageContext.ViewStatus viewStatus = EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
		{
			switch (viewStatus)
			{
			case EndlessMainPageContext.ViewStatus.ShowCurrentGroup:
				SetupRankList();
				break;
			case EndlessMainPageContext.ViewStatus.ShowTopGroup:
				SetupTopGroupRankList();
				break;
			}
			return _demoteRankList;
		}

		public bool CanRequestTopGroupInfo()
		{
			if (endlessData.cur_top_group_level < TopGroupLevel || endlessData.group_level == TopGroupLevel)
			{
				return false;
			}
			if (topGroupData == null)
			{
				return true;
			}
			if (_lastTimeGetTopGroupData.AddHours(1.0) < TimeUtil.Now)
			{
				return true;
			}
			return false;
		}

		public bool CanSeeTopGroupInfo()
		{
			return endlessData.cur_top_group_level == TopGroupLevel && endlessData.group_level != TopGroupLevel;
		}

		public bool PlayerInvisible(int uid)
		{
			EndlessPlayerData val = endlessPlayerDataDict[uid];
			if (val.hidden_expire_timeSpecified && Miscs.GetDateTimeFromTimeStamp(val.hidden_expire_time) > TimeUtil.Now)
			{
				return true;
			}
			return false;
		}

		public bool SelfInvisible()
		{
			return PlayerInvisible(UID);
		}
	}
}
