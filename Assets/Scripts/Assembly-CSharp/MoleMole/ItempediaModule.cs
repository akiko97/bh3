using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class ItempediaModule : BaseModule
	{
		private List<int> _unlockedItemIds;

		private List<int> _weaponIds = new List<int>();

		private List<int> _stigmataIds = new List<int>();

		private List<int> _itemIds = new List<int>();

		public ItempediaModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
		}

		public override bool OnPacket(NetPacketV1 packet)
		{
			ushort cmdId = packet.getCmdId();
			if (cmdId == 105)
			{
				return OnGetHasGotItemIdListRsp(packet.getData<GetHasGotItemIdListRsp>());
			}
			return false;
		}

		public int[] GetAllUnlockItems()
		{
			return _unlockedItemIds.ToArray();
		}

		private bool OnGetHasGotItemIdListRsp(GetHasGotItemIdListRsp rsp)
		{
			UpdateIdList();
			_unlockedItemIds = new List<int>(rsp.item_id_list.Count);
			int i = 0;
			for (int count = rsp.item_id_list.Count; i < count; i++)
			{
				if (IsInItempedia((int)rsp.item_id_list[i]))
				{
					_unlockedItemIds.Add((int)rsp.item_id_list[i]);
				}
			}
			return false;
		}

		private void UpdateIdList()
		{
			_weaponIds.Clear();
			_stigmataIds.Clear();
			_itemIds.Clear();
			List<WeaponMetaData> allWeaponData = GetAllWeaponData();
			int i = 0;
			for (int count = allWeaponData.Count; i < count; i++)
			{
				_weaponIds.Add(allWeaponData[i].ID);
			}
			List<StigmataMetaData> allStigmataData = GetAllStigmataData();
			int j = 0;
			for (int count2 = allStigmataData.Count; j < count2; j++)
			{
				_stigmataIds.Add(allStigmataData[j].ID);
			}
			List<ItemMetaData> allItemData = GetAllItemData();
			int k = 0;
			for (int count3 = allItemData.Count; k < count3; k++)
			{
				_itemIds.Add(allItemData[k].ID);
			}
		}

		public bool IsInItempedia(int id)
		{
			return _weaponIds.Contains(id) || _stigmataIds.Contains(id) || _itemIds.Contains(id);
		}

		public int GetUnlockCountTotal()
		{
			return GetUnlockCountWeapon() + GetUnlockCountStigmata();
		}

		public int GetUnlockCountWeapon()
		{
			return GetUnlockCountByPredicate((int x) => x / 10000 == 2);
		}

		public int GetUnlockCountStigmata()
		{
			return GetUnlockCountByPredicate((int x) => x / 10000 == 3);
		}

		public int GetUnlockCountItem()
		{
			return GetUnlockCountByPredicate((int x) => x < 10000);
		}

		public int GetAllWeaponCount()
		{
			return _weaponIds.Count;
		}

		public int GetAllStigmataCount()
		{
			return _stigmataIds.Count;
		}

		public int GetAllItemCount()
		{
			return _itemIds.Count;
		}

		public int GetItempediaTotalCount()
		{
			return GetAllWeaponCount() + GetAllStigmataCount();
		}

		public void UnlockItem(int id)
		{
			if (IsContainedInMultiList(id, _weaponIds, _stigmataIds, _itemIds) && !_unlockedItemIds.Contains(id))
			{
				_unlockedItemIds.Add(id);
			}
		}

		private bool IsContainedInMultiList(int id, params List<int>[] lists)
		{
			int num = 0;
			while (true)
			{
				int i = 0;
				for (int num2 = lists.Length; i < num2; i++)
				{
					List<int> list = lists[i];
					if (list.Count <= i)
					{
						continue;
					}
					int j = 0;
					for (int count = list.Count; j < count; j++)
					{
						if (list[j] == id)
						{
							return true;
						}
					}
				}
				bool flag = true;
				int k = 0;
				for (int num3 = lists.Length; k < num3; k++)
				{
					if (lists[k].Count > num)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					break;
				}
				num++;
			}
			return false;
		}

		private int GetUnlockCountByPredicate(Predicate<int> pred)
		{
			if (_unlockedItemIds == null)
			{
				return 0;
			}
			int num = 0;
			int i = 0;
			for (int count = _unlockedItemIds.Count; i < count; i++)
			{
				if (pred(_unlockedItemIds[i]))
				{
					num++;
				}
			}
			return num;
		}

		private List<WeaponMetaData> GetAllWeaponData()
		{
			List<WeaponMetaData> itemList = WeaponMetaDataReader.GetItemList();
			List<WeaponMetaData> list = new List<WeaponMetaData>();
			int i = 0;
			for (int count = itemList.Count; i < count; i++)
			{
				WeaponMetaData weaponMetaData = itemList[i];
				if (!ItempediaData.IsInBlacklist(weaponMetaData.ID) && weaponMetaData.subRarity == 0)
				{
					list.Add(weaponMetaData);
				}
			}
			return list;
		}

		private List<StigmataMetaData> GetAllStigmataData()
		{
			List<StigmataMetaData> itemList = StigmataMetaDataReader.GetItemList();
			List<StigmataMetaData> list = new List<StigmataMetaData>();
			int i = 0;
			for (int count = itemList.Count; i < count; i++)
			{
				StigmataMetaData stigmataMetaData = itemList[i];
				if (!ItempediaData.IsInBlacklist(stigmataMetaData.ID) && stigmataMetaData.subRarity == 0)
				{
					list.Add(stigmataMetaData);
				}
			}
			return list;
		}

		private List<ItemMetaData> GetAllItemData()
		{
			List<ItemMetaData> itemList = ItemMetaDataReader.GetItemList();
			List<ItemMetaData> list = new List<ItemMetaData>();
			int i = 0;
			for (int count = itemList.Count; i < count; i++)
			{
				ItemMetaData itemMetaData = itemList[i];
				if (!ItempediaData.IsInBlacklist(itemMetaData.ID))
				{
					list.Add(itemMetaData);
				}
			}
			return list;
		}
	}
}
