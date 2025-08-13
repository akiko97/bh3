using System;
using System.Collections.Generic;
using MoleMole.Config;
using proto;

namespace MoleMole
{
	public class StorageModule : BaseModule
	{
		public enum StorageSortType
		{
			Rarity_DESC = 0,
			Rarity_ASC = 1,
			Level_DESC = 2,
			Level_ASC = 3,
			BaseType_DESC = 4,
			BaseType_ASC = 5,
			Cost_DESC = 6,
			Cost_ASC = 7,
			Suite_DESC = 8,
			Suite_ASC = 9,
			Time_DESC = 10,
			Time_ASC = 11
		}

		private Dictionary<KeyValuePair<Type, int>, StorageDataItemBase> _userStorageDict;

		private HashSet<int> _everOwnItemIDs;

		public Dictionary<string, StorageSortType> sortTypeMap;

		public Dictionary<StorageSortType, Comparison<StorageDataItemBase>> sortComparisionMap;

		public List<StorageDataItemBase> UserStorageItemList { get; private set; }

		public StorageModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			UserStorageItemList = new List<StorageDataItemBase>();
			_userStorageDict = new Dictionary<KeyValuePair<Type, int>, StorageDataItemBase>();
			_everOwnItemIDs = new HashSet<int>();
			InitForSort();
		}

