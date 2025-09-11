using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class NetworkManager
	{
		private const float DISPATCH_CONNECT_TIMEOUT_SECOND = 3f;

		private const int QUEUE_CAPACITY = 20;

		public readonly ConfigChannel channelConfig;

		private List<string> _globalDispatchUrlList;

		private MiClient _client;

		private MonoClientPacketConsumer _clientPacketConsumer;

		private Dictionary<int, DateTime> _lastRequestTimeDict;

		private Dictionary<int, float> _requestMinIntervalDict;

		public readonly ProtobufSerializer serializer;

		public bool alreadyLogin;

		public uint loginRandomNum;

		private string _uuid;

		private Queue<NetPacketV1> _packetSendQueue;

		private Dictionary<uint, bool> _CMD_SHOULD_ENQUEUE_MAP;

		public GlobalDispatchDataItem GlobalDispatchData { get; private set; }

		public DispatchServerDataItem DispatchSeverData { get; set; }

		public NetworkManager()
		{
			_client = new MiClient();
			Singleton<CommandMap>.Create();
			_packetSendQueue = new Queue<NetPacketV1>();
			InitClientPacketConsumer();
			serializer = new ProtobufSerializer();
			alreadyLogin = false;
			channelConfig = ConfigUtil.LoadJSONConfig<ConfigChannel>("DataPersistent/BuildChannel/ChannelConfig");
			_globalDispatchUrlList = channelConfig.DispatchUrls;
			GlobalVars.DataUseAssetBundle = channelConfig.DataUseAssetBundle;
			GlobalVars.ResourceUseAssetBundle = channelConfig.EventUseAssetBundle;
			_lastRequestTimeDict = new Dictionary<int, DateTime>();
			SetupRequestMinIntervalDict();
		}

		public void SendPacketsOnLoginSuccess(bool forceAll = false, uint serverProcessedPacketId = 0)
		{
			if (forceAll || !alreadyLogin)
			{
				RequestConfigData();
				RequestGetAllEquipmentData();
				RequestGetAllAvatarData();
				RequestGetAvatarTeamData();
				RequestRecommandFriendList();
				RequestHasGotItemIdList();
				RequestGetBulletin();
				RequestIslandVenture();
				RequestGetIsland();
				RequestGetCollectCabin();
				RequestGetFinishGuideData();
				RequestGetAllLevelData();
				RequestGetVipRewardData();
				RequestGetShopList();
				RequestGachaDisplayInfo();
				RequestGetMailData();
			}
			RequestGetAllMainData();
			RequestGetAllWeekDayActivityData();
			RequestGetAssistantFrozenList();
			RequestEnterWorldChatroom();
			RequestFriendList();
			RequestAskAddFriendList();
			RequestRecommandFriendList();
			RequestGetMissionData();
			RequestGetInviteeFriend();
			RequestGetInviteFriend();
			if (alreadyLogin && serverProcessedPacketId != 0)
			{
				SendQueuePacketWhenReconnected(serverProcessedPacketId);
			}
		}

		public void RequestPlayerToken()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected I4, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetPlayerTokenRsp>();
				return;
			}
			AccountType accountType = Singleton<AccountManager>.Instance.manager.AccountType;
			string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
			string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
			string accountExt = Singleton<AccountManager>.Instance.manager.AccountExt;
			GetPlayerTokenReq val2;
			if (!string.IsNullOrEmpty(accountUid))
			{
				GetPlayerTokenReq val = new GetPlayerTokenReq();
				val.account_type = (uint)(int)accountType;
				val.account_uid = accountUid;
				val.account_token = accountToken;
				val.account_ext = accountExt;
				val.token = string.Empty;
				val2 = val;
			}
			else
			{
				val2 = GetTestPlayerTokenReq();
			}
			val2.version = GetGameVersion();
			val2.device = SystemInfo.deviceModel;
			val2.system_info = SystemInfo.operatingSystem;
			SendPacket<GetPlayerTokenReq>(val2);
		}

		public void RequestBindAccount()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected I4, but got Unknown
			AccountType accountType = Singleton<AccountManager>.Instance.manager.AccountType;
			string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
			string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
			string accountExt = Singleton<AccountManager>.Instance.manager.AccountExt;
			if (!string.IsNullOrEmpty(accountUid) && !string.IsNullOrEmpty(accountToken))
			{
				BindAccountReq val = new BindAccountReq();
				val.account_type = (uint)(int)accountType;
				val.account_uid = accountUid;
				val.account_token = accountToken;
				val.account_ext = accountExt;
				BindAccountReq data = val;
				SendPacket<BindAccountReq>(data);
			}
		}

		public void RequestPlayerLogin()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<PlayerLoginRsp>();
				return;
			}
			loginRandomNum = Singleton<MiHoYoGameData>.Instance.LocalData.LoginRandomNum;
			if (loginRandomNum == 0)
			{
				loginRandomNum = GeneralLoginRandomNum();
				Singleton<MiHoYoGameData>.Instance.LocalData.LoginRandomNum = loginRandomNum;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			PlayerLoginReq val = new PlayerLoginReq();
			val.login_random_num = loginRandomNum;
			val.last_server_packet_id = _clientPacketConsumer.lastServerPacketId;
			if (Singleton<AccountManager>.Instance.apkCommentInfo != null)
			{
				string cps = Singleton<AccountManager>.Instance.apkCommentInfo.cps;
				if (!string.IsNullOrEmpty(cps))
				{
					val.cps = cps;
				}
				string checksum = Singleton<AccountManager>.Instance.apkCommentInfo.checksum;
				if (!string.IsNullOrEmpty(checksum))
				{
					val.check_sum = checksum;
				}
			}
			if (string.IsNullOrEmpty(_uuid))
			{
				_uuid = GetPersistentUUID();
			}
			val.device_uuid = _uuid;
			val.android_signatures = Singleton<AccountManager>.Instance.apkSignature;
			SendPacket<PlayerLoginReq>(val);
		}

		public void RequestConfigData()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetConfigReq data = new GetConfigReq();
			SendPacket<GetConfigReq>(data);
		}

		public void RequestGetAllMainData()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetMainDataRsp>();
				return;
			}
			GetMainDataReq val = new GetMainDataReq();
			val.type_list.Add((GetMainDataReq.DataType)0);
			SendPacket<GetMainDataReq>(val);
		}

		public void RequestGetStaminaRecoverLeftTime()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetMainDataRsp>();
				return;
			}
			GetMainDataReq val = new GetMainDataReq();
			val.type_list.Add((GetMainDataReq.DataType)7);
			SendPacket<GetMainDataReq>(val);
		}

		public void RequestGetSkillPointRecoverLeftTime()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetMainDataRsp>();
				return;
			}
			GetMainDataReq val = new GetMainDataReq();
			val.type_list.Add((GetMainDataReq.DataType)10);
			SendPacket<GetMainDataReq>(val);
		}

		public void RequestGetAllAvatarData()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetAvatarDataRsp>(FakePacketHelper.GetFakeAvatarDataRsp());
				return;
			}
			GetAvatarDataReq val = new GetAvatarDataReq();
			val.avatar_id_list.Add(0u);
			SendPacket<GetAvatarDataReq>(val);
		}

		public void RequestGetAllLevelData()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetStageDataRsp>(FakePacketHelper.GetFakeStageDataRsp());
				return;
			}
			GetStageDataReq val = new GetStageDataReq();
			val.stage_id_list.Add(0u);
			SendPacket<GetStageDataReq>(val);
		}

		public void RequestGetAllWeekDayActivityData()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetWeekDayActivityDataReq data = new GetWeekDayActivityDataReq();
			SendPacket<GetWeekDayActivityDataReq>(data);
		}

		public void RequestGetAvatarTeamData()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetAvatarTeamDataRsp>(FakePacketHelper.GetFakeGetAvatarTeamDataRsp());
			}
			else
			{
				SendPacket<GetAvatarTeamDataReq>(new GetAvatarTeamDataReq());
			}
		}

		public void RequestGetScoinExchangeInfo()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetScoinExchangeInfoRsp>();
			}
			else
			{
				SendPacket<GetScoinExchangeInfoReq>(new GetScoinExchangeInfoReq());
			}
		}

		public void RequestScoinExchange()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<ScoinExchangeRsp>();
			}
			else
			{
				SendPacket<ScoinExchangeReq>(new ScoinExchangeReq());
			}
		}

		public void RequestGetStaminaExchangeInfo()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetStaminaExchangeInfoRsp>();
			}
			else
			{
				SendPacket<GetStaminaExchangeInfoReq>(new GetStaminaExchangeInfoReq());
			}
		}

		public void RequestStaminaExchange()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<StaminaExchangeRsp>();
			}
			else
			{
				SendPacket<StaminaExchangeReq>(new StaminaExchangeReq());
			}
		}

		public void RequestGetSkillPointExchangeInfo()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetSkillPointExchangeInfoRsp>();
			}
			else
			{
				SendPacket<GetSkillPointExchangeInfoReq>(new GetSkillPointExchangeInfoReq());
			}
		}

		public void RequestSkillPointExchange()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<SkillPointExchangeRsp>();
			}
			else
			{
				SendPacket<SkillPointExchangeReq>(new SkillPointExchangeReq());
			}
		}

		public void RequestNicknameChange(string newName)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			NicknameModifyReq val = new NicknameModifyReq();
			val.nickname = newName;
			SendPacket<NicknameModifyReq>(val);
		}

		public void RequestSelfDescChange(string desc)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			SetSelfDescReq val = new SetSelfDescReq();
			val.self_desc = desc;
			SendPacket<SetSelfDescReq>(val);
		}

		public void RequestEquipmentPowerUp(StorageDataItemBase mainItem, List<StorageDataItemBase> consumeItemList)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<EquipmentPowerUpRsp>();
				return;
			}
			EquipmentPowerUpReq val = new EquipmentPowerUpReq();
			EquipmentItem val2 = new EquipmentItem();
			SetEquipmentItemByStorageDataItem(val2, mainItem);
			val.main_item = val2;
			EquipmentItemList val3 = new EquipmentItemList();
			foreach (StorageDataItemBase consumeItem in consumeItemList)
			{
				EquipmentItem val4 = new EquipmentItem();
				SetEquipmentItemByStorageDataItem(val4, consumeItem);
				val3.item_list.Add(val4);
			}
			val.consume_item_list = val3;
			SendPacket<EquipmentPowerUpReq>(val);
		}

		private void SetEquipmentItemByStorageDataItem(EquipmentItem equipmentItem, StorageDataItemBase storageDataItem)
		{
			if (storageDataItem.GetType() == typeof(WeaponDataItem))
			{
				equipmentItem.type = (EquipmentType)3;
				equipmentItem.id_or_unique_id = (uint)storageDataItem.uid;
			}
			if (storageDataItem.GetType() == typeof(StigmataDataItem))
			{
				equipmentItem.type = (EquipmentType)4;
				equipmentItem.id_or_unique_id = (uint)storageDataItem.uid;
			}
			if (storageDataItem.GetType() == typeof(MaterialDataItem))
			{
				equipmentItem.type = (EquipmentType)1;
				equipmentItem.id_or_unique_id = (uint)storageDataItem.ID;
				equipmentItem.num = (uint)storageDataItem.number;
			}
		}

		public void RequestEquipmentSell(List<StorageDataItemBase> storageItemList)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			EquipmentItemList val = new EquipmentItemList();
			List<AvatarFragment> list = new List<AvatarFragment>();
			foreach (StorageDataItemBase storageItem in storageItemList)
			{
				if (storageItem is AvatarFragmentDataItem)
				{
					AvatarFragment val2 = new AvatarFragment();
					val2.fragment_id = (uint)storageItem.ID;
					val2.num = (uint)storageItem.number;
					list.Add(val2);
				}
				else
				{
					EquipmentItem val3 = new EquipmentItem();
					SetEquipmentItemByStorageDataItem(val3, storageItem);
					val.item_list.Add(val3);
				}
			}
			if (val.item_list.Count > 0)
			{
				EquipmentSellReq val4 = new EquipmentSellReq();
				val4.sell_item_list = val;
				SendPacket<EquipmentSellReq>(val4);
			}
			if (list.Count > 0)
			{
				SellAvatarFragmentReq val5 = new SellAvatarFragmentReq();
				val5.fragment_list.AddRange(list);
				SendPacket<SellAvatarFragmentReq>(val5);
			}
		}

		public void RequestGetAllEquipmentData()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetEquipmentDataRsp>(FakePacketHelper.GetFakeEquipmentDataRsp());
				return;
			}
			GetEquipmentDataReq val = new GetEquipmentDataReq();
			val.weapon_unique_id_list.Add(0u);
			val.stigmata_unique_id_list.Add(0u);
			val.material_id_list.Add(0u);
			SendPacket<GetEquipmentDataReq>(val);
		}

		public void RequestGetSpecifiedEquipmentData(List<uint> weaponUIDList, List<uint> stigmataUIDList, List<uint> materialIDList)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetEquipmentDataReq val = new GetEquipmentDataReq();
			foreach (uint weaponUID in weaponUIDList)
			{
				val.weapon_unique_id_list.Add(weaponUID);
			}
			foreach (uint stigmataUID in stigmataUIDList)
			{
				val.stigmata_unique_id_list.Add(stigmataUID);
			}
			foreach (uint materialID in materialIDList)
			{
				val.material_id_list.Add(materialID);
			}
			SendPacket<GetEquipmentDataReq>(val);
		}

		public void RequestAvatarStarUp(int m_avatarId)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetAvatarDataRsp>();
				SendFakePacket<AvatarStarUpRsp>();
			}
			else
			{
				AvatarStarUpReq val = new AvatarStarUpReq();
				val.avatar_id = (uint)m_avatarId;
				SendPacket<AvatarStarUpReq>(val);
			}
		}

		public void RequestEquipmentEvo(List<StorageDataItemBase> resourceList, StorageDataItemBase mainItem)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			EquipmentEvoReq val = new EquipmentEvoReq();
			EquipmentItem val2 = new EquipmentItem();
			SetEquipmentItemByStorageDataItem(val2, mainItem);
			val.main_item = val2;
			EquipmentItemList val3 = new EquipmentItemList();
			foreach (StorageDataItemBase resource in resourceList)
			{
				EquipmentItem val4 = new EquipmentItem();
				SetEquipmentItemByStorageDataItem(val4, resource);
				val3.item_list.Add(val4);
			}
			val.consume_item_list = val3;
			SendPacket<EquipmentEvoReq>(val);
		}

		public void RequestDressEquipmentReq(int avatarID, StorageDataItemBase dataItem, EquipmentSlot slot)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			DressEquipmentReq val = new DressEquipmentReq();
			val.avatar_id = (uint)avatarID;
			val.slot = slot;
			val.unique_id = ((dataItem != null) ? ((uint)dataItem.uid) : 0u);
			SendPacket<DressEquipmentReq>(val);
		}

		public void RequestAddAvatarExpByMaterial(int avatarID, int materialID, int num)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AddAvatarExpByMaterialReq val = new AddAvatarExpByMaterialReq();
			val.avatar_id = (uint)avatarID;
			val.material_id = (uint)materialID;
			val.material_num = (uint)num;
			SendPacket<AddAvatarExpByMaterialReq>(val);
		}

		public void RequestLevelBeginReq(LevelDataItem level, int helperUid = 0)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<StageBeginRsp>();
				return;
			}
			StageBeginReq val = new StageBeginReq();
			val.stage_id = (uint)level.levelId;
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType);
			foreach (int item in memberList)
			{
				val.avatar_id_list.Add((uint)item);
			}
			if (helperUid != 0)
			{
				val.assistant_uid = (uint)helperUid;
			}
			SendPacket<StageBeginReq>(val);
		}

		public void RequestEndlessLevelBeginReq()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			EndlessStageBeginReq val = new EndlessStageBeginReq();
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)4);
			foreach (int item in memberList)
			{
				val.avatar_id_list.Add((uint)item);
			}
			SendPacket<EndlessStageBeginReq>(val);
		}

		public void RequestLevelEndReq(int levelId, StageEndStatus status, int scoinReward, int avatarExpReward, List<int> challengeIndexList, List<DropItem> drops, List<StageCheatData> cheatDataList, bool hashChanged, string signKey)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<StageEndRsp>(FakePacketHelper.GetFakeStageEndRsp());
				return;
			}
			StageEndReqBody val = new StageEndReqBody();
			val.stage_id = (uint)levelId;
			val.end_status = status;
			val.scoin_reward = (uint)scoinReward;
			val.avatar_exp_reward = (uint)avatarExpReward;
			foreach (int challengeIndex in challengeIndexList)
			{
				val.challenge_index_list.Add((uint)challengeIndex);
			}
			val.drop_item_list.AddRange(drops);
			if (cheatDataList != null)
			{
				val.cheat_data_list.AddRange(cheatDataList);
			}
			val.is_hash_changed = hashChanged;
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.SetLength(0L);
			memoryStream.Position = 0L;
			Singleton<NetworkManager>.Instance.serializer.Serialize(memoryStream, val);
			byte[] body = memoryStream.ToArray();
			byte[] bytes = Encoding.Default.GetBytes(signKey);
			memoryStream.Write(bytes, 0, bytes.Length);
			byte[] bytes2 = memoryStream.ToArray();
			StageEndReq val2 = new StageEndReq();
			val2.body = body;
			val2.sign = SecurityUtil.SHA256(bytes2);
			SendPacket<StageEndReq>(val2);
			Singleton<LevelModule>.Instance.SaveLevelEndReqInfo(val2);
		}

		public void RequestEndlessFloorEndReq(StageEndStatus status, List<DropItem> drops, List<EndlessAvatarHp> avatarHPList, bool hashChanged)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			EndlessStageEndReq val = new EndlessStageEndReq();
			val.end_status = status;
			val.drop_item_list.AddRange(drops);
			val.avatar_hp_list.AddRange(avatarHPList);
			val.is_hash_changed = hashChanged;
			SendPacket<EndlessStageEndReq>(val);
		}

		public void NotifyUpdateAvatarTeam(StageType levelType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected I4, but got Unknown
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			UpdateAvatarTeamNotify val = new UpdateAvatarTeamNotify();
			val.team = new AvatarTeam();
			val.team.stage_type = (uint)(int)levelType;
			foreach (int member in Singleton<PlayerModule>.Instance.playerData.GetMemberList(levelType))
			{
				val.team.avatar_id_list.Add((uint)member);
			}
			SendPacket<UpdateAvatarTeamNotify>(val);
		}

		public void RequestAvatarSubSkillLevelUp(int avatarID, int skillID, int subSkillID)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AvatarSubSkillLevelUpReq val = new AvatarSubSkillLevelUpReq();
			val.avatar_id = (uint)avatarID;
			val.skill_id = (uint)skillID;
			val.sub_skill_id = (uint)subSkillID;
			SendPacket<AvatarSubSkillLevelUpReq>(val);
		}

		public void RequestGachaDisplayInfo()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetGachaDisplayRsp>(FakePacketHelper.GetFakeGetGachaDisplayRsp());
				return;
			}
			GetGachaDisplayReq data = new GetGachaDisplayReq();
			SendPacket<GetGachaDisplayReq>(data);
		}

		public void RequestGacha(GachaType type, int num)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GachaRsp>();
				return;
			}
			GachaReq val = new GachaReq();
			val.type = type;
			val.num = (uint)num;
			SendPacket<GachaReq>(val);
		}

		public void RequestChapterDropList(ChapterDataItem chapter)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetStageDropDisplayReq val = new GetStageDropDisplayReq();
			foreach (LevelDataItem allLevel in chapter.GetAllLevelList())
			{
				val.stage_id_list.Add((uint)allLevel.levelId);
			}
			SendPacket<GetStageDropDisplayReq>(val);
		}

		public void RequestLevelDropList(List<uint> levelIDList)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetStageDropDisplayReq val = new GetStageDropDisplayReq();
			val.stage_id_list.AddRange(levelIDList);
			SendPacket<GetStageDropDisplayReq>(val);
		}

		public void RequestChangeEquipmentProtectdStatus(StorageDataItemBase storageDataItem)
		{
			if (!(storageDataItem is MaterialDataItem))
			{
				List<WeaponDataItem> list = new List<WeaponDataItem>();
				List<StigmataDataItem> list2 = new List<StigmataDataItem>();
				if (storageDataItem is WeaponDataItem)
				{
					list.Add(storageDataItem as WeaponDataItem);
				}
				else
				{
					list2.Add(storageDataItem as StigmataDataItem);
				}
				RequestUpdateEquipmentProtectdStatus(list, list2, !storageDataItem.isProtected);
			}
		}

		public void RequestIdentifyStigmataAffix(StorageDataItemBase storageItem)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			IdentifyStigmataAffixReq val = new IdentifyStigmataAffixReq();
			val.unique_id = (uint)storageItem.uid;
			SendPacket<IdentifyStigmataAffixReq>(val);
		}

		public void RequestUpdateEquipmentProtectdStatus(List<WeaponDataItem> weaponList, List<StigmataDataItem> stigmataList, bool isProtected)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			UpdateEquipmentProtectedStatusReq val = new UpdateEquipmentProtectedStatusReq();
			if (weaponList != null)
			{
				foreach (WeaponDataItem weapon in weaponList)
				{
					val.weapon_unique_id_list.Add((uint)weapon.uid);
				}
			}
			if (stigmataList != null)
			{
				foreach (StigmataDataItem stigmata in stigmataList)
				{
					val.stigmata_unique_id_list.Add((uint)stigmata.uid);
				}
			}
			val.is_protected = isProtected;
			SendPacket<UpdateEquipmentProtectedStatusReq>(val);
		}

		public void RequestFriendDetailInfo(int uid)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetPlayerDetailDataRsp>(FakePacketHelper.GetFakePlayerDetailDataRsp((uint)uid));
				return;
			}
			GetPlayerDetailDataReq val = new GetPlayerDetailDataReq();
			val.target_uid = (uint)uid;
			SendPacket<GetPlayerDetailDataReq>(val);
		}

		public void RequestAddFriend(int targetUid)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AddFriendReq val = new AddFriendReq();
			val.action = (AddFriendAction)1;
			val.target_uid = (uint)targetUid;
			SendPacket<AddFriendReq>(val);
		}

		public void RequestAgreeFriend(int targetUid)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AddFriendReq val = new AddFriendReq();
			val.action = (AddFriendAction)2;
			val.target_uid = (uint)targetUid;
			SendPacket<AddFriendReq>(val);
		}

		public void RequestRejectFriend(int targetUid)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AddFriendReq val = new AddFriendReq();
			val.action = (AddFriendAction)3;
			val.target_uid = (uint)targetUid;
			SendPacket<AddFriendReq>(val);
		}

		public void RequestDelFriend(int targetUid)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			DelFriendReq val = new DelFriendReq();
			val.target_uid = (uint)targetUid;
			SendPacket<DelFriendReq>(val);
		}

		public void RequestAskAddFriendList()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetAskAddFriendListRsp>(FakePacketHelper.GetFakeAskAddFriendListRsp());
				return;
			}
			GetAskAddFriendListReq data = new GetAskAddFriendListReq();
			SendPacket<GetAskAddFriendListReq>(data);
		}

		public void RequestFriendList()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetFriendListRsp>(FakePacketHelper.GetFakeFriendListRsp());
				return;
			}
			GetFriendListReq data = new GetFriendListReq();
			SendPacket<GetFriendListReq>(data);
		}

		public void RequestRecommandFriendList()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetRecommendFriendListRsp>(FakePacketHelper.GetFakeRecommendFriendListRsp());
				return;
			}
			GetRecommendFriendListReq data = new GetRecommendFriendListReq();
			SendPacket<GetRecommendFriendListReq>(data);
		}

		public void RequestSetFriendDesc(string desc)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			SetSelfDescReq val = new SetSelfDescReq();
			val.self_desc = desc;
			SendPacket<SetSelfDescReq>(val);
		}

		public void RequestGetMailData()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetMailDataRsp>(FakePacketHelper.GetFakeMailDataRsp());
				return;
			}
			GetMailDataReq data = new GetMailDataReq();
			SendPacket<GetMailDataReq>(data);
		}

		public void RequestGetAllMailAttachment()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetMailAttachmentReq data = new GetMailAttachmentReq();
			SendPacket<GetMailAttachmentReq>(data);
		}

		public void RequestGetOneMailAttachment(MailDataItem mailData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			MailKey val = new MailKey();
			val.id = (uint)mailData.ID;
			val.type = mailData.type;
			GetMailAttachmentReq val2 = new GetMailAttachmentReq();
			val2.mail_key_list.Add(val);
			SendPacket<GetMailAttachmentReq>(val2);
		}

		public void RequestEnterWorldChatroom(int chatRoomId = 0)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			EnterWorldChatroomReq val = new EnterWorldChatroomReq();
			val.chatroom_id = (uint)chatRoomId;
			SendPacket<EnterWorldChatroomReq>(val);
		}

		public void NotifySendWorldChatMsg(string message)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			SendWorldChatMsgNotify val = new SendWorldChatMsgNotify();
			val.msg = message;
			SendPacket<SendWorldChatMsgNotify>(val);
		}

		public void NotifySendGuildChatMsg(string message)
		{
		}

		public void NotifySendFriendChatMsg(int targetUid, string message)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			SendFriendChatMsgNotify val = new SendFriendChatMsgNotify();
			val.msg = message;
			val.target_uid = (uint)targetUid;
			SendPacket<SendFriendChatMsgNotify>(val);
		}

		public void RequestGetAssistantFrozenList()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetAssistantFrozenListReq data = new GetAssistantFrozenListReq();
			SendPacket<GetAssistantFrozenListReq>(data);
		}

		public void RequestHasGotItemIdList()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetHasGotItemIdListReq data = new GetHasGotItemIdListReq();
			SendPacket<GetHasGotItemIdListReq>(data);
		}

		public void RequestAvatarRevive(bool is_retry = false)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<AvatarReviveRsp>(FakePacketHelper.GetAvatarReviveRsp());
				return;
			}
			AvatarReviveReq val = new AvatarReviveReq();
			val.is_retry = is_retry;
			SendPacket<AvatarReviveReq>(val);
		}

		public void RequestStageEnterTimes(uint _stageId)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			ResetStageEnterTimesReq val = new ResetStageEnterTimesReq();
			val.stage_id = _stageId;
			SendPacket<ResetStageEnterTimesReq>(val);
		}

		public void RequestGetMissionData()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetMissionDataReq data = new GetMissionDataReq();
			SendPacket<GetMissionDataReq>(data);
		}

		public void RequestGetMissionReward(uint missionId)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetMissionRewardReq val = new GetMissionRewardReq();
			val.mission_id = missionId;
			SendPacket<GetMissionRewardReq>(val);
		}

		public void RequestUpdateMissionProgress(MissionFinishWay finishWay, uint finishParaInt, string finishParaStr, uint progressAdd)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			UpdateMissionProgressReq val = new UpdateMissionProgressReq();
			val.finish_way = finishWay;
			val.finish_para = finishParaInt;
			val.finish_para_str = finishParaStr;
			val.progress_add = progressAdd;
			SendPacket<UpdateMissionProgressReq>(val);
		}

		public void RequestGetFinishGuideData()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				SendFakePacket<GetFinishGuideDataRsp>(FakePacketHelper.GetFakeFinishGuideDataRsp());
				return;
			}
			GetFinishGuideDataReq data = new GetFinishGuideDataReq();
			SendPacket<GetFinishGuideDataReq>(data);
		}

		public void RequestFinishGuideReport(uint guideID, bool isForceFinish)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			FinishGuideReportReq val = new FinishGuideReportReq();
			List<uint> list = new List<uint>();
			list.Add(guideID);
			val.guide_id_list.Clear();
			val.guide_id_list.AddRange(list);
			val.is_force_finish = isForceFinish;
			SendPacket<FinishGuideReportReq>(val);
		}

		public void RequestFinishGuideReport(List<int> guideIDList, bool isForceFinish)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			FinishGuideReportReq val = new FinishGuideReportReq();
			List<uint> list = new List<uint>();
			foreach (int guideID in guideIDList)
			{
				list.Add((uint)guideID);
			}
			val.guide_id_list.Clear();
			val.guide_id_list.AddRange(list);
			val.is_force_finish = isForceFinish;
			SendPacket<FinishGuideReportReq>(val);
		}

		public void RequestStageInnerDataReport(List<AvatarStastics> avatarInfoList, List<MonsterStastics> monsterInfoList, PlayerStastics playerData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Expected O, but got Unknown
			StageInnerDataReportReq val = new StageInnerDataReportReq();
			foreach (AvatarStastics avatarInfo in avatarInfoList)
			{
				StageInnerAvatarData val2 = new StageInnerAvatarData();
				val2.avatar_id = (uint)avatarInfo.avatarID;
				val2.stage_id = (uint)avatarInfo.stageID;
				val2.avatar_level = (uint)avatarInfo.avatarLevel;
				val2.avatar_star = (uint)avatarInfo.avatarStar;
				val2.total_output = (uint)Mathf.FloorToInt(avatarInfo.avatarDamage);
				val2.no_restrict_output = (uint)Mathf.FloorToInt(avatarInfo.normalDamage);
				val2.do_restrict_output = (uint)Mathf.FloorToInt(avatarInfo.restrictionDamage);
				val2.be_restrict_output = (uint)Mathf.FloorToInt(avatarInfo.beRestrictedDamage);
				val2.total_input = (uint)Mathf.FloorToInt(avatarInfo.avatarBeDamaged);
				val2.battle_time = (uint)Mathf.FloorToInt(avatarInfo.battleTime);
				val2.total_time = (uint)Mathf.FloorToInt(avatarInfo.onStageTime);
				val2.enter_times = (uint)avatarInfo.swapInTimes;
				val2.leave_times = (uint)avatarInfo.swapOutTimes;
				val2.do_break_times = (uint)avatarInfo.avatarBreakTimes;
				val2.be_break_times = (uint)avatarInfo.avatarBeingBreakTimes;
				val2.do_hit_times = (uint)avatarInfo.avatarHitTimes;
				val2.be_hit_times = (uint)avatarInfo.avatarBeingHitTimes;
				val2.exskill_times = (uint)avatarInfo.avatarSkill02Times;
				val2.evade_times = (uint)avatarInfo.avatarEvadeTimes;
				val2.evade_success_times = (uint)avatarInfo.avatarEvadeSuccessTimes;
				val2.evade_effect_times = (uint)avatarInfo.avatarEvadeEffectTimes;
				val2.attack_sp_recover = (uint)Mathf.FloorToInt(avatarInfo.selfSPRecover);
				val2.total_sp_recover = (uint)Mathf.FloorToInt(avatarInfo.SpRecover);
				val2.dps = (uint)Mathf.FloorToInt(avatarInfo.dps);
				val2.special_attack_times = (uint)Mathf.FloorToInt((int)avatarInfo.avatarSpecialAttackTimes);
				val2.weapon_active_skill = (uint)Mathf.FloorToInt(avatarInfo.avatarActiveWeaponSkillDamage);
				val.avatar_list.Add(val2);
			}
			foreach (MonsterStastics monsterInfo in monsterInfoList)
			{
				StageInnerMonsterData val3 = new StageInnerMonsterData();
				val3.monster_name = monsterInfo.key.monsterName;
				val3.monster_type = monsterInfo.key.configType;
				val3.monster_level = (uint)monsterInfo.key.level;
				val3.monster_num = (uint)monsterInfo.monsterCount;
				val3.avg_output = (uint)Mathf.FloorToInt(monsterInfo.damage);
				val3.avg_live_time = (uint)Mathf.FloorToInt(monsterInfo.aliveTime);
				val3.dps = (uint)Mathf.FloorToInt(monsterInfo.dps);
				val3.hit_avatar_times = (uint)monsterInfo.hitAvatarTimes;
				val3.break_avatar_times = (uint)monsterInfo.breakAvatarTimes;
				val.monster_list.Add(val3);
			}
			val.rotate_camera_times = (uint)playerData.screenRotateTimes;
			val.stage_time = (uint)UIUtil.FloorToIntCustom(playerData.stageTime);
			val.max_combo_num = (uint)Singleton<LevelScoreManager>.Instance.maxComboNum;
			SendPacket<StageInnerDataReportReq>(val);
		}

		public void RequestExchangeAvatarWeapon(int avatarID1, int avatarID2)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			ExchangeAvatarWeaponReq val = new ExchangeAvatarWeaponReq();
			val.avatar_id_1 = (uint)avatarID1;
			val.avatar_id_2 = (uint)avatarID2;
			SendPacket<ExchangeAvatarWeaponReq>(val);
		}

		public void RequestGetBulletin()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			SendPacket<GetBulletinReq>(new GetBulletinReq());
			Singleton<BulletinModule>.Instance.LastCheckBulletinTime = TimeUtil.Now;
		}

		public void RequestGetSignInRewardStatus()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetSignInRewardStatusReq data = new GetSignInRewardStatusReq();
			SendPacket<GetSignInRewardStatusReq>(data);
		}

		public void RequestGetSignInReward()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetSignInRewardReq data = new GetSignInRewardReq();
			SendPacket<GetSignInRewardReq>(data);
		}

		public void RequestEndlessData()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetEndlessDataReq data = new GetEndlessDataReq();
			SendPacket<GetEndlessDataReq>(data);
		}

		public void RequestEndlessAvatarHp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetEndlessAvatarHpReq data = new GetEndlessAvatarHpReq();
			SendPacket<GetEndlessAvatarHpReq>(data);
		}

		public void RequestUseEndlessItem(uint itemId, int targetUid = -1)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			UseEndlessItemReq val = new UseEndlessItemReq();
			val.item_id = itemId;
			if (targetUid > 0)
			{
				val.target_uid = (uint)targetUid;
			}
			SendPacket<UseEndlessItemReq>(val);
		}

		public void RequestGalAddGoodFeel(int avatarId, int amount, uint type)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AddGoodfeelReq val = new AddGoodfeelReq();
			val.avatar_id = (uint)avatarId;
			val.add_goodfeel = amount;
			val.add_goodfeel_type = type;
			SendPacket<AddGoodfeelReq>(val);
		}

		public void RequestGetIsland()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetIslandReq data = new GetIslandReq();
			SendPacket<GetIslandReq>(data);
		}

		public void RequestCabinLevelUp(CabinType cabinType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected I4, but got Unknown
			LevelUpCabinReq val = new LevelUpCabinReq();
			val.cabin_type = (uint)(int)cabinType;
			SendPacket<LevelUpCabinReq>(val);
		}

		public void RequestExtendCabin(CabinType cabinType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected I4, but got Unknown
			ExtendCabinReq val = new ExtendCabinReq();
			val.cabin_type = (uint)(int)cabinType;
			SendPacket<ExtendCabinReq>(val);
		}

		public void RequestFinishCabinLevelUp(CabinType cabinType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected I4, but got Unknown
			FinishCabinLevelUpReq val = new FinishCabinLevelUpReq();
			val.cabin_type = (uint)(int)cabinType;
			SendPacket<FinishCabinLevelUpReq>(val);
		}

		public void RequestFeedStigmataAffix(int mainUID, int consumeUID)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			FeedStigmataAffixReq val = new FeedStigmataAffixReq();
			val.main_unique_id = (uint)mainUID;
			val.consume_unique_id = (uint)consumeUID;
			SendPacket<FeedStigmataAffixReq>(val);
		}

		public void RequestSelectNewStigmataAffix()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			SelectNewStigmataAffixReq data = new SelectNewStigmataAffixReq();
			SendPacket<SelectNewStigmataAffixReq>(data);
		}

		public void RequestProductList()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetProductListReq data = new GetProductListReq();
			SendPacket<GetProductListReq>(data);
		}

		public void RequestAddCabinTech(CabinType cabinType, int x, int y)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected I4, but got Unknown
			AddCabinTechReq val = new AddCabinTechReq();
			val.cabin_type = (uint)(int)cabinType;
			val.pos_x = x;
			val.pos_y = y;
			SendPacket<AddCabinTechReq>(val);
		}

		public void RequestCheckAppleReceipt(string receipt)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			VerifyItunesOrderNotify val = new VerifyItunesOrderNotify();
			val.receipt_data = receipt;
			SendPacket<VerifyItunesOrderNotify>(val);
		}

		public void RequestResetCabinTech(CabinType cabinType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected I4, but got Unknown
			ResetCabinTechReq val = new ResetCabinTechReq();
			val.cabin_type = (uint)(int)cabinType;
			SendPacket<ResetCabinTechReq>(val);
		}

		public void RequestGetVipRewardData()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetVipRewardDataReq data = new GetVipRewardDataReq();
			SendPacket<GetVipRewardDataReq>(data);
		}

		public void RequestIslandVenture()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetIslandVentureReq data = new GetIslandVentureReq();
			SendPacket<GetIslandVentureReq>(data);
		}

		public void RequestGetVipReward(int VipLevel)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetVipRewardReq val = new GetVipRewardReq();
			val.vip_level = (uint)VipLevel;
			SendPacket<GetVipRewardReq>(val);
		}

		public void RequestRefreshIslandVenture()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			RefreshIslandVentureReq data = new RefreshIslandVentureReq();
			SendPacket<RefreshIslandVentureReq>(data);
		}

		public void RequestDispatchIslandVenture(int ventureId, List<int> avatarIdList)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			DispatchIslandVentureReq val = new DispatchIslandVentureReq();
			val.venture_id = (uint)ventureId;
			foreach (int avatarId in avatarIdList)
			{
				val.avatar_id_list.Add((uint)avatarId);
			}
			SendPacket<DispatchIslandVentureReq>(val);
		}

		public void RequestGetIslandVentureReward(int ventureId)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetIslandVentureRewardReq val = new GetIslandVentureRewardReq();
			val.venture_id = (uint)ventureId;
			SendPacket<GetIslandVentureRewardReq>(val);
		}

		public void RequestCancelDispatchIslandVenture(int ventureId)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			CancelDispatchIslandVentureReq val = new CancelDispatchIslandVentureReq();
			val.venture_id = (uint)ventureId;
			SendPacket<CancelDispatchIslandVentureReq>(val);
		}

		public void RequestGetShopList()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetShopListReq data = new GetShopListReq();
			SendPacket<GetShopListReq>(data);
		}

		public void RequestBuyGoods(int shopID, int goodsID)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			BuyGoodsReq val = new BuyGoodsReq();
			val.shop_id = (uint)shopID;
			val.goods_id = (uint)goodsID;
			SendPacket<BuyGoodsReq>(val);
		}

		public void RequestManualRefreshShop(int shopID)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			ManualRefreshShopReq val = new ManualRefreshShopReq();
			val.shop_id = (uint)shopID;
			SendPacket<ManualRefreshShopReq>(val);
		}

		public void RequestIslandDisjoinEquipment(EquipmentType type, uint id)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			IslandDisjoinEquipmentReq val = new IslandDisjoinEquipmentReq();
			val.type = type;
			val.unique_id = id;
			SendPacket<IslandDisjoinEquipmentReq>(val);
		}

		public void RequestIslandCollect()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			IslandCollectReq data = new IslandCollectReq();
			SendPacket<IslandCollectReq>(data);
		}

		public void RequestGetCollectCabin()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetCollectCabinReq data = new GetCollectCabinReq();
			SendPacket<GetCollectCabinReq>(data);
		}

		public void RequestCreateWeiXinOrder(string productID, string productName, float productPrice)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			CreateWeiXinOrderReq val = new CreateWeiXinOrderReq();
			val.body = productName;
			val.attach = Singleton<PlayerModule>.Instance.playerData.userId + "|" + productID;
			val.total_fee = ((int)(productPrice * 100f)).ToString();
			val.notify_url = Singleton<NetworkManager>.Instance.DispatchSeverData.oaServerUrl + "/callback/weixin";
			SendPacket<CreateWeiXinOrderReq>(val);
		}

		public void RequestSpeedUpIslandVenture(int ventureID, int materialID, int num)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			SpeedUpIslandVentureReq val = new SpeedUpIslandVentureReq();
			val.venture_id = (uint)ventureID;
			val.material_id = (uint)materialID;
			val.num = (uint)num;
			SendPacket<SpeedUpIslandVentureReq>(val);
		}

		public void RequestGuideReward(int rewardID)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetGuideRewardReq val = new GetGuideRewardReq();
			val.guide_id = (uint)rewardID;
			SendPacket<GetGuideRewardReq>(val);
		}

		public void RequestGetRedeemCodeInfo(string redeemCode)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetRedeemCodeInfoReq val = new GetRedeemCodeInfoReq();
			val.redeem_code = redeemCode;
			SendPacket<GetRedeemCodeInfoReq>(val);
		}

		public void RequestExchangeRedeemCode(string redeemCode)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			ExchangeRedeemCodeReq val = new ExchangeRedeemCodeReq();
			val.redeem_code = redeemCode;
			SendPacket<ExchangeRedeemCodeReq>(val);
		}

		public void RequestBuyGachaTicket(int ticketID, int num)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			BuyGachaTicketReq val = new BuyGachaTicketReq();
			val.material_id = (uint)ticketID;
			val.num = (uint)num;
			SendPacket<BuyGachaTicketReq>(val);
		}

		public void RequestAntiCheatSDKReport()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AntiCheatSDKReportReq data = new AntiCheatSDKReportReq();
			SendPacket<AntiCheatSDKReportReq>(data);
		}

		public void RequestGetEndlessTopGroup()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetEndlessTopGroupReq data = new GetEndlessTopGroupReq();
			SendPacket<GetEndlessTopGroupReq>(data);
		}

		public void RequestGetRefreshIslandVentureInfo()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetRefreshIslandVentureInfoReq data = new GetRefreshIslandVentureInfoReq();
			SendPacket<GetRefreshIslandVentureInfoReq>(data);
		}

		public void RequestCommentReport(CommentType comment, uint times)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			CommentReportReq val = new CommentReportReq();
			val.comment = comment;
			val.times = times;
			SendPacket<CommentReportReq>(val);
		}

		public void RequestGetInviteFriend()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetInviteFriendReq data = new GetInviteFriendReq();
			SendPacket<GetInviteFriendReq>(data);
		}

		public void RequestGetInviteeFriend()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetInviteeFriendReq data = new GetInviteeFriendReq();
			SendPacket<GetInviteeFriendReq>(data);
		}

		public void RequestGetAcceptFriendInvite(string inviteCode)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AcceptFriendInviteReq val = new AcceptFriendInviteReq();
			val.invite_code = inviteCode;
			SendPacket<AcceptFriendInviteReq>(val);
		}

		private void InitClientPacketConsumer()
		{
			if (_clientPacketConsumer == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "NetPacketConsumer";
				_clientPacketConsumer = gameObject.AddComponent<MonoClientPacketConsumer>();
			}
			_clientPacketConsumer.Init(_client);
			_clientPacketConsumer.gameObject.SetActive(false);
		}

		private bool ConnectGameServer(string host, ushort port, int timeout_ms = 3000)
		{
			if (_client.isConnected())
			{
				if (_client.Host == host && _client.Port == port)
				{
					return true;
				}
				_client.disconnect();
			}
			bool flag = _client.connect(host, port, timeout_ms);
			if (flag)
			{
				NetPacketV1 netPacketV = new NetPacketV1();
				netPacketV.setCmdId(1);
				_client.setKeepalive(10000, netPacketV);
				_client.setDisconnectCallback(UnexceptedDisconnectCallback);
				_clientPacketConsumer.gameObject.SetActive(true);
			}
			return flag;
		}

		public void DisConnect()
		{
			_client.disconnect();
		}

		public void SendFakePacket<T>()
		{
			T fakeRsp = FakePacketHelper.GetFakeRsp<T>();
			SendFakePacket(fakeRsp);
		}

		public void SendFakePacket<T>(T data)
		{
			Singleton<ApplicationManager>.Instance.StartCoroutine(DoSendFakePacket(data));
		}

		private IEnumerator DoSendFakePacket<T>(T data)
		{
			yield return null;
			ushort cmdID = Singleton<CommandMap>.Instance.GetCmdIDByType(typeof(T));
			NetPacketV1 req_pack = new NetPacketV1();
			req_pack.setUserId((uint)Singleton<PlayerModule>.Instance.playerData.userId);
			req_pack.setCmdId(cmdID);
			req_pack.setData(data);
			Type cmdType = Singleton<CommandMap>.Instance.GetTypeByCmdID(cmdID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.NetwrokPacket, req_pack));
		}

		public void SendPacket<T>(T data)
		{
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				return;
			}
			ushort cmdIDByType = Singleton<CommandMap>.Instance.GetCmdIDByType(typeof(T));
			if (CheckRequestTimeValid(cmdIDByType))
			{
				NetPacketV1 netPacketV = new NetPacketV1();
				netPacketV.setUserId((uint)Singleton<PlayerModule>.Instance.playerData.userId);
				netPacketV.setCmdId(cmdIDByType);
				netPacketV.setData(data);
				if (cmdIDByType != 4 && cmdIDByType != 6)
				{
					netPacketV.setTime(++_clientPacketConsumer.clientPacketId);
				}
				Type typeByCmdID = Singleton<CommandMap>.Instance.GetTypeByCmdID(cmdIDByType);
				TryCacheSendPacket(netPacketV);
				_client.send(netPacketV);
			}
		}

		public void UnexceptedDisconnectCallback()
		{
		}

		public void QuickLogin()
		{
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				RequestPlayerToken();
				return;
			}
			Singleton<ApplicationManager>.Instance.StartCoroutine(ConnectDispatchServer(delegate
			{
				if (ConnectGameServer(DispatchSeverData.host, DispatchSeverData.port))
				{
					RequestPlayerToken();
				}
			}));
		}

		public void LoginGameServer()
		{
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				RequestPlayerToken();
			}
			else if (ConnectGameServer(DispatchSeverData.host, DispatchSeverData.port))
			{
				RequestPlayerToken();
			}
		}

		public void ProcessWaitStopAnotherLogin()
		{
			float num = 2f;
			if (alreadyLogin)
			{
				Singleton<ApplicationManager>.Instance.Invoke(num, RequestPlayerLogin);
				return;
			}
			LoadingWheelWidgetContext loadingWheelWidgetContext = new LoadingWheelWidgetContext();
			loadingWheelWidgetContext.SetMaxWaitTime(num);
			loadingWheelWidgetContext.timeOutCallBack = RequestPlayerLogin;
			Singleton<MainUIManager>.Instance.ShowWidget(loadingWheelWidgetContext);
		}

		public IEnumerator ConnectGlobalDispatchServer(Action successCallback = null)
		{
			string retJsonString = string.Empty;
			for (int count = 0; count < _globalDispatchUrlList.Count; count++)
			{
				float timer = 0f;
				string globalDispatchUrl = _globalDispatchUrlList[count] + "?version=" + GetGameVersion();
				bool timeout = false;
				WWW www = new WWW(globalDispatchUrl);
				while (!www.isDone)
				{
					if (timer > 3f)
					{
						timeout = true;
						break;
					}
					timer += Time.deltaTime;
					yield return null;
				}
				if (!string.IsNullOrEmpty(www.error) || timeout)
				{
					www.Dispose();
					continue;
				}
				retJsonString = www.text;
				break;
			}
			OnConnectGlobalDispatch(retJsonString, successCallback);
		}

		private bool OnConnectGlobalDispatch(string retJsonString, Action successCallback = null)
		{
			bool flag = false;
			bool flag2 = false;
			string errorMsg = string.Empty;
			int retCode = 0;
			JSONNode retJson = null;
			string desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConnectGlobalDispatchErr");
			flag2 = TryGetRetCodeFromJsonString(retJsonString, out retJson, out errorMsg, out retCode);
			flag = flag2;
			if (flag2)
			{
				if (retCode == 0)
				{
					GlobalDispatchData = new GlobalDispatchDataItem(retJson);
					if (GlobalDispatchData.regionList.Count <= 0)
					{
						flag = false;
						errorMsg = "Error: GlobalDispatchData.regionList.Count <= 0";
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = false;
					errorMsg = (string)retJson["msg"] + " retcode=" + retCode;
					desc = errorMsg;
				}
			}
			if (flag)
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Tittle_GlobalDispatchUnknownErr"),
					desc = desc,
					notDestroyAfterTouchBG = true,
					hideCloseBtn = true,
					buttonCallBack = delegate
					{
						TryReconnectGlobalDispatch();
					}
				});
				if (!string.IsNullOrEmpty(retJsonString))
				{
					SuperDebug.VeryImportantError("Connect Global Dispatch Error msg=" + errorMsg + " retJsonString=" + retJsonString);
				}
			}
			return flag;
		}

		private bool TryGetRetCodeFromJsonString(string jsonString, out JSONNode retJson, out string errorMsg, out int retCode)
		{
			bool flag = false;
			errorMsg = string.Empty;
			retCode = 0;
			retJson = null;
			if (string.IsNullOrEmpty(jsonString))
			{
				flag = false;
				errorMsg = "Error: retJsonString IsNullOrEmpty!";
			}
			else
			{
				retJson = JSON.Parse(jsonString);
				if (retJson == null || string.IsNullOrEmpty(retJson["retcode"]))
				{
					flag = false;
					errorMsg = "Error: JSON.Parse null!";
				}
				else if (!int.TryParse(retJson["retcode"].Value, out retCode))
				{
					flag = false;
					errorMsg = "Error: retcode is not integer!";
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		public IEnumerator ConnectDispatchServer(Action successCallback = null)
		{
			int lastLoginUserId = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginUserId;
			string dispatchUrl = GlobalDispatchData.regionList[0].dispatchUrl + "?version=" + GetGameVersion() + "&uid=" + lastLoginUserId;
			WWW www = new WWW(dispatchUrl);
			yield return www;
			string retString = string.Empty;
			if (string.IsNullOrEmpty(www.error))
			{
				retString = www.text;
			}
			OnConnectDispatchServer(retString, successCallback);
			www.Dispose();
		}

		private bool OnConnectDispatchServer(string retJsonString, Action successCallback = null)
		{
			bool flag = false;
			string errorMsg = string.Empty;
			int retCode = 0;
			JSONNode retJson = null;
			if (!TryGetRetCodeFromJsonString(retJsonString, out retJson, out errorMsg, out retCode))
			{
				if (!alreadyLogin)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
					{
						type = GeneralDialogContext.ButtonType.SingleButton,
						title = LocalizationGeneralLogic.GetText("Menu_NetError"),
						desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConnectDispatchErr"),
						notDestroyAfterTouchBG = true,
						hideCloseBtn = true,
						buttonCallBack = delegate
						{
							TryReconnectDispatch();
						}
					});
				}
				if (!string.IsNullOrEmpty(retJsonString))
				{
					SuperDebug.VeryImportantError("Connect Dispatch Error msg=" + errorMsg + " retJsonString=" + retJsonString);
				}
				return false;
			}
			retCode = retJson["retcode"].AsInt;
			switch (retCode)
			{
			case 0:
				DispatchSeverData = new DispatchServerDataItem(retJson);
				Singleton<AssetBundleManager>.Instance.remoteAssetBundleUrl = DispatchSeverData.assetBundleUrl;
				if (DispatchSeverData.dataUseAssetBundleUseSever)
				{
					GlobalVars.DataUseAssetBundle = DispatchSeverData.dataUseAssetBundle;
				}
				if (DispatchSeverData.resUseAssetBundleUseSever)
				{
					GlobalVars.ResourceUseAssetBundle = DispatchSeverData.resUseAssetBundle;
				}
				if (successCallback != null)
				{
					successCallback();
				}
				return true;
			case 4:
			{
				string forceUupdateUrl = OpeUtil.ConvertEventUrl(retJson["force_update_url"]);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_NewVersion"),
					desc = retJson["msg"],
					notDestroyAfterTouchBG = true,
					hideCloseBtn = true,
					buttonCallBack = delegate
					{
						Application.OpenURL(forceUupdateUrl);
					}
				});
				break;
			}
			case 2:
			{
				DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp((uint)retJson["stop_begin_time"].AsInt);
				DateTime dateTimeFromTimeStamp2 = Miscs.GetDateTimeFromTimeStamp((uint)retJson["stop_end_time"].AsInt);
				string desc = string.Concat(retJson["msg"], Environment.NewLine, LocalizationGeneralLogic.GetText("Menu_ServerStopTime", dateTimeFromTimeStamp.ToString("MM-dd HH:mm"), dateTimeFromTimeStamp2.ToString("MM-dd HH:mm")));
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_ServerStop"),
					desc = desc,
					notDestroyAfterTouchBG = true,
					hideCloseBtn = true,
					buttonCallBack = delegate
					{
						TryReconnectDispatch();
					}
				});
				break;
			}
			default:
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Tittle_DispatchUnknownErr"),
					desc = (string)retJson["msg"] + " retcode=" + retCode,
					notDestroyAfterTouchBG = true,
					hideCloseBtn = true,
					buttonCallBack = delegate
					{
						TryReconnectDispatch();
					}
				});
				break;
			}
			return false;
		}

		private void TryReconnectDispatch()
		{
			if (alreadyLogin)
			{
				QuickLogin();
				return;
			}
			MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
			if (monoGameEntry != null)
			{
				monoGameEntry.ConnectDispatch();
			}
		}

		private void TryReconnectGlobalDispatch()
		{
			if (!alreadyLogin)
			{
				MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
				if (monoGameEntry != null)
				{
					monoGameEntry.ConnentGlobalDispatch();
				}
			}
		}

		private GetPlayerTokenReq GetTestPlayerTokenReq()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			string persistentUUID = GetPersistentUUID();
			GetPlayerTokenReq val = new GetPlayerTokenReq();
			val.account_type = 0u;
			val.account_uid = string.Empty;
			val.account_token = string.Empty;
			val.account_ext = string.Empty;
			val.token = persistentUUID;
			return val;
		}

		private static string GetPersistentUUID()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		private static bool CheckDeviceUniqueIdentifier()
		{
			string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
			return !string.IsNullOrEmpty(deviceUniqueIdentifier) && deviceUniqueIdentifier != "00000000-0000-0000-0000-000000000000";
		}

		public string GetGameVersion()
		{
			return "0.9.9_" + channelConfig.ChannelName;
		}

		private uint GeneralLoginRandomNum()
		{
			return (uint)(DateTime.Now.Ticks >> 13);
		}

		private bool CheckRequestTimeValid(int cmdId)
		{
			if (!_requestMinIntervalDict.ContainsKey(cmdId))
			{
				return true;
			}
			bool flag = false;
			if (_lastRequestTimeDict.ContainsKey(cmdId))
			{
				if (_lastRequestTimeDict[cmdId].AddSeconds(_requestMinIntervalDict[cmdId]) < TimeUtil.Now)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				_lastRequestTimeDict[cmdId] = TimeUtil.Now;
			}
			return flag;
		}

		private void SetupRequestMinIntervalDict()
		{
			_requestMinIntervalDict = new Dictionary<int, float>
			{
				{ 14, 1f },
				{ 18, 1f },
				{ 54, 1f },
				{ 31, 3f },
				{ 33, 3f },
				{ 102, 3f },
				{ 29, 3f },
				{ 37, 3f },
				{ 35, 3f },
				{ 50, 1f },
				{ 86, 1f },
				{ 106, 1f },
				{ 135, 1f },
				{ 147, 1f },
				{ 158, 3f },
				{ 160, 3f },
				{ 162, 1f },
				{ 193, 3f },
				{ 195, 3f },
				{ 173, 3f },
				{ 171, 3f },
				{ 175, 1f },
				{ 203, 1f },
				{ 205, 3f },
				{ 179, 3f }
			};
		}

		private void BuildCmdShouldEnqueueMap()
		{
			_CMD_SHOULD_ENQUEUE_MAP = new Dictionary<uint, bool>
			{
				{ 201u, true },
				{ 58u, true },
				{ 43u, true },
				{ 72u, true },
				{ 45u, true },
				{ 131u, true },
				{ 141u, true },
				{ 143u, true },
				{ 106u, true },
				{ 98u, true },
				{ 100u, true },
				{ 112u, true },
				{ 117u, true },
				{ 129u, true },
				{ 31u, true }
			};
		}

		private void TryCacheSendPacket(NetPacketV1 reqPack)
		{
			if (_CMD_SHOULD_ENQUEUE_MAP == null)
			{
				BuildCmdShouldEnqueueMap();
			}
			if (_CMD_SHOULD_ENQUEUE_MAP.ContainsKey(reqPack.getCmdId()))
			{
				if (_packetSendQueue.Count >= 20)
				{
					_packetSendQueue.Dequeue();
				}
				_packetSendQueue.Enqueue(reqPack);
			}
		}

		private void SendQueuePacketWhenReconnected(uint serverProcessedPacketId)
		{
			foreach (NetPacketV1 item in _packetSendQueue)
			{
				if (item.getTime() > serverProcessedPacketId)
				{
					Type typeByCmdID = Singleton<CommandMap>.Instance.GetTypeByCmdID(item.getCmdId());
					_client.send(item);
				}
			}
		}

		public void Destroy()
		{
			DisConnect();
			Singleton<CommandMap>.Destroy();
			UnityEngine.Object.Destroy(_clientPacketConsumer.gameObject);
		}

		public void SetRepeatLogin()
		{
			_clientPacketConsumer.SetRepeatLogin();
		}
	}
}
