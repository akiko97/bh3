using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class FriendModule : BaseModule
	{
		public enum FriendSortType
		{
			Friend_NEW = 0,
			Request_NEW = 1,
			Level_DESC = 2,
			Level_ASC = 3,
			Star_DESC = 4,
			Star_ASC = 5,
			Combat_DESC = 6,
			Combat_ASC = 7
		}

		public List<FriendBriefDataItem> friendsList;

		private Dictionary<int, FriendBriefDataItem> _friendsBriefInfoDict;

		private Dictionary<int, CacheData<FriendBriefDataItem>> _playerBriefInfoCacheDict;

		private Dictionary<int, CacheData<FriendDetailDataItem>> _playerDetialInfoCacheDict;

		public List<FriendBriefDataItem> askingList;

		private Dictionary<int, FriendBriefDataItem> _askingDict;

		public List<FriendBriefDataItem> recommandedPlayerList;

		public List<FriendBriefDataItem> helperStrangerList;

		private Dictionary<int, DateTime> _helperFrozenMap;

		private HashSet<int> _requestAddPlayerUIDSet;

		public Dictionary<string, FriendSortType> sortTypeMap;

		public Dictionary<FriendSortType, Comparison<FriendBriefDataItem>> sortComparisionMap;

		public FriendModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			friendsList = new List<FriendBriefDataItem>();
			_friendsBriefInfoDict = new Dictionary<int, FriendBriefDataItem>();
			_playerBriefInfoCacheDict = new Dictionary<int, CacheData<FriendBriefDataItem>>();
			_playerDetialInfoCacheDict = new Dictionary<int, CacheData<FriendDetailDataItem>>();
			askingList = new List<FriendBriefDataItem>();
			_askingDict = new Dictionary<int, FriendBriefDataItem>();
			recommandedPlayerList = new List<FriendBriefDataItem>();
			helperStrangerList = new List<FriendBriefDataItem>();
			_helperFrozenMap = new Dictionary<int, DateTime>();
			_requestAddPlayerUIDSet = new HashSet<int>();
			InitForSort();
		}

		private void InitForSort()
		{
			sortTypeMap = new Dictionary<string, FriendSortType>();
			sortComparisionMap = new Dictionary<FriendSortType, Comparison<FriendBriefDataItem>>();
			string[] tAB_KEY = FriendOverviewPageContext.TAB_KEY;
			foreach (string text in tAB_KEY)
			{
				if (text == "FriendListTab")
				{
					sortTypeMap.Add(text, FriendSortType.Friend_NEW);
				}
				else if (text == "RequestListTab")
				{
					sortTypeMap.Add(text, FriendSortType.Request_NEW);
				}
				else
				{
					sortTypeMap.Add(text, FriendSortType.Level_DESC);
				}
			}
			sortComparisionMap.Add(FriendSortType.Friend_NEW, FriendBriefDataItem.CompareToFriendNew);
			sortComparisionMap.Add(FriendSortType.Request_NEW, FriendBriefDataItem.CompareToRequestNew);
			sortComparisionMap.Add(FriendSortType.Level_DESC, FriendBriefDataItem.CompareToLevelDesc);
			sortComparisionMap.Add(FriendSortType.Level_ASC, FriendBriefDataItem.CompareToLevelAsc);
			sortComparisionMap.Add(FriendSortType.Star_DESC, FriendBriefDataItem.CompareToAvatarStarDesc);
			sortComparisionMap.Add(FriendSortType.Star_ASC, FriendBriefDataItem.CompareToAvatarStarAsc);
			sortComparisionMap.Add(FriendSortType.Combat_DESC, FriendBriefDataItem.CompareToAvatarCombatDesc);
			sortComparisionMap.Add(FriendSortType.Combat_ASC, FriendBriefDataItem.CompareToAvatarCombatAsc);
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 73:
				return OnGetPlayerDetailDataRsp(pkt.getData<GetPlayerDetailDataRsp>());
			case 65:
				return OnGetFriendListRsp(pkt.getData<GetFriendListRsp>());
			case 67:
				return OnAddFriendRsp(pkt.getData<AddFriendRsp>());
			case 71:
				return OnGetAskAddFriendListRsp(pkt.getData<GetAskAddFriendListRsp>());
			case 77:
				return OnGetRecommandFriendListRsp(pkt.getData<GetRecommendFriendListRsp>());
			case 80:
				return OnDelFriendNotify(pkt.getData<DelFriendNotify>());
			case 101:
				return OnGetAssistantFrozenListRsp(pkt.getData<GetAssistantFrozenListRsp>());
			default:
				return false;
			}
		}

		private bool OnGetPlayerDetailDataRsp(GetPlayerDetailDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				PlayerDetailData detail = rsp.detail;
				FriendDetailDataItem value = new FriendDetailDataItem(detail);
				_playerDetialInfoCacheDict[(int)detail.uid] = new CacheData<FriendDetailDataItem>(value);
			}
			return false;
		}

		private bool OnGetFriendListRsp(GetFriendListRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (rsp.is_whole_dataSpecified && rsp.is_whole_data)
				{
					_friendsBriefInfoDict.Clear();
					friendsList.Clear();
				}
				for (int i = 0; i < rsp.friend_list.Count; i++)
				{
					PlayerFriendBriefData briefData = rsp.friend_list[i];
					FriendBriefDataItem friendBriefDataItem = new FriendBriefDataItem(briefData);
					if (_friendsBriefInfoDict.ContainsKey(friendBriefDataItem.uid))
					{
						friendsList.Remove(_friendsBriefInfoDict[friendBriefDataItem.uid]);
						friendsList.Insert(0, friendBriefDataItem);
					}
					else
					{
						friendsList.Add(friendBriefDataItem);
					}
					_friendsBriefInfoDict[friendBriefDataItem.uid] = friendBriefDataItem;
					_playerBriefInfoCacheDict[friendBriefDataItem.uid] = new CacheData<FriendBriefDataItem>(friendBriefDataItem);
				}
			}
			return false;
		}

		private bool OnAddFriendRsp(AddFriendRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				int target_uid = (int)rsp.target_uid;
				Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.Remove(target_uid);
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			return false;
		}

		private bool OnDelFriendRsp(DelFriendRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
			}
			return false;
		}

		private bool OnGetAskAddFriendListRsp(GetAskAddFriendListRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				bool flag = _askingDict.Count <= 0;
				for (int i = 0; i < rsp.ask_list.Count; i++)
				{
					PlayerFriendBriefData briefData = rsp.ask_list[i];
					FriendBriefDataItem friendBriefDataItem = new FriendBriefDataItem(briefData);
					if (_askingDict.ContainsKey(friendBriefDataItem.uid))
					{
						askingList.Remove(_askingDict[friendBriefDataItem.uid]);
						askingList.Insert(0, friendBriefDataItem);
					}
					else
					{
						askingList.Add(friendBriefDataItem);
					}
					_askingDict[friendBriefDataItem.uid] = friendBriefDataItem;
					_playerBriefInfoCacheDict[friendBriefDataItem.uid] = new CacheData<FriendBriefDataItem>(friendBriefDataItem);
				}
				if (flag)
				{
					HashSet<int> hashSet = new HashSet<int>();
					foreach (int item in Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet)
					{
						if (_askingDict.ContainsKey(item))
						{
							hashSet.Add(item);
						}
					}
					if (hashSet.Count > 0)
					{
						Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.ExceptWith(hashSet);
						Singleton<MiHoYoGameData>.Instance.Save();
					}
				}
			}
			return false;
		}

		private bool OnGetRecommandFriendListRsp(GetRecommendFriendListRsp rsp)
		{
			recommandedPlayerList.Clear();
			helperStrangerList.Clear();
			for (int i = 0; i < rsp.recommend_list.Count; i++)
			{
				PlayerFriendBriefData val = rsp.recommend_list[i];
				if (!_friendsBriefInfoDict.ContainsKey((int)val.uid))
				{
					FriendBriefDataItem friendBriefDataItem = new FriendBriefDataItem(val);
					if (i < MiscData.Config.BasicConfig.RecommendFriendListNum)
					{
						recommandedPlayerList.Add(friendBriefDataItem);
					}
					else
					{
						helperStrangerList.Add(friendBriefDataItem);
					}
					_playerBriefInfoCacheDict[friendBriefDataItem.uid] = new CacheData<FriendBriefDataItem>(friendBriefDataItem);
				}
			}
			if (helperStrangerList.Count == 0)
			{
				helperStrangerList.AddRange(recommandedPlayerList);
			}
			return false;
		}

		private bool OnDelFriendNotify(DelFriendNotify rsp)
		{
			int target_uid = (int)rsp.target_uid;
			FriendBriefDataItem friendBriefDataItem = null;
			foreach (FriendBriefDataItem friends in friendsList)
			{
				if (friends.uid == target_uid)
				{
					friendBriefDataItem = friends;
					break;
				}
			}
			if (friendBriefDataItem != null)
			{
				friendsList.Remove(friendBriefDataItem);
			}
			_friendsBriefInfoDict.Remove((int)rsp.target_uid);
			return false;
		}

		private bool OnGetAssistantFrozenListRsp(GetAssistantFrozenListRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode != 0)
			{
				return false;
			}
			foreach (AssistantFrozen item in rsp.frozen_list)
			{
				int uid = (int)item.uid;
				DateTime value = TimeUtil.Now.AddSeconds((int)item.left_frozen_time);
				_helperFrozenMap[uid] = value;
			}
			return false;
		}

		public FriendDetailDataItem TryGetFriendDetailInfo(int uid)
		{
			CacheData<FriendDetailDataItem> value;
			_playerDetialInfoCacheDict.TryGetValue(uid, out value);
			if (value != null && value.CacheValid)
			{
				return value.Value;
			}
			return null;
		}

		public void RemoveFriendInfo(FriendOverviewPageContext.FriendTab friendType, int targetUid)
		{
			switch (friendType)
			{
			case FriendOverviewPageContext.FriendTab.FriendListTab:
				_friendsBriefInfoDict.Remove(targetUid);
				friendsList.RemoveAll((FriendBriefDataItem x) => x.uid == targetUid);
				break;
			case FriendOverviewPageContext.FriendTab.AddFriendTab:
				recommandedPlayerList.RemoveAll((FriendBriefDataItem x) => x.uid == targetUid);
				break;
			case FriendOverviewPageContext.FriendTab.RequestListTab:
				_askingDict.Remove(targetUid);
				askingList.RemoveAll((FriendBriefDataItem x) => x.uid == targetUid);
				break;
			}
		}

		public bool IsMyFriend(int targetUid)
		{
			return _friendsBriefInfoDict.ContainsKey(targetUid);
		}

		public FriendBriefDataItem TryGetFriendBriefData(int targetUid)
		{
			if (_playerBriefInfoCacheDict.ContainsKey(targetUid) && _playerBriefInfoCacheDict[targetUid].CacheValid)
			{
				return _playerBriefInfoCacheDict[targetUid].Value;
			}
			foreach (FriendBriefDataItem friends in friendsList)
			{
				if (friends.uid == targetUid)
				{
					return friends;
				}
			}
			return null;
		}

		public FriendDetailDataItem TryGetFriendDetailData(int targetUid)
		{
			if (_playerDetialInfoCacheDict.ContainsKey(targetUid) && _playerDetialInfoCacheDict[targetUid].CacheValid)
			{
				return _playerDetialInfoCacheDict[targetUid].Value;
			}
			return null;
		}

		public string TryGetPlayerNickName(int targetUid)
		{
			if (_playerBriefInfoCacheDict.ContainsKey(targetUid) && _playerBriefInfoCacheDict[targetUid].CacheValid)
			{
				return _playerBriefInfoCacheDict[targetUid].Value.nickName;
			}
			if (_playerDetialInfoCacheDict.ContainsKey(targetUid) && _playerDetialInfoCacheDict[targetUid].CacheValid)
			{
				return _playerDetialInfoCacheDict[targetUid].Value.nickName;
			}
			FriendBriefDataItem friendBriefDataItem = recommandedPlayerList.Find((FriendBriefDataItem x) => x.uid == targetUid);
			if (friendBriefDataItem != null)
			{
				return friendBriefDataItem.nickName;
			}
			return targetUid.ToString();
		}

		public bool isHelperFrozen(int uid)
		{
			DateTime helperNextAvaliableTime = GetHelperNextAvaliableTime(uid);
			return helperNextAvaliableTime > TimeUtil.Now;
		}

		public DateTime GetHelperNextAvaliableTime(int uid)
		{
			if (_helperFrozenMap.ContainsKey(uid))
			{
				return _helperFrozenMap[uid];
			}
			return TimeUtil.Now;
		}

		public FriendBriefDataItem GetOneStrangeHelper()
		{
			FriendBriefDataItem friendBriefDataItem = helperStrangerList.Find((FriendBriefDataItem x) => !isHelperFrozen(x.uid) && !_friendsBriefInfoDict.ContainsKey(x.uid));
			if (friendBriefDataItem == null)
			{
				Singleton<NetworkManager>.Instance.RequestRecommandFriendList();
			}
			return friendBriefDataItem;
		}

		public bool IsOldFriend(int friendUID)
		{
			return Singleton<MiHoYoGameData>.Instance.LocalData.OldFriendUIDSet.Contains(friendUID);
		}

		public void MarkFriendAsOld(int friendUID)
		{
			Singleton<MiHoYoGameData>.Instance.LocalData.OldFriendUIDSet.Add(friendUID);
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		public void MarkAllFriendsAsOld()
		{
			MarkFriendsAsOld(friendsList);
		}

		public void MarkFriendsAsOld(List<FriendBriefDataItem> friends)
		{
			foreach (FriendBriefDataItem friend in friends)
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.OldFriendUIDSet.Add(friend.uid);
			}
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		public bool HasNewFriend()
		{
			foreach (FriendBriefDataItem friends in friendsList)
			{
				if (!IsOldFriend(friends.uid))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsOldRequest(int friendUID)
		{
			return Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.Contains(friendUID);
		}

		public void MarkRequestAsOld(int friendUID)
		{
			Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.Add(friendUID);
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		public void MarkAllRequestsAsOld()
		{
			MarkRequestsAsOld(askingList);
		}

		public void MarkRequestsAsOld(List<FriendBriefDataItem> requests)
		{
			foreach (FriendBriefDataItem request in requests)
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.Add(request.uid);
			}
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		public bool HasNewRequest()
		{
			foreach (FriendBriefDataItem asking in askingList)
			{
				if (!IsOldRequest(asking.uid))
				{
					return true;
				}
			}
			return false;
		}

		public void RecordRequestAddFriend(int friendUID)
		{
			_requestAddPlayerUIDSet.Add(friendUID);
		}

		public bool IsRequestAddOnce(int friendUID)
		{
			return _requestAddPlayerUIDSet.Contains(friendUID);
		}

		public List<FriendBriefDataItem> GetRecommandFriendList()
		{
			List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
			foreach (FriendBriefDataItem recommandedPlayer in recommandedPlayerList)
			{
				if (!_friendsBriefInfoDict.ContainsKey(recommandedPlayer.uid) && !_requestAddPlayerUIDSet.Contains(recommandedPlayer.uid))
				{
					list.Add(recommandedPlayer);
				}
			}
			return list;
		}
	}
}
