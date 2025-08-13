using System;
using System.Collections.Generic;
using UniRx;
using proto;

namespace MoleMole
{
	public class ChatModule : BaseModule
	{
		public List<ChatMsgDataItem> worldChatMsgList;

		private Dictionary<int, List<ChatMsgDataItem>> _friendChatMsgMap;

		public List<ChatMsgDataItem> guildChatMsgList;

		private Dictionary<int, Tuple<bool, int>> _friendNewMsgWeightDic;

		private int _weight;

		public int worldHistoryMsgIndex;

		public int guildHistoryMsgIndex;

		public Dictionary<int, int> friendHistoryMsgIndexDic;

		public int chatRoomId;

		public DateTime lastWorldChatTime;

		private ChatModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			worldChatMsgList = new List<ChatMsgDataItem>();
			guildChatMsgList = new List<ChatMsgDataItem>();
			_friendChatMsgMap = new Dictionary<int, List<ChatMsgDataItem>>();
			_friendNewMsgWeightDic = new Dictionary<int, Tuple<bool, int>>();
			friendHistoryMsgIndexDic = new Dictionary<int, int>();
			chatRoomId = 0;
			_weight = 0;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 7:
				return OnPlayerLoginRsp(pkt.getData<PlayerLoginRsp>());
			case 89:
				return OnEnterWorldChatroomRsp(pkt.getData<EnterWorldChatroomRsp>());
			case 91:
				return OnRecvWorldChatMsgNotify(pkt.getData<RecvWorldChatMsgNotify>());
			case 93:
				return OnRecvFriendChatMsgNotify(pkt.getData<RecvFriendChatMsgNotify>());
			case 94:
				return OnRecvFriendOfflineChatMsgNotify(pkt.getData<RecvFriendOfflineChatMsgNotify>());
			case 97:
				return OnRecvSystemChatMsgNotify(pkt.getData<RecvSystemChatMsgNotify>());
			default:
				return false;
			}
		}

		private bool OnPlayerLoginRsp(PlayerLoginRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Dictionary<int, List<ChatMsgDataItem>> friendChatMsgMap = Singleton<MiHoYoGameData>.Instance.LocalData.FriendChatMsgMap;
				foreach (KeyValuePair<int, List<ChatMsgDataItem>> item in friendChatMsgMap)
				{
					if (!_friendChatMsgMap.ContainsKey(item.Key))
					{
						_friendChatMsgMap.Add(item.Key, new List<ChatMsgDataItem>());
					}
					_friendChatMsgMap[item.Key].AddRange(item.Value);
				}
			}
			foreach (int key in _friendChatMsgMap.Keys)
			{
				if (!friendHistoryMsgIndexDic.ContainsKey(key))
				{
					friendHistoryMsgIndexDic.Add(key, _friendChatMsgMap[key].Count);
				}
			}
			return false;
		}

		public bool IsWorldChatAllowed(ChatMsgDataItem msgItem)
		{
			return IsInWorldChatCD(msgItem.time) && IsInWorldChatLevel();
		}

		private bool IsInWorldChatCD(DateTime chatTime)
		{
			double totalSeconds = (chatTime - lastWorldChatTime).TotalSeconds;
			return totalSeconds >= (double)MiscData.Config.ChatConfig.WorldChatInterval;
		}

		private bool IsInWorldChatLevel()
		{
			return Singleton<PlayerModule>.Instance.playerData.teamLevel >= MiscData.Config.ChatConfig.WorldChatLevelRequirment;
		}

		private bool OnEnterWorldChatroomRsp(EnterWorldChatroomRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				chatRoomId = (int)rsp.chatroom_id;
				worldChatMsgList.Clear();
				foreach (ChatMsg item in rsp.his_chat_msg_list)
				{
					worldChatMsgList.Add(new ChatMsgDataItem(item));
				}
				worldHistoryMsgIndex = worldChatMsgList.Count;
			}
			return false;
		}

		private bool OnRecvWorldChatMsgNotify(RecvWorldChatMsgNotify rsp)
		{
			worldChatMsgList.Add(new ChatMsgDataItem(rsp.chat_msg));
			return false;
		}

		private bool OnRecvFriendChatMsgNotify(RecvFriendChatMsgNotify rsp)
		{
			AddFriendChatMsg(new ChatMsgDataItem(rsp.chat_msg));
			return false;
		}

		private bool OnRecvFriendOfflineChatMsgNotify(RecvFriendOfflineChatMsgNotify rsp)
		{
			foreach (ChatMsg item in rsp.chat_msg_list)
			{
				AddFriendChatMsg(new ChatMsgDataItem(item));
			}
			return false;
		}

		private bool OnRecvSystemChatMsgNotify(RecvSystemChatMsgNotify rsp)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			SystemChatMsgType type = rsp.chat_msg.type;
			if ((int)type == 1)
			{
				worldChatMsgList.Add(new ChatMsgDataItem(rsp.chat_msg));
			}
			return false;
		}

		private void AddFriendChatMsg(ChatMsgDataItem msgData)
		{
			if (!_friendChatMsgMap.ContainsKey(msgData.uid))
			{
				_friendChatMsgMap.Add(msgData.uid, new List<ChatMsgDataItem>());
			}
			_friendChatMsgMap[msgData.uid].Add(msgData);
			SaveLocalFriendChatMsg();
			SetFriendMsgNew(msgData);
		}

		private void SetFriendMsgNew(ChatMsgDataItem msgData)
		{
			if (!_friendNewMsgWeightDic.ContainsKey(msgData.uid))
			{
				_friendNewMsgWeightDic.Add(msgData.uid, Tuple.Create(true, _weight++));
			}
			_friendNewMsgWeightDic[msgData.uid] = Tuple.Create(true, _weight++);
		}

		public void SetFriendMsgRead(int uid)
		{
			if (_friendNewMsgWeightDic.ContainsKey(uid))
			{
				_friendNewMsgWeightDic[uid] = Tuple.Create(false, _friendNewMsgWeightDic[uid].Item2);
			}
		}

		public bool GetFriendMsgNewState(int uid)
		{
			if (!_friendNewMsgWeightDic.ContainsKey(uid))
			{
				return false;
			}
			return _friendNewMsgWeightDic[uid].Item1;
		}

		public bool IsFriendChatListHasNewMsg()
		{
			foreach (KeyValuePair<int, Tuple<bool, int>> item in _friendNewMsgWeightDic)
			{
				if (item.Value.Item1)
				{
					return true;
				}
			}
			return false;
		}

		public List<ChatMsgDataItem> GetFriendChatMsgList(int friendId)
		{
			List<ChatMsgDataItem> result = new List<ChatMsgDataItem>();
			if (friendId <= 0 || !_friendChatMsgMap.ContainsKey(friendId))
			{
				return result;
			}
			return _friendChatMsgMap[friendId];
		}

		public int GetFriendChatCount()
		{
			int num = 0;
			foreach (int key in _friendNewMsgWeightDic.Keys)
			{
				if (Singleton<FriendModule>.Instance.IsMyFriend(key))
				{
					num++;
				}
			}
			return num;
		}

		public List<int> GetSortedChatFriendList()
		{
			List<int> list = new List<int>();
			foreach (int key in _friendNewMsgWeightDic.Keys)
			{
				if (!list.Contains(key))
				{
					list.Add(key);
				}
			}
			list.Sort(MostRecentOrderCompare);
			return list;
		}

		public int MostRecentOrderCompare(int uidA, int uidB)
		{
			int item = _friendNewMsgWeightDic[uidA].Item2;
			int item2 = _friendNewMsgWeightDic[uidB].Item2;
			return item2 - item;
		}

		public void AddFriendChatMsgByMySelf(ChatMsgDataItem msgData, int friendId, bool needSave = true)
		{
			if (!_friendChatMsgMap.ContainsKey(friendId))
			{
				_friendChatMsgMap.Add(friendId, new List<ChatMsgDataItem>());
			}
			_friendChatMsgMap[friendId].Add(msgData);
			if (!_friendNewMsgWeightDic.ContainsKey(friendId))
			{
				_friendNewMsgWeightDic.Add(friendId, Tuple.Create(false, 0));
			}
			else
			{
				_friendNewMsgWeightDic[friendId] = Tuple.Create(false, _friendNewMsgWeightDic[friendId].Item2);
			}
			if (needSave)
			{
				SaveLocalFriendChatMsg();
			}
		}

		private void SaveLocalFriendChatMsg()
		{
			int cacheOfflineChatMsgMaxNum = MiscData.Config.ChatConfig.CacheOfflineChatMsgMaxNum;
			Dictionary<int, List<ChatMsgDataItem>> friendChatMsgMap = Singleton<MiHoYoGameData>.Instance.LocalData.FriendChatMsgMap;
			foreach (KeyValuePair<int, List<ChatMsgDataItem>> item in _friendChatMsgMap)
			{
				List<ChatMsgDataItem> value = ((item.Value.Count <= cacheOfflineChatMsgMaxNum) ? item.Value : item.Value.GetRange(item.Value.Count - cacheOfflineChatMsgMaxNum, cacheOfflineChatMsgMaxNum));
				friendChatMsgMap[item.Key] = value;
			}
			Singleton<MiHoYoGameData>.Instance.Save();
		}
	}
}
