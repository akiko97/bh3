using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class AvatarModule : BaseModule
	{
		private Dictionary<int, AvatarDataItem> _userAvatarDict;

		public List<AvatarDataItem> UserAvatarList { get; private set; }

		public bool anyAvatarCanUnlock
		{
			get
			{
				foreach (AvatarDataItem userAvatar in UserAvatarList)
				{
					if (userAvatar.CanUnlock)
					{
						return true;
					}
				}
				return false;
			}
		}

		public AvatarModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			UserAvatarList = new List<AvatarDataItem>();
			_userAvatarDict = new Dictionary<int, AvatarDataItem>();
			AddAllAvatarsFromMetaData();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 25)
			{
				return OnGetAvatarDataRsp(pkt.getData<GetAvatarDataRsp>());
			}
			return false;
		}

		private bool OnGetAvatarDataRsp(GetAvatarDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode != 0)
			{
				return false;
			}
			foreach (Avatar item in rsp.avatar_list)
			{
				int avatar_id = (int)item.avatar_id;
				_userAvatarDict[avatar_id].Initialized = true;
				if (!_userAvatarDict[avatar_id].UnLocked && item.star != 0)
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.UnlockAvatar, avatar_id));
				}
				_userAvatarDict[avatar_id].UnLocked = item.star != 0;
				if (_userAvatarDict[avatar_id].UnLocked)
				{
					UpdateField(ref _userAvatarDict[avatar_id].star, (int)item.star, _userAvatarDict[avatar_id].OnStarUpdate);
					UpdateField(ref _userAvatarDict[avatar_id].star, (int)item.star, _userAvatarDict[avatar_id].OnStarUpdate);
					UpdateEquipment(_userAvatarDict[avatar_id], item);
				}
				UpdateField(ref _userAvatarDict[avatar_id].level, (int)item.level, _userAvatarDict[avatar_id].OnLevelUpdate);
				_userAvatarDict[avatar_id].exp = (int)item.exp;
				_userAvatarDict[avatar_id].fragment = (int)item.fragment;
				foreach (proto.AvatarSkill item2 in item.skill_list)
				{
					AvatarSkillDataItem avatarSkillBySkillID = _userAvatarDict[avatar_id].GetAvatarSkillBySkillID((int)item2.skill_id);
					foreach (AvatarSubSkill item3 in item2.sub_skill_list)
					{
						AvatarSubSkillDataItem avatarSubSkillBySubSkillId = avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId((int)item3.sub_skill_id);
						avatarSubSkillBySubSkillId.level = (int)item3.level;
					}
				}
			}
			return false;
		}

		private void UpdateEquipment(AvatarDataItem dataItem, Avatar packetItem)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			int[] array = new int[4]
			{
				(int)packetItem.weapon_unique_id,
				(int)packetItem.stigmata_unique_id_1,
				(int)packetItem.stigmata_unique_id_2,
				(int)packetItem.stigmata_unique_id_3
			};
			for (int i = 0; i < array.Length; i++)
			{
				EquipmentSlot slot = AvatarDataItem.EQUIP_SLOTS[i];
				UpdateEquipmentBySlot(dataItem, slot, array[i]);
			}
		}

		private void UpdateEquipmentBySlot(AvatarDataItem avatar, EquipmentSlot slot, int newUid)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			Type type = (((int)slot != 1) ? typeof(StigmataDataItem) : typeof(WeaponDataItem));
			StorageDataItemBase storageDataItemBase = avatar.equipsMap[slot];
			avatar.equipsMap[slot] = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(type, newUid);
			StorageDataItemBase storageDataItemBase2 = avatar.equipsMap[slot];
			if (storageDataItemBase != null && storageDataItemBase.avatarID == avatar.avatarID)
			{
				storageDataItemBase.avatarID = -1;
			}
			if (storageDataItemBase2 != null)
			{
				storageDataItemBase2.avatarID = avatar.avatarID;
			}
		}

		private void AddAllAvatarsFromMetaData()
		{
			List<AvatarMetaData> itemList = AvatarMetaDataReader.GetItemList();
			foreach (AvatarMetaData item in itemList)
			{
				if (!_userAvatarDict.ContainsKey(item.avatarID))
				{
					AddAvatar(GetDummyAvatarDataItem(item.avatarID));
				}
			}
		}

		private void AddAvatar(AvatarDataItem avatar)
		{
			UserAvatarList.Add(avatar);
			_userAvatarDict.Add(avatar.avatarID, avatar);
		}

		public AvatarDataItem GetAvatarByID(int avatarID)
		{
			return _userAvatarDict[avatarID];
		}

		public AvatarDataItem TryGetAvatarByID(int avatarID)
		{
			AvatarDataItem value;
			_userAvatarDict.TryGetValue(avatarID, out value);
			return value;
		}

		public AvatarDataItem GetDummyAvatarDataItem(int id)
		{
			return new AvatarDataItem(id);
		}

		public List<KeyValuePair<string, bool>> GetCanUnlockSkillNameList(int avatarID, int levelBefore, int starBefore, int levelAfter, int starAfter)
		{
			List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
			AvatarDataItem avatarDataItem = new AvatarDataItem(avatarID, levelBefore, starBefore);
			AvatarDataItem avatarDataItem2 = new AvatarDataItem(avatarID, levelAfter, starAfter);
			for (int i = 0; i < avatarDataItem.skillDataList.Count; i++)
			{
				AvatarSkillDataItem avatarSkillDataItem = avatarDataItem.skillDataList[i];
				AvatarSkillDataItem avatarSkillDataItem2 = avatarDataItem2.skillDataList[i];
				if (avatarSkillDataItem2.UnLocked && !avatarSkillDataItem.UnLocked)
				{
					list.Add(new KeyValuePair<string, bool>(avatarSkillDataItem2.SkillName, false));
				}
				for (int j = 0; j < avatarSkillDataItem.avatarSubSkillList.Count; j++)
				{
					AvatarSubSkillDataItem avatarSubSkillDataItem = avatarSkillDataItem.avatarSubSkillList[j];
					AvatarSubSkillDataItem avatarSubSkillDataItem2 = avatarSkillDataItem2.avatarSubSkillList[j];
					bool flag = levelBefore >= avatarSubSkillDataItem.UnlockLv && starBefore >= avatarSubSkillDataItem.UnlockStar;
					if (levelAfter >= avatarSubSkillDataItem2.UnlockLv && starAfter >= avatarSubSkillDataItem2.UnlockStar && !flag)
					{
						list.Add(new KeyValuePair<string, bool>(avatarSkillDataItem2.SkillName + "." + avatarSubSkillDataItem2.Name, true));
					}
				}
			}
			return list;
		}

		public AvatarDataItem GetDummyAvatarDataItem(string avatarRegistryKey, int level = 1, int star = 0)
		{
			AvatarMetaData avatarMetaDataByRegistryKey = GetAvatarMetaDataByRegistryKey(avatarRegistryKey);
			AvatarDataItem avatarDataItem;
			if (avatarMetaDataByRegistryKey == null)
			{
				avatarDataItem = GetUnusedAvatarDataItem(avatarRegistryKey);
			}
			else if (star == 0)
			{
				AvatarStarMetaData starMetaData = new AvatarStarMetaData(avatarMetaDataByRegistryKey.avatarID, 0, 1, 1, "SpriteOutput/AvatarIcon/101", "SpriteOutput/AvatarIconSide/101", "SpriteOutput/AvatarTachie/KianaC2", 1000f, 0f, 1000f, 0f, 100f, 0f, 0f, 0f, 0f, 0f);
				avatarDataItem = new AvatarDataItem(avatarMetaDataByRegistryKey.avatarID, avatarMetaDataByRegistryKey, ClassMetaDataReader.GetClassMetaDataByKey(avatarMetaDataByRegistryKey.classID), starMetaData, AvatarLevelMetaDataReader.GetAvatarLevelMetaDataByKey(level), level, star);
				avatarDataItem.UpdateSkillInfo();
			}
			else
			{
				avatarDataItem = new AvatarDataItem(avatarMetaDataByRegistryKey.avatarID, level, star);
			}
			return avatarDataItem;
		}

		public AvatarMetaData GetAvatarMetaDataByRegistryKey(string registryKey)
		{
			List<AvatarMetaData> itemList = AvatarMetaDataReader.GetItemList();
			return itemList.Find((AvatarMetaData avatarMetaData) => avatarMetaData.avatarRegistryKey == registryKey);
		}

		private AvatarDataItem GetUnusedAvatarDataItem(string avatarRegistryKey)
		{
			int avatarID = 100001;
			AvatarMetaData metaData = new AvatarMetaData(avatarID, 1, "DUMMY_" + avatarRegistryKey, avatarRegistryKey, "DUMMY AVATAR FOR DEVLOPEMENT :" + avatarRegistryKey, avatarRegistryKey, new List<int>(), 0, new List<int> { 11, 12, 13, 14, 15, 16 }, 1, 0, 0, 0, 0, 14, 15, 3f, 0f, 0f, 0, 0f, 0f, 0f, 0, 0f, 0f, 0f, 0, 0f, 0);
			ClassMetaData classMetaData = new ClassMetaData(10000, "DUMMY CLASS", "DUMMY CLASS", "FirstName", "LastName");
			AvatarLevelMetaData levelMetaData = new AvatarLevelMetaData(1, 1, 0, 1f);
			AvatarStarMetaData starMetaData = new AvatarStarMetaData(avatarID, 1, 1, 1, "SpriteOutput/AvatarIcon/101", "SpriteOutput/AvatarIconSide/101", "SpriteOutput/AvatarTachie/KianaC2", 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f);
			return new AvatarDataItem(avatarID, metaData, classMetaData, starMetaData, levelMetaData, 1, 1);
		}
	}
}