		private void InitForSort()
		{
			sortTypeMap = new Dictionary<string, StorageSortType>();
			sortComparisionMap = new Dictionary<StorageSortType, Comparison<StorageDataItemBase>>();
			string[] tAB_KEY = StorageShowPageContext.TAB_KEY;
			foreach (string key in tAB_KEY)
			{
				sortTypeMap.Add(key, StorageSortType.Rarity_DESC);
			}
			sortComparisionMap.Add(StorageSortType.Rarity_DESC, StorageDataItemBase.CompareToRarityDesc);
			sortComparisionMap.Add(StorageSortType.Rarity_ASC, StorageDataItemBase.CompareToRarityAsc);
			sortComparisionMap.Add(StorageSortType.Level_DESC, StorageDataItemBase.CompareToLevelDesc);
			sortComparisionMap.Add(StorageSortType.Level_ASC, StorageDataItemBase.CompareToLevelAsc);
			sortComparisionMap.Add(StorageSortType.Cost_DESC, StorageDataItemBase.CompareToCostDesc);
			sortComparisionMap.Add(StorageSortType.Cost_ASC, StorageDataItemBase.CompareToCostAsc);
			sortComparisionMap.Add(StorageSortType.BaseType_DESC, StorageDataItemBase.CompareToBaseTypeDesc);
			sortComparisionMap.Add(StorageSortType.BaseType_ASC, StorageDataItemBase.CompareToBaseTypeAsc);
			sortComparisionMap.Add(StorageSortType.Time_DESC, StorageDataItemBase.CompareToUidDesc);
			sortComparisionMap.Add(StorageSortType.Time_ASC, StorageDataItemBase.CompareToUidAsc);
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 27:
				return OnGetEquipmentDataRsp(pkt.getData<GetEquipmentDataRsp>());
			case 28:
				return OnDelEquipmentNotify(pkt.getData<DelEquipmentNotify>());
			case 32:
				return OnEquipmentPowerUpRsp(pkt.getData<EquipmentPowerUpRsp>());
			case 34:
				return OnEquipmentSellRsp(pkt.getData<EquipmentSellRsp>());
			case 103:
				return OnSellAvatarFragmentRsp(pkt.getData<SellAvatarFragmentRsp>());
			case 38:
				return OnEquipmentEvoRsp(pkt.getData<EquipmentEvoRsp>());
			case 40:
				return OnDressEquipmentRsp(pkt.getData<DressEquipmentRsp>());
			case 105:
				return OnGetHasGotItemIdListRsp(pkt.getData<GetHasGotItemIdListRsp>());
			default:
				return false;
			}
		}

		private bool OnGetEquipmentDataRsp(GetEquipmentDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				foreach (Weapon item in rsp.weapon_list)
				{
					WeaponDataItem weaponDataItem = null;
					if (_userStorageDict.ContainsKey(new KeyValuePair<Type, int>(typeof(WeaponDataItem), (int)item.unique_id)))
					{
						weaponDataItem = _userStorageDict[new KeyValuePair<Type, int>(typeof(WeaponDataItem), (int)item.unique_id)] as WeaponDataItem;
					}
					else
					{
						WeaponMetaData weaponMetaDataByKey = WeaponMetaDataReader.GetWeaponMetaDataByKey((int)item.id);
						weaponDataItem = new WeaponDataItem((int)item.unique_id, weaponMetaDataByKey);
						UserStorageItemList.Add(weaponDataItem);
						_userStorageDict.Add(new KeyValuePair<Type, int>(weaponDataItem.GetType(), weaponDataItem.uid), weaponDataItem);
						Singleton<ItempediaModule>.Instance.UnlockItem((int)item.id);
					}
					weaponDataItem.level = (int)item.level;
					weaponDataItem.exp = (int)item.exp;
					weaponDataItem.isProtected = item.is_protected;
				}
				foreach (Stigmata item2 in rsp.stigmata_list)
				{
					StigmataDataItem stigmataDataItem = null;
					if (_userStorageDict.ContainsKey(new KeyValuePair<Type, int>(typeof(StigmataDataItem), (int)item2.unique_id)))
					{
						stigmataDataItem = _userStorageDict[new KeyValuePair<Type, int>(typeof(StigmataDataItem), (int)item2.unique_id)] as StigmataDataItem;
					}
					else
					{
						StigmataMetaData stigmataMetaDataByKey = StigmataMetaDataReader.GetStigmataMetaDataByKey((int)item2.id);
						stigmataDataItem = new StigmataDataItem((int)item2.unique_id, stigmataMetaDataByKey);
						UserStorageItemList.Add(stigmataDataItem);
						_userStorageDict.Add(new KeyValuePair<Type, int>(stigmataDataItem.GetType(), stigmataDataItem.uid), stigmataDataItem);
						Singleton<ItempediaModule>.Instance.UnlockItem((int)item2.id);
					}
					stigmataDataItem.level = (int)item2.level;
					stigmataDataItem.exp = (int)item2.exp;
					stigmataDataItem.isProtected = item2.is_protected;
					bool is_affix_identify = !item2.is_affix_identifySpecified || item2.is_affix_identify;
					int pre_affix_id = (int)(item2.pre_affix_idSpecified ? item2.pre_affix_id : 0);
					int suf_affix_id = (int)(item2.suf_affix_idSpecified ? item2.suf_affix_id : 0);
					stigmataDataItem.SetAffixSkill(is_affix_identify, pre_affix_id, suf_affix_id);
				}
				foreach (Material item3 in rsp.material_list)
				{
					MaterialDataItem materialDataItem = null;
					if (_userStorageDict.ContainsKey(new KeyValuePair<Type, int>(typeof(MaterialDataItem), (int)item3.id)))
					{
						materialDataItem = _userStorageDict[new KeyValuePair<Type, int>(typeof(MaterialDataItem), (int)item3.id)] as MaterialDataItem;
						if (item3.num == 0)
						{
							_userStorageDict.Remove(new KeyValuePair<Type, int>(typeof(MaterialDataItem), (int)item3.id));
							UserStorageItemList.Remove(materialDataItem);
							continue;
						}
					}
					else
					{
						ItemMetaData itemMetaDataByKey = ItemMetaDataReader.GetItemMetaDataByKey((int)item3.id);
						materialDataItem = new MaterialDataItem(itemMetaDataByKey);
						UserStorageItemList.Add(materialDataItem);
						_userStorageDict.Add(new KeyValuePair<Type, int>(materialDataItem.GetType(), materialDataItem.ID), materialDataItem);
						Singleton<ItempediaModule>.Instance.UnlockItem((int)item3.id);
					}
					materialDataItem.number = (int)item3.num;
				}
			}
			return false;
		}

		private bool OnDelEquipmentNotify(DelEquipmentNotify rsp)
		{
			foreach (uint item in rsp.weapon_unique_id_list)
			{
				int value = (int)item;
				WeaponDataItem weaponDataItem = null;
				if (_userStorageDict.ContainsKey(new KeyValuePair<Type, int>(typeof(WeaponDataItem), value)))
				{
					weaponDataItem = _userStorageDict[new KeyValuePair<Type, int>(typeof(WeaponDataItem), value)] as WeaponDataItem;
					UserStorageItemList.Remove(weaponDataItem);
					_userStorageDict.Remove(new KeyValuePair<Type, int>(typeof(WeaponDataItem), value));
				}
			}
			foreach (uint item2 in rsp.stigmata_unique_id_list)
			{
				int value2 = (int)item2;
				StigmataDataItem stigmataDataItem = null;
				if (_userStorageDict.ContainsKey(new KeyValuePair<Type, int>(typeof(StigmataDataItem), value2)))
				{
					stigmataDataItem = _userStorageDict[new KeyValuePair<Type, int>(typeof(StigmataDataItem), value2)] as StigmataDataItem;
					UserStorageItemList.Remove(stigmataDataItem);
					_userStorageDict.Remove(new KeyValuePair<Type, int>(typeof(StigmataDataItem), value2));
				}
			}
			return false;
		}

		private bool OnEquipmentPowerUpRsp(EquipmentPowerUpRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
			}
			return false;
		}

		private bool OnEquipmentSellRsp(EquipmentSellRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
			}
			return false;
		}

		private bool OnSellAvatarFragmentRsp(SellAvatarFragmentRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
			}
			return false;
		}

		private bool OnEquipmentEvoRsp(EquipmentEvoRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				EquipmentItem new_item = rsp.new_item;
				Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
			}
			return false;
		}

		private bool OnDressEquipmentRsp(DressEquipmentRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
			}
			return false;
		}

		private bool OnGetHasGotItemIdListRsp(GetHasGotItemIdListRsp rsp)
		{
			_everOwnItemIDs.Clear();
			foreach (uint item in rsp.item_id_list)
			{
				_everOwnItemIDs.Add((int)item);
			}
			return false;
		}

		public bool IsItemNew(int id)
		{
			return !_everOwnItemIDs.Contains(id);
		}

		public void RecordNewItem(int id)
		{
			_everOwnItemIDs.Add(id);
		}

		public StorageDataItemBase GetStorageItemByTypeAndID(Type type, int id)
		{
			KeyValuePair<Type, int> key = new KeyValuePair<Type, int>(type, id);
			StorageDataItemBase value;
			_userStorageDict.TryGetValue(key, out value);
			return value;
		}

		public List<StorageDataItemBase> GetAllUserWeapons()
		{
			return UserStorageItemList.FindAll((StorageDataItemBase dataItem) => dataItem is WeaponDataItem);
		}

		public List<StorageDataItemBase> GetAllUserStigmata()
		{
			return UserStorageItemList.FindAll((StorageDataItemBase dataItem) => dataItem is StigmataDataItem);
		}

		public List<StorageDataItemBase> GetAllUserMaterial()
		{
			return UserStorageItemList.FindAll((StorageDataItemBase dataItem) => dataItem is MaterialDataItem);
		}

		public List<StorageDataItemBase> GetAllVentureSpeedUpMaterial()
		{
			return UserStorageItemList.FindAll((StorageDataItemBase dataItem) => IsVentureSpeedUpMaterial(dataItem.ID));
		}

		public bool IsVentureSpeedUpMaterial(int metaID)
		{
			return MaterialVentureSpeedUpDataReader.TryGetMaterialVentureSpeedUpDataByKey(metaID) != null;
		}

		public bool HasEnoughItem(int itemMetaId, int itemNum)
		{
			if (GetDummyStorageDataItem(itemMetaId) == null)
			{
				return true;
			}
			StorageDataItemBase value;
			_userStorageDict.TryGetValue(new KeyValuePair<Type, int>(typeof(MaterialDataItem), itemMetaId), out value);
			if (value == null || value.number < itemNum)
			{
				return false;
			}
			return true;
		}

		public List<StorageDataItemBase> GetAllAvatarExpAddMaterial()
		{
			List<StorageDataItemBase> list = new List<StorageDataItemBase>();
			list.AddRange(UserStorageItemList.FindAll((StorageDataItemBase dataItem) => dataItem is MaterialDataItem && (dataItem as MaterialDataItem).GetAvatarExpProvideNum() > 0f));
			return list;
		}

		public StorageDataItemBase GetDummyStorageDataItem(int metaId, int level = 1)
		{
			StorageDataItemBase storageDataItemBase = null;
			ItemMetaData itemMetaData = ItemMetaDataReader.TryGetItemMetaDataByKey(metaId);
			if (itemMetaData != null)
			{
				storageDataItemBase = new MaterialDataItem(itemMetaData);
				storageDataItemBase.level = level;
				return storageDataItemBase;
			}
			WeaponMetaData weaponMetaData = WeaponMetaDataReader.TryGetWeaponMetaDataByKey(metaId);
			if (weaponMetaData != null)
			{
				storageDataItemBase = new WeaponDataItem(0, weaponMetaData);
				storageDataItemBase.level = level;
				return storageDataItemBase;
			}
			StigmataMetaData stigmataMetaData = StigmataMetaDataReader.TryGetStigmataMetaDataByKey(metaId);
			if (stigmataMetaData != null)
			{
				storageDataItemBase = new StigmataDataItem(0, stigmataMetaData);
				storageDataItemBase.level = level;
				(storageDataItemBase as StigmataDataItem).SetDummyAffixSkill();
				return storageDataItemBase;
			}
			AvatarCardMetaData avatarCardMetaData = AvatarCardMetaDataReader.TryGetAvatarCardMetaDataByKey(metaId);
			if (avatarCardMetaData != null)
			{
				storageDataItemBase = new AvatarCardDataItem(avatarCardMetaData);
				storageDataItemBase.level = level;
				return storageDataItemBase;
			}
			AvatarFragmentMetaData avatarFragmentMetaData = AvatarFragmentMetaDataReader.TryGetAvatarFragmentMetaDataByKey(metaId);
			if (avatarFragmentMetaData != null)
			{
				storageDataItemBase = new AvatarFragmentDataItem(avatarFragmentMetaData);
				storageDataItemBase.level = level;
				return storageDataItemBase;
			}
			EndlessToolMetaData endlessToolMetaData = EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey(metaId);
			if (endlessToolMetaData != null)
			{
				storageDataItemBase = new EndlessToolDataItem(metaId);
				storageDataItemBase.level = level;
				return storageDataItemBase;
			}
			return null;
		}

		public List<StorageDataItemBase> TryGetStorageDataItemByMetaId(int metaId, int number = 1)
		{
			return UserStorageItemList.FindAll((StorageDataItemBase x) => x.ID == metaId && x.number >= number);
		}

		public int GetCurrentCapacity()
		{
			return UserStorageItemList.Count;
		}

		public bool IsFull()
		{
			return UserStorageItemList.Count >= Singleton<PlayerModule>.Instance.playerData.equipmentSizeLimit;
		}

		public List<StorageDataItemBase> GetFragmentList()
		{
			List<StorageDataItemBase> list = new List<StorageDataItemBase>();
			foreach (AvatarDataItem userAvatar in Singleton<AvatarModule>.Instance.UserAvatarList)
			{
				if (userAvatar.fragment > 0)
				{
					list.Add(userAvatar.GetAvatarFragmentDataItem());
				}
			}
			return list;
		}

		public MaterialDataItem TryGetMaterialDataByID(int id)
		{
			StorageDataItemBase value;
			_userStorageDict.TryGetValue(new KeyValuePair<Type, int>(typeof(MaterialDataItem), id), out value);
			return value as MaterialDataItem;
		}

		public List<StigmataDataItem> GetStigmatasCanUseForNewAffix(StigmataDataItem stigmata)
		{
			List<StorageDataItemBase> allUserStigmata = GetAllUserStigmata();
			List<StigmataDataItem> list = new List<StigmataDataItem>();
			foreach (StorageDataItemBase item in allUserStigmata)
			{
				StigmataDataItem stigmataDataItem = item as StigmataDataItem;
				if (stigmataDataItem != null && stigmataDataItem.uid != stigmata.uid && (stigmataDataItem.ID == stigmata.ID || StigmataMetaDataReaderExtend.IsEvoRelation(stigmataDataItem.ID, stigmata.ID)) && stigmataDataItem.CanRefine)
				{
					list.Add(stigmataDataItem);
				}
			}
			return list;
		}

		public WeaponDataItem GetDummyFirstWeaponDataByRole(EntityRoleName role, int level)
		{
			foreach (ConfigWeapon value in WeaponData.GetAllWeaponConfigs().Values)
			{
				if (value.OwnerRole == role)
				{
					WeaponMetaData weaponMetaData = new WeaponMetaData(value.WeaponID, "DUMMY WEAPON", 1, 1, 1, 1, 1, 1, 1, 1, 1f, 1f, 1f, 1f, "TYPE_FOON", 1, 1, "BODY_MOD", "DUMMY WEAPON", "DUMMY WEAPON", 1, string.Empty, string.Empty, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 1, new List<string>(), 1, 0, 0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f);
					return new WeaponDataItem(1, weaponMetaData);
				}
			}
			return null;
		}

		public WeaponDataItem GetDummyWeaponDataItem(int weaponID, int level)
		{
			WeaponMetaData weaponMetaDataByKey = WeaponMetaDataReader.GetWeaponMetaDataByKey(weaponID);
			WeaponDataItem weaponDataItem = new WeaponDataItem(0, weaponMetaDataByKey);
			weaponDataItem.level = level;
			return weaponDataItem;
		}

		public StigmataDataItem GetDummyStigmataDataItem(int stigmataID, int level)
		{
			StigmataMetaData stigmataMetaDataByKey = StigmataMetaDataReader.GetStigmataMetaDataByKey(stigmataID);
			if (stigmataMetaDataByKey == null)
			{
				return null;
			}
			StigmataDataItem stigmataDataItem = new StigmataDataItem(20000, stigmataMetaDataByKey);
			stigmataDataItem.level = level;
			return stigmataDataItem;
		}
	}
}
